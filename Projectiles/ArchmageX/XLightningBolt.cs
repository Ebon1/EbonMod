using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using static System.Net.Mime.MediaTypeNames;
using EbonianMod.Dusts;

namespace EbonianMod.Projectiles.ArchmageX
{
    public class XLightningBolt : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/Empty";
        int MAX_TIME = 40;
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = MAX_TIME;
            Projectile.hide = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindNPCs.Add(index);
        public override void OnSpawn(IEntitySource source)
        {
            end = Projectile.Center;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!RunOnce || points.Count < 2) return false;
            float a = 0f;
            bool ye = false;
            for (int i = 1; i < points.Count; i++)
            {
                ye = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), points[i], points[i - 1], Projectile.width, ref a);
                if (ye) break;
            }
            return ye;
        }
        bool RunOnce;
        List<Vector2> points = new List<Vector2>();
        Vector2 end;
        public override void AI() //growing laser, originates from fixed point
        {
            Projectile.direction = end.X > Projectile.Center.X ? 1 : -1;
            Projectile.rotation = Projectile.velocity.ToRotation();

            float progress = Utils.GetLerpValue(0, MAX_TIME, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 2, 0, 1);

            int n;

            Vector2 start = Projectile.Center;
            Projectile.ai[2] = MathHelper.Min(Projectile.ai[2] + 1f, 20);
            end = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Helper.TRay.CastLength(Projectile.Center, Projectile.rotation.ToRotationVector2(), 2000) + 32);

            if (!RunOnce)
            {
                if (Projectile.ai[1] == 0)
                    SoundEngine.PlaySound(EbonianSounds.xSpirit.WithPitchOffset(-0.5f), Projectile.Center);
                n = 20;
                points.Clear();
                //Vector2 start = Projectile.Center + Helper.FromAToB(player.Center, Main.MouseWorld) * 40;
                Vector2 dir = (end - start).RotatedBy(MathHelper.PiOver2);
                dir.Normalize();
                float x = Main.rand.NextFloat(1, 7);
                float last = 0;
                for (int i = 0; i < n; i++)
                {
                    if (i == n - 1)
                        x = 0;
                    float a = Main.rand.NextFloat(-x, x).Safe();
                    if (last < 0 && a > 0)
                        a = Main.rand.NextFloat(0.1f, x).Safe();
                    else if (last > 0 && a < 0)
                        a = Main.rand.NextFloat(-x, -0.1f).Safe();
                    if (i < 3)
                        a = 0;
                    Vector2 point = Vector2.SmoothStep(start, end, i / (float)n) + dir * a;
                    points.Add(point);
                    //Dust.NewDustPerfect(point, DustType<XGoopDustDark>(), Helper.FromAToB(i == 0 ? Projectile.Center : points[i - 1], point) * 4, 0, default, 0.35f);
                    Dust.NewDustPerfect(point, DustType<XGoopDust>(), Helper.FromAToB(i == 0 ? Projectile.Center : points[i - 1], point) * 4, 0, Color.White * 0.7f, 0.25f).customData = 1;
                    x -= i / (float)n;
                    last = a;
                }
                RunOnce = true;
            }
            else if (points.Count > 2)
            {
                Projectile.ai[0]++;

                if (Projectile.ai[0] % 3 == 0)
                {
                    float s = 1;
                    for (int i = 0; i < points.Count; i++)
                    {
                        if (i > 1)
                        {
                            for (float j = 0; j < 2; j++)
                            {
                                Vector2 pos = Vector2.Lerp(i == 0 ? Projectile.Center : points[i - 1], points[i], j / 15f);
                                if (Main.rand.NextBool())
                                {
                                    float velF = Main.rand.NextFloat(1, 5);
                                    //Dust.NewDustPerfect(pos, DustType<XGoopDustDark>(), Helper.FromAToB(pos, points[i]) * velF, 0, default, 0.5f * s);
                                    //Dust.NewDustPerfect(pos, DustType<XGoopDust>(), Helper.FromAToB(pos, points[i]).RotatedByRandom(MathHelper.PiOver4) * velF, 0, Color.White * 0.7f, 0.4f * s).customData = 1;
                                }
                                //if (Main.rand.NextBool(4) && j % 6 == 0 && Projectile.ai[0] < 7)
                                //       Dust.NewDustPerfect(pos, DustType<SparkleDust>(), Main.rand.NextVector2Unit(), 0, Color.Indigo * s, Main.rand.NextFloat(0.1f, 0.15f) * s);
                            }
                        }
                        s -= i / (float)points.Count * 0.01f;
                    }


                    SoundStyle sound = SoundID.DD2_LightningAuraZap;
                    sound.Volume = 0.5f;
                    if (Projectile.ai[1] == 0)
                        SoundEngine.PlaySound(sound, Projectile.Center);
                }
            }
            points[0] = Projectile.Center;
            points[points.Count - 1] = end;

        }
        float animationOffset;
        public override bool PreDraw(ref Color lightColor)
        {
            if (!RunOnce || points.Count < 2) return false;

            Texture2D tex = ExtraTextures2.spark_05;
            float s = 0f;
            List<VertexPositionColorTexture> vertices = new();
            List<VertexPositionColorTexture> vertices2 = new();

            animationOffset -= 0.05f;
            if (animationOffset <= 0)
                animationOffset = 1;
            animationOffset = MathHelper.Clamp(animationOffset, float.Epsilon, 1 - float.Epsilon);
            if (points.Count > 1)
            {
                for (int i = 1; i < points.Count; i++)
                {
                    if (i < points.Count / 2)
                        s = MathHelper.SmoothStep(0, 1, (float)(i) / (points.Count / 2));
                    else
                        s = MathHelper.SmoothStep(1, 0, (float)(i - (points.Count / 2)) / (points.Count / 2));

                    float alpha = Projectile.scale * s;

                    Vector2 start = points[i] - Main.screenPosition;
                    Vector2 end = points[i - 1] - Main.screenPosition;
                    float rot = Helper.FromAToB(start, end).ToRotation();
                    if (points.Count < 5)
                        rot = Projectile.velocity.ToRotation();
                    float y = MathHelper.Lerp(-5, 0, s);

                    float _off = animationOffset;
                    if (_off > 1) _off = -_off + 1;
                    float off = _off + (float)(i - 1) / points.Count;
                    for (float j = 0; j < 5; j++)
                    {
                        vertices.Add(Helper.AsVertex(start + new Vector2(2 + s * Projectile.scale * 80, 0).RotatedBy(rot + MathHelper.PiOver2), Color.Indigo * alpha * 1.5f, new Vector2(off, 0)));
                        vertices.Add(Helper.AsVertex(start + new Vector2(2 + s * Projectile.scale * 80, 0).RotatedBy(rot - MathHelper.PiOver2), Color.Indigo * alpha * 1.5f, new Vector2(off, 1)));
                    }
                }
            }
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            if (vertices.Count >= 3)
            {
                Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, tex, false);

                //Helper.DrawTexturedPrimitives(vertices2.ToArray(), PrimitiveType.TriangleStrip, tex, false);
            }

            Main.spriteBatch.ApplySaved();
            return false;
        }
    }
}
