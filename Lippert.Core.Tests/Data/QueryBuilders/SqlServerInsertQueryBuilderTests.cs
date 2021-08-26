using System;
using Lippert.Core.Configuration;
using Lippert.Core.Data.QueryBuilders;
using Lippert.Core.Tests.TestSchema;
using NUnit.Framework;

namespace Lippert.Core.Tests.Data.QueryBuilders
{
	[TestFixture]
	public class SqlServerInsertQueryBuilderTests
	{
		[OneTimeSetUp]
		public void OneTimeSetUp() => ReflectingRegistrationSource.CodebaseNamespacePrefix = nameof(Lippert);

		private string[] SplitQuery(string query) => query.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

		[Test]
		public void TestBuildsInsertQuery()
		{
			//--Act
			var query = new SqlServerInsertQueryBuilder().Insert<Client>();

			//--Assert
			Console.WriteLine(query);
			var queryLines = SplitQuery(query);
			Assert.AreEqual(new[]
			{
				"declare @outputResult table(",
				"  [Id] uniqueidentifier,",
				"  [CreatedDateUtc] datetime,",
				"  [ModifiedDateUtc] datetime",
				");",
				"insert into [Client]([CreatedByUserId], [ModifiedByUserId], [Name], [IsActive])",
				"output inserted.[Id], inserted.[CreatedDateUtc], inserted.[ModifiedDateUtc] into @outputResult([Id], [CreatedDateUtc], [ModifiedDateUtc])",
				"values(@CreatedByUserId, @ModifiedByUserId, @Name, @IsActive);",
				"select * from @outputResult;"
			}, queryLines);
		}

		[Test]
		public void TestBuildsManyToManyInsertQuery()
		{
			//--Act
			var query = new SqlServerInsertQueryBuilder().Insert<ClientUser>();

			//--Assert
			Console.WriteLine(query);
			var queryLines = SplitQuery(query);
			Assert.AreEqual(new[]
			{
				"insert into [Client_User]([ClientId], [UserId], [IsActive])",
				"values(@ClientId, @UserId, @IsActive);"
			}, queryLines);
		}
	}
}