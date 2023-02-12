using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
namespace EbonianMod.Items.Accessories
{
    internal class BrainAcc : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Judgemental Brain");
            Tooltip.SetDefault("While the brains are active, they slowly heal you.\nEach individual brain can be destroyed by enemies but if the player gets hit, all the brains die.\n\"For the first time ever! Experience being Flesh Prison from the hit game ultra murder!\"");
        }
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = 4;
        }
        public int timering = 0;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            List<int> brains = new List<int>();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var npcc = Main.npc[i];
                if (npcc.type == ModContent.NPCType<TinyBrain>() && npcc.active)
                {
                    brains.Add(i);
                }
            }
            if (brains.Count <= 0)
            {
                timering++;
                if (timering >= 500)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        float angle = 2f * (float)Math.PI / 8f * k;
                        Vector2 pos = player.Center + 60 * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                        int npc = NPC.NewNPC(player.GetSource_Accessory(Item), (int)pos.X, (int)pos.Y, ModContent.NPCType<TinyBrain>());
                        Main.npc[npc].localAI[0] = k;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc);
                    }
                    timering = 0;
                }
            }
            else
            {
                timering = 0;
            }
            EbonianPlayer modPlayer = player.GetModPlayer<EbonianPlayer>();
            modPlayer.brainAcc = true;
        }
    }
}