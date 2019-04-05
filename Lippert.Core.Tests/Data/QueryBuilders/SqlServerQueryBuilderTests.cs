using System;
using Lippert.Core.Configuration;
using Lippert.Core.Data.QueryBuilders;
using Lippert.Core.Tests.TestSchema;
using NUnit.Framework;

namespace Lippert.Core.Tests.Data.QueryBuilders
{
	[TestFixture]
	public class SqlServerQueryBuilderTests
	{
		[OneTimeSetUp]
		public void OneTimeSetUp() => ReflectingRegistrationSource.CodebaseNamespacePrefix = "Lippert";

		private string[] SplitQuery(string query) =>
#if TARGET_FRAMEWORK_NET471
			query.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
#else
			query.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
#endif

		[Test]
		public void TestBuildsSelectByKeyQuerySingle()
		{
			//--Act
			var query = new SqlServerQueryBuilder().SelectByKey<Client>();

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
			var query = new SqlServerQueryBuilder().SelectByKey<ClientUser>();

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
			var query = new SqlServerQueryBuilder().SelectAll<Client>();

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
			var query = new SqlServerQueryBuilder().Select(new PredicateBuilder<Client>()
				.Filter(x => x.IsActive));

			//--Assert
			Console.WriteLine(query);
			var queryLines = SplitQuery(query);
			Assert.AreEqual(3, queryLines.Length);
			Assert.AreEqual("select [Id], [CreatedByUserId], [CreatedDateUtc], [ModifiedByUserId], [ModifiedDateUtc], [Name], [IsActive]", queryLines[0]);
			Assert.AreEqual("from [Client]", queryLines[1]);
			Assert.AreEqual("where [IsActive] = @IsActive", queryLines[2]);
		}

		[Test]
		public void TestBuildsInsertQuery()
		{
			//--Act
			var query = new SqlServerQueryBuilder().Insert<Client>();

			//--Assert
			Console.WriteLine(query);
			var queryLines = SplitQuery(query);
			Assert.AreEqual(3, queryLines.Length);
			Assert.AreEqual("insert into [Client]([CreatedByUserId], [ModifiedByUserId], [Name], [IsActive])", queryLines[0]);
			Assert.AreEqual("ouput inserted.[Id], inserted.[CreatedDateUtc], inserted.[ModifiedDateUtc]", queryLines[1]);
			Assert.AreEqual("values(@CreatedByUserId, @ModifiedByUserId, @Name, @IsActive)", queryLines[2]);
		}

		[Test]
		public void TestBuildsManyToManyInsertQuery()
		{
			//--Act
			var query = new SqlServerQueryBuilder().Insert<ClientUser>();

			//--Assert
			Console.WriteLine(query);
			var queryLines = SplitQuery(query);
			Assert.AreEqual(2, queryLines.Length);
			Assert.AreEqual("insert into [Client_User]([ClientId], [UserId], [IsActive])", queryLines[0]);
			Assert.AreEqual("values(@ClientId, @UserId, @IsActive)", queryLines[1]);
		}

		[Test]
		public void TestBuildsUpdateQuery()
		{
			//--Act
			var query = new SqlServerQueryBuilder().Update<Client>();

			//--Assert
			Console.WriteLine(query);
			var queryLines = SplitQuery(query);
			Assert.AreEqual(3, queryLines.Length);
			Assert.AreEqual("update [Client]", queryLines[0]);
			Assert.AreEqual("set [ModifiedByUserId] = @ModifiedByUserId, [ModifiedDateUtc] = @ModifiedDateUtc, [Name] = @Name, [IsActive] = @IsActive", queryLines[1]);
			Assert.AreEqual("where [Id] = @Id", queryLines[2]);
		}

		[Test]
		public void TestBuildsSelectiveUpdateQuery()
		{
			//--Act
			var query = new SqlServerQueryBuilder().Update<Client>(c => c.ModifiedByUserId);

			//--Assert
			Console.WriteLine(query);
			var queryLines = SplitQuery(query);
			Assert.AreEqual(3, queryLines.Length);
			Assert.AreEqual("update [Client]", queryLines[0]);
			Assert.AreEqual("set [ModifiedByUserId] = @ModifiedByUserId", queryLines[1]);
			Assert.AreEqual("where [Id] = @Id", queryLines[2]);
		}

		[Test]
		public void TestBuildsSelectiveUpdateQueryWithBuilder()
		{
			//--Act
			var query = new SqlServerQueryBuilder().Update(new UpdateBuilder<Client>()
				.Set(x => x.ModifiedDateUtc)
				.Filter(x => x.CreatedByUserId));

			//--Assert
			Console.WriteLine(query);
			var queryLines = SplitQuery(query);
			Assert.AreEqual(3, queryLines.Length);
			Assert.AreEqual("update [Client]", queryLines[0]);
			Assert.AreEqual("set [ModifiedDateUtc] = @ModifiedDateUtc", queryLines[1]);
			Assert.AreEqual("where [CreatedByUserId] = @CreatedByUserId", queryLines[2]);
		}

		[Test]
		public void TestCannotBuildSelectiveUpdateForIgnoredColumn()
		{
			//--Act
			Assert.Throws<ArgumentException>(() => new SqlServerQueryBuilder().Update(new UpdateBuilder<Client>()
				.Set(x => x.CreatedByUserId)));
		}
	}
}