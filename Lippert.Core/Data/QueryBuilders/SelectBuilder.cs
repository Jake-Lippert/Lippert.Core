using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lippert.Core.Data.Contracts;

namespace Lippert.Core.Data.QueryBuilders
{
	public class SelectBuilder<T>
	{
		internal ITableMap<T> TableMap { get; } = SqlServerQueryBuilder.GetTableMap<T>();

		internal List<IColumnMap> FilterColumns { get; } = new List<IColumnMap>();
		public SelectBuilder<T> Filter(Expression<Func<T, object>> column)
		{
			FilterColumns.Add(TableMap[column ?? throw new ArgumentNullException(nameof(column))]);
			return this;
		}

		public SelectBuilder<T> Key()
		{
			FilterColumns.AddRange(TableMap.KeyColumns);
			return this;
		}
	}
}