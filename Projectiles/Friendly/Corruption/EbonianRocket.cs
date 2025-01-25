using EbonianMod.Effects.Prims;
using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Projectiles.Friendly.Corruption
{
    public class EbonianRocket : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(36, 30);
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 400;
            Projectile.penetrate = -1;
        }
        float vfxOffset;
        public override bool PreDraw(ref Color lightColor)
        {

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

                if (i > 0 && Projectile.oldPos[i] != Vector2.Zero)
                {
                    Color col = Color.Green * mult * 5 * s;

                    float __off = vfxOffset;
                    if (__off > 1) __off = -__off + 1;
                    float _off = (__off + mult)%1f;
                    vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(20 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() + MathHelper.PiOver2), col, new Vector2(_off, 0)));
                    vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(20 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() - MathHelper.PiOver2), col, new Vector2(_off, 1)));
                }
            }
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (vertices.Count > 2)
            {
                Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, Helper.GetExtraTexture("FlamesSeamless"), false);
            }
            Main.spriteBatch.ApplySaved();

            bool shouldDraw = (Projectile.ai[1] == 0);
            return shouldDraw;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[1] != 2)
                Projectile.ai[1] = 1;
            return false;
        }
        public override void Kill(int timeLeft)
        {
            if (Projectile.ai[1] == 0)
            {
                Projectile a = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileType<GreenShockwave>(), Projectile.damage, 0, Projectile.owner);
                a.hostile = false;
                a.friendly = true;
                Helper.DustExplosion(Projectile.Center, Vector2.One, 0, Color.Green * 0.75f, true, true);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
        {
            if (Projectile.ai[1] != 2)
                Projectile.ai[1] = 1;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[1] == 1)
            {
                Projectile.timeLeft = 45;
                Projectile.velocity *= 0;
                Projectile a = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileType<GreenShockwave>(), Projectile.damage, 0, Projectile.owner);
                a.hostile = false;
                a.friendly = true;
                Helper.DustExplosion(Projectile.Center, Vector2.One, 0, Color.Green * 0.75f, true, true);
                Projectile.ai[1] = 2;
            }
            if (Projectile.ai[1] == 2)
            {
                Projectile.damage = 0;
                return;
            }
            Vector2 move = Vector2.Zero;
            float distance = 5050f;
            bool target = false;
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                if (Main.npc[k].active && !Main.npc[k].friendly && !Main.npc[k].dontTakeDamage && Main.npc[k].type != NPCID.TargetDummy)
                {
                    Vector2 newMove = Main.npc[k].Center - Projectile.Center;
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < distance)
                    {
                        move = newMove;
                        distance = distanceTo;
                        target = true;
                    }
                }
            }
            if (target && Projectile.timeLeft > 45)
            {
                AdjustMagnitude(ref move);
                float thing = Utils.GetLerpValue(0, 400, Projectile.timeLeft);
                float vel = MathHelper.Lerp(25, 8, thing);
                Projectile.velocity = (vel * Projectile.velocity + move) / vel;
                AdjustMagnitude(ref Projectile.velocity);
            }
            if (Projectile.timeLeft < 45)
            {
                Projectile.velocity *= 0.9f;
            }
        }

        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            float thing = Utils.GetLerpValue(0, 400, Projectile.timeLeft);
            float target = MathHelper.Lerp(25, 8, thing);
            if (magnitude > target)
            {
                vector *= target / magnitude;
            }
        }
    }
}