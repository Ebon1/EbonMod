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

namespace EbonianMod.Projectiles.Exol
{
    public class IgnosLightning : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/Empty";
        int MAX_TIME = 120;
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = MAX_TIME;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }
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

            float progress = Utils.GetLerpValue(0, MAX_TIME, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 3, 0, 1);

            if (Projectile.ai[1] > 0)
                Projectile.ai[1] -= 0.1f;
            int n;

            Vector2 start = Projectile.Center;
            Projectile.ai[2] = MathHelper.Min(Projectile.ai[2] + 1f, 20);
            end += Projectile.velocity * Projectile.ai[2];
            if (!RunOnce)
            {
                n = 3;
                end = Projectile.Center + Projectile.velocity * 20;
                Vector2 dir = (end - start).RotatedBy(MathHelper.PiOver2);
                dir.Normalize();
                float x = Main.rand.NextFloat(30, 40);
                for (int i = 0; i < n; i++)
                {
                    if (i == n - 1)
                        x = 0;
                    Vector2 point = Vector2.SmoothStep(start, end, i / (float)n) + dir * Main.rand.NextFloat(-x, x).Safe(); //x being maximum magnitude
                    points.Add(point);
                    x -= i / (float)n;
                }
                SoundEngine.PlaySound(SoundID.Item72, Projectile.Center);

                RunOnce = true;
            }
            else if (points.Count > 2)
            {
                Projectile.direction = end.X > Projectile.Center.X ? 1 : -1;
                Projectile.rotation = Projectile.velocity.ToRotation();

                n = Math.Clamp((int)(Projectile.Distance(end) * 0.035f), 3, 100);

                Vector2 dirr = (end - start).RotatedBy(MathHelper.PiOver2);
                dirr.Normalize();
                for (int i = 0; i < points.Count; i++)
                {
                    points[i] = Vector2.SmoothStep(points[i], Vector2.SmoothStep(start, end, i / (float)n), 0.35f);
                }
                Projectile.ai[0]++;

                if (Projectile.ai[0] % 3 == 0)
                {
                    SoundStyle s = SoundID.DD2_LightningAuraZap;
                    s.Volume = 0.5f;
                    SoundEngine.PlaySound(s, Projectile.Center);
                    points.Clear();
                    //Vector2 start = Projectile.Center + Helper.FromAToB(player.Center, Main.MouseWorld) * 40;
                    Vector2 dir = (end - start).RotatedBy(MathHelper.PiOver2);
                    dir.Normalize();
                    float x = Main.rand.NextFloat(30, 40);
                    for (int i = 0; i < n; i++)
                    {
                        if (i == n - 1)
                            x = 0;
                        float a = Main.rand.NextFloat(-x, x).Safe();
                        if (i < 3)
                            a = 0;
                        Vector2 point = Vector2.SmoothStep(start, end, i / (float)n) + dir * a; //x being maximum magnitude
                        points.Add(point);
                        x -= i / (float)n;
                    }
                }


                points[0] = Projectile.Center;
                points[points.Count - 1] = end;

            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!RunOnce || points.Count < 2) return false;
            Main.spriteBatch.Reload(SpriteSortMode.Immediate);

            float scale = Projectile.scale * 2;
            Texture2D bolt = Helper.GetExtraTexture("laser2");
            Main.spriteBatch.Reload(BlendState.Additive);
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

                    Color color = Color.OrangeRed * s * Projectile.scale;

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

                    s -= i / (float)points.Count * 0.03f;
                }
                Helper.DrawTexturedPrimitives(vertices, PrimitiveType.TriangleList, bolt);
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
    }
}
