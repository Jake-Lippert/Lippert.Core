using System;
using System.Collections.Generic;
using Lippert.Core.Collections;
using Lippert.Core.Data.Contracts;

namespace Lippert.Core.Data
{
	public static class ColumnValueProvider
	{
		private static readonly IDictionary<Type, List<ITableMapBuilder>> _tableMapBuilders;

		static ColumnValueProvider()
		{
			_tableMapBuilders = RetrievalDictionary.Build((Type type) => TableMapSource.GetTableMapBuilders(type));
		}


		public static void ApplyInsertValues<TRecord>(TRecord record)
		{
			foreach (var provider in _tableMapBuilders[typeof(TRecord)])
			{
				var setInsertValues = typeof(ITableMapBuilder<>).MakeGenericType(provider.ModelType).GetMethod(nameof(ITableMapBuilder<object>.SetInsertValues));
				setInsertValues.Invoke(provider, new object?[] { record });
			}
		}

		public static void ApplyUpdateValues<TRecord>(TRecord record)
		{
			foreach (var provider in _tableMapBuilders[typeof(TRecord)])
			{
				var setUpdateValues = typeof(ITableMapBuilder<>).MakeGenericType(provider.ModelType).GetMethod(nameof(ITableMapBuilder<object>.SetUpdateValues));
				setUpdateValues.Invoke(provider, new object?[] { record });
			}
		}
	}
}