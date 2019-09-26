using System;
using System.Collections.Generic;

namespace Lippert.Core.Reflection.Extensions
{
	public static class TypeExtensions
	{
		public static bool IsAssignableTo<T>(this Type type) => typeof(T).IsAssignableFrom(type);

		/// <summary>
		/// Gets all of the types that make up the specified type
		/// </summary>
		/// <param name="seedType">The top-most type to deconstruct</param>
		/// <param name="includeSeed">Should the top-most type be included in the results?</param>
		/// <param name="includeInterfaces">Should interfaces be included in the results?</param>
		public static IEnumerable<Type> GetBaseTypes(this Type seedType, bool includeSeed, bool includeInterfaces = false)
		{
			if (seedType != typeof(object))
			{
				if (includeSeed)
				{
					yield return seedType;
				}

				foreach (var type in seedType.BaseType.GetBaseTypes(includeSeed, includeInterfaces))
				{
					yield return type;
				}

				if (includeInterfaces)
				{
					foreach (var type in seedType.GetInterfaces())
					{
						yield return type;
					}
				}
			}
		}
	}
}