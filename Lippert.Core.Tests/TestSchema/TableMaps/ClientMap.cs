using Lippert.Core.Data;

namespace Lippert.Core.Tests.TestSchema.TableMaps
{
	public class ClientMap : TableMap<Client>
	{
		public ClientMap()
		{
			AutoMap();
		}
	}
}