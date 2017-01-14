using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Terraria
{
	public partial class Player
	{
		internal Action[] ResetEffectsHooks;
		internal Action[] UpdateDeadHooks;
		internal Action<IList<Item>>[] SetupStartInventoryHooks;
		internal Action[] UpdateBiomesHooks;
		internal Func<Player, bool>[] CustomBiomesMatchHooks;
		internal Action<Player>[] CopyCustomBiomesToHooks;
		internal Action[] UpdateBiomeVisualsHooks;
		internal Func<Texture2D>[] GetMapBackgroundImageHooks;
		internal Action[] UpdateBadLifeRegenHooks;
		internal Action[] UpdateLifeRegenHooks;
		internal ActionR<float>[] NaturalLifeRegenHooks;
		internal Action[] PreUpdateHooks;
		internal Action[] SetControlsHooks;
		internal Action[] PreUpdateBuffsHooks;
		internal Action[] PostUpdateBuffsHooks;
		internal ActionRRR<bool, bool, bool>[] UpdateEquipsHooks;
		internal Action[] UpdateVanityAccessoriesHooks;
		internal Action[] PostUpdateEquipsHooks;
		internal Action[] PostUpdateMiscEffectsHooks;
		internal Action[] PostUpdateRunSpeedsHooks;
		internal Action[] PostUpdateHooks;
		internal Action[] FrameEffectsHooks;
		internal FuncNNRRRRRRR<bool, bool, int, int, bool, bool, bool, bool, PlayerDeathReason, bool>[] PreHurtHooks;
		internal Action<bool, bool, double, int, bool>[] HurtHooks;
		internal Action<bool, bool, double, int, bool>[] PostHurtHooks;
		internal FuncNNNRRR<double, int, bool, bool, bool, PlayerDeathReason, bool>[] PreKillHooks;
		internal Action<double, int, bool, PlayerDeathReason>[] KillHooks;
		internal Func<bool>[] PreItemCheckHooks;
		internal Action[] PostItemCheckHooks;
		internal ActionNR<Item, int>[] GetWeaponDamageHooks;
		internal ActionNR<Item, float>[] GetWeaponKnockbackHooks;
		internal Func<Item, Item, bool>[] ConsumeAmmoHooks;
		internal FuncNRRRRRR<Item, Vector2, float, float, int, int, float, bool>[] ShootHooks;
		internal Action<Item, Rectangle>[] MeleeEffectsHooks;
		internal Action<float, float, Entity>[] OnHitAnythingHooks;
		internal Func<Item, NPC, bool?>[] CanHitNPCHooks;
		internal ActionNNRRR<Item, NPC, int, float, bool>[] ModifyHitNPCHooks;
		internal Action<Item, NPC, int, float, bool>[] OnHitNPCHooks;
		internal Func<Projectile, NPC, bool?>[] CanHitNPCWithProjHooks;
		internal ActionNNRRRR<Projectile, NPC, int, float, bool, int>[] ModifyHitNPCWithProjHooks;
		internal Action<Projectile, NPC, int, float, bool>[] OnHitNPCWithProjHooks;
		internal Func<Item, Player, bool>[] CanHitPvpHooks;
		internal ActionNNRR<Item, Player, int, bool>[] ModifyHitPvpHooks;
		internal Action<Item, Player, int, bool>[] OnHitPvpHooks;
		internal Func<Projectile, Player, bool>[] CanHitPvpWithProjHooks;
		internal ActionNNRR<Projectile, Player, int, bool>[] ModifyHitPvpWithProjHooks;
		internal Action<Projectile, Player, int, bool>[] OnHitPvpWithProjHooks;

		private void BuildHooks<R>(out R[] hooks, Func<ModPlayer, R> func) where R : class
		{
			HookBuilder.Build(out hooks, modPlayers, func);
		}

		public void SetupHooks()
		{
			BuildHooks(out ResetEffectsHooks, m => m.ResetEffects);
			BuildHooks(out UpdateDeadHooks, m => m.UpdateDead);
			BuildHooks(out SetupStartInventoryHooks, m => m.SetupStartInventory);
			BuildHooks(out UpdateBiomesHooks, m => m.UpdateBiomes);
			BuildHooks(out CustomBiomesMatchHooks, m => m.CustomBiomesMatch);
			BuildHooks(out CopyCustomBiomesToHooks, m => m.CopyCustomBiomesTo);
			BuildHooks(out UpdateBiomeVisualsHooks, m => m.UpdateBiomeVisuals);
			BuildHooks(out GetMapBackgroundImageHooks, m => m.GetMapBackgroundImage);
			BuildHooks(out UpdateBadLifeRegenHooks, m => m.UpdateBadLifeRegen);
			BuildHooks(out UpdateLifeRegenHooks, m => m.UpdateLifeRegen);
			BuildHooks(out PreUpdateHooks, m => m.PreUpdate);
			BuildHooks(out SetControlsHooks, m => m.SetControls);
			BuildHooks(out PreUpdateBuffsHooks, m => m.PreUpdateBuffs);
			BuildHooks(out PostUpdateBuffsHooks, m => m.PostUpdateBuffs);
			BuildHooks(out UpdateEquipsHooks, m => m.UpdateEquips);
			BuildHooks(out UpdateVanityAccessoriesHooks, m => m.UpdateVanityAccessories);
			BuildHooks(out PostUpdateEquipsHooks, m => m.PostUpdateEquips);
			BuildHooks(out PostUpdateMiscEffectsHooks, m => m.PostUpdateMiscEffects);
			BuildHooks(out PostUpdateRunSpeedsHooks, m => m.PostUpdateRunSpeeds);
			BuildHooks(out PostUpdateHooks, m => m.PostUpdate);
			BuildHooks(out FrameEffectsHooks, m => m.FrameEffects);
			BuildHooks(out PreHurtHooks, m => m.PreHurt);
			BuildHooks(out HurtHooks, m => m.Hurt);
			BuildHooks(out PostHurtHooks, m => m.PostHurt);
			BuildHooks(out PreKillHooks, m => m.PreKill);
			BuildHooks(out KillHooks, m => m.Kill);
			BuildHooks(out PreItemCheckHooks, m => m.PreItemCheck);
			BuildHooks(out PostItemCheckHooks, m => m.PostItemCheck);
			BuildHooks(out GetWeaponDamageHooks, m => m.GetWeaponDamage);
			BuildHooks(out GetWeaponKnockbackHooks, m => m.GetWeaponKnockback);
			BuildHooks(out ConsumeAmmoHooks, m => m.ConsumeAmmo);
			BuildHooks(out ShootHooks, m => m.Shoot);
			BuildHooks(out MeleeEffectsHooks, m => m.MeleeEffects);
			BuildHooks(out OnHitAnythingHooks, m => m.OnHitAnything);
			BuildHooks(out CanHitNPCHooks, m => m.CanHitNPC);
			BuildHooks(out ModifyHitNPCHooks, m => m.ModifyHitNPC);
			BuildHooks(out OnHitNPCHooks, m => m.OnHitNPC);
			BuildHooks(out CanHitNPCWithProjHooks, m => m.CanHitNPCWithProj);
			BuildHooks(out ModifyHitNPCWithProjHooks, m => m.ModifyHitNPCWithProj);
			BuildHooks(out OnHitNPCWithProjHooks, m => m.OnHitNPCWithProj);
			BuildHooks(out CanHitPvpHooks, m => m.CanHitPvp);
			BuildHooks(out ModifyHitPvpHooks, m => m.ModifyHitPvp);
			BuildHooks(out OnHitPvpHooks, m => m.OnHitPvp);
			BuildHooks(out CanHitPvpWithProjHooks, m => m.CanHitPvpWithProj);
			BuildHooks(out ModifyHitPvpWithProjHooks, m => m.ModifyHitPvpWithProj);
			BuildHooks(out OnHitPvpWithProjHooks, m => m.OnHitPvpWithProj);
		}
	}
}