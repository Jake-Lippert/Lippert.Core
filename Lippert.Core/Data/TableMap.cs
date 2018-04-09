using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Lippert.Core.Collections.Extensions;
using Lippert.Core.Data.Contracts;
using Lippert.Core.Reflection;

namespace Lippert.Core.Data
{
	public static class TableMap
	{
		private static readonly Dictionary<Type, ITableMap> _tableMaps = new Dictionary<Type, ITableMap>();

		public static void SetMap<T>(ITableMap<T> tableMap)
		{
			if (tableMap != null)
			{
				_tableMaps[typeof(T)] = tableMap;
			}
		}

		public static ITableMap<T> GetMap<T>() =>
			_tableMaps.TryGetValue(typeof(T), out var map) && map is Contracts.ITableMap<T> tableMap ? tableMap : default;
	}

	public abstract class TableMap<T> : ITableMap<T>
	{
		private static readonly Regex _validName = new Regex(@"^[A-Za-z][A-Za-z_0-9]*$");

		public TableMap()
		{
			Table(typeof(T).Name);
			TypeColumns = new ReadOnlyDictionary<Type, Dictionary<PropertyInfo, ColumnMap<T>>>(GetTypes(typeof(T)).Distinct()
				.ToDictionary(t => t, t => new Dictionary<PropertyInfo, ColumnMap<T>>()));

			List<Type> GetTypes(Type type)
			{
				var types = new List<Type>();
				if (type != typeof(object))
				{
					types.Add(type);
					types.AddRange(GetTypes(type.BaseType));
					types.AddRange(type.GetInterfaces());
				}

				return types;
			}
		}

		public string TableName { get; private set; }
		public void Table(string name)
		{
			if (!_validName.IsMatch(name ?? throw new ArgumentNullException(nameof(name))))
			{
				throw new ArgumentException($"The table name '{name}' is not valid.", nameof(name));
			}

			TableName = name;
		}

		public ReadOnlyDictionary<Type, Dictionary<PropertyInfo, ColumnMap<T>>> TypeColumns { get; }
		public Dictionary<PropertyInfo, ColumnMap<T>> InstanceColumns => TypeColumns[typeof(T)];
		public List<ColumnMap<T>> InsertColumns => InstanceColumns.Values.Where(c => !c.IgnoreOperations.HasFlag(IgnoreBehavior.Insert)).ToList();
		public List<ColumnMap<T>> UpdateColumns => InstanceColumns.Values.Where(c => !c.IgnoreOperations.HasFlag(IgnoreBehavior.Update)).ToList();
		public List<ColumnMap<T>> SelectColumns => InstanceColumns.Values.Where(c => !c.IgnoreOperations.HasFlag(IgnoreBehavior.Select)).ToList();
		public List<ColumnMap<T>> KeyColumns => InstanceColumns.Values.Where(c => c.Behavior.HasFlag(ColumnBehavior.Key)).ToList();

		public ColumnMap<T> Map(Expression<Func<T, object>> column) =>
			Map(new ColumnMap<T>(column ?? throw new ArgumentNullException(nameof(column))));
		public ColumnMap<T> Map(ColumnMap<T> columnMap)
		{
			foreach (var (type, columns) in TypeColumns.AsTuples())
			{
				if (type.IsInterface ^ columnMap.Property.DeclaringType.IsInterface)
				{
					if (PropertyAccessor.Get(columnMap.Property, type) is PropertyInfo targetProperty)
					{
						TypeColumns[type].Add(targetProperty, columnMap);
					}
				}
				else if (columnMap.Property.DeclaringType.IsAssignableFrom(type))
				{
					TypeColumns[type].Add(columnMap.Property, columnMap);
				}
			}

			return columnMap;
		}

		public ColumnMap<T> this[Expression<Func<T, object>> column] => this[PropertyAccessor.Get(column ?? throw new ArgumentNullException(nameof(column)))];
		public ColumnMap<T> this[PropertyInfo property] => TypeColumns[(property ?? throw new ArgumentNullException(nameof(property))).DeclaringType][property];

		public void AutoMap() => AutoMap(map => { });
		public void AutoMap(params Expression<Func<T, object>>[] includedColumns)
		{
			foreach (var includedColumn in includedColumns)
			{
				Map(includedColumn);
			}
		}
		public void AutoMap(Action<ColumnMap<T>> configureColumn)
		{
			foreach (var property in typeof(T).GetProperties().Except(InstanceColumns.Select(c => c.Key)))
			{
				configureColumn(Map(new ColumnMap<T>(property)));
			}
		}
	}
}