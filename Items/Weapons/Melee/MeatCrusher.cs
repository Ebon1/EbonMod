using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EbonianMod.Projectiles;
using EbonianMod.Projectiles.VFXProjectiles;
using EbonianMod.Projectiles.Friendly.Corruption;
//using EbonianMod.Worldgen.Subworlds;
//using SubworldLibrary;

namespace EbonianMod.Items.Weapons.Melee
{
    public class MeatCrusher : ModItem
    {
        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.width = Item.height = 80;
            Item.crit = 30;
            Item.damage = 50;
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
            Item.shoot = ModContent.ProjectileType<MeatCrusherP>();
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
    public class MeatCrusherP : HeldSword
    {
        public override string Texture => "EbonianMod/Items/Weapons/Melee/MeatCrusher";
        public override void SetExtraDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            swingTime = 60;
            holdOffset = 50;
        }
        public override float Ease(float x)
        {
            return (float)(x < 0.5 ? 16 * x * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 5) / 2);
        }
        float lerpProg = 1, swingProgress, rotation;
        public override void ExtraAI()
        {
            if (lerpProg < 0)
                Projectile.timeLeft++;
            if (lerpProg < 1)
                lerpProg += 0.1f;
            Player player = Main.player[Projectile.owner];
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
        }
        public override bool? CanDamage()
        {
            return Projectile.ai[0] < 3;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            lerpProg = -1;
            swingTime += 5;
            Projectile.ai[0]++;
            EbonianSystem.ScreenShakeAmount = 2;
            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), target.Center, new Vector2(Main.rand.NextFloat(-0.5f, 0.5f) * i, Main.rand.NextFloat(-2, -3)), ModContent.ProjectileType<VileMeatChunk>(), Projectile.damage, 0, Projectile.owner);
            }
        }
    }
}
