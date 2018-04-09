using System;

namespace Lippert.Core.Extensions
{
	public static class ObjectExtensions
    {
		public static TResult With<T, TResult>(this T source, Func<T, TResult> selector) => selector(source);
	}
}