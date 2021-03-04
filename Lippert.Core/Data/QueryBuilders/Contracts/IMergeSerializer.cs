using System.Collections.Generic;

namespace Lippert.Core.Data.QueryBuilders.Contracts
{
	public interface IMergeSerializer<T>
	{
		Dictionary<System.Reflection.PropertyInfo, string> Aliases { get; }
		string SerializeForMerge(IEnumerable<T> records);
	}
}