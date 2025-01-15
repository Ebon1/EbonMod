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
using Terraria;
using Terraria.ModLoader;
using EbonianMod.NPCs.Garbage;
using EbonianMod.Dusts;

namespace EbonianMod.Projectiles.Garbage
{
    public class GarbageLaserSmall1 : ModProjectile
    {
        public override string Texture => Helper.Empty;
        int maxTime = 200;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1500;
        }
        public override void SetDefaults()
        {
            Projectile.Size = Vector2.One;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = maxTime;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.ToRotation().ToRotationVector2() * 600, 30, ref a) && Projectile.scale > 0.5f;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        public override void AI()
        {
            Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.NextFloat(-30, 30), 0).RotatedBy(Projectile.velocity.ToRotation()) + Projectile.velocity.ToRotation().ToRotationVector2() * Main.rand.NextFloat(250, 290), DustType<LineDustFollowPoint>(), Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.35f) * 3 * Main.rand.NextFloat(2, 5), 0, Color.OrangeRed, Main.rand.NextFloat(0.05f, 0.3f)).customData = Projectile.Center + Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * 1200;

            Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * Main.rand.NextFloat(50), DustID.Smoke, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(5, 8));
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (npc.active && npc.type == NPCType<HotGarbage>())
                Projectile.Center = npc.Center + new Vector2(-7 * npc.direction, npc.height * 0.4f);
            float progress = Utils.GetLerpValue(0, maxTime, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp(MathF.Sin(progress * MathHelper.Pi) * 5, 0, 1);
            Projectile.ai[2] += 0.025f;
            if (Projectile.ai[2] >= 1)
                Projectile.ai[2] = 0;
            Projectile.ai[2] = MathHelper.Clamp(Projectile.ai[2], 0, 1);

            Projectile.ai[1] -= 0.04f;
            if (Projectile.ai[1] <= 0)
                Projectile.ai[1] = 1;
            Projectile.ai[1] = MathHelper.Clamp(Projectile.ai[1], float.Epsilon, 1 - float.Epsilon);
            SoundStyle style = SoundID.Item13;
            SoundStyle style2 = SoundID.Item34;
            style.MaxInstances = 0;
            style2.MaxInstances = 0;
            if (Projectile.timeLeft % 10 == 0)
            {
                SoundEngine.PlaySound(style, Projectile.Center);
                SoundEngine.PlaySound(style2.WithPitchOffset(-0.5f).WithVolumeScale(0.5f), Projectile.Center);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {

            if (lightColor != Color.Transparent) return false;

            List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
            List<VertexPositionColorTexture> vertices2 = new List<VertexPositionColorTexture>();
            List<VertexPositionColorTexture> vertices3 = new List<VertexPositionColorTexture>();
            List<VertexPositionColorTexture> vertices4 = new List<VertexPositionColorTexture>();
            Texture2D texture = Helper.GetExtraTexture("trail_01");
            Texture2D texture2 = Helper.GetExtraTexture("Ex1");
            float progress = Utils.GetLerpValue(0, maxTime, Projectile.timeLeft);
            float i_progress = MathHelper.Clamp(MathHelper.SmoothStep(1, 0, progress) * 50, 0, 1);
            Vector2 start = Projectile.Center - Main.screenPosition;
            float factor = MathHelper.Lerp(MathF.Sin(Main.GlobalTimeWrappedHourly * 2), MathF.Cos(Main.GlobalTimeWrappedHourly * 2), (MathF.Sin(Main.GlobalTimeWrappedHourly) + 1) * 0.5f);
            Vector2 off = (Projectile.velocity.ToRotation().ToRotationVector2() * (600 + (factor * 30)));
            Vector2 end = start + off;
            float rot = Helper.FromAToB(start, end).ToRotation();

            float s = 0f;
            for (float i = 0; i < 1; i += 0.002f)
            {
                if (i < 0.5f)
                    s = MathHelper.Clamp(i * 3.5f, 0, 0.5f);
                else
                    s = MathHelper.Clamp((-i + 1) * 2, 0, 0.5f);

                float __off = Projectile.ai[1];
                if (__off > 1) __off = -__off + 1;
                float _off = __off + i;

                Color col = Color.Lerp(Color.DarkRed, Color.Orange, i) * (s * s * 4);
                vertices.Add(Helper.AsVertex(start + off * i + new Vector2(50 + MathHelper.SmoothStep(0, 50, i * 3), 0).RotatedBy(rot + MathHelper.PiOver2) * i_progress, new Vector2(_off, 1), col * Projectile.scale));
                vertices.Add(Helper.AsVertex(start + off * i + new Vector2(50 + MathHelper.SmoothStep(0, 50, i * 3), 0).RotatedBy(rot - MathHelper.PiOver2) * i_progress, new Vector2(_off, 0), col * Projectile.scale));

                col = Color.Lerp(Color.DarkRed, Color.Orange, i) * (s * s * 4);
                vertices2.Add(Helper.AsVertex(start + off * i + new Vector2(30 + MathHelper.SmoothStep(0, 250, i), 0).RotatedBy(rot + MathHelper.PiOver2) * i_progress, new Vector2(_off, 1), col * Projectile.scale));
                vertices2.Add(Helper.AsVertex(start + off * i + new Vector2(30 + MathHelper.SmoothStep(0, 250, i), 0).RotatedBy(rot - MathHelper.PiOver2) * i_progress, new Vector2(_off, 0), col * Projectile.scale));


            }

            //Main.graphics.GraphicsDevice.Textures[0] = texture;
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (vertices.Count >= 3 && vertices2.Count >= 3)
            {
                for (int i = 0; i < 2; i++)
                {
                    Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, texture, false);
                    Helper.DrawTexturedPrimitives(vertices2.ToArray(), PrimitiveType.TriangleStrip, texture2, false);
                }
            }
            Main.spriteBatch.ApplySaved();

            return false;
        }
    }
    public class GarbageLaserSmall2 : ModProjectile
    {
        public override string Texture => Helper.Empty;
        int maxTime = 160;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1500;
        }
        public override void SetDefaults()
        {
            Projectile.Size = Vector2.One;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = maxTime;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.ToRotation().ToRotationVector2() * 800, 50, ref a) && Projectile.scale > 0.5f;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        public override void AI()
        {
            Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.NextFloat(-150, 150), 0).RotatedBy(Projectile.velocity.ToRotation()) + Projectile.velocity.ToRotation().ToRotationVector2() * Main.rand.NextFloat(250, 290), DustType<LineDustFollowPoint>(), Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * 3 * Main.rand.NextFloat(5, 10), 0, Color.Orange, Main.rand.NextFloat(0.05f, 0.3f)).customData = Projectile.Center + Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * 1200;

            Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * Main.rand.NextFloat(50), DustID.Smoke, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(5, 8));


            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (npc.active && npc.type == NPCType<HotGarbage>())
                Projectile.Center = npc.Center + new Vector2(-7 * npc.direction, npc.height * 0.4f);
            float progress = Utils.GetLerpValue(0, maxTime, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp(MathF.Sin(progress * MathHelper.Pi) * 5, 0, 1);
            Projectile.ai[2] += 0.025f;
            if (Projectile.ai[2] >= 1)
                Projectile.ai[2] = 0;
            Projectile.ai[2] = MathHelper.Clamp(Projectile.ai[2], 0, 1);

            Projectile.ai[1] -= 0.05f;
            if (Projectile.ai[1] <= 0)
                Projectile.ai[1] = 1;
            Projectile.ai[1] = MathHelper.Clamp(Projectile.ai[1], float.Epsilon, 1 - float.Epsilon);
            SoundStyle style = SoundID.Item13;
            SoundStyle style2 = SoundID.Item34;
            style.MaxInstances = 0;
            style2.MaxInstances = 0;
            if (Projectile.timeLeft % 10 == 0)
            {
                SoundEngine.PlaySound(style.WithPitchOffset(0.3f).WithVolumeScale(0.5f), Projectile.Center);
                SoundEngine.PlaySound(style2.WithPitchOffset(-0.2f).WithVolumeScale(0.5f), Projectile.Center);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {

            if (lightColor != Color.Transparent) return false;

            List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
            List<VertexPositionColorTexture> vertices2 = new List<VertexPositionColorTexture>();
            Texture2D texture = Helper.GetExtraTexture("FlamesSeamless");
            Texture2D texture2 = Helper.GetExtraTexture("wavyLaser2");
            float progress = Utils.GetLerpValue(0, maxTime, Projectile.timeLeft);
            float i_progress = MathHelper.Clamp(MathHelper.SmoothStep(1, 0, progress) * 50, 0, 1);
            Vector2 start = Projectile.Center - Main.screenPosition;
            float factor = MathHelper.Lerp(MathF.Sin(Main.GlobalTimeWrappedHourly * 2), MathF.Cos(Main.GlobalTimeWrappedHourly * 2), (MathF.Sin(Main.GlobalTimeWrappedHourly) + 1) * 0.5f);
            Vector2 off = (Projectile.velocity.ToRotation().ToRotationVector2() * (800 + (factor * 30)));
            Vector2 end = start + off;
            float rot = Helper.FromAToB(start, end).ToRotation();

            float s = 0f;
            for (float i = 0; i < 1; i += 0.005f)
            {
                if (i < 0.5f)
                    s = MathHelper.Clamp(i * 3.5f, 0, 0.5f);
                else
                    s = MathHelper.Clamp((-i + 1) * 2, 0, 0.5f);

                float __off = Projectile.ai[1];
                if (__off > 1) __off = -__off + 1;
                float _off = __off + i;

                Color col = Color.Lerp(Color.Red, Color.OrangeRed, i) * (s * s * 4);
                vertices.Add(Helper.AsVertex(start + off * i + new Vector2(MathHelper.SmoothStep(0, 70, i * 3), 0).RotatedBy(rot + MathHelper.PiOver2) * i_progress, new Vector2(_off, 1), col * Projectile.scale));
                vertices.Add(Helper.AsVertex(start + off * i + new Vector2(MathHelper.SmoothStep(0, 70, i * 3), 0).RotatedBy(rot - MathHelper.PiOver2) * i_progress, new Vector2(_off, 0), col * Projectile.scale));

                col = Color.Lerp(Color.OrangeRed, Color.Orange, i) * (s * s * 2);
                vertices2.Add(Helper.AsVertex(start + off * i + new Vector2(MathHelper.Lerp(0, 100, i), 0).RotatedBy(rot + MathHelper.PiOver2) * i_progress, new Vector2(_off, 1), col * Projectile.scale));
                vertices2.Add(Helper.AsVertex(start + off * i + new Vector2(MathHelper.Lerp(0, 100, i), 0).RotatedBy(rot - MathHelper.PiOver2) * i_progress, new Vector2(_off, 0), col * Projectile.scale));
            }

            //Main.graphics.GraphicsDevice.Textures[0] = texture;
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (vertices.Count >= 3 && vertices2.Count >= 3)
            {
                for (int i = 0; i < 4; i++)
                {
                    Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, texture, false);
                    Helper.DrawTexturedPrimitives(vertices2.ToArray(), PrimitiveType.TriangleStrip, texture2, false);
                }
            }
            Main.spriteBatch.ApplySaved();

            return false;
        }
    }
    public class GarbageLaserSmall3 : ModProjectile
    {
        public override string Texture => Helper.Empty;
        int maxTime = 140;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1500;
        }
        public override void SetDefaults()
        {
            Projectile.Size = Vector2.One;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = maxTime;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.ToRotation().ToRotationVector2() * 1000, 100, ref a) && Projectile.scale > 0.5f;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        public override void AI()
        {
            Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.NextFloat(-300, 300), 0).RotatedBy(Projectile.velocity.ToRotation()) + Projectile.velocity.ToRotation().ToRotationVector2() * Main.rand.NextFloat(250, 290), DustType<LineDustFollowPoint>(), Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * 3 * Main.rand.NextFloat(4, 7), 0, Color.Gold, Main.rand.NextFloat(0.05f, 0.3f)).customData = Projectile.Center + Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * 1200;

            Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * Main.rand.NextFloat(50), DustID.Smoke, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(5, 8));
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (npc.active && npc.type == NPCType<HotGarbage>())
                Projectile.Center = npc.Center + new Vector2(-7 * npc.direction, npc.height * 0.4f);
            float progress = Utils.GetLerpValue(0, maxTime, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp(MathF.Sin(progress * MathHelper.Pi) * 5, 0, 1);
            Projectile.ai[2] += 0.05f;
            if (Projectile.ai[2] >= 1)
                Projectile.ai[2] = 0;
            Projectile.ai[2] = MathHelper.Clamp(Projectile.ai[2], 0, 1);

            Projectile.ai[1] -= 0.05f;
            if (Projectile.ai[1] <= 0)
                Projectile.ai[1] = 1;
            Projectile.ai[1] = MathHelper.Clamp(Projectile.ai[1], float.Epsilon, 1 - float.Epsilon);
            SoundStyle style = SoundID.Item13;
            SoundStyle style2 = SoundID.Item34;
            style.MaxInstances = 0;
            style2.MaxInstances = 0;
            if (Projectile.timeLeft % 10 == 0)
            {
                SoundEngine.PlaySound(style.WithPitchOffset(0.6f).WithVolumeScale(0.5f), Projectile.Center);
                SoundEngine.PlaySound(style2.WithPitchOffset(0.2f).WithVolumeScale(0.5f), Projectile.Center);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {

            if (lightColor != Color.Transparent) return false;

            List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
            List<VertexPositionColorTexture> vertices2 = new List<VertexPositionColorTexture>();
            List<VertexPositionColorTexture> vertices3 = new List<VertexPositionColorTexture>();
            Texture2D texture = Helper.GetExtraTexture("trail_04");
            Texture2D texture2 = Helper.GetExtraTexture("FlamesSeamless");
            float progress = Utils.GetLerpValue(0, maxTime, Projectile.timeLeft);
            float i_progress = MathHelper.Clamp(MathHelper.SmoothStep(1, 0, progress) * 50, 0, 1);
            Vector2 start = Projectile.Center - Main.screenPosition;
            float factor = MathHelper.Lerp(MathF.Sin(Main.GlobalTimeWrappedHourly * 2), MathF.Cos(Main.GlobalTimeWrappedHourly * 2), (MathF.Sin(Main.GlobalTimeWrappedHourly) + 1) * 0.5f);
            Vector2 off = (Projectile.velocity.ToRotation().ToRotationVector2() * (1000 + (factor * 30)));
            Vector2 end = start + off;
            float rot = Helper.FromAToB(start, end).ToRotation();

            float s = 0f;
            for (float i = 0; i < 1; i += 0.01f)
            {
                if (i < 0.5f)
                    s = MathHelper.Clamp(i * 3.5f, 0, 0.5f);
                else
                    s = MathHelper.Clamp((-i + 1) * 2, 0, 0.5f);

                float __off = Projectile.ai[1];
                if (__off > 1) __off = -__off + 1;
                float _off = __off + i;

                Color col = Color.Lerp(Color.DarkRed, Color.Orange, i) * (s * s * 4);
                vertices.Add(Helper.AsVertex(start + off * i + new Vector2(50 + MathHelper.SmoothStep(0, 50, i * 3), 0).RotatedBy(rot + MathHelper.PiOver2) * i_progress, new Vector2(_off, 1), col * Projectile.scale));
                vertices.Add(Helper.AsVertex(start + off * i + new Vector2(50 + MathHelper.SmoothStep(0, 50, i * 3), 0).RotatedBy(rot - MathHelper.PiOver2) * i_progress, new Vector2(_off, 0), col * Projectile.scale));

                col = Color.Lerp(Color.DarkRed, Color.Orange, i) * (s * s * 4);
                vertices2.Add(Helper.AsVertex(start + off * i + new Vector2(50 + MathHelper.SmoothStep(0, 100, i), 0).RotatedBy(rot + MathHelper.PiOver2) * i_progress, new Vector2(_off, 1), col * Projectile.scale));
                vertices2.Add(Helper.AsVertex(start + off * i + new Vector2(50 + MathHelper.SmoothStep(0, 100, i), 0).RotatedBy(rot - MathHelper.PiOver2) * i_progress, new Vector2(_off, 0), col * Projectile.scale));


            }

            //Main.graphics.GraphicsDevice.Textures[0] = texture;
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (vertices.Count >= 3 && vertices2.Count >= 3)
            {
                for (int i = 0; i < 2; i++)
                {
                    Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, texture, false);
                    Helper.DrawTexturedPrimitives(vertices2.ToArray(), PrimitiveType.TriangleStrip, texture2, false);
                }
            }
            Main.spriteBatch.ApplySaved();

            return false;
        }
    }
}
