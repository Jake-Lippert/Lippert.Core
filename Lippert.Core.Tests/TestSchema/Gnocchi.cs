namespace Lippert.Core.Tests.TestSchema
{
	public class Gnocchi
	{
		public long Id { get; set; }//--Don't actually configure this as a key (No-Key => Gnocchi)
		public string? Name { get; set; }
		public decimal Rating { get; set; }
	}
}