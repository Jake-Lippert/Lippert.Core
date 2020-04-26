using System;
using Lippert.Core.Reflection;
using Lippert.Core.Reflection.Extensions;
using Lippert.Core.Tests.TestSchema;
using Lippert.Core.Tests.TestSchema.Contracts;
using NUnit.Framework;

namespace Lippert.Core.Tests.Reflection
{
	[TestFixture]
	public class PropertyAccessorTests
	{
		#region Get
		[Test]
		public void TestImplementingClass()
		{
			var propIC = PropertyAccessor.Get<ICreateFields>(x => x.CreatedByUserId);
			var propICE = PropertyAccessor.Get<ICreateEditFields>(x => x.CreatedByUserId);
			var propC = PropertyAccessor.Get<Client>(x => x.CreatedByUserId);
			var propCIC = propIC.Get<Client>();
			var propCICE = propICE.Get<Client>();
			Console.WriteLine($"IC: {propIC.ReflectedType}.{propIC.Name}");
			Console.WriteLine($"ICE: {propICE.ReflectedType}.{propICE.Name}");
			Console.WriteLine($"C: {propC.ReflectedType}.{propC.Name}");
			Console.WriteLine($"CIC: {propCIC?.ReflectedType}.{propCIC?.Name}");
			Console.WriteLine($"CICE: {propCICE?.ReflectedType}.{propCICE?.Name}");

			Assert.AreEqual(propIC, propICE);
			Assert.AreEqual(propCIC, propCICE);
			//--The interface is not the same as the implementing class...
			Assert.AreNotEqual(propC, propIC);
			Assert.AreNotEqual(propC, propICE);
			//--...though the interface can be mapped to the class in question
			Assert.AreEqual(propC, propCIC);
			Assert.AreEqual(propC, propCICE);
		}

		[Test]
		public void TestInheritedImplementation()
		{
			var propI = PropertyAccessor.Get<IGuidIdentifier>(x => x.Id);
			var propU = PropertyAccessor.Get<User>(x => x.Id);
			var propE = PropertyAccessor.Get<Employee>(x => x.Id);
			var propEI = propI.Get<Employee>();
			Console.WriteLine($"I: {propI.ReflectedType}.{propI.Name}");
			Console.WriteLine($"U: {propU.ReflectedType}.{propU.Name}");
			Console.WriteLine($"E: {propE.ReflectedType}.{propE.Name}");
			Console.WriteLine($"EI: {propEI?.ReflectedType}.{propEI?.Name}");

			//--Employee inherits User, so the property is actually User's
			Assert.AreEqual(propU, propE);
			Assert.AreEqual(typeof(User), propEI?.DeclaringType);
			Assert.AreEqual(typeof(Employee), propEI?.ReflectedType);
		}

		[Test]
		public void TestGettingInterfaceFromClass()
		{
			var propIC = PropertyAccessor.Get<ICreateFields>(x => x.CreatedByUserId);
			var propICE = PropertyAccessor.Get<ICreateEditFields>(x => x.CreatedByUserId);
			var propC = PropertyAccessor.Get<Client>(x => x.CreatedByUserId);
			var propICC = propC.Get<ICreateFields>();
			var propICEC = propC.Get<ICreateEditFields>();
			Console.WriteLine($"IC: {propIC.ReflectedType}.{propIC.Name}");
			Console.WriteLine($"ICE: {propICE.ReflectedType}.{propICE.Name}");
			Console.WriteLine($"C: {propC.ReflectedType}.{propC.Name}");
			Console.WriteLine($"ICC: {propICC?.ReflectedType}.{propICC?.Name}");
			Console.WriteLine($"ICEC: {propICEC?.ReflectedType}.{propICEC?.Name}");

			Assert.AreEqual(propIC, propICE);
			Assert.IsNull(propICEC);
			//--The implementing class is not the same as the interface...
			Assert.AreNotEqual(propC, propIC);
			Assert.AreNotEqual(propC, propICE);
			//--...though the class can be mapped to the interface in question
			Assert.AreEqual(propIC, propICC);
		}

		[Test]
		public void TestSourceAndTargetCannotBothBeInterfaces()
		{
			var propIC = PropertyAccessor.Get<ICreateFields>(x => x.CreatedByUserId);
			var propICE = PropertyAccessor.Get<ICreateEditFields>(x => x.CreatedByUserId);
			Assert.Throws<InvalidOperationException>(() => propIC.Get<ICreateEditFields>());
			Assert.Throws<InvalidOperationException>(() => propICE.Get<ICreateFields>());
		}

		[Test]
		public void TestSourceAndTargetCannotBothBeClasses()
		{
			var propU = PropertyAccessor.Get<User>(x => x.Id);
			var propE = PropertyAccessor.Get<Employee>(x => x.Id);
			Assert.Throws<InvalidOperationException>(() => propU.Get<Employee>());
			Assert.Throws<InvalidOperationException>(() => propE.Get<User>());
		}

		[Test]
		public void TestPropertyDoesntExistOnInterfaceTarget()
		{
			var propC = PropertyAccessor.Get<Client>(x => x.ModifiedByUserId);
			var propICC = propC.Get<ICreateFields>();
			Assert.IsNull(propICC);
		}

		[Test]
		public void TestSourceClassDoesntHaveTargetInterface()
		{
			var propU = PropertyAccessor.Get<User>(x => x.Id);
			Assert.Throws<ArgumentException>(() => propU.Get<ICreateFields>());
		}

		[Test]
		public void TestTargetClassDoesntHaveSourceInterface()
		{
			var propIC = PropertyAccessor.Get<ICreateFields>(x => x.CreatedByUserId);
			Assert.Throws<ArgumentException>(() => propIC.Get<User>());
		}

		[Test]
		public void TestThrowsArgumentExceptionForMethods()
		{
			Assert.Throws<ArgumentException>(() => PropertyAccessor.Get<string>(x => x.ToString()));
		}
		#endregion


		#region TryGet
		[Test]
		public void TestTryGetProperty()
		{
			//--Act
			var success = PropertyAccessor.TryGet<Client>(x => x.CreatedByUserId, out var propertyInfo);

			//--Assert
			Assert.IsTrue(success);
			Assert.AreEqual(typeof(Client), propertyInfo?.DeclaringType);
			Assert.AreEqual(nameof(Client.CreatedByUserId), propertyInfo?.Name);
		}

		[Test]
		public void TestTryGetPropertyUnsuccessfulForMethods()
		{
			//--Act
			var success = PropertyAccessor.TryGet<Client>(x => x.ToString(), out var propertyInfo);

			//--Assert
			Assert.IsFalse(success);
			Assert.IsNull(propertyInfo);
		}
		#endregion
	}
}