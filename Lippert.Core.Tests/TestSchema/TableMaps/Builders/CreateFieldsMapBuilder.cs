using Lippert.Core.Data;
using Lippert.Core.Data.Contracts;
using Lippert.Core.Tests.TestSchema.Contracts;

namespace Lippert.Core.Tests.TestSchema.TableMaps.Builders
{
	public class CreateFieldsMapBuilder : TableMapBuilder<ICreateFields>
	{
		public override void Map<TRecord>(ITableMap<TRecord> tableMap)
		{
			tableMap.Map(x => x.CreatedByUserId).Ignore(SqlOperation.Update);
			tableMap.Map(x => x.CreatedDateUtc).Generated();
		}
	}
}