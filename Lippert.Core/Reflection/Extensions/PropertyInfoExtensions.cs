using System;
using System.Linq;
using System.Reflection;
using Lippert.Core.Extensions;

namespace Lippert.Core.Reflection.Extensions
{
	public static class PropertyInfoExtensions
	{
		/// <summary>
		/// Get an interface's version of a class's property or a class's version of an interface's property
		/// </summary>
		public static PropertyInfo Get<TTarget>(this PropertyInfo property) => Get(property, typeof(TTarget));
		/// <summary>
		/// Get an interface's version of a class's property or a class's version of an interface's property
		/// </summary>
		/// <seealso cref="https://stackoverflow.com/a/3285867/595473"/>
		public static PropertyInfo Get(this PropertyInfo property, Type targetType)
		{
			MethodInfo targetMethod = default;
			var getter = property.GetMethod;
			if (targetType.IsInterface)
			{
				if (property.DeclaringType.IsInterface)
				{
					throw new InvalidOperationException($"The type '{targetType.Name}' and the declaring type of property '{nameof(property)}' cannot both be interfaces.");
				}

				var mapping = property.DeclaringType.GetInterfaceMap(targetType);

				//--Get the interface's version of a class's property
				targetMethod = mapping.TargetMethods.Zip(mapping.InterfaceMethods, (target, @interface) => (target, @interface))
					.FirstOrDefault(x => x.target == getter).@interface;
			}
			else
			{
				if (!property.DeclaringType.IsInterface)
				{
					throw new InvalidOperationException($"The type '{targetType.Name}' and the declaring type of property '{nameof(property)}' cannot both be classes.");
				}

				var mapping = targetType.GetInterfaceMap(property.DeclaringType);

				//--Get the class's version of an interface's property
				targetMethod = mapping.InterfaceMethods.Zip(mapping.TargetMethods, (@interface, target) => (@interface, target))
					.FirstOrDefault(x => x.@interface == getter).target;
			}

			return targetMethod?.With(target => targetType.GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.NonPublic)
				.FirstOrDefault(targetProperty => target == targetProperty.GetMethod));
		}

		/// <summary>
		/// Gets the declaring type of a property; if desired, the the type of the interface that defines the property can be returned instead
		/// </summary>
		public static (Type type, PropertyInfo property) GetDeclaringType(this PropertyInfo property, bool getInterface = false)
		{
			if (getInterface)
			{
				foreach (var @interface in property.DeclaringType.GetInterfaces())
				{
					if (property.TryGet(@interface, out var propertyInfo))
					{
						return (@interface, propertyInfo);
					}
				}
			}

			return (property.DeclaringType, property);
		}

		/// <summary>
		/// Get an interface's version of a class's property or a class's version of an interface's property
		/// </summary>
		public static bool TryGet<TTarget>(this PropertyInfo property, out PropertyInfo propertyInfo) => TryGet(property, typeof(TTarget), out propertyInfo);
		/// <summary>
		/// Get an interface's version of a class's property or a class's version of an interface's property
		/// </summary>
		public static bool TryGet(this PropertyInfo property, Type targetType, out PropertyInfo propertyInfo)
		{
			try
			{
				return (propertyInfo = Get(property, targetType)) != default;
			}
			catch
			{
				propertyInfo = default;
				return false;
			}
		}
	}
}