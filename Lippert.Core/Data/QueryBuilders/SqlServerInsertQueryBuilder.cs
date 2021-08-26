using System;
using System.Linq;
using Lippert.Core.Collections.Extensions;
using Lippert.Core.Data.Contracts;

namespace Lippert.Core.Data.QueryBuilders
{
	public class SqlServerInsertQueryBuilder : SqlServerQueryBuilder
	{
		public string Insert<T>() => Insert(GetTableMap<T>());
		public string Insert<T>(ITableMap<T> tableMap)
		{
			var insertColumns = tableMap.InsertColumns;
			var insert = $"insert into {BuildTableIdentifier(tableMap)}({string.Join(", ", insertColumns.Select(BuildColumnIdentifier))})";
			var values = $"values({string.Join(", ", insertColumns.Select(ic => BuildColumnParameter(ic)))});";

			var generatedColumns = tableMap.GeneratedColumns;
			if (generatedColumns.Any())
			{
				return string.Join(Environment.NewLine,
					"declare @outputResult table(",
					string.Join($",{Environment.NewLine}", generatedColumns.Select(c => $"  {BuildColumnIdentifier(c)} {c.GetSqlType()}")),
					");",
					insert,
					$"output {string.Join(", ", generatedColumns.Select(c => $"inserted.{BuildColumnIdentifier(c)}"))} into @outputResult({string.Join(", ", generatedColumns.Select(c => BuildColumnIdentifier(c)))})",
					values,
					"select * from @outputResult;");
			}
			else
			{
				return string.Join(Environment.NewLine, insert, values);
			}
		}
	}
}