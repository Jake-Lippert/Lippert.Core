using System.Collections.Generic;
using Lippert.Core.Data.Contracts;

namespace Lippert.Core.Data.QueryBuilders.Contracts
{
	public interface IPredicateBuilder<T>
	{
		ITableMap<T> TableMap { get; }
		IEnumerable<IColumnMap> GetFilterColumns(bool defaultToKey);
	}
}