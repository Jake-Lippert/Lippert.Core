using System.Collections.Generic;

namespace Lippert.Core.Data.QueryBuilders.Contracts
{
	public interface IValuedPredicateBuilder<T> : IPredicateBuilder<T>
	{
		IEnumerable<Data.Contracts.IValuedColumnMap> GetFilterColumns();
	}
}