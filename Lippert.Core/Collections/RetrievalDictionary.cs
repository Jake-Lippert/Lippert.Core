using System;
using System.Collections.Generic;

namespace Lippert.Core.Collections
{
	public static class RetrievalDictionary
	{
		public static IDictionary<TKey, TValue> Build<TKey, TValue>(Func<TKey, TValue> func) => new Instance<TKey, TValue>(func);

		private class Instance<TKey, TValue> : Dictionary<TKey, TValue>, IDictionary<TKey, TValue>
		{
			private readonly Func<TKey, TValue> _retrievalFunc;

			public Instance(Func<TKey, TValue> retrievalFunc) => _retrievalFunc = retrievalFunc;

			public new TValue this[TKey key]
			{
				get => TryGetValue(key, out var result) ? result : base[key] = _retrievalFunc(key);
				set => base[key] = value;
			}

			TValue IDictionary<TKey, TValue>.this[TKey key]
			{
				get => this[key];
				set => this[key] = value;
			}
		}
	}
}