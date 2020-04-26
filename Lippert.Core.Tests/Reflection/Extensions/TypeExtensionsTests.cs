using System;
using System.Linq;
using Lippert.Core.Reflection.Extensions;
using Lippert.Core.Tests.TestSchema;
using Lippert.Core.Tests.TestSchema.Contracts;
using NUnit.Framework;

namespace Lippert.Core.Tests.Reflection.Extensions
{
	[TestFixture]
	public class TypeExtensionsTests
	{
		#region IsAssignableTo
		[Test]
		[TestCase(typeof(Employee), typeof(Employee), true)]
		[TestCase(typeof(Employee), typeof(User), true)]
		[TestCase(typeof(Employee), typeof(IGuidIdentifier), true)]
		[TestCase(typeof(User), typeof(Employee), false)]
		[TestCase(typeof(User), typeof(User), true)]
		[TestCase(typeof(User), typeof(IGuidIdentifier), true)]
		[TestCase(typeof(IGuidIdentifier), typeof(Employee), false)]
		[TestCase(typeof(IGuidIdentifier), typeof(User), false)]
		[TestCase(typeof(IGuidIdentifier), typeof(IGuidIdentifier), true)]
		public void TestIsAssignableTo(Type type, Type target, bool shouldBeAbleToAssign)
		{
			//--Arrange
			var isAssignableTo = typeof(TypeExtensions).GetMethod(nameof(TypeExtensions.IsAssignableTo), new[] { typeof(Type), typeof(Type) });

			//--Act
			var canAssignTo = isAssignableTo.Invoke(null, new[] { type, target });

			//--Assert
			Assert.AreEqual(shouldBeAbleToAssign, canAssignTo);
		}

		[Test]
		[TestCase(typeof(Employee), typeof(Employee), true)]
		[TestCase(typeof(Employee), typeof(User), true)]
		[TestCase(typeof(Employee), typeof(IGuidIdentifier), true)]
		[TestCase(typeof(User), typeof(Employee), false)]
		[TestCase(typeof(User), typeof(User), true)]
		[TestCase(typeof(User), typeof(IGuidIdentifier), true)]
		[TestCase(typeof(IGuidIdentifier), typeof(Employee), false)]
		[TestCase(typeof(IGuidIdentifier), typeof(User), false)]
		[TestCase(typeof(IGuidIdentifier), typeof(IGuidIdentifier), true)]
		public void TestGenericIsAssignableTo(Type type, Type target, bool shouldBeAbleToAssign)
		{
			//--Arrange
			var isAssignableTo = typeof(TypeExtensions).GetMethod(nameof(TypeExtensions.IsAssignableTo), new[] { typeof(Type) })
				.MakeGenericMethod(target);

			//--Act
			var canAssignTo = isAssignableTo.Invoke(null, new[] { type });

			//--Assert
			Assert.AreEqual(shouldBeAbleToAssign, canAssignTo);
		}
		#endregion


		#region GetBaseTypes
		[Test]
		[TestCase(typeof(Employee), true, true, "Employee|User|IGuidIdentifier")]
		[TestCase(typeof(Employee), true, false, "Employee|User")]
		[TestCase(typeof(Employee), false, true, "User|IGuidIdentifier")]
		[TestCase(typeof(Employee), false, false, "User")]
		[TestCase(typeof(User), true, true, "User|IGuidIdentifier")]
		[TestCase(typeof(User), true, false, "User")]
		[TestCase(typeof(User), false, true, "IGuidIdentifier")]
		[TestCase(typeof(User), false, false, "")]
		[TestCase(typeof(IGuidIdentifier), true, true, "IGuidIdentifier")]
		[TestCase(typeof(IGuidIdentifier), true, false, "IGuidIdentifier")]
		[TestCase(typeof(IGuidIdentifier), false, true, "")]
		[TestCase(typeof(IGuidIdentifier), false, false, "")]
		public void TestGetBaseTypes(Type seedType, bool includeSeed, bool includeInterfaces, string expectedTypes)
		{
			//--Arrange
			var expected = expectedTypes.Split("|".ToArray(), StringSplitOptions.RemoveEmptyEntries);

			//--Act
			var actual = seedType.GetBaseTypes(includeSeed, includeInterfaces);

			//--Assert
			CollectionAssert.AreEquivalent(expected, actual.Select(x => x.Name));
		}
		#endregion
	}
}