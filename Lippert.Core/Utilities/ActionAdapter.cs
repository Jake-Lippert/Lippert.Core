using System;

namespace Lippert.Core.Utilities
{
	public static class ActionAdapter
	{
		public static Func<bool> Create(Action action) => () => { action(); return true; };
		public static Func<T, bool> Create<T>(Action<T> action) => (T t) => { action(t); return true; };
		public static Func<T1, T2, bool> Create<T1, T2>(Action<T1, T2> action) => (T1 t1, T2 t2) => { action(t1, t2); return true; };
		public static Func<T1, T2, T3, bool> Create<T1, T2, T3>(Action<T1, T2, T3> action) => (T1 t1, T2 t2, T3 t3) => { action(t1, t2, t3); return true; };
		public static Func<T1, T2, T3, T4, bool> Create<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action) => (T1 t1, T2 t2, T3 t3, T4 t4) => { action(t1, t2, t3, t4); return true; };
		public static Func<T1, T2, T3, T4, T5, bool> Create<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action) => (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) => { action(t1, t2, t3, t4, t5); return true; };
		public static Func<T1, T2, T3, T4, T5, T6, bool> Create<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action) => (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) => { action(t1, t2, t3, t4, t5, t6); return true; };
	}
}