using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Lippert.Core.Data.Contracts;

namespace Lippert.Core.Data.QueryBuilders
{
	public class UpdateBuilder<T>
	{
		private readonly List<IColumnMap> _setColumns = new List<IColumnMap>(), _filterColumns = new List<IColumnMap>();

		internal ITableMap<T> TableMap { get; } = SqlServerQueryBuilder.GetTableMap<T>();

		internal List<IColumnMap> SetColumns => _setColumns.Any() ? _setColumns : TableMap.UpdateColumns;
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

		internal List<IColumnMap> FilterColumns => _filterColumns.Any() ? _filterColumns : TableMap.KeyColumns;
		public UpdateBuilder<T> Filter(Expression<Func<T, object>> column)
		{
			_filterColumns.Add(TableMap[column ?? throw new ArgumentNullException(nameof(column))]);
			return this;
		}
	}
}