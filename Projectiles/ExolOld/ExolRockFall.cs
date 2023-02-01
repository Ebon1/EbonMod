using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics;

namespace ExolRebirth.Projectiles.ExolOld
{
    public class ExolRockFall : ModProjectile
    {
        public override string Texture => "ExolRebirth/Projectiles/ExolOld/ExolRing1";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("ash");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = 1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 260;

        }


        public override bool PreDraw(ref Color lightColor)
        {
            default(FlameLashDrawer).Draw(Main.projectile[Projectile.whoAmI]);
            Texture2D texture = Mod.Assets.Request<Texture2D>("Projectiles/ExolOld/ExolRing1").Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0, Projectile.gfxOffY);
                Color color = Color.White;
                Main.EntitySpriteDraw(texture, drawPos, null, color * 0.5f, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * ((255 - Projectile.alpha) / 255f);
        }

        int timer = 0;
        public override void AI()
        {
            if (++timer >= 13)
            {
                Projectile.velocity = new Vector2(0, 20);
            }
            if (Projectile.owner == Main.myPlayer && Main.rand.Next(2) == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 4);
            }
        }
    }
}