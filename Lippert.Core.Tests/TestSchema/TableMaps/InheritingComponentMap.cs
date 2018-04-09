using Lippert.Core.Data;

namespace Lippert.Core.Tests.TestSchema.TableMaps
{
	public class InheritingComponentMap : TableMap<InheritingComponent>
    {
		public InheritingComponentMap()
		{
			Map(x => x.Id).Key();
			Map(x => x.BaseId).Ignore(IgnoreBehavior.Update);
			AutoMap(x => x.Category, x => x.Cost);
		}
    }
}