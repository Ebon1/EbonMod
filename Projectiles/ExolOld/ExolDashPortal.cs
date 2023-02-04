using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EbonianMod.NPCs.Exol;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace EbonianMod.Projectiles.ExolOld
{
    public class ExolDashPortal : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exol Portal effect");
            Main.projFrames[Projectile.type] = 8;
        }
        public int KillTimer = 0;
        public static bool Minus = false;

        public override void SetDefaults()
        {
            Projectile.width = 136;
            Projectile.height = 120;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            KillTimer++;
            Player player = Main.player[Projectile.owner];
            if (KillTimer == 1)
            {
                if (Main.rand.Next(2) == 0)
                {
                    Minus = false;
                }
                else
                {
                    Minus = true;
                }
            }
            if (KillTimer <= 40)
            {
                if (Minus)
                {
                    Projectile.Center = new Vector2(player.position.X - 350, player.position.Y);
                }
                else
                {
                    Projectile.Center = new Vector2(player.position.X + 350, player.position.Y);
                }
            }
            else if (KillTimer >= 40)
            {
                Projectile.Kill();
            }


            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 8)
                {
                    Projectile.frame = 0;
                }
            }
            {
                Lighting.AddLight(Projectile.position, 0.25f, 0, 0.5f);
            }

        }
        public override Color? GetAlpha(Color lightColor)
        {
            if (KillTimer >= 15)
            {
                return new Color(0, 0, 100) * ((105 - Projectile.alpha) / 105f);
            }
            return Color.White * ((225 - Projectile.alpha) / 225f);
        }
    }
}