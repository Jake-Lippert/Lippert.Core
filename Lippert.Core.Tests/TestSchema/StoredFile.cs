using System;
using Lippert.Core.Tests.TestSchema.Contracts;

namespace Lippert.Core.Tests.TestSchema
{
	public class StoredFile : IGuidIdentifier
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public byte[]? FileBytes { get; set; }
	}
}