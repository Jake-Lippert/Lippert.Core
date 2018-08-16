using Lippert.Core.Data;

namespace Lippert.Core.Tests.TestSchema.TableMaps
{
	public class EmployeeMap : TableMap<Employee>
	{
		public EmployeeMap()
			: this(false) { }
		public EmployeeMap(bool includeBaseProperties)
			: base(includeBaseProperties ? typeof(User) : typeof(Employee))
		{
			Map(x => x.UserId).Key(false);
			AutoMap();
		}
	}
}