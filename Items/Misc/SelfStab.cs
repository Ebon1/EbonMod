using EbonianMod.Common.Systems;
using EbonianMod.Projectiles.Dev;
using EbonianMod.Projectiles.Friendly.Crimson;
using EbonianMod.Projectiles.VFXProjectiles;
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

namespace EbonianMod.Items.Misc
{
    public class SelfStab : ModItem
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
            Item.rare = ItemRarityID.LightRed;
            Item.shootSpeed = 1f;
            Item.shoot = ProjectileType<SelfStabP>();
        }
        public override bool? CanAutoReuseItem(Player player)
        {
            return false;
        }
        public override bool AllowPrefix(int pre)
        {
            return false;
        }
    }
    public class SelfStabP : ModProjectile
    {
        public override string Texture => "EbonianMod/Items/Misc/SelfStab";
        int swingTime = 15;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = swingTime + 45;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public float Ease(float x)
        {
            /*return (float)(x == 0
              ? 0
              : x == 1
              ? 1
              : x < 0.5 ? Math.Pow(2, 20 * x - 10) / 2
              : (2 - Math.Pow(2, -20 * x + 10)) / 2);*/
            return x < 0.5
              ? (1 - MathF.Sqrt(1 - MathF.Pow(2 * x, 2))) / 2
              : (MathF.Sqrt(1 - MathF.Pow(-2 * x + 2, 2)) + 1) / 2;
        }
        public float ScaleFunction(float progress)
        {
            return 0.7f + (float)Math.Sin(progress * Math.PI) * 0.5f;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems)
                return;
            player.itemTime = 2;
            player.itemAnimation = 2; if (player.HeldItem.type != ItemType<SelfStab>()) player.itemTime = 0; player.itemAnimation = 0;
            Player.CompositeArmStretchAmount stretch = Player.CompositeArmStretchAmount.None;
            if (Projectile.timeLeft == 22)
            {
                int dmg = player.statLifeMax2 - 20;
                PlayerDeathReason customReason = new()
                {
                    SourceItem = player.HeldItem,
                    SourceCustomReason = $"{player.name} decided to take the easy way out..."
                };
                Player.HurtInfo info = new();
                info.Damage = dmg;
                info.DamageSource = customReason;
                player.Hurt(info);
                player.immuneTime = 0;
                SoundEngine.PlaySound(EbonianSounds.fleshHit, player.Center);
                EbonianSystem.ScreenShakeAmount = 5;
                player.GetModPlayer<EbonianPlayer>().stabDirection = new Vector2(-player.direction, Main.rand.NextFloat(-1, 0));
                for (int i = 0; i < 5; i++)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, new Vector2(-player.direction, Main.rand.NextFloat(-1, 0)).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(3, 7), ProjectileType<Gibs>(), 15, 0, ai2: 0);
                }
                for (int i = 0; i < 30; i++)
                {
                    Dust.NewDustPerfect(player.Center, DustID.Blood, new Vector2(-player.direction, Main.rand.NextFloat(-1, 0)).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(4.5f, 7));
                    if (i % 5 == 0)
                        Dust.NewDustPerfect(player.Center, DustID.Blood, new Vector2(player.direction, Main.rand.NextFloat(-1, 0)).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(2, 4));
                }
                player.AddBuff(BuffType<SelfStabB>(), 60 * 60);
                player.AddBuff(BuffID.PotionSickness, 60 * 120);
            }
            if (Projectile.timeLeft <= 30 && Projectile.timeLeft > 15)
            {
                float _swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft - 15));
                float swingProgress = MathHelper.Lerp(1, 0, _swingProgress);
                Projectile.Center = player.Center + Vector2.Lerp(new Vector2(player.direction * 40, 3), new Vector2(-10 * player.direction, 0), swingProgress);
                if (swingProgress < 0.25f)
                    stretch = Player.CompositeArmStretchAmount.Full;
                else if (swingProgress < 0.5f)
                    stretch = Player.CompositeArmStretchAmount.ThreeQuarters;
                else if (swingProgress < 0.75f)
                    stretch = Player.CompositeArmStretchAmount.Quarter;
            }
            else
            {
                if (Projectile.timeLeft > 30)
                {
                    Projectile.Center = player.Center + new Vector2(player.direction * 40, 3);
                    stretch = Player.CompositeArmStretchAmount.Full;
                }
                else
                {
                    Projectile.Center = player.Center + new Vector2(player.direction * -10, 0);
                    stretch = Player.CompositeArmStretchAmount.None;
                }
            }
            Projectile.rotation = MathHelper.PiOver4 + (player.direction == 1 ? MathHelper.Pi : 0);
            Projectile.direction = player.direction;
            if (player.gravDir != -1)
                player.SetCompositeArmFront(true, stretch, Projectile.rotation + MathHelper.Pi - MathHelper.PiOver4 - MathHelper.PiOver2);
            if (player.gravDir != -1)
                player.SetCompositeArmBack(true, stretch, Projectile.rotation + (MathHelper.PiOver4 / 8) + MathHelper.Pi - MathHelper.PiOver4 - MathHelper.PiOver2);

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 orig = texture.Size() / 2;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), lightColor, Projectile.rotation + (Projectile.direction == -1 ? 0 : MathHelper.PiOver2 * 3), orig, Projectile.scale, Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
            return false;
        }
    }
    public class SelfStabB : ModBuff
    {
        public override string Texture => "EbonianMod/Buffs/SelfStabB";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.5f;
            player.GetDamage(DamageClass.Generic).Flat += 10;
            player.lifeRegen = 0;
            player.lifeRegenTime = 0;
            Vector2 dir = player.GetModPlayer<EbonianPlayer>().stabDirection.RotatedByRandom(MathHelper.PiOver4 / 2) * (Main.rand.NextBool(5) ? -1 : 1);
            if (player.buffTime[buffIndex] > 60 * 50)
            {
                if (player.buffTime[buffIndex] % 15 == 0)
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, dir * Main.rand.NextFloat(1, 5), ProjectileType<Gibs>(), 15, 0);
            }
            else if (player.buffTime[buffIndex] > 60 * 30)
            {
                if (player.buffTime[buffIndex] % 10 == 0)
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, dir * Main.rand.NextFloat(1, 5), ProjectileType<Gibs>(), 15, 0);
            }
            else if (player.buffTime[buffIndex] < 60 * 30)
            {
                if (player.buffTime[buffIndex] % 5 == 0)
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, dir * Main.rand.NextFloat(1, 5), ProjectileType<Gibs>(), 15, 0);
            }
        }
    }
}
