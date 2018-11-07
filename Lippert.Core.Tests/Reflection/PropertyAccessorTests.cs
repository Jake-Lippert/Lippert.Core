using System;
using Lippert.Core.Reflection;
using Lippert.Core.Tests.TestSchema;
using Lippert.Core.Tests.TestSchema.Contracts;
using NUnit.Framework;

namespace Lippert.Core.Tests.Reflection
{
	[TestFixture]
	public class PropertyAccessorTests
	{
		[Test]
		public void TestImplementingClass()
		{
			var propIC = PropertyAccessor.Get<ICreateFields>(x => x.CreatedByUserId);
			var propICE = PropertyAccessor.Get<ICreateEditFields>(x => x.CreatedByUserId);
			var propC = PropertyAccessor.Get<Client>(x => x.CreatedByUserId);
			var propCIC = PropertyAccessor.Get<Client>(propIC);
			var propCICE = PropertyAccessor.Get<Client>(propICE);
			Console.WriteLine($"IC: {propIC.ReflectedType}.{propIC.Name}");
			Console.WriteLine($"ICE: {propICE.ReflectedType}.{propICE.Name}");
			Console.WriteLine($"C: {propC.ReflectedType}.{propC.Name}");
			Console.WriteLine($"CIC: {propCIC.ReflectedType}.{propCIC.Name}");
			Console.WriteLine($"CICE: {propCICE.ReflectedType}.{propCICE.Name}");

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
			var propEI = PropertyAccessor.Get<Employee>(propI);
			Console.WriteLine($"I: {propI.ReflectedType}.{propI.Name}");
			Console.WriteLine($"U: {propU.ReflectedType}.{propU.Name}");
			Console.WriteLine($"E: {propE.ReflectedType}.{propE.Name}");
			Console.WriteLine($"EI: {propEI.ReflectedType}.{propEI.Name}");

			//--Employee inherits User, so the property is actually User's
			Assert.AreEqual(propU, propE);
			Assert.AreEqual(typeof(User), propEI.DeclaringType);
			Assert.AreEqual(typeof(Employee), propEI.ReflectedType);
		}

		[Test]
		public void TestGettingInterfaceFromClass()
		{
			var propIC = PropertyAccessor.Get<ICreateFields>(x => x.CreatedByUserId);
			var propICE = PropertyAccessor.Get<ICreateEditFields>(x => x.CreatedByUserId);
			var propC = PropertyAccessor.Get<Client>(x => x.CreatedByUserId);
			var propICC = PropertyAccessor.Get<ICreateFields>(propC);
			var propICEC = PropertyAccessor.Get<ICreateEditFields>(propC);
			Console.WriteLine($"IC: {propIC.ReflectedType}.{propIC.Name}");
			Console.WriteLine($"ICE: {propICE.ReflectedType}.{propICE.Name}");
			Console.WriteLine($"C: {propC.ReflectedType}.{propC.Name}");
			Console.WriteLine($"ICC: {propICC.ReflectedType}.{propICC.Name}");
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
			Assert.Throws<InvalidOperationException>(() => PropertyAccessor.Get<ICreateEditFields>(propIC));
			Assert.Throws<InvalidOperationException>(() => PropertyAccessor.Get<ICreateFields>(propICE));
		}

		[Test]
		public void TestSourceAndTargetCannotBothBeClasses()
		{
			var propU = PropertyAccessor.Get<User>(x => x.Id);
			var propE = PropertyAccessor.Get<Employee>(x => x.Id);
			Assert.Throws<InvalidOperationException>(() => PropertyAccessor.Get<Employee>(propU));
			Assert.Throws<InvalidOperationException>(() => PropertyAccessor.Get<User>(propE));
		}

		[Test]
		public void TestPropertyDoesntExistOnInterfaceTarget()
		{
			var propC = PropertyAccessor.Get<Client>(x => x.ModifiedByUserId);
			var propICC = PropertyAccessor.Get<ICreateFields>(propC);
			Assert.IsNull(propICC);
		}

		[Test]
		public void TestSourceClassDoesntHaveTargetInterface()
		{
			var propU = PropertyAccessor.Get<User>(x => x.Id);
			Assert.Throws<ArgumentException>(() => PropertyAccessor.Get<ICreateFields>(propU));
		}

		[Test]
		public void TestTargetClassDoesntHaveSourceInterface()
		{
			var propIC = PropertyAccessor.Get<ICreateFields>(x => x.CreatedByUserId);
			Assert.Throws<ArgumentException>(() => PropertyAccessor.Get<User>(propIC));
		}

		[Test]
		public void TestThrowsArgumentExceptionForMethods()
		{
			Assert.Throws<ArgumentException>(() => PropertyAccessor.Get<string>(x => x.ToString()));
		}
	}
}