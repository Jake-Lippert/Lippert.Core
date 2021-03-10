using System.Collections.Generic;
using System.Xml.Linq;
using Lippert.Core.Collections.Extensions;

namespace Lippert.Core.Data.QueryBuilders.MergeSerializers
{
	/// <summary>
	/// A merge serializer that converts models into xml, as needed by <see cref="SqlServerMergeQueryBuilder"/>
	/// </summary>
	/// <typeparam name="T">The type to be serialized for merge operations</typeparam>
	public class XmlMergeSerializer<T> : MergeSerializerBase<T>
	{
		public XmlMergeSerializer(Data.Contracts.ITableMap<T> tableMap)
			: base(tableMap) { }

		/// <summary>
		/// Serializes the specified records into xml that can be used with <see cref="SqlServerMergeQueryBuilder"/>'s sql
		/// </summary>
		public override string SerializeForMerge(IEnumerable<T> records)
		{
			var toSerialize = new XElement("_");
			foreach (var (record, index) in records.Indexed())
			{
				var xmlRecord = new XElement("_", new XAttribute("_", index));
				foreach (var (property, alias) in Aliases.AsTuples())
				{
					//--Don't serialize null values
					if (GetPropertyValue(record, property) is { } value)
					{
						xmlRecord.Add(new XAttribute($"_{alias}", value));
					}
				}

				toSerialize.Add(xmlRecord);
			}

			return toSerialize.ToString(SaveOptions.DisableFormatting);
		}

		/// <summary>
		/// Build the 'minification' alias to be used when parsing the serialized records
		/// </summary>
		public override string BuildColumnParserAlias(string? alias) => $"@_{alias}";
	}
}