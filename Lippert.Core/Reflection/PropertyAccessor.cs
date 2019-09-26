using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Lippert.Core.Reflection
{
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
		/// Get the property info for a property specified by an expression
		/// </summary>
		public static bool TryGet<T>(Expression<Func<T, object>> selector, out PropertyInfo propertyInfo) => TryGet<T, object>(selector, out propertyInfo);
		/// <summary>
		/// Get the property info for a property specified by an expression
		/// </summary>
		public static bool TryGet<T, TProperty>(Expression<Func<T, TProperty>> selector, out PropertyInfo propertyInfo)
		{
			try
			{
				return (propertyInfo = Get(selector)) != default;
			}
			catch
			{
				propertyInfo = default;
				return false;
			}
		}
	}
}