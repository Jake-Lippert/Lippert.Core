using System;
using System.Linq;
using Lippert.Core.Collections.Extensions;
using Lippert.Core.Data.QueryBuilders.Contracts;

namespace Lippert.Core.Data.QueryBuilders
{
	public class SqlServerSelectQueryBuilder : SqlServerQueryBuilder
	{
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
	}
}