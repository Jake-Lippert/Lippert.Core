using System;

namespace Lippert.Core.Tests.TestSchema
{
	public class InheritingComponent : BaseRecord
	{
		public new Guid Id { get; set; }
		public Guid BaseId
		{
			get => base.Id;
			set => base.Id = value;
		}
		public string Category { get; set; }
		public decimal Cost { get; set; }
	}
}