using System;

namespace Lippert.Core.Extensions
{
	public static class DateTimeExtensions
    {
		/// <summary>
		/// Creates a new System.DateTime object that has the same number of ticks as the
		/// specified System.DateTime, but is designated as either local time, Coordinated
		/// Universal Time (UTC), or neither, as indicated by the specified System.DateTimeKind value.
		/// </summary>
		public static DateTime SpecifyKind(this DateTime value, DateTimeKind kind) => DateTime.SpecifyKind(value, kind);
    }
}