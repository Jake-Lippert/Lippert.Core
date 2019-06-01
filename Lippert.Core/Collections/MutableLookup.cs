using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lippert.Core.Collections
{
	public class MutableLookup<TKey, TElement> : ILookup<TKey, TElement>
	{
		//--hackyKey is here so that 'null' can be used as a key in a dictionary.  Not super-ideal, but it simplifies things
		private readonly Dictionary<(TKey actualKey, bool hackyKey), List<TElement>> _backingElements = new Dictionary<(TKey, bool), List<TElement>>();

		public MutableLookup() { }
		public MutableLookup(IEnumerable<IGrouping<TKey, TElement>> groupings)
		{
			foreach (var grouping in groupings)
			{
				Add(grouping.Key, grouping);
			}
		}


		private static (TKey, bool) BuildKey(TKey key) => (key, default);

		public IEnumerable<TElement> this[TKey key]
		{
			get
			{
				if (_backingElements.TryGetValue(BuildKey(key), out var elements))
				{
					return elements.AsEnumerable();
				}

				return Enumerable.Empty<TElement>();
			}
			set
			{
				_backingElements[BuildKey(key)] = value.ToList();
			}
		}

		public void Add(TKey key, TElement element)
		{
			if (!_backingElements.ContainsKey(BuildKey(key)))
			{
				_backingElements.Add(BuildKey(key), new List<TElement>());
			}

			_backingElements[BuildKey(key)].Add(element);
		}
		public void Add(TKey key, IEnumerable<TElement> elements)
		{
			if (!_backingElements.ContainsKey(BuildKey(key)))
			{
				_backingElements.Add(BuildKey(key), new List<TElement>());
			}

			_backingElements[BuildKey(key)].AddRange(elements);
		}

		public int Count => _backingElements.Count;

		public bool Contains(TKey key) => _backingElements.TryGetValue(BuildKey(key), out var elements) && elements.Any();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator() =>
			_backingElements.SelectMany(be => be.Value.Select(e => (key: be.Key.actualKey, element: e)))
				.GroupBy(x => x.key, x => x.element)
				.GetEnumerator();
	}
}