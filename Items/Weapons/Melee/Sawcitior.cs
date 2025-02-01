using EbonianMod.Items.Materials;
using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Items.Weapons.Melee
{
    public class Sawcitior : ModItem
    {
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noUseGraphic = true;
            Item.consumable = false;
            Item.Size = new(20);
            Item.useAnimation = 25;
            Item.crit = 15;
            Item.useTime = 25;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 58;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 8, 0, 0);
            Item.shoot = ProjectileType<SawcitiorP>();
            Item.rare = ItemRarityID.LightRed;
            Item.shootSpeed = 7f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.WoodenBoomerang, 2).AddIngredient(ItemType<CecitiorMaterial>(), 20).AddTile(TileID.MythrilAnvil).Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = -1; i < 2; i++)
            {
                if (i == 0) continue;
                Projectile.NewProjectile(source, position, velocity.RotatedBy(i * PiOver4 * 0.55f), type, damage, knockback, player.whoAmI, ai1: i);
            }
            return false;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ProjectileType<SawcitiorP>()] < 10;
        }
    }
    public class SawcitiorP : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 56;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = 500;
            Projectile.tileCollide = false;

        }
        Vector2 startVel;
        public override void AI()
        {
            if (startVel == Vector2.Zero)
                startVel = Projectile.velocity;
            Projectile.frame = (Projectile.velocity.Length() < 10 ? 1 : 0);
            Projectile.rotation += MathHelper.ToRadians(Projectile.velocity.Length() * 4);

            //Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(3));
            Player player = Main.player[Projectile.owner];
            if (player.active)
            {
                Projectile.timeLeft = 2;
                Projectile.ai[0]++;
                if (Projectile.ai[0] < 30)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * .5f, Scale: .8f);
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Ichor, Projectile.velocity.X * .5f, Projectile.velocity.Y * .5f, Scale: .8f).noGravity = true;
                    Projectile.velocity = Projectile.velocity.RotatedBy(ToRadians(2.3f * 0.5f * -Projectile.ai[1])) * 1.05f;
                }
                if (Projectile.ai[0] == 33)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Dust.NewDust(Projectile.position + Projectile.velocity * 2, Projectile.width, Projectile.height, DustID.Bone, Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, 6), Scale: 0.8f);
                        Dust.NewDust(Projectile.position + Projectile.velocity * 2, Projectile.width, Projectile.height, DustID.Blood, Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, 6), Scale: 1.6f);
                        Dust.NewDustDirect(Projectile.position + Projectile.velocity * 2, Projectile.width, Projectile.height, DustID.Ichor, Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, 6), Scale: 1.6f).noGravity = true;
                    }
                    SoundEngine.PlaySound(EbonianSounds.chomp1.WithVolumeScale(0.7f), Projectile.Center);
                }
                if (Projectile.ai[0] > 30 && Projectile.ai[0] < 50)
                    Projectile.velocity *= 0.3f;
                if (Projectile.ai[2] > 0)
                {
                    if (Projectile.ai[0] > 50)
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(Projectile.Center, player.Center).RotatedBy(PiOver4 * 0.25f * Projectile.ai[1] * Clamp(Utils.GetLerpValue(0, 300, player.Distance(Projectile.Center)), 0, 1)) * 30f, 0.1f);
                    if (Projectile.ai[0] > 50 && Projectile.Center.Distance(player.Center) < 50)
                        Projectile.Kill();
                }
                else
                {
                    if (Projectile.ai[0] > 45)
                    {
                        if (Main.myPlayer == Projectile.owner)
                            Projectile.velocity = startVel * 0.7f;
                        Projectile.ai[0] = -4;
                        Projectile.ai[2]++;
                    }
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
        {
            target.AddBuff(BuffID.Ichor, 150);
        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
        }
    }
}
