using Lippert.Core.Data;

namespace Lippert.Core.Tests.TestSchema.TableMaps
{
	public class LargeRecordMap : TableMap<LargeRecord>
	{
		public LargeRecordMap()
		{
			Map(x => x.IdA).Key(false);
			Map(x => x.IdB).Key(false);
			AutoMap();
		}
	}
}