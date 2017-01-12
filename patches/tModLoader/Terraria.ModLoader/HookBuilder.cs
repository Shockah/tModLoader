using System;
using System.Collections.Generic;
using System.Linq;

namespace Terraria.ModLoader
{
	public static class HookBuilder
	{
		private static IEnumerable<R> DependencySorted<T, R>(IDictionary<T, R> list) where R : class
		{
			IList<ModConstraint> constraints = new List<ModConstraint>();
			IDictionary<T, ModCallOrderAttribute[]> modCallAttribs = new Dictionary<T, ModCallOrderAttribute[]>();
			foreach (KeyValuePair<T, R> kvp in list)
			{
				ModCallOrderAttribute[] attribs = (kvp.Value as Delegate).Method.GetCustomAttributes(typeof(ModCallOrderAttribute), false) as ModCallOrderAttribute[];
				modCallAttribs[kvp.Key] = attribs;
				foreach (ModCallOrderAttribute attrib in attribs)
				{
					if (attrib.order == CallOrder.Default)
						continue;

					ModConstraint constraint = new ModConstraint(kvp.Key.GetType().FullName, attrib.type);
					if (attrib.order == CallOrder.Late)
						constraint = constraint.Reverse();

					ModConstraint reverseConstraint = constraint.Reverse();
					if (constraints.Contains(reverseConstraint))
						constraints.Remove(reverseConstraint);
					else
						constraints.Add(constraint);
				}
			}

			List<Tuple<T, R>> ordered = new List<Tuple<T, R>>();
			foreach (KeyValuePair<T, R> kvp in list)
			{
				ordered.Add(Tuple.Create(kvp.Key, kvp.Value));
			}

			ordered.Sort((pair1, pair2) => {
				string type1 = pair1.Item1.GetType().FullName;
				string type2 = pair2.Item1.GetType().FullName;

				foreach (ModConstraint constraint in constraints)
				{
					if (constraint.type1 == type1 && constraint.type2 == type2)
						return -1;
					if (constraint.type1 == type2 && constraint.type2 == type1)
						return 1;
				}

				return 0;
			});

			return ordered.Select(pair => pair.Item2);
		}

		public static IEnumerable<R> Build<T, R>(IEnumerable<T> list, Func<T, R> func) where R : class
		{
			IList<Tuple<T, R>>[] order = new IList<Tuple<T, R>>[Enum.GetValues(typeof(CallOrder)).Length];
			for (int i = 0; i < order.Length; i++)
			{
				order[i] = new List<Tuple<T, R>>();
			}

			foreach (T t in list)
			{
				R hook = func(t);
				CallOrderAttribute[] attribs = (hook as Delegate).Method.GetCustomAttributes(typeof(CallOrderAttribute), false) as CallOrderAttribute[];
				CallOrder callOrder = attribs.Length == 1 ? attribs[0].order : CallOrder.Default;
				order[(int)callOrder].Add(Tuple.Create(t, hook));
			}

			return order.Select(hooks => DependencySorted(hooks.ToDictionary(pair => pair.Item1, pair => pair.Item2))).SelectMany(hook => hook);
		}

		public static void Build<T, R>(out R[] hooks, IEnumerable<T> list, Func<T, R> func) where R : class
		{
			hooks = Build(list, func).ToArray();
		}
	}

	internal sealed class ModConstraint
	{
		public readonly string type1;
		public readonly string type2;

		public ModConstraint(string type1, string type2)
		{
			this.type1 = type1;
			this.type2 = type2;
		}

		public override int GetHashCode()
		{
			int hash = 13;
			hash = hash * 7 + type1.GetHashCode();
			hash = hash * 7 + type2.GetHashCode();
			return hash;
		}

		public override bool Equals(object obj)
		{
			ModConstraint o = obj as ModConstraint;
			if (o == null)
				return false;
			return type1 == o.type1 && type2 == o.type2;
		}

		public ModConstraint Reverse()
		{
			return new ModConstraint(type2, type1);
		}
	}

	[AttributeUsage(AttributeTargets.Method)]
	public sealed class CallOrderAttribute : Attribute
	{
		public readonly CallOrder order;

		public CallOrderAttribute(CallOrder order)
		{
			this.order = order;
		}
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public sealed class ModCallOrderAttribute : Attribute
	{
		public readonly CallOrder order;
		public readonly string type;

		public ModCallOrderAttribute(CallOrder order, string type)
		{
			this.order = order;
			this.type = type;
		}
	}

	public enum CallOrder
	{
		Early, Default, Late
	}
}