using System;
using System.Linq;
using System.Linq.Expressions;
using Lippert.Core.Collections.Extensions;
using Lippert.Core.Data.QueryBuilders.Contracts;

namespace Lippert.Core.Data.QueryBuilders
{
	public class SqlServerUpdateQueryBuilder : SqlServerQueryBuilder
	{
		public string Update<T>() => Update(new UpdateBuilder<T>());
		public string Update<T>(params Expression<Func<T, object?>>[] selectiveUpdateColumns) =>
			Update(selectiveUpdateColumns.Aggregate(new UpdateBuilder<T>(), (builder, column) => builder.Set(column)));
		public string Update<T>(IUpdateBuilder<T> updateBuilder)
		{
			var filterColumns = updateBuilder.GetFilterColumns(true).ToList();
			var setColumns = updateBuilder.GetSetColumns().ToList();
			var underscoreRequired = filterColumns.Select(fc => fc.ColumnName).Intersect(setColumns.Select(sc => sc.ColumnName)).Any();

			return string.Join(Environment.NewLine,
				$"update {BuildTableIdentifier(updateBuilder.TableMap)}",
				$"set {string.Join(", ", setColumns.Select(sc => BuildColumnEquals(sc, prependUnderscore: underscoreRequired)))}",
				BuildWhereClause(filterColumns));
		}
	}
}