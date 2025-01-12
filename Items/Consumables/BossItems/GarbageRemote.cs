using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using EbonianMod.NPCs.Garbage;
using Microsoft.Xna.Framework;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using System.Collections.Generic;
using System.Linq;
using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using static tModPorter.ProgressUpdate;
using EbonianMod.Common.Systems;
using EbonianMod.Items.Consumables.Food;

namespace EbonianMod.Items.Consumables.BossItems
{
    public class GarbageRemote : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 12;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 1;
            Item.value = 1000000;
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = tooltips.IndexOf(tooltips.FirstOrDefault(x => x.Text == "Be warned."));
            tooltips[index].OverrideColor = Color.Red;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<Potato>(), 5).AddIngredient(RecipeGroupID.IronBar, 20).AddIngredient(ItemID.Glass, 10).Register();
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<HotGarbage>()) && player.ownedProjectileCounts[ModContent.ProjectileType<GarbageRemoteP>()] <= 0;
        }

        public override bool? UseItem(Player player)
        {
            //NPC.NewNPCDirect(player.GetSource_FromThis(), player.Center + new Microsoft.Xna.Framework.Vector2(300, -200), ModContent.NPCType<HotGarbage>());
            Terraria.Audio.SoundEngine.PlaySound(EbonianSounds.garbageSignal.WithVolumeScale(3), player.position);
            Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<GarbageRemoteP>(), 0, 0, player.whoAmI);
            return true;
        }
    }
    public class GarbageRemoteP : ModProjectile
    {
        public override string Texture => "EbonianMod/Items/Consumables/BossItems/GarbageRemote";
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 46;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 430;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture + "_Overlay").Value;
            if (Projectile.timeLeft > 155)
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Projectile.Size / 2, 1, SpriteEffects.None, 0);
        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            EbonianSystem.ScreenShakeAmount = 10;
            Projectile a = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileID.DaybreakExplosion, 50, 0);
            a.hostile = true;
            a.friendly = false;
        }
        List<Vector2> points = new List<Vector2>();
        Vector2 end;
        Vector2 basePos;
        Vector2 pos;
        float rot;
        public override void AI()
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.type == ModContent.NPCType<HotGarbage>())
                {
                    if (npc.Distance(Projectile.Center) < Projectile.Size.Length())
                        Projectile.Kill();
                }
            }
            Player player = Main.player[Projectile.owner];
            if (Projectile.ai[0] == 0)
            {
                Projectile.Center = Helper.TRay.Cast(player.Center + new Vector2(40 * player.direction, -50), Vector2.UnitY, 1000, true) - new Vector2(0, 23);
                basePos = Helper.TRay.Cast(player.Center + new Vector2(40 * player.direction, -50), Vector2.UnitY, 1000, true) - new Vector2(0, 23);
                Projectile.ai[0] = 1;
            }
            else
            {
                if (Projectile.timeLeft > 155)
                {
                    if (Projectile.timeLeft % 5 - (Projectile.timeLeft < 200 ? 2 : 0) == 0)
                    {
                        rot = Main.rand.NextFloat(-0.5f, 0.5f);
                        pos = basePos + Main.rand.NextVector2Circular(5, 5);
                    }
                    if (pos != Vector2.Zero)
                        Projectile.Center = Vector2.Lerp(Projectile.Center, pos, 0.1f);
                    Projectile.rotation = MathHelper.Lerp(Projectile.rotation, rot, 0.5f);
                };
            }
            Projectile.ai[1]++;
            if (Projectile.ai[1] % 5 == 0 && Main.rand.NextBool(4) && Projectile.timeLeft > 155)
            {
                Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<YellowShockwave>(), 0, 0);
                /*for (int i = -1; i < 2; i++)
                {
                    if (i == 0)
                        continue;
                    Projectile.NewProjectile(null, Projectile.Center, -Vector2.UnitY * 5, ModContent.ProjectileType<GarbageSignals>(), 0, 0, player.whoAmI, i * (1 + Projectile.ai[1] * 0.005f));
                }*/
            }
            int n = 15;
            Vector2 start = Projectile.Center;
            if (Projectile.timeLeft == 120)
            {
                NPC.NewNPCDirect(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(0, -Main.screenHeight), ModContent.NPCType<HotGarbage>());
            }
            if (Projectile.timeLeft == 155)
            {
                EbonianSystem.ScreenShakeAmount = 5;
                Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BigGrayShockwave>(), 0, 0);
                end = Projectile.Center + new Vector2(0, -Main.screenHeight);
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
            }
            if (Projectile.timeLeft <= 155)
                if (Main.rand.NextBool(5))
                    Helper.DustExplosion(Projectile.Center, Vector2.One, 2, Color.Gray * 0.45f, false, false, 0.6f, 0.5f, new(Main.rand.NextFloat(-4, 4), -10));
            if (Projectile.timeLeft <= 155 && Projectile.timeLeft > 125)
            {
                Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], 1, 0.1f);
                if (points.Count > 2)
                {

                    Vector2 dirr = (end - start).RotatedBy(MathHelper.PiOver2);
                    dirr.Normalize();
                    for (int i = 0; i < points.Count; i++)
                    {
                        points[i] = Vector2.SmoothStep(points[i], Vector2.SmoothStep(start, end, i / (float)n), 0.35f);
                    }
                    Projectile.ai[0]++;

                    /*if (Projectile.timeLeft < 120)
                    {
                        foreach (NPC npc in Main.npc)
                        {
                            if (npc.active && npc.type == ModContent.NPCType<HotGarbage>())
                            {
                                if (npc.Center.Distance(Projectile.Center) < 3000)
                                {
                                    end = npc.Bottom;
                                    break;
                                }
                            }
                        }
                    }*/

                    if (Projectile.ai[0] % 2 == 0)
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


                    points[0] = start;
                    points[points.Count - 1] = end;
                }
                /*if (Projectile.ai[0] < 1 && Projectile.timeLeft < 400 && Projectile.timeLeft > 200)
                    Projectile.ai[0] += 0.05f;
                else if (Projectile.ai[0] > 0 && Projectile.timeLeft < 200)
                    Projectile.ai[0] -= 0.025f;*/

            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Reload(BlendState.Additive);
            /*Texture2D tex = Helper.GetExtraTexture("laser4");
            Vector2 pos = Projectile.Center;
            for (int i = 0; i < 1080; i++)
            {
                Vector2 scale = new Vector2(1, 2 * Projectile.scale + MathHelper.Clamp((MathF.Sin(i * 0.15f - Main.GlobalTimeWrappedHourly * 6) + 1) * 0.5f, 0, 1f));
                Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, Color.LawnGreen * ((MathF.Sin(Main.GlobalTimeWrappedHourly * 3) + 1.5f) * Projectile.ai[2]), -Vector2.UnitY.ToRotation(), new Vector2(0, tex.Height / 2), scale, SpriteEffects.None, 0);
                pos += -Vector2.UnitY;
            }*/
            Main.spriteBatch.Reload(SpriteSortMode.Immediate);

            float mult = 0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly/* * 2*/) * 0.1f;
            float scale = Projectile.scale * 2;
            Texture2D texture = ModContent.Request<Texture2D>("EbonianMod/Extras/explosion").Value;
            Texture2D bolt = Helper.GetExtraTexture("laser2");
            Texture2D boltTransparent = Helper.GetExtraTexture("laser5");
            Main.spriteBatch.Reload(BlendState.Additive);
            float s = 1;
            if (points.Count > 2 && Projectile.timeLeft <= 155 && Projectile.timeLeft > 125)
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

                    Color color = Color.Orange * s;

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
            return true;
        }
    }
    public class GarbageSignals : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D a = Helper.GetExtraTexture("explosion");
            Main.spriteBatch.Reload(BlendState.Additive);
            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(a, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.LawnGreen * 0.5f * alpha * (1f - fadeMult * i), 0, a.Size() / 2, 0.1f * (1f - fadeMult * i), SpriteEffects.None, 0);
            }
            for (int i = 0; i < 3; i++)
                Main.spriteBatch.Draw(a, Projectile.Center - Main.screenPosition, null, Color.LawnGreen * 0.5f * alpha, 0, a.Size() / 2, 0.1f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
        }
        /*public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.timeLeft > 60)
                Projectile.velocity = -oldVelocity;
            else
                Projectile.velocity = Vector2.Zero;
            return false;
        }*/
        Vector2 initCenter, initVel;
        float alpha = 1;
        public override void AI()
        {
            if (Projectile.timeLeft == 299)
            {
                initCenter = Projectile.Center;
                initVel = Projectile.velocity;
            }
            if (Projectile.timeLeft < 60)
            {
                if (alpha > 0)
                    alpha -= 0.025f;
                //Projectile.velocity *= 0.5f;
                //Projectile.aiStyle = -1;
            }
            if (initCenter != Vector2.Zero)
                Projectile.SineMovement(initCenter, initVel, 0.15f, 60);

        }
    }
}
