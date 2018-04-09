using Lippert.Core.Data;

namespace Lippert.Core.Tests.TestSchema.TableMaps
{
	public class ClientMap : CommonColumnMapBase<Client>
	{
		public ClientMap(bool useBase)
		{
			if (useBase)
			{
				MapProperties();
			}
			else
			{
				Map(x => x.CreatedByUserId).Ignore(IgnoreBehavior.Update);
				Map(x => x.CreatedDateUtc).Generated();
				Map(x => x.ModifiedByUserId);
				Map(x => x.ModifiedDateUtc).Ignore(IgnoreBehavior.Insert);
			}

			AutoMap();
		}
	}
}