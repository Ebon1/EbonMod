using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria;
using EbonianMod.Dusts;
using Terraria.ID;
using EbonianMod.Common.Systems;
using Terraria.Audio;
using Terraria.DataStructures;

namespace EbonianMod.Projectiles.ArchmageX
{
    public class XAmethyst : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 250;
            Projectile.Size = new(18, 18);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = -oldVelocity;
            return false;
        }
        public override bool? CanDamage() => Projectile.velocity.Length() > 2.5f;
        public override bool PreKill(int timeLeft)
        {
            int i = 0;
            foreach (Vector2 pos in Projectile.oldPos)
            {
                var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
                float mult = (1f - fadeMult * i);
                Dust.NewDustPerfect(pos + Projectile.Size / 2, DustType<SparkleDust>(), Main.rand.NextVector2Circular(3, 3), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f) * mult);
                Dust.NewDustPerfect(pos + Projectile.Size / 2, DustType<SparkleDust>(), Vector2.Zero, 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.15f) * mult);
                i++;
            }
            SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);
            return true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                Dust.NewDustPerfect(Projectile.Center, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
            }
        }
        float vfxOffset;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Texture2D glow = Helper.GetTexture(Texture + "_Glow");
            Main.spriteBatch.Reload(BlendState.Additive);
            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            vfxOffset -= 0.015f;
            if (vfxOffset <= 0)
                vfxOffset = 1;
            vfxOffset = MathHelper.Clamp(vfxOffset, float.Epsilon, 1 - float.Epsilon);
            List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
            float s = 0;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float mult = (1f - fadeMult * i);

                if (mult < 0.5f)
                    s = MathHelper.Clamp(mult * 3.5f, 0, 0.5f) * 3;
                else
                    s = MathHelper.Clamp((-mult + 1) * 2, 0, 0.5f) * 3;

                if (i > 0 && Projectile.oldPos[i] != Vector2.Zero && Projectile.oldPos[i] != Projectile.position)
                {
                    Color col = Color.Lerp(Color.Indigo * 0.5f, Color.Gray, (float)(i / Projectile.oldPos.Length)) * 3;

                    float __off = vfxOffset;
                    if (__off > 1) __off = -__off + 1;
                    float _off = (__off + mult) % 1f;
                    vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(50 * mult, 0).RotatedBy(Projectile.velocity.ToRotation() - MathHelper.PiOver2), col, new Vector2(_off, 1)));
                    vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(50 * mult, 0).RotatedBy(Projectile.velocity.ToRotation() + MathHelper.PiOver2), col, new Vector2(_off, 0)));
                }
            }
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (vertices.Count > 2)
            {
                Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, ExtraTextures.laser2, false);
            }
            Main.spriteBatch.ApplySaved();
            Main.EntitySpriteDraw(glow, Projectile.Center - Projectile.velocity - Main.screenPosition, null, Color.Indigo, Projectile.rotation, glow.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            Main.EntitySpriteDraw(tex, Projectile.Center - Projectile.velocity - Main.screenPosition, null, Color.White, Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.velocity.Length() < 10)
                Projectile.velocity *= 1.05f;
        }
    }
    public class XAmethystCloseIn : ModProjectile
    {
        public override string Texture => "EbonianMod/Projectiles/ArchmageX/XAmethyst";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 250;
            Projectile.Size = new(18, 18);
        }
        float vfxOffset;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Texture2D glow = Helper.GetTexture(Texture + "_Glow");
            float alpha = Projectile.ai[2];
            Main.spriteBatch.Reload(BlendState.Additive);
            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            vfxOffset -= 0.015f;
            if (vfxOffset <= 0)
                vfxOffset = 1;
            vfxOffset = MathHelper.Clamp(vfxOffset, float.Epsilon, 1 - float.Epsilon);
            List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
            float s = 0;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float mult = (1f - fadeMult * i);

                if (mult < 0.5f)
                    s = MathHelper.Clamp(mult * 3.5f, 0, 0.5f) * 3;
                else
                    s = MathHelper.Clamp((-mult + 1) * 2, 0, 0.5f) * 3;

                if (i > 0 && Projectile.oldPos[i] != Vector2.Zero && Projectile.oldPos[i] != Projectile.position)
                {
                    Color col = Color.Lerp(Color.Indigo * 0.5f, Color.Gray, (float)(i / Projectile.oldPos.Length)) * 3 * alpha;

                    float __off = vfxOffset;
                    if (__off > 1) __off = -__off + 1;
                    float _off = (__off + mult) % 1f;
                    vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 + Projectile.velocity - Main.screenPosition + new Vector2(50 * mult, 0).RotatedBy(Projectile.velocity.ToRotation() + MathHelper.PiOver2), col, new Vector2(_off, 0)));
                    vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 + Projectile.velocity - Main.screenPosition + new Vector2(50 * mult, 0).RotatedBy(Projectile.velocity.ToRotation() - MathHelper.PiOver2), col, new Vector2(_off, 1)));
                }
            }
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (vertices.Count > 2)
            {
                Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, ExtraTextures.laser2, false);
            }
            Main.spriteBatch.ApplySaved();
            Main.EntitySpriteDraw(glow, Projectile.Center - Projectile.velocity * .5f - Main.screenPosition, null, Color.Indigo * alpha, Projectile.rotation, glow.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            Main.EntitySpriteDraw(tex, Projectile.Center - Projectile.velocity * .5f - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                Dust.NewDustPerfect(Projectile.Center, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
            }
        }
        public override bool? CanDamage() => Projectile.velocity.Length() > .5f;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 pos = new(Projectile.ai[0], Projectile.ai[1]);
            if (Projectile.velocity.Length() < 10)
                Projectile.velocity *= 1.05f;
            else
                Projectile.velocity = Projectile.velocity.RotatedBy(ToRadians(Helper.FromAToB(Projectile.Center, pos).X < 0 ? 0.25f : -0.25f));

            if (Projectile.timeLeft > 230 && Projectile.ai[2] < 1)
                Projectile.ai[2] += 0.1f;
            if (Projectile.timeLeft < 15)
            {
                Projectile.ai[2] -= 0.1f;
                if (Projectile.ai[2] <= 0)
                    Projectile.Kill();
            }
            if (pos != Vector2.Zero)
            {
                if (Projectile.Distance(pos) < 100)
                {
                    Projectile.ai[2] -= 0.1f;
                    if (Projectile.ai[2] <= 0)
                        Projectile.Kill();
                }
            }
        }
    }
}
