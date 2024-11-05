using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;

namespace EbonianMod.Projectiles.Friendly.Generic
{
    public class PotatoP : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.WoodenArrowFriendly);
            Projectile.Size = new Vector2(14, 20);
            Projectile.extraUpdates = 1;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit1, Projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f));
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f));
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 400);
        }
        public override void AI()
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.Torch);
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f));
        }
        float animationOffset;
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            animationOffset -= 0.05f;
            if (animationOffset <= 0)
                animationOffset = 1;
            animationOffset = MathHelper.Clamp(animationOffset, float.Epsilon, 1 - float.Epsilon);
            List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>(ProjectileID.Sets.TrailCacheLength[Type]);
            for (int i = 0; i < Projectile.oldPos.Length - 1; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;
                float mult = 1f - (1f / Projectile.oldPos.Length) * i;
                mult *= mult;

                float __off = animationOffset;
                if (__off > 1) __off = -__off + 1;
                float _off = __off + (float)i / Projectile.oldPos.Length;

                if (mult > 0)
                {
                    vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * 6 - Main.screenPosition + Projectile.Size / 2 + new Vector2(10 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i], Projectile.oldPos[i + 1]).ToRotation() + MathHelper.PiOver2), Color.OrangeRed * mult, new Vector2(_off, 0)));
                    vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * 6 - Main.screenPosition + Projectile.Size / 2 + new Vector2(10 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i], Projectile.oldPos[i + 1]).ToRotation() - MathHelper.PiOver2), Color.OrangeRed * mult, new Vector2(_off, 1)));
                }
            }
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            if (vertices.Count > 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, Helper.GetExtraTexture("LintyTrail"), false);
                    Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, Helper.GetExtraTexture("FlamesSeamless"), false);
                }
            }
            Main.spriteBatch.ApplySaved();
            return true;
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = Helper.GetExtraTexture("fireball");
            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Draw(tex, Projectile.Center - Projectile.velocity.ToRotation().ToRotationVector2() * 14 - Main.screenPosition, null, Color.OrangeRed, Projectile.rotation, tex.Size() / 2, 0.55f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}
