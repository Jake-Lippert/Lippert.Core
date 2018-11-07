using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Lippert.Core.Extensions;

namespace Lippert.Core.Reflection
{
	/// <seealso cref="https://stackoverflow.com/a/3285867/595473"/>
	public static class PropertyAccessor
	{
		/// <summary>
		/// Get the property info for a property specified by an expression
		/// </summary>
		public static PropertyInfo Get<T>(Expression<Func<T, object>> selector) => Get<T, object>(selector);
		/// <summary>
		/// Get the property info for a property specified by an expression
		/// </summary>
		public static PropertyInfo Get<T, TProperty>(Expression<Func<T, TProperty>> selector)
		{
			if (selector is LambdaExpression lambda)
			{
				var memberExpression = ExtractMemberExpression(lambda.Body);
				if (memberExpression == null)
				{
					throw new ArgumentException("Selector must be member access expression", nameof(selector));
				}
				if (memberExpression.Member.DeclaringType == default)
				{
					throw new InvalidOperationException("Property does not have declaring type");
				}

				return memberExpression.Member.DeclaringType.GetProperty(memberExpression.Member.Name);
			}

			throw new ArgumentException("Selector must be lambda expression", nameof(selector));

			MemberExpression ExtractMemberExpression(Expression expression)
			{
				switch (expression)
				{
					case MemberExpression memberExpression:
						return memberExpression;
					case UnaryExpression unaryExpression:
						return ExtractMemberExpression(unaryExpression.Operand);
					default:
						return default;
				}
			}
		}

		/// <summary>
		/// Get an interface's version of a class's property or a class's version of an interface's property
		/// </summary>
		public static PropertyInfo Get<TTarget>(PropertyInfo property) => Get(property, typeof(TTarget));
		/// <summary>
		/// Get an interface's version of a class's property or a class's version of an interface's property
		/// </summary>
		public static PropertyInfo Get(PropertyInfo property, Type targetType)
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
	}
}