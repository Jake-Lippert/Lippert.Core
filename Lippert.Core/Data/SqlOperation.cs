namespace Lippert.Core.Data
{
	[System.Flags]
	public enum SqlOperation
	{
		None = 0,
		Insert = 1,
		Update = 2,
		Select = 4,
		All = Insert | Update | Select
	}
}