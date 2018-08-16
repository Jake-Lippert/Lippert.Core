using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lippert.Core.Collections.Extensions;
using NUnit.Framework;

namespace Lippert.Core.Tests.Collections.Extensions
{
	[TestFixture]
	public class IDictionaryExtensionsTests
	{
		[Test]
		public void TestAddsItemsToDictionary()
		{
			//--Arrange
			var existing = Enumerable.Range(0, 5).ToDictionary(x => x, x => x.ToString());
			var toAdd = Enumerable.Range(5, 5).ToDictionary(x => x, x => x.ToString());

			//--Act
			existing.AddRange(toAdd);

			//--Assert
			foreach (var i in Enumerable.Range(0, 10))
			{
				Assert.AreEqual(i.ToString(), existing[i]);
			}
		}

		[Test]
		public void TestDuplicateKeysCantBeAddedToDictionary()
		{
			//--Arrange
			var existing = Enumerable.Range(0, 5).ToDictionary(x => x, x => x.ToString());
			var toAdd = Enumerable.Range(4, 5).ToDictionary(x => x, x => x.ToString());

			//--Act/Assert
			Assert.Throws<ArgumentException>(() => existing.AddRange(toAdd));
		}

		[Test]
		public void TestAddsCollectionsToDictionaryByKey()
		{
			//--Arrange
			var existing = Enumerable.Range(0, 5).ToDictionary(x => x.ToString(), x => x);

			//--Act
			existing.AddRange(Enumerable.Range(5, 5), x => x.ToString());

			//--Assert
			foreach (var i in Enumerable.Range(0, 10))
			{
				Assert.AreEqual(i, existing[i.ToString()]);
			}
		}

		[Test]
		public void TestDuplicateItemsCantBeAddedToDictionaryByKey()
		{
			//--Arrange
			var existing = Enumerable.Range(0, 5).ToDictionary(x => x.ToString(), x => x);

			//--Act/Assert
			Assert.Throws<ArgumentException>(() => existing.AddRange(Enumerable.Range(4, 5), x => x.ToString()));
		}
	}
}