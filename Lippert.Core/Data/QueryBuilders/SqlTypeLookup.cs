using System;
using System.Collections.Generic;
using Lippert.Core.Collections;
using Lippert.Core.Data.Contracts;

namespace Lippert.Core.Data.QueryBuilders
{
	/// <summary>
	/// Provides functionality for mapping .Net types to sql types
	/// </summary>
	public static class SqlTypeLookup
	{
		private static readonly Dictionary<Type, string> _sqlTypeLookup;
		private static readonly IDictionary<Type, string> _sqlTypes;

		static SqlTypeLookup()
		{
			_sqlTypeLookup = new()
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
				[typeof(string)] = "nvarchar"
			};
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
		/// Gets the sql type for the specified .Net type
		/// </summary>
		public static string GetSqlType<T>() => GetSqlType(typeof(T));
		/// <summary>
		/// Gets the sql type for the specified .Net type
		/// </summary>
		public static string GetSqlType(this Type type) => _sqlTypes[type];
		/// <summary>
		/// Gets the sql type for the specified .Net type
		/// </summary>
		public static bool TryGetSqlType<T>(out string? sqlType) => TryGetSqlType(typeof(T), out sqlType);
		/// <summary>
		/// Gets the sql type for the specified .Net type
		/// </summary>
		public static bool TryGetSqlType(this Type type, out string? sqlType)
		{
			try
			{
				sqlType = GetSqlType(type);
				return true;
			}
			catch
			{
				sqlType = null;
				return false;
			}
		}

		/// <summary>
		/// Gets the sql type(including its length, precision, and scale where appropriate) for the specified column mapping
		/// </summary>
		public static string GetSqlType(this IColumnMap columnMap)
		{
			var sqlType = GetSqlType(columnMap.Property.PropertyType);
			if (columnMap.Property.PropertyType == typeof(string))
			{
				return $"{sqlType}({(columnMap.Length == int.MaxValue ? "max" : columnMap.Length)})";
			}
			else if (columnMap.Precision > 0 && (
				columnMap.Property.PropertyType == typeof(decimal) ||
				columnMap.Property.PropertyType == typeof(decimal?) ||
				columnMap.Property.PropertyType == typeof(float) ||
				columnMap.Property.PropertyType == typeof(float?) ||
				columnMap.Property.PropertyType == typeof(double) ||
				columnMap.Property.PropertyType == typeof(double?)))
			{
				return $"{sqlType}({columnMap.Precision},{columnMap.Scale})";
			}
			else
			{
				return sqlType;
			}
		}
		/// <summary>
		/// Gets the sql type(including its length, precision, and scale where appropriate) for the specified column mapping
		/// </summary>
		public static bool TryGetSqlType(this IColumnMap columnMap, out string? sqlType)
		{
			try
			{
				sqlType = GetSqlType(columnMap);
				return true;
			}
			catch
			{
				sqlType = null;
				return false;
			}
		}
	}
}