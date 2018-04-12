using System;
using System.Collections.Generic;

namespace Lippert.Core.Collections.Extensions
{
	public static class IDictionaryExtensions
    {
		/// <summary>
		/// Adds the elements of the specified collection to the dictionary
		/// </summary>
		public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> source) =>
			dictionary.AddRange(source, kvp => kvp.Key, kvp => kvp.Value);

		/// <summary>
		/// Adds the elements of the specified collection to the dictionary with a selector for the keys
		/// </summary>
		public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TValue> source, Func<TValue, TKey> keySelector) =>
			dictionary.AddRange(source, keySelector, x => x);

		/// <summary>
		/// Adds the elements of the specified collection to the dictionary with selectors for the keys and values
		/// </summary>
		public static void AddRange<TKey, TValue, TSource>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
		{
			foreach (var item in source)
			{
				dictionary.Add(keySelector(item), valueSelector(item));
			}
		}
	}
}