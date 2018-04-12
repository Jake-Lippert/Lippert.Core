using Lippert.Core.Data.Contracts;

namespace Lippert.Core.Data
{
	public abstract class TableMapBuilder<TComponent> : ITableMapBuilder
	{
		bool ITableMapBuilder.HandlesType<TRecord>() => typeof(TComponent).IsAssignableFrom(typeof(TRecord));

		/// <summary>
		/// Configures component column mappings for a table's map
		/// </summary>
		public abstract void Map<TRecord>(ITableMap<TRecord> tableMap)
			where TRecord : TComponent;
	}
}