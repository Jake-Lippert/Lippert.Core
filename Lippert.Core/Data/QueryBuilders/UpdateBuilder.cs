using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Lippert.Core.Data.Contracts;
using Lippert.Core.Reflection;

namespace Lippert.Core.Data.QueryBuilders
{
	public class UpdateBuilder<T> : PredicateBuilder<T>
	{
		private readonly List<IColumnMap> _setColumns = new List<IColumnMap>();
		
		public List<IColumnMap> SetColumns => _setColumns.Any() ? _setColumns : TableMap.UpdateColumns;
		public UpdateBuilder<T> Set<TColumn>(Expression<Func<T, TColumn>> column) => Set(column, default, false);
		public UpdateBuilder<T> Set<TColumn>(Expression<Func<T, TColumn>> column, TColumn value) => Set(column, value, true);
		private UpdateBuilder<T> Set<TColumn>(Expression<Func<T, TColumn>> column, TColumn value, bool includeValue) =>
			Set(PropertyAccessor.Get(column ?? throw new ArgumentNullException(nameof(column))), value, includeValue);
		internal UpdateBuilder<T> Set(PropertyInfo column, object value, bool includeValue)
		{
			var columnMap = TableMap[column ?? throw new ArgumentNullException(nameof(column))];
			if (!TableMap.UpdateColumns.Contains(columnMap))
			{
				throw new ArgumentException($"The column '{columnMap.ColumnName}' is not available for updates.", nameof(column));
			}

			_setColumns.RemoveAll(cm => cm.Property == columnMap.Property);
			_setColumns.Add(includeValue ? new ValuedColumnMap(columnMap, value) : columnMap);
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

		public new UpdateBuilder<T> Filter<TColumn>(Expression<Func<T, TColumn>> column, TColumn value = default)
		{
			base.Filter(column, value);
			return this;
		}

		public bool UnderscoreRequired => GetFilterColumns(true).Select(fc => fc.ColumnName).Intersect(SetColumns.Select(sc => sc.ColumnName)).Any();
	}
}