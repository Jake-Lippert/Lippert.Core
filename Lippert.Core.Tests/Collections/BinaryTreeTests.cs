using System;
using System.Linq;
using Lippert.Core.Collections;
using NUnit.Framework;

namespace Lippert.Core.Tests.Collections
{
	[TestFixture]
	public class BinaryTreeTests
	{
		[Test]
		public void TestTreeIteratesInOrder()
		{
			//--Arrange
			var tree = new BinaryTree<int>(8)
			{
				new BinaryTree<int>(4)
				{
					new BinaryTree<int>(2) { 1, 3 },
					new BinaryTree<int>(6) { 5, 7 }
				},
				new BinaryTree<int>(12)
				{
					new BinaryTree<int>(10) { 9, 11 },
					new BinaryTree<int>(14) { 13, 15 }
				}
			};

			//--Act
			var enumerated = tree.ToList();

			//--Assert
			foreach (var (x, i) in enumerated.Select((x, i) => (x, i)))
			{
				Assert.AreEqual(i + 1, x);
			}
		}

		[Test]
		public void TestTreeIteratesInPreOrder()
		{
			//--Arrange
			var tree = new BinaryTree<int>(4)
			{
				new BinaryTree<int>(2) { 1, 3 },
				new BinaryTree<int>(6) { 5, 7 }
			};

			//--Act
			var enumerated = tree.EnumerateTree(TreeTraversalOrder.PreOrder).ToList();

			//--Assert
			foreach (var (expected, actual) in new[] { 4, 2, 1, 3, 6, 5, 7 }.Zip(enumerated, (ex, ac) => (ex, ac)))
			{
				Assert.AreEqual(expected, actual);
			}
		}

		[Test]
		public void TestTreeIteratesInPostOrder()
		{
			//--Arrange
			var tree = new BinaryTree<int>(4)
			{
				new BinaryTree<int>(2) { 1, 3 },
				new BinaryTree<int>(6) { 5, 7 }
			};

			//--Act
			var enumerated = tree.EnumerateTree(TreeTraversalOrder.PostOrder).ToList();

			//--Assert
			foreach (var (expected, actual) in new[] { 1, 3, 2, 5, 7, 6, 4 }.Zip(enumerated, (ex, ac) => (ex, ac)))
			{
				Assert.AreEqual(expected, actual);
			}
		}

		[Test]
		public void TestBuildsTreeFromRootAndEnumeratesInOrder()
		{
			//--Arrange
			var tree = new BinaryTree<int>(50);
			var rand = new Random();
			foreach (var x in Enumerable.Range(0, 2 * tree.Value + 1).Where(x => x != tree.Value).OrderBy(x => rand.Next()))
			{
				tree.Add(x);
			}

			//--Act
			var enumerated = tree.ToList();

			//--Assert
			foreach (var (x, i) in enumerated.Select((x, i) => (x, i)))
			{
				Assert.AreEqual(i, x);
			}
		}
	}
}