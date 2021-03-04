using Lippert.Core.Data.QueryBuilders;
using Lippert.Core.Tests.TestSchema;
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
	}
}