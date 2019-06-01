using System;
using System.Collections.Generic;
using System.Linq;
using Lippert.Core.Collections;
using Lippert.Core.Collections.Extensions;
using NUnit.Framework;

namespace Lippert.Core.Tests.Collections
{
	[TestFixture]
	public class MutableLookupTests
	{
		private readonly Random _random = new Random();

		private List<(int? key, Guid element)> BuildSourceCollection(bool includeNull) => Enumerable.Range(1, 1000)
			.Select(x => _random.Next(includeNull ? -1 : 0, 100))
			.Select(x => (key: x < 0 ? default(int?) : x, element: Guid.NewGuid()))
			.ToList();

		[Test]
		public void TestCountEquivalence([Values(false, true)] bool includeNull)
		{
			//--Arrange
			var collection = BuildSourceCollection(includeNull);
			var expected = collection.ToLookup(x => x.key, x => x.element);

			//--Act
			var actual = collection.ToMutableLookup(x => x.key, x => x.element);

			//--Assert
			Assert.AreEqual(expected.Count, actual.Count);//--Testing built-in property
			Assert.AreEqual(expected.Count, actual.Count());//--Testing via typed enumeration
			var count = 0;
			foreach (var element in (System.Collections.IEnumerable)actual)
			{
				count++;
			}
			Assert.AreEqual(expected.Count, count);//--Testing via typeless enumeration
		}

		[Test]
		public void TestOverallEquivalence([Values(false, true)] bool includeNull)
		{
			//--Arrange
			var collection = BuildSourceCollection(includeNull);
			var expected = collection.ToLookup(x => x.key, x => x.element);

			//--Act/Assert
			var actual = new MutableLookup<long?, Guid>();
			foreach (var grouping in expected)
			{
				CollectionAssert.IsEmpty(actual[grouping.Key]);
				actual[grouping.Key] = grouping;
			}

			//--Assert
			Assert.IsFalse(actual.Contains(-1));
			CollectionAssert.AreEquivalent(expected.Select(x => x.Key), actual.Select(x => x.Key));
			foreach (var expectedGrouping in expected)
			{
				Assert.IsTrue(actual.Contains(expectedGrouping.Key));
				CollectionAssert.AreEquivalent(expectedGrouping, actual[expectedGrouping.Key]);
			}

			foreach (var (expectedGrouping, actualGrouping) in expected.Zip(actual, (e, a) => (e, a)))
			{
				Assert.AreEqual(expectedGrouping.Key, actualGrouping.Key);
				CollectionAssert.AreEquivalent(expectedGrouping, actualGrouping);
			}
		}

		[Test]
		public void TestCollectionInitializer()
		{
			//--Act
			var actual = new MutableLookup<long?, Guid>
			{
				{ 0, Guid.NewGuid() },
				{ 0, Guid.NewGuid() },
				{ 1, Guid.NewGuid() },
				{ 1, Guid.NewGuid() },
				{ 1, Guid.NewGuid() },
				{ 2, Guid.NewGuid() },
				{ 2, Guid.NewGuid() },
				{ 2, Guid.NewGuid() },
				{ 2, Guid.NewGuid() }
			};

			//--Assert
			Assert.AreEqual(0, actual[-1].Count());
			Assert.AreEqual(2, actual[0].Count());
			Assert.AreEqual(3, actual[1].Count());
			Assert.AreEqual(4, actual[2].Count());
		}
	}
}