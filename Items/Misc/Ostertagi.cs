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
                EbonianSystem.ScreenShakeAmount = 5;
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<CursedFlameExplosion>(), 0, 0, ai2: 0);
                for (int i = 0; i < 5; i++)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(3, 7), ModContent.ProjectileType<OstertagiWorm>(), 15, 0, ai2: 0);
                }
                for (int i = 0; i < 30; i++)
                {
                    Dust.NewDustPerfect(player.Center, DustID.CorruptGibs, new Vector2(-player.direction, Main.rand.NextFloat(-1, 0)).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(4.5f, 7));
                    if (i % 5 == 0)
                        Dust.NewDustPerfect(player.Center, DustID.CursedTorch, new Vector2(player.direction, Main.rand.NextFloat(-1, 0)).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(2, 4));
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
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetAttackSpeed(DamageClass.Generic) += 3.65f;
            player.lifeRegen = 0;
            player.lifeRegenTime = 0;
            Vector2 dir = Main.rand.NextVector2Unit();
            if (player.buffTime[buffIndex] > 60 * 30)
            {
                if (player.buffTime[buffIndex] % 30 == 0)
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, dir * Main.rand.NextFloat(1, 5), ModContent.ProjectileType<OstertagiWorm>(), 5, 0);
            }
            else if (player.buffTime[buffIndex] > 60 * 10)
            {
                if (player.buffTime[buffIndex] % 20 == 0)
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, dir * Main.rand.NextFloat(1, 5), ModContent.ProjectileType<OstertagiWorm>(), 5, 0);
            }
            else if (player.buffTime[buffIndex] < 60 * 10)
            {
                if (player.buffTime[buffIndex] % 10 == 0)
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, dir * Main.rand.NextFloat(1, 5), ModContent.ProjectileType<OstertagiWorm>(), 5, 0);
            }
        }
    }
}
