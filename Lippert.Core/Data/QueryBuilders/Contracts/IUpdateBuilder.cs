using System.Collections.Generic;

namespace Lippert.Core.Data.QueryBuilders.Contracts
{
	public interface IUpdateBuilder<T> : IPredicateBuilder<T>
	{
		IEnumerable<Data.Contracts.IColumnMap> GetSetColumns();
	}
}