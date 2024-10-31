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

namespace EbonianMod.Items.Weapons.Ranged
{
    public class IchorFlintlock : ModItem
    {
        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.width = 48;
            Item.height = 66;
            Item.crit = 45;
            Item.damage = 28;
            Item.useAnimation = 32;
            Item.useTime = 32;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            //Item.reuseDelay = 45;
            Item.DamageType = DamageClass.Ranged;
            //Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.LightRed;
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<IchorFlintlockP>();

            Item.channel = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.TheUndertaker).AddIngredient(ItemID.TissueSample, 20).AddIngredient(ItemID.Ichor, 15).AddTile(TileID.Anvils).Register();
        }
    }
    public class IchorFlintlockP : ModProjectile
    {
        public override string Texture => "EbonianMod/Items/Weapons/Ranged/IchorFlintlock";
        public virtual float Ease(float f)
        {
            return 1 - (float)Math.Pow(2, 10 * f - 10);
        }
        public virtual float ScaleFunction(float progress)
        {
            return 0.7f + (float)Math.Sin(progress * Math.PI) * 0.5f;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
        }
        float holdOffset;
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.Size = new(28, 24);
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = 20;
            holdOffset = 22;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            float progress = Ease(Utils.GetLerpValue(0f, 15, Projectile.timeLeft));
            if (Projectile.timeLeft == 19)
            {
                if (Projectile.ai[2] >= 5)
                {
                    Projectile.ai[2] = 0;
                    for (int i = 0; i < 15; i++)
                        Dust.NewDustPerfect(Projectile.Center, DustID.IchorTorch, Projectile.velocity.RotatedByRandom(1) * Main.rand.NextFloat(1, 5f));
                    SoundEngine.PlaySound(SoundID.Item92);
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity * 10, ModContent.ProjectileType<IchorBlast>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Item11);
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity * 20, ProjectileID.IchorBullet, Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
            if (Projectile.timeLeft > 15)
            {
                holdOffset--;
                if (Projectile.direction == -1)
                {
                    Projectile.ai[0] += MathHelper.ToRadians(4.5f / 2);
                }
                else
                {
                    Projectile.ai[0] -= MathHelper.ToRadians(4.5f / 2);
                }
            }
            else
            {
                if (Projectile.direction == -1)
                {
                    Projectile.ai[0] -= MathHelper.ToRadians(.9f);
                }
                else
                {
                    Projectile.ai[0] += MathHelper.ToRadians(.9f);
                }
            }
            Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
            Vector2 pos = player.RotatedRelativePoint(player.MountedCenter);
            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            player.itemRotation = (Projectile.velocity.ToRotation() + Projectile.ai[0]) * player.direction;
            pos += (Projectile.velocity.ToRotation() + Projectile.ai[0]).ToRotationVector2() * holdOffset;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - MathHelper.PiOver2 + Projectile.ai[0]);

            Projectile.rotation = (pos - player.Center).ToRotation() + Projectile.ai[0] * Projectile.spriteDirection;
            Projectile.Center = pos - Vector2.UnitY * 2;
            player.heldProj = Projectile.whoAmI;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            SpriteEffects effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2, Projectile.scale, effects, 0);
            return false;
        }
        public override bool PreKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (player.active && player.channel && !player.dead && !player.CCed && !player.noItems)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    Vector2 dir = Vector2.Normalize(Main.MouseWorld - player.Center);
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), player.Center, dir, Projectile.type, Projectile.damage, Projectile.knockBack, player.whoAmI);
                    proj.rotation = Projectile.rotation;
                    proj.Center = Projectile.Center;
                    proj.ai[2] = Projectile.ai[2] + 1;
                }
            }
            return base.PreKill(timeLeft);
        }
        public override void Kill(int timeLeft)
        {
        }
    }
}