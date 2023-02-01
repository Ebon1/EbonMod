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
    public class ExolSmallFlame : ModProjectile
    {
        public int KillTimer = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spiteful Flames");
            Main.projFrames[Projectile.type] = 4;
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
            Player player = Main.player[Projectile.owner];
            if (!player.active)
            {
                Projectile.Kill();
            }
            Projectile.timeLeft = 2;

            if (KillTimer <= 82)
            {
                Projectile.ai[1] += 2f * (float)Math.PI / 600f * Projectile.localAI[1];
            }
            else
            {
                Projectile.ai[1] += 2f * (float)Math.PI / 600f * 0;
            }

            Projectile.ai[1] %= 2f * (float)Math.PI;
            Projectile.Center = player.Center + Projectile.localAI[0] * new Vector2((float)Math.Cos(Projectile.ai[1]), (float)Math.Sin(Projectile.ai[1]));
            if (++KillTimer >= 92)
            {
                for (int i = 0; i < 200; i++)
                {
                    Player target = Main.player[i];
                    float shootToX = target.position.X + (float)target.width * 0.5f - Projectile.Center.X;
                    float shootToY = target.position.Y - Projectile.Center.Y;
                    float distance = (float)System.Math.Sqrt((double)(shootToX * shootToX + shootToY * shootToY));

                    if (target.active)
                    {
                        int damage = 20;
                        distance = 3f / distance;
                        shootToX *= distance * 4;
                        shootToY *= distance * 4;
                        int proj = Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center.X, Projectile.Center.Y, shootToX, shootToY, ModContent.ProjectileType<ExolSmallerFlame>(), damage, Projectile.knockBack, Main.myPlayer, 0, 0);
                        Main.projectile[proj].timeLeft = 300;
                        Main.projectile[proj].netUpdate = true;
                        Projectile.netUpdate = true;

                        Projectile.Kill();

                    }
                }
            }
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * ((255 - Projectile.alpha) / 255f);
        }



    }
}