using System;
using Lippert.Core.Tests.TestSchema.Contracts;

namespace Lippert.Core.Tests.TestSchema
{
	public class Purchase : IGuidIdentifier
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public decimal Cost { get; set; }
	}
}