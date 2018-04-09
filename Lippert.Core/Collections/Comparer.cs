using System;
using System.Collections.Generic;

namespace Lippert.Core.Collections
{
	/// <summary>
	/// A generic implementation of <see cref="IComparer<T>"/>.
	/// </summary>
	/// <typeparam name="T">The type of objects to compare.</typeparam>
	public sealed class Comparer<T> : IComparer<T>
	{
		private readonly Func<T, T, int> _compare;

		private Comparer(Func<T, T, int> compare) => _compare = compare;


		public int Compare(T x, T y) => _compare(x, y);

		public static IComparer<T> Create(Func<T, T, int> compare) => new Comparer<T>(compare ?? throw new ArgumentNullException(nameof(compare)));

		public static IComparer<T> Create<TCompare>(Func<T, TCompare> selector)
			where TCompare : IComparable<TCompare>
		{
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}

			var defaultComparer = System.Collections.Generic.Comparer<TCompare>.Default;
			return Create((l, r) => defaultComparer.Compare(selector(l), selector(r)));
		}
	}
}