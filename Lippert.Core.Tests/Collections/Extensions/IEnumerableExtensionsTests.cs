using System;
using System.Linq;
using Lippert.Core.Collections.Extensions;
using NUnit.Framework;

namespace Lippert.Core.Tests.Collections.Extensions
{
	[TestFixture]
    public class IEnumerableExtensionsTests
	{
		[Test]
		public void TestLeftJoin()
		{
			//--Arrange
			var left = Enumerable.Range(0, 10).Select(x => (x, nameL: x.ToString())).ToList();
			var right = Enumerable.Range(5, 10).Select(x => (x, nameR: x.ToString())).ToList();

			//--Act
			var joined = left.LeftJoin(right, l => l.x, r => r.x, (l, r) => (l.x, l.nameL, r.nameR))
				.ToDictionary(lr => lr.x, lr => (lr.nameL, lr.nameR));

			//--Assert
			Assert.AreEqual(10, joined.Count);
			foreach (var key in Enumerable.Range(0, 10))
			{
				Assert.IsTrue(joined.TryGetValue(key, out var lr));
				Assert.AreEqual(key.ToString(), lr.nameL);
				Assert.AreEqual(key < 5 ? null : key.ToString(), lr.nameR);
			}
		}

		[Test]
		public void TestRightJoin()
		{
			//--Arrange
			var left = Enumerable.Range(5, 10).Select(x => (x, nameL: x.ToString())).ToList();
			var right = Enumerable.Range(0, 10).Select(x => (x, nameR: x.ToString())).ToList();

			//--Act
			var joined = left.RightJoin(right, l => l.x, r => r.x, (l, r) => (r.x, l.nameL, r.nameR))
				.ToDictionary(lr => lr.x, lr => (lr.nameL, lr.nameR));

			//--Assert
			Assert.AreEqual(10, joined.Count);
			foreach (var key in Enumerable.Range(0, 10))
			{
				Assert.IsTrue(joined.TryGetValue(key, out var lr));
				Assert.AreEqual(key.ToString(), lr.nameR);
				Assert.AreEqual(key < 5 ? null : key.ToString(), lr.nameL);
			}
		}


		[Test]
		public void TestCustomOrdering()
		{
			//--Arrange
			var random = new Random();
			var scrambled = Enumerable.Range(0, 52).Select(x => 'A' + 32 * (x / 26) + x % 26).Join(
				Enumerable.Range(0, 52).Select(x => 'A' + 32 * (x / 26) + x % 26), l => true, r => true,
				(l, r) => $"{(char)l}_{(char)r}")
				.OrderBy(x => random.Next())
				.ToList();

			//--Act
			var sorted = scrambled.OrderBy(StringComparer.InvariantCultureIgnoreCase).ToList();

			//--Assert
			for (var i = 1; i < scrambled.Count; i++)
			{
				Assert.LessOrEqual(StringComparer.InvariantCultureIgnoreCase.Compare(sorted[i - 1], sorted[i]), 0);
			}
		}

		[Test]
		public void TestCustomOrderingDescending()
		{
			//--Arrange
			var random = new Random();
			var scrambled = Enumerable.Range(0, 52).Select(x => 'A' + 32 * (x / 26) + x % 26).Join(
				Enumerable.Range(0, 52).Select(x => 'A' + 32 * (x / 26) + x % 26), l => true, r => true,
				(l, r) => $"{(char)l}_{(char)r}")
				.OrderBy(x => random.Next())
				.ToList();

			//--Act
			var sorted = scrambled.OrderByDescending(StringComparer.InvariantCultureIgnoreCase).ToList();

			//--Assert
			for (var i = 1; i < scrambled.Count; i++)
			{
				Assert.GreaterOrEqual(StringComparer.InvariantCultureIgnoreCase.Compare(sorted[i - 1], sorted[i]), 0);
			}
		}


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