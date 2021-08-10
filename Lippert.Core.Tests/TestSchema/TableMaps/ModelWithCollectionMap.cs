using Lippert.Core.Data;

namespace Lippert.Core.Tests.TestSchema.TableMaps
{
	public class ModelWithCollectionMap : TableMap<ModelWithCollection>
	{
		public ModelWithCollectionMap() : this(false) { }
		public ModelWithCollectionMap(bool autoMapBeforeIgnore)
		{
			if (autoMapBeforeIgnore)
			{
				AutoMap();
				Map(x => x.Users).Ignore();
			}
			else
			{
				Map(x => x.Users).Ignore();
				AutoMap();
			}
		}
	}
}