using System;

namespace Lippert.Core.Data.Contracts
{
	public interface ITableMapBuilder
	{
		bool HandlesType<T>();
		Type GetHandledType();
	}
}