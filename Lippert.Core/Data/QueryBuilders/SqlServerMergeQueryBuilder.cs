using System;
using System.Collections.Generic;
using System.Linq;
using Lippert.Core.Collections.Extensions;
using Lippert.Core.Data.Contracts;
using Lippert.Core.Data.QueryBuilders.MergeSerializers;
using Lippert.Core.Extensions;

namespace Lippert.Core.Data.QueryBuilders
{
	/// <summary>
	/// Builds sql server merge queries
	/// </summary>
	public class SqlServerMergeQueryBuilder : SqlServerQueryBuilder
	{
		public static string CorrelationIndexIdentifier = BuildIdentifier($"<{{{nameof(RecordMergeCorrelation.CorrelationIndex)}}}>");

		/// <summary>
		/// Use sql merge to get all of the generated values back for all rows AND be able to map them to the correct objects
		/// </summary>
		/// <param name="mergeSerializer">When this method returns, contains a <see cref="Contracts.IMergeSerializer"/> that can be used to serialize collections into what's expected by the @serialized parameter</param>
		/// <param name="mergeDefinition">Should inserts/updates/deletes be included in the merge statement?  Should there be additional filtering?</param>
		/// <param name="tableMap">Optionally specify a table map to be used to build out the query</param>
		/// <remarks>
		/// Note: The OPENJSON function is available only under compatibility level 130 or higher.
		/// If your database compatibility level is lower than 130, SQL Server can't find and run the OPENJSON function.
		/// </remarks>
		/// <seealso cref="https://docs.microsoft.com/en-us/sql/t-sql/functions/openjson-transact-sql"/>
		/// <returns>
		/// Creates sql which expects a @serialized parameter whose value is a json or xml array string; if xml, the main and child nodes are named '_'.
		/// The properties of the collection items are named _0 through _z, _10 through _zz, _100 through _zzz, and so-on.
		/// A collection item property '_' is expected, which contains the index of the item within the array.
		/// </returns>
		public string Merge<T>(out Contracts.IMergeSerializer<T> mergeSerializer, MergeDefinition<T> mergeDefinition, ITableMap<T>? tableMap = null, bool useJson = false)
		{
			//--Build a table map and figure out which columns should be serialized
			tableMap ??= GetTableMap<T>();

			//--Build a merge serializer and the core of the merge statement
			mergeSerializer = useJson ? new JsonMergeSerializer<T>(tableMap) : new XmlMergeSerializer<T>(tableMap);
			var outputColumns = BuildOutputColumns(mergeDefinition, tableMap);
			var merge = new System.Text.StringBuilder();
			merge.AppendLine("declare @outputResult table(");
			merge.AppendLine(string.Join($",{Environment.NewLine}", outputColumns.Select(column => $"  {column.columnIdentifier} {column.sqlType}")));
			merge.AppendLine(");");
			merge.AppendLines(BuildCoreMergeLines(mergeSerializer, mergeDefinition, tableMap));

			//--Build the components of the merge
			merge.AppendLines(BuildInsertLines(mergeDefinition, tableMap));
			merge.AppendLines(BuildUpdateLines(mergeDefinition, tableMap));
			merge.AppendLines(BuildDeleteLines(mergeDefinition));

			//--Build the merge's output statement so we can determine which records were inserted/updated/deleted
			merge.AppendLine($"output {string.Join(", ", outputColumns.Select(oc => oc.value))} into @outputResult({string.Join(", ", outputColumns.Select(column => column.columnIdentifier))});");
			merge.Append("select * from @outputResult;");

			return merge.ToString();
		}

		/// <summary>
		/// Build the core lines for the merge statement
		/// </summary>
		public IEnumerable<string> BuildCoreMergeLines<T>(Contracts.IMergeSerializer<T> mergeSerializer, MergeDefinition<T> mergeDefinition, ITableMap<T> tableMap)
		{
			if (mergeSerializer is XmlMergeSerializer<T>)
			{
				//--Create an internal representation of the XML document
				yield return "declare @preparedDoc int;";
				yield return "exec sp_xml_preparedocument @preparedDoc output, @serialized;";
				yield return "";
			}

			//--merge [Table] as target
			//	using (......) as source on (target.[Key] = source.[Key])
			yield return $"merge {BuildTableIdentifier(mergeSerializer.TableMap)} as target";
			yield return @$"using (select * from open{mergeSerializer switch
			{
				JsonMergeSerializer<T> _ => "Json(@serialized)",
				XmlMergeSerializer<T> _ => "Xml(@preparedDoc, '/_/_')",
				_ => throw new ArgumentException($"No data serialization defined for merge serializer type '{mergeSerializer.GetType().FullName}'.")
			}} with (";
			yield return string.Join($",{Environment.NewLine}", BuildColumnParsers());
			yield return $")) as source on ({string.Join(" and ", BuildJoinConditions())})";

			IEnumerable<string> BuildColumnParsers()
			{
				yield return FormatColumnParser(null);

				var sourceColumns = (mergeDefinition.IncludeInsert, mergeDefinition.IncludeUpdate, mergeDefinition.IncludeDelete) switch
				{
					(true, false, _) => tableMap.KeyColumns.Union(tableMap.InsertColumns).ToList(),
					(true, true, _) => tableMap.KeyColumns.Union(tableMap.UpsertColumns).ToList(),
					(false, true, _) => tableMap.KeyColumns.Union(tableMap.UpdateColumns).ToList(),
					(false, false, true) => tableMap.KeyColumns,
					_ => throw new ArgumentException("At least one of insert, update, or delete must be included in the merge."),
				};
				foreach (var sourceColumn in sourceColumns)
				{
					yield return FormatColumnParser(sourceColumn);
				}

				string FormatColumnParser(IColumnMap? columnMap) => $"  {mergeSerializer.BuildColumnParser(columnMap)}";
			}
			IEnumerable<string> BuildJoinConditions()
			{
				if (!mergeSerializer.TableMap.KeyColumns.Any())
				{
					throw new InvalidOperationException($"At least one key column must be configured for type '{mergeSerializer.TableMap.ModelType.FullName}'");
				}

				foreach (var keyColumn in mergeSerializer.TableMap.KeyColumns)
				{
					var columnIdentifier = BuildColumnIdentifier(keyColumn);
					yield return $"target.{columnIdentifier} = source.{columnIdentifier}";
				}
			}
		}
		/// <summary>
		/// Build the insert lines for the merge statement
		/// </summary>
		/// <remarks>
		/// Yields no lines if the merge definition doesn't have inserts included
		/// </remarks>
		public IEnumerable<string> BuildInsertLines<T>(MergeDefinition<T> mergeDefinition, ITableMap<T> tableMap)
		{
			if (mergeDefinition.IncludeInsert)
			{
				var insertColumns = tableMap.InsertColumns;
				yield return $"when not matched by target then insert({string.Join(", ", BuildInsertColumns())})";
				yield return $"  values({string.Join(", ", BuildInsertValueColumns())})";

				IEnumerable<string> BuildInsertColumns() => insertColumns.Select(c => BuildColumnIdentifier(c));
				IEnumerable<string> BuildInsertValueColumns() => insertColumns.Select(c => $"source.{BuildColumnIdentifier(c)}");
			}
		}
		/// <summary>
		/// Build the update lines for the merge statement
		/// </summary>
		/// <remarks>
		/// Yields no lines if the merge definition doesn't have updates included
		/// </remarks>
		public IEnumerable<string> BuildUpdateLines<T>(MergeDefinition<T> mergeDefinition, ITableMap<T> tableMap)
		{
			if (mergeDefinition.IncludeUpdate)
			{
				if (!tableMap.UpdateColumns.Any())
				{
					throw new InvalidOperationException($"At least one update column must be configured for type '{tableMap.ModelType.FullName}'");
				}

				yield return "when matched then update set";
				yield return $"{string.Join($",{Environment.NewLine}", BuildUpdateValueAssignments())}";
			}

			IEnumerable<string> BuildUpdateValueAssignments() => tableMap.UpdateColumns.Select(c => BuildColumnIdentifier(c).With(ci => $"  target.{ci} = source.{ci}"));
		}
		/// <summary>
		/// Build the delete lines for the merge statement
		/// </summary>
		/// <remarks>
		/// Yields no lines if the merge definition doesn't have deletes included
		/// </remarks>
		public IEnumerable<string> BuildDeleteLines<T>(MergeDefinition<T> mergeDefinition)
		{
			if (mergeDefinition.IncludeDelete)
			{
				yield return $"when not matched by source{string.Join("", mergeDefinition.GetDeleteFilterColumns().Select(BuildColumnFilter))} then delete";

				static string BuildColumnFilter(IValuedColumnMap valuedColumnMap, int index) => valuedColumnMap switch
				{
					{ Value: null } => $" and target.{BuildColumnIdentifier(valuedColumnMap)} is null",
					_ => $" and target.{BuildColumnIdentifier(valuedColumnMap)} = @deleteFilter{index}"
				};
			}
		}
		/// <summary>
		/// Build the output columns for the merge statement
		/// </summary>
		public IEnumerable<(string value, string columnIdentifier, string sqlType)> BuildOutputColumns<T>(MergeDefinition<T> mergeDefinition, ITableMap<T> tableMap)
		{
			var correlation = new ColumnMap<RecordMergeCorrelation>(x => x.CorrelationIndex);
			yield return ((mergeDefinition.IncludeInsert, mergeDefinition.IncludeUpdate, mergeDefinition.IncludeDelete) switch
			{
				(false, false, true) => "null",//--If this is a Delete-only merge statement 'source' won't be able to be bound, so just use null
				_ => $"source.{CorrelationIndexIdentifier}"
			}, BuildColumnIdentifier(correlation), correlation.GetSqlType());
			var action = new ColumnMap<RecordMergeCorrelation>(x => x.Action);
			yield return ("$action", BuildColumnIdentifier(action), action.GetSqlType());
			yield return ("null", BuildIdentifier(SplitOn), "bit");

			foreach (var column in tableMap.InstanceColumns.Values.Where(ic => ic.IgnoreOperations != (SqlOperation.Insert | SqlOperation.Update | SqlOperation.Select)))
			{
				var columnIdentifier = BuildColumnIdentifier(column);
				yield return ((mergeDefinition.IncludeInsert, mergeDefinition.IncludeUpdate, mergeDefinition.IncludeDelete) switch
				{
					(false, false, true) => $"deleted.{columnIdentifier}",
					(_, _, true) => $"coalesce(inserted.{columnIdentifier}, deleted.{columnIdentifier})",
					(_, _, false) => $"inserted.{columnIdentifier}"
				}, columnIdentifier, column.GetSqlType());
			}
		}


		public class RecordMergeCorrelation
		{
			public int? CorrelationIndex { get; set; }
			public string Action { get; set; } = string.Empty;
		}
	}
}