using System.Reflection;

namespace Lippert.Core.Data.Contracts
{
	public interface IColumnMap
	{
		PropertyInfo Property { get; }
		string ColumnName { get; }

		ColumnBehavior Behavior { get; }
		SqlOperation IgnoreOperations { get; }

		IColumnMap Key(bool isGenerated = true);

		IColumnMap Generated(bool allowUpdates = false);

		IColumnMap Ignore(SqlOperation behavior = SqlOperation.Insert | SqlOperation.Update | SqlOperation.Select);
	}
}