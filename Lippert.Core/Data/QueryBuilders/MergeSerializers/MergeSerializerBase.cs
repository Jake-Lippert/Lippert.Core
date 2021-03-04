using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lippert.Core.Data.QueryBuilders
{
	public abstract class MergeSerializerBase<T> : Contracts.IMergeSerializer<T>
	{
		protected MergeSerializerBase(Dictionary<PropertyInfo, string> aliases) => Aliases = aliases;

		public Dictionary<PropertyInfo, string> Aliases { get; }

		public abstract string SerializeForMerge(IEnumerable<T> records);

		public static object GetPropertyValue(T record, PropertyInfo property) => property.GetValue(record) switch
		{
			Enum e => Convert.ChangeType(e, Enum.GetUnderlyingType(e.GetType())),
			var propertyValue => propertyValue
		};
	}
}