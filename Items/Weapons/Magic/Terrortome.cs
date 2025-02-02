using EbonianMod.Items.Materials;
using EbonianMod.Items.Misc;
using EbonianMod.Items.Weapons.Melee;
using EbonianMod.Projectiles.Friendly.Corruption;
using EbonianMod.Projectiles.Terrortoma;
using EbonianMod.Projectiles.VFXProjectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Terraria.GameContent.Bestiary.IL_BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions;

namespace EbonianMod.Items.Weapons.Magic
{
    public class Terrortome : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 53;
            Item.width = 32;
            Item.height = 38;
            Item.maxStack = 1;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.knockBack = 2f;
            Item.rare = ItemRarityID.LightRed;
            Item.mana = 100;
            Item.noMelee = true;
            Item.staff[Item.type] = true;
            Item.shoot = ProjectileType<TerrortomeFlame>();
            SoundStyle s = EbonianSounds.chargedBeamImpactOnly.WithVolumeScale(0.5f);
            s.PitchVariance = 0.2f;
            Item.UseSound = s;
            Item.shootSpeed = 8f;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Magic;

        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.CursedFlame).AddIngredient(ItemID.SoulofSight, 15).AddIngredient(ItemType<TerrortomaMaterial>(), 20).AddTile(TileID.Bookcases).Register();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position -= new Vector2(0, 220);
            velocity = Helper.FromAToB(position, Main.MouseWorld);
            velocity.Normalize();
            type = Main.rand.Next([ProjectileType<TerrortomeFlame>()]);
            velocity = velocity.RotatedByRandom(PiOver4);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: 2.5f);
            //Projectile.NewProjectile(source, position, -velocity, type, damage, knockback, player.whoAmI, ai1: -3);
            return false;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ProjectileType<TerrortomeFlame>()] < 1;
        }
    }
    public class TerrortomeWorm : OstertagiWorm
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Magic;
        }
    }
    public class TerrortomeFlame : TBeam
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 165;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 80;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, Main.rand.Next(20, 100));
        }
        bool RunOnce = true;
        public override void AI()
        {
            if (RunOnce)
            {
                Projectile.velocity.Normalize();
                Projectile.rotation = Projectile.velocity.ToRotation();
                RunOnce = false;
            }
            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.position - new Vector2(30, 0) + Projectile.velocity.ToRotation().ToRotationVector2() * Main.rand.NextFloat(Projectile.ai[0]), 60, 60, DustID.CursedTorch, 2f);
                Main.dust[dust].scale = 2f;
                Main.dust[dust].velocity = Main.rand.NextVector2Circular(5, 5);
                Main.dust[dust].noGravity = true;
            }

            if (Projectile.timeLeft % 5 == 0)
            {
                if (Projectile.timeLeft % 15 == 0)
                    Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0, ai2: 1);

                Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileType<TerrortomaScream>(), 0, 0);
            }
            Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], 2, 0.01f);

            Projectile.ai[0] = MathHelper.SmoothStep(Projectile.ai[0], 1800, 0.35f);

            Projectile.rotation += ToRadians(Projectile.ai[1]) * Projectile.ai[2];

            Projectile.velocity = Projectile.rotation.ToRotationVector2();
            //Projectile.velocity = -Projectile.velocity.RotatedBy(MathHelper.ToRadians(Projectile.ai[1]));

            float progress = Utils.GetLerpValue(0, 165, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 1.5f * (Projectile.scale + 0.5f), 0, 1);
        }
    }
}
