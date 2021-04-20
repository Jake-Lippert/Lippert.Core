using Lippert.Core.Data;

namespace Lippert.Core.Tests.TestSchema.TableMaps
{
	public class PurchaseMap : TableMap<Purchase>
	{
		public PurchaseMap()
		{
			Map(x => x.Name, 20);
			Map(x => x.Cost, 10, 2);
		}
	}
}