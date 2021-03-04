using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Lippert.Core.Collections.Extensions;

namespace Lippert.Core.Data.QueryBuilders.MergeSerializers
{
	public class XmlMergeSerializer<T> : MergeSerializerBase<T>
	{
		public XmlMergeSerializer(Dictionary<PropertyInfo, string> aliases)
			: base(aliases) { }

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
	}
}