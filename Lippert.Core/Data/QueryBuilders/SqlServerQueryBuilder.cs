using System;
using System.Linq;
using System.Linq.Expressions;
using Lippert.Core.Data.Contracts;

namespace Lippert.Core.Data.QueryBuilders
{
	public class SqlServerQueryBuilder
    {
		public string SelectAll<T>()
		{
			var tableMap = TableMap.GetMap<T>();

			return string.Join(Environment.NewLine,
				$"select {string.Join(", ", tableMap.SelectColumns.Select(BuildColumnIdentifier))}",
				$"from {BuildTableIdentifier(tableMap)}");
		}

		public string Insert<T>()
		{
			var tableMap = TableMap.GetMap<T>();
			var insertColumns = tableMap.InsertColumns;
			var generatedColumns = tableMap.InstanceColumns.Values.Where(c => c.Behavior.HasFlag(ColumnBehavior.Generated) ||
				(c.IgnoreOperations.HasFlag(IgnoreBehavior.Insert) && !c.IgnoreOperations.HasFlag(IgnoreBehavior.Select))).ToList();

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
			$"where {string.Join(" and ", updateBuilder.FilterColumns.Select(BuildColumnEquals))}");

		public string BuildTableIdentifier<T>(ITableMap<T> table) => $"[{table.TableName}]";
		public string BuildColumnIdentifier<T>(ColumnMap<T> column) => $"[{column.ColumnName}]";
		public string BuildColumnParameter<T>(ColumnMap<T> column) => $"@{column.ColumnName}";
		public string BuildColumnEquals<T>(ColumnMap<T> column) => $"{BuildColumnIdentifier(column)} = {BuildColumnParameter(column)}";
	}
}