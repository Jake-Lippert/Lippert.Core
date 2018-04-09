using System;

namespace Lippert.Core.Utilities
{
	public static class ActionAdapter
	{
		public static Func<bool> Create(Action action) =>
			new Func<bool>(() => { action(); return true; });
		public static Func<T, bool> Create<T>(Action<T> action) =>
			new Func<T, bool>(x => { action(x); return true; });
		public static Func<T1, T2, bool> Create<T1, T2>(Action<T1, T2> action) =>
			new Func<T1, T2, bool>((x, y) => { action(x, y); return true; });
	}
}