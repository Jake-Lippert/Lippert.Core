﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lippert.Core.Collections
{
	/// <summary>
	/// Represents a tree whose nodes may contain any number of child nodes
	/// </summary>
	public class NTree<T> : IEnumerable<T>
	{
		/// <summary>
		/// Creates an N-Tree
		/// </summary>
		/// <param name="value">Specifies the root value of the tree</param>
		public NTree(T value) => Value = value;

		public T Value { get; }
		public List<NTree<T>> Children { get; set; } = new List<NTree<T>>();

		public void Add(T child) => Add(new NTree<T>(child));
		public void Add(NTree<T> child) => Children.Add(child);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public IEnumerator<T> GetEnumerator() => EnumerateTree().GetEnumerator();
		public IEnumerable<T> EnumerateTree()
		{
			yield return Value;
			foreach (var childNode in Children.SelectMany(x => x))
			{
				yield return childNode;
			}
		}
	}
}