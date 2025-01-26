using EbonianMod.Projectiles.Friendly.Corruption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EbonianMod.Common.Systems.Misc;
using Terraria.DataStructures;
using ReLogic.Utilities;
using Terraria.Audio;
using EbonianMod.Common.Systems;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace EbonianMod.Items.Weapons.Melee
{
    public class GarbageFlail : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ItemRarityID.Green;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.knockBack = 4f;
            Item.damage = 25;
            Item.noUseGraphic = true;
            Item.shoot = ProjectileType<GarbageFlailP>();
            Item.shootSpeed = 1;
            Item.UseSound = SoundID.Item1;
            Item.DamageType = DamageClass.Melee;
            Item.crit = 9;
            Item.channel = true;
        }
    }
    public class GarbageFlailP : ModProjectile
    {
        public virtual float Ease(float f)
        {
            return 1 - (float)Math.Pow(2, 10 * f - 10);
        }
        public virtual float ScaleFunction(float progress)
        {
            return 0.7f + (float)Math.Sin(progress * Math.PI) * 0.5f;
        }
        float holdOffset;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.Size = new(14, 20);
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = 25;
            holdOffset = 8;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
        }
        float lerpT = 0.5f;
        Verlet verlet;
        SlotId slot;
        public override void OnSpawn(IEntitySource source)
        {
            verlet = new Verlet(Projectile.Center, 2, 15, stiffness: 40);
            SoundEngine.PlaySound(SoundID.Item23.WithPitchOffset(0.5f));

            slot = SoundEngine.PlaySound(EbonianSounds.ghizasWheel.WithPitchOffset(-0.35f));
            if (SoundEngine.TryGetActiveSound(slot, out var _sound))
            {
                _sound.Volume = 0;
            }
        }
        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(slot, out var sound))
            {
                sound.Stop();
            }
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
            if (player.HeldItem.type != ItemType<GarbageFlail>()) player.itemTime = 0; player.itemAnimation = 0;
            if (SoundEngine.TryGetActiveSound(slot, out var _sound))
            {
                _sound.Volume = MathHelper.Lerp(_sound.Volume, Projectile.ai[0] * 0.8f, 0.15f);
            }

            Projectile.timeLeft = 10;
            Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
            Vector2 pos = player.RotatedRelativePoint(player.MountedCenter);
            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            player.itemRotation = (Projectile.velocity.ToRotation()) * player.direction;
            pos += (Projectile.velocity.ToRotation()).ToRotationVector2() * holdOffset;
            Projectile.Center = pos;
            Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.direction == -1 ? MathHelper.Pi : 0;
            if (player.gravDir != -1)
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - MathHelper.PiOver2);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(player.Center, Main.MouseWorld), lerpT).SafeNormalize(Vector2.UnitX);

            Projectile.ai[0] = MathHelper.Clamp(MathHelper.Lerp(Projectile.ai[0], 1, 0.05f), 0, 1);
            Projectile.ai[1] += MathHelper.ToRadians(Projectile.ai[0] * 18.2f);
            Projectile.ai[2] += MathHelper.ToRadians(Projectile.ai[0] * 12.6f);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 headPos = Projectile.Center + new Vector2(80, 0).RotatedBy(Projectile.ai[1]);
            if (verlet != null)
                for (int i = 0; i < verlet.points.Count; i++)
                {
                    if (targetHitbox.Intersects(new Rectangle((int)verlet.points[i].position.X, (int)verlet.points[i].position.Y, 5, 5)))
                        return true;
                }
            return targetHitbox.Intersects(new Rectangle((int)headPos.X - 17, (int)headPos.Y - 17, 34, 34));
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D head = Helper.GetTexture("Items/Weapons/Melee/GarbageFlailHead");

            Vector2 headPos = Projectile.Center + new Vector2(80, 0).RotatedBy(Projectile.ai[1]);
            if (verlet != null)
            {
                if (!Main.gamePaused)
                    verlet.Update(Projectile.Center, headPos);
                verlet.Draw(Main.spriteBatch, "Items/Weapons/Melee/GarbageFlailChain");
            }
            Main.spriteBatch.Draw(head, headPos - Main.screenPosition, null, lightColor, Projectile.ai[2], head.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return true;
        }
    }
}
