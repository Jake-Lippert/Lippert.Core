using System;
using Lippert.Core.Extensions;
using NUnit.Framework;

namespace Lippert.Core.Tests.Extensions
{
	[TestFixture]
	public class DateTimeExtensionsTests
	{
		[Test]
		[TestCase(DateTimeKind.Local)]
		[TestCase(DateTimeKind.Unspecified)]
		[TestCase(DateTimeKind.Utc)]
		public void TestSpecifiesDateTimeKind(DateTimeKind kind)
		{
			//--Arrange
			DateTime dateTime;
			switch (kind)
			{
				case DateTimeKind.Local:
				case DateTimeKind.Unspecified:
					dateTime = DateTime.UtcNow;
					break;
				case DateTimeKind.Utc:
					dateTime = DateTime.Now;
					break;
				default:
					throw new ArgumentException(kind.ToString());
			}

			//--Act
			var specified = dateTime.SpecifyKind(kind);

			//--Assert
			Assert.AreEqual(kind, specified.Kind);
			Assert.AreNotEqual(kind, dateTime.Kind);
			Assert.AreEqual(dateTime, specified);
		}
	}
}