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

namespace EbonianMod.Projectiles.Terrortoma
{
    public class TBeam : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 25;
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
            ProjectileID.Sets.TrailCacheLength[Type] = 2;
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
        bool RunOnce;

        public override void AI()
        {
            if (RunOnce)
            {
                Projectile.velocity.Normalize();
                damage = Projectile.damage;
                RunOnce = false;
            }
            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.position - new Vector2(30, 0) + Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(Projectile.ai[0]), 60, 60, DustID.CursedTorch, 2f);
                Main.dust[dust].scale = 2f;
                Main.dust[dust].velocity = Main.rand.NextVector2Circular(5, 5);
                Main.dust[dust].noGravity = true;
            }
            if (visualOffset == 0)
                visualOffset = Main.rand.NextFloat(0.75f, 1);
            visual1 += 30 * visualOffset;
            visual2 += 35 * visualOffset;

            if (Projectile.rotation != Projectile.oldRot[1])
            {
                float len = 2048;
                Vector2 vel = Projectile.velocity;
                vel.Normalize();
            }
            Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], .006f, 0.015f);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(Projectile.Center, Main.LocalPlayer.Center), Projectile.ai[2]);
            Projectile.ai[0] = MathHelper.SmoothStep(Projectile.ai[0], 2048, 0.35f);

            Projectile.ai[1] -= 0.03f;
            if (Projectile.ai[1] <= 0)
                Projectile.ai[1] = 1;
            Projectile.ai[1] = MathHelper.Clamp(Projectile.ai[1], float.Epsilon, 1 - float.Epsilon);

            Projectile.rotation = Projectile.velocity.ToRotation();

            //Projectile.velocity = -Projectile.velocity.RotatedBy(MathHelper.ToRadians(Projectile.ai[1]));

            float progress = Utils.GetLerpValue(0, 165, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 5, 0, 1);
        }
        float visualOffset;
        float visual1, visual2;
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix);

            List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
            List<VertexPositionColorTexture> vertices2 = new List<VertexPositionColorTexture>();
            Texture2D texture = Helper.GetExtraTexture("FlamesSeamless");
            Texture2D texture2 = Helper.GetExtraTexture("Extras2/spark_08");
            float progress = Utils.GetLerpValue(0, 165, Projectile.timeLeft);
            float i_progress = MathHelper.Clamp(MathHelper.SmoothStep(1, 0, progress) * 50, 0, 1);
            Vector2 start = Projectile.Center - Main.screenPosition;
            Vector2 off = (Projectile.velocity.ToRotation().ToRotationVector2() * (2000 + (MathF.Sin(Main.GlobalTimeWrappedHourly * 0.1f) * 250)));
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

                Color col = Color.Lerp(Color.LawnGreen, Color.Green, i) * (s * s * 5);
                vertices.Add(Helper.AsVertex(start + off * i + new Vector2(MathHelper.SmoothStep(0, 420, i * 3), 0).RotatedBy(rot + MathHelper.PiOver2) * i_progress, new Vector2(_off, 1), col * Projectile.scale));
                vertices.Add(Helper.AsVertex(start + off * i + new Vector2(MathHelper.SmoothStep(0, 420, i * 3), 0).RotatedBy(rot - MathHelper.PiOver2) * i_progress, new Vector2(_off, 0), col * Projectile.scale));

                col = Color.Lerp(Color.White, Color.Olive, i) * (s * s * 2);
                vertices2.Add(Helper.AsVertex(start + off * i + new Vector2(MathHelper.Lerp(0, 950, i * 2), 0).RotatedBy(rot + MathHelper.PiOver2) * i_progress, new Vector2(_off, 1), col * Projectile.scale));
                vertices2.Add(Helper.AsVertex(start + off * i + new Vector2(MathHelper.Lerp(0, 950, i * 2), 0).RotatedBy(rot - MathHelper.PiOver2) * i_progress, new Vector2(_off, 0), col * Projectile.scale));
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
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
}