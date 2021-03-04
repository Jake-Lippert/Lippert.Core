using Lippert.Core.Data;

namespace Lippert.Core.Tests.TestSchema.TableMaps
{
	public class SuperEmployeeMap : TableMap<SuperEmployee>
	{
		public SuperEmployeeMap()
		{
			Map(x => x.EmployeeId).Key(false);
			AutoMap();
		}
	}
}