using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Lippert.Core.Data.Contracts
{
	public interface ITableMap
	{
		string TableName { get; }
		void Table(string name);
		void AutoMap();
	}
	public interface ITableMap<T> : ITableMap
	{
		Dictionary<PropertyInfo, ColumnMap<T>> InstanceColumns { get; }
		List<ColumnMap<T>> InsertColumns { get; }
		List<ColumnMap<T>> UpdateColumns { get; }
		List<ColumnMap<T>> SelectColumns { get; }
		List<ColumnMap<T>> KeyColumns { get; }
		ColumnMap<T> Map(Expression<Func<T, object>> column);
		ColumnMap<T> this[Expression<Func<T, object>> column] { get; }
		ColumnMap<T> this[PropertyInfo property] { get; }
		void AutoMap(params Expression<Func<T, object>>[] includedColumns);
		void AutoMap(Action<ColumnMap<T>> configureColumn);
	}
}