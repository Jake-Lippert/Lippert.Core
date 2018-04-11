using System;
using Lippert.Core.Tests.TestSchema.Contracts;

namespace Lippert.Core.Tests.TestSchema
{
	public class BaseRecord : IGuidIdentifier
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
	}
}