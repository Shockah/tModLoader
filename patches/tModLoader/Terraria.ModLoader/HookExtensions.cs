using System;
using System.Collections.Generic;

namespace Terraria.ModLoader
{
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
		#endregion
	}
}