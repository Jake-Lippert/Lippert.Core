using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Lippert.Core.Data.Contracts;

namespace Lippert.Core.Data.QueryBuilders
{
	public class UpdateBuilder<T> : PredicateBuilder<T>
	{
		private readonly List<IColumnMap> _setColumns = new List<IColumnMap>();
		
		public List<IColumnMap> SetColumns => _setColumns.Any() ? _setColumns : TableMap.UpdateColumns;
		public UpdateBuilder<T> Set(Expression<Func<T, object>> column)
		{
			var columnMap = TableMap[column ?? throw new ArgumentNullException(nameof(column))];
			if (!TableMap.UpdateColumns.Contains(columnMap))
			{
				throw new ArgumentException($"The column '{columnMap.ColumnName}' is not available for updates.", nameof(column));
			}

			_setColumns.Add(columnMap);
			return this;
		}
		public UpdateBuilder<T> Set(Expression<Func<T, object>> column, object value)
		{
			var columnMap = TableMap[column ?? throw new ArgumentNullException(nameof(column))];
			if (!TableMap.UpdateColumns.Contains(columnMap))
			{
				throw new ArgumentException($"The column '{columnMap.ColumnName}' is not available for updates.", nameof(column));
			}

			_setColumns.Add(new ValuedColumnMap(columnMap, value));
			return this;
		}

		public new UpdateBuilder<T> Key()
		{
			base.Key();
			return this;
		}
		public UpdateBuilder<T> Key<TKey>(TKey key)
		{
			base.Key(key);
			return this;
		}

		public new UpdateBuilder<T> Filter(Expression<Func<T, object>> column, object value = default)
		{
			base.Filter(column, value);
			return this;
		}

		public bool UnderscoreRequired => GetFilterColumns(true).Select(fc => fc.ColumnName).Intersect(SetColumns.Select(sc => sc.ColumnName)).Any();
	}
}