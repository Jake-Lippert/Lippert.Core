using System;

namespace Lippert.Core.Data.Contracts
{
	public interface ITableMapBuilder
	{
		Type ModelType { get; }
		bool HandlesType<T>();
	}

	public interface ITableMapBuilder<TComponent> : ITableMapBuilder
	{
		void SetInsertValues(TComponent component);
		void SetUpdateValues(TComponent component);
	}
}