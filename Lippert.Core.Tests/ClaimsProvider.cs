using System;

namespace Lippert.Core.Tests
{
	public static class ClaimsProvider
	{
		public static Claims UserClaims { get; } = new Claims();

		public class Claims
		{
			public Guid ClientId { get; set; }
			public Guid UserId { get; set; }
		}
	}
}