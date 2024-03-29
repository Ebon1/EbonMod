using EbonianMod.Common.Systems.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.Corruption.Rolypoly
{
    public class Rolypoly : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.Size = new Vector2(200, 200);
            NPC.lifeMax = 300;
            NPC.defense = 5;
            NPC.knockBackResist = 0.1f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = -1;
            NPC.damage = 30;
            NPC.behindTiles = true;
        }
        Verlet verlet;
        int texNum;
        float rot, rotFactor;
        int amount;
        float lerpFactor = 1f;
        public override void OnSpawn(IEntitySource source)
        {
            NPC.scale = Main.rand.Next(new float[] { 1f, 1f, 0.75f, 0.75f, 0.75f, 0.5f });
            switch (NPC.scale)
            {
                case 1:
                    amount = 16;
                    break;
                case 0.75f:
                    amount = 12;
                    break;
                case 0.5f:
                    amount = 8;
                    break;
            }
            texNum = Main.rand.Next(9999999);
            NPC.Size = new Vector2(200, 200) * NPC.scale;
            verlet = new Verlet(NPC.Center, 16, amount, 0, false, false, 4);
        }
        public override bool CheckDead()
        {
            if (verlet != null)
                for (int i = 0; i < verlet.points.Count; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), verlet.points[i].position, Main.rand.NextVector2Circular(4, 4), ModContent.Find<ModGore>("EbonianMod/Rolypoly" + Main.rand.Next(3)).Type);
                }
            return true;
        }
        public override void AI()
        {
            lerpFactor = MathHelper.Lerp(lerpFactor, 0.25f, 0.1f);
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            if (NPC.collideY)
            {
                if (NPC.ai[3]++ % 40 <= 20)
                {
                    NPC.FaceTarget();
                }

                Dust.NewDust(NPC.BottomLeft, NPC.width, 2, DustID.CorruptGibs, NPC.velocity.X, NPC.velocity.Y);
            }
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 17 * NPC.direction, 0.05f);
            if (NPC.collideX)
            {
                if (NPC.velocity.Y > -15)
                    NPC.velocity.Y -= 1f;
            }
            else
                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, 30, 0.2f);

            rot = MathHelper.Lerp(rot, MathHelper.ToRadians(rotFactor), 1f);

            rotFactor += NPC.velocity.X * 0.25f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (verlet != null)
            {
                verlet.Update(NPC.Center + new Vector2(0, 75 * NPC.scale), NPC.Center + new Vector2(0, 74 * NPC.scale), 1f);
                for (int i = 0; i < verlet.points.Count; i++)
                {
                    float angle = Helper.CircleDividedEqually(i, amount - 1);
                    //NPC.Center + new Vector2(0, 75 * NPC.scale).RotatedBy(angle + rot)
                    verlet.points[i].position = Vector2.Lerp(verlet.points[i].position, Helper.TRay.Cast(NPC.Center, (angle + rot).ToRotationVector2(), 128 * NPC.scale, false), lerpFactor);
                }
                verlet.Draw(spriteBatch, Texture + "_Tex", textureVariation: true, maxVariants: 3, variantSeed: texNum);
            }
            return false;
        }
    }
}
