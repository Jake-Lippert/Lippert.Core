using System;
using System.Linq;
using Lippert.Core.Data.QueryBuilders.Contracts;

namespace Lippert.Core.Data.QueryBuilders
{
	public class SqlServerDeleteQueryBuilder : SqlServerQueryBuilder
	{
		public string DeleteKey<T>() => Delete(new PredicateBuilder<T>());
		public string Delete<T>(IPredicateBuilder<T> deleteBuilder)
		{
			var filterColumns = deleteBuilder.GetFilterColumns(true).ToList();

			return string.Join(Environment.NewLine,
				$"delete from {BuildTableIdentifier(deleteBuilder.TableMap)}",
				BuildWhereClause(filterColumns));
		}
	}
}