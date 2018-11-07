using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lippert.Core.Data.Contracts
{
	public interface ITableMapBuilder
	{
		Type ModelType { get; }
		bool HandlesType<T>();

		List<(PropertyInfo column, object value)> GetInsertValues();
		List<(PropertyInfo column, object value)> GetUpdateValues();
	}
}