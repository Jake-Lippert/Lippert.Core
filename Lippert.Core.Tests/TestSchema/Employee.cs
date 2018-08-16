using System;

namespace Lippert.Core.Tests.TestSchema
{
	public class Employee : User
	{
		public Guid UserId
		{
			get => Id;
			set => Id = value;
		}
		public Guid CompanyId { get; set; }
	}
}