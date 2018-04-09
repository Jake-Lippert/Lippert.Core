using System;

namespace Lippert.Core.Utilities
{
	/// <remarks>
	/// Ended up very similar to https://gist.github.com/Coeur/914e9459b1aaab979635
	/// </remarks>
	public static class TypeSwitch
	{
		public static FuncCase<TSource, TResult> On<TSource, TResult>(TSource value) => new FuncCase<TSource, TResult>(value);

		/// <summary>
		/// Internal class used by the <see cref="TypeSwitch"/> static class.
		/// </summary>
		/// <typeparam name="TSource">The source type.</typeparam>
		public sealed class FuncCase<TSource, TReturn>
		{
			/// <summary>
			/// The source value.
			/// </summary>
			private readonly TSource _value;
			/// <summary>
			/// Whether a switch case handled the value.
			/// </summary>
			private bool _handled;
			private TReturn _result;

			/// <summary>
			/// Initializes a new instance of the <see cref="FuncCase{TSource}"/> class.
			/// </summary>
			/// <param name="value">The switch value.</param>
			internal FuncCase(TSource value) => _value = value;

			/// <summary>
			/// Executes the specified piece of code when the type of the argument is assignable to the
			/// specified type.
			/// </summary>
			/// <typeparam name="TCase">The target type.</typeparam>
			/// <param name="func">The action to execute.</param>
			/// <returns>An object on which further switch cases can be specified.</returns>
			public FuncCase<TSource, TReturn> Case<TCase>(Func<TReturn> func)
				where TCase : TSource => Case<TCase>(x => func());

			/// <summary>
			/// Executes the specified piece of code when the type of the argument is assignable to the
			/// specified type.
			/// </summary>
			/// <typeparam name="TCase">The target type.</typeparam>
			/// <param name="func">The action to execute.</param>
			/// <returns>An object on which further switch cases can be specified.</returns>
			public FuncCase<TSource, TReturn> Case<TCase>(Func<TCase, TReturn> func)
				where TCase : TSource
			{
				if (!_handled && _value is TCase caseValue)
				{
					_result = (func ?? throw new ArgumentNullException(nameof(func)))(caseValue);
					_handled = true;
				}

				return this;
			}

			/// <summary>
			/// Executes the specified piece of code when none of the other cases handles the specified type.
			/// </summary>
			/// <param name="func">The action to execute.</param>
			public TReturn Default(Func<TReturn> func) => Default(x => func());

			/// <summary>
			/// Executes the specified piece of code when none of the other cases handles the specified type.
			/// </summary>
			/// <param name="func">The action to execute.</param>
			public TReturn Default(Func<TSource, TReturn> func)
			{
				if (_handled)
				{
					return _result;
				}

				return (func ?? throw new ArgumentNullException(nameof(func)))(_value);
			}

			public TReturn Result() => Default(() => throw new InvalidOperationException($"{nameof(Result)}() was called on a {nameof(TypeSwitch)} 'block' which had no matching {nameof(Case)} statements."));
		}
	}
}