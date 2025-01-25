using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace EbonianMod.Projectiles.Garbage
{
    public class GarbageTelegraph : ModProjectile
    {
        public override string Texture => Helper.Placeholder;
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(5, 5);
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 40;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Reload(BlendState.Additive);
            if (Projectile.ai[0] > 0)
            {
                Texture2D chevron = Helper.GetExtraTexture("chevron_single");
                Vector2 pos = Projectile.Center - Projectile.rotation.ToRotationVector2() * 180;
                float progress = Utils.GetLerpValue(0, 40, Projectile.timeLeft);
                float eAlpha = MathHelper.Lerp(1, 0, Projectile.ai[2]);
                for (float i = 0; i < Projectile.ai[0]; i += 60)
                {
                    float x = MathHelper.Clamp(MathHelper.SmoothStep(1, 0, (i / Projectile.ai[0]) * 5), 0, 1);
                    float f = MathHelper.Lerp(40, 0, x);
                    float alpha = MathHelper.Lerp(1, 0, x);
                    float chevS = MathHelper.Clamp(MathHelper.Lerp(0.5f, 3, i / Projectile.ai[0]), 0, 1);
                    //Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, Color.Lerp(Color.Maroon, Color.Red, i / Projectile.ai[0]) * (Projectile.ai[1] * alpha), Projectile.rotation, new Vector2(0, tex.Height / 2), scale, SpriteEffects.None, 0);
                    pos += Projectile.rotation.ToRotationVector2() * 60;

                    Vector2 chevP = pos + Projectile.rotation.ToRotationVector2() * ((Projectile.ai[2] * 240) + (progress * 10));
                    for (int k = 0; k < 4; k++)
                        Main.spriteBatch.Draw(chevron, chevP - Main.screenPosition, null, Color.Lerp(Color.Maroon, Color.Red, i / Projectile.ai[0]) * (Projectile.ai[1] * eAlpha * 10), Projectile.rotation, new Vector2(0, chevron.Height / 2), Projectile.ai[2] * 0.65f, SpriteEffects.None, 0);
                }
            }

            if (Projectile.ai[0] > 0)
            {
                Texture2D tex = Helper.GetExtraTexture("laser4");
                Vector2 start = Projectile.Center;
                Vector2 end = Projectile.Center + Projectile.velocity * Projectile.ai[0];
                Vector2 scale = new Vector2(1f, Projectile.ai[1]);
                float eAlpha = MathHelper.Lerp(1, 0, Projectile.ai[2]);
                List<VertexPositionColorTexture> vertices2 = new();
                List<VertexPositionColorTexture> vertices3 = new();
                for (float i = 0; i < Projectile.ai[0]; i++)
                {
                    float x = MathHelper.Clamp(MathHelper.SmoothStep(1, 0, (i / Projectile.ai[0]) * 5), 0, 1);
                    float f = 100;// MathHelper.Lerp(100, 0, x);
                    float alpha = MathHelper.Lerp(1, 0, x);
                    Color col = Color.Lerp(Color.Red, Color.IndianRed, i / Projectile.ai[0]) * (Projectile.ai[1] * alpha * Lerp(2, 0, i / Projectile.ai[0]));

                    Vector2 pos = Vector2.Lerp(start, end, (float)i / Projectile.ai[0]) - Main.screenPosition;
                    col = Color.Lerp(Color.Red, Color.IndianRed, i / Projectile.ai[0]) * (eAlpha * alpha * Lerp(2, 0, i / Projectile.ai[0]));

                    pos = Vector2.Lerp(start, end, (float)i / Projectile.ai[0]) + Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2) * (f * Projectile.ai[2]) - Main.screenPosition;
                    vertices2.Add(Helper.AsVertex(pos + new Vector2(4, 0).RotatedBy(Projectile.velocity.ToRotation() + PiOver2), col, new Vector2(0, 0)));
                    vertices2.Add(Helper.AsVertex(pos + new Vector2(4, 0).RotatedBy(Projectile.velocity.ToRotation() - PiOver2), col, new Vector2(1, 1)));

                    pos = Vector2.Lerp(start, end, (float)i / Projectile.ai[0]) + Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2) * (f * -Projectile.ai[2]) - Main.screenPosition;
                    vertices3.Add(Helper.AsVertex(pos + new Vector2(4, 0).RotatedBy(Projectile.velocity.ToRotation() + PiOver2), col, new Vector2(0, 0)));
                    vertices3.Add(Helper.AsVertex(pos + new Vector2(4, 0).RotatedBy(Projectile.velocity.ToRotation() - PiOver2), col, new Vector2(1, 1)));
                }

                if (vertices2.Count > 2 && vertices3.Count > 2)
                {
                    Helper.DrawTexturedPrimitives(vertices2.ToArray(), PrimitiveType.TriangleStrip, Helper.GetExtraTexture("Tentacle"), false);
                    Helper.DrawTexturedPrimitives(vertices3.ToArray(), PrimitiveType.TriangleStrip, Helper.GetExtraTexture("Tentacle"), false);
                }
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        public override void AI()
        {
            float progress = Utils.GetLerpValue(0, 40, Projectile.timeLeft);
            Projectile.ai[1] = MathHelper.Clamp(MathF.Sin(progress * MathF.PI) * 0.5f, 0, 1);
            Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], 1, 0.1f);

            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[0] == 0)
                Projectile.ai[0] = 2000;
        }
    }
    public class GarbageTelegraphSmall : ModProjectile
    {
        public override string Texture => Helper.Placeholder;
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(5, 5);
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 40;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Reload(BlendState.Additive);
            if (Projectile.ai[0] > 0)
            {
                Texture2D tex = Helper.GetExtraTexture("laser4");
                Vector2 start = Projectile.Center;
                Vector2 end = Projectile.Center + Projectile.velocity * Projectile.ai[0];
                Vector2 scale = new Vector2(1f, Projectile.ai[1]);
                float eAlpha = MathHelper.Lerp(1, 0, Projectile.ai[2]);
                List<VertexPositionColorTexture> vertices = new();
                List<VertexPositionColorTexture> vertices2 = new();
                List<VertexPositionColorTexture> vertices3 = new();
                for (float i = 0; i < Projectile.ai[0]; i++)
                {
                    float x = MathHelper.Clamp(MathHelper.SmoothStep(1, 0, (i / Projectile.ai[0]) * 5), 0, 1);
                    float f = MathHelper.Lerp(30, 0, x);
                    float alpha = MathHelper.Lerp(1, 0, x);
                    Color col = Color.Lerp(Color.Maroon, Color.Red, i / Projectile.ai[0]) * (Projectile.ai[1] * alpha * Lerp(2, 0, i / Projectile.ai[0]));

                    Vector2 pos = Vector2.Lerp(start, end, (float)i / Projectile.ai[0]) - Main.screenPosition;
                    vertices.Add(Helper.AsVertex(pos + new Vector2(4, 0).RotatedBy(Projectile.velocity.ToRotation() + PiOver2), col, new Vector2(0, 0)));
                    vertices.Add(Helper.AsVertex(pos + new Vector2(4, 0).RotatedBy(Projectile.velocity.ToRotation() - PiOver2), col, new Vector2(1, 1)));

                    col = Color.Lerp(Color.Maroon, Color.Red, i / Projectile.ai[0]) * (eAlpha * alpha * Lerp(2, 0, i / Projectile.ai[0]));

                    pos = Vector2.Lerp(start, end, (float)i / Projectile.ai[0]) + Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2) * (f * Projectile.ai[2]) - Main.screenPosition;
                    vertices2.Add(Helper.AsVertex(pos + new Vector2(4, 0).RotatedBy(Projectile.velocity.ToRotation() + PiOver2), col, new Vector2(0, 0)));
                    vertices2.Add(Helper.AsVertex(pos + new Vector2(4, 0).RotatedBy(Projectile.velocity.ToRotation() - PiOver2), col, new Vector2(1, 1)));

                    pos = Vector2.Lerp(start, end, (float)i / Projectile.ai[0]) + Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2) * (f * -Projectile.ai[2]) - Main.screenPosition;
                    vertices3.Add(Helper.AsVertex(pos + new Vector2(4, 0).RotatedBy(Projectile.velocity.ToRotation() + PiOver2), col, new Vector2(0, 0)));
                    vertices3.Add(Helper.AsVertex(pos + new Vector2(4, 0).RotatedBy(Projectile.velocity.ToRotation() - PiOver2), col, new Vector2(1, 1)));
                }

                if (vertices.Count > 2 && vertices2.Count > 2 && vertices3.Count > 2)
                {
                    Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, Helper.GetExtraTexture("Tentacle"), false);
                    Helper.DrawTexturedPrimitives(vertices2.ToArray(), PrimitiveType.TriangleStrip, Helper.GetExtraTexture("Tentacle"), false);
                    Helper.DrawTexturedPrimitives(vertices3.ToArray(), PrimitiveType.TriangleStrip, Helper.GetExtraTexture("Tentacle"), false);
                }
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        public override void AI()
        {
            float progress = Utils.GetLerpValue(0, 40, Projectile.timeLeft);
            Projectile.ai[1] = MathHelper.Clamp(MathF.Sin(progress * MathF.PI) * 0.5f, 0, 1);
            Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], 1, 0.1f);

            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[0] == 0)
                Projectile.ai[0] = 1300;
        }
    }
}
