namespace Lippert.Core.Data.Contracts
{
	public interface IValuedColumnMap : IColumnMap
	{
		object Value { get; }
	}
}