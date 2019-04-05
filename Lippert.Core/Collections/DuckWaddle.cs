using System;
using System.Collections.Generic;
using System.Linq;

namespace Lippert.Core.Collections
{
	public static class DuckWaddle
	{
		/// <summary>
		/// Compares two sorted collections and operates on a venn diagram-like comparison
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="comparer"></param>
		/// <param name="leftOnly"></param>
		/// <param name="both"></param>
		/// <param name="rightOnly"></param>
		public static void Compare<T>(IEnumerable<T> left, IEnumerable<T> right, IComparer<T> comparer,
			Action<T> leftOnly, Action<T, T> both, Action<T> rightOnly)
		{
			using (IEnumerator<T> leftEnumerator = left.GetEnumerator(), rightEnumerator = right.GetEnumerator())
			{
				bool hasLeft = leftEnumerator.MoveNext(), hasRight = rightEnumerator.MoveNext();

				while (hasLeft && hasRight)
				{
					var comparison = comparer.Compare(leftEnumerator.Current, rightEnumerator.Current);

					if (comparison < 0)
					{
						leftOnly(leftEnumerator.Current);
						hasLeft = leftEnumerator.MoveNext();
					}
					else if (comparison > 0)
					{
						rightOnly(rightEnumerator.Current);
						hasRight = rightEnumerator.MoveNext();
					}
					else
					{
						both(leftEnumerator.Current, rightEnumerator.Current);
						hasLeft = leftEnumerator.MoveNext();
						hasRight = rightEnumerator.MoveNext();
					}
				}

				while (hasLeft)
				{
					leftOnly(leftEnumerator.Current);
					hasLeft = leftEnumerator.MoveNext();
				}
				while (hasRight)
				{
					rightOnly(rightEnumerator.Current);
					hasRight = rightEnumerator.MoveNext();
				}
			}
		}

		/// <summary>
		/// Compares two sorted collections and operates on a venn diagram-like comparison
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TCompare"></typeparam>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="selector"></param>
		/// <param name="leftOnly"></param>
		/// <param name="both"></param>
		/// <param name="rightOnly"></param>
		public static void Compare<T, TCompare>(IEnumerable<T> left, IEnumerable<T> right, Func<T, TCompare> selector,
			Action<T> leftOnly, Action<T, T> both, Action<T> rightOnly)
			where TCompare : IComparable<TCompare> =>
			Compare(left, right, CreateComparer(selector), leftOnly, both, rightOnly);

		/// <summary>
		/// Compares two sorted collections and operates on a venn diagram-like comparison
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="leftOnly"></param>
		/// <param name="both"></param>
		/// <param name="rightOnly"></param>
		public static void Compare<T>(IEnumerable<T> left, IEnumerable<T> right,
			Action<T> leftOnly, Action<T, T> both, Action<T> rightOnly)
			where T : IComparable<T> =>
			Compare(left, right, Comparer<T>.Default, leftOnly, both, rightOnly);

		/// <summary>
		/// Compares two sorted collections and provides a venn diagram-like result
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static (List<T> Left, List<(T Left, T Right)> Both, List<T> Right) Compare<T>(IEnumerable<T> left, IEnumerable<T> right, IComparer<T> comparer)
		{
			var result = new
			{
				Left = new List<T>(),
				Both = new List<(T, T)>(),
				Right = new List<T>()
			};

			Compare(left, right, comparer, l => result.Left.Add(l), (l, r) => result.Both.Add((l, r)), r => result.Right.Add(r));

			return (result.Left, result.Both, result.Right);
		}

		/// <summary>
		/// Compares two sorted collections and provides a venn diagram-like result
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TCompare"></typeparam>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="selector"></param>
		/// <returns></returns>
		public static (List<T> Left, List<(T Left, T Right)> Both, List<T> Right) Compare<T, TCompare>(IEnumerable<T> left, IEnumerable<T> right, Func<T, TCompare> selector)
			where TCompare : IComparable<TCompare> =>
			Compare(left, right, CreateComparer(selector));

		/// <summary>
		/// Compares two sorted collections and provides a venn diagram-like result
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static (List<T> Left, List<(T Left, T Right)> Both, List<T> Right) Compare<T>(IEnumerable<T> left, IEnumerable<T> right)
			where T : IComparable<T> =>
			Compare(left, right, Comparer<T>.Default);

		/// <summary>
		/// Sorts and compares two collections and provides a venn diagram-like result
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static (List<T> Left, List<(T Left, T Right)> Both, List<T> Right) SortAndCompare<T>(IEnumerable<T> left, IEnumerable<T> right, IComparer<T> comparer) =>
			Compare(left.OrderBy(l => l, comparer), right.OrderBy(r => r, comparer), comparer);

		/// <summary>
		/// Sorts and compares two collections and provides a venn diagram-like result
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TCompare"></typeparam>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="selector"></param>
		/// <returns></returns>
		public static (List<T> Left, List<(T Left, T Right)> Both, List<T> Right) SortAndCompare<T, TCompare>(IEnumerable<T> left, IEnumerable<T> right, Func<T, TCompare> selector)
			where TCompare : IComparable<TCompare> =>
			SortAndCompare(left, right, CreateComparer(selector));

		/// <summary>
		/// Sorts and compares two collections and provides a venn diagram-like result
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static (List<T> Left, List<(T Left, T Right)> Both, List<T> Right) SortAndCompare<T>(IEnumerable<T> left, IEnumerable<T> right)
			where T : IComparable<T> =>
			SortAndCompare(left, right, Comparer<T>.Default);

		/// <summary>
		/// Sorts and compares two sorted collections and operates on a venn diagram-like comparison
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="comparer"></param>
		/// <param name="leftOnly"></param>
		/// <param name="both"></param>
		/// <param name="rightOnly"></param>
		public static void SortAndCompare<T>(IEnumerable<T> left, IEnumerable<T> right, IComparer<T> comparer,
			Action<T> leftOnly, Action<T, T> both, Action<T> rightOnly) =>
			Compare(left.OrderBy(l => l, comparer), right.OrderBy(r => r, comparer), comparer, leftOnly, both, rightOnly);

		/// <summary>
		/// Sorts and compares two sorted collections and operates on a venn diagram-like comparison
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TCompare"></typeparam>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="selector"></param>
		/// <param name="leftOnly"></param>
		/// <param name="both"></param>
		/// <param name="rightOnly"></param>
		public static void SortAndCompare<T, TCompare>(IEnumerable<T> left, IEnumerable<T> right, Func<T, TCompare> selector,
			Action<T> leftOnly, Action<T, T> both, Action<T> rightOnly)
			where TCompare : IComparable<TCompare> =>
			SortAndCompare(left, right, CreateComparer(selector), leftOnly, both, rightOnly);

		/// <summary>
		/// Sorts and compares two sorted collections and operates on a venn diagram-like comparison
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="leftOnly"></param>
		/// <param name="both"></param>
		/// <param name="rightOnly"></param>
		public static void SortAndCompare<T>(IEnumerable<T> left, IEnumerable<T> right,
			Action<T> leftOnly, Action<T, T> both, Action<T> rightOnly)
			where T : IComparable<T> =>
			SortAndCompare(left, right, Comparer<T>.Default, leftOnly, both, rightOnly);


		private static IComparer<T> CreateComparer<T, TCompare>(Func<T, TCompare> selector)
			where TCompare : IComparable<TCompare> =>
			Comparer<T>.Create((T x, T y) => selector(x).CompareTo(selector(y)));
	}
}