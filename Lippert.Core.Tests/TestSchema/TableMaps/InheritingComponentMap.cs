using Lippert.Core.Data;

namespace Lippert.Core.Tests.TestSchema.TableMaps
{
	public class InheritingComponentMap : TableMap<InheritingComponent>
    {
		public InheritingComponentMap()
		{
			Map(x => x.BaseId).Ignore(SqlOperation.Update);
			AutoMap(x => x.Category, x => x.Cost);
		}
    }
}