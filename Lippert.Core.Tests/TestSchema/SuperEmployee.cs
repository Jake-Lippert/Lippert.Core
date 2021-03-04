namespace Lippert.Core.Tests.TestSchema
{
	public class SuperEmployee : Employee
	{
		public System.Guid EmployeeId { get; set; }
		public string? SomeAwesomeFieldA { get; set; }
		public string? SomeAwesomeFieldB { get; set; }
	}
}