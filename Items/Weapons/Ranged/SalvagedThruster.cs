using EbonianMod.Items.Weapons.Ranged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbonianMod.Items.Weapons.Ranged
{
    public class SalvagedThruster : ModItem
    {
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 30;
            Item.knockBack = 0;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.shoot = ProjectileType<SalvagedThrusterP>();
            Item.shootSpeed = 1f;
            Item.rare = ItemRarityID.Green;
            Item.useStyle = 5;
            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
        }

        public override bool? CanAutoReuseItem(Player player) => false;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            velocity.Normalize();
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }

    }
    public class SalvagedThrusterP : ModProjectile
    {
        float holdOffset = 13;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Size = new Vector2(44, 28);
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
            Main.projFrames[Type] = 2;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 210);
        }
        public override bool? CanDamage() => Projectile.frame == 1;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * (Projectile.width / 2 + 70), 50, ref a);
        }
        public override void AI()
        {

            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems || !player.channel)
            {
                Projectile.Kill();
                return;
            }
            if (player.itemTime < 2)
            {
                player.itemTime = 2;
                player.itemAnimation = 2;
            }

            Projectile.timeLeft = 10;
            Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
            Vector2 pos = player.RotatedRelativePoint(player.MountedCenter);
            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            player.itemRotation = (Projectile.velocity.ToRotation()) * player.direction;
            pos += (Projectile.velocity.ToRotation()).ToRotationVector2() * holdOffset;
            Projectile.Center = pos;
            player.heldProj = Projectile.whoAmI;
            Projectile.rotation = Projectile.velocity.ToRotation();
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - PiOver2);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(player.Center, Main.MouseWorld), 0.8f).SafeNormalize(Vector2.UnitX);


            Projectile.ai[2]++;

            if (Projectile.ai[2] % 5 == 0)
            {
                bool success = false;
                for (int j = 0; j < 58; j++)
                {
                    if (player.inventory[j].ammo == AmmoID.Gel && player.inventory[j].stack > 0)
                    {
                        if (player.inventory[j].maxStack > 1 && Projectile.ai[2] % 30 == 0)
                            player.inventory[j].stack--;
                        success = true;
                        break;
                    }
                }
                if (!success)
                {
                    Dust.NewDustPerfect(pos + Projectile.velocity * Projectile.width / 2, DustID.Smoke, Projectile.velocity.RotatedByRandom(PiOver4 / 3) * Main.rand.NextFloat(0.1f, 3));
                    Projectile.frame = 0;
                    return;
                }
                else
                    Projectile.frame = 1;

                SoundEngine.PlaySound(SoundID.Item34, Projectile.Center);
                Dust.NewDustPerfect(pos + Projectile.velocity * Main.rand.NextFloat(50), DustID.Torch, Projectile.velocity.RotatedByRandom(PiOver4 * 0.4f) * Main.rand.NextFloat(5, 10));
            }
            if (Projectile.ai[2] % 4 == 0)
            {
                Projectile.ai[0]++;
                if (Projectile.ai[0] < 3 || Projectile.ai[0] > 6)
                    Projectile.ai[0] = 3;
            }
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = Helper.GetTexture("NPCs/Garbage/HotGarbage_Fire");

            if (Projectile.frame == 1)
            {
                Main.EntitySpriteDraw(tex, Projectile.Center - new Vector2(0, 8).RotatedBy(Projectile.rotation) + Projectile.velocity * Projectile.width / 2 - Main.screenPosition, new Rectangle(0, (int)Projectile.ai[0] * 76, 70, 76), Color.White, Projectile.rotation, new Vector2(0, 76 / 2), 1, SpriteEffects.None);
            }
        }
    }
}

