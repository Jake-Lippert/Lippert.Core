using System;

namespace Lippert.Core.Tests.TestSchema.Contracts
{
	public interface IEditFields
	{
		Guid ModifiedByUserId { get; set; }
		DateTime ModifiedDateUtc { get; set; }
	}
}