using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader.Default;
using Terraria.DataStructures;
using Terraria.GameInput;

namespace Terraria.ModLoader
{
	public static class PlayerHooks
	{
		private static readonly IList<ModPlayer> players = new List<ModPlayer>();
		private static readonly IDictionary<string, int> indexes = new Dictionary<string, int>();

		internal static void Add(ModPlayer player)
		{
			indexes[player.mod.Name + ':' + player.Name] = players.Count;
			players.Add(player);
		}

		internal static void Unload()
		{
			players.Clear();
			indexes.Clear();
		}

		internal static void SetupPlayer(Player player)
		{
			player.modPlayers = players.Select(modPlayer => modPlayer.CreateFor(player)).ToArray();
			player.SetupHooks();
		}

		internal static ModPlayer GetModPlayer(Player player, Mod mod, string name)
		{
			int index;
			return indexes.TryGetValue(mod.Name + ':' + name, out index) ? player.modPlayers[index] : null;
		}

		public static void ResetEffects(Player player)
		{
			player.ResetEffectsHooks.Call();
		}

		public static void UpdateDead(Player player)
		{
			player.UpdateDeadHooks.Call();
		}

		public static IList<Item> SetupStartInventory(Player player)
		{
			IList<Item> items = new List<Item>();
			Item item = new Item();
			item.SetDefaults("Copper Shortsword");
			item.Prefix(-1);
			items.Add(item);
			item = new Item();
			item.SetDefaults("Copper Pickaxe");
			item.Prefix(-1);
			items.Add(item);
			item = new Item();
			item.SetDefaults("Copper Axe");
			item.Prefix(-1);
			items.Add(item);
			player.SetupStartInventoryHooks.Call(items);
			IDictionary<int, int> counts = new Dictionary<int, int>();
			foreach (Item item0 in items)
			{
				if (item0.maxStack > 1)
				{
					if (!counts.ContainsKey(item0.netID))
					{
						counts[item0.netID] = 0;
					}
					counts[item0.netID] += item0.stack;
				}
			}
			int k = 0;
			while (k < items.Count)
			{
				bool flag = true;
				int id = items[k].netID;
				if (counts.ContainsKey(id))
				{
					items[k].stack = counts[id];
					if (items[k].stack > items[k].maxStack)
					{
						items[k].stack = items[k].maxStack;
					}
					counts[id] -= items[k].stack;
					if (items[k].stack <= 0)
					{
						items.RemoveAt(k);
						flag = false;
					}
				}
				if (flag)
				{
					k++;
				}
			}
			return items;
		}

		public static void SetStartInventory(Player player, IList<Item> items)
		{
			if (items.Count <= 50)
			{
				for (int k = 0; k < items.Count; k++)
				{
					player.inventory[k] = items[k];
				}
			}
			else
			{
				for (int k = 0; k < 49; k++)
				{
					player.inventory[k] = items[k];
				}
				Item bag = new Item();
				bag.SetDefaults(ModLoader.GetMod("ModLoader").ItemType("StartBag"));
				for (int k = 49; k < items.Count; k++)
				{
					((StartBag)bag.modItem).AddItem(items[k]);
				}
				player.inventory[49] = bag;
			}
		}

		public static void SetStartInventory(Player player)
		{
			SetStartInventory(player, SetupStartInventory(player));
		}

		public static void UpdateBiomes(Player player)
		{
			player.UpdateBiomesHooks.Call();
		}

		public static bool CustomBiomesMatch(Player player, Player other)
		{
			return player.CustomBiomesMatchHooks.CallUntilFalse(other);
		}

		public static void CopyCustomBiomesTo(Player player, Player other)
		{
			player.CopyCustomBiomesToHooks.Call(other);
		}

		public static void SendCustomBiomes(Player player, BinaryWriter writer)
		{
			ushort count = 0;
			byte[] data;
			using (MemoryStream stream = new MemoryStream())
			{
				using (BinaryWriter customWriter = new BinaryWriter(stream))
				{
					foreach (ModPlayer modPlayer in player.modPlayers)
					{
						if (SendCustomBiomes(modPlayer, customWriter))
						{
							count++;
						}
					}
					customWriter.Flush();
					data = stream.ToArray();
				}
			}
			writer.Write(count);
			writer.Write(data);
		}

		private static bool SendCustomBiomes(ModPlayer modPlayer, BinaryWriter writer)
		{
			byte[] data;
			using (MemoryStream stream = new MemoryStream())
			{
				using (BinaryWriter customWriter = new BinaryWriter(stream))
				{
					modPlayer.SendCustomBiomes(customWriter);
					customWriter.Flush();
					data = stream.ToArray();
				}
			}
			if (data.Length > 0)
			{
				writer.Write(modPlayer.mod.Name);
				writer.Write(modPlayer.Name);
				writer.Write((byte)data.Length);
				writer.Write(data);
				return true;
			}
			return false;
		}

		public static void ReceiveCustomBiomes(Player player, BinaryReader reader)
		{
			int count = reader.ReadUInt16();
			for (int k = 0; k < count; k++)
			{
				string modName = reader.ReadString();
				string name = reader.ReadString();
				byte[] data = reader.ReadBytes(reader.ReadByte());
				Mod mod = ModLoader.GetMod(modName);
				ModPlayer modPlayer = mod == null ? null : player.GetModPlayer(mod, name);
				if (modPlayer != null)
				{
					using (MemoryStream stream = new MemoryStream(data))
					{
						using (BinaryReader customReader = new BinaryReader(stream))
						{
							try
							{
								modPlayer.ReceiveCustomBiomes(customReader);
							}
							catch
							{
							}
						}
					}
				}
			}
		}

		public static void UpdateBiomeVisuals(Player player)
		{
			player.UpdateBiomeVisualsHooks.Call();
		}

		public static void clientClone(Player player, Player clientClone)
		{
			foreach (ModPlayer modPlayer in player.modPlayers)
			{
				modPlayer.clientClone(clientClone.GetModPlayer(modPlayer.mod, modPlayer.Name));
			}
		}

		public static void SyncPlayer(Player player, int toWho, int fromWho, bool newPlayer)
		{
			foreach (ModPlayer modPlayer in player.modPlayers)
			{
				modPlayer.SyncPlayer(toWho, fromWho, newPlayer);
			}
		}

		public static void SendClientChanges(Player player, Player clientPlayer)
		{
			foreach (ModPlayer modPlayer in player.modPlayers)
			{
				modPlayer.SendClientChanges(clientPlayer.GetModPlayer(modPlayer.mod, modPlayer.Name));
			}
		}

		public static Texture2D GetMapBackgroundImage(Player player)
		{
			return player.GetMapBackgroundImageHooks.CallUntilNonNull();
		}

		public static void UpdateBadLifeRegen(Player player)
		{
			player.UpdateBadLifeRegenHooks.Call();
		}

		public static void UpdateLifeRegen(Player player)
		{
			player.UpdateLifeRegenHooks.Call();
		}

		public static void NaturalLifeRegen(Player player, ref float regen)
		{
			player.NaturalLifeRegenHooks.Call(ref regen);
		}

		public static void PreUpdate(Player player)
		{
			player.PreUpdateHooks.Call();
		}

		public static void SetControls(Player player)
		{
			player.SetControlsHooks.Call();
		}

		public static void PreUpdateBuffs(Player player)
		{
			player.PreUpdateBuffsHooks.Call();
		}

		public static void PostUpdateBuffs(Player player)
		{
			player.PostUpdateBuffsHooks.Call();
		}

		public static void UpdateEquips(Player player, ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
		{
			player.UpdateEquipsHooks.Call(ref wallSpeedBuff, ref tileSpeedBuff, ref tileRangeBuff);
		}

		public static void UpdateVanityAccessories(Player player)
		{
			player.UpdateVanityAccessoriesHooks.Call();
		}

		public static void PostUpdateEquips(Player player)
		{
			player.PostUpdateEquipsHooks.Call();
		}

		public static void PostUpdateMiscEffects(Player player)
		{
			player.PostUpdateMiscEffectsHooks.Call();
		}

		public static void PostUpdateRunSpeeds(Player player)
		{
			player.PostUpdateRunSpeedsHooks.Call();
		}

		public static void PostUpdate(Player player)
		{
			player.PostUpdateHooks.Call();
		}

		public static void FrameEffects(Player player)
		{
			player.FrameEffectsHooks.Call();
		}

		public static bool PreHurt(Player player, bool pvp, bool quiet, ref int damage, ref int hitDirection,
			ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			return player.PreHurtHooks.CallConditionalWithDefaultTrue(pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource);
		}

		public static void Hurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			player.HurtHooks.Call(pvp, quiet, damage, hitDirection, crit);
		}

		public static void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			player.PostHurtHooks.Call(pvp, quiet, damage, hitDirection, crit);
		}

		public static bool PreKill(Player player, double damage, int hitDirection, bool pvp, ref bool playSound,
			ref bool genGore, ref PlayerDeathReason damageSource)
		{
			return player.PreKillHooks.CallConditionalWithDefaultTrue(damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);
		}

		public static void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			player.KillHooks.Call(damage, hitDirection, pvp, damageSource);
		}

		public static bool PreItemCheck(Player player)
		{
			return player.PreItemCheckHooks.CallUntilFalse();
		}

		public static void PostItemCheck(Player player)
		{
			player.PostItemCheckHooks.Call();
		}

		public static void GetWeaponDamage(Player player, Item item, ref int damage)
		{
			player.GetWeaponDamageHooks.Call(item, ref damage);
		}

		public static void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			player.ProcessTriggersHooks.Call(triggersSet);
		}

		public static void GetWeaponKnockback(Player player, Item item, ref float knockback)
		{
			player.GetWeaponKnockbackHooks.Call(item, ref knockback);
		}

		public static bool ConsumeAmmo(Player player, Item weapon, Item ammo)
		{
			return player.ConsumeAmmoHooks.CallUntilFalse(weapon, ammo);
		}

		public static bool Shoot(Player player, Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			return player.ShootHooks.CallUntilFalse(item, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
		}

		public static void MeleeEffects(Player player, Item item, Rectangle hitbox)
		{
			player.MeleeEffectsHooks.Call(item, hitbox);
		}

		public static void OnHitAnything(Player player, float x, float y, Entity victim)
		{
			player.OnHitAnythingHooks.Call(x, y, victim);
		}

		public static bool? CanHitNPC(Player player, Item item, NPC target)
		{
			return player.CanHitNPCHooks.CallUntilFalse(item, target);
		}

		public static void ModifyHitNPC(Player player, Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			player.ModifyHitNPCHooks.Call(item, target, ref damage, ref knockback, ref crit);
		}

		public static void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			player.OnHitNPCHooks.Call(item, target, damage, knockback, crit);
		}

		public static bool? CanHitNPCWithProj(Projectile proj, NPC target)
		{
			if (proj.npcProj || proj.trap)
				return null;

			Player player = Main.player[proj.owner];
			return player.CanHitNPCWithProjHooks.CallUntilFalse(proj, target);
		}

		public static void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (proj.npcProj || proj.trap)
				return;

			Player player = Main.player[proj.owner];
			player.ModifyHitNPCWithProjHooks.Call(proj, target, ref damage, ref knockback, ref crit, ref hitDirection);
		}

		public static void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			if (proj.npcProj || proj.trap)
				return;

			Player player = Main.player[proj.owner];
			player.OnHitNPCWithProjHooks.Call(proj, target, damage, knockback, crit);
		}

		public static bool CanHitPvp(Player player, Item item, Player target)
		{
			return player.CanHitPvpHooks.CallUntilFalse(item, target);
		}

		public static void ModifyHitPvp(Player player, Item item, Player target, ref int damage, ref bool crit)
		{
			player.ModifyHitPvpHooks.Call(item, target, ref damage, ref crit);
		}

		public static void OnHitPvp(Player player, Item item, Player target, int damage, bool crit)
		{
			player.OnHitPvpHooks.Call(item, target, damage, crit);
		}

		public static bool CanHitPvpWithProj(Projectile proj, Player target)
		{
			Player player = Main.player[proj.owner];
			return player.CanHitPvpWithProjHooks.CallUntilFalse(proj, target);
		}

		public static void ModifyHitPvpWithProj(Projectile proj, Player target, ref int damage, ref bool crit)
		{
			Player player = Main.player[proj.owner];
			player.ModifyHitPvpWithProjHooks.Call(proj, target, ref damage, ref crit);
		}

		public static void OnHitPvpWithProj(Projectile proj, Player target, int damage, bool crit)
		{
			Player player = Main.player[proj.owner];
			player.OnHitPvpWithProjHooks.Call(proj, target, damage, crit);
		}

		public static bool CanBeHitByNPC(Player player, NPC npc, ref int cooldownSlot)
		{
			return player.CanBeHitByNPCHooks.CallUntilFalse(npc, ref cooldownSlot);
		}

		public static void ModifyHitByNPC(Player player, NPC npc, ref int damage, ref bool crit)
		{
			player.ModifyHitByNPCHooks.Call(npc, ref damage, ref crit);
		}

		public static void OnHitByNPC(Player player, NPC npc, int damage, bool crit)
		{
			player.OnHitByNPCHooks.Call(npc, damage, crit);
		}

		public static bool CanBeHitByProjectile(Player player, Projectile proj)
		{
			return player.CanBeHitByProjectileHooks.CallUntilFalse(proj);
		}

		public static void ModifyHitByProjectile(Player player, Projectile proj, ref int damage, ref bool crit)
		{
			player.ModifyHitByProjectileHooks.Call(proj, ref damage, ref crit);
		}

		public static void OnHitByProjectile(Player player, Projectile proj, int damage, bool crit)
		{
			player.OnHitByProjectileHooks.Call(proj, damage, crit);
		}

		public static void CatchFish(Player player, Item fishingRod, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk)
		{
			int i = 0;
			while (i < 58)
			{
				if (player.inventory[i].stack > 0 && player.inventory[i].bait > 0)
				{
					break;
				}
				i++;
			}
			player.CatchFishHooks.Call(fishingRod, player.inventory[i], power, liquidType, poolSize, worldLayer, questFish, ref caughtType, ref junk);
		}

		public static void GetFishingLevel(Player player, Item fishingRod, Item bait, ref int fishingLevel)
		{
			player.GetFishingLevelHooks.Call(fishingRod, bait, ref fishingLevel);
		}

		public static void AnglerQuestReward(Player player, float rareMultiplier, List<Item> rewardItems)
		{
			player.AnglerQuestRewardHooks.Call(rareMultiplier, rewardItems);
		}

		public static void GetDyeTraderReward(Player player, List<int> rewardPool)
		{
			player.GetDyeTraderRewardHooks.Call(rewardPool);
		}

		public static void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
			drawInfo.drawPlayer.DrawEffectsHooks.Call(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
		}

		public static void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
		{
			drawInfo.drawPlayer.ModifyDrawInfoHooks.Call(ref drawInfo);
		}

		public static List<PlayerLayer> GetDrawLayers(Player drawPlayer)
		{
			List<PlayerLayer> layers = new List<PlayerLayer>();
			layers.Add(PlayerLayer.HairBack);
			layers.Add(PlayerLayer.MountBack);
			layers.Add(PlayerLayer.MiscEffectsBack);
			layers.Add(PlayerLayer.BackAcc);
			layers.Add(PlayerLayer.Wings);
			layers.Add(PlayerLayer.BalloonAcc);
			layers.Add(PlayerLayer.Skin);
			if (drawPlayer.wearsRobe)
			{
				layers.Add(PlayerLayer.ShoeAcc);
				layers.Add(PlayerLayer.Legs);
			}
			else
			{
				layers.Add(PlayerLayer.Legs);
				layers.Add(PlayerLayer.ShoeAcc);
			}
			layers.Add(PlayerLayer.Body);
			layers.Add(PlayerLayer.HandOffAcc);
			layers.Add(PlayerLayer.WaistAcc);
			layers.Add(PlayerLayer.NeckAcc);
			layers.Add(PlayerLayer.Face);
			layers.Add(PlayerLayer.Hair);
			layers.Add(PlayerLayer.Head);
			layers.Add(PlayerLayer.FaceAcc);
			if (drawPlayer.mount.Cart)
			{
				layers.Add(PlayerLayer.ShieldAcc);
				layers.Add(PlayerLayer.MountFront);
			}
			else
			{
				layers.Add(PlayerLayer.MountFront);
				layers.Add(PlayerLayer.ShieldAcc);
			}
			layers.Add(PlayerLayer.SolarShield);
			layers.Add(PlayerLayer.HeldProjBack);
			layers.Add(PlayerLayer.HeldItem);
			layers.Add(PlayerLayer.Arms);
			layers.Add(PlayerLayer.HandOnAcc);
			layers.Add(PlayerLayer.HeldProjFront);
			layers.Add(PlayerLayer.FrontAcc);
			layers.Add(PlayerLayer.MiscEffectsFront);
			foreach (PlayerLayer layer in layers)
			{
				layer.visible = true;
			}
			drawPlayer.ModifyDrawLayersHooks.Call(layers);
			return layers;
		}

		public static List<PlayerHeadLayer> GetDrawHeadLayers(Player drawPlayer)
		{
			List<PlayerHeadLayer> layers = new List<PlayerHeadLayer>();
			layers.Add(PlayerHeadLayer.Head);
			layers.Add(PlayerHeadLayer.Hair);
			layers.Add(PlayerHeadLayer.AltHair);
			layers.Add(PlayerHeadLayer.Armor);
			layers.Add(PlayerHeadLayer.FaceAcc);
			foreach (PlayerHeadLayer layer in layers)
			{
				layer.visible = true;
			}
			drawPlayer.ModifyDrawHeadLayersHooks.Call(layers);
			return layers;
		}

		public static void ModifyScreenPosition(Player player)
		{
			player.ModifyScreenPositionHooks.Call();
		}

		public static void ModifyZoom(Player player, ref float zoom)
		{
			player.ModifyZoomHooks.Call(ref zoom);
		}

		public static void PlayerConnect(int playerIndex)
		{
			Player player = Main.player[playerIndex];
			player.PlayerConnectHooks.Call(player);
		}

		public static void PlayerDisconnect(int playerIndex)
		{
			Player player = Main.player[playerIndex];
			player.PlayerDisconnectHooks.Call(player);
		}

		// Do NOT hook into the Player.Hooks.OnEnterWorld event
		public static void OnEnterWorld(int playerIndex)
		{
			Player player = Main.player[playerIndex];
			player.OnEnterWorldHooks.Call(player);
		}
	}
}
