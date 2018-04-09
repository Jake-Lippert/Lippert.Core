using System;

namespace Lippert.Core.Tests.TestSchema.Contracts
{
	public interface ICreateFields
	{
		Guid CreatedByUserId { get; set; }
		DateTime CreatedDateUtc { get; set; }
	}
}