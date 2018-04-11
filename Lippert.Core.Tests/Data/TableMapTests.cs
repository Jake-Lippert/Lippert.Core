using System;
using Lippert.Core.Collections.Extensions;
using Lippert.Core.Configuration;
using Lippert.Core.Data;
using Lippert.Core.Tests.TestSchema;
using Lippert.Core.Tests.TestSchema.Contracts;
using Lippert.Core.Tests.TestSchema.TableMaps;
using NUnit.Framework;

namespace Lippert.Core.Tests.Data
{
	[TestFixture]
	public class TableMapTests
	{
		[OneTimeSetUp]
		public void OneTimeSetUp() => ReflectingRegistrationSource.CodebaseNamespacePrefix = "Lippert";

		private void DisplayTableMapColumns<T>(TableMap<T> tableMap)
		{
			foreach (var (type, columns) in tableMap.TypeColumns.AsTuples())
			{
				Console.WriteLine(type);
				foreach (var (_, column) in columns.AsTuples())
				{
					Console.WriteLine($"-{column.ColumnName}");
				}
			}
		}

		[Test]
		public void TestCreateBasicMap()
		{
			//--Act
			var userMap = new UserMap();
			DisplayTableMapColumns(userMap);

			//--Assert
			Assert.AreEqual("User", userMap.TableName);

			var id = userMap[x => x.Id];
			Assert.AreEqual("Id", id.ColumnName);
			Assert.AreEqual(ColumnBehavior.Key | ColumnBehavior.Generated, id.Behavior);
			Assert.AreEqual(IgnoreBehavior.Insert | IgnoreBehavior.Update, id.IgnoreOperations);
		}

		[Test]
		public void TestCreateMapWithHelper([Values(false, true)] bool accessUsingInterface)
		{
			//--Act
			var clientMap = new ClientMap();
			DisplayTableMapColumns(clientMap);

			//--Assert
			Assert.AreEqual("Client", clientMap.TableName);

			var id = clientMap[typeof(IGuidIdentifier).GetProperty(nameof(IGuidIdentifier.Id))];
			Assert.AreEqual("Id", id.ColumnName);
			Assert.AreEqual(ColumnBehavior.Key | ColumnBehavior.Generated, id.Behavior);
			Assert.AreEqual(IgnoreBehavior.Insert | IgnoreBehavior.Update, id.IgnoreOperations);

			var createdByUserId = clientMap[(accessUsingInterface ? typeof(ICreateFields) : typeof(Client)).GetProperty(nameof(Client.CreatedByUserId))];
			Assert.AreEqual("CreatedByUserId", createdByUserId.ColumnName);
			Assert.AreEqual(ColumnBehavior.Basic, createdByUserId.Behavior);
			Assert.AreEqual(IgnoreBehavior.Update, createdByUserId.IgnoreOperations);

			var createdDateUtc = clientMap[(accessUsingInterface ? typeof(ICreateFields) : typeof(Client)).GetProperty(nameof(Client.CreatedDateUtc))];
			Assert.AreEqual("CreatedDateUtc", createdDateUtc.ColumnName);
			Assert.AreEqual(ColumnBehavior.Generated, createdDateUtc.Behavior);
			Assert.AreEqual(IgnoreBehavior.Insert | IgnoreBehavior.Update, createdDateUtc.IgnoreOperations);

			var modifiedByUserId = clientMap[(accessUsingInterface ? typeof(IEditFields) : typeof(Client)).GetProperty(nameof(Client.ModifiedByUserId))];
			Assert.AreEqual("ModifiedByUserId", modifiedByUserId.ColumnName);
			Assert.AreEqual(ColumnBehavior.Basic, modifiedByUserId.Behavior);
			Assert.AreEqual((IgnoreBehavior)0, modifiedByUserId.IgnoreOperations);

			var modifiedDateUtc = clientMap[(accessUsingInterface ? typeof(IEditFields) : typeof(Client)).GetProperty(nameof(Client.ModifiedDateUtc))];
			Assert.AreEqual("ModifiedDateUtc", modifiedDateUtc.ColumnName);
			Assert.AreEqual(ColumnBehavior.Basic, modifiedDateUtc.Behavior);
			Assert.AreEqual(IgnoreBehavior.Insert, modifiedDateUtc.IgnoreOperations);
		}

		[Test]
		public void TestCreatesInheritingComponentMap()
		{
			//--Act
			var inheritingComponentMap = new InheritingComponentMap();
			DisplayTableMapColumns(inheritingComponentMap);

			//--Assert
			Assert.AreEqual("InheritingComponent", inheritingComponentMap.TableName);
			Assert.AreEqual(4, inheritingComponentMap.SelectColumns.Count);

			var id = inheritingComponentMap[x => x.Id];
			var idBase = typeof(BaseRecord).GetProperty(nameof(BaseRecord.Id));
			Assert.AreEqual("Id", id.ColumnName);
			Assert.AreNotEqual(idBase, id.Property);//--The base class has an Id as well, make sure the inherting record doesn't reference that property
			Assert.AreEqual(ColumnBehavior.Key | ColumnBehavior.Generated, id.Behavior);
			Assert.AreEqual(IgnoreBehavior.Insert | IgnoreBehavior.Update, id.IgnoreOperations);

			//var id_ = inheritingComponentMap[idBase];
			//Assert.AreEqual("Id", id_.ColumnName);
			//Assert.AreEqual(idBase, id_.Property);
			//Assert.AreEqual(ColumnBehavior.Key | ColumnBehavior.Generated, id_.Behavior);
			//Assert.AreEqual(IgnoreBehavior.Insert | IgnoreBehavior.Update, id_.IgnoreOperations);

			var baseId = inheritingComponentMap[x => x.BaseId];
			Assert.AreEqual("BaseId", baseId.ColumnName);
			Assert.AreEqual(ColumnBehavior.Basic, baseId.Behavior);
			Assert.AreEqual(IgnoreBehavior.Update, baseId.IgnoreOperations);

			var category = inheritingComponentMap[x => x.Category];
			Assert.AreEqual("Category", category.ColumnName);
			Assert.AreEqual(ColumnBehavior.Basic, category.Behavior);
			Assert.AreEqual((IgnoreBehavior)0, category.IgnoreOperations);

			var cost = inheritingComponentMap[x => x.Cost];
			Assert.AreEqual("Cost", cost.ColumnName);
			Assert.AreEqual(ColumnBehavior.Basic, cost.Behavior);
			Assert.AreEqual((IgnoreBehavior)0, cost.IgnoreOperations);
		}
	}
}