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
			Assert.AreEqual(3, queryLines.Length);
			Assert.AreEqual("insert into [Client]([CreatedByUserId], [ModifiedByUserId], [Name], [IsActive])", queryLines[0]);
			Assert.AreEqual("output inserted.[Id], inserted.[CreatedDateUtc], inserted.[ModifiedDateUtc]", queryLines[1]);
			Assert.AreEqual("values(@CreatedByUserId, @ModifiedByUserId, @Name, @IsActive)", queryLines[2]);
		}

		[Test]
		public void TestBuildsManyToManyInsertQuery()
		{
			//--Act
			var query = new SqlServerInsertQueryBuilder().Insert<ClientUser>();

			//--Assert
			Console.WriteLine(query);
			var queryLines = SplitQuery(query);
			Assert.AreEqual(2, queryLines.Length);
			Assert.AreEqual("insert into [Client_User]([ClientId], [UserId], [IsActive])", queryLines[0]);
			Assert.AreEqual("values(@ClientId, @UserId, @IsActive)", queryLines[1]);
		}
	}
}