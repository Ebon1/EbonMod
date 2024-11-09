using EbonianMod.Projectiles.Friendly.Corruption;
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
using Terraria.ModLoader;
using Terraria;
using static EbonianMod.Helper;
using EbonianMod.Projectiles.Friendly.Crimson;
using EbonianMod.Items.Materials;

namespace EbonianMod.Items.Weapons.Melee
{
    internal class Serration : ModItem
    {
        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.width = Item.height = 80;
            Item.crit = 30;
            Item.damage = 130;
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
            Item.shoot = ModContent.ProjectileType<SerrationP>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<ToothToothbrush>()).AddIngredient(ModContent.ItemType<CecitiorMaterial>(), 20).AddTile(TileID.MythrilAnvil).Register();
        }
        public override bool? CanAutoReuseItem(Player player)
        {
            return false;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, Vector2.UnitX * player.direction, type, damage, knockback, player.whoAmI, 0, -player.direction, 1);
            return false;
        }
    }
    public class SerrationP : HeldSword
    {
        public override string Texture => "EbonianMod/Items/Weapons/Melee/Serration";
        public override void SetExtraDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            swingTime = 65;
            minSwingTime = 57;
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
            if (player.active && player.channel && !player.dead && !player.CCed && !player.noItems)
                Projectile.NewProjectile(null, Projectile.Center, Vector2.UnitX * player.direction, Projectile.type, Projectile.damage, Projectile.knockBack, player.whoAmI, 0, -player.direction, 1);
        }
        public override void ExtraAI()
        {
            Player player = Main.player[Projectile.owner];
            if (lerpProg != 1 && lerpProg != -1)
                lerpProg = MathHelper.SmoothStep(lerpProg, 1, 0.1f);
            if (swingProgress > 0.25f && swingProgress < 0.85f)
                if (Projectile.ai[0] == 0 && Helper.TRay.CastLength(Projectile.Center, Vector2.UnitY, 100) < 15)
                {
                    Projectile.ai[0] = 1;
                    EbonianSystem.ScreenShakeAmount = 5;
                    Projectile.timeLeft = 15;
                    SoundEngine.PlaySound(SoundID.Item70, Projectile.Center);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), TRay.Cast(Projectile.Center - Vector2.UnitY * 30, Vector2.UnitY, 500, true), new Vector2((float)player.direction, 0), ModContent.ProjectileType<SerrationSpike>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

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
            player.SetCompositeArmFront(true, stretch, rotation - MathHelper.PiOver2);
            player.SetCompositeArmBack(true, stretch, rotation - MathHelper.PiOver2 - MathHelper.PiOver4);
        }
        public override bool? CanDamage()
        {
            return Projectile.ai[0] < 1 && swingProgress > 0.35f && swingProgress < 0.65f;
        }
        bool _hit = false;
        public override void OnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            EbonianSystem.ScreenShakeAmount = 2;
            if (!_hit)
            {
                lerpProg = -.25f;
                _hit = true;
            }
        }
    }
}
