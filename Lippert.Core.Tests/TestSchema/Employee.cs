using System;

namespace Lippert.Core.Tests.TestSchema
{
	public class Employee : User
	{
		public Guid CompanyId { get; set; }
	}
}