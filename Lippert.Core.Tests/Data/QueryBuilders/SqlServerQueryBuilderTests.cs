using System;
using Lippert.Core.Data;
using Lippert.Core.Data.QueryBuilders;
using Lippert.Core.Tests.TestSchema;
using Lippert.Core.Tests.TestSchema.TableMaps;
using NUnit.Framework;

namespace Lippert.Core.Tests.Data.QueryBuilders
{
	[TestFixture]
	public class SqlServerQueryBuilderTests
	{
		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			TableMap.SetMap(new ClientMap(true));
			TableMap.SetMap(new UserMap());
			TableMap.SetMap(new ClientUserMap());
		}

		[Test]
		public void TestBuildsSelectQuery()
		{
			//--Act
			var query = new SqlServerQueryBuilder().SelectAll<Client>();

			//--Assert
			Console.WriteLine(query);
			var queryLines = query.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
			Assert.AreEqual("select [Id], [CreatedByUserId], [CreatedDateUtc], [ModifiedByUserId], [ModifiedDateUtc], [Name], [IsActive]", queryLines[0]);
			Assert.AreEqual("from [Client]", queryLines[1]);
		}

		[Test]
		public void TestBuildsInsertQuery()
		{
			//--Act
			var query = new SqlServerQueryBuilder().Insert<Client>();

			//--Assert
			Console.WriteLine(query);
			var queryLines = query.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
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
			var queryLines = query.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
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
			var queryLines = query.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
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
			var queryLines = query.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
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
			var queryLines = query.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
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