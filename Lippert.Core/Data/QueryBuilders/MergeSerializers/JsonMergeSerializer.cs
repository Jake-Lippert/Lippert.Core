using System.Collections.Generic;
using System.Reflection;
using Lippert.Core.Collections.Extensions;
using Newtonsoft.Json.Linq;

namespace Lippert.Core.Data.QueryBuilders.MergeSerializers
{
	public class JsonMergeSerializer<T> : MergeSerializerBase<T>
	{
		public JsonMergeSerializer(Dictionary<PropertyInfo, string> aliases)
			: base(aliases) { }

		public override string SerializeForMerge(IEnumerable<T> records)
		{
			var toSerialize = new JArray();
			foreach (var (record, index) in records.Indexed())
			{
				var jsonRecord = new JObject(new JProperty("_", index));
				foreach (var (property, alias) in Aliases.AsTuples())
				{
					jsonRecord.Add(new JProperty($"_{alias}", GetPropertyValue(record, property)));
				}

				toSerialize.Add(jsonRecord);
			}

			return toSerialize.ToString(Newtonsoft.Json.Formatting.None);
		}
	}
}