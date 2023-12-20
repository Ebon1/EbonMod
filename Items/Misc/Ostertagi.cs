using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using EbonianMod.Projectiles.Friendly.Corruption;
using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace EbonianMod.Items.Misc
{
    public class Ostertagi : ModItem
    {
        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.width = Item.height = 80;
            Item.crit = 30;
            Item.damage = 480;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.noUseGraphic = true;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.channel = true;
            Item.DamageType = DamageClass.Melee;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = 5;
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<OstertagiP>();
        }
        public override bool? CanAutoReuseItem(Player player)
        {
            return false;
        }
    }
    public class OstertagiP : ModProjectile
    {
        public override string Texture => "EbonianMod/Items/Misc/Ostertagi";
        int swingTime = 15;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = swingTime + 45;
        }
        public override bool PreDraw(ref Color lightColor) => false;
        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems)
                return;
            Player.CompositeArmStretchAmount stretch = Player.CompositeArmStretchAmount.None;
            player.itemTime = 2;
            player.itemAnimation = 2;
            Projectile.velocity = -Vector2.UnitY;
            if (Projectile.timeLeft > 27)
            {
                stretch = Player.CompositeArmStretchAmount.Quarter;
                Projectile.rotation = Projectile.velocity.RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly * 5) * 0.5f).ToRotation() * -player.direction;
            }
            else
            {
                stretch = Player.CompositeArmStretchAmount.Full;
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, (Projectile.velocity.RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly * 10) * 0.5f).ToRotation() * -player.direction) - (MathHelper.PiOver2 * player.direction), 0.1f);
            }
            player.SetCompositeArmFront(true, stretch, Projectile.rotation + MathHelper.Pi);
            player.SetCompositeArmBack(true, stretch, Projectile.rotation + (MathHelper.PiOver4 / 8) + MathHelper.Pi);
            if (Projectile.timeLeft == 22)
            {
                int dmg = player.statLifeMax2 - 20;
                PlayerDeathReason customReason = new()
                {
                    SourceItem = player.HeldItem,
                    SourceCustomReason = $"{player.name} didn't mind the worms."
                };
                Player.HurtInfo info = new();
                info.Damage = dmg;
                info.DamageSource = customReason;
                player.Hurt(info);
                player.immuneTime = 0;
                SoundEngine.PlaySound(new SoundStyle("EbonianMod/Sounds/NPCHit/fleshHit"), player.Center);

                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<OstertagiExplosion>(), 0, 0, 0);
                for (int i = 0; i < 5; i++)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(3, 7), ModContent.ProjectileType<OstertagiWorm>(), 15, 0, ai2: 0);
                }
                for (int i = 0; i < 30; i++)
                {
                    Dust.NewDustPerfect(player.Center, DustID.CorruptGibs, new Vector2(-player.direction, Main.rand.NextFloat(-1, 0)).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(4.5f, 7));
                }
                player.AddBuff(ModContent.BuffType<OstertagiB>(), 60 * 45);
                player.AddBuff(BuffID.PotionSickness, 60 * 120);
            }
        }
    }
    public class OstertagiB : ModBuff
    {
        public override string Texture => "EbonianMod/Buffs/OstertagiB";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        float val = 30;

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetAttackSpeed(DamageClass.Generic) += 3.65f;
            player.lifeRegen = 0;
            player.lifeRegenTime = 0;
            Vector2 dir = Main.rand.NextVector2Unit();

            if (player.buffTime[buffIndex] > 60 * 30)
            {
                if (player.buffTime[buffIndex] % 30 == 0)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, dir * Main.rand.NextFloat(1, 5), ModContent.ProjectileType<OstertagiWorm>(), 5, 0);

                    EbonianSystem.ScreenShakeAmount = 2;

                    SoundEngine.PlaySound(new SoundStyle("EbonianMod/Sounds/NPCHit/fleshHit") with { PitchVariance = 0.3f, Volume = 0.3f }, player.Center);

                    for (int k = 0; k < 15; k++)
                    {
                        Dust.NewDustPerfect(player.MountedCenter, DustID.Blood, dir.RotatedByRandom(1) * Main.rand.NextFloat(1, 5), 0, default, Main.rand.NextFloat(1, 2));
                    }
                }
            }
            else if (player.buffTime[buffIndex] > 60 * 10)
            {
                if (player.buffTime[buffIndex] % 20 == 0)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, dir * Main.rand.NextFloat(1, 5), ModContent.ProjectileType<OstertagiWorm>(), 5, 0);

                    EbonianSystem.ScreenShakeAmount = 2;

                    SoundEngine.PlaySound(new SoundStyle("EbonianMod/Sounds/NPCHit/fleshHit") with { PitchVariance = 0.3f, Volume = 0.3f }, player.Center);

                    for (int k = 0; k < 15; k++)
                    {
                        Dust.NewDustPerfect(player.MountedCenter, DustID.Blood, dir.RotatedByRandom(1) * Main.rand.NextFloat(1, 5), 0, default, Main.rand.NextFloat(1, 2));
                    }
                }
            }
            else if (player.buffTime[buffIndex] < 60 * 10)
            {
                if (player.buffTime[buffIndex] % 10 == 0)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, dir * Main.rand.NextFloat(1, 5), ModContent.ProjectileType<OstertagiWorm>(), 5, 0);

                    EbonianSystem.ScreenShakeAmount = 2;

                    SoundEngine.PlaySound(new SoundStyle("EbonianMod/Sounds/NPCHit/fleshHit") with { PitchVariance = 0.3f, Volume = 0.3f }, player.Center);

                    for (int k = 0; k < 15; k++)
                    {
                        Dust.NewDustPerfect(player.MountedCenter, DustID.Blood, dir.RotatedByRandom(1) * Main.rand.NextFloat(1, 5), 0, default, Main.rand.NextFloat(1, 2));
                    }
                }
            }
        }
    }

    public class OstertagiExplosion : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/Fire";

        public override bool ShouldUpdatePosition() => false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.Size = new Vector2(98);
            Projectile.scale = 0.2f;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            EbonianSystem.ScreenShakeAmount = 5;

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);

            Projectile.rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);

            for (int k = 0; k < 20; k++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 15), 0, default, Main.rand.NextFloat(1, 3)).noGravity = true;
                Dust.NewDustPerfect(Projectile.Center, DustID.Granite, Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 15), 100, default, Main.rand.NextFloat(1, 2)).noGravity = true;
            }
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Cursed);

            Projectile.scale += 0.1f;

            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }
            else
            {
                Projectile.alpha += 10;
            }

            if (Projectile.frameCounter++ >= 3 && Projectile.frame <= Main.projFrames[Type])
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
        }

        float magicRotation;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color color = new Color(123, 255, 0, 0) * Projectile.Opacity;

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            texture = Helper.GetTexture("Extras/vortex");

            sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            origin = sourceRectangle.Size() / 2f;
            color = new Color(123, 255, 0, 0) * Projectile.Opacity;

            magicRotation += 0.1f;
            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation + magicRotation, origin, Projectile.scale * 0.5f, SpriteEffects.None, 0);

            return false;
        }
    }
}
