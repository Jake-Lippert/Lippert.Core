using System;
using System.Collections.Generic;

namespace Lippert.Core.Reflection.Extensions
{
	public static class TypeExtensions
	{
		public static bool IsAssignableTo(this Type type, Type target) => target.IsAssignableFrom(type);
		public static bool IsAssignableTo<T>(this Type type) => type.IsAssignableTo(typeof(T));

		/// <summary>
		/// Gets all of the types that make up the specified type
		/// </summary>
		/// <param name="seedType">The top-most type to deconstruct</param>
		/// <param name="includeSeed">Should the top-most type be included in the results?</param>
		/// <param name="includeInterfaces">Should interfaces be included in the results?</param>
		public static HashSet<Type> GetBaseTypes(this Type seedType, bool includeSeed, bool includeInterfaces = false)
		{
			var types = new HashSet<Type>();

			if (seedType != typeof(object))
			{
				if (includeSeed)
				{
					types.Add(seedType);
				}

				if (seedType.BaseType is { } baseType)
				{
					types.UnionWith(baseType.GetBaseTypes(true, includeInterfaces));
				}

				if (includeInterfaces)
				{
					types.UnionWith(seedType.GetInterfaces());
				}
			}

			return types;
		}
	}
}