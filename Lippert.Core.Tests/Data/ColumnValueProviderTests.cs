using System;
using System.Linq;
using Lippert.Core.Configuration;
using Lippert.Core.Data;
using Lippert.Core.Data.QueryBuilders;
using Lippert.Core.Data.QueryBuilders.Contracts;
using Lippert.Core.Tests.TestSchema;
using NUnit.Framework;

namespace Lippert.Core.Tests.Data
{
	[TestFixture]
	public class ColumnValueProviderTests
	{
		private Guid _currentClientId, _currentUserId;
		
		[OneTimeSetUp]
		public void OneTimeSetUp() => ReflectingRegistrationSource.CodebaseNamespacePrefix = "Lippert";

		[SetUp]
		public void SetUp()
		{
			ClaimsProvider.UserClaims.ClientId = _currentClientId = Guid.NewGuid();
			ClaimsProvider.UserClaims.UserId = _currentUserId = Guid.NewGuid();
		}

		[Test]
		public void TestApplyingInsertValues()
		{
			//--Arrange
			var client = new Client();

			//--Act
			ColumnValueProvider.ApplyInsertValues(client);

			//--Assert
			Assert.AreEqual(_currentUserId, client.CreatedByUserId);
			Assert.AreEqual(_currentUserId, client.ModifiedByUserId);
		}

		[Test]
		public void TestApplyingInsertValuesWhereFieldDefinedByMapAndBuilder()
		{
			//--Arrange
			var clientUser = new ClientUser
			{
				UserId = _currentUserId,
				IsActive = true
			};

			//--Act
			ColumnValueProvider.ApplyInsertValues(clientUser);

			//--Assert
			Assert.AreEqual(_currentClientId, clientUser.ClientId);
			Assert.AreEqual(_currentUserId, clientUser.UserId);
			Assert.AreEqual(true, clientUser.IsActive);
		}

		[Test]
		public void TestApplyingUpdateValues()
		{
			//--Arrange
			var client = new Client();
			var now = DateTime.UtcNow;

			//--Act
			ColumnValueProvider.ApplyUpdateValues(client);

			//--Assert
			Assert.AreEqual(_currentUserId, client.ModifiedByUserId);
			Assert.LessOrEqual(now, client.ModifiedDateUtc);
			Assert.GreaterOrEqual(now.AddSeconds(1), client.ModifiedDateUtc);
		}

		[Test]
		public void TestApplyingUpdateBuilderValues()
		{
			//--Arrange
			IValuedUpdateBuilder<Client> updateBuilder = new ValuedUpdateBuilder<Client>()
				.Set(x => x.Name, "Inactive")
				.Filter(c => c.IsActive, false);
			var now = DateTime.UtcNow;

			//--Act
			ColumnValueProvider.ApplyUpdateBuilderValues(updateBuilder);

			//--Assert
			Assert.AreEqual(_currentUserId, updateBuilder.GetSetColumns()
				.OfType<ValuedColumnMap>()
				.Single(x => x.ColumnName == nameof(Client.ModifiedByUserId)).Value);
			var modifiedDateUtc = (DateTime?)updateBuilder.GetSetColumns()
				.OfType<ValuedColumnMap>()
				.Single(x => x.ColumnName == nameof(Client.ModifiedDateUtc)).Value;
			Assert.LessOrEqual(now, modifiedDateUtc);
			Assert.GreaterOrEqual(now.AddSeconds(1), modifiedDateUtc);
		}
	}
}
