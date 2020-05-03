using System;
using System.Linq.Expressions;
using System.Reflection;
using Lippert.Core.Data.Contracts;
using Lippert.Core.Reflection;

namespace Lippert.Core.Data
{
	public class ColumnMap<T> : IColumnMap
	{
		internal ColumnMap(Expression<Func<T, object?>> column)
			: this(PropertyAccessor.Get(column)) { }
		internal ColumnMap(PropertyInfo property) => ColumnName = (Property = property).Name;


		public PropertyInfo Property { get; }
		public string ColumnName { get; }

		public ColumnBehavior Behavior { get; private set; } = ColumnBehavior.Basic;
		public SqlOperation IgnoreOperations { get; private set; } = 0;

		public IColumnMap Key(bool isGenerated = true)
		{
			if (IgnoreOperations.HasFlag(SqlOperation.Select))
			{
				throw new InvalidOperationException($"Column '{ColumnName}' cannot be a key because select operations are ignored.");
			}

			Behavior |= ColumnBehavior.Key;

			if (isGenerated)
			{
				return Generated();
			}
			else
			{
				IgnoreOperations |= SqlOperation.Update;
			}

			return this;
		}

		public IColumnMap Generated(bool allowUpdates = false)
		{
			if (IgnoreOperations.HasFlag(SqlOperation.Select))
			{
				throw new InvalidOperationException($"Column '{ColumnName}' cannot be generated because select operations are ignored.");
			}
			
			Behavior |= ColumnBehavior.Generated;
			IgnoreOperations |= SqlOperation.Insert | (allowUpdates ? 0 : SqlOperation.Update);

			return this;
		}

		public IColumnMap Ignore(SqlOperation behavior = SqlOperation.Insert | SqlOperation.Update | SqlOperation.Select)
		{
			if (behavior.HasFlag(SqlOperation.Select))
			{
				if (Behavior.HasFlag(ColumnBehavior.Key))
				{
					throw new InvalidOperationException($"Column '{ColumnName}' cannot ignore select operations because it is a key.");
				}
				if (Behavior.HasFlag(ColumnBehavior.Generated))
				{
					throw new InvalidOperationException($"Column '{ColumnName}' cannot ignore select operations because it is generated.");
				}
			}

			IgnoreOperations |= behavior;

			return this;
		}


		public override string ToString() => $"{typeof(T).Name}: {Property.DeclaringType.Name}.{Property.Name}";
	}
}