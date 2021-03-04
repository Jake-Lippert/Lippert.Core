using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Lippert.Core.Data.Contracts;
using Lippert.Core.Data.QueryBuilders.Contracts;

namespace Lippert.Core.Data.QueryBuilders
{
	public class UpdateBuilder<T> : PredicateBuilder<T>, IUpdateBuilder<T>
	{
		private readonly List<IColumnMap> _setColumns = new List<IColumnMap>();
		
		public UpdateBuilder<T> Set(Expression<Func<T, object?>> column)
		{
			var columnMap = TableMap[column ?? throw new ArgumentNullException(nameof(column))];
			if (!TableMap.UpdateColumns.Contains(columnMap))
			{
				throw new ArgumentException($"The column '{columnMap.ColumnName}' is not available for updates.", nameof(column));
			}

			_setColumns.RemoveAll(cm => cm.Property == columnMap.Property);
			_setColumns.Add(columnMap);
			return this;
		}

		public new UpdateBuilder<T> Key()
		{
			base.Key();
			return this;
		}

		public new UpdateBuilder<T> Filter(Expression<Func<T, object?>> column)
		{
			base.Filter(column);
			return this;
		}

		IEnumerable<IColumnMap> IUpdateBuilder<T>.GetSetColumns() => (_setColumns.Any() ? _setColumns : TableMap.UpdateColumns).AsEnumerable();
	}
}