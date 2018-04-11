using System;
using Lippert.Core.Data.Contracts;

namespace Lippert.Core.Data
{
	public abstract class TableMapBuilder<TRecord> : ITableMapBuilder
	{
		bool ITableMapBuilder.HandlesType<T>() => typeof(TRecord).IsAssignableFrom(typeof(T));
		Type ITableMapBuilder.GetHandledType() => typeof(TRecord);

		public abstract void Map<TComponent>(ITableMap<TComponent> tableMap)
			where TComponent : TRecord;
	}
}