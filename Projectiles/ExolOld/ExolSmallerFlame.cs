using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EbonianMod.Buffs;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace EbonianMod.Projectiles.ExolOld
{
    public class ExolSmallerFlame : ModProjectile
    {
        public int KillTimer = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flame");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 400;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = true;

        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * ((225 - Projectile.alpha) / 225f);
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }
            {
                Lighting.AddLight(Projectile.position, 0.25f, 0, 0.5f);
            }

        }
    }
    public class ExolFireRise : ModProjectile
    {
        public int KillTimer = 0;
        public override string Texture => "EbonianMod/Projectiles/ExolOld/ExolSmallerFlame";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spiteful Flame");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.aiStyle = 0;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = true;

            Projectile.timeLeft = 400;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * ((255 - Projectile.alpha) / 255f);
        }
        public override void AI()
        {
            if (++KillTimer >= 23)
            {
                Projectile.velocity = new Vector2(0, -15);
            }

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }
            {
                Lighting.AddLight(Projectile.position, 0.25f, 0, 0.5f);
            }
        }
    }
}
