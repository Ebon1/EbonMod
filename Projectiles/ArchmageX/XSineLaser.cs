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
using EbonianMod.Common.Systems;

namespace EbonianMod.Projectiles.ArchmageX
{
    public class XSineLaser : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/Empty";
        int MAX_TIME = 80;
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
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
            for (int i = 1; i < MathHelper.Clamp(Projectile.ai[0] * (2 + Projectile.localAI[0] * 2), 2, points.Count - 1); i++)
            {
                ye = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), points[i], points[i - 1], Projectile.width, ref a);
                if (ye) break;
            }
            return ye;
        }
        bool RunOnce;
        List<Vector2> points = new List<Vector2>();
        Vector2 end;
        float a;
        public override void AI() //growing laser, originates from fixed point
        {
            if (Projectile.damage == 0 || Projectile.localAI[0] == 1)
            {
                if (MAX_TIME > 80)
                {
                    Projectile.timeLeft = 80;
                    MAX_TIME = 80;
                }
            }
            Projectile.direction = end.X > Projectile.Center.X ? 1 : -1;
            Projectile.rotation = Projectile.velocity.ToRotation();

            float progress = Utils.GetLerpValue(0, MAX_TIME, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * (Projectile.damage == 0 ? 0.5f : 3), 0, 1);

            int n;

            Vector2 start = Projectile.Center;
            Projectile.ai[2] = MathHelper.Min(Projectile.ai[2] + 1f, 20);
            end = Projectile.Center + Projectile.rotation.ToRotationVector2() * 2000;

            if (Projectile.damage == 0)
            {
                float t = Utils.GetLerpValue(0, MAX_TIME - 20, Projectile.timeLeft - 20);
                if (Projectile.timeLeft > 20)
                    a = MathHelper.SmoothStep(Projectile.ai[1], 0, t);
            }

            if (!RunOnce)
            {
                n = 300;
                points.Clear();
                //Vector2 start = Projectile.Center + Helper.FromAToB(player.Center, Main.MouseWorld) * 40;
                Vector2 dir = (end - start).RotatedBy(MathHelper.PiOver2);
                dir.Normalize();
                float x = 0;
                if (Projectile.damage != 0)
                {
                    a = Projectile.ai[1];
                }
                for (int i = 0; i < n; i++)
                {
                    if (i == n - 1)
                        x = 0;
                    Vector2 point = Vector2.Lerp(start, end, i / (float)n) + dir * x; //x being maximum magnitude

                    points.Add(point);
                    //if (Projectile.damage != 0 && i != 0 && Collision.CanHitLine(Projectile.Center, 1, 1, points[i], 1, 1))
                    //  Dust.NewDustPerfect(point, ModContent.DustType<XGoopDust>(), Helper.FromAToB(i == 0 ? Projectile.Center : points[i - 1], point) * 4, 0, default, 0.25f);

                    x = MathF.Cos(4.7124f + ((float)i / n) * 35) * Projectile.ai[1];
                }
                RunOnce = true;
            }
            else if (points.Count > 2)
            {

                if (Projectile.damage != 0)
                {
                    Projectile.ai[0]++;

                    if (Projectile.ai[0] % 3 == 0)
                    {
                        float s = 1;
                        for (int i = 0; i < MathHelper.Clamp(Projectile.ai[0] * (2 + Projectile.localAI[0] * 2), 2, points.Count - 1); i++)
                        {
                            if (i > 1)
                            {
                                if (!Collision.CanHitLine(Projectile.Center, 1, 1, points[i], 1, 1))
                                    continue;
                                if (Main.rand.NextBool())
                                    Dust.NewDustPerfect(points[i], ModContent.DustType<XGoopDust>(), Helper.FromAToB(i == 0 ? Projectile.Center : points[i - 1], points[i]) * Main.rand.NextFloat(4, 8), 0, default, 0.5f * s);
                                if (Main.rand.NextBool(4) && i % 4 == 0)
                                    Dust.NewDustPerfect(points[i], ModContent.DustType<SparkleDust>(), Helper.FromAToB(i == 0 ? Projectile.Center : points[i - 1], points[i]) * Main.rand.NextFloat(4, 8), 0, Color.Indigo * s, Main.rand.NextFloat(0.1f, 0.15f) * s);
                            }
                            s -= i / (float)points.Count * 0.01f;
                        }
                    }
                }
                else
                {
                    points.Clear();
                    n = 300;
                    Vector2 dir = (end - start).RotatedBy(MathHelper.PiOver2);
                    dir.Normalize();
                    float x = 0;
                    for (int i = 0; i < n; i++)
                    {
                        if (i == n - 1)
                            x = 0;
                        Vector2 point = Vector2.Lerp(start, end, i / (float)n) + dir * x; //x being maximum magnitude
                        points.Add(point);
                        x = MathF.Cos(4.7124f + ((float)i / n) * 35) * a;
                    }
                }
                points[0] = Projectile.Center;
                points[points.Count - 1] = end;

            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!RunOnce || points.Count < 2 || Projectile.damage != 0) return false;
            Main.spriteBatch.Reload(SpriteSortMode.Immediate);

            float scale = Projectile.scale * (Projectile.damage == 0 ? 4 : 8);
            Texture2D tex = Helper.GetExtraTexture("Extras2/star_09");
            Texture2D bolt = Helper.GetExtraTexture("laser_purple");
            if (Projectile.damage == 0)
                bolt = Helper.GetExtraTexture("laser3");
            Main.spriteBatch.Reload(BlendState.Additive);
            if (Projectile.damage != 0)
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Indigo * Projectile.scale, Main.GameUpdateCount * -0.003f, tex.Size() / 2, 0.2f * 2, SpriteEffects.None, 0);
            float s = 1;
            if (points.Count > 2)
            {
                VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[(points.Count - 1) * 6];
                for (int i = 0; i < points.Count - 1; i++)
                {
                    Vector2 start = points[i];
                    Vector2 end = points[i + 1];
                    float num = Vector2.Distance(points[i], points[i + 1]);
                    Vector2 vector = (end - start) / num;
                    Vector2 vector2 = start;
                    float rotation = vector.ToRotation();

                    Color color = Color.Indigo * (s * Projectile.scale);
                    if (Projectile.damage == 0)
                        color = Color.White * (s * Projectile.scale);

                    if (!Collision.CanHitLine(Projectile.Center, 1, 1, points[i], 1, 1))
                        color = Color.Transparent;

                    Vector2 pos1 = points[i] - Main.screenPosition;
                    Vector2 pos2 = points[i + 1] - Main.screenPosition;
                    Vector2 dir1 = Helper.GetRotation(points, i) * 10 * scale * s;
                    Vector2 dir2 = Helper.GetRotation(points, i + 1) * 10 * scale * (s + i / (float)points.Count * 0.03f);
                    Vector2 v1 = pos1 + dir1;
                    Vector2 v2 = pos1 - dir1;
                    Vector2 v3 = pos2 + dir2;
                    Vector2 v4 = pos2 - dir2;
                    float p1 = i / (float)points.Count;
                    float p2 = (i + 1) / (float)points.Count;
                    vertices[i * 6] = Helper.AsVertex(v1, color, new Vector2(p1, 0));
                    vertices[i * 6 + 1] = Helper.AsVertex(v3, color, new Vector2(p2, 0));
                    vertices[i * 6 + 2] = Helper.AsVertex(v4, color, new Vector2(p2, 1));

                    vertices[i * 6 + 3] = Helper.AsVertex(v4, color, new Vector2(p2, 1));
                    vertices[i * 6 + 4] = Helper.AsVertex(v2, color, new Vector2(p1, 1));
                    vertices[i * 6 + 5] = Helper.AsVertex(v1, color, new Vector2(p1, 0));

                    s -= i / (float)points.Count * 0.01f;
                }
                Helper.DrawTexturedPrimitives(vertices, PrimitiveType.TriangleList, bolt);
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
    }
}