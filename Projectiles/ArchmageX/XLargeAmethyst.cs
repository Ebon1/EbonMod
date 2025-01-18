using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using EbonianMod.Dusts;
using Terraria.Audio;

namespace EbonianMod.Projectiles.ArchmageX
{
    public class XLargeAmethyst : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 90;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 250;
            Projectile.Size = new(28, 28);
        }
        public override bool? CanDamage() => false;
        public override bool PreKill(int timeLeft)
        {
            int i = 0;
            foreach (Vector2 pos in Projectile.oldPos)
            {
                var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
                float mult = (1f - fadeMult * i);
                Dust.NewDustPerfect(pos + Projectile.Size / 2 + (Projectile.Size / 4).RotatedBy((Main.GameUpdateCount + i * 4) * 0.03f), DustType<SparkleDust>(), Main.rand.NextVector2Circular(3, 3), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f) * mult);
                Dust.NewDustPerfect(pos + Projectile.Size / 2 + (Projectile.Size / 4).RotatedBy((Main.GameUpdateCount + i * 4) * 0.03f), DustType<SparkleDust>(), Vector2.Zero, 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.15f) * mult);

                Dust.NewDustPerfect(pos + Projectile.Size / 2 - (Projectile.Size / 4).RotatedBy((Main.GameUpdateCount + i * 4) * 0.03f), DustType<SparkleDust>(), Main.rand.NextVector2Circular(3, 3), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f) * mult);
                Dust.NewDustPerfect(pos + Projectile.Size / 2 - (Projectile.Size / 4).RotatedBy((Main.GameUpdateCount + i * 4) * 0.03f), DustType<SparkleDust>(), Vector2.Zero, 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.15f) * mult);
                i++;
            }
            SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);
            for (int h = 0; h < (Projectile.ai[2] == 0 ? 5 : 3); h++)
            {
                Projectile.NewProjectile(null, Projectile.Center, Main.rand.NextVector2Circular(5, 5), ProjectileType<XAmethystShard>(), Projectile.damage, 0);
            }

            Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0);
            return true;
        }
        float vfxOffset;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Texture2D glow = Helper.GetTexture(Texture + "_Glow");
            Main.spriteBatch.Reload(BlendState.Additive);
            /*var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float mult = (1f - fadeMult * i);
                if (i > 0)
                    for (float j = 0; j < 10; j++)
                    {
                        Vector2 pos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[i - 1], (float)(j / 10));
                        Color col = Color.Lerp(Color.Indigo * 0.5f, Color.Gray, (float)(i / Projectile.oldPos.Length));
                        Main.spriteBatch.Draw(TextureAssets.Projectile[ProjectileType<Gibs>()].Value, pos + Projectile.Size / 2 + (Projectile.Size / 4).RotatedBy((Main.GameUpdateCount + i * 4) * 0.03f) - Main.screenPosition, null, col * (0.35f), 0, TextureAssets.Projectile[ProjectileType<Gibs>()].Value.Size() / 2, 0.025f * mult + (((MathF.Sin(Main.GlobalTimeWrappedHourly * 3) + 1) / 2) * 0.005f), SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(TextureAssets.Projectile[ProjectileType<Gibs>()].Value, pos + Projectile.Size / 2 - (Projectile.Size / 4).RotatedBy((Main.GameUpdateCount - i * 4) * 0.03f) - Main.screenPosition, null, col * (0.35f), 0, TextureAssets.Projectile[ProjectileType<Gibs>()].Value.Size() / 2, 0.025f * mult + (((MathF.Sin(Main.GlobalTimeWrappedHourly * 3) + 1) / 2) * 0.005f), SpriteEffects.None, 0);
                    }
            }*/

            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            vfxOffset -= 0.015f;
            if (vfxOffset <= 0)
                vfxOffset = 1;
            vfxOffset = MathHelper.Clamp(vfxOffset, float.Epsilon, 1 - float.Epsilon);
            List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
            List<VertexPositionColorTexture> vertices2 = new List<VertexPositionColorTexture>();
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
                    Color col = Color.Lerp(Color.Indigo * 0.5f, Color.Gray, (float)(i / Projectile.oldPos.Length)) * 2;

                    float __off = vfxOffset;
                    if (__off > 1) __off = -__off + 1;
                    float _off = __off + mult;
                    vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 + (Projectile.Size / 4).RotatedBy((Main.GameUpdateCount + i * 4) * 0.03f) - Main.screenPosition + new Vector2(20 * mult * s, 0).RotatedBy(Projectile.velocity.ToRotation() - MathHelper.PiOver2), col, new Vector2(_off, 1)));
                    vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 + (Projectile.Size / 4).RotatedBy((Main.GameUpdateCount + i * 4) * 0.03f) - Main.screenPosition + new Vector2(20 * mult * s, 0).RotatedBy(Projectile.velocity.ToRotation() + MathHelper.PiOver2), col, new Vector2(_off, 0)));

                    vertices2.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - (Projectile.Size / 4).RotatedBy((Main.GameUpdateCount - i * 4) * 0.03f) - Main.screenPosition + new Vector2(20 * mult * s, 0).RotatedBy(Projectile.velocity.ToRotation() - MathHelper.PiOver2), col, new Vector2(_off, 1)));
                    vertices2.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - (Projectile.Size / 4).RotatedBy((Main.GameUpdateCount - i * 4) * 0.03f) - Main.screenPosition + new Vector2(20 * mult * s, 0).RotatedBy(Projectile.velocity.ToRotation() + MathHelper.PiOver2), col, new Vector2(_off, 0)));
                }
            }
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (vertices.Count > 2 && vertices2.Count > 2)
            {
                Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, Helper.GetExtraTexture("wavyLaser"), false);
                Helper.DrawTexturedPrimitives(vertices2.ToArray(), PrimitiveType.TriangleStrip, Helper.GetExtraTexture("wavyLaser"), false);
            }
            Main.spriteBatch.ApplySaved();
            for (int i = 0; i < 2; i++)
                Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, glow.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
        Vector2 p;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[0]++;
            float progress = Utils.GetLerpValue(0, 250, Projectile.timeLeft);
            float vel = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 3f, 0, 2);
            if (Projectile.timeLeft > 70)
            {
                if (Projectile.ai[2] == 0)
                {
                    Projectile.velocity = Helper.FromAToB(Projectile.Center, player.Center + Helper.FromAToB(player.Center, Projectile.Center) * 50) * 3 * vel;
                    if (Projectile.Distance(player.Center) < 90)
                        Projectile.timeLeft = 70;
                }
                else
                {
                    Projectile.velocity = Projectile.velocity.RotatedBy(ToRadians(2)) * 1.05f;
                    Projectile.timeLeft -= 2;
                }
            }
            else if (Projectile.timeLeft <= 70 && Projectile.timeLeft > 50)
            {
                Projectile.velocity *= 0.9f;
                p = Projectile.Center;
            }
            if (Projectile.timeLeft <= 50)
            {
                Projectile.ai[1] += 0.1f;
                Projectile.Center = p + Main.rand.NextVector2Circular(Projectile.ai[1], Projectile.ai[1]);
            }
        }
    }
}
