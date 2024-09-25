using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using static Terraria.GameContent.Bestiary.IL_BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions;
using static EbonianMod.Helper;
using Terraria.ID;
using Terraria.Map;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace EbonianMod.Projectiles.Terrortoma
{
    public class TBeam : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 165;
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
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.ai[0], 60, ref a) && Projectile.scale > 0.5f;
        }
        bool RunOnce = true;

        public override void AI()
        {
            if (RunOnce)
            {
                Projectile.velocity.Normalize();
                Projectile.rotation = Projectile.velocity.ToRotation();
                RunOnce = false;
            }
            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.position - new Vector2(30, 0) + Projectile.velocity.ToRotation().ToRotationVector2() * Main.rand.NextFloat(Projectile.ai[0]), 60, 60, DustID.CursedTorch, 2f);
                Main.dust[dust].scale = 2f;
                Main.dust[dust].velocity = Main.rand.NextVector2Circular(5, 5);
                Main.dust[dust].noGravity = true;
            }
            Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], .05f, 0.015f);

            Projectile.ai[0] = MathHelper.SmoothStep(Projectile.ai[0], 2048, 0.35f);

            visual1 -= 0.04f;
            if (visual1 <= 0)
                visual1 = 1;
            visual1 = MathHelper.Clamp(visual1, float.Epsilon, 1 - float.Epsilon);

            visual2 -= 0.038f;
            if (visual2 <= 0)
                visual2 = 1;
            visual2 = MathHelper.Clamp(visual2, float.Epsilon, 1 - float.Epsilon);

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(Projectile.Center, Main.LocalPlayer.Center), Projectile.ai[2]);
            startSize = MathHelper.Lerp(startSize, 0, 0.01f);
            //Projectile.velocity = -Projectile.velocity.RotatedBy(MathHelper.ToRadians(Projectile.ai[1]));

            float progress = Utils.GetLerpValue(0, 165, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 4 * (Projectile.scale + 0.5f), 0, 1);
        }
        float visual1, visual2, startSize = 2f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Helper.GetExtraTexture("FlamesSeamless");
            Texture2D texture2 = Helper.GetExtraTexture("trail_04");
            float progress = Utils.GetLerpValue(0, 165, Projectile.timeLeft);
            float i_progress = MathHelper.Clamp(MathHelper.SmoothStep(1, 0.2f, progress) * 50, 0, 1 / MathHelper.Clamp(startSize, 1, 2));

            DrawVertices(Projectile.velocity.ToRotation(), texture, texture2, i_progress, 1);
            return false;
        }
        void DrawVertices(float rotation, Texture2D texture, Texture2D texture2, float i_progress, float alphaOffset)
        {
            Main.spriteBatch.SaveCurrent();
            List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
            List<VertexPositionColorTexture> vertices2 = new List<VertexPositionColorTexture>();
            Vector2 start = Projectile.Center - Main.screenPosition;
            Vector2 off = (rotation.ToRotationVector2() * (1600));
            Vector2 end = start + off;
            float rot = Helper.FromAToB(start, end).ToRotation();

            float s = 0.5f;
            for (float i = 0; i < 1; i += 0.002f)
            {
                if (i < 0.5f)
                    s = MathHelper.Clamp(i * 3.5f, 0, 0.5f);
                else
                    s = MathHelper.Clamp((-i + 1) * 2, 0, 0.5f);

                float __off = visual1;
                if (__off > 1) __off = -__off + 1;
                float _off = __off + i;

                float __off2 = visual2;
                if (__off2 > 1) __off = -__off + 1;
                float _off2 = __off + i;

                Color col = Color.Lerp(Color.LimeGreen, Color.LimeGreen * 1.2f, i) * (s * s * 2f * alphaOffset);
                vertices.Add(Helper.AsVertex(start + off * i + new Vector2(MathHelper.SmoothStep(MathHelper.Lerp(60, 100, i) * startSize, MathHelper.SmoothStep(120, 300, i), i * 3) * MathHelper.Clamp(startSize, 1, 2), 0).RotatedBy(rot + MathHelper.PiOver2) * i_progress, new Vector2(_off, 1), col * Projectile.scale));
                vertices.Add(Helper.AsVertex(start + off * i + new Vector2(MathHelper.SmoothStep(MathHelper.Lerp(60, 100, i) * startSize, MathHelper.SmoothStep(120, 300, i), i * 3) * MathHelper.Clamp(startSize, 1, 2), 0).RotatedBy(rot - MathHelper.PiOver2) * i_progress, new Vector2(_off, 0), col * Projectile.scale));

                col = Color.White * (s * s * 1.5f * alphaOffset);
                vertices2.Add(Helper.AsVertex(start + off * i + new Vector2(MathHelper.Lerp(MathHelper.Lerp(0, 30, i) * startSize, MathHelper.SmoothStep(100, 250, i), i * 2) * MathHelper.Clamp(startSize, 1, 2), 0).RotatedBy(rot + MathHelper.PiOver2) * i_progress, new Vector2(_off2, 1), col * Projectile.scale));
                vertices2.Add(Helper.AsVertex(start + off * i + new Vector2(MathHelper.Lerp(MathHelper.Lerp(0, 30, i) * startSize, MathHelper.SmoothStep(100, 250, i), i * 2) * MathHelper.Clamp(startSize, 1, 2), 0).RotatedBy(rot - MathHelper.PiOver2) * i_progress, new Vector2(_off2, 0), col * Projectile.scale));
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            if (vertices.Count >= 3 && vertices2.Count >= 3)
            {
                for (int i = 0; i < 2; i++)
                {
                    Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, texture, false);

                    Helper.DrawTexturedPrimitives(vertices2.ToArray(), PrimitiveType.TriangleStrip, texture2, false);
                    Helper.DrawTexturedPrimitives(vertices2.ToArray(), PrimitiveType.TriangleStrip, texture, false);

                    Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, texture2, false);
                }
            }
            Main.spriteBatch.ApplySaved();
        }
    }
}