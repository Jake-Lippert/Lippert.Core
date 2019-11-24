using System;
using System.Collections.Generic;
using System.Linq;
using Lippert.Core.Collections;
using Moq;
using NUnit.Framework;

namespace Lippert.Core.Tests.Collections
{
	[TestFixture]
	public class CompositeDisposableTests
	{
		[Test]
		public void TestDisposesAllChildrenOnDispose()
		{
			//--Arrange
			var count = new Random().Next(10, 100);
			var disposableMocks = Enumerable.Range(0, count).Select(x => new Mock<IDisposableThing>()).ToList();
			for (var i = 0; i < count; i++)
			{
				disposableMocks[i].Setup(x => x.DoThing())
					.Returns(i);
			}

			//--Act
			var results = new List<int>();
			using (var composite = new CompositeDisposable<IDisposableThing>(disposableMocks.Select(x => x.Object)))
			{
				foreach (var part in (System.Collections.IEnumerable)composite)
				{
					results.Add(((IDisposableThing)part).DoThing());
				}
				foreach (var disposable in composite)
				{
					results.Add(disposable.DoThing());
				}
			}

			//--Assert
			disposableMocks.ForEach(dm =>
			{
				dm.Verify(d => d.DoThing(), Times.Exactly(2));
				dm.Verify(d => d.Dispose(), Times.Once());
			});
			for (var i = 0; i < count; i++)
			{
				Assert.AreEqual(i, results[i]);
			}
		}
	}

	public interface IDisposableThing : IDisposable
	{
		int DoThing();
	}
}