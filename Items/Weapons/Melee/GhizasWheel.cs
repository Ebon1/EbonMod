using EbonianMod.Common.Systems;
using EbonianMod.Dusts;
using EbonianMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Items.Weapons.Melee
{
    public class GhizasWheel : ModItem
    {
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.damage = 30;
            Item.knockBack = 0;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.shoot = ProjectileType<GhizasWheelP>();
            Item.shootSpeed = 1f;
            Item.rare = ItemRarityID.LightRed;
            Item.useStyle = 5;
            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.CursedFlame, 20).AddIngredient(ItemID.SoulofNight, 10).AddIngredient(ItemID.RottenChunk, 20).AddTile(TileID.MythrilAnvil).Register();
        }
        public override bool? CanAutoReuseItem(Player player) => false;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            velocity.Normalize();
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
    public class GhizasWheelP : ModProjectile
    {
        float holdOffset = 30;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Size = new Vector2(48, 36);
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[2] = 16;
            target.AddBuff(BuffID.CursedInferno, 500);

            Vector2 pos = Projectile.Center + new Vector2(22, -16).RotatedBy(Projectile.rotation);

            for (int i = 0; i < 3; i++)
            {
                if (Main.rand.NextBool())
                    Dust.NewDustPerfect(pos + Helper.FromAToB(pos, target.Center).RotatedByRandom(MathHelper.PiOver4 * 0.7f) * 16, DustType<LineDustFollowPoint>(), Helper.FromAToB(pos, target.Center).RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(5, 8), 0, Color.Lerp(Color.Gold, Color.DarkOrange, Main.rand.NextFloat()), Main.rand.NextFloat(0.05f, 0.1f));
                else
                    Dust.NewDustPerfect(pos + Helper.FromAToB(pos, target.Center).RotatedByRandom(MathHelper.PiOver4 * 0.7f) * 16, DustType<SparkleDust>(), Helper.FromAToB(pos, target.Center).RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(5, 8), 0, Color.Lerp(Color.Gold, Color.DarkOrange, Main.rand.NextFloat()), Main.rand.NextFloat(0.025f, 0.075f));
            }
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            Vector2 pos = Projectile.Center + new Vector2(22, -16).RotatedBy(Projectile.rotation);
            hitbox.Width = 36;
            hitbox.Height = 36;
            hitbox.X = (int)pos.X - hitbox.Width / 2;
            hitbox.Y = (int)pos.Y - hitbox.Height / 2;
        }
        SlotId slot, slot2;
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item23.WithPitchOffset(0.5f));
            slot = SoundEngine.PlaySound(EbonianSounds.ghizasWheel);
            if (SoundEngine.TryGetActiveSound(slot, out var sound))
            {
                sound.Volume = 0;
            }
            slot2 = SoundEngine.PlaySound(EbonianSounds.ghizasWheel.WithPitchOffset(0.35f));
            if (SoundEngine.TryGetActiveSound(slot2, out var _sound))
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
            if (SoundEngine.TryGetActiveSound(slot2, out var _sound))
            {
                _sound.Stop();
            }
        }
        float lerpT = 0.5f;
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

            Projectile.ai[2]--;

            Vector2 wheelPos = Projectile.Center + new Vector2(22, -16).RotatedBy(Projectile.rotation);
            Rectangle hitbox = new();
            hitbox.Width = 36;
            hitbox.Height = 36;
            hitbox.X = (int)wheelPos.X - hitbox.Width / 2;
            hitbox.Y = (int)wheelPos.Y - hitbox.Height / 2;

            if (SoundEngine.TryGetActiveSound(slot2, out var _sound))
            {
                if (Projectile.ai[2] <= 0)
                {
                    _sound.Volume = MathHelper.Lerp(_sound.Volume, 0f, 0.1f);
                }
                else
                {
                    _sound.Volume = MathHelper.Lerp(_sound.Volume, Projectile.ai[0] * 0.8f, 0.15f);
                }
            }
            if (Projectile.ai[2] > 0)
            {
                lerpT = MathHelper.Lerp(lerpT, 0.02f, 0.3f);
            }
            else
            {
                lerpT = MathHelper.Lerp(lerpT, 0.5f, 0.1f);
            }
            Projectile.timeLeft = 10;
            Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
            Vector2 pos = player.RotatedRelativePoint(player.MountedCenter);
            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            player.itemRotation = (Projectile.velocity.ToRotation()) * player.direction;
            pos += (Projectile.velocity.ToRotation()).ToRotationVector2() * holdOffset;
            Projectile.Center = pos;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - MathHelper.PiOver2);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(player.Center, Main.MouseWorld), lerpT).SafeNormalize(Vector2.UnitX);

            Projectile.ai[0] = MathHelper.Clamp(MathHelper.Lerp(Projectile.ai[0], 1, 0.05f), 0, 1);

            if (SoundEngine.TryGetActiveSound(slot, out var sound))
            {
                sound.Volume = MathHelper.Lerp(sound.Volume, Projectile.ai[0] * 0.7f, 0.1f);
            }

            Projectile.ai[1] += MathHelper.ToRadians(Projectile.ai[0] * 9.2f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D wheel = Helper.GetTexture(Texture + "_Wheel");
            Vector2 pos = Projectile.Center + new Vector2(22, -16).RotatedBy(Projectile.rotation);
            Main.spriteBatch.Draw(wheel, pos - Main.screenPosition, null, lightColor, Projectile.ai[1], wheel.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            for (int i = 0; i < 15; i++)
            {
                float alpha = MathHelper.Lerp(0.5f, 0, (float)i / 15) * Projectile.ai[0];
                Main.spriteBatch.Draw(wheel, pos - Main.screenPosition, null, lightColor * alpha, Projectile.ai[1] + MathHelper.ToRadians(i * 3), wheel.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            }
            return true;
        }
    }
}
