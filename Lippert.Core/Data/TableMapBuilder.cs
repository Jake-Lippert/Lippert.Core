using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Lippert.Core.Data.Contracts;
using Lippert.Core.Reflection;

namespace Lippert.Core.Data
{
	public abstract class TableMapBuilder<TComponent> : ITableMapBuilder
	{
		Type ITableMapBuilder.ModelType => typeof(TComponent);
		bool ITableMapBuilder.HandlesType<TRecord>() => typeof(TComponent).IsAssignableFrom(typeof(TRecord));

		/// <summary>
		/// Configures component column mappings for a table's map
		/// </summary>
		public abstract void Map<TRecord>(ITableMap<TRecord> tableMap)
			where TRecord : TComponent;


		public virtual List<(PropertyInfo column, object value)> GetInsertValues() => new List<(PropertyInfo, object)>();
		public virtual List<(PropertyInfo column, object value)> GetUpdateValues() => new List<(PropertyInfo, object)>();

		protected (PropertyInfo column, object value) SetValue<TProperty>(Expression<Func<TComponent, TProperty>> column, TProperty value) => (PropertyAccessor.Get(column), value);
	}
}