using System;
using System.Linq;
using Lippert.Core.Configuration;
using NUnit.Framework;

namespace Lippert.Core.Tests.Configuration
{
	[TestFixture]
    public class ReflectingRegistrationSourceTests
    {
		[Test]
		public void TestGetsDependencies()
		{
			//--Arrange
			ReflectingRegistrationSource.CodebaseNamespacePrefix = "Lippert";

			//--Act
			var dependencies = ReflectingRegistrationSource.GetCodebaseDependencies()
				.ToDictionary(x => (Type)x.Class, x => x.ImplementedInterfaces);

			//--Assert
			CollectionAssert.AreEquivalent(new Type[] { typeof(Core.Data.Contracts.IValuedColumnMap) }, dependencies[typeof(Core.Data.ValuedColumnMap)]);
		}
    }
}