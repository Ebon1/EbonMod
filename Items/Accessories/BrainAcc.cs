using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
namespace EbonianMod.Items.Accessories
{
    public class BrainAcc : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = 4;
            Item.expert = true;
            Item.defense = 10;
        }
        public int timer = 0;
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
                timer++;
                if (timer >= 300)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        float angle = 2f * (float)Math.PI / 8f * k;
                        Vector2 pos = player.Center + 60 * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                        int npc = NPC.NewNPC(player.GetSource_Accessory(Item), (int)pos.X, (int)pos.Y, ModContent.NPCType<TinyBrain>());
                        Main.npc[npc].localAI[0] = k;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc);
                    }
                    timer = 0;
                }
            }
            else
            {
                timer = 0;
            }
            EbonianPlayer modPlayer = player.GetModPlayer<EbonianPlayer>();
            modPlayer.brainAcc = true;
        }
    }
}