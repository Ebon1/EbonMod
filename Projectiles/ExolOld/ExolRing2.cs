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
    public class ExolRing2 : ModProjectile
    {
        public int KillTimer = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infernal Gem");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.aiStyle = 0;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = true;

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Mod.Assets.Request<Texture2D>("Projectiles/ExolOld/ExolRing2").Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0, Projectile.gfxOffY);
                Main.EntitySpriteDraw(texture, drawPos, null, lightColor * 0.5f, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return true;
        }
        public float rotation = 0;
        public override Color? GetAlpha(Color lightColor)
        {
            if (KillTimer >= 30)
            {
                return Color.Red * ((105 - Projectile.alpha) / 105f);
            }
            return Color.White * ((225 - Projectile.alpha) / 225f);
        }
        public override void AI()
        {
            KillTimer++;
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6);
            }
            if (KillTimer >= 47)
            {
                Vector2 Sped = new Vector2(0, 0);
                Vector2 ProjPos = new Vector2(Projectile.position.X, Projectile.position.Y);
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), ProjPos, Sped, 695, 0, 0, 0, 0);

                for (int k = 0; k < 15; k++)
                {
                    float angle = 2f * (float)Math.PI / 15f * k;
                    Vector2 velocity = new Vector2(5f, 5f).RotatedBy(angle);
                    int damage = ((Main.expertMode) ? 12 : 20);
                    int projInt = Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, velocity, ModContent.ProjectileType<ExolRing3>(), damage, 0, 0, 1);
                    Main.projectile[projInt].tileCollide = false;
                }
                Projectile.Kill();
            }

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;
            }
            {
                Lighting.AddLight(Projectile.position, 0.25f, 0, 0.5f);
            }
        }
    }
}