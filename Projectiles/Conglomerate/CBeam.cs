using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using EbonianMod.Dusts;
using Terraria.DataStructures;
using EbonianMod.Projectiles.VFXProjectiles;
using Terraria.Utilities;
using System.Reflection.Metadata;
using EbonianMod.NPCs.Conglomerate;

namespace EbonianMod.Projectiles.Conglomerate
{
    internal class CBeam : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 135;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override string Texture => "EbonianMod/Extras/Empty";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 7000;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
        }
        int damage;
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.ai[0], 60, ref a) && Projectile.scale > 0.5f;
        }
        bool RunOnce = true;
        public override bool PreAI()
        {
            NPC owner = Main.npc[(int)Projectile.ai[2]];

            if (owner.active && owner.type == ModContent.NPCType<NPCs.Conglomerate.Conglomerate>())
            {
                Projectile.velocity = (owner.rotation + MathHelper.PiOver2).ToRotationVector2();
                Projectile.Center = owner.Center;
            }
            return base.PreAI();
        }
        public override void AI()
        {
            if (RunOnce)
            {
                Projectile.velocity.Normalize();
                Projectile.rotation = Projectile.velocity.ToRotation();
                RunOnce = false;
            }
            if (Projectile.timeLeft == 131)
            {
                Projectile.NewProjectile(null, Projectile.Center + Projectile.velocity * 30, Vector2.Zero, ModContent.ProjectileType<CFlareExplosion>(), 0, 0, -1, 0, 0, Projectile.whoAmI);
                EbonianSystem.ScreenShakeAmount = 15;
            }
            if (Projectile.timeLeft % 5 == 0 && Projectile.timeLeft > 5)
            {
                if (EbonianSystem.ScreenShakeAmount < 13)
                    EbonianSystem.ScreenShakeAmount = 13;
                Projectile.NewProjectile(null, Projectile.Center + Projectile.velocity * 30, Vector2.Zero, ModContent.ProjectileType<CFlareExplosion>(), 0, 0, -1, 0, 0, Projectile.whoAmI);
            }
            NPC owner = Main.npc[(int)Projectile.ai[2]];

            if (owner.active && owner.type == ModContent.NPCType<NPCs.Conglomerate.Conglomerate>())
            {
                Projectile.velocity = (owner.rotation + MathHelper.PiOver2).ToRotationVector2();
                Projectile.Center = owner.Center;
            }
            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.position - new Vector2(30, 0) + Projectile.velocity.ToRotation().ToRotationVector2() * Main.rand.NextFloat(Projectile.ai[0]), 60, 60, DustID.CursedTorch, 2f);
                Main.dust[dust].scale = 2f;
                Main.dust[dust].velocity = Main.rand.NextVector2Circular(5, 5);
                Main.dust[dust].noGravity = true;
            }

            Projectile.ai[0] = MathHelper.SmoothStep(Projectile.ai[0], 2048, 0.35f);

            visual1 -= 0.04f;
            if (visual1 <= 0)
                visual1 = 1;
            visual1 = MathHelper.Clamp(visual1, float.Epsilon, 1 - float.Epsilon);

            visual2 -= 0.038f;
            if (visual2 <= 0)
                visual2 = 1;
            visual2 = MathHelper.Clamp(visual2, float.Epsilon, 1 - float.Epsilon);

            startSize = MathHelper.Lerp(startSize, 0, 0.01f);
            //Projectile.velocity = -Projectile.velocity.RotatedBy(MathHelper.ToRadians(Projectile.ai[1]));
            Projectile.rotation = Projectile.velocity.ToRotation();
            float progress = Utils.GetLerpValue(0, 135, Projectile.timeLeft);
            if (Projectile.timeLeft < 135)
                Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 5 * (Projectile.scale + 0.5f), 0, 1);
        }
        float visual1, visual2, startSize = 2f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Helper.GetExtraTexture("FlamesSeamless");
            Texture2D texture2 = Helper.GetExtraTexture("trail_04");
            Texture2D tex = Helper.GetExtraTexture("Extras2/light_01");

            float progress = Utils.GetLerpValue(0, 135, Projectile.timeLeft);
            float i_progress = MathHelper.Clamp(MathHelper.SmoothStep(1, 0.2f, progress) * 50, 0, 1 / MathHelper.Clamp(startSize, 1, 2));

            float alpha = (0.35f + MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.1f) * Projectile.scale;
            if (Projectile.timeLeft < 134)
            {
                Main.spriteBatch.Reload(BlendState.Additive);
                for (int i = 0; i < 3; i++)
                    Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.Red, Color.LimeGreen, 0.5f) * alpha, 0, tex.Size() / 2, 5 + Projectile.scale, SpriteEffects.None, 0);
                Main.spriteBatch.Reload(BlendState.AlphaBlend);
                DrawVertices(Projectile.Center, Projectile.velocity.ToRotation(), texture, texture2, i_progress, 8);
            }
            return false;
        }
        void DrawVertices(Vector2 pos, float rotation, Texture2D texture, Texture2D texture2, float i_progress, float alphaOffset, float quality = 0.002f, float max = 1)
        {
            Main.spriteBatch.SaveCurrent();
            List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
            List<VertexPositionColorTexture> vertices2 = new List<VertexPositionColorTexture>();
            Vector2 start = pos - Projectile.velocity * 75 - Main.screenPosition;
            Vector2 off = (rotation.ToRotationVector2() * (2048));
            Vector2 end = start + off;
            float rot = Helper.FromAToB(start, end).ToRotation();

            float s = 0.5f;
            for (float i = 1 - max; i < max; i += quality)
            {
                if (i < max / 2)
                    s = MathHelper.Clamp(i * 5f, 0, 0.5f);
                else
                    s = MathHelper.Clamp((-i + max) * 2, 0, 0.5f);

                float __off = visual1;
                if (__off > 1) __off = -__off + 1;
                float _off = __off + i;

                Color col = Color.Lerp(Color.Red, Color.Red * 1.2f, i) * (s * s * 2f * alphaOffset);
                vertices.Add(Helper.AsVertex(start + off * i + new Vector2(MathHelper.SmoothStep(MathHelper.Lerp(60, 100, i) * startSize, MathHelper.SmoothStep(220, 500, i), i * 3) * MathHelper.Clamp(startSize, 1, 2), 0).RotatedBy(rot + MathHelper.PiOver2) * i_progress, new Vector2(_off, 1), col * Projectile.scale));

                col = Color.Lerp(Color.Green, Color.Green * 1.2f, i) * (s * s * 2f * alphaOffset);
                vertices.Add(Helper.AsVertex(start + off * i + new Vector2(MathHelper.SmoothStep(MathHelper.Lerp(60, 100, i) * startSize, MathHelper.SmoothStep(220, 500, i), i * 3) * MathHelper.Clamp(startSize, 1, 2), 0).RotatedBy(rot - MathHelper.PiOver2) * i_progress, new Vector2(_off, 0), col * Projectile.scale));

                col = Color.White * (s * s * alphaOffset * MathHelper.SmoothStep(1, 0, i));
                vertices2.Add(Helper.AsVertex(start + off * i + new Vector2(MathHelper.Lerp(MathHelper.Lerp(0, 30, i) * startSize, MathHelper.SmoothStep(200, 10, i), i * 2) * MathHelper.Clamp(startSize, 1, 2), 0).RotatedBy(rot + MathHelper.PiOver2) * i_progress, new Vector2(_off, 1), col * Projectile.scale));
                vertices2.Add(Helper.AsVertex(start + off * i + new Vector2(MathHelper.Lerp(MathHelper.Lerp(0, 30, i) * startSize, MathHelper.SmoothStep(200, 10, i), i * 2) * MathHelper.Clamp(startSize, 1, 2), 0).RotatedBy(rot - MathHelper.PiOver2) * i_progress, new Vector2(_off, 0), col * Projectile.scale));
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (vertices.Count >= 3 && vertices2.Count >= 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, texture, false);
                    Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, texture2, false);
                }

                Helper.DrawTexturedPrimitives(vertices2.ToArray(), PrimitiveType.TriangleStrip, Helper.GetExtraTexture("Tentacle"), false);
                Helper.DrawTexturedPrimitives(vertices2.ToArray(), PrimitiveType.TriangleStrip, texture, false);

            }
            Main.spriteBatch.ApplySaved();
        }
    }
    public class CFlareExplosion : ModProjectile
    {
        public override string Texture => Helper.Placeholder;
        public override void SetDefaults()
        {
            Projectile.height = 1;
            Projectile.width = 1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;
            Projectile.tileCollide = false;
        }
        public override bool ShouldUpdatePosition() => false;
        int seed;
        public override void OnSpawn(IEntitySource source)
        {
            seed = Main.rand.Next(int.MaxValue);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (seed == 0) return false;
            Texture2D tex = Helper.GetExtraTexture("cone5");
            UnifiedRandom rand = new UnifiedRandom(seed);
            float max = 40;
            Main.spriteBatch.Reload(BlendState.Additive);
            float ringScale = MathHelper.Lerp(1, 0, MathHelper.Clamp(vfxIncrement * 6.5f, 0, 1));
            if (ringScale > 0.01f)
            {
                for (float i = 0; i < max; i++)
                {
                    float angle = Helper.CircleDividedEqually(i, max);
                    float scale = rand.NextFloat(0.2f, 1.15f);
                    Vector2 offset = new Vector2(Main.rand.NextFloat(50, 100) * ringScale * scale, 0).RotatedBy(angle);
                    for (float j = 0; j < 2; j++)
                        Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.Lerp(Color.Red, Color.LimeGreen, rand.NextFloat()) * ringScale, angle, tex.Size() / 2, new Vector2(MathHelper.Clamp(vfxIncrement * 6.5f, 0, 1), ringScale) * scale * 0.2f, SpriteEffects.None, 0);
                }
            }

            float alpha = MathHelper.Lerp(0.5f, 0, Projectile.ai[1]) * 2;
            for (float i = 0; i < max; i++)
            {
                float angle = Helper.CircleDividedEqually(i, max);
                float scale = rand.NextFloat(0.2f, 1.15f);
                Vector2 offset = new Vector2(Main.rand.NextFloat(50) * Projectile.ai[1] * scale, 0).RotatedBy(angle);
                for (float j = 0; j < 2; j++)
                    Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.Lerp(Color.Red, Color.LimeGreen, rand.NextFloat()) * alpha, angle, tex.Size() / 2, new Vector2(Projectile.ai[1], alpha) * scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        float vfxIncrement;
        public override void AI()
        {
            vfxIncrement = MathHelper.Lerp(vfxIncrement, 0.2f, 0.18f);
            Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], Projectile.ai[0] + vfxIncrement + (Projectile.ai[0] * 0.075f), 0.25f);
            Projectile.ai[1] = MathHelper.SmoothStep(0, 1.5f, Projectile.ai[0]);

            Projectile owner = Main.projectile[(int)Projectile.ai[2]];
            if (owner.active && owner.type == ModContent.ProjectileType<CBeam>())
            {
                Projectile.Center = owner.Center;
            }

            if (Projectile.timeLeft >= 190 && Projectile.timeLeft < 194)
            {
                UnifiedRandom rand = new UnifiedRandom(seed);
                float max = 10 + ((Projectile.timeLeft - 190) * 10);
                for (int i = ((Projectile.timeLeft - 190) * 10); i < max; i++)
                {
                    float angle = Helper.CircleDividedEqually(i, max);
                    float scale = rand.NextFloat(1f - (Projectile.timeLeft - 190) * 0.2f);
                    Vector2 offset = new Vector2(Main.rand.NextFloat(50) * scale, 0).RotatedBy(angle);
                    int jMax = rand.Next(3, 5);
                    for (int j = 0; j < jMax; j++)
                        Dust.NewDustPerfect(Projectile.Center + offset * 0.5f, Main.rand.NextBool() ? DustID.IchorTorch : DustID.CursedTorch, Helper.FromAToB(Projectile.Center, Projectile.Center + offset).RotatedByRandom(MathHelper.PiOver4 * (j == 0 ? 0 : 1)) * (scale * 20)).noGravity = true;
                }
            }

            if (Projectile.ai[1] > 1.15f)
                Projectile.Kill();
        }
    }
}
