using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using EbonianMod.Dusts;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using EbonianMod.Effects.Prims;

namespace EbonianMod.Projectiles.ArchmageX
{
    public class XTentacle : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/laser_purple";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 100;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        private List<float> rots;

        public int len;
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 160;
            Projectile.netUpdate = true;
            Projectile.netUpdate2 = true;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.extraUpdates = 2;
            rots = new List<float>();
            len = 0;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public Color drawColor;
        public Vector2 OldPoint(int i)
        {
            return Projectile.Center - Main.screenPosition + (Projectile.oldRot[i] * (float)(90f - i) / 90f + Projectile.ai[0]).ToRotationVector2() * (.12f * i * (float)(1f - (float)Math.Pow(.975f, i)) / .025f);
        }
        float value;
        public override void AI()
        {
            if (Projectile.timeLeft == 99)
            {
                if (Projectile.ai[0] == 0)
                    Projectile.ai[0] = 70;
                if (Projectile.ai[1] == 0)
                    Projectile.ai[1] = 0.5f;
            }
            for (int i = 0; i < 3; i++)
            {
                value += Projectile.ai[1];
                if (Projectile.timeLeft % 1 == 0)
                {
                    float factor = 1f;
                    Vector2 velocity = Projectile.velocity * factor * 4f;
                    Projectile.rotation = 0.1f * (float)Math.Sin((double)(value / 100f)) + velocity.ToRotation();
                    rots.Insert(0, Projectile.rotation);
                    while (rots.Count > Projectile.ai[0])
                    {
                        rots.RemoveAt(rots.Count - 1);
                    }
                }
                if (len < Projectile.ai[0] && Projectile.timeLeft > 80)
                {
                    len++;
                }
                if (len >= 0 && Projectile.timeLeft <= 80)
                {
                    len--;
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int i = 1; i < len; i++)
            {
                float factor = (float)i / (float)len;
                float w = 10 * MathHelper.SmoothStep(0.8f, 0.1f, factor);
                if (Collision.CheckAABBvAABBCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - new Vector2(w, w) + Utils.RotatedBy(new Vector2((float)(5 * i), 0f), rots[i]), new Vector2(w, w) * 2f))
                {
                    return true;
                }
            }
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (lightColor != Color.Transparent)
                return false;
            List<VertexInfo2> bars = new List<VertexInfo2>();
            for (int i = 1; i < len; i++)
            {
                float factor = (float)i / (float)len;
                Vector2 v0 = Projectile.Center + Utils.RotatedBy(new Vector2((float)(5 * (i - 1)), 0f), rots[i - 1]);
                Vector2 v1 = Projectile.Center + Utils.RotatedBy(new Vector2((float)(5 * i), 0f), rots[i]);
                Vector2 normaldir = v1 - v0;
                normaldir = new Vector2(normaldir.Y, 0f - normaldir.X);
                ((Vector2)(normaldir)).Normalize();
                float w = 10 * MathHelper.SmoothStep(0.8f, 0.1f, factor);
                bars.Add(new VertexInfo2(v1 + w * normaldir, new Vector3(factor, 0f, 0f), drawColor));
                bars.Add(new VertexInfo2(v1 - w * normaldir, new Vector3(factor, 1f, 0f), drawColor));
            }
            if (bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin((SpriteSortMode)1, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
                Matrix projection = Matrix.CreateOrthographicOffCenter(0f, (float)Main.screenWidth, (float)Main.screenHeight, 0f, 0f, 1f);
                Matrix model = Matrix.CreateTranslation(new Vector3(0f - Main.screenPosition.X, 0f - Main.screenPosition.Y, 0f)) * Main.GameViewMatrix.ZoomMatrix;
                EbonianMod.Tentacle.Parameters[0].SetValue(model * projection);
                EbonianMod.Tentacle.CurrentTechnique.Passes[0]
                    .Apply();
                ((Game)Main.instance).GraphicsDevice.Textures[0] = (Texture)(object)ModContent.Request<Texture2D>(Texture, (AssetRequestMode)2).Value;
                ((Game)Main.instance).GraphicsDevice.DrawUserPrimitives<VertexInfo2>((PrimitiveType)1, bars.ToArray(), 0, bars.Count - 2);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin((SpriteSortMode)0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, (Effect)null, Main.GameViewMatrix.TransformationMatrix);
            }
            return false;
        }
    }
}
