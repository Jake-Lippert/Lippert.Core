using System.Reflection;

namespace Lippert.Core.Data.Contracts
{
	public interface IColumnMap
	{
		PropertyInfo Property { get; }
		string ColumnName { get; }

		ColumnBehavior Behavior { get; }
		IgnoreBehavior IgnoreOperations { get; }

		IColumnMap Key(bool isGenerated = true);

		IColumnMap Generated();

		IColumnMap Ignore(IgnoreBehavior behavior = IgnoreBehavior.Insert | IgnoreBehavior.Update | IgnoreBehavior.Select);
	}
}