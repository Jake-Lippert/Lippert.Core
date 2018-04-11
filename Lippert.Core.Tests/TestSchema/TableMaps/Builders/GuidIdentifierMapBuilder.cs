using Lippert.Core.Data;
using Lippert.Core.Data.Contracts;
using Lippert.Core.Tests.TestSchema.Contracts;

namespace Lippert.Core.Tests.TestSchema.TableMaps.Builders
{
	public class GuidIdentifierMapBuilder : TableMapBuilder<IGuidIdentifier>
	{
		public override void Map<TRecord>(ITableMap<TRecord> tableMap) => tableMap.Map(x => x.Id).Key();
	}
}