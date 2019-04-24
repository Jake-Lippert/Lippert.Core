using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Lippert.Core.Data.Contracts;
using Lippert.Core.Data.QueryBuilders.Contracts;
using Lippert.Core.Reflection;

namespace Lippert.Core.Data.QueryBuilders
{
	public class ValuedPredicateBuilder<T> : IValuedPredicateBuilder<T>
	{
		private readonly List<IValuedColumnMap> _filterColumns = new List<IValuedColumnMap>();

		public ITableMap<T> TableMap { get; } = SqlServerQueryBuilder.GetTableMap<T>();

		public ValuedPredicateBuilder<T> Key(object key)
		{
			var keyType = key.GetType();
			if (TableMap.ModelType.IsAssignableFrom(keyType))
			{
				foreach (var keyColumn in TableMap.KeyColumns)
				{
					_filterColumns.Add(new ValuedColumnMap(keyColumn, keyColumn.Property.GetValue(key)));
				}
			}
			else if (keyType.GetProperties().Any())
			{
				foreach (var keyColumn in TableMap.KeyColumns)
				{
					var keyProperty = keyType.GetProperty(keyColumn.Property.Name, BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.NonPublic);
					_filterColumns.Add(new ValuedColumnMap(keyColumn, keyProperty.GetValue(key)));
				}
			}
			else
			{
				var keyColumn = TableMap.KeyColumns.Single();
				_filterColumns.Add(new ValuedColumnMap(keyColumn, key));
			}

			return this;
		}

		public ValuedPredicateBuilder<T> Filter<TColumn>(Expression<Func<T, TColumn>> column, TColumn value)
		{
			_filterColumns.Add(new ValuedColumnMap(TableMap[PropertyAccessor.Get(column ?? throw new ArgumentNullException(nameof(column)))], value));
			return this;
		}

		IEnumerable<IColumnMap> IPredicateBuilder<T>.GetFilterColumns(bool defaultToKey) => ((IValuedPredicateBuilder<T>)this).GetFilterColumns();

		IEnumerable<IValuedColumnMap> IValuedPredicateBuilder<T>.GetFilterColumns() => _filterColumns.AsEnumerable();
	}
}