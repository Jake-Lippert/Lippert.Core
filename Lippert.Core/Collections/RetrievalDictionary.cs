using System;
using System.Collections.Generic;

namespace Lippert.Core.Collections
{
	public static class RetrievalDictionary
	{
		/// <summary>
		/// Builds a cache-like dictionary that can retrieve values that aren't present
		/// </summary>
		public static IDictionary<TKey, TValue> Build<TKey, TValue>(Func<TKey, TValue> func) => new Instance<TKey, TValue>(func);

		/// <summary>
		/// A cache-like dictionary that can retrieve values that aren't present
		/// </summary>
		private class Instance<TKey, TValue> : Dictionary<TKey, TValue>, IDictionary<TKey, TValue>
		{
			private readonly Func<TKey, TValue> _retrievalFunc;

			public Instance(Func<TKey, TValue> retrievalFunc) => _retrievalFunc = retrievalFunc;

			/// <summary>
			/// Retrieves the value for a given key.
			/// If the key is not present, the configured function will be called to obtain and store the value.
			/// </summary>
			public new TValue this[TKey key]
			{
				get => base.TryGetValue(key, out var result) ? result : base[key] = _retrievalFunc(key);
				set => base[key] = value;
			}
			public new bool TryGetValue(TKey key, out TValue value)
			{
				try
				{
					value = this[key];
					return true;
				}
				catch
				{
					value = default!;
					return false;
				}
			}

			TValue IDictionary<TKey, TValue>.this[TKey key]
			{
				get => this[key];
				set => this[key] = value;
			}
		}
	}
}