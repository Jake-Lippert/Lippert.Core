﻿using System;
using Lippert.Core.Data;
using Lippert.Core.Data.Contracts;
using Lippert.Core.Tests.TestSchema.Contracts;

namespace Lippert.Core.Tests.TestSchema.TableMaps.Builders
{
	public class EditFieldsMapBuilder : TableMapBuilder<IEditFields>
	{
		public override void Map<TRecord>(ITableMap<TRecord> tableMap)
		{
			tableMap.Map(x => x.ModifiedByUserId);
			tableMap.Map(x => x.ModifiedDateUtc).Generated(allowUpdates: true);
		}


		public override void SetInsertValues(IEditFields component)
		{
			component.ModifiedByUserId = ClaimsProvider.UserClaims.UserId;
		}

		public override void SetUpdateValues(IEditFields component)
		{
			component.ModifiedByUserId = ClaimsProvider.UserClaims.UserId;
			component.ModifiedDateUtc = DateTime.UtcNow;
		}
	}
}