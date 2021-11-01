using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lippert.Core.Collections.Extensions;
using Lippert.Core.Data.Contracts;
using Lippert.Core.Extensions;

namespace Lippert.Core.Data.QueryBuilders
{

	/// <summary>
	/// Common functionality for merge serializers to be used by <see cref="SqlServerMergeQueryBuilder"/>
	/// </summary>
	/// <typeparam name="T">The type to be serialized for merge operations</typeparam>
	public abstract class MergeSerializerBase<T> : Contracts.IMergeSerializer<T>
	{
		protected MergeSerializerBase(ITableMap<T> tableMap)
		{
			TableMap = tableMap;
			Aliases = BuildShortColumnNames(TableMap.InstanceColumns.Select(c => c.Value));
		}

		/// <summary>
		/// The table map for the model being serialized
		/// </summary>
		public ITableMap<T> TableMap { get; }
		/// <summary>
		/// A mapping from model properties to their 'minified' names used during serialization
		/// </summary>
		public Dictionary<PropertyInfo, string> Aliases { get; }

		/// <summary>
		/// Serializes the specified records into a version that can be used with <see cref="SqlServerMergeQueryBuilder"/>'s sql
		/// </summary>
		public abstract string SerializeForMerge(IEnumerable<T> records);

		/// <summary>
		/// Gets the model's value for the specified property
		/// </summary>
		public static object GetPropertyValue(T record, PropertyInfo property) => property.GetValue(record) switch
		{
			//--Investigate if there's any way to get to a SqlMapper.TypeHandler; probably can't from this library.
			Enum e => Convert.ChangeType(e, Enum.GetUnderlyingType(e.GetType())),
			byte[] b => Convert.ToBase64String(b),
			var propertyValue => propertyValue
		};

		/// <summary>
		/// 'Minify' column names to help make xml and json shorter
		/// </summary>
		public Dictionary<PropertyInfo, string> BuildShortColumnNames(IEnumerable<IColumnMap> columns)
		{
			return columns.ToDictionary((column, _) => column.Property, (_, index) => BuildShortColumnName(index));

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

		/// <summary>
		/// Build the column identifier for a given column map, or the correlation index identifier if none is supplied
		/// </summary>
		public string BuildColumnIdentifier(IColumnMap? columnMap) => columnMap switch
		{
			{ } column => SqlServerQueryBuilder.BuildColumnIdentifier(column),
			_ => SqlServerMergeQueryBuilder.CorrelationIndexIdentifier
		};
		/// <summary>
		/// Build the 'minification' alias to be used when parsing the serialized records
		/// </summary>
		public abstract string BuildColumnParserAlias(string? alias);
		/// <summary>
		/// Build the sql that's needed for each column defined by the openXml or openJson with(...) clause
		/// </summary>
		public string BuildColumnParser(IColumnMap? columnMap)
		{
			return columnMap switch
			{
				{ } column => $"{BuildColumnIdentifier(columnMap)} {GetSqlType(column)} '{BuildColumnParserAlias(Aliases[column.Property])}'",
				_ => $"{BuildColumnIdentifier(columnMap)} {SqlTypeLookup.GetSqlType<int>()} '{BuildColumnParserAlias(null)}'"
			};

			static string GetSqlType(IColumnMap column)
			{
				if (column.Property.PropertyType == typeof(byte[]))
				{
					return $"{SqlTypeLookup.GetSqlType<string>()}(max)";
				}
				else
				{
					return column.GetSqlType();
				}
			}
		}
	}
}