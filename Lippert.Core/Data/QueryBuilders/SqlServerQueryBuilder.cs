using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Lippert.Core.Collections;
using Lippert.Core.Collections.Extensions;
using Lippert.Core.Data.Contracts;
using Lippert.Core.Data.QueryBuilders.Contracts;

namespace Lippert.Core.Data.QueryBuilders
{
	public class SqlServerQueryBuilder
	{
		private static readonly IDictionary<Type, ITableMap> _tableMaps;

		static SqlServerQueryBuilder()
		{
			_tableMaps = RetrievalDictionary.Build((Type type) => TableMapSource.GetTableMap(type));
			_tableMaps.AddRange(TableMapSource.GetTableMaps(), x => x.ModelType);
		}

		public static ITableMap<T> GetTableMap<T>() => (ITableMap<T>)_tableMaps[typeof(T)];


		public string SelectByKey<T>() => Select(new PredicateBuilder<T>().Key());
		public string SelectAll<T>() => Select(new PredicateBuilder<T>());
		public string Select<T>(IPredicateBuilder<T> selectBuilder)
		{
			var select = $"select {string.Join(", ", selectBuilder.TableMap.SelectColumns.Select(BuildColumnIdentifier))}";
			var from = $"from {BuildTableIdentifier(selectBuilder.TableMap)}";

			var filterColumns = selectBuilder.GetFilterColumns(false).ToList();
			if (filterColumns.Any())
			{
				return string.Join(Environment.NewLine, select, from, BuildWhereClause(filterColumns));
			}
			else
			{
				return string.Join(Environment.NewLine, select, from);
			}
		}

		public string Insert<T>() => Insert(GetTableMap<T>());
		public string Insert<T>(ITableMap<T> tableMap)
		{
			var insertColumns = tableMap.InsertColumns;
			var generatedColumns = tableMap.GeneratedColumns;

			var insert = $"insert into {BuildTableIdentifier(tableMap)}({string.Join(", ", insertColumns.Select(BuildColumnIdentifier))})";
			var values = $"values({string.Join(", ", insertColumns.Select(ic => BuildColumnParameter(ic)))})";
			
			if (generatedColumns.Any())
			{
				return string.Join(Environment.NewLine,
					insert,
					$"output {string.Join(", ", generatedColumns.Select(c => $"inserted.{BuildColumnIdentifier(c)}"))}",
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
		public string Update<T>(IUpdateBuilder<T> updateBuilder)
		{
			var filterColumns = updateBuilder.GetFilterColumns(true).ToList();
			var setColumns = updateBuilder.GetSetColumns().ToList();
			var underscoreRequired = filterColumns.Select(fc => fc.ColumnName).Intersect(setColumns.Select(sc => sc.ColumnName)).Any();

			return string.Join(Environment.NewLine,
				$"update {BuildTableIdentifier(updateBuilder.TableMap)}",
				$"set {string.Join(", ", setColumns.Select(sc => BuildColumnEquals(sc, underscoreRequired)))}",
				BuildWhereClause(filterColumns));
		}

		public string DeleteKey<T>() => Delete(new PredicateBuilder<T>());
		public string Delete<T>(IPredicateBuilder<T> deleteBuilder)
		{
			var filterColumns = deleteBuilder.GetFilterColumns(true).ToList();

			return string.Join(Environment.NewLine,
				$"delete from {BuildTableIdentifier(deleteBuilder.TableMap)}",
				BuildWhereClause(filterColumns));
		}

		public string BuildTableIdentifier(ITableMap table) => $"[{table.TableName}]";
		public string BuildColumnIdentifier(IColumnMap column) => $"[{column.ColumnName}]";
		public string BuildColumnParameter(IColumnMap column, bool prependUnderscore = false) => $"@{(prependUnderscore ? "_" : null)}{column.ColumnName}";
		public string BuildColumnEquals(IColumnMap column, bool prependUnderscore = false) => $"{BuildColumnIdentifier(column)} = {BuildColumnParameter(column, prependUnderscore)}";
		public string BuildWhereClause(IEnumerable<IColumnMap> columns) => $"where {string.Join(" and ", columns.Select(c => BuildColumnEquals(c)))}";
	}
}