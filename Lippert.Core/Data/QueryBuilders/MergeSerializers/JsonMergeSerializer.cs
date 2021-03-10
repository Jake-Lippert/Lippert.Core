using System.Collections.Generic;
using Lippert.Core.Collections.Extensions;
using Newtonsoft.Json.Linq;

namespace Lippert.Core.Data.QueryBuilders.MergeSerializers
{
	/// <summary>
	/// A merge serializer that converts models into json, as needed by <see cref="SqlServerMergeQueryBuilder"/>
	/// </summary>
	/// <typeparam name="T">The type to be serialized for merge operations</typeparam>
	public class JsonMergeSerializer<T> : MergeSerializerBase<T>
	{
		public JsonMergeSerializer(Data.Contracts.ITableMap<T> tableMap)
			: base(tableMap) { }

		/// <summary>
		/// Serializes the specified records into json that can be used with <see cref="SqlServerMergeQueryBuilder"/>'s sql
		/// </summary>
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

		/// <summary>
		/// Build the 'minification' alias to be used when parsing the serialized records
		/// </summary>
		public override string BuildColumnParserAlias(string? alias) => $"$._{alias}";
	}
}