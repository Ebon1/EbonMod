using EbonianMod.Common.Systems;
using EbonianMod.Dusts;
using EbonianMod.Projectiles.ArchmageX;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using EbonianMod.Projectiles.VFXProjectiles;

namespace EbonianMod.Projectiles.Conglomerate
{
    public class CBeamInstant : ModProjectile
    {
        public override string Texture => Helper.Empty;
        int MaxTime = 60;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 100;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = MaxTime;
            Projectile.Size = new(32, 32);
        }
        float alpha;
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => Projectile.ai[1] > 0.1f;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.ToRotation().ToRotationVector2() * 2000, 30, ref a);
        }
        public override void AI()
        {
            Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], 0, 0.1f);
            if (Projectile.timeLeft < 31 && Projectile.timeLeft > 10)
                Projectile.ai[1] = MathHelper.Lerp(Projectile.ai[1], 1, 0.3f);
            if (Projectile.timeLeft > 30)
                Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], 1f, 0.1f);
            else
            {
                if (Projectile.timeLeft > 10)
                {
                    Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], 0, 0.2f);
                }
                else
                {
                    Projectile.ai[1] = MathHelper.Lerp(Projectile.ai[1], 0, 0.05f);
                }
            }
            if (Projectile.timeLeft == 30)
            {
                EbonianSystem.conglomerateSkyFlash = 3;
                EbonianSystem.ScreenShakeAmount = 10f;
                Projectile.NewProjectile(null, Projectile.Center + Projectile.velocity * 10, Vector2.Zero, ModContent.ProjectileType<ConglomerateScream>(), 0, 0);
                SoundEngine.PlaySound(EbonianSounds.exolDash, Projectile.Center);

                Projectile.ai[2] = 0.5f;
            }
            if (Projectile.timeLeft == 29)
                Projectile.ai[2] = 1;

            Projectile.localAI[0] -= 0.07f;
            if (Projectile.localAI[0] <= 0)
                Projectile.localAI[0] = 1;
            Projectile.localAI[0] = MathHelper.Clamp(Projectile.localAI[0], float.Epsilon, 1 - float.Epsilon);
        }
        public override bool PreDraw(ref Color lightColor)
        {

            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            List<VertexPositionColorTexture> verticesMain1 = new List<VertexPositionColorTexture>();
            List<VertexPositionColorTexture> verticesMain2 = new List<VertexPositionColorTexture>();

            List<VertexPositionColorTexture> verticesFlash = new List<VertexPositionColorTexture>();

            List<VertexPositionColorTexture> verticesTelegraph1 = new List<VertexPositionColorTexture>();
            List<VertexPositionColorTexture> verticesTelegraph2 = new List<VertexPositionColorTexture>();
            Texture2D texture = Helper.GetExtraTexture("FlamesSeamless");
            Texture2D texture2 = Helper.GetExtraTexture("Ex1");
            Texture2D texture3 = Helper.GetExtraTexture("LintyTrail");
            Vector2 start = Projectile.Center - Main.screenPosition;
            Vector2 off = (Projectile.velocity.ToRotation().ToRotationVector2() * 1528);
            Vector2 end = start + off;
            float rot = Helper.FromAToB(start, end).ToRotation();

            float s = 0f;
            float sLin = 0f;
            for (float i = 0; i < 1; i += 0.001f)
            {
                if (i < 0.5f)
                    s = MathHelper.Clamp(i * 3.5f, 0, 0.5f);
                else
                    s = MathHelper.Clamp((-i + 1) * 2, 0, 0.5f);

                if (i < 0.5f)
                    sLin = MathHelper.Clamp(i, 0, 0.5f);
                else
                    sLin = MathHelper.Clamp((-i + 1), 0, 0.5f);

                float __off = Projectile.localAI[0];
                if (__off > 1) __off = -__off + 1;
                float _off = __off + i;

                Color col = Color.Red * 2 * s;
                Color col2 = Color.Green * 2 * s;
                if (Projectile.ai[0] > 0)
                {
                    verticesTelegraph1.Add(Helper.AsVertex(start + off * i + new Vector2(MathHelper.SmoothStep(10, 450, i), 0).RotatedBy(rot + MathHelper.PiOver2) * Projectile.ai[0], new Vector2(_off, 1), col * Projectile.ai[0]));
                    verticesTelegraph1.Add(Helper.AsVertex(start + off * i + new Vector2(MathHelper.SmoothStep(10, 450, i), 0).RotatedBy(rot - MathHelper.PiOver2) * Projectile.ai[0], new Vector2(_off, 0), col2 * Projectile.ai[0]));

                    col = Color.Maroon * 0.25f * s;
                    col2 = Color.LawnGreen * 0.25f * s;
                    verticesTelegraph2.Add(Helper.AsVertex(start + off * i + new Vector2(420, 0).RotatedBy(rot + MathHelper.PiOver2) * Projectile.ai[0], new Vector2(_off, 1), col * Projectile.ai[0]));
                    verticesTelegraph2.Add(Helper.AsVertex(start + off * i + new Vector2(420, 0).RotatedBy(rot - MathHelper.PiOver2) * Projectile.ai[0], new Vector2(_off, 0), col2 * Projectile.ai[0]));
                }
                if (Projectile.ai[1] > 0)
                {
                    float cA = MathHelper.Lerp(s, sLin, i);
                    col = Color.Maroon * (cA);
                    col2 = Color.LawnGreen * (cA);
                    verticesMain1.Add(Helper.AsVertex(start + off * i + new Vector2(MathHelper.SmoothStep(40, 100, i * 2), 0).RotatedBy(rot + MathHelper.PiOver2) * Projectile.ai[1], new Vector2(_off, 1), col * Projectile.ai[1] * 2));
                    verticesMain1.Add(Helper.AsVertex(start + off * i + new Vector2(MathHelper.SmoothStep(40, 100, i * 2), 0).RotatedBy(rot - MathHelper.PiOver2) * Projectile.ai[1], new Vector2(_off, 0), col2 * Projectile.ai[1] * 2));

                    col = Color.Lerp(Color.White, Color.Maroon, Projectile.ai[2]) * (cA * 0.5f);
                    col2 = Color.Lerp(Color.White, Color.LawnGreen, Projectile.ai[2]) * (cA * 0.5f);
                    verticesMain2.Add(Helper.AsVertex(start + off * i + new Vector2(MathHelper.SmoothStep(10, 20, i * 2), 0).RotatedBy(rot + MathHelper.PiOver2) * Projectile.ai[1], new Vector2(_off, 1), col * Projectile.ai[1]));
                    verticesMain2.Add(Helper.AsVertex(start + off * i + new Vector2(MathHelper.SmoothStep(10, 20, i * 2), 0).RotatedBy(rot - MathHelper.PiOver2) * Projectile.ai[1], new Vector2(_off, 0), col2 * Projectile.ai[1]));
                }
                if (Projectile.ai[2] > 0)
                {
                    col = Color.Lerp(Color.Maroon, Color.Transparent, i) * (s);
                    col2 = Color.Lerp(Color.LawnGreen, Color.Transparent, i) * (s);
                    verticesFlash.Add(Helper.AsVertex(start + off * 1.2f * i + new Vector2(MathHelper.SmoothStep(60, 105, i * 3), 0).RotatedBy(rot + MathHelper.PiOver2) * Projectile.ai[2], new Vector2(_off, 1), col * Projectile.ai[2] * 4));
                    verticesFlash.Add(Helper.AsVertex(start + off * 1.2f * i + new Vector2(MathHelper.SmoothStep(60, 105, i * 3), 0).RotatedBy(rot - MathHelper.PiOver2) * Projectile.ai[2], new Vector2(_off, 0), col2 * Projectile.ai[2] * 4));
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (verticesMain1.Count >= 3 && verticesMain2.Count >= 3)
            {
                for (int i = 0; i < 6; i++)
                {
                    Helper.DrawTexturedPrimitives(verticesMain1.ToArray(), PrimitiveType.TriangleStrip, texture, false);
                    Helper.DrawTexturedPrimitives(verticesMain2.ToArray(), PrimitiveType.TriangleStrip, texture3, false);
                }
            }
            if (verticesTelegraph1.Count >= 3 && verticesTelegraph2.Count >= 3)
            {
                Helper.DrawTexturedPrimitives(verticesTelegraph1.ToArray(), PrimitiveType.TriangleStrip, texture2, false);
                Helper.DrawTexturedPrimitives(verticesTelegraph2.ToArray(), PrimitiveType.TriangleStrip, texture2, false);
            }
            if (verticesFlash.Count >= 3)
            {
                for (int i = 0; i < 2; i++)
                {
                    Helper.DrawTexturedPrimitives(verticesFlash.ToArray(), PrimitiveType.TriangleStrip, texture3, false);
                }
            }
            Main.spriteBatch.ApplySaved();
            return false;
        }
    }
}
