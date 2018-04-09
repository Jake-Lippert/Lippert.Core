using Lippert.Core.Data;

namespace Lippert.Core.Tests.TestSchema.TableMaps
{
	public class UserMap : TableMap<User>
	{
		public UserMap()
		{
			Map(x => x.Id).Key();
		}
	}
}