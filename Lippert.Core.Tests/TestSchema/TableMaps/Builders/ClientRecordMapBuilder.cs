using Lippert.Core.Data;
using Lippert.Core.Data.Contracts;
using Lippert.Core.Tests.TestSchema.Contracts;

namespace Lippert.Core.Tests.TestSchema.TableMaps.Builders
{
	public class ClientRecordMapBuilder : TableMapBuilder<IClientRecord>
	{
		public override void Map<TRecord>(ITableMap<TRecord> tableMap)
		{
			tableMap.Map(x => x.ClientId).Ignore(SqlOperation.Update);
		}


		public override void SetInsertValues(IClientRecord component)
		{
			component.ClientId = ClaimsProvider.UserClaims.ClientId;
		}
	}
}