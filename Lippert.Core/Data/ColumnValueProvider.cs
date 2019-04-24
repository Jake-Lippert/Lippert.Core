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
				foreach (var (propertyInfo, value) in provider.GetInsertValues())
				{
					propertyInfo.SetValue(record, value);
				}
			}
		}

		public static void ApplyUpdateValues<TRecord>(TRecord record)
		{
			foreach (var provider in _tableMapBuilders[typeof(TRecord)])
			{
				foreach (var (propertyInfo, value) in provider.GetUpdateValues())
				{
					propertyInfo.SetValue(record, value);
				}
			}
		}

		public static void ApplyUpdateBuilderValues<TRecord>(QueryBuilders.Contracts.IValuedUpdateBuilder<TRecord> updateBuilder)
		{
			foreach (var provider in _tableMapBuilders[typeof(TRecord)])
			{
				foreach (var (propertyInfo, value) in provider.GetUpdateValues())
				{
					updateBuilder.Set(propertyInfo, value);
				}
			}
		}
	}
}