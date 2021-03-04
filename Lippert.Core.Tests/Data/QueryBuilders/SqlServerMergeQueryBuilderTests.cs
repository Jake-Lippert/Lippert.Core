using System;
using Lippert.Core.Configuration;
using Lippert.Core.Data;
using Lippert.Core.Data.QueryBuilders;
using Lippert.Core.Tests.TestSchema;
using NUnit.Framework;

namespace Lippert.Core.Tests.Data.QueryBuilders
{
	[TestFixture]
	public class SqlServerMergeQueryBuilderTests
	{
		[OneTimeSetUp]
		public void OneTimeSetUp() => ReflectingRegistrationSource.CodebaseNamespacePrefix = nameof(Lippert);

		private string[] SplitQuery(string query) => query.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

		[Test]
		public void TestBuildsMerge([Values(true, false)] bool useJson, [Values(true, false)] bool includeInsert, [Values(true, false)] bool includeUpdate)
		{
			//--Arrange
			var mergeOperations = (includeInsert ? SqlOperation.Insert : 0) | (includeUpdate ? SqlOperation.Update : 0);

			//--Act/Assert
			switch (mergeOperations)
			{
				case SqlOperation.None:
					Assert.Throws<ArgumentException>(() => new SqlServerMergeQueryBuilder().Merge<Client>(out var aliases, mergeOperations, useJson: useJson));
					break;
				default:
					var query = new SqlServerMergeQueryBuilder().Merge<Client>(out var aliases, mergeOperations, useJson: useJson);
					Console.WriteLine(query);
					break;
			}
		}

		[Test]
		public void TestBuildsJsonConverterForUpdateMerge()
		{
			//--Act
			var query = new SqlServerMergeQueryBuilder().Merge<LargeRecord>(out var mergeSerializer, SqlOperation.Update, useJson: true);
			var largeRecords = new[]
			{
				new LargeRecord
				{
					IdA = Guid.NewGuid(),
					IdB = Guid.NewGuid(),
					Property1 = EnumState.ValueB
				}
			};
			var serialized = mergeSerializer.SerializeForMerge(largeRecords);

			//--Assert
			Console.WriteLine(serialized);
			var queryLines = SplitQuery(query);
			Assert.AreEqual(3/*merge using correlationIndex*/ + 2/*key*/ + 2 * 36/*columns to update*/ + 2/*as source on () when matched*/ + 1/*output*/, queryLines.Length);
			Assert.AreEqual(new[]
			{
"merge [LargeRecord] as target",
"using (select * from openJson(@serialized) with (",
"  [<{CorrelationIndex}>] int '$._',",
"  [IdA] uniqueidentifier '$._0',",
"  [IdB] uniqueidentifier '$._1',",
"  [Property1] smallint '$._2',",
"  [Property2] nvarchar(max) '$._3',",
"  [Property3] nvarchar(max) '$._4',",
"  [Property4] nvarchar(max) '$._5',",
"  [Property5] nvarchar(max) '$._6',",
"  [Property6] nvarchar(max) '$._7',",
"  [Property7] nvarchar(max) '$._8',",
"  [Property8] nvarchar(max) '$._9',",
"  [Property9] nvarchar(max) '$._a',",
"  [PropertyA] nvarchar(max) '$._b',",
"  [PropertyB] nvarchar(max) '$._c',",
"  [PropertyC] nvarchar(max) '$._d',",
"  [PropertyD] nvarchar(max) '$._e',",
"  [PropertyE] nvarchar(max) '$._f',",
"  [PropertyF] nvarchar(max) '$._g',",
"  [PropertyG] nvarchar(max) '$._h',",
"  [PropertyH] nvarchar(max) '$._i',",
"  [PropertyI] nvarchar(max) '$._j',",
"  [PropertyJ] nvarchar(max) '$._k',",
"  [PropertyK] nvarchar(max) '$._l',",
"  [PropertyL] nvarchar(max) '$._m',",
"  [PropertyM] nvarchar(max) '$._n',",
"  [PropertyN] nvarchar(max) '$._o',",
"  [PropertyO] nvarchar(max) '$._p',",
"  [PropertyP] nvarchar(max) '$._q',",
"  [PropertyQ] nvarchar(max) '$._r',",
"  [PropertyR] nvarchar(max) '$._s',",
"  [PropertyS] nvarchar(max) '$._t',",
"  [PropertyT] nvarchar(max) '$._u',",
"  [PropertyU] nvarchar(max) '$._v',",
"  [PropertyV] nvarchar(max) '$._w',",
"  [PropertyW] nvarchar(max) '$._x',",
"  [PropertyX] nvarchar(max) '$._y',",
"  [PropertyY] nvarchar(max) '$._z',",
"  [PropertyZ] nvarchar(max) '$._10',",
"  [Property10] nvarchar(max) '$._11'",
")) as source on (target.[IdA] = source.[IdA] and target.[IdB] = source.[IdB])",
"when matched then update set",
"  target.[Property1] = source.[Property1],",
"  target.[Property2] = source.[Property2],",
"  target.[Property3] = source.[Property3],",
"  target.[Property4] = source.[Property4],",
"  target.[Property5] = source.[Property5],",
"  target.[Property6] = source.[Property6],",
"  target.[Property7] = source.[Property7],",
"  target.[Property8] = source.[Property8],",
"  target.[Property9] = source.[Property9],",
"  target.[PropertyA] = source.[PropertyA],",
"  target.[PropertyB] = source.[PropertyB],",
"  target.[PropertyC] = source.[PropertyC],",
"  target.[PropertyD] = source.[PropertyD],",
"  target.[PropertyE] = source.[PropertyE],",
"  target.[PropertyF] = source.[PropertyF],",
"  target.[PropertyG] = source.[PropertyG],",
"  target.[PropertyH] = source.[PropertyH],",
"  target.[PropertyI] = source.[PropertyI],",
"  target.[PropertyJ] = source.[PropertyJ],",
"  target.[PropertyK] = source.[PropertyK],",
"  target.[PropertyL] = source.[PropertyL],",
"  target.[PropertyM] = source.[PropertyM],",
"  target.[PropertyN] = source.[PropertyN],",
"  target.[PropertyO] = source.[PropertyO],",
"  target.[PropertyP] = source.[PropertyP],",
"  target.[PropertyQ] = source.[PropertyQ],",
"  target.[PropertyR] = source.[PropertyR],",
"  target.[PropertyS] = source.[PropertyS],",
"  target.[PropertyT] = source.[PropertyT],",
"  target.[PropertyU] = source.[PropertyU],",
"  target.[PropertyV] = source.[PropertyV],",
"  target.[PropertyW] = source.[PropertyW],",
"  target.[PropertyX] = source.[PropertyX],",
"  target.[PropertyY] = source.[PropertyY],",
"  target.[PropertyZ] = source.[PropertyZ],",
"  target.[Property10] = source.[Property10]",
"output source.[<{CorrelationIndex}>] as [CorrelationIndex], $action as [Action];"
			}, queryLines);
			Assert.AreEqual($"[{{\"_\":0,\"_0\":\"{largeRecords[0].IdA}\",\"_1\":\"{largeRecords[0].IdB}\",\"_2\":2,\"_3\":null,\"_4\":null,\"_5\":null,\"_6\":null,\"_7\":null,\"_8\":null,\"_9\":null,\"_a\":null,\"_b\":null,\"_c\":null,\"_d\":null,\"_e\":null,\"_f\":null,\"_g\":null,\"_h\":null,\"_i\":null,\"_j\":null,\"_k\":null,\"_l\":null,\"_m\":null,\"_n\":null,\"_o\":null,\"_p\":null,\"_q\":null,\"_r\":null,\"_s\":null,\"_t\":null,\"_u\":null,\"_v\":null,\"_w\":null,\"_x\":null,\"_y\":null,\"_z\":null,\"_10\":null,\"_11\":null}}]", serialized);
		}
	}
}