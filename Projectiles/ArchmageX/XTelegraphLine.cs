using EbonianMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Projectiles.ArchmageX
{
    public class XTelegraphLine : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(5, 5);
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 40;
        }
        float vfxOffset;
        public override bool PreDraw(ref Color lightColor)
        {
            vfxOffset -= 0.07f;
            if (vfxOffset <= 0)
                vfxOffset = 1;
            vfxOffset = MathHelper.Clamp(vfxOffset, float.Epsilon, 1 - float.Epsilon);
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
                var fadeMult = Helper.Safe(1f / Projectile.ai[0]);
                float s = 0;
                for (float i = 0; i < Projectile.ai[0]; i++)
                {
                    float mult = (1f - fadeMult * i);
                    if (mult < 0.5f)
                        s = MathHelper.Clamp(mult * 3.5f, 0, 0.5f) * 3;
                    else
                        s = MathHelper.Clamp((-mult + 1) * 2, 0, 0.5f) * 3;
                    float x = MathHelper.Clamp(MathHelper.SmoothStep(1, 0, (i / Projectile.ai[0]) * 5), 0, 1);
                    float f = MathHelper.Lerp(30, 0, x);
                    float alpha = MathHelper.Lerp(1, 0, x);
                    float _off = 1 - vfxOffset;
                    if (_off > 1) _off = -_off + 1;
                    float off = (_off + mult) % 1f;
                    Color col = Color.Lerp(Color.Indigo, Color.Magenta, i / Projectile.ai[0]) * (Projectile.ai[1] * alpha * Lerp(2, 0, i / Projectile.ai[0]));

                    Vector2 pos = Vector2.Lerp(start, end, (float)i / Projectile.ai[0]) - Main.screenPosition;
                    vertices.Add(Helper.AsVertex(pos + new Vector2(10 * s, 0).RotatedBy(Projectile.velocity.ToRotation() + PiOver2), col, new Vector2(off, 1)));
                    vertices.Add(Helper.AsVertex(pos + new Vector2(10 * s, 0).RotatedBy(Projectile.velocity.ToRotation() - PiOver2), col, new Vector2(off, 0)));


                    _off = vfxOffset;
                    if (_off > 1) _off = -_off + 1;
                    off = (_off + mult) % 1f;

                    col = Color.Lerp(Color.Indigo, Color.Magenta, i / Projectile.ai[0]) * (eAlpha * alpha * Lerp(2, 0, i / Projectile.ai[0]));

                    pos = Vector2.Lerp(start, end, (float)i / Projectile.ai[0]) + Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2) * (f * Projectile.ai[2]) - Main.screenPosition;
                    vertices2.Add(Helper.AsVertex(pos + new Vector2(10 * s, 0).RotatedBy(Projectile.velocity.ToRotation() + PiOver2), col, new Vector2(off, 1)));
                    vertices2.Add(Helper.AsVertex(pos + new Vector2(10 * s, 0).RotatedBy(Projectile.velocity.ToRotation() - PiOver2), col, new Vector2(off, 0)));

                    pos = Vector2.Lerp(start, end, (float)i / Projectile.ai[0]) + Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2) * (f * -Projectile.ai[2]) - Main.screenPosition;
                    vertices3.Add(Helper.AsVertex(pos + new Vector2(10 * s, 0).RotatedBy(Projectile.velocity.ToRotation() + PiOver2), col, new Vector2(off, 1)));
                    vertices3.Add(Helper.AsVertex(pos + new Vector2(10 * s, 0).RotatedBy(Projectile.velocity.ToRotation() - PiOver2), col, new Vector2(off, 0)));
                }

                if (vertices.Count > 2 && vertices2.Count > 2 && vertices3.Count > 2)
                {
                    Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, Helper.GetExtraTexture("LintyTrail"), false);
                    Helper.DrawTexturedPrimitives(vertices2.ToArray(), PrimitiveType.TriangleStrip, Helper.GetExtraTexture("LintyTrail"), false);
                    Helper.DrawTexturedPrimitives(vertices3.ToArray(), PrimitiveType.TriangleStrip, Helper.GetExtraTexture("LintyTrail"), false);
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
                Projectile.ai[0] = Helper.TRay.CastLength(Projectile.Center, Projectile.rotation.ToRotationVector2(), 2000);
        }
    }
}
