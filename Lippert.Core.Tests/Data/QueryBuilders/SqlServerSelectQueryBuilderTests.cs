using System;
using Lippert.Core.Configuration;
using Lippert.Core.Data.QueryBuilders;
using Lippert.Core.Tests.TestSchema;
using NUnit.Framework;

namespace Lippert.Core.Tests.Data.QueryBuilders
{
	[TestFixture]
	public class SqlServerSelectQueryBuilderTests
	{
		[OneTimeSetUp]
		public void OneTimeSetUp() => ReflectingRegistrationSource.CodebaseNamespacePrefix = nameof(Lippert);

		private string[] SplitQuery(string query) => query.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

		[Test]
		public void TestBuildsSelectByKeyQuerySingle()
		{
			//--Act
			var query = new SqlServerSelectQueryBuilder().SelectByKey<Client>();

			//--Assert
			Console.WriteLine(query);
			var queryLines = SplitQuery(query);
			Assert.AreEqual(3, queryLines.Length);
			Assert.AreEqual("select [Id], [CreatedByUserId], [CreatedDateUtc], [ModifiedByUserId], [ModifiedDateUtc], [Name], [IsActive]", queryLines[0]);
			Assert.AreEqual("from [Client]", queryLines[1]);
			Assert.AreEqual("where [Id] = @Id", queryLines[2]);
		}

		[Test]
		public void TestBuildsSelectByKeyQueryMultiple()
		{
			//--Act
			var query = new SqlServerSelectQueryBuilder().SelectByKey<ClientUser>();

			//--Assert
			Console.WriteLine(query);
			var queryLines = SplitQuery(query);
			Assert.AreEqual(3, queryLines.Length);
			Assert.AreEqual("select [ClientId], [UserId], [IsActive]", queryLines[0]);
			Assert.AreEqual("from [Client_User]", queryLines[1]);
			Assert.AreEqual("where [ClientId] = @ClientId and [UserId] = @UserId", queryLines[2]);
		}

		[Test]
		public void TestBuildsSelectAllQuery()
		{
			//--Act
			var query = new SqlServerSelectQueryBuilder().SelectAll<Client>();

			//--Assert
			Console.WriteLine(query);
			var queryLines = SplitQuery(query);
			Assert.AreEqual(2, queryLines.Length);
			Assert.AreEqual("select [Id], [CreatedByUserId], [CreatedDateUtc], [ModifiedByUserId], [ModifiedDateUtc], [Name], [IsActive]", queryLines[0]);
			Assert.AreEqual("from [Client]", queryLines[1]);
		}

		[Test]
		public void TestBuildsSelectQueryWithBuilder()
		{
			//--Act
			var query = new SqlServerSelectQueryBuilder().Select(new PredicateBuilder<Client>()
				.Filter(x => x.IsActive));

			//--Assert
			Console.WriteLine(query);
			var queryLines = SplitQuery(query);
			Assert.AreEqual(3, queryLines.Length);
			Assert.AreEqual("select [Id], [CreatedByUserId], [CreatedDateUtc], [ModifiedByUserId], [ModifiedDateUtc], [Name], [IsActive]", queryLines[0]);
			Assert.AreEqual("from [Client]", queryLines[1]);
			Assert.AreEqual("where [IsActive] = @IsActive", queryLines[2]);
		}
	}
}