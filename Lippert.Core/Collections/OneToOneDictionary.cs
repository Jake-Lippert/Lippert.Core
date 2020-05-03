using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lippert.Core.Collections.Extensions;

namespace Lippert.Core.Collections
{
	/// <summary>
	/// A two-way dictionary that synchronizes both backing source dictionaries
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <remarks>
	///	IDictionary<T2, T1> - Cannot implement both because they may unify for some type parameter substitutions;
	///	Use implicit operator to get a OneToOneDictionary<T2, T1> : IDictionary<T2, T1>
	/// </remarks>
	public class OneToOneDictionary<T1, T2> : IDictionary<T1, T2>
	{
		private readonly HashSet<T1> _knownKeys1;
		private readonly HashSet<T2> _knownKeys2;
		private readonly IDictionary<T1, T2> _dictionary1;
		private readonly IDictionary<T2, T1> _dictionary2;
		private OneToOneDictionary<T2, T1>? _reversed;

		public OneToOneDictionary(IDictionary<T1, T2> dictionary1, IDictionary<T2, T1>? dictionary2 = null)
		{
			_knownKeys1 = (_dictionary1 = dictionary1).Keys.ToHashSet();
			_knownKeys2 = (_dictionary2 = dictionary2 ?? new Dictionary<T2, T1>()).Keys.ToHashSet();
		}
		public OneToOneDictionary()
			: this(new Dictionary<T1, T2>(), new Dictionary<T2, T1>()) { }
		public OneToOneDictionary(IEnumerable<(T1 key, T2 value)> source)
			: this() => AddRange(source);
		public OneToOneDictionary(IEnumerable<(T2 key, T1 value)> source)
			: this() => AddRange(source);
		public OneToOneDictionary(IEnumerable<KeyValuePair<T1, T2>> source)
			: this(source.Select(x => (x.Key, x.Value))) { }
		public OneToOneDictionary(IEnumerable<KeyValuePair<T2, T1>> source)
			: this(source.Select(x => (x.Key, x.Value))) { }

		public T2 this[T1 key]
		{
			get
			{
				if (!_dictionary1.ContainsKey(key))
				{
					SynchronizeBackingDictionaries();
				}

				var value = _dictionary1[key];
				if (!_dictionary2.ContainsKey(value))
				{
					SynchronizeBackingDictionaries();
				}

				return value;
			}
			set => SetValue(_dictionary1, _dictionary2, key, value);
		}
		public T1 this[T2 key]
		{
			get
			{
				if (!_dictionary2.ContainsKey(key))
				{
					SynchronizeBackingDictionaries();
				}

				var value = _dictionary2[key];
				if (!_dictionary1.ContainsKey(value))
				{
					SynchronizeBackingDictionaries();
				}

				return value;
			}
			set => SetValue(_dictionary2, _dictionary1, key, value);
		}
		private static void SetValue<TA, TB>(IDictionary<TA, TB> dictionaryA, IDictionary<TB, TA> dictionaryB, TA key, TB value) => dictionaryA[dictionaryB[value] = key] = value;

		public int Count => ((IDictionary<T1, T2>)this).Keys.Count;
		public bool IsReadOnly => _dictionary1.IsReadOnly || _dictionary2.IsReadOnly;
		ICollection<T1> IDictionary<T1, T2>.Keys => _dictionary1.Keys.Union(_dictionary2.Values).ToList();
		ICollection<T2> IDictionary<T1, T2>.Values => _dictionary1.Values.Union(_dictionary2.Keys).ToList();


		void ICollection<KeyValuePair<T1, T2>>.Add(KeyValuePair<T1, T2> item) => Add(item.Key, item.Value);
		public void Add(T1 key, T2 value) => Add(_dictionary1, _dictionary2, key, value);
		public void Add(T2 key, T1 value) => Add(_dictionary2, _dictionary1, key, value);
		public static void Add<TA, TB>(IDictionary<TA, TB> dictionaryA, IDictionary<TB, TA> dictionaryB, TA key, TB value)
		{
			dictionaryA.Add(key, value);
			dictionaryB[value] = key;
		}
		public void AddRange(IEnumerable<(T1 key, T2 value)> source)
		{
			foreach (var (key, value) in source)
			{
				Add(key, value);
			}
		}
		public void AddRange(IEnumerable<(T2 key, T1 value)> source)
		{
			foreach (var (key, value) in source)
			{
				Add(key, value);
			}
		}

		public void Clear()
		{
			_dictionary1.Clear();
			_dictionary2.Clear();
		}

		bool ICollection<KeyValuePair<T1, T2>>.Contains(KeyValuePair<T1, T2> item) => ContainsKey(item.Key);
		public bool ContainsKey(T1 key)
		{
			SynchronizeBackingDictionaries();
			return _dictionary1.ContainsKey(key);
		}
		public bool ContainsKey(T2 key)
		{
			SynchronizeBackingDictionaries();
			return _dictionary2.ContainsKey(key);
		}

		void ICollection<KeyValuePair<T1, T2>>.CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex)
		{
			SynchronizeBackingDictionaries();
			_dictionary1.CopyTo(array, arrayIndex);
		}

		public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
		{
			SynchronizeBackingDictionaries();
			return _dictionary1.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		bool ICollection<KeyValuePair<T1, T2>>.Remove(KeyValuePair<T1, T2> item) => Remove(item.Key);
		public bool Remove(T1 key) => Remove(_dictionary1, _dictionary2, key);
		public bool Remove(T2 key) => Remove(_dictionary2, _dictionary1, key);
		private static bool Remove<TA, TB>(IDictionary<TA, TB> dictionaryA, IDictionary<TB, TA> dictionaryB, TA key)
		{
			if (dictionaryA.TryGetValue(key, out var value))
			{
				dictionaryB.Remove(value);
			}

			return dictionaryA.Remove(key);
		}

		public bool TryGetValue(T1 key, out T2 value)
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
		public bool TryGetValue(T2 key, out T1 value)
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

		private void SynchronizeBackingDictionaries()
		{
			foreach (var kvp in _dictionary1.Where(x => !_knownKeys1.Contains(x.Key)))
			{
				_dictionary2[kvp.Value] = kvp.Key;
			}
			foreach (var kvp in _dictionary2.Where(x => !_knownKeys2.Contains(x.Key)))
			{
				_dictionary1[kvp.Value] = kvp.Key;
			}

			_knownKeys1.UnionWith(_dictionary1.Keys);
			_knownKeys2.UnionWith(_dictionary2.Keys);
		}

		public static implicit operator OneToOneDictionary<T2, T1>(OneToOneDictionary<T1, T2> original) => original._reversed ??= new OneToOneDictionary<T2, T1>(original._dictionary2, original._dictionary1);
	}
}