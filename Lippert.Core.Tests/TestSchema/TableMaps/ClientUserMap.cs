using Lippert.Core.Data;

namespace Lippert.Core.Tests.TestSchema.TableMaps
{
	public class ClientUserMap : TableMap<ClientUser>
	{
		public ClientUserMap()
		{
			Table("Client_User");
			Map(x => x.ClientId).Key(isGenerated: false);
			Map(x => x.UserId).Key(isGenerated: false);
			AutoMap();
		}
	}
}