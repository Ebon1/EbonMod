using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace EbonianMod.Projectiles.Enemy.Corruption
{
    public class RegorgerBolt : ModProjectile
    {
        public override string Texture => Helper.Placeholder;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 50;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2000;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 0;
            Projectile.extraUpdates = 4;
            Projectile.timeLeft = 1000;
            Projectile.Size = new(5, 5);
        }
        float vfxOffset;
        public override bool PreDraw(ref Color lightColor)
        {
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
                    Color col = Color.LawnGreen * mult * 2 * s;

                    float __off = vfxOffset;
                    if (__off > 1) __off = -__off + 1;
                    float _off = __off + mult;
                    vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(40 * mult, 0).RotatedBy(Projectile.velocity.ToRotation() + MathHelper.PiOver2), col, new Vector2(_off, 0)));
                    vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(40 * mult, 0).RotatedBy(Projectile.velocity.ToRotation() - MathHelper.PiOver2), col, new Vector2(_off, 1)));
                }
            }
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (vertices.Count > 2)
            {
                Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, Helper.GetExtraTexture("laser2"), false);
            }
            Main.spriteBatch.ApplySaved();
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[2]++ == 0)
                Projectile.timeLeft = 30 * 4;
            Projectile.velocity = Vector2.Zero;
            return false;
        }
        Vector2 velocity;
        public override void OnSpawn(IEntitySource source)
        {
            velocity = Projectile.velocity * 5;
        }
        public override void AI()
        {
            if (Projectile.timeLeft % 5 == 0)
                Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, Projectile.velocity * Main.rand.NextFloat(), Scale: 2).noGravity = true;
            if (Projectile.ai[2] > 0)
            {
                Projectile.ai[1] = MathHelper.Lerp(Projectile.ai[1], 1, 0.001f);
            }
            Projectile.ai[0]++;
            Projectile.direction = velocity.X > 0 ? 1 : -1;
            Projectile.velocity = velocity;
            Projectile.Center += new Vector2(0, MathF.Sin(Projectile.ai[0] * (1 / 3f)) * 0.1f).RotatedBy(velocity.ToRotation());
            if (Projectile.ai[0] > 550)
                Projectile.ai[0] = 0;
        }
    }
}
