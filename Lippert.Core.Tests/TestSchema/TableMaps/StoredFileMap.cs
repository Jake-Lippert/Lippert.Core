using Lippert.Core.Data;

namespace Lippert.Core.Tests.TestSchema.TableMaps
{
	public class StoredFileMap : TableMap<StoredFile>
    {
		public StoredFileMap()
		{
			Map(x => x.Name, 100);
			Map(x => x.FileBytes, int.MaxValue);
			AutoMap();
		}
    }
}