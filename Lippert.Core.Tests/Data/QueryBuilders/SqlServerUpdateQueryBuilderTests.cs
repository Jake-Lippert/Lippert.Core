using System;
using Lippert.Core.Configuration;
using Lippert.Core.Data.QueryBuilders;
using Lippert.Core.Tests.TestSchema;
using NUnit.Framework;

namespace Lippert.Core.Tests.Data.QueryBuilders
{
	[TestFixture]
	public class SqlServerUpdateQueryBuilderTests
	{
		[OneTimeSetUp]
		public void OneTimeSetUp() => ReflectingRegistrationSource.CodebaseNamespacePrefix = nameof(Lippert);

		private string[] SplitQuery(string query) => query.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

		[Test]
		public void TestBuildsUpdateQuery()
		{
			//--Act
			var query = new SqlServerUpdateQueryBuilder().Update<Client>();

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
			var query = new SqlServerUpdateQueryBuilder().Update<Client>(c => c.ModifiedByUserId);

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
			var query = new SqlServerUpdateQueryBuilder().Update(new UpdateBuilder<Client>()
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
			Assert.Throws<ArgumentException>(() => new SqlServerUpdateQueryBuilder().Update(new UpdateBuilder<Client>()
				.Set(x => x.CreatedByUserId)));
		}
	}
}