using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Projectiles.Friendly.Underworld
{
    public class MagmaArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 35;
            ProjectileID.Sets.TrailingMode[Type] = 2;
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
                    Color col = Color.Red * mult * 3;

                    float __off = vfxOffset;
                    if (__off > 1) __off = -__off + 1;
                    float _off = __off + mult;
                    vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(15 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() + MathHelper.PiOver2), col, new Vector2(_off, 0)));
                    vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(15 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() - MathHelper.PiOver2), col, new Vector2(_off, 1)));
                }
            }
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (vertices.Count > 2)
            {
                Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, ExtraTextures.FlamesSeamless, false);
            }
            Main.spriteBatch.ApplySaved();
            return true;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 500;
            Projectile.Size = new(18, 38);
        }
        float startVel;
        public override void OnKill(int timeLeft)
        {
            foreach (Vector2 pos in Projectile.oldPos)
                for (int i = 0; i < 2; i++)
                    Dust.NewDustPerfect(pos + Projectile.Size / 2, DustID.Torch, Main.rand.NextVector2Circular(3, 3));
            //Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileID.DaybreakExplosion, Projectile.damage, Projectile.knockBack);
            //Projectile a = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileType<FlameExplosion>(), Projectile.damage, Projectile.knockBack);
            //a.hostile = false;
            //a.friendly = true;
            for (int i = 0; i < Main.rand.Next(1, 3); i++)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Main.rand.NextVector2Circular(15, 15), ProjectileType<Gibs>(), Projectile.damage, Projectile.knockBack, ai2: 1);
            }
        }
        Vector2 target;
        public override void AI()
        {
            if (Projectile.velocity.Y < 8 && Projectile.timeLeft < 485)
                Projectile.velocity.Y += 0.1f;
            if (Projectile.timeLeft % 2 == 0)
                Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Projectile.velocity).noGravity = true;
            else
                Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Main.rand.NextVector2Circular(1.5f, 1.5f));
            if (Projectile.ai[2] == 0)
            {
                startVel = Projectile.velocity.Length();
                target = Main.MouseWorld;
                Projectile.ai[2] = 1;
                //Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity, ProjectileType<Gibs>(), Projectile.damage, Projectile.knockBack, ai2: 1, ai1: 1);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            /**/
        }
    }

    public class MagmaArrowHostile : ModProjectile
    {
        public override string Texture => "EbonianMod/Projectiles/Friendly/Underworld/MagmaArrow";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 35;
            ProjectileID.Sets.TrailingMode[Type] = 2;
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
                    Color col = Color.Red * mult * 3;

                    float __off = vfxOffset;
                    if (__off > 1) __off = -__off + 1;
                    float _off = __off + mult;
                    vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(15 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() + MathHelper.PiOver2), col, new Vector2(_off, 0)));
                    vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(15 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() - MathHelper.PiOver2), col, new Vector2(_off, 1)));
                }
            }
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (vertices.Count > 2)
            {
                Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, ExtraTextures.FlamesSeamless, false);
            }
            Main.spriteBatch.ApplySaved();
            return true;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 500;
            Projectile.Size = new(18, 38);
        }
        public override void AI()
        {
            if (Projectile.velocity.Y < 8 && Projectile.timeLeft < 485)
                Projectile.velocity.Y += 0.1f;
            if (Projectile.timeLeft % 2 == 0)
                Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Projectile.velocity).noGravity = true;
            else
                Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Main.rand.NextVector2Circular(1.5f, 1.5f));
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            /**/
        }
    }
}
