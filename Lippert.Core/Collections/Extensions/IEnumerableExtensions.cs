using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Lippert.Core.Collections.Extensions
{
	public static class IEnumerableExtensions
	{
		/// <summary>
		/// Returns the sequence of KeyValuePairs as named tuples
		/// </summary>
		public static IEnumerable<(TKey key, TValue value)> AsTuples<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source) =>
			source.Select(kvp => (kvp.Key, kvp.Value));
		/// <summary>
		/// Returns the sequence of Groupings as named tuples
		/// </summary>
		public static IEnumerable<(TKey key, List<TValue> values)> AsTuples<TKey, TValue>(this IEnumerable<IGrouping<TKey, TValue>> source) =>
			source.Select(grouping => (grouping.Key, grouping.ToList()));

		/// <summary>
		/// Returns an empty enumerable if the source is null
		/// </summary>
		public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source) => source ?? Enumerable.Empty<T>();

		/// <summary>
		/// Projects each element of a sequence into a named tuple including the element's index
		/// </summary>
		public static IEnumerable<(T item, int index)> Indexed<T>(this IEnumerable<T> source) => source.Select((x, i) => (x, i));

		public static Dictionary<TKey, List<T>> GroupToDictionary<T, TKey, TValue>(this IEnumerable<T> source, Func<T, TKey> keySelector) =>
			source.GroupToDictionary(keySelector, (key, x) => x.ToList());
		public static Dictionary<TKey, TValue> GroupToDictionary<T, TKey, TValue>(this IEnumerable<T> source, Func<T, TKey> keySelector, Func<IEnumerable<T>, TValue> valueSelector) =>
			source.GroupToDictionary(keySelector, (key, x) => valueSelector(x));
		public static Dictionary<TKey, TValue> GroupToDictionary<T, TKey, TValue>(this IEnumerable<T> source, Func<T, TKey> keySelector, Func<IEnumerable<T>, TValue> valueSelector, IEqualityComparer<TKey> comparer) =>
			source.GroupToDictionary(keySelector, (key, x) => valueSelector(x), comparer);
		public static Dictionary<TKey, TValue> GroupToDictionary<T, TKey, TValue>(this IEnumerable<T> source, Func<T, TKey> keySelector, Func<TKey, IEnumerable<T>, TValue> valueSelector) =>
			source.GroupBy(keySelector).ToDictionary(x => x.Key, x => valueSelector(x.Key, x));
		public static Dictionary<TKey, TValue> GroupToDictionary<T, TKey, TValue>(this IEnumerable<T> source, Func<T, TKey> keySelector, Func<TKey, IEnumerable<T>, TValue> valueSelector, IEqualityComparer<TKey> comparer) =>
			source.GroupBy(keySelector, comparer).ToDictionary(x => x.Key, x => valueSelector(x.Key, x), comparer);

		public static IEnumerable<TResult> LeftJoin<TLeft, TRight, TKey, TResult>(this IEnumerable<TLeft> left, IEnumerable<TRight> right,
			Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector,
			Func<TLeft, TRight, TResult> resultSelector) =>
			from l in left
			join r in right
				on leftKeySelector(l) equals rightKeySelector(r) into joined
			from j in joined.DefaultIfEmpty()
			select resultSelector(l, j);

		public static IEnumerable<TResult> ProcessBatch<T, TResult>(this IEnumerable<T> source, int batchSize, Func<List<T>, TResult> func)
		{
			if (batchSize > 0)
			{
				var batch = new List<T>(batchSize);
				foreach (var item in source)
				{
					batch.Add(item);

					if (batch.Count >= batchSize)
					{
						yield return func(batch);
						batch = new List<T>(batchSize);
					}
				}

				if (batch.Count > 0)
				{
					yield return func(batch);
				}
			}
			else
			{
				throw new ArgumentException($"'{nameof(batchSize)}' must be greater than 0");
			}
		}

		public static IEnumerable<TResult> RightJoin<TLeft, TRight, TKey, TResult>(this IEnumerable<TLeft> left, IEnumerable<TRight> right,
			Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector,
			Func<TLeft, TRight, TResult> resultSelector) =>
			right.LeftJoin(left, rightKeySelector, leftKeySelector, (r, l) => resultSelector(l, r));

		public static IEnumerable<TResult> Select<T1, T2, TResult>(this IEnumerable<(T1, T2)> source, Func<T1, T2, TResult> selector) => source.Select(x => selector(x.Item1, x.Item2));
		public static IEnumerable<TResult> Select<T1, T2, T3, TResult>(this IEnumerable<(T1, T2, T3)> source, Func<T1, T2, T3, TResult> selector) => source.Select(x => selector(x.Item1, x.Item2, x.Item3));
		public static IEnumerable<TResult> Select<T1, T2, T3, T4, TResult>(this IEnumerable<(T1, T2, T3, T4)> source, Func<T1, T2, T3, T4, TResult> selector) => source.Select(x => selector(x.Item1, x.Item2, x.Item3, x.Item4));
		public static IEnumerable<TResult> Select<T1, T2, T3, T4, T5, TResult>(this IEnumerable<(T1, T2, T3, T4, T5)> source, Func<T1, T2, T3, T4, T5, TResult> selector) => source.Select(x => selector(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5));
		public static IEnumerable<TResult> Select<T1, T2, T3, T4, T5, T6, TResult>(this IEnumerable<(T1, T2, T3, T4, T5, T6)> source, Func<T1, T2, T3, T4, T5, T6, TResult> selector) => source.Select(x => selector(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6));

		/// <summary>
		/// Builds a binary tree whose root node is the first value in source
		/// </summary>
		public static BinaryTree<T> ToBinaryTree<T>(this IEnumerable<T> source)
			where T : IComparable<T>
		{
			BinaryTree<T>? tree = null;
			foreach (var item in source)
			{
				if (tree == null)
				{
					tree = new BinaryTree<T>(item);
				}
				else
				{
					tree.Add(item);
				}
			}

			return tree ?? throw new InvalidOperationException("The sequence must contain at least one element.");
		}

		/// <summary>
		/// Builds a DataTable using the properties on the specified object
		/// https://offbyoneerrors.wordpress.com/2015/11/06/passing-collections-of-structured-data-to-stored-procedures/
		/// </summary>
		public static DataTable ToDataTable<T>(this IEnumerable<T> data)
		{
			var dataTable = new DataTable();

			var properties = typeof(T).GetProperties();
			foreach (var property in properties)
			{
				var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
				//--Create a column with the same name and data type as the current property
				dataTable.Columns.Add(property.Name, propertyType.IsEnum ? Enum.GetUnderlyingType(propertyType) : propertyType);
			}

			foreach (var item in data)
			{
				//--Create a row with all of the properties for this object
				var row = dataTable.NewRow();
				foreach (var prop in properties)
				{
					var propertyValue = prop.GetValue(item, null);
					if (prop.PropertyType.IsEnum)
					{
						propertyValue = Convert.ChangeType(propertyValue, Enum.GetUnderlyingType(prop.PropertyType));
					}

					row[prop.Name] = propertyValue ?? DBNull.Value;
				}

				dataTable.Rows.Add(row);
			}

			return dataTable;
		}

		/// <summary>
		/// Creates a <see cref="Dictionary{TKey, TValue}"/> from an <see cref="IEnumerable{T}"/> according to specified key selector and element selector functions that provide element indices.
		/// </summary>
		public static Dictionary<TKey, TValue> ToDictionary<T, TKey, TValue>(this IEnumerable<T> source, Func<T, int, TKey> keySelector, Func<T, int, TValue> valueSelector) =>
			source.Indexed().ToDictionary(x => keySelector(x.item, x.index), x => valueSelector(x.item, x.index));

		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source) => source.ToHashSet(x => x);
		public static HashSet<TSet> ToHashSet<TSource, TSet>(this IEnumerable<TSource> source, Func<TSource, TSet> selector) => new(source.ToList().Select(selector));

		public static MutableLookup<TKey, T> ToMutableLookup<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector) =>
			source.ToMutableLookup(keySelector, x => x);
		public static MutableLookup<TKey, TElement> ToMutableLookup<T, TKey, TElement>(this IEnumerable<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector) =>
			new(source.GroupBy(keySelector, elementSelector));

		/// <summary>
		/// Builds an N-Tree given the parent-child relations specified
		/// </summary>
		/// <seealso cref="https://stackoverflow.com/a/18018037/595473"/>
		public static NTree<TKey, TValue> ToNTree<TValue, TKey>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector, Func<TValue, TKey> parentKeySelector, TValue defaultRoot)
		{
			//--Initialize the map
			var nodeMap = source.ToDictionary(keySelector, x => (ParentId: parentKeySelector(x), Tree: new NTree<TKey, TValue>(keySelector(x), x)));

			List<NTree<TKey, TValue>> roots = new(), parentsNotFound = new();
			foreach (var (parentId, tree) in nodeMap.Values)
			{
				//--TODO: I don't like adding the !, but it seems to be the only way to avoid being yelled at by the compiler at the moment.
				//  Besides, the Equals check won't explode if the default value ends up being null
				if (Equals(parentId, default(TKey)!))
				{
					roots.Add(tree);
				}
				//--If you have dangling branches check that nodeMap[parentId] exists
				else if (nodeMap.TryGetValue(parentId, out var parentNode))
				{
					parentNode.Tree.Add(tree);
				}
				else
				{
					parentsNotFound.Add(tree);
				}
			}

			return roots.Count switch
			{
				//--If we have no roots
				0 => new NTree<TKey, TValue>(keySelector(defaultRoot), defaultRoot)
				{
					Children = parentsNotFound
				},
				//--If we have a single root whose parent key doesn't match the specified default root
				1 when roots.Single() is var root && !Equals(parentKeySelector(root.Value), keySelector(defaultRoot)) => root,
				//--If we have one root whose parent key matches the specified default root
				//--If we have multiple roots
				_ => new NTree<TKey, TValue>(keySelector(defaultRoot), defaultRoot)
				{
					Children = roots
				}
			};
		}

		public static IEnumerable<(T1, T2)> Where<T1, T2>(this IEnumerable<(T1, T2)> source, Func<T1, T2, bool> predicate) => source.Where(x => predicate(x.Item1, x.Item2));
		public static IEnumerable<(T1, T2, T3)> Where<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> source, Func<T1, T2, T3, bool> predicate) => source.Where(x => predicate(x.Item1, x.Item2, x.Item3));
		public static IEnumerable<(T1, T2, T3, T4)> Where<T1, T2, T3, T4>(this IEnumerable<(T1, T2, T3, T4)> source, Func<T1, T2, T3, T4, bool> predicate) => source.Where(x => predicate(x.Item1, x.Item2, x.Item3, x.Item4));
		public static IEnumerable<(T1, T2, T3, T4, T5)> Where<T1, T2, T3, T4, T5>(this IEnumerable<(T1, T2, T3, T4, T5)> source, Func<T1, T2, T3, T4, T5, bool> predicate) => source.Where(x => predicate(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5));
		public static IEnumerable<(T1, T2, T3, T4, T5, T6)> Where<T1, T2, T3, T4, T5, T6>(this IEnumerable<(T1, T2, T3, T4, T5, T6)> source, Func<T1, T2, T3, T4, T5, T6, bool> predicate) => source.Where(x => predicate(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6));
	}
}