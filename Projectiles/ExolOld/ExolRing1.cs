using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ExolRebirth.Buffs;
using ExolRebirth.NPCs.Exol;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace ExolRebirth.Projectiles.ExolOld
{
    public class ExolRing1 : ModProjectile
    {
        public int KillTimer = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infernal Gem");
        }



        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.netImportant = true;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }


        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }
        public override void AI()
        {
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6);
            }
            NPC center = Main.npc[(int)Projectile.ai[0]];
            if (!center.active || center.type != ModContent.NPCType<NPCs.Exol.Exol>())
            {
                Projectile.Kill();
            }
            Projectile.timeLeft = 2;
            Projectile.ai[1] += 2f * (float)Math.PI / 600f * 10;
            Projectile.ai[1] %= 2f * (float)Math.PI;
            Projectile.Center = center.Center + Projectile.localAI[0] * new Vector2((float)Math.Cos(Projectile.ai[1]), (float)Math.Sin(Projectile.ai[1]));
            if (++KillTimer == (int)Projectile.localAI[1])
            {
                for (int i = 0; i < 200; i++)
                {
                    Player target = Main.player[i];
                    float shootToX = target.position.X + (float)target.width * 0.5f - Projectile.Center.X;
                    float shootToY = target.position.Y - Projectile.Center.Y;
                    float distance = (float)System.Math.Sqrt((double)(shootToX * shootToX + shootToY * shootToY));

                    if (target.active)
                    {
                        int damage = ((Main.expertMode) ? 12 : 20);
                        distance = 3f / distance;
                        shootToX *= distance * 6.5f;
                        shootToY *= distance * 6.5f;
                        int proj = Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center.X, Projectile.Center.Y, shootToX, shootToY, ModContent.ProjectileType<ExolRing2>(), damage, Projectile.knockBack, Main.myPlayer, 0, 0);
                        Main.projectile[proj].timeLeft = 300;
                        Main.projectile[proj].netUpdate = true;
                        Projectile.netUpdate = true;

                        Projectile.Kill();

                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * ((255 - Projectile.alpha) / 255f);
        }



    }
}