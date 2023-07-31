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

namespace EbonianMod.Projectiles.Exol
{
    public class ESun : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/Sun";
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
                Projectile.ai[1] += 0.05f;


                for (float i = 0.25f; i < 1.75f; i += 0.25f)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        float angle = Helper.CircleDividedEqually(j, 10) + i;
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.UnitX.RotatedBy(angle) * (5 * i), ModContent.ProjectileType<EFire>(), Projectile.damage, Projectile.knockBack);
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
            float alpha = MathHelper.Lerp(1, 0, Projectile.ai[1]);
            Main.spriteBatch.Draw(a, Projectile.Center - Main.screenPosition, null, Color.White * alpha * (Projectile.scale * 2), Projectile.rotation, a.Size() / 2, Projectile.scale + Projectile.ai[1], SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return Projectile.scale >= 0.45f;
        }
        public override void AI()
        {
            if (Projectile.scale < 0.35f)
            {
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
                Projectile.velocity *= 0.99f;
                if (Projectile.velocity.Length() < 2)
                {
                    Death();
                }
            }
            else
                Projectile.velocity = Helper.FromAToB(Projectile.Center, Main.player[Projectile.owner].Center) * 20f;
            if (Projectile.ai[1] > 0)
            {
                Projectile.ai[1] += 0.05f;
                if (Projectile.ai[1] > 1)
                    Projectile.Kill();
                else
                    Projectile.timeLeft++;
            }
            Projectile.rotation += MathHelper.ToRadians(1);
            Projectile.scale = MathHelper.SmoothStep(Projectile.scale, 0.5f, 0.075f);
        }
    }
}
