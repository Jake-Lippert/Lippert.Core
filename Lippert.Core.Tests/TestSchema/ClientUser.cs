using System;

namespace Lippert.Core.Tests.TestSchema
{
	public class ClientUser
    {
		public Guid ClientId { get; set; }
		public Guid UserId { get; set; }
		public bool IsActive { get; set; }
    }
}