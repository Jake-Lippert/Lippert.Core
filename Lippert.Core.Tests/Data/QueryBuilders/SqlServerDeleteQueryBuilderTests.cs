using System;
using Lippert.Core.Configuration;
using Lippert.Core.Data.QueryBuilders;
using Lippert.Core.Tests.TestSchema;
using NUnit.Framework;

namespace Lippert.Core.Tests.Data.QueryBuilders
{
	[TestFixture]
	public class SqlServerDeleteQueryBuilderTests
	{
		[OneTimeSetUp]
		public void OneTimeSetUp() => ReflectingRegistrationSource.CodebaseNamespacePrefix = nameof(Lippert);

		private string[] SplitQuery(string query) => query.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

		[Test]
		public void TestBuildsDeleteByKeyQuery()
		{
			//--Act
			var query = new SqlServerDeleteQueryBuilder().DeleteKey<Client>();

			//--Assert
			Console.WriteLine(query);
			var queryLines = SplitQuery(query);
			Assert.AreEqual(2, queryLines.Length);
			Assert.AreEqual("delete from [Client]", queryLines[0]);
			Assert.AreEqual("where [Id] = @Id", queryLines[1]);
		}

		[Test]
		public void TestBuildsManyToManyDeleteByKeyQuery()
		{
			//--Act
			var query = new SqlServerDeleteQueryBuilder().DeleteKey<ClientUser>();

			//--Assert
			Console.WriteLine(query);
			var queryLines = SplitQuery(query);
			Assert.AreEqual(2, queryLines.Length);
			Assert.AreEqual("delete from [Client_User]", queryLines[0]);
			Assert.AreEqual("where [ClientId] = @ClientId and [UserId] = @UserId", queryLines[1]);
		}

		[Test]
		public void TestBuildsDeleteQuery()
		{
			//--Act
			var query = new SqlServerDeleteQueryBuilder().Delete(new PredicateBuilder<Client>().Filter(x => x.IsActive));

			//--Assert
			Console.WriteLine(query);
			var queryLines = SplitQuery(query);
			Assert.AreEqual(2, queryLines.Length);
			Assert.AreEqual("delete from [Client]", queryLines[0]);
			Assert.AreEqual("where [IsActive] = @IsActive", queryLines[1]);
		}

		[Test]
		public void TestBuildsManyToManyDeleteQuery()
		{
			//--Act
			var query = new SqlServerDeleteQueryBuilder().Delete(new PredicateBuilder<ClientUser>().Filter(x => x.UserId).Filter(x => x.IsActive));

			//--Assert
			Console.WriteLine(query);
			var queryLines = SplitQuery(query);
			Assert.AreEqual(2, queryLines.Length);
			Assert.AreEqual("delete from [Client_User]", queryLines[0]);
			Assert.AreEqual("where [UserId] = @UserId and [IsActive] = @IsActive", queryLines[1]);
		}
	}
}