using System.Collections.Generic;

namespace Lippert.Core.Data.QueryBuilders.Contracts
{
	/// <summary>
	/// An interface for merge serializers to be used by <see cref="SqlServerMergeQueryBuilder"/>
	/// </summary>
	/// <typeparam name="T">The type to be serialized for merge operations</typeparam>
	public interface IMergeSerializer<T>
	{
		/// <summary>
		/// The table map for the model being serialized
		/// </summary>
		Data.Contracts.ITableMap<T> TableMap { get; }
		/// <summary>
		/// A mapping from model properties to their 'minified' names used during serialization
		/// </summary>
		Dictionary<System.Reflection.PropertyInfo, string> Aliases { get; }
		/// <summary>
		/// Serializes the specified records into a version that can be used with <see cref="SqlServerMergeQueryBuilder"/>'s sql
		/// </summary>
		string SerializeForMerge(IEnumerable<T> records);
		/// <summary>
		/// Builds a parser for the specified column in order to deserialize the 'minified' representation into something that can be easily used within merge statements
		/// </summary>
		string BuildColumnParser(Data.Contracts.IColumnMap? columnMap);
	}
}