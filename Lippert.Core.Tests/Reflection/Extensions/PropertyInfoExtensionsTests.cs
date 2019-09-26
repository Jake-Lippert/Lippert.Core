using Lippert.Core.Reflection;
using Lippert.Core.Reflection.Extensions;
using Lippert.Core.Tests.TestSchema;
using Lippert.Core.Tests.TestSchema.Contracts;
using NUnit.Framework;

namespace Lippert.Core.Tests.Reflection
{
	[TestFixture]
	public class PropertyInfoExtensionsTests
	{
		[Test]
		public void TestFindsOriginalInterface()
		{
			//--Arrange
			var propC = PropertyAccessor.Get<Client>(x => x.CreatedByUserId);

			//--Act
			var (type, property) = propC.GetDeclaringType(true);

			//--Assert
			Assert.AreEqual(typeof(ICreateFields), type);
			Assert.AreEqual(typeof(ICreateFields), property.DeclaringType);
		}
	}
}