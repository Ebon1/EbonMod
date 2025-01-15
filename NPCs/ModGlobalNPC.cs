
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;
using System;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using EbonianMod.NPCs.Garbage;
using EbonianMod.Items.Weapons.Magic;
using EbonianMod.NPCs.Cecitior;
using EbonianMod.Items.Armor.Vanity;

namespace EbonianMod.NPCs
{
    public class ModGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool stunned;
        public override void ResetEffects(NPC npc)
        {
            stunned = false;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (stunned)
            {
                npc.velocity = Vector2.Zero;
            }
        }
        /*public override bool PreAI(NPC npc) {
            if (stunned) {
            return false;
            }
            return base.PreAI(npc);
        }*/
    }
    public class NonInstancedGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.Zombie)
                npcLoot.Add(ItemDropRule.Common(ItemType<ClementinesCap>(), 300));
        }
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (NPC.AnyNPCs(NPCType<Terrortoma.Terrortoma>()) || NPC.AnyNPCs(NPCType<Cecitior.Cecitior>()) || NPC.AnyNPCs(NPCType<ArchmageX.ArchmageX>()) || NPC.AnyNPCs(NPCType<HotGarbage>()))
            {
                maxSpawns = 0;
                spawnRate = 0;

            }
        }
    }
}