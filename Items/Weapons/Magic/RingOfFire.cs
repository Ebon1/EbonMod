using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;
using EbonianMod.Items.Weapons.Melee;
using Terraria.GameContent;
using Terraria.Audio;
using EbonianMod.Projectiles.Friendly;

using static Terraria.ModLoader.PlayerDrawLayer;
using System.IO;
using System.Collections.Generic;
using EbonianMod.Common.Systems;

namespace EbonianMod.Items.Weapons.Magic
{
    public class RingOfFire : ModItem
    {
        public override void SetDefaults()
        {
            Item.Size = new Vector2(32);
            Item.damage = 1;
            Item.mana = 10;
            Item.DamageType = DamageClass.Magic;
            Item.useAnimation = 32;
            Item.useTime = 32;
            Item.reuseDelay = 70;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Orange;
            Item.shootSpeed = 15f;
            Item.shoot = ModContent.ProjectileType<RingOfFireP>();
        }
    }
    public class RingOfFireP : ModProjectile
    {
        public override string Texture => "EbonianMod/Items/Weapons/Magic/RingOfFire";
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(32);
            Projectile.DamageType = DamageClass.Magic;
            Projectile.aiStyle = 2;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[2] == 0)
            {
                Projectile.Center += oldVelocity;
                Projectile.damage = 0;
                Projectile.velocity = Vector2.Zero;
                Projectile.ai[2] = 1;
                Projectile.aiStyle = -1;
                if (Projectile.timeLeft > 200)
                    Projectile.timeLeft = 200;
            }
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[2] == 0)
            {
                Projectile.damage = 0;
                Projectile.ai[1] = target.whoAmI;
                Projectile.localAI[1] = Helper.FromAToB(target.Center, Projectile.Center, false).X;
                Projectile.localAI[0] = Helper.FromAToB(target.Center, Projectile.Center, false).Y;
                Projectile.ai[2] = 2;
                if (Projectile.timeLeft > 200)
                    Projectile.timeLeft = 200;
                Projectile.velocity = Vector2.Zero;
                Projectile.aiStyle = -1;
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[1]);
            writer.Write(Projectile.localAI[0]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[1] = reader.ReadSingle();
            Projectile.localAI[0] = reader.ReadSingle();
        }
        public override void Kill(int timeLeft)
        {
            Helper.DustExplosion(Projectile.Center, Vector2.One, 2, Color.Transparent, false, true);
        }
        public override void AI()
        {
            if (Projectile.ai[2] > 0)
            {
                Projectile.ai[0]++;
                Projectile.damage = 0;
                if (Projectile.ai[2] == 2)
                {
                    if (Main.npc[(int)Projectile.ai[1]].active)
                        Projectile.Center = Main.npc[(int)Projectile.ai[1]].Center + new Vector2(Projectile.localAI[1], Projectile.localAI[0]);
                    else
                    {
                        Projectile.damage = 1;
                        Projectile.ai[2] = 0;
                        Projectile.aiStyle = 2;
                    }
                }
                if (Projectile.ai[0] % 15 == 0 && Projectile.ai[0] > 50)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        float angle = Helper.CircleDividedEqually(i, 8) + Projectile.rotation;
                        Vector2 vel = Vector2.One.RotatedBy(angle);
                        Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, vel, Scale: Main.rand.NextFloat(1, 2));
                    }
                }
                if (Projectile.ai[0] % 50 == 0 || Projectile.ai[0] == 1)
                {
                    SoundStyle sound = SoundID.Item34;
                    sound.Volume = 2;
                    SoundEngine.PlaySound(sound, Projectile.Center);
                    for (int i = 0; i < 8; i++)
                    {
                        float angle = Helper.CircleDividedEqually(i, 8) + Projectile.rotation;
                        Vector2 vel = Vector2.One.RotatedBy(angle);
                        Projectile a = Projectile.NewProjectileDirect(Terraria.Entity.InheritSource(Projectile), Projectile.Center, vel, ModContent.ProjectileType<RingOfFireP2>(), 5, Projectile.knockBack, Projectile.owner);
                        a.friendly = true;
                        a.hostile = false;
                        a.localAI[0] = 100;
                        a.localAI[1] = 2;
                        a.ai[2] = Projectile.whoAmI;
                        a.scale = 0.25f;
                        a.ai[1] = 0.25f;
                        a.ArmorPenetration = 9999;
                    }
                }
            }
        }
    }
    public class RingOfFireP2 : ModProjectile
    {
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.DD2BetsyFlameBreath;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetRect)
        {
            float collisionPoint17 = 0f;
            float num22 = Projectile.ai[0] / 25f;
            if (num22 > 1f)
            {
                num22 = 1f;
            }
            float num23 = (Projectile.ai[0] - 38f) / 40f;
            if (num23 < 0f)
            {
                num23 = 0f;
            }
            Vector2 lineStart = Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.localAI[0] * num23;
            Vector2 lineEnd = Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.localAI[0] * num22;
            if (Collision.CheckAABBvLineCollision(targetRect.TopLeft(), targetRect.Size(), lineStart, lineEnd, 40f * Projectile.scale, ref collisionPoint17))
            {
                return true;
            }
            return false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hit.Crit = false;
            target.AddBuff(BuffID.OnFire, 50);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile proj = Projectile;
            Vector2 center2 = proj.Center;
            center2 -= Main.screenPosition;
            float num215 = 40f;
            float num216 = num215 * 2f;
            float num217 = (float)proj.frameCounter / num215;
            Texture2D value40 = TextureAssets.Projectile[proj.type].Value;
            Microsoft.Xna.Framework.Color transparent = Microsoft.Xna.Framework.Color.Transparent;
            Microsoft.Xna.Framework.Color color60 = new Microsoft.Xna.Framework.Color(255, 255, 255, 0);
            Microsoft.Xna.Framework.Color color61 = new Microsoft.Xna.Framework.Color(180, 30, 30, 200);
            Microsoft.Xna.Framework.Color color62 = new Microsoft.Xna.Framework.Color(0, 0, 0, 30);
            ulong seed = 1uL;
            for (float num218 = 0f; num218 < 15f; num218 += 1f)
            {
                float num219 = Utils.RandomFloat(ref seed) * 0.25f - 0.125f;
                Vector2 vector49 = (proj.rotation + (Projectile.scale < 1 ? 0 : num219)).ToRotationVector2();
                Vector2 value41 = center2 + vector49 * Projectile.localAI[0];
                float num220 = num217 + num218 * (1f / 15f);
                int num221 = (int)(num220 / (1f / 15f));
                num220 %= 1f;
                if ((!(num220 > num217 % 1f) || !((float)proj.frameCounter < num215)) && (!(num220 < num217 % 1f) || !((float)proj.frameCounter >= num216 - num215)))
                {
                    transparent = ((num220 < 0.1f) ? Microsoft.Xna.Framework.Color.Lerp(Microsoft.Xna.Framework.Color.Transparent, color60, Utils.GetLerpValue(0f, 0.1f, num220, clamped: true)) : ((num220 < 0.35f) ? color60 : ((num220 < 0.7f) ? Microsoft.Xna.Framework.Color.Lerp(color60, color61, Utils.GetLerpValue(0.35f, 0.7f, num220, clamped: true)) : ((num220 < 0.9f) ? Microsoft.Xna.Framework.Color.Lerp(color61, color62, Utils.GetLerpValue(0.7f, 0.9f, num220, clamped: true)) : ((!(num220 < 1f)) ? Microsoft.Xna.Framework.Color.Transparent : Microsoft.Xna.Framework.Color.Lerp(color62, Microsoft.Xna.Framework.Color.Transparent, Utils.GetLerpValue(0.9f, 1f, num220, clamped: true)))))));
                    float num222 = 0.9f + num220 * 0.8f;
                    num222 *= num222;
                    num222 *= 0.8f;
                    Vector2 position7 = Vector2.SmoothStep(center2, value41, num220);
                    Microsoft.Xna.Framework.Rectangle rectangle8 = value40.Frame(1, 7, 0, (int)(num220 * 7f));
                    Main.EntitySpriteDraw(value40, position7, rectangle8, transparent, proj.rotation + (float)Math.PI * 2f * (num220 + Main.GlobalTimeWrappedHourly * 1.2f) * 0.2f + (float)num221 * ((float)Math.PI * 2f / 5f), rectangle8.Size() / 2f, num222 * Projectile.scale, SpriteEffects.None, 0);
                }
            }
            return false;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.hide = true;
            Projectile.localNPCHitCooldown = 5;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            proj = Main.projectile[(int)Projectile.ai[2]];
            if (Projectile.localAI[1] == 0)
                Projectile.localAI[1] = 1;
            if (Projectile.localAI[0] == 0)
                Projectile.localAI[0] = 400f;
            if (Projectile.ai[1] == 0)
                Projectile.ai[1] = 1;
        }
        Projectile proj = null;
        public override void AI()
        {
            Projectile.CritChance = 0;
            proj = Main.projectile[(int)Projectile.ai[2]];
            if (proj != null)
                if (proj.active && proj.type == ModContent.ProjectileType<RingOfFireP>() && proj.whoAmI == Projectile.ai[2])
                    Projectile.Center = Main.projectile[(int)Projectile.ai[2]].Center;
            if (Projectile.ai[1] != 0)
                Projectile.scale = Projectile.ai[1];
            float num = -8f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            DelegateMethods.v3_1 = new Vector3(1.2f, 1f, 0.3f);
            float num2 = Projectile.ai[0] / 40f;
            if (num2 > 1f)
            {
                num2 = 1f;
            }
            float num3 = (Projectile.ai[0] - 38f) / 40f;
            if (num3 < 0f)
            {
                num3 = 0f;
            }
            Utils.PlotTileLine(Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.localAI[0] * num3, Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.localAI[0] * num2, 16f, DelegateMethods.CastLight);
            Utils.PlotTileLine(Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(0.19634954631328583) * Projectile.localAI[0] * num3, Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(0.19634954631328583) * Projectile.localAI[0] * num2, 16f, DelegateMethods.CastLight);
            Utils.PlotTileLine(Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(-0.19634954631328583) * Projectile.localAI[0] * num3, Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(-0.19634954631328583) * Projectile.localAI[0] * num2, 16f, DelegateMethods.CastLight);
            if (num3 == 0f && num2 > 0.1f && Projectile.ai[0] > 300)
            {
                for (int i = 0; i < 3; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 6);
                    dust.fadeIn = 1.5f;
                    dust.velocity = Projectile.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloatDirection() * ((float)Math.PI / 12f)) * (0.5f + Main.rand.NextFloat() * 2.5f) * 15f * Projectile.scale;
                    dust.noLight = true;
                    dust.noGravity = true;
                    dust.alpha = 200;
                }
            }
            Projectile.frameCounter += (int)Projectile.localAI[1];
            Projectile.ai[0] += Projectile.localAI[1];
            if (Projectile.ai[0] >= 78f)
            {
                Projectile.Kill();
            }
        }
    }
}
