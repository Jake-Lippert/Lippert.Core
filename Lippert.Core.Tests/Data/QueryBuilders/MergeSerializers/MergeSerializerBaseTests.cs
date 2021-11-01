using Lippert.Core.Data.QueryBuilders;
using Lippert.Core.Data.QueryBuilders.MergeSerializers;
using Lippert.Core.Tests.TestSchema;
using Lippert.Core.Tests.TestSchema.TableMaps;
using NUnit.Framework;

namespace Lippert.Core.Tests.Data.QueryBuilders.MergeSerializers
{
	[TestFixture]
	public class MergeSerializerBaseTests
	{
		[Test]
		public void TestEnumConvertedToUnderlyingType()
		{
			//--Arrange
			var record = new LargeRecord
			{
				Property1 = EnumState.ValueC
			};

			//--Act
			var result = MergeSerializerBase<LargeRecord>.GetPropertyValue(record, typeof(LargeRecord).GetProperty(nameof(LargeRecord.Property1)));

			//--Assert
			Assert.AreEqual((short)4, result);
		}

		[Test]
		public void TestBuildsStringColumnParser()
		{
			//--Arrange
			var tableMap = new SuperEmployeeMap();
			var mergeSerializer = new JsonMergeSerializer<SuperEmployee>(tableMap);

			//--Act
			var parser = mergeSerializer.BuildColumnParser(tableMap[x => x.SomeAwesomeFieldA]);

			//--Assert
			StringAssert.StartsWith("[SomeAwesomeFieldA] nvarchar(max) '$._", parser);
		}

		[Test]
		public void TestBuildsStringColumnParserWithConfiguredLength()
		{
			//--Arrange
			var tableMap = new PurchaseMap();
			var mergeSerializer = new JsonMergeSerializer<Purchase>(tableMap);

			//--Act
			var parser = mergeSerializer.BuildColumnParser(tableMap[x => x.Name]);

			//--Assert
			StringAssert.StartsWith("[Name] nvarchar(20) '$._", parser);
		}

		[Test]
		public void TestBuildsBinaryColumnParserWithConfiguredLength()
		{
			//--Arrange
			var tableMap = new StoredFileMap();
			var mergeSerializer = new JsonMergeSerializer<StoredFile>(tableMap);

			//--Act
			var parser = mergeSerializer.BuildColumnParser(tableMap[x => x.FileBytes]);

			//--Assert
			StringAssert.StartsWith("[FileBytes] nvarchar(max) '$._", parser);
		}

		[Test]
		public void TestBuildsDecimalColumnParserWithConfiguredPrecisionAndScale()
		{
			//--Arrange
			var tableMap = new PurchaseMap();
			var mergeSerializer = new JsonMergeSerializer<Purchase>(tableMap);

			//--Act
			var parser = mergeSerializer.BuildColumnParser(tableMap[x => x.Cost]);

			//--Assert
			StringAssert.StartsWith("[Cost] decimal(10,2) '$._", parser);
		}
	}
}