using System;
using Lippert.Core.Data.QueryBuilders.MergeSerializers;
using Lippert.Core.Tests.TestSchema;
using Lippert.Core.Tests.TestSchema.TableMaps;
using NUnit.Framework;

namespace Lippert.Core.Tests.Data.QueryBuilders.MergeSerializers
{
	[TestFixture]
	public class JsonMergeSerializerTests
	{
		[Test]
		public void TestMergeSerialization()
		{
			//--Arrange
			var idA = Guid.NewGuid();
			var idB = Guid.NewGuid();
			var records = new[]
			{
				new SuperEmployee
				{
					EmployeeId = idA,
					SomeAwesomeFieldA = "Name A",
					SomeAwesomeFieldB = null
				},
				new SuperEmployee
				{
					EmployeeId = idB,
					SomeAwesomeFieldA = null,
					SomeAwesomeFieldB = "Name B"
				}
			};
			var tableMap = new SuperEmployeeMap();

			//--Act
			var serialized = new JsonMergeSerializer<SuperEmployee>(tableMap).SerializeForMerge(records);

			//--Assert
			Assert.AreEqual($"[{{\"_\":0,\"_0\":\"{idA}\",\"_1\":\"Name A\",\"_2\":null}},{{\"_\":1,\"_0\":\"{idB}\",\"_1\":null,\"_2\":\"Name B\"}}]", serialized);
		}
	}
}