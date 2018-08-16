using System;
using System.Linq;
using Lippert.Core.Collections;
using Lippert.Core.Collections.Extensions;
using Moq;
using NUnit.Framework;

namespace Lippert.Core.Tests.Collections
{
	[TestFixture]
	public class RetrievalDictionaryTests
	{
		[Test]
		public void TestRetrievalOnlyHappensOncePerKey()
		{
			//--Arrange
			var ids = Enumerable.Range(0, 10).Select(x => Guid.NewGuid()).ToList();
			var repoMock = new Mock<IExpensiveOperationRepository>();
			repoMock.Setup(x => x.RunQueryThatTakesForever(It.IsAny<Guid>()))
				.Returns((Guid id) => new TestSchema.Employee { Id = id, CompanyId = Guid.NewGuid() });
			var mockObject = repoMock.Object;

			//--Act
			var retrievalDictionary = RetrievalDictionary.Build((Guid id) => mockObject.RunQueryThatTakesForever(id));
			var resultLookup = ids.Join(Enumerable.Range(0, 10), l => true, r => true, (l, r) => l)
				.ToLookup(x => x, x => retrievalDictionary[x]);

			//--Assert
			Assert.AreEqual(ids.Count, resultLookup.Count);
			Assert.AreEqual(ids.Count, resultLookup.SelectMany(x => x).Distinct().Count());
			foreach (var (id, results) in resultLookup.AsTuples())
			{
				Assert.AreEqual(10, results.Count);
				Assert.AreEqual(1, results.Distinct().Count());

				repoMock.Verify(x => x.RunQueryThatTakesForever(id), Times.Exactly(1));
			}
		}

		[Test]
		public void TestRetrievalDoesntHappenWhenPreloaded()
		{
			//--Arrange
			var ids = Enumerable.Range(0, 10).Select(x => Guid.NewGuid()).ToList();
			var repoMock = new Mock<IExpensiveOperationRepository>();
			var mockObject = repoMock.Object;

			//--Act
			var retrievalDictionary = RetrievalDictionary.Build((Guid id) => mockObject.RunQueryThatTakesForever(id));
			ids.ForEach(id => retrievalDictionary.Add(id, new TestSchema.Employee { Id = id, CompanyId = Guid.NewGuid() }));
			var resultLookup = ids.Join(Enumerable.Range(0, 10), l => true, r => true, (l, r) => l)
				.ToLookup(x => x, x => retrievalDictionary[x]);

			//--Assert
			Assert.AreEqual(ids.Count, resultLookup.Count);
			Assert.AreEqual(ids.Count, resultLookup.SelectMany(x => x).Distinct().Count());
			foreach (var (id, results) in resultLookup.AsTuples())
			{
				Assert.AreEqual(10, results.Count);
				Assert.AreEqual(1, results.Distinct().Count());

				repoMock.Verify(x => x.RunQueryThatTakesForever(It.IsAny<Guid>()), Times.Never());
			}
		}

		[Test]
		public void TestRetrievedValueCanBeOverwritten()
		{
			//--Arrange
			var guid = Guid.NewGuid();
			var repoMock = new Mock<IExpensiveOperationRepository>();
			repoMock.Setup(x => x.RunQueryThatTakesForever(It.IsAny<Guid>()))
				.Returns((Guid id) => new TestSchema.Employee { Id = id, CompanyId = Guid.NewGuid() });
			var mockObject = repoMock.Object;

			//--Act
			var retrievalDictionary = RetrievalDictionary.Build((Guid id) => mockObject.RunQueryThatTakesForever(id));
			var results = Enumerable.Range(0, 10).Select(x => retrievalDictionary[guid]).ToList();
			retrievalDictionary[guid] = new TestSchema.Employee { Id = guid, CompanyId = Guid.NewGuid() };
			results.AddRange(Enumerable.Range(0, 10).Select(x => retrievalDictionary[guid]));

			//--Assert
			Assert.AreEqual(2, results.GroupBy(x => x.CompanyId).Count());
			Assert.AreEqual(1, results.Take(10).GroupBy(x => x.CompanyId).Count());
			Assert.AreEqual(1, results.Skip(10).GroupBy(x => x.CompanyId).Count());
		}


		public interface IExpensiveOperationRepository
		{
			TestSchema.Employee RunQueryThatTakesForever(Guid id);
		}
	}
}