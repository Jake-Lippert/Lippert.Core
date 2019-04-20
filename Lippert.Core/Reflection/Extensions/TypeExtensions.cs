using System;

namespace Lippert.Core.Reflection.Extensions
{
	public static class TypeExtensions
	{
		public static bool IsAssignableTo<T>(this Type type) => typeof(T).IsAssignableFrom(type);
	}
}