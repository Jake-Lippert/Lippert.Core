using System;

namespace Lippert.Core.Tests.TestSchema
{
	public class User : Contracts.IGuidIdentifier
	{
		public Guid Id { get; set; }
	}
}