using EbonianMod.Items.Materials;
using EbonianMod.Projectiles.Friendly.Crimson;
using EbonianMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using static EbonianMod.Helper;
using EbonianMod.Projectiles.VFXProjectiles;

namespace EbonianMod.Items.Weapons.Melee
{
    public class ToothToothbrush : ModItem
    {
        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.width = Item.height = 50;
            Item.crit = 10;
            Item.damage = 20;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.noUseGraphic = true;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.channel = true;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.DamageType = DamageClass.Melee;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Green;
            Item.shootSpeed = 1f;
            Item.shoot = ProjectileType<ToothToothbrushP>();
        }
        public override bool? CanAutoReuseItem(Player player)
        {
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Bone, 20).AddIngredient(ItemID.ViciousPowder, 10).AddTile(TileID.Anvils).Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, Vector2.UnitX * player.direction, type, damage, knockback, player.whoAmI, 0, -player.direction, 1);
            return false;
        }
    }
    public class ToothToothbrushP : HeldSword
    {
        public override string Texture => "EbonianMod/Items/Weapons/Melee/ToothToothbrush";
        public override void SetExtraDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            swingTime = 40;
            minSwingTime = 30;
            holdOffset = 30;
            Projectile.tileCollide = false;
            modifyCooldown = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public override float Ease(float x)
        {
            return x == 0
  ? 0
  : x == 1
  ? 1
  : x < 0.5 ? MathF.Pow(2, 20 * x - 10) / 2
  : (2 - MathF.Pow(2, -20 * x + 10)) / 2;
        }
        float lerpProg = 1, swingProgress, rotation;
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.ChangeDir(Main.MouseWorld.X < player.Center.X ? -1 : 1);
            if (player.active && player.channel && !player.dead && !player.CCed && !player.noItems)
                Projectile.NewProjectile(null, Projectile.Center, Vector2.UnitX * player.direction, Projectile.type, Projectile.damage, Projectile.knockBack, player.whoAmI, 0, -player.direction, 1);
        }
        public override void ExtraAI()
        {
            Player player = Main.player[Projectile.owner];
            if (lerpProg != 1 && lerpProg != -1)
                lerpProg = MathHelper.SmoothStep(lerpProg, 1, 0.1f);
            if (swingProgress > 0.05f && swingProgress < 0.95f)
                if (Projectile.ai[0] == 0 && Helper.TRay.CastLength(Projectile.Center, Vector2.UnitY, 100) < 15)
                {
                    Projectile.ai[0] = 1;
                    Projectile.timeLeft = 15;
                    SoundEngine.PlaySound(SoundID.Item70, Projectile.Center);
                    for (int i = 0; i < 6; i++)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Helper.TRay.Cast(Projectile.Center - new Vector2(0, 30), Vector2.UnitY, 60) - new Vector2(0, 10), Main.rand.NextVector2Circular(15, 15), ProjectileType<Gibs>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                    lerpProg = -1;
                }
            int direction = (int)Projectile.ai[1];
            if (lerpProg >= 0)
                swingProgress = MathHelper.Lerp(swingProgress, Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft)), lerpProg);
            float defRot = Projectile.velocity.ToRotation();
            float start = defRot - (MathHelper.PiOver2 + MathHelper.PiOver4);
            float end = defRot + (MathHelper.PiOver2 + MathHelper.PiOver4);
            if (lerpProg >= 0)
                rotation = MathHelper.Lerp(rotation, direction == 1 ? start + MathHelper.Pi * 3 / 2 * swingProgress : end - MathHelper.Pi * 3 / 2 * swingProgress, lerpProg);
            Vector2 position = player.GetFrontHandPosition(stretch, rotation - MathHelper.PiOver2) +
                rotation.ToRotationVector2() * holdOffset * ScaleFunction(swingProgress);
            Projectile.Center = position;
            Projectile.rotation = (position - player.Center).ToRotation() + MathHelper.PiOver4;
            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            player.heldProj = Projectile.whoAmI;
            if (player.gravDir != -1)
                player.SetCompositeArmFront(true, stretch, rotation - MathHelper.PiOver2);
            if (player.gravDir != -1)
                player.SetCompositeArmBack(true, stretch, rotation - MathHelper.PiOver2 - MathHelper.PiOver4);
        }
        public override bool? CanDamage()
        {
            return Projectile.ai[0] < 1 && swingProgress > 0.15f && swingProgress < 0.95f;
        }
        bool _hit = false;
        public override void OnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!_hit)
            {
                lerpProg = -.25f;
                _hit = true;
            }
        }
    }
}
