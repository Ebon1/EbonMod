
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
}