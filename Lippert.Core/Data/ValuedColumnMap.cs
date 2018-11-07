using System.Reflection;
using Lippert.Core.Data.Contracts;

namespace Lippert.Core.Data
{
	public class ValuedColumnMap : IValuedColumnMap
	{
		private readonly IColumnMap _column;

		internal ValuedColumnMap(IColumnMap column, object value)
		{
			_column = column;
			Value = value;
		}


		public object Value { get; }

		public PropertyInfo Property => _column.Property;

		public string ColumnName => _column.ColumnName;

		public ColumnBehavior Behavior => _column.Behavior;

		public SqlOperation IgnoreOperations => _column.IgnoreOperations;

		public IColumnMap Generated(bool allowUpdates = false) => _column.Generated(allowUpdates);
		public IColumnMap Ignore(SqlOperation behavior = SqlOperation.Insert | SqlOperation.Update | SqlOperation.Select) => _column.Ignore(behavior);
		public IColumnMap Key(bool isGenerated = true) => _column.Key(isGenerated);
	}
}