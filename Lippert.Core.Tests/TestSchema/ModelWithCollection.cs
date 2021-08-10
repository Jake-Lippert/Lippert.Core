using System;
using System.Collections.Generic;

namespace Lippert.Core.Tests.TestSchema
{
	public class ModelWithCollection : Contracts.IGuidIdentifier
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public List<User> Users { get; set; } = new List<User>();
	}
}