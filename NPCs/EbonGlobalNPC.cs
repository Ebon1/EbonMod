
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
using EbonianMod.NPCs.ArchmageX;

namespace EbonianMod.NPCs
{
    public class EbonGlobalNPC : GlobalNPC
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
        public override void GetChat(NPC npc, ref string chat)
        {
            if (npc.type == NPCID.Wizard)
            {
                if (Main.rand.NextBool(10))
                {
                    WeightedRandom<string> _chat = new();
                    if (GetInstance<EbonianSystem>().xareusFuckingDies)
                        _chat.Add(Language.GetTextValue("Mods.EbonianMod.Dialogue.Wizard.WizardAboutX1"));
                    else if (GetInstance<EbonianSystem>().downedXareus)
                        _chat.Add(Language.GetTextValue("Mods.EbonianMod.Dialogue.Wizard.WizardAboutX2"));
                    else if (GetInstance<EbonianSystem>().timesDiedToXareus > 0)
                    {
                        _chat.Add(Language.GetTextValue("Mods.EbonianMod.Dialogue.Wizard.WizardAboutX3"));
                        _chat.Add(Language.GetTextValue("Mods.EbonianMod.Dialogue.Wizard.WizardAboutX4"));
                        _chat.Add(Language.GetTextValue("Mods.EbonianMod.Dialogue.Wizard.WizardAboutX5"));
                    }
                    else if (NPC.AnyNPCs(NPCType<ArchmageStaffNPC>()))
                    {
                        _chat.Add(Language.GetTextValue("Mods.EbonianMod.Dialogue.Wizard.WizardAboutX6"));
                        _chat.Add(Language.GetTextValue("Mods.EbonianMod.Dialogue.Wizard.WizardAboutX7"));
                    }
                    if (GetInstance<EbonianSystem>().timesDiedToXareus > 0)
                        _chat.Add(Language.GetTextValue("Mods.EbonianMod.Dialogue.Wizard.WizardAboutX8"));
                    chat = _chat;
                }
            }
        }
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