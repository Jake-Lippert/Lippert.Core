using System;
using System.Collections.Generic;
using System.Linq;
using Lippert.Core.Data.Contracts;
using Lippert.Core.Data.QueryBuilders.Contracts;

namespace Lippert.Core.Data.QueryBuilders
{
	/// <summary>
	/// Build and represent the components of merge statements
	/// </summary>
	public class MergeDefinition<T>
	{
		private readonly ComponentDefinition _insertDefinition = new ComponentDefinition();
		private readonly ComponentDefinition _updateDefinition = new ComponentDefinition();
		private readonly ComponentDefinition _deleteDefinition = new ComponentDefinition();

		/// <summary>
		/// Should inserts be included in this merge statement?
		/// </summary>
		public bool IncludeInsert => _insertDefinition.Include;
		/// <summary>
		/// Should updates be included in this merge statement?
		/// </summary>
		public bool IncludeUpdate => _updateDefinition.Include;
		/// <summary>
		/// Should deletes be included in this merge statement?
		/// </summary>
		public bool IncludeDelete => _deleteDefinition.Include;

		/// <summary>
		/// Configure the insert component of a merge statement
		/// </summary>
		public MergeDefinition<T> Insert(/*Func<ValuedPredicateBuilder<T>, ValuedPredicateBuilder<T>>? predicateBuilder = null*/)
		{
			_insertDefinition.Configure();
			return this;
		}
		/// <summary>
		/// Configure the update component of a merge statement
		/// </summary>
		public MergeDefinition<T> Update(/*Func<ValuedPredicateBuilder<T>, ValuedPredicateBuilder<T>>? predicateBuilder = null*/)
		{
			_updateDefinition.Configure();
			return this;
		}
		/// <summary>
		/// Configure the delete component of a merge statement
		/// </summary>
		public MergeDefinition<T> Delete(Func<ValuedPredicateBuilder<T>, ValuedPredicateBuilder<T>>? predicateBuilder = null)
		{
			_deleteDefinition.Configure(predicateBuilder);
			return this;
		}

		//public IEnumerable<IValuedColumnMap> GetInsertFilterColumns() => _insertDefinition.GetFilterColumns();
		//public IEnumerable<IValuedColumnMap> GetUpdateFilterColumns() => _updateDefinition.GetFilterColumns();
		/// <summary>
		/// Gets the filter columns/values for the delete component
		/// </summary>
		public IEnumerable<IValuedColumnMap> GetDeleteFilterColumns() => _deleteDefinition.GetFilterColumns();

		private class ComponentDefinition
		{
			private ValuedPredicateBuilder<T>? _predicate = null;

			internal bool Include => _predicate is { };

			internal ComponentDefinition Configure(Func<ValuedPredicateBuilder<T>, ValuedPredicateBuilder<T>>? predicateBuilder = null)
			{
				_predicate ??= new ValuedPredicateBuilder<T>();
				predicateBuilder?.Invoke(_predicate);

				return this;
			}

			internal IEnumerable<IValuedColumnMap> GetFilterColumns() => _predicate switch
			{
				IValuedPredicateBuilder<T> builder => builder.GetFilterColumns(),
				_ => Enumerable.Empty<IValuedColumnMap>()
			};
		}
	}
}