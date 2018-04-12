using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Lippert.Core.Collections;
using Lippert.Core.Collections.Extensions;
using Lippert.Core.Data.Contracts;

namespace Lippert.Core.Data.QueryBuilders
{
	public class SqlServerQueryBuilder
	{
		private static readonly IDictionary<Type, ITableMap> _tableMaps;

		static SqlServerQueryBuilder()
		{
			_tableMaps = RetrievalDictionary.Build((Type type) => TableMapSource.GetTableMap(type));
			_tableMaps.AddRange(TableMapSource.GetTableMaps(), x => x.GetModelType());
		}

		public static ITableMap<T> GetTableMap<T>() => (ITableMap<T>)_tableMaps[typeof(T)];


		public string SelectByKey<T>() => Select(new SelectBuilder<T>().Key());
		public string SelectAll<T>() => Select(new SelectBuilder<T>());
		public string Select<T>(SelectBuilder<T> selectBuilder)
		{
			var tableMap = GetTableMap<T>();

			var select = $"select {string.Join(", ", tableMap.SelectColumns.Select(BuildColumnIdentifier))}";
			var from = $"from {BuildTableIdentifier(tableMap)}";

			if (selectBuilder.FilterColumns.Any())
			{
				return string.Join(Environment.NewLine, select, from, BuildWhereClause(selectBuilder.FilterColumns));
			}
			else
			{
				return string.Join(Environment.NewLine, select, from);
			}
		}

		public string Insert<T>()
		{
			var tableMap = GetTableMap<T>();
			var insertColumns = tableMap.InsertColumns;
			var generatedColumns = tableMap.GeneratedColumns;

			var insert = $"insert into {BuildTableIdentifier(tableMap)}({string.Join(", ", insertColumns.Select(BuildColumnIdentifier))})";
			var values = $"values({string.Join(", ", insertColumns.Select(BuildColumnParameter))})";
			
			if (generatedColumns.Any())
			{
				return string.Join(Environment.NewLine,
					insert,
					$"ouput {string.Join(", ", generatedColumns.Select(c => $"inserted.{BuildColumnIdentifier(c)}"))}",
					values);
			}
			else
			{
				return string.Join(Environment.NewLine, insert, values);
			}
		}

		public string Update<T>() => Update(new UpdateBuilder<T>());
		public string Update<T>(params Expression<Func<T, object>>[] selectiveUpdateColumns) =>
			Update(selectiveUpdateColumns.Aggregate(new UpdateBuilder<T>(), (builder, column) => builder.Set(column)));
		public string Update<T>(UpdateBuilder<T> updateBuilder) => string.Join(Environment.NewLine,
			$"update {BuildTableIdentifier(updateBuilder.TableMap)}",
			$"set {string.Join(", ", updateBuilder.SetColumns.Select(BuildColumnEquals))}",
			BuildWhereClause(updateBuilder.FilterColumns));

		public string BuildTableIdentifier(ITableMap table) => $"[{table.TableName}]";
		public string BuildColumnIdentifier(IColumnMap column) => $"[{column.ColumnName}]";
		public string BuildColumnParameter(IColumnMap column) => $"@{column.ColumnName}";
		public string BuildColumnEquals(IColumnMap column) => $"{BuildColumnIdentifier(column)} = {BuildColumnParameter(column)}";
		public string BuildWhereClause(IEnumerable<IColumnMap> columns) => $"where {string.Join(" and ", columns.Select(BuildColumnEquals))}";
	}
}