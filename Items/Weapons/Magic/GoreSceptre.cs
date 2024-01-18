using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using EbonianMod.Projectiles.VFXProjectiles;
using EbonianMod.Common.Systems;

namespace EbonianMod.Items.Weapons.Magic
{
    public class GoreSceptre : ModItem
    {
        public override void SetStaticDefaults()
        {

            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 1;
            Item.width = 40;
            Item.height = 40;
            Item.mana = 3;
            Item.useTime = 2;
            Item.DamageType = DamageClass.Magic;
            Item.useAnimation = 2;
            Item.useStyle = 5;
            Item.knockBack = 10;
            Item.value = 1000;
            Item.rare = 2;
            Item.UseSound = SoundID.Item8;
            Item.channel = true;
            Item.noMelee = true;
            Item.autoReuse = false;
            Item.useTurn = false;
            Item.shoot = ModContent.ProjectileType<GoreBeam>();
            Item.shootSpeed = 1;
        }
        //int uses = -2;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            {
                Projectile.NewProjectile(source, position + velocity * 14, velocity, ModContent.ProjectileType<GoreBeam>(), damage, knockback, player.whoAmI);
                return false;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.CrimtaneBar, 35).AddTile(TileID.Anvils).Register();
        }
    }
    public class GoreBeam : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/Empty";
        int MAX_TIME = 40;
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = MAX_TIME;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers hit)
        {
            Rectangle rec = new Rectangle((int)end.X - 25, (int)end.Y - 25, 50, 50);
            if (target.getRect().Intersects(rec))
                hit.SetCrit();
            else
                hit.DisableCrit();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Rectangle rec = new Rectangle((int)end.X - 25, (int)end.Y - 25, 50, 50);
            if (target.getRect().Intersects(rec))
            {

            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            end = Main.player[Projectile.owner].Center;
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
        public override void AI()
        {
            if (Projectile.ai[1] > 0)
                Projectile.ai[1] -= 0.1f;
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems || !player.channel || !player.CheckMana(Projectile.ai[2] >= 20 ? 1 : (int)(20 - (Projectile.ai[2] / 2))))
            {
                Projectile.Kill();
                return;
            }
            int n = 25;

            Vector2 start = player.Center + Helper.FromAToB(player.Center, Main.MouseWorld) * 40;
            if (!RunOnce)
            {
                n = 5;
                end = player.Center;
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
                SoundEngine.PlaySound(SoundID.Item72, player.Center);

                RunOnce = true;
            }
            else if (points.Count > 2)
            {
                Projectile.direction = end.X > Projectile.Center.X ? 1 : -1;
                player.ChangeDir(Projectile.direction);
                Projectile.timeLeft = 2;
                player.itemTime = 2;
                player.itemAnimation = 2;
                player.itemRotation = Helper.FromAToB(player.Center, Main.MouseWorld).ToRotation() + (player.direction == -1 ? MathHelper.Pi : 0);
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Helper.FromAToB(player.Center, Main.MouseWorld).ToRotation() - MathHelper.PiOver2);
                Projectile.velocity = Helper.FromAToB(player.Center, Main.MouseWorld);
                Projectile.rotation = Projectile.velocity.ToRotation();

                if (player.Distance(end) < 200)
                    n = 7;
                else if (player.Distance(end) < 300)
                    n = 10;
                else if (player.Distance(end) < 450)
                    n = 15;
                else if (player.Distance(end) < 700)
                    n = 20;

                Vector2 dirr = (end - start).RotatedBy(MathHelper.PiOver2);
                dirr.Normalize();
                for (int i = 0; i < points.Count; i++)
                {
                    points[i] = Vector2.SmoothStep(points[i], Vector2.SmoothStep(start, end, i / (float)n), 0.35f);
                }
                Projectile.ai[0]++;


                if (Projectile.ai[2] >= 20)
                {
                    player.CheckMana(1, true);
                }

                if (Projectile.ai[0] > 90 - Projectile.ai[2] * 3 && Projectile.ai[2] < 20)
                {
                    Projectile.ai[1] = 1f;

                    player.CheckMana((int)(20 - (Projectile.ai[2] / 2)), true);
                    SoundEngine.PlaySound(EbonianSounds.heartbeat, end);
                    Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), end, Vector2.Zero, ModContent.ProjectileType<BloodShockwave2>(), 0, 0, Projectile.owner);
                    Projectile.damage++;
                    Projectile.ai[2]++;
                    Projectile.ai[0] = 0;
                }

                if (Projectile.ai[0] % 3 == 0)
                {
                    SoundStyle s = SoundID.DD2_LightningAuraZap;
                    s.Volume = 0.5f;
                    SoundEngine.PlaySound(s, player.Center);
                    points.Clear();
                    //Vector2 start = Projectile.Center + Helper.FromAToB(player.Center, Main.MouseWorld) * 40;
                    Vector2 dir = (end - start).RotatedBy(MathHelper.PiOver2);
                    dir.Normalize();
                    float x = Main.rand.NextFloat(30, 40) - Projectile.damage;
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


                points[0] = player.Center + Helper.FromAToB(player.Center, Main.MouseWorld) * 40 - new Vector2(0, 3).RotatedBy(dirr.ToRotation() + MathHelper.Pi / 2);
                float range = (Projectile.ai[2] + 2) * 96;
                Vector2 offset = Helper.FromAToB(player.Center, Main.MouseWorld, false);
                if (offset.Length() > range)
                {
                    offset.Normalize();
                    offset *= range;
                }
                end = Vector2.Lerp(end, player.Center + offset, 0.2f);
                points[points.Count - 1] = end;
                Projectile.Center = player.Center;

            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!RunOnce || points.Count < 2) return false;
            Main.spriteBatch.Reload(SpriteSortMode.Immediate);

            float mult = 0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly/* * 2*/) * 0.1f;
            float scale = Projectile.scale * 2;
            Texture2D texture = ModContent.Request<Texture2D>("EbonianMod/Extras/explosion").Value;
            Texture2D bolt = Helper.GetExtraTexture("laser2");
            Texture2D boltTransparent = Helper.GetExtraTexture("laser5");
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

                    Color color = Color.Maroon * s;

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
            //for (int i = 0; i < 5; i++)
            //   Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.Maroon, 0, new Vector2(texture.Width, texture.Height) / 2, scale * 0.2f, SpriteEffects.None, 0f);
            //for (int i = 0; i < 5; i++)
            texture = Helper.GetTexture("Projectiles/Friendly/Crimson/HeadGoreSceptre_Extra");
            Main.spriteBatch.Draw(texture, end - Main.screenPosition, null, Color.Maroon * (Projectile.ai[2] < 20 ? Projectile.ai[1] * 2 : 1), 0, new Vector2(texture.Width, texture.Height) / 2, 1 + Projectile.ai[1], SpriteEffects.None, 0f);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            texture = Helper.GetTexture("Projectiles/Friendly/Crimson/HeadGoreSceptre");
            Main.spriteBatch.Draw(texture, end - Main.screenPosition, null, Color.White, 0, new Vector2(texture.Width, texture.Height) / 2, 1 + Projectile.ai[1], SpriteEffects.None, 0f);
            return false;
        }
    }
}