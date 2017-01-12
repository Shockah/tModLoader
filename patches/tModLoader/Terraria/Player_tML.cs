using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Terraria
{
	public partial class Player
	{
		internal Action[] resetEffectsHooks;
		internal Action[] updateDeadHooks;
		internal Action<IList<Item>>[] setupStartInventoryHooks;

		private void BuildHooks<R>(out R[] hooks, Func<ModPlayer, R> func) where R : class
		{
			HookBuilder.Build(out hooks, modPlayers, func);
		}

		public void SetupHooks()
		{
			BuildHooks(out resetEffectsHooks, m => m.ResetEffects);
			BuildHooks(out updateDeadHooks, m => m.UpdateDead);
			BuildHooks(out setupStartInventoryHooks, m => m.SetupStartInventory);
		}
	}
}