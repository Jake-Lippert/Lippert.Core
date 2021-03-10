using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lippert.Core.Collections;
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
		private readonly IDictionary<Type, string> _sqlTypes;
		private static readonly Dictionary<Type, string> _sqlTypeLookup = new Dictionary<Type, string>
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

		protected MergeSerializerBase(ITableMap<T> tableMap)
		{
			TableMap = tableMap;
			Aliases = BuildShortColumnNames(TableMap.InstanceColumns.Select(c => c.Value));
			_sqlTypes = RetrievalDictionary.Build((Type type) => GetSqlType(type));

			/// <summary>
			/// Get the sql type for the specified .Net type
			/// </summary>
			static string GetSqlType(Type type) => type switch
			{
				//--Investigate if there's any way to get to a SqlMapper.TypeHandler; probably can't from this library.
				{ IsEnum: true } => GetSqlType(Enum.GetUnderlyingType(type)),
				{ IsGenericType: true } when type.GetGenericTypeDefinition() == typeof(Nullable<>) => GetSqlType(Nullable.GetUnderlyingType(type)),
				_ => _sqlTypeLookup[type]
			};
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
				IColumnMap column => FormatColumnParser(SqlServerQueryBuilder.BuildColumnIdentifier(column), column.Property.PropertyType, Aliases[column.Property]),
				_ => FormatColumnParser(SqlServerMergeQueryBuilder.CorrelationIndexIdentifier, typeof(int), null)
			};

			string FormatColumnParser(string columnIdentifier, Type type, string? alias) => $"{columnIdentifier} {_sqlTypes[type]} '{BuildColumnParserAlias(alias)}'";
		}
	}
}