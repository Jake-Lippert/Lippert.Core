using System;
using System.Collections.Generic;
using System.Linq;
using Lippert.Core.Collections;
using Lippert.Core.Collections.Extensions;
using Lippert.Core.Data.Contracts;

namespace Lippert.Core.Data.QueryBuilders
{
	public abstract class SqlServerQueryBuilder
	{
		protected static readonly IDictionary<Type, ITableMap> _tableMaps;

		static SqlServerQueryBuilder()
		{
			_tableMaps = RetrievalDictionary.Build((Type type) => TableMapSource.GetTableMap(type));
			_tableMaps.AddRange(TableMapSource.GetTableMaps());
		}

		public static ITableMap<T> GetTableMap<T>() => (ITableMap<T>)_tableMaps[typeof(T)];


		public static string SplitOn = "<{Split}>";
		public static string BuildIdentifier(string name) => $"[{name}]";
		public static string BuildTableIdentifier(ITableMap table) => BuildIdentifier(table.TableName);
		public static string BuildColumnIdentifier(IColumnMap column) => BuildIdentifier(column.ColumnName);
		public static string BuildColumnParameter(IColumnMap column, bool prependUnderscore = false) => $"@{(prependUnderscore ? "_" : null)}{column.ColumnName}";
		public static string BuildColumnEquals(IColumnMap column, bool prependUnderscore = false) => $"{BuildColumnIdentifier(column)} = {BuildColumnParameter(column, prependUnderscore)}";
		public static string BuildWhereClause(IEnumerable<IColumnMap> columns) => $"where {string.Join(" and ", columns.Select(c => BuildColumnEquals(c)))}";
	}
}