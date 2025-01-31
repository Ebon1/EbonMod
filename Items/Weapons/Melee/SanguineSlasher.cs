using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.DataStructures;
using EbonianMod.Projectiles;
using Terraria.Audio;
using System.Collections.Generic;
using EbonianMod.Projectiles.Terrortoma;
using EbonianMod.Projectiles.Cecitior;
using Terraria.GameContent;
using EbonianMod.Common.Systems;

namespace EbonianMod.Items.Weapons.Melee
{
    public class SanguineSlasher : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(25); // heal
        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.width = 48;
            Item.height = 66;
            Item.crit = 15;
            Item.damage = 18;
            Item.useAnimation = 32;
            Item.useTime = 32;
            Item.noUseGraphic = true;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.channel = true;
            //Item.reuseDelay = 45;
            Item.DamageType = DamageClass.Melee;
            //Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Green;
            Item.shootSpeed = 1f;
            Item.shoot = ProjectileType<SanguineSlasherP>();
        }
        int dir = 1;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            dir = -dir;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 1, dir);
            return false;
        }
    }
    public class SanguineSlasherP : HeldSword
    {
        public override void SetExtraDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 88;
            swingTime = 30;
            holdOffset = 28;
        }
        public override void AI()
        {
            Projectile.ai[2] = 0;
            if (Projectile.ai[0] % 3 == 0)
            {
                Projectile.scale = 1.4f;
                Projectile.ai[1] = 0;
            }
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems)
            {
                return;
            }
            if (player.itemTime < 2)
            {
                player.itemTime = 2;
                player.itemAnimation = 2;
            }
            if (player.HeldItem.type != ItemType<SanguineSlasher>()) { Projectile.Kill(); }
            if (Projectile.ai[2] != 0)
            {
                ExtraAI();
                return;
            }
            if (Projectile.ai[1] != 0)
            {
                visualOffset = new Vector2(-16, Projectile.ai[1] == -1 ? 4 : 0).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.scale;
                int direction = (int)Projectile.ai[1];
                float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
                float defRot = Projectile.velocity.ToRotation();
                float start = defRot - (MathHelper.PiOver2 + MathHelper.PiOver4);
                float end = defRot + (MathHelper.PiOver2 + MathHelper.PiOver4);
                float rotation = direction == 1 ? start + MathHelper.Pi * 3 / 2 * swingProgress : end - MathHelper.Pi * 3 / 2 * swingProgress;
                Vector2 position = player.GetFrontHandPosition(stretch, rotation - MathHelper.PiOver2) +
                    rotation.ToRotationVector2() * holdOffset * ScaleFunction(swingProgress);
                Projectile.Center = position;
                Projectile.rotation = (position - player.Center).ToRotation() + MathHelper.PiOver4;
                player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
                player.SetCompositeArmFront(true, stretch, rotation - MathHelper.PiOver2);
            }
            else
            {
                visualOffset = new Vector2(-18, 14).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.scale;
                float progress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
                jawRotation = MathHelper.SmoothStep(-MathHelper.PiOver4, 0, progress);
                holdOffset = baseHoldOffset * (progress + 1.5f);
                Vector2 pos = player.RotatedRelativePoint(player.MountedCenter);
                player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
                player.itemRotation = Projectile.velocity.ToRotation() * player.direction;
                pos += Projectile.velocity.ToRotation().ToRotationVector2() * holdOffset;
                if (player.gravDir != -1)
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - MathHelper.PiOver2);
                if (player.gravDir != -1)
                    Projectile.rotation = (pos - player.Center).ToRotation() + MathHelper.PiOver2 - MathHelper.PiOver4 * Projectile.spriteDirection;
                Projectile.Center = pos;
                player.itemTime = 2;
                player.itemAnimation = 2;
                if (Projectile.timeLeft == swingTime - 2)
                    SoundEngine.PlaySound(EbonianSounds.chomp0, player.Center);
                if (Projectile.timeLeft == swingTime - 8)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        Dust.NewDustPerfect(Projectile.Center + visualOffset + Projectile.velocity.ToRotation().ToRotationVector2() * 25, DustID.Blood, Main.rand.NextVector2Circular(10, 10));
                    }
                }
            }
            ExtraAI();
        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (player.active && player.channel && !player.dead && !player.CCed && !player.noItems)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    Vector2 dir = Vector2.Normalize(Main.MouseWorld - player.Center);
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), player.Center, dir, Projectile.type, Projectile.damage, Projectile.knockBack, player.whoAmI, Projectile.ai[0] + 1, (Projectile.ai[1] == 0 ? 1 : -Projectile.ai[1]));
                    proj.rotation = Projectile.rotation;
                    proj.Center = Projectile.Center;
                    proj.ai[0] = Projectile.ai[0] + 1;
                }
            }
        }
        bool _hit;
        public override void OnHit(NPC target, NPC.HitInfo hitinfo, int damage)
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.ai[0] % 3 == 0 && !_hit)
            {
                player.Heal(25);
                _hit = true;
            }
        }
        float jawRotation;
        public override void PostDraw(Color lightColor)
        {
            float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D jaw = Helper.GetTexture(Texture + "_Jaw");
            Texture2D head = Helper.GetTexture(Texture + "_Head");
            Vector2 orig = texture.Size() / 2;
            if (Projectile.ai[1] == 0)
            {
                Main.EntitySpriteDraw(texture, Projectile.Center + visualOffset - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), lightColor, Projectile.rotation + (Projectile.ai[1] == -1 ? 0 : MathHelper.PiOver2 * 3), orig, Projectile.scale, Projectile.ai[1] == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
                Main.EntitySpriteDraw(jaw, Projectile.Center + new Vector2(-18, 22).RotatedBy(Projectile.rotation + jawRotation + (Projectile.ai[1] == -1 ? 0 : MathHelper.PiOver2 * 3)) * Projectile.scale + visualOffset - Main.screenPosition, null, lightColor, Projectile.rotation + jawRotation + (Projectile.ai[1] == -1 ? 0 : MathHelper.PiOver2 * 3), new Vector2(0, jaw.Height), Projectile.scale, Projectile.ai[1] == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);

                Vector2 headOff = new Vector2(-23, 18);
                Vector2 headOrig = new Vector2(0, head.Height);
                if (Projectile.ai[1] != -1)
                {
                    headOrig = new Vector2(0, jaw.Height);
                    headOff = new Vector2(-20, 14);
                }
                Main.EntitySpriteDraw(head, Projectile.Center + headOff.RotatedBy(Projectile.rotation - jawRotation + (Projectile.ai[1] == -1 ? 0 : MathHelper.PiOver2 * 3)) * Projectile.scale + visualOffset - Main.screenPosition, null, lightColor, Projectile.rotation - jawRotation + (Projectile.ai[1] == -1 ? 0 : MathHelper.PiOver2 * 3), headOrig, Projectile.scale, Projectile.ai[1] == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
            }
            else
            {
                texture = Helper.GetTexture("Items/Weapons/Melee/SanguineSlasher");
                Main.EntitySpriteDraw(texture, Projectile.Center + visualOffset - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), lightColor, Projectile.rotation + (Projectile.ai[1] == -1 ? 0 : MathHelper.PiOver2 * 3), orig, Projectile.scale, Projectile.ai[1] == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
            }

            Player player = Main.player[Projectile.owner];
            Texture2D slash = ExtraTextures2.slash_06;
            float mult = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            float alpha = (float)Math.Sin(mult * Math.PI);
            Vector2 pos = player.Center + Projectile.velocity * 45f;
            Main.spriteBatch.Reload(BlendState.Additive);
            if (Projectile.ai[0] % 3 == 0)
                Main.spriteBatch.Draw(slash, pos + visualOffset - Main.screenPosition, null, Color.Maroon * alpha * 0.5f, Projectile.velocity.ToRotation(), slash.Size() / 2, Projectile.scale * 0.35f, SpriteEffects.None, 0f);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item1);
        }
    }
}
