using System;
using Lippert.Core.Collections.Extensions;
using NUnit.Framework;

namespace Lippert.Core.Tests.Collections.Extensions
{
	[TestFixture]
    public class IEnumerableExtensionsTests
	{
		[Test]
		public void TestDataTableConversionWithByteEnum()
		{
			//--Arrange
			var data = new[]
			{
				new { ColumnValue = ByteEnum.Zero },
				new { ColumnValue = ByteEnum.One },
				new { ColumnValue = ByteEnum.Two }
			};

			//--Act
			var result = data.ToDataTable();

			//--Assert
			Assert.AreEqual("ColumnValue", result.Columns[0].ColumnName);
			Assert.AreEqual(typeof(byte), result.Columns[0].DataType);
			foreach (var (x, i) in data.Indexed())
			{
				Assert.AreEqual(typeof(byte), result.Rows[i][0].GetType());
				Assert.AreEqual((byte)x.ColumnValue, result.Rows[i][0]);
			}
		}
		[Test]
		public void TestDataTableConversionWithShortEnum()
		{
			//--Arrange
			var data = new[]
			{
				new { ColumnValue = ShortEnum.Zero },
				new { ColumnValue = ShortEnum.One },
				new { ColumnValue = ShortEnum.Two }
			};

			//--Act
			var result = data.ToDataTable();

			//--Assert
			Assert.AreEqual("ColumnValue", result.Columns[0].ColumnName);
			Assert.AreEqual(typeof(short), result.Columns[0].DataType);
			foreach (var (x, i) in data.Indexed())
			{
				Assert.AreEqual(typeof(short), result.Rows[i][0].GetType());
				Assert.AreEqual((short)x.ColumnValue, result.Rows[i][0]);
			}
		}
		[Test]
		public void TestDataTableConversionWithIntEnum()
		{
			//--Arrange
			var data = new[]
			{
				new { ColumnValue = IntEnum.Zero },
				new { ColumnValue = IntEnum.One },
				new { ColumnValue = IntEnum.Two }
			};

			//--Act
			var result = data.ToDataTable();

			//--Assert
			Assert.AreEqual("ColumnValue", result.Columns[0].ColumnName);
			Assert.AreEqual(typeof(int), result.Columns[0].DataType);
			foreach (var (x, i) in data.Indexed())
			{
				Assert.AreEqual(typeof(int), result.Rows[i][0].GetType());
				Assert.AreEqual((int)x.ColumnValue, result.Rows[i][0]);
			}
		}
		[Test]
		public void TestDataTableConversionWithLongEnum()
		{
			//--Arrange
			var data = new[]
			{
				new { ColumnValue = LongEnum.Zero },
				new { ColumnValue = LongEnum.One },
				new { ColumnValue = LongEnum.Two }
			};

			//--Act
			var result = data.ToDataTable();

			//--Assert
			Assert.AreEqual("ColumnValue", result.Columns[0].ColumnName);
			Assert.AreEqual(typeof(long), result.Columns[0].DataType);
			foreach (var (x, i) in data.Indexed())
			{
				Assert.AreEqual(typeof(long), result.Rows[i][0].GetType());
				Assert.AreEqual((long)x.ColumnValue, result.Rows[i][0]);
			}
		}

		[Test]
		public void TestDataTableConversionWithNullableByteEnum()
		{
			//--Arrange
			var data = new[]
			{
				new { ColumnValue = default(ByteEnum?) }
			};

			//--Act
			var result = data.ToDataTable();

			//--Assert
			Assert.AreEqual("ColumnValue", result.Columns[0].ColumnName);
			Assert.AreEqual(typeof(byte), result.Columns[0].DataType);
			foreach (var (x, i) in data.Indexed())
			{
				Assert.AreEqual(DBNull.Value, result.Rows[i][0]);
			}
		}
		[Test]
		public void TestDataTableConversionWithNullableShortEnum()
		{
			//--Arrange
			var data = new[]
			{
				new { ColumnValue = default(ShortEnum?) }
			};

			//--Act
			var result = data.ToDataTable();

			//--Assert
			Assert.AreEqual("ColumnValue", result.Columns[0].ColumnName);
			Assert.AreEqual(typeof(short), result.Columns[0].DataType);
			foreach (var (x, i) in data.Indexed())
			{
				Assert.AreEqual(DBNull.Value, result.Rows[i][0]);
			}
		}
		[Test]
		public void TestDataTableConversionWithNullableIntEnum()
		{
			//--Arrange
			var data = new[]
			{
				new { ColumnValue = default(IntEnum?) }
			};

			//--Act
			var result = data.ToDataTable();

			//--Assert
			Assert.AreEqual("ColumnValue", result.Columns[0].ColumnName);
			Assert.AreEqual(typeof(int), result.Columns[0].DataType);
			foreach (var (x, i) in data.Indexed())
			{
				Assert.AreEqual(DBNull.Value, result.Rows[i][0]);
			}
		}
		[Test]
		public void TestDataTableConversionWithNullableLongEnum()
		{
			//--Arrange
			var data = new[]
			{
				new { ColumnValue = default(LongEnum?) }
			};

			//--Act
			var result = data.ToDataTable();

			//--Assert
			Assert.AreEqual("ColumnValue", result.Columns[0].ColumnName);
			Assert.AreEqual(typeof(long), result.Columns[0].DataType);
			foreach (var (x, i) in data.Indexed())
			{
				Assert.AreEqual(DBNull.Value, result.Rows[i][0]);
			}
		}

		public enum ByteEnum : byte
		{
			Zero = 0,
			One = 1,
			Two = 2
		}
		public enum ShortEnum : short
		{
			Zero = 0,
			One = 1,
			Two = 2
		}
		public enum IntEnum : int
		{
			Zero = 0,
			One = 1,
			Two = 2
		}
		public enum LongEnum : long
		{
			Zero = 0,
			One = 1,
			Two = 2
		}
	}
}