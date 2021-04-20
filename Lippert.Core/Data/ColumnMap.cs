using System;
using System.Linq.Expressions;
using System.Reflection;
using Lippert.Core.Data.Contracts;
using Lippert.Core.Reflection;

namespace Lippert.Core.Data
{
	public class ColumnMap<T> : IColumnMap
	{
		internal ColumnMap(PropertyInfo property)
		{
			Property = property;
			ColumnName = Property.Name;
			if (Property.PropertyType == typeof(string))
			{
				Length = int.MaxValue;
			}
		}

		internal ColumnMap(Expression<Func<T, object?>> column)
			: this(PropertyAccessor.Get(column)) { }
		internal ColumnMap(Expression<Func<T, string?>> column, int length)
			: this(PropertyAccessor.Get(column))
		{
			Length = length;
		}
		internal ColumnMap(Expression<Func<T, decimal?>> column, int precision, int scale)
			: this(PropertyAccessor.Get(column))
		{
			Precision = precision;
			Scale = scale;
		}
		internal ColumnMap(Expression<Func<T, float?>> column, int precision, int scale)
			: this(PropertyAccessor.Get(column))
		{
			Precision = precision;
			Scale = scale;
		}
		internal ColumnMap(Expression<Func<T, double?>> column, int precision, int scale)
			: this(PropertyAccessor.Get(column))
		{
			Precision = precision;
			Scale = scale;
		}


		public PropertyInfo Property { get; }
		public string ColumnName { get; }

		public ColumnBehavior Behavior { get; private set; } = ColumnBehavior.Basic;
		public SqlOperation IgnoreOperations { get; private set; } = 0;

		public int Length { get; internal set; }
		public int Precision { get; internal set; }
		public int Scale { get; internal set; }

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