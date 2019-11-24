using System.Collections.Generic;
using System.Reflection;
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


		public override List<(PropertyInfo column, object? value)> GetInsertValues() => new List<(PropertyInfo column, object? value)>
		{
			SetValue(x => x.ClientId, ClaimsProvider.UserClaims.ClientId)
		};
	}
}