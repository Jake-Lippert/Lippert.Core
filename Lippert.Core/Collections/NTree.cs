using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lippert.Core.Collections
{
	/// <summary>
	/// Represents a tree whose nodes may contain any number of child nodes
	/// </summary>
	public class NTree<TKey, TValue> : IEnumerable<TValue>
	{
		/// <summary>
		/// Creates an N-Tree
		/// </summary>
		/// <param name="key">Specifies the key for the tree node</param>
		/// <param name="value">Specifies the value for the tree node</param>
		public NTree(TKey key, TValue value)
		{
			Key = key;
			Value = value;
		}

		public TValue Value { get; }
		public TKey Key { get; }
		public List<NTree<TKey, TValue>> Children { get; set; } = new List<NTree<TKey, TValue>>();

		public void Add(TKey key, TValue value) => Add(new NTree<TKey, TValue>(key, value));
		public void Add(NTree<TKey, TValue> child) => Children.Add(child);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public IEnumerator<TValue> GetEnumerator() => EnumerateTree().GetEnumerator();
		public IEnumerable<TValue> EnumerateTree()
		{
			yield return Value;
			foreach (var childNode in Children.SelectMany(x => x))
			{
				yield return childNode;
			}
		}

		/// <summary>
		/// Gets the tree node whose key equals the specified indexer
		/// </summary>
		/// <param name="key">Specifies which node to attempt to retrieve</param>
		public NTree<TKey, TValue> this[TKey key] => TryGetNode(key, out var node) && node is { } ? node : throw new KeyNotFoundException();
		/// <summary>
		/// Gets the tree node whose key equals the specified indexer
		/// </summary>
		/// <param name="key">The key of the node to get</param>
		/// <param name="node">When this method returns, contains the node associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
		/// <returns>True if able to find a matching node, false otherwise</returns>
		public bool TryGetNode(TKey key, out NTree<TKey, TValue>? node)
		{
			if (Equals(key, Key))
			{
				node = this;
				return true;
			}

			foreach (var child in Children)
			{
				if (child.TryGetNode(key, out node))
				{
					return true;
				}
			}

			node = null;
			return false;
		}
	}
}