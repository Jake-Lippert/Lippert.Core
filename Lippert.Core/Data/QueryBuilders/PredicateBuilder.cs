using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Lippert.Core.Data.Contracts;
using Lippert.Core.Data.QueryBuilders.Contracts;
using Lippert.Core.Reflection;

namespace Lippert.Core.Data.QueryBuilders
{
	public class PredicateBuilder<T> : IPredicateBuilder<T>
	{
		private readonly List<IColumnMap> _filterColumns = new List<IColumnMap>();

		public ITableMap<T> TableMap { get; } = SqlServerQueryBuilder.GetTableMap<T>();

		public PredicateBuilder<T> Key()
		{
			_filterColumns.AddRange(TableMap.KeyColumns);
			return this;
		}
		public PredicateBuilder<T> Filter(Expression<Func<T, object>> column)
		{
			_filterColumns.Add(TableMap[PropertyAccessor.Get(column ?? throw new ArgumentNullException(nameof(column)))]);
			return this;
		}

		IEnumerable<IColumnMap> IPredicateBuilder<T>.GetFilterColumns(bool defaultToKey)
		{
			if (defaultToKey && !_filterColumns.Any())
			{
				return TableMap.KeyColumns;
			}

			return _filterColumns.AsEnumerable();
		}
	}
}