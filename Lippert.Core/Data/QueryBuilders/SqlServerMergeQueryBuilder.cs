using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lippert.Core.Collections.Extensions;
using Lippert.Core.Data.Contracts;
using Lippert.Core.Data.QueryBuilders.MergeSerializers;
using Lippert.Core.Extensions;

namespace Lippert.Core.Data.QueryBuilders
{
	public class SqlServerMergeQueryBuilder : SqlServerQueryBuilder
	{
		public static string CorrelationIndexIdentifier = BuildIdentifier($"<{{{nameof(RecordMergeCorrelation.CorrelationIndex)}}}>");

		/// <summary>
		/// Use sql merge to get all of the generated values back for all rows AND be able to map them to the correct objects
		/// </summary>
		/// <param name="mergeSerializer">When this method returns, contains a <see cref="Contracts.IMergeSerializer"/> that can be used to serialize collections into what's expected by the @serialized parameter</param>
		/// <param name="mergeOperations">Should inserts/updates/deletes be included in the merge statement?</param>
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
		public string Merge<T>(out Contracts.IMergeSerializer<T> mergeSerializer, SqlOperation mergeOperations, ITableMap<T>? tableMap = null, bool useJson = false)
		{
			var merge = new System.Text.StringBuilder();
			if (!useJson)
			{
				//--Create an internal representation of the XML document
				merge.AppendLine("declare @preparedDoc int;")
					.AppendLine("exec sp_xml_preparedocument @preparedDoc output, @serialized;")
					.AppendLine();
			}

			tableMap ??= GetTableMap<T>();
			var keyColumns = tableMap.KeyColumns;
			var insertColumns = tableMap.InsertColumns;
			var updateColumns = tableMap.UpdateColumns;
			var (includeInsert, includeUpdate, includeDelete) = (mergeOperations.HasFlag(SqlOperation.Insert), mergeOperations.HasFlag(SqlOperation.Update), mergeOperations.HasFlag(SqlOperation.Delete));
			var sourceColumns = (includeInsert, includeUpdate, includeDelete) switch
			{
				(true, false, _) => keyColumns.Union(insertColumns).ToList(),
				(true, true, _) => keyColumns.Union(tableMap.UpsertColumns).ToList(),
				(false, true, _) => keyColumns.Union(updateColumns).ToList(),
				(false, false, true) => keyColumns,
				_ => throw new ArgumentException("At least one of insert, update, or delete must be included in the merge."),
			};
			var aliases = BuildShortColumnNames(sourceColumns);
			mergeSerializer = useJson ? new JsonMergeSerializer<T>(aliases) : new XmlMergeSerializer<T>(aliases);

			merge.AppendLine($"merge {BuildTableIdentifier(tableMap)} as target")
				.AppendLine($"using (select * from open{(useJson ? "Json(@serialized)" : "Xml(@preparedDoc, '/_/_')")} with (")
				.AppendLine($"  {BuildColumnParser(null, aliases, useJson)},{string.Join(",", sourceColumns.Select(c => $"{Environment.NewLine}  {BuildColumnParser(c, aliases, useJson)}"))}")
				.AppendLine($")) as source on ({string.Join(" and ", keyColumns.Select(c => BuildColumnIdentifier(c).With(ci => $"target.{ci} = source.{ci}")))})");

			string? generatedOnInsert = null;
			if (includeInsert)
			{
				merge.AppendLine($"when not matched by target then insert({string.Join(", ", insertColumns.Select(BuildColumnIdentifier))})")
					.AppendLine($"  values({string.Join(", ", insertColumns.Select(c => $"source.{BuildColumnIdentifier(c)}"))})");

				var generatedColumns = tableMap.GeneratedColumns;
				if (generatedColumns.Any())
				{
					generatedOnInsert = $", null as {BuildIdentifier(SplitOn)}, {string.Join(", ", generatedColumns.Select(c => $"inserted.{BuildColumnIdentifier(c)}"))}";
				}
			}
			if (includeUpdate)
			{
				merge.AppendLine("when matched then update set")
					.AppendLine($"{string.Join($",{Environment.NewLine}", updateColumns.Select(c => BuildColumnIdentifier(c).With(ci => $"  target.{ci} = source.{ci}")))}");
			}
			if (includeDelete)
			{
				merge.AppendLine("when not matched by source then delete");
			}

			merge.Append($"output source.{CorrelationIndexIdentifier} as [{nameof(RecordMergeCorrelation.CorrelationIndex)}], $action as [{nameof(RecordMergeCorrelation.Action)}]{generatedOnInsert};");

			return merge.ToString();

			/// <summary>'Minify' column names to help make xml and json shorter</summary>
			static Dictionary<PropertyInfo, string> BuildShortColumnNames(List<IColumnMap> insertColumns)
			{
				return insertColumns.Select((c, i) => (column: c, index: i))
					.ToDictionary(x => x.column.Property, x => BuildShortColumnName(x.index));

				static string BuildShortColumnName(int index)
				{
					return new string(BuildAlias(index).Reverse().ToArray());
					//--Enumerates remainders from least significant to most in order to convert from base-10 to base-36
					static IEnumerable<char> BuildAlias(int i)
					{
						yield return (char)(i % 36).With(r => r < 10 ? '0' + r : 'a' + (r - 10));
						if (i >= 36)
						{
							foreach (var a in BuildAlias(i / 36))
							{
								yield return a;
							}
						}
					}
				}
			}
			/// <summary>Build the sql that's needed for each column defined by the openXml or openJson with(...) clause</summary>
			static string BuildColumnParser(IColumnMap? columnMap, Dictionary<PropertyInfo, string> aliases, bool useJson)
			{
				if (columnMap is IColumnMap column)
				{
					return Format(BuildColumnIdentifier(column), column.Property.PropertyType, aliases[column.Property]);
				}

				return Format(CorrelationIndexIdentifier, typeof(int), null);

				string Format(string columnIdentifier, Type type, string? alias) => $"{columnIdentifier} {GetSqlType(type)} '{(useJson ? "$." : "@")}_{alias}'";

				static string GetSqlType(Type type)
				{
					if (type.IsEnum)
					{
						return GetSqlType(Enum.GetUnderlyingType(type));
					}
					if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
					{
						return GetSqlType(Nullable.GetUnderlyingType(type));
					}

					return sqlTypeLookup[type];
				}
			}
		}

		protected static readonly Dictionary<Type, string> sqlTypeLookup = new Dictionary<Type, string>
		{
			[typeof(Guid)] = "uniqueidentifier",
			[typeof(bool)] = "bit",
			[typeof(byte)] = "tinyint",
			[typeof(char)] = "nvarchar",
			[typeof(DateTime)] = "datetime",
			[typeof(decimal)] = "decimal",
			[typeof(double)] = "float",
			[typeof(short)] = "smallint",
			[typeof(int)] = "int",
			[typeof(long)] = "bigint",
			[typeof(float)] = "float",
			[typeof(string)] = "nvarchar(max)"
		};


		public class RecordMergeCorrelation
		{
			public int CorrelationIndex { get; set; }
			public string Action { get; set; } = string.Empty;
		}
	}
}