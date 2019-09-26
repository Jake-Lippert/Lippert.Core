using System;
using System.Collections.Generic;
using System.Linq;
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
			Assert.AreEqual(SqlOperation.Insert | SqlOperation.Update, id.IgnoreOperations);
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
			Assert.AreEqual(SqlOperation.Insert | SqlOperation.Update, id.IgnoreOperations);

			var createdByUserId = clientMap[(accessUsingInterface ? typeof(ICreateFields) : typeof(Client)).GetProperty(nameof(Client.CreatedByUserId))];
			Assert.AreEqual("CreatedByUserId", createdByUserId.ColumnName);
			Assert.AreEqual(ColumnBehavior.Basic, createdByUserId.Behavior);
			Assert.AreEqual(SqlOperation.Update, createdByUserId.IgnoreOperations);

			var createdDateUtc = clientMap[(accessUsingInterface ? typeof(ICreateFields) : typeof(Client)).GetProperty(nameof(Client.CreatedDateUtc))];
			Assert.AreEqual("CreatedDateUtc", createdDateUtc.ColumnName);
			Assert.AreEqual(ColumnBehavior.Generated, createdDateUtc.Behavior);
			Assert.AreEqual(SqlOperation.Insert | SqlOperation.Update, createdDateUtc.IgnoreOperations);

			var modifiedByUserId = clientMap[(accessUsingInterface ? typeof(IEditFields) : typeof(Client)).GetProperty(nameof(Client.ModifiedByUserId))];
			Assert.AreEqual("ModifiedByUserId", modifiedByUserId.ColumnName);
			Assert.AreEqual(ColumnBehavior.Basic, modifiedByUserId.Behavior);
			Assert.AreEqual((SqlOperation)0, modifiedByUserId.IgnoreOperations);

			var modifiedDateUtc = clientMap[(accessUsingInterface ? typeof(IEditFields) : typeof(Client)).GetProperty(nameof(Client.ModifiedDateUtc))];
			Assert.AreEqual("ModifiedDateUtc", modifiedDateUtc.ColumnName);
			Assert.AreEqual(ColumnBehavior.Generated, modifiedDateUtc.Behavior);
			Assert.AreEqual(SqlOperation.Insert, modifiedDateUtc.IgnoreOperations);
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
			Assert.AreEqual(SqlOperation.Insert | SqlOperation.Update, id.IgnoreOperations);

			var baseId = inheritingComponentMap[x => x.BaseId];
			Assert.AreEqual("BaseId", baseId.ColumnName);
			Assert.AreEqual(ColumnBehavior.Basic, baseId.Behavior);
			Assert.AreEqual(SqlOperation.Update, baseId.IgnoreOperations);

			var category = inheritingComponentMap[x => x.Category];
			Assert.AreEqual("Category", category.ColumnName);
			Assert.AreEqual(ColumnBehavior.Basic, category.Behavior);
			Assert.AreEqual((SqlOperation)0, category.IgnoreOperations);

			var cost = inheritingComponentMap[x => x.Cost];
			Assert.AreEqual("Cost", cost.ColumnName);
			Assert.AreEqual(ColumnBehavior.Basic, cost.Behavior);
			Assert.AreEqual((SqlOperation)0, cost.IgnoreOperations);

			var generated = inheritingComponentMap.GeneratedColumns.Single();
			Assert.AreEqual("Id", generated.ColumnName);
			Assert.AreEqual(ColumnBehavior.Generated | ColumnBehavior.Key, generated.Behavior);
			Assert.AreEqual(SqlOperation.Insert | SqlOperation.Update, generated.IgnoreOperations);
		}

		[Test]
		public void TestCreateMapForInheritingClass([Values(false, true)] bool includeBaseProperties)
		{
			//--Act
			var employeeMap = new EmployeeMap(includeBaseProperties);
			DisplayTableMapColumns(employeeMap);

			//--Assert
			Assert.AreEqual("Employee", employeeMap.TableName);

			if (includeBaseProperties)
			{
				TestId();
			}
			else
			{
				Assert.Throws<KeyNotFoundException>(() => TestId(), "Id should not be mapped");
			}

			void TestId()
			{
				var id = employeeMap[x => x.Id];
				Assert.AreEqual("Id", id.ColumnName);
				Assert.AreEqual(ColumnBehavior.Key | ColumnBehavior.Generated, id.Behavior);
				Assert.AreEqual(SqlOperation.Insert | SqlOperation.Update, id.IgnoreOperations);
			}

			var userId = employeeMap[x => x.UserId];
			Assert.AreEqual("UserId", userId.ColumnName);
			Assert.AreEqual(ColumnBehavior.Key, userId.Behavior);
			Assert.AreEqual(SqlOperation.Update, userId.IgnoreOperations);

			var companyId = employeeMap[x => x.CompanyId];
			Assert.AreEqual("CompanyId", companyId.ColumnName);
			Assert.AreEqual(ColumnBehavior.Basic, companyId.Behavior);
			Assert.AreEqual(SqlOperation.None, companyId.IgnoreOperations);
		}

		[Test]
		public void TestCreateMapForClassWithFieldDefinedByMapAndBuilder()
		{
			//--Act
			var clientUserMap = new ClientUserMap();
			DisplayTableMapColumns(clientUserMap);

			//--Assert
			Assert.AreEqual("Client_User", clientUserMap.TableName);

			var clientId = clientUserMap[x => x.ClientId];
			Assert.AreEqual(nameof(ClientUser.ClientId), clientId.ColumnName);
			Assert.AreEqual(ColumnBehavior.Key, clientId.Behavior);
			Assert.AreEqual(SqlOperation.Update, clientId.IgnoreOperations);

			var userId = clientUserMap[x => x.UserId];
			Assert.AreEqual(nameof(ClientUser.UserId), userId.ColumnName);
			Assert.AreEqual(ColumnBehavior.Key, userId.Behavior);
			Assert.AreEqual(SqlOperation.Update, userId.IgnoreOperations);

			var isActive = clientUserMap[x => x.IsActive];
			Assert.AreEqual(nameof(ClientUser.IsActive), isActive.ColumnName);
			Assert.AreEqual(ColumnBehavior.Basic, isActive.Behavior);
			Assert.AreEqual(SqlOperation.None, isActive.IgnoreOperations);

			foreach (var type in clientUserMap.TypeColumns)
			{
				Console.WriteLine(type.Key.Name);
				foreach (var (property, columnMap) in type.Value.AsTuples())
				{
					Console.WriteLine($"-{property} => {columnMap.Behavior}");
				}
			}

			Assert.AreSame(clientUserMap.TypeColumns[typeof(IClientRecord)].Values.First(), clientUserMap.TypeColumns[typeof(ClientUser)].Values.First());
		}
	}
}