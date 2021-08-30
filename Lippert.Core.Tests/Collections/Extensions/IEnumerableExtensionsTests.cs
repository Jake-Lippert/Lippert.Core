﻿using System;
using System.Collections.Generic;
using System.Linq;
using Lippert.Core.Collections.Extensions;
using Moq;
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


		#region ProcessBatch
		[Test]
		public void TestProcessBatch([Values(-1, 0, 5, 10, 25, 75, 100)] int batchSize, [Values(0, 4, 10, 30, 50, 76, 105, 501)] int totalItems)
		{
			//--Arrange
			var iteratedItemCount = 0;
			var items = Enumerable.Range(0, totalItems)
				.Select(x =>
				{
					iteratedItemCount++;
					return x;
				});

			//--Mock
			var logicMock = new Mock<IBatchedLogic>();
			logicMock.Setup(x => x.ProcessItems(It.IsAny<List<int>>()))
				.Returns((List<int> batchItems) => (Guid.NewGuid(), batchItems));

			//--Act/Assert
			var batches = new List<(Guid id, List<int> items)>();
			Assert.Zero(iteratedItemCount);
			try
			{
				foreach (var batch in items.ProcessBatch(batchSize, batch => logicMock.Object.ProcessItems(batch)))
				{
					batches.Add(batch);

					Assert.AreEqual(batches.Sum(x => x.items.Count), iteratedItemCount);
					logicMock.Verify(x => x.ProcessItems(It.IsAny<List<int>>()), Times.Exactly(batches.Count));
				}

				Assert.AreEqual(totalItems, iteratedItemCount);
				Assert.AreEqual(totalItems / batchSize + (totalItems % batchSize > 0 ? 1 : 0), batches.Count);
				Assert.AreEqual(totalItems, batches.Sum(x => x.items.Count));
			}
			catch (ArgumentException ae) when (batchSize <= 0)
			{
				Assert.AreEqual("'batchSize' must be greater than 0", ae.Message);
			}
		}
		public interface IBatchedLogic
		{
			(Guid id, List<int> items) ProcessItems(List<int> items);
		}
		#endregion


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
			foreach (var (_, i) in data.Indexed())
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
			foreach (var (_, i) in data.Indexed())
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
			foreach (var (_, i) in data.Indexed())
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
			foreach (var (_, i) in data.Indexed())
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
		

		[Test]
		[TestCase(86)]
		[TestCase(101)]
		[TestCase(980)]
		public void TestBuildsBinaryTreeAndEnumeratesInOrder(int size)
		{
			//--Arrange
			var rand = new Random();

			//--Act
			var tree = Enumerable.Range(0, size).OrderBy(x => rand.Next()).ToBinaryTree();
			var enumerated = tree.ToList();

			//--Assert
			foreach (var (x, i) in enumerated.Select((x, i) => (x, i)))
			{
				Assert.AreEqual(i, x);
			}
		}

		[Test]
		public void TestBuildsNTreeAndEnumerates()
		{
			//--Arrange
			var expectedDirectories = new string?[]
			{
				"Dir A",
				"Dir A/Dir A1",
				"Dir A/Dir A1/File 1",
				"Dir A/Dir A1/File 2",
				"Dir A/Dir A1/File 3",
				"Dir A/Dir A2",
				"Dir A/Dir A2/File 0",
				"Dir B",
				"Dir B/Dir B1",
				"Dir B/Dir B1/File 1",
				"Dir B/Dir B1/File 2",
				"Dir C",
				"Dir C/Dir C1",
				"Dir C/Dir C1/File 4",
				"Dir C/Dir C2",
				"Dir C/Dir C2/File 5"
			};

			//--Act
			var tree = expectedDirectories.ToNTree(x => x, x => x!.LastIndexOf('/') < 0 ? null : x.Substring(0, x.LastIndexOf('/')), null);

			//--Assert
			Assert.AreEqual(expectedDirectories.Length + 1, tree.Count());
			Assert.AreEqual(null, tree.Value);
			Assert.AreEqual("Dir A", tree.Children[0].Value);
			Assert.AreEqual("Dir A/Dir A1", tree.Children[0].Children[0].Value);
			Assert.AreEqual("Dir A/Dir A1/File 1", tree.Children[0].Children[0].Children[0].Value);
			Assert.AreEqual("Dir A/Dir A1/File 2", tree.Children[0].Children[0].Children[1].Value);
			Assert.AreEqual("Dir A/Dir A1/File 3", tree.Children[0].Children[0].Children[2].Value);
			Assert.AreEqual("Dir A/Dir A2", tree.Children[0].Children[1].Value);
			Assert.AreEqual("Dir A/Dir A2/File 0", tree.Children[0].Children[1].Children[0].Value);
			Assert.AreEqual("Dir B", tree.Children[1].Value);
			Assert.AreEqual("Dir B/Dir B1", tree.Children[1].Children[0].Value);
			Assert.AreEqual("Dir B/Dir B1/File 1", tree.Children[1].Children[0].Children[0].Value);
			Assert.AreEqual("Dir B/Dir B1/File 2", tree.Children[1].Children[0].Children[1].Value);
			Assert.AreEqual("Dir C", tree.Children[2].Value);
			Assert.AreEqual("Dir C/Dir C1", tree.Children[2].Children[0].Value);
			Assert.AreEqual("Dir C/Dir C1/File 4", tree.Children[2].Children[0].Children[0].Value);
			Assert.AreEqual("Dir C/Dir C2", tree.Children[2].Children[1].Value);
			Assert.AreEqual("Dir C/Dir C2/File 5", tree.Children[2].Children[1].Children[0].Value);

			Assert.IsFalse(tree.TryGetNode("Dir D", out var _));
			Assert.Throws<KeyNotFoundException>(() => _ = tree["Dir D"]);
			foreach (var directory in expectedDirectories)
			{
				Assert.IsTrue(tree.TryGetNode(directory, out var node));
				Assert.AreEqual(directory, node?.Key);
				Assert.AreEqual(directory, tree[directory].Value);
			}
		}

		[Test]
		public void TestBuildsNTreeWithSingleRootAndEnumerates()
		{
			//--Arrange
			var expectedDirectories = new string?[]
			{
				"Dir A",
				"Dir A/Dir A1",
				"Dir A/Dir A1/File 1",
				"Dir A/Dir A1/File 2",
				"Dir A/Dir A1/File 3",
				"Dir A/Dir A2",
				"Dir A/Dir A2/File 0"
			};

			//--Act
			var tree = expectedDirectories.ToNTree(x => x, x => x!.LastIndexOf('/') < 0 ? null : x.Substring(0, x.LastIndexOf('/')), null);

			//--Assert
			Assert.AreEqual(expectedDirectories.Length + 1, tree.Count());
			Assert.AreEqual(null, tree.Value);
			Assert.AreEqual("Dir A", tree.Children[0].Value);
			Assert.AreEqual("Dir A/Dir A1", tree.Children[0].Children[0].Value);
			Assert.AreEqual("Dir A/Dir A1/File 1", tree.Children[0].Children[0].Children[0].Value);
			Assert.AreEqual("Dir A/Dir A1/File 2", tree.Children[0].Children[0].Children[1].Value);
			Assert.AreEqual("Dir A/Dir A1/File 3", tree.Children[0].Children[0].Children[2].Value);
			Assert.AreEqual("Dir A/Dir A2", tree.Children[0].Children[1].Value);
			Assert.AreEqual("Dir A/Dir A2/File 0", tree.Children[0].Children[1].Children[0].Value);

			Assert.IsFalse(tree.TryGetNode("Dir D", out var _));
			Assert.Throws<KeyNotFoundException>(() => _ = tree["Dir D"]);
			foreach (var directory in expectedDirectories)
			{
				Assert.IsTrue(tree.TryGetNode(directory, out var node));
				Assert.AreEqual(directory, node?.Key);
				Assert.AreEqual(directory, tree[directory].Value);
			}
		}

		[Test]
		public void TestBuildsNTreeWithSingleRootMatchingDefaultAndEnumerates()
		{
			//--Arrange
			var expectedDirectories = new string[]
			{
				"Dir A/Dir A1",
				"Dir A/Dir A1/File 1",
				"Dir A/Dir A1/File 2",
				"Dir A/Dir A1/File 3",
				"Dir A/Dir A2",
				"Dir A/Dir A2/File 0"
			};

			//--Act
			var tree = expectedDirectories.OrderBy(x => Guid.NewGuid()).ToNTree(x => x, x => x!.LastIndexOf('/') < 0 ? null : x.Substring(0, x.LastIndexOf('/')), "Dir A");

			//--Assert
			Assert.AreEqual(expectedDirectories.Length + 1, tree.Count());
			Assert.AreEqual("Dir A", tree.Value);
			Assert.IsTrue(tree.TryGetNode("Dir A/Dir A1", out var dirA));
			Assert.IsTrue(dirA!.TryGetNode("Dir A/Dir A1/File 1", out _));
			Assert.IsTrue(dirA.TryGetNode("Dir A/Dir A1/File 2", out _));
			Assert.IsTrue(dirA.TryGetNode("Dir A/Dir A1/File 3", out _));
			Assert.IsTrue(tree.TryGetNode("Dir A/Dir A2", out var dirB));
			Assert.IsTrue(dirB!.TryGetNode("Dir A/Dir A2/File 0", out _));
		}
	}
}