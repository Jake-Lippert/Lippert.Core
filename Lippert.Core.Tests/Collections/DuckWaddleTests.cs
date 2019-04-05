using System;
using System.Linq;
using Lippert.Core.Collections;
using NUnit.Framework;

namespace Lippert.Core.Tests.Collections
{
	[TestFixture]
	public class DuckWaddleTests
	{
		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public void TestComparesInts(bool flip)
		{
			//--Arrange
			var left = new[] { 1, 2, 3, 5, 7, 8, 10, 14 };
			var right = new[] { 2, 3, 5, 6, 7, 9, 11, 13, 14, 17, 25, 34 };

			//--Act
			var (leftOnly, both, rightOnly) = DuckWaddle.Compare(flip ? right : left, flip ? left : right);

			//--Assert
			CollectionAssert.AreEquivalent(new[] { 1, 8, 10 }, flip ? rightOnly : leftOnly);
			CollectionAssert.AreEquivalent(new[] { 2, 3, 5, 7, 14 }, both.Select(x => x.Left));
			CollectionAssert.AreEquivalent(new[] { 2, 3, 5, 7, 14 }, both.Select(x => x.Right));
			CollectionAssert.AreEquivalent(new[] { 6, 9, 11, 13, 17, 25, 34 }, flip ? leftOnly : rightOnly);
		}

		[Test]
		public void TestSortsAndComparesInts()
		{
			//--Arrange
			var random = new Random();
			var left = new[] { 1, 2, 3, 5, 7, 8, 10, 14 }.OrderBy(x => random.Next()).ToList();
			var right = new[] { 2, 3, 5, 6, 7, 9, 11, 13, 14, 17, 25, 34 }.OrderBy(x => random.Next()).ToList();

			//--Act
			var (leftOnly, both, rightOnly) = DuckWaddle.SortAndCompare(left, right);

			//--Assert
			CollectionAssert.AreEquivalent(new[] { 1, 8, 10 }, leftOnly);
			CollectionAssert.AreEquivalent(new[] { 2, 3, 5, 7, 14 }, both.Select(x => x.Left));
			CollectionAssert.AreEquivalent(new[] { 2, 3, 5, 7, 14 }, both.Select(x => x.Right));
			CollectionAssert.AreEquivalent(new[] { 6, 9, 11, 13, 17, 25, 34 }, rightOnly);
		}

		[Test]
		public void TestSortsAndComparesStrings()
		{
			//--Arrange
			var random = new Random();
			var left = new[] { 1, 2, 3, 5, 7, 8, 10, 14 }.Select(x => x.ToString()).OrderBy(x => random.Next()).ToList();
			var right = new[] { 2, 3, 5, 6, 7, 9, 11, 13, 14, 17, 25, 34 }.Select(x => x.ToString()).OrderBy(x => random.Next()).ToList();

			//--Act
			var (leftOnly, both, rightOnly) = DuckWaddle.SortAndCompare(left, right, x => long.Parse(x));

			//--Assert
			CollectionAssert.AreEquivalent(new[] { 1, 8, 10 }.Select(x => x.ToString()), leftOnly);
			CollectionAssert.AreEquivalent(new[] { 2, 3, 5, 7, 14 }.Select(x => x.ToString()), both.Select(x => x.Left));
			CollectionAssert.AreEquivalent(new[] { 2, 3, 5, 7, 14 }.Select(x => x.ToString()), both.Select(x => x.Right));
			CollectionAssert.AreEquivalent(new[] { 6, 9, 11, 13, 17, 25, 34 }.Select(x => x.ToString()), rightOnly);
		}
	}
}