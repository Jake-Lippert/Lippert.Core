using System.Collections.Generic;
using System.Reflection;

namespace Lippert.Core.Data.QueryBuilders.Contracts
{
	public interface IValuedUpdateBuilder<T> : IUpdateBuilder<T>, IValuedPredicateBuilder<T>
	{
		new IEnumerable<Data.Contracts.IValuedColumnMap> GetSetColumns();
		void Set(PropertyInfo propertyInfo, object? value);
	}
}