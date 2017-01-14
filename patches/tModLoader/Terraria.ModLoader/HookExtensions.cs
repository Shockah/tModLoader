using System;
using System.Collections.Generic;

namespace Terraria.ModLoader
{
	#region Custom delegates
	public delegate void ActionR<T1>(ref T1 t1);
	public delegate void ActionNR<T1, T2>(T1 t1, ref T2 t2);
	public delegate void ActionRR<T1, T2>(ref T1 t1, ref T2 t2);
	public delegate void ActionRRR<T1, T2, T3>(ref T1 t1, ref T2 t2, ref T3 t3);
	public delegate R FuncNNNRRR<T1, T2, T3, T4, T5, T6, R>(T1 t1, T2 t2, T3 t3, ref T4 t4, ref T5 t5, ref T6 t6);
	public delegate R FuncNRRRRRR<T1, T2, T3, T4, T5, T6, T7, R>(T1 t1, ref T2 t2, ref T3 t3, ref T4 t4, ref T5 t5, ref T6 t6, ref T7 t7);
	public delegate R FuncNNRRRRRRR<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(T1 t1, T2 t2, ref T3 t3, ref T4 t4, ref T5 t5, ref T6 t6, ref T7 t7, ref T8 t8, ref T9 t9);
	#endregion

	public static class HookExtensions
	{
		#region Call
		public static void Call(this IEnumerable<Action> list)
		{
			foreach (var f in list)
				f();
		}

		public static void Call<T1>(this IEnumerable<Action<T1>> list, T1 t1)
		{
			foreach (var f in list)
				f(t1);
		}

		public static void Call<T1, T2>(this IEnumerable<Action<T1, T2>> list, T1 t1, T2 t2)
		{
			foreach (var f in list)
				f(t1, t2);
		}

		public static void Call<T1, T2, T3>(this IEnumerable<Action<T1, T2, T3>> list, T1 t1, T2 t2, T3 t3)
		{
			foreach (var f in list)
				f(t1, t2, t3);
		}

		public static void Call<T1, T2, T3, T4>(this IEnumerable<Action<T1, T2, T3, T4>> list, T1 t1, T2 t2, T3 t3, T4 t4)
		{
			foreach (var f in list)
				f(t1, t2, t3, t4);
		}

		public static void Call<T1, T2, T3, T4, T5>(this IEnumerable<Action<T1, T2, T3, T4, T5>> list, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
		{
			foreach (var f in list)
				f(t1, t2, t3, t4, t5);
		}

		public static void Call<T1, T2, T3, T4, T5, T6>(this IEnumerable<Action<T1, T2, T3, T4, T5, T6>> list, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
		{
			foreach (var f in list)
				f(t1, t2, t3, t4, t5, t6);
		}

		public static void Call<T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<Action<T1, T2, T3, T4, T5, T6, T7>> list, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
		{
			foreach (var f in list)
				f(t1, t2, t3, t4, t5, t6, t7);
		}

		public static void Call<T1, T2, T3, T4, T5, T6, T7, T8>(this IEnumerable<Action<T1, T2, T3, T4, T5, T6, T7, T8>> list, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
		{
			foreach (var f in list)
				f(t1, t2, t3, t4, t5, t6, t7, t8);
		}

		public static void Call<T1>(this IEnumerable<ActionR<T1>> list, ref T1 t1)
		{
			foreach (var f in list)
				f(ref t1);
		}

		public static void Call<T1, T2>(this IEnumerable<ActionNR<T1, T2>> list, T1 t1, ref T2 t2)
		{
			foreach (var f in list)
				f(t1, ref t2);
		}

		public static void Call<T1, T2>(this IEnumerable<ActionRR<T1, T2>> list, ref T1 t1, ref T2 t2)
		{
			foreach (var f in list)
				f(ref t1, ref t2);
		}

		public static void Call<T1, T2, T3>(this IEnumerable<ActionRRR<T1, T2, T3>> list, ref T1 t1, ref T2 t2, ref T3 t3)
		{
			foreach (var f in list)
				f(ref t1, ref t2, ref t3);
		}
		#endregion

		#region CallUntilFalse
		public static bool CallUntilFalse(this IEnumerable<Func<bool>> list)
		{
			foreach (var f in list)
				if (f())
					return false;
			return true;
		}

		public static bool CallUntilFalse<T1>(this IEnumerable<Func<T1, bool>> list, T1 t1)
		{
			foreach (var f in list)
				if (f(t1))
					return false;
			return true;
		}

		public static bool CallUntilFalse<T1, T2>(this IEnumerable<Func<T1, T2, bool>> list, T1 t1, T2 t2)
		{
			foreach (var f in list)
				if (f(t1, t2))
					return false;
			return true;
		}

		public static bool CallUntilFalse<T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<FuncNRRRRRR<T1, T2, T3, T4, T5, T6, T7, bool>> list, T1 t1, ref T2 t2, ref T3 t3, ref T4 t4, ref T5 t5, ref T6 t6, ref T7 t7)
		{
			foreach (var f in list)
				if (f(t1, ref t2, ref t3, ref t4, ref t5, ref t6, ref t7))
					return false;
			return true;
		}
		#endregion

		#region CallUntilNonNull
		public static R CallUntilNonNull<R>(this IEnumerable<Func<R>> list) where R : class
		{
			foreach (var f in list)
			{
				R value = f();
				if (value != null)
					return value;
			}
			return null;
		}

		public static R CallUntilNonNull<T1, R>(this IEnumerable<Func<T1, R>> list, T1 t1) where R : class
		{
			foreach (var f in list)
			{
				R value = f(t1);
				if (value != null)
					return value;
			}
			return null;
		}

		public static R CallUntilNonNull<T1, T2, R>(this IEnumerable<Func<T1, T2, R>> list, T1 t1, T2 t2) where R : class
		{
			foreach (var f in list)
			{
				R value = f(t1, t2);
				if (value != null)
					return value;
			}
			return null;
		}
		#endregion

		#region CallConditionalWithDefaultTrue
		public static bool CallConditionalWithDefaultTrue<T1, T2, T3, T4, T5, T6>(this IEnumerable<FuncNNNRRR<T1, T2, T3, T4, T5, T6, bool>> list, T1 t1, T2 t2, T3 t3, ref T4 t4, ref T5 t5, ref T6 t6)
		{
			bool value = true;
			foreach (var f in list)
			{
				if (!f(t1, t2, t3, ref t4, ref t5, ref t6))
					value = false;
			}
			return value;
		}

		public static bool CallConditionalWithDefaultTrue<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IEnumerable<FuncNNRRRRRRR<T1, T2, T3, T4, T5, T6, T7, T8, T9, bool>> list, T1 t1, T2 t2, ref T3 t3, ref T4 t4, ref T5 t5, ref T6 t6, ref T7 t7, ref T8 t8, ref T9 t9)
		{
			bool value = true;
			foreach (var f in list)
			{
				if (!f(t1, t2, ref t3, ref t4, ref t5, ref t6, ref t7, ref t8, ref t9))
					value = false;
			}
			return value;
		}
		#endregion
	}
}