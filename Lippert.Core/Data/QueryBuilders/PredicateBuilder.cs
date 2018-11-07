using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Lippert.Core.Data.Contracts;
using Lippert.Core.Reflection;

namespace Lippert.Core.Data.QueryBuilders
{
	public class PredicateBuilder<T>
	{
		protected readonly List<IColumnMap> _filterColumns = new List<IColumnMap>();

		internal ITableMap<T> TableMap { get; } = SqlServerQueryBuilder.GetTableMap<T>();

		public PredicateBuilder<T> Key()
		{
			_filterColumns.AddRange(TableMap.KeyColumns);
			return this;
		}
		public PredicateBuilder<T> Key(object key)
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

		public PredicateBuilder<T> Filter<TColumn>(Expression<Func<T, TColumn>> column)
		{
			_filterColumns.Add(TableMap[PropertyAccessor.Get(column ?? throw new ArgumentNullException(nameof(column)))]);
			return this;
		}
		public PredicateBuilder<T> Filter<TColumn>(Expression<Func<T, TColumn>> column, TColumn value)
		{
			_filterColumns.Add(new ValuedColumnMap(TableMap[PropertyAccessor.Get(column ?? throw new ArgumentNullException(nameof(column)))], value));
			return this;
		}

		public List<IColumnMap> GetFilterColumns(bool defaultToKey)
		{
			if (defaultToKey && !_filterColumns.Any())
			{
				return TableMap.KeyColumns;
			}

			return _filterColumns;
		}
	}
}