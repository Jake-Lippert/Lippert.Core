using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Lippert.Core.Collections.Extensions
{
	public static class IEnumerableExtensions
	{
		public static IEnumerable<(TKey key, TValue value)> AsTuples<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source) =>
			source.Select(kvp => (kvp.Key, kvp.Value));
		public static IEnumerable<(TKey key, List<TValue> values)> AsTuples<TKey, TValue>(this IEnumerable<IGrouping<TKey, TValue>> source) =>
			source.Select(grouping => (grouping.Key, grouping.ToList()));

		public static IEnumerable<(T item, int index)> Indexed<T>(this IEnumerable<T> source) => source.Select((x, i) => (x, i));

		public static Dictionary<TKey, List<T>> GroupToDictionary<T, TKey, TValue>(this IEnumerable<T> source, Func<T, TKey> keySelector) =>
			source.GroupToDictionary(keySelector, (key, x) => x.ToList());
		public static Dictionary<TKey, TValue> GroupToDictionary<T, TKey, TValue>(this IEnumerable<T> source, Func<T, TKey> keySelector, Func<IEnumerable<T>, TValue> valueSelector) =>
			source.GroupToDictionary(keySelector, (key, x) => valueSelector(x));
		public static Dictionary<TKey, TValue> GroupToDictionary<T, TKey, TValue>(this IEnumerable<T> source, Func<T, TKey> keySelector, Func<IEnumerable<T>, TValue> valueSelector, IEqualityComparer<TKey> comparer) =>
			source.GroupToDictionary(keySelector, (key, x) => valueSelector(x), comparer);
		public static Dictionary<TKey, TValue> GroupToDictionary<T, TKey, TValue>(this IEnumerable<T> source, Func<T, TKey> keySelector, Func<TKey, IEnumerable<T>, TValue> valueSelector) =>
			source.GroupBy(keySelector).ToDictionary(x => x.Key, x => valueSelector(x.Key, x));
		public static Dictionary<TKey, TValue> GroupToDictionary<T, TKey, TValue>(this IEnumerable<T> source, Func<T, TKey> keySelector, Func<TKey, IEnumerable<T>, TValue> valueSelector, IEqualityComparer<TKey> comparer) =>
			source.GroupBy(keySelector, comparer).ToDictionary(x => x.Key, x => valueSelector(x.Key, x), comparer);

		public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> source, IComparer<T> comparer) => source.OrderBy(x => x, comparer);

		public static IEnumerable<T> OrderByDescending<T>(this IEnumerable<T> source, IComparer<T> comparer) => source.OrderByDescending(x => x, comparer);

		/// <summary>
		/// Builds a DataTable using the properties on the specified object
		/// https://offbyoneerrors.wordpress.com/2015/11/06/passing-collections-of-structured-data-to-stored-procedures/
		/// </summary>
		public static DataTable ToDataTable<T>(this IEnumerable<T> data)
		{
			var dataTable = new DataTable();

			var properties = typeof(T).GetProperties();
			foreach (var property in properties)
			{
				var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
				//--Create a column with the same name and data type as the current property
				dataTable.Columns.Add(property.Name, propertyType.IsEnum ? Enum.GetUnderlyingType(propertyType) : propertyType);
			}

			foreach (var item in data)
			{
				//--Create a row with all of the properties for this object
				var row = dataTable.NewRow();
				foreach (var prop in properties)
				{
					var propertyValue = prop.GetValue(item, null);
					if (prop.PropertyType.IsEnum)
					{
						propertyValue = Convert.ChangeType(propertyValue, Enum.GetUnderlyingType(prop.PropertyType));
					}

					row[prop.Name] = propertyValue ?? DBNull.Value;
				}

				dataTable.Rows.Add(row);
			}

			return dataTable;
		}
	}
}