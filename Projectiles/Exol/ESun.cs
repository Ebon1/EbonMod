using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using EbonianMod.Dusts;
using Terraria.ID;
using EbonianMod.Effects.Prims;

namespace EbonianMod.Projectiles.Exol
{
    public class ESun : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/Sun";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 512 / 2;
            Projectile.height = 512 / 2;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.scale = 0f;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.hostile = true;
            Projectile.timeLeft = 650;
        }
        void Death()
        {
            if (Projectile.ai[1] <= 0)
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.ai[1] += 0.05f;


                for (float i = 0.25f; i < 1.75f; i += 0.25f)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        float angle = Helper.CircleDividedEqually(j, 8) + i;
                        Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.UnitX.RotatedBy(angle) * (i), ModContent.ProjectileType<EFire3>(), Projectile.damage, Projectile.knockBack);
                    }
                }
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.scale > 0.45f)
            {
                Projectile.velocity = Vector2.Zero;
                Death();
            }
            return false;
        }
        public override bool? CanDamage() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D a = TextureAssets.Projectile[Type].Value;
            Main.spriteBatch.Reload(BlendState.Additive);
            if (ShouldUpdatePosition())
                (default(MassiveFireTrail)).Draw(Projectile);
            float alpha = MathHelper.Lerp(1, 0, Projectile.ai[1]);
            for (int i = 0; i < 2; i++)
                Main.spriteBatch.Draw(a, Projectile.Center - Main.screenPosition, null, Color.White * alpha * (Projectile.scale * 2), Main.GameUpdateCount * 0.03f, a.Size() / 2, Projectile.scale + Projectile.ai[1], SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return Projectile.scale >= 0.45f;
        }
        Vector2 lastPos;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.scale < 0.35f)
            {
                lastPos = Main.player[Projectile.owner].Center;
                Vector2 pos = Projectile.Center + 300 * Main.rand.NextVector2Unit();
                Dust a = Dust.NewDustPerfect(pos, ModContent.DustType<GenericAdditiveDust>(), Helper.FromAToB(pos, Projectile.Center) * Main.rand.NextFloat(10, 20), newColor: Color.OrangeRed, Scale: Main.rand.NextFloat(0.05f, 0.15f));
                a.noGravity = false;
                a.customData = 1;

            }
            else
            {
                Projectile.timeLeft++;
            }
            if (ShouldUpdatePosition())
            {
                if (++Projectile.ai[2] > 60)
                    Projectile.velocity *= 0.95f;
                if (Projectile.velocity.Length() < 2)
                {
                    Death();
                }
            }
            else
                Projectile.velocity = Helper.FromAToB(Projectile.Center, lastPos) * 30f;
            if (Projectile.ai[1] > 0)
            {
                Projectile.ai[1] += 0.05f;
                if (Projectile.ai[1] > 1)
                    Projectile.Kill();
                else
                    Projectile.timeLeft++;
            }
            Projectile.scale = MathHelper.SmoothStep(Projectile.scale, 0.5f, 0.075f);
        }
    }
}
