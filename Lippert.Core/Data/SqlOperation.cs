namespace Lippert.Core.Data
{
	[System.Flags]
	public enum SqlOperation
	{
		None = 0,
		Insert = 1 << 0,
		Update = 1 << 1,
		Select = 1 << 2
	}
}