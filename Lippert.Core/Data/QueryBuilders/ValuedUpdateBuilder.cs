using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Lippert.Core.Data.Contracts;
using Lippert.Core.Data.QueryBuilders.Contracts;
using Lippert.Core.Reflection;

namespace Lippert.Core.Data.QueryBuilders
{
	public class ValuedUpdateBuilder<T> : ValuedPredicateBuilder<T>, IValuedUpdateBuilder<T>
	{
		private readonly List<IValuedColumnMap> _setColumns = new List<IValuedColumnMap>();

		public ValuedUpdateBuilder<T> Set<TColumn>(Expression<Func<T, TColumn>> column, TColumn value)
		{
			((IValuedUpdateBuilder<T>)this).Set(PropertyAccessor.Get(column ?? throw new ArgumentNullException(nameof(column))), value);
			return this;
		}
		void IValuedUpdateBuilder<T>.Set(System.Reflection.PropertyInfo column, object value)
		{
			var columnMap = TableMap[column ?? throw new ArgumentNullException(nameof(column))];
			if (!TableMap.UpdateColumns.Contains(columnMap))
			{
				throw new ArgumentException($"The column '{columnMap.ColumnName}' is not available for updates.", nameof(column));
			}

			_setColumns.RemoveAll(cm => cm.Property == columnMap.Property);
			_setColumns.Add(new ValuedColumnMap(columnMap, value));
		}

		public ValuedUpdateBuilder<T> Key<TKey>(TKey key)
		{
			base.Key(key);
			return this;
		}

		public new ValuedUpdateBuilder<T> Filter<TColumn>(Expression<Func<T, TColumn>> column, TColumn value)
		{
			base.Filter(column, value);
			return this;
		}

		IEnumerable<IColumnMap> IUpdateBuilder<T>.GetSetColumns() => ((IValuedUpdateBuilder<T>)this).GetSetColumns();
		IEnumerable<IValuedColumnMap> IValuedUpdateBuilder<T>.GetSetColumns() => _setColumns.AsEnumerable();
	}
}