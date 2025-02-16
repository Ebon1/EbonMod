using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using EbonianMod.Common.Systems;
using Terraria.GameContent;
using EbonianMod.Effects.Prims;

namespace EbonianMod.Projectiles.Cecitior
{
    internal class CecitiorClawSlash : ModProjectile
    {
        public override string Texture => Helper.Empty;
        const int max = 40;
        public override void SetDefaults()
        {
            Projectile.Size = Vector2.One;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = max;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            return (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * MathHelper.Lerp(0, 200, MathHelper.Clamp(Projectile.ai[0], 0, 1f)), 29, ref a) || Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + -Projectile.velocity.SafeNormalize(Vector2.UnitX) * MathHelper.Lerp(0, 200, MathHelper.Clamp(Projectile.ai[0] - 1, 0, 1f)), 29, ref a)) && Projectile.scale > 0.5f;
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundStyle a = SoundID.DD2_SonicBoomBladeSlash;
            a.PitchVariance = 0.2f;
            a.MaxInstances = 30;
            SoundEngine.PlaySound(a, Projectile.Center);

            if (Projectile.ai[2] == 1)
                SoundEngine.PlaySound(EbonianSounds.cecitiorSlice, Projectile.Center);
            else
                SoundEngine.PlaySound(EbonianSounds.clawSwipe.WithVolumeScale(1.5f), Projectile.Center);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            float progress = Utils.GetLerpValue(0, max, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * (Projectile.scale + 0.5f), 0, 1);
            Projectile.ai[0] += 0.17f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ExtraTextures.laser4;
            float s = 0f;
            List<VertexInfo2> vertices = new();
            List<VertexInfo2> vertices2 = new();

            for (int i = 1; i < 100; i++)
            {
                Vector2 pos = Projectile.Center + Projectile.velocity * i * 2;
                Vector2 lastPos = Projectile.Center + Projectile.velocity * (i * 2 - 1);
                if (i < 50)
                    s = MathHelper.Lerp(0, 1, (float)(i) / 50);
                else
                    s = MathHelper.Lerp(1, 0, (float)(i - 50) / 50);

                float alpha = Projectile.scale * s * 0.25f;

                Vector2 start = pos - Main.screenPosition;
                Vector2 end = lastPos - Main.screenPosition;
                float rot = Projectile.velocity.ToRotation();
                float y = MathHelper.Lerp(-2, 2, s);
                vertices.Add(new VertexInfo2(start + new Vector2(2 + s * Projectile.scale * (105), y).RotatedBy(rot + MathHelper.PiOver2), new Vector3(s * 0.5f, 0, 0), Color.Maroon * alpha));
                vertices.Add(new VertexInfo2(start + new Vector2(2 + s * Projectile.scale * (105), y).RotatedBy(rot - MathHelper.PiOver2), new Vector3(s * 0.5f, 1, 0), Color.Maroon * alpha));

                vertices2.Add(new VertexInfo2(start + new Vector2(2 + s * Projectile.scale * (75) * 3.5f, y).RotatedBy(rot + MathHelper.PiOver2), new Vector3(s * 0.5f, 0, 0), Color.Black * alpha * .15f));
                vertices2.Add(new VertexInfo2(start + new Vector2(2 + s * Projectile.scale * (75) * 3.5f, y).RotatedBy(rot - MathHelper.PiOver2), new Vector3(s * 0.5f, 1, 0), Color.Black * alpha * .15f));

                pos = Projectile.Center - Projectile.velocity * i * 2;
                start = pos - Main.screenPosition;
                vertices.Add(new VertexInfo2(start + new Vector2(2 + s * Projectile.scale * (105), y).RotatedBy(rot + MathHelper.PiOver2), new Vector3(s * 0.5f, 0, 0), Color.Maroon * alpha));
                vertices.Add(new VertexInfo2(start + new Vector2(2 + s * Projectile.scale * (105), y).RotatedBy(rot - MathHelper.PiOver2), new Vector3(s * 0.5f, 1, 0), Color.Maroon * alpha));

                vertices2.Add(new VertexInfo2(start + new Vector2(2 + s * Projectile.scale * (75) * 3.5f, y).RotatedBy(rot + MathHelper.PiOver2), new Vector3(s * 0.5f, 0, 0), Color.Black * alpha * .15f));
                vertices2.Add(new VertexInfo2(start + new Vector2(2 + s * Projectile.scale * (75) * 3.5f, y).RotatedBy(rot - MathHelper.PiOver2), new Vector3(s * 0.5f, 1, 0), Color.Black * alpha * .15f));
            }
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.Textures[0] = tex;
            if (vertices.Count >= 3)
            {
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices2.ToArray(), 0, vertices2.Count - 2);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                for (int j = 0; j < 2; j++)
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

            }
            Main.spriteBatch.ApplySaved();
            return false;
        }
    }
    internal class ClawSlashGreen : ModProjectile
    {
        public override string Texture => Helper.Empty;
        const int max = 40;
        public override void SetDefaults()
        {
            Projectile.Size = Vector2.One;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = max;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            return (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * MathHelper.Lerp(0, 200, MathHelper.Clamp(Projectile.ai[0], 0, 1f)), 29, ref a) || Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + -Projectile.velocity.SafeNormalize(Vector2.UnitX) * MathHelper.Lerp(0, 200, MathHelper.Clamp(Projectile.ai[0] - 1, 0, 1f)), 29, ref a)) && Projectile.scale > 0.5f;
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundStyle a = SoundID.DD2_SonicBoomBladeSlash;
            a.PitchVariance = 0.2f;
            a.MaxInstances = 30;
            SoundEngine.PlaySound(a, Projectile.Center);

            if (Projectile.ai[2] == 1)
                SoundEngine.PlaySound(EbonianSounds.cecitiorSlice, Projectile.Center);
            else
                SoundEngine.PlaySound(EbonianSounds.clawSwipe.WithVolumeScale(1.5f), Projectile.Center);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            float progress = Utils.GetLerpValue(0, max, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * (Projectile.scale + 0.5f), 0, 1);
            Projectile.ai[0] += 0.17f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ExtraTextures.laser4;
            float s = 0f;
            List<VertexInfo2> vertices = new();
            List<VertexInfo2> vertices2 = new();

            for (int i = 1; i < 100; i++)
            {
                Vector2 pos = Projectile.Center + Projectile.velocity * i * 2;
                Vector2 lastPos = Projectile.Center + Projectile.velocity * (i * 2 - 1);
                if (i < 50)
                    s = MathHelper.Lerp(0, 1, (float)(i) / 50);
                else
                    s = MathHelper.Lerp(1, 0, (float)(i - 50) / 50);

                float alpha = Projectile.scale * s * 0.25f;

                Vector2 start = pos - Main.screenPosition;
                Vector2 end = lastPos - Main.screenPosition;
                float rot = Projectile.velocity.ToRotation();
                float y = MathHelper.Lerp(-2, 2, s);
                vertices.Add(new VertexInfo2(start + new Vector2(2 + s * Projectile.scale * (105), y).RotatedBy(rot + MathHelper.PiOver2), new Vector3(s * 0.5f, 0, 0), Color.LawnGreen * alpha));
                vertices.Add(new VertexInfo2(start + new Vector2(2 + s * Projectile.scale * (105), y).RotatedBy(rot - MathHelper.PiOver2), new Vector3(s * 0.5f, 1, 0), Color.LawnGreen * alpha));

                vertices2.Add(new VertexInfo2(start + new Vector2(2 + s * Projectile.scale * (75) * 3.5f, y).RotatedBy(rot + MathHelper.PiOver2), new Vector3(s * 0.5f, 0, 0), Color.Black * alpha * .15f));
                vertices2.Add(new VertexInfo2(start + new Vector2(2 + s * Projectile.scale * (75) * 3.5f, y).RotatedBy(rot - MathHelper.PiOver2), new Vector3(s * 0.5f, 1, 0), Color.Black * alpha * .15f));

                pos = Projectile.Center - Projectile.velocity * i * 2;
                start = pos - Main.screenPosition;
                vertices.Add(new VertexInfo2(start + new Vector2(2 + s * Projectile.scale * (105), y).RotatedBy(rot + MathHelper.PiOver2), new Vector3(s * 0.5f, 0, 0), Color.LawnGreen * alpha));
                vertices.Add(new VertexInfo2(start + new Vector2(2 + s * Projectile.scale * (105), y).RotatedBy(rot - MathHelper.PiOver2), new Vector3(s * 0.5f, 1, 0), Color.LawnGreen * alpha));

                vertices2.Add(new VertexInfo2(start + new Vector2(2 + s * Projectile.scale * (75) * 3.5f, y).RotatedBy(rot + MathHelper.PiOver2), new Vector3(s * 0.5f, 0, 0), Color.Black * alpha * .15f));
                vertices2.Add(new VertexInfo2(start + new Vector2(2 + s * Projectile.scale * (75) * 3.5f, y).RotatedBy(rot - MathHelper.PiOver2), new Vector3(s * 0.5f, 1, 0), Color.Black * alpha * .15f));
            }
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.Textures[0] = tex;
            if (vertices.Count >= 3)
            {
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices2.ToArray(), 0, vertices2.Count - 2);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                for (int j = 0; j < 2; j++)
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

            }
            Main.spriteBatch.ApplySaved();
            return false;
        }
    }
}
