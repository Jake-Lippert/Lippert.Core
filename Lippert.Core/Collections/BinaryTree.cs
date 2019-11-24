using System;
using System.Collections;
using System.Collections.Generic;
using Lippert.Core.Collections.Extensions;

namespace Lippert.Core.Collections
{
	/// <summary>
	/// Represents a non-balancing binary tree capable of pre-order, in-order, and post-order traversal of its nodes
	/// </summary>
	public class BinaryTree<T> : IEnumerable<T>
		where T : IComparable<T>
	{
		/// <summary>
		/// Creates a binary tree
		/// </summary>
		/// <param name="value">Specifies the root value of the tree</param>
		public BinaryTree(T value) => Value = value;

		public T Value { get; }
		public BinaryTree<T>? Left { get; private set; }
		public BinaryTree<T>? Right { get; private set; }

		public void Add(T child) => Add(new BinaryTree<T>(child));
		public void Add(BinaryTree<T> child)
		{
			//--TODO: Enforce _traversalOrder?
			if (child.Value.CompareTo(Value) < 0)
			{
				if (Left == null)
				{
					Left = child;
				}
				else
				{
					Left.Add(child);
				}
			}
			else
			{
				if (Right == null)
				{
					Right = child;
				}
				else
				{
					Right.Add(child);
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public IEnumerator<T> GetEnumerator() => EnumerateTree(TreeTraversalOrder.InOrder).GetEnumerator();
		public IEnumerable<T> EnumerateTree(TreeTraversalOrder order)
		{
			switch (order)
			{
				case TreeTraversalOrder.InOrder:
					foreach (var childNode in (Left?.EnumerateTree(order)).EmptyIfNull())
					{
						yield return childNode;
					}

					yield return Value;

					foreach (var childNode in (Right?.EnumerateTree(order)).EmptyIfNull())
					{
						yield return childNode;
					}
					break;

				case TreeTraversalOrder.PreOrder:
					yield return Value;

					foreach (var childNode in (Left?.EnumerateTree(order)).EmptyIfNull())
					{
						yield return childNode;
					}

					foreach (var childNode in (Right?.EnumerateTree(order)).EmptyIfNull())
					{
						yield return childNode;
					}
					break;

				case TreeTraversalOrder.PostOrder:
					foreach (var childNode in (Left?.EnumerateTree(order)).EmptyIfNull())
					{
						yield return childNode;
					}

					foreach (var childNode in (Right?.EnumerateTree(order)).EmptyIfNull())
					{
						yield return childNode;
					}

					yield return Value;
					break;

				default:
					throw new ArgumentException($"Invalid order specified: {order}", nameof(order));
			}
		}
	}
}