using System;
using System.Collections.Generic;
using Lippert.Core.Data.QueryBuilders.MergeSerializers;
using Lippert.Core.Tests.TestSchema;
using NUnit.Framework;

namespace Lippert.Core.Tests.Data.QueryBuilders.MergeSerializers
{
	[TestFixture]
	public class XmlMergeSerializerTests
	{
		[Test]
		public void TestNullValueExcludedFromMergeSerialization()
		{
			//--Arrange
			var idA = Guid.NewGuid();
			var idB = Guid.NewGuid();
			var records = new[]
			{
				new SuperEmployee
				{
					EmployeeId = idA,
					SomeAwesomeFieldA = "Name A",
					SomeAwesomeFieldB = null
				},
				new SuperEmployee
				{
					EmployeeId = idB,
					SomeAwesomeFieldA = null,
					SomeAwesomeFieldB = "Name B"
				}
			};
			var aliases = new Dictionary<System.Reflection.PropertyInfo, string>
			{
				[typeof(SuperEmployee).GetProperty(nameof(SuperEmployee.EmployeeId))] = "0",
				[typeof(SuperEmployee).GetProperty(nameof(SuperEmployee.SomeAwesomeFieldA))] = "1",
				[typeof(SuperEmployee).GetProperty(nameof(SuperEmployee.SomeAwesomeFieldB))] = "2"
			};

			//--Act
			var serialized = new XmlMergeSerializer<SuperEmployee>(aliases).SerializeForMerge(records);

			//--Assert
			Assert.AreEqual($"<_><_ _=\"0\" _0=\"{idA}\" _1=\"Name A\" /><_ _=\"1\" _0=\"{idB}\" _2=\"Name B\" /></_>", serialized);
		}
	}
}