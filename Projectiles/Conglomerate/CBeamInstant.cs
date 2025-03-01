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
        int MaxTime = 100;
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
        public override bool? CanDamage() => Projectile.timeLeft < 30;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.ToRotation().ToRotationVector2() * 2000, 30, ref a);
        }
        public override void AI()
        {
            if (Projectile.localAI[1] == 0)
                Projectile.localAI[1] = Main.rand.Next(1, 3);
            if (Projectile.timeLeft == MaxTime - 10)
            {
                Projectile.NewProjectile(null, Projectile.Center + Projectile.velocity * 50, Projectile.velocity * 20, ProjectileType<CGhostCeci>(), 30, 0, -1, Projectile.localAI[1]);
            }
            Projectile.ai[2] = Lerp(Projectile.ai[2], 0, 0.1f);
            if (Projectile.timeLeft < 31 && Projectile.timeLeft > 10)
                Projectile.ai[1] = Lerp(Projectile.ai[1], 1, 0.3f);
            if (Projectile.timeLeft > 30)
                Projectile.ai[0] = Lerp(Projectile.ai[0], 1f, 0.1f);
            else
            {
                if (Projectile.timeLeft > 10)
                {
                    Projectile.ai[0] = Lerp(Projectile.ai[0], 0, 0.2f);
                }
                else
                {
                    Projectile.ai[1] = Lerp(Projectile.ai[1], 0, 0.2f);
                }
            }
            if (Projectile.timeLeft == 30)
            {
                if (EbonianSystem.conglomerateSkyFlash < 20)
                    EbonianSystem.conglomerateSkyFlash = 20f;
                EbonianSystem.conglomerateSkyFlash = 3;
                CameraSystem.ScreenShakeAmount = 10f;
                Projectile.NewProjectile(null, Projectile.Center + Projectile.velocity * 10, Vector2.Zero, ModContent.ProjectileType<BlurScream>(), 0, 0);
                SoundEngine.PlaySound(EbonianSounds.exolDash, Projectile.Center);

                Projectile.ai[2] = 0.5f;
            }
            if (Projectile.timeLeft == 29)
                Projectile.ai[2] = 1;

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.localAI[0] -= 0.04f;
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            List<VertexPositionColorTexture> verticesMain1 = new List<VertexPositionColorTexture>();
            List<VertexPositionColorTexture> verticesMain2 = new List<VertexPositionColorTexture>();

            List<VertexPositionColorTexture> verticesFlash = new List<VertexPositionColorTexture>();

            List<VertexPositionColorTexture> verticesTelegraph1 = new List<VertexPositionColorTexture>();
            List<VertexPositionColorTexture> verticesTelegraph2 = new List<VertexPositionColorTexture>();
            Texture2D texture = ExtraTextures.FlamesSeamless;
            Texture2D texture2 = ExtraTextures.Ex1;
            Texture2D texture3 = ExtraTextures.FlamesSeamless;
            Vector2 start = Projectile.Center - Main.screenPosition;
            Vector2 off = (Projectile.velocity.ToRotation().ToRotationVector2() * 1728);
            Vector2 end = start + off;
            float rot = Helper.FromAToB(start, end).ToRotation();

            float s = 0f;
            float sLin = 0f;
            for (float i = 0; i < 1; i += 0.02f)
            {
                if (i < 0.5f)
                    s = Clamp(i * 3.5f, 0, 0.5f);
                else
                    s = Clamp((-i + 1) * 2, 0, 0.5f);

                if (i < 0.5f)
                    sLin = Clamp(i, 0, 0.5f);
                else
                    sLin = Clamp((-i + 1), 0, 0.5f);

                float _off = Projectile.localAI[0] + i;

                Color col = Color.Red * 2 * s;
                Color col2 = Color.Green * 2 * s;
                if (Projectile.ai[0] > 0)
                {
                    verticesTelegraph1.Add(Helper.AsVertex(start + off * i + new Vector2(350, 0).RotatedBy(rot + PiOver2) * Projectile.ai[0], new Vector2(_off, 1), (Projectile.localAI[1] == 1 ? col : col2) * Projectile.ai[0] * 0.75f));
                    verticesTelegraph1.Add(Helper.AsVertex(start + off * i + new Vector2(350, 0).RotatedBy(rot - PiOver2) * Projectile.ai[0], new Vector2(_off, 0), (Projectile.localAI[1] == 1 ? col : col2) * Projectile.ai[0] * 0.75f));

                    col = Color.Maroon * 0.25f * s;
                    col2 = Color.LawnGreen * 0.5f * s;
                    verticesTelegraph2.Add(Helper.AsVertex(start + off * i + new Vector2(320, 0).RotatedBy(rot + PiOver2) * Projectile.ai[0], new Vector2(_off, 1), (Projectile.localAI[1] == 1 ? col : col2) * Projectile.ai[0]));
                    verticesTelegraph2.Add(Helper.AsVertex(start + off * i + new Vector2(320, 0).RotatedBy(rot - PiOver2) * Projectile.ai[0], new Vector2(_off, 0), (Projectile.localAI[1] == 1 ? col : col2) * Projectile.ai[0]));
                }
                if (Projectile.ai[1] > 0)
                {
                    float cA = Lerp(s, sLin, i);
                    col = Color.Maroon * (cA * 2);
                    col2 = Color.Green * (cA * 2);
                    verticesMain1.Add(Helper.AsVertex(start + off * i + new Vector2(100, 0).RotatedBy(rot + PiOver2) * Projectile.ai[1], new Vector2(_off, 1), (Projectile.localAI[1] == 1 ? col : col2) * Projectile.ai[1] * 2));
                    verticesMain1.Add(Helper.AsVertex(start + off * i + new Vector2(100, 0).RotatedBy(rot - PiOver2) * Projectile.ai[1], new Vector2(_off, 0), (Projectile.localAI[1] == 1 ? col : col2) * Projectile.ai[1] * 2));

                    col = Color.Lerp(Color.White * 0.5f, Color.Maroon * 3, Projectile.ai[1]) * (cA);
                    col2 = Color.Lerp(Color.White * 0.5f, Color.Green * 3, Projectile.ai[1]) * (cA);
                    verticesMain2.Add(Helper.AsVertex(start + off * i + new Vector2(20, 0).RotatedBy(rot + PiOver2) * Projectile.ai[1], new Vector2(_off, 1), (Projectile.localAI[1] == 1 ? col : col2) * Projectile.ai[1]));
                    verticesMain2.Add(Helper.AsVertex(start + off * i + new Vector2(20, 0).RotatedBy(rot - PiOver2) * Projectile.ai[1], new Vector2(_off, 0), (Projectile.localAI[1] == 1 ? col : col2) * Projectile.ai[1]));
                }
                if (Projectile.ai[2] > 0)
                {
                    col = Color.Lerp(Color.Maroon, Color.Transparent, i) * (s);
                    col2 = Color.Lerp(Color.LawnGreen, Color.Transparent, i) * (s);
                    verticesFlash.Add(Helper.AsVertex(start + off * 1.2f * i + new Vector2(105, 0).RotatedBy(rot + PiOver2) * Projectile.ai[2], new Vector2(_off, 1), (Projectile.localAI[1] == 1 ? col : col2) * Projectile.ai[2] * 4));
                    verticesFlash.Add(Helper.AsVertex(start + off * 1.2f * i + new Vector2(105, 0).RotatedBy(rot - PiOver2) * Projectile.ai[2], new Vector2(_off, 0), (Projectile.localAI[1] == 1 ? col : col2) * Projectile.ai[2] * 4));
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
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
