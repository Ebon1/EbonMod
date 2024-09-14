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
using Terraria.GameContent;
using EbonianMod.NPCs.Garbage;

using static System.Net.Mime.MediaTypeNames;

namespace EbonianMod.Items.Weapons.Melee
{
    public class IgnosSword : ModItem
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.width = 52;
            Item.height = 62;
            Item.crit = 32;
            Item.damage = 30;
            Item.useAnimation = 32;
            Item.useTime = 32;
            Item.noUseGraphic = true;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.channel = true;

            //Item.useTurn = true;
            Item.DamageType = DamageClass.Melee;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.LightPurple;
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<IgnosP>();
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.HellstoneBar, 35).AddTile(TileID.MythrilAnvil).Register();
        }
    }
    public class IgnosP : ModProjectile
    {
        public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.ai[0] > 0)
                target.StrikeNPC(hitinfo);
            if (Projectile.ai[0] < 3 && Projectile.ai[1] <= 0)
            {
                Projectile.ai[1] = 300;
                Projectile.ai[0]++;
            }
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 16;

            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;

        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.Size = new(162, 126);
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = 15;
        }
        Rectangle rectangle = new Rectangle();
        public void HandleHitboxRight(ref Rectangle hitbox)
        {
            switch (Projectile.frame)
            {
                case 0:
                    hitbox.Width = 68;
                    hitbox.Height = 48;
                    hitbox.X = (int)Projectile.Center.X - (hitbox.Width);
                    hitbox.Y = (int)Projectile.Center.Y - 24;
                    break;
                case 1:
                    hitbox.Width = 63;
                    hitbox.Height = 54;
                    hitbox.X = (int)Projectile.Center.X - hitbox.Width;
                    hitbox.Y = (int)Projectile.Center.Y - 34;
                    break;
                case 2:
                    hitbox.Width = 108;
                    hitbox.Height = 104;
                    hitbox.X = (int)Projectile.Center.X - 20;
                    hitbox.Y = (int)Projectile.Top.Y - 5;
                    break;
                case 3:
                    hitbox.Width = 84;
                    hitbox.Height = 62;
                    hitbox.X = (int)Projectile.Center.X;
                    hitbox.Y = (int)Projectile.Center.Y;
                    break;
                case 4:
                    hitbox.Width = 31;
                    hitbox.Height = 50;
                    hitbox.X = (int)Projectile.Center.X - 10;
                    hitbox.Y = (int)Projectile.Center.Y + 15;
                    break;
                case 5:
                    hitbox.Width = 58;
                    hitbox.Height = 34;
                    hitbox.X = (int)Projectile.Center.X - hitbox.Width;
                    hitbox.Y = (int)Projectile.Center.Y + 15;
                    break;
                case 6:
                    hitbox.Width = Projectile.width / 2;
                    hitbox.Height = 14;
                    hitbox.X = (int)Projectile.Center.X - hitbox.Width;
                    hitbox.Y = (int)Projectile.Center.Y + 15;
                    break;
                case 7:
                    hitbox.Width = Projectile.width / 2;
                    hitbox.Height = 14;
                    hitbox.X = (int)Projectile.Center.X - hitbox.Width;
                    hitbox.Y = (int)Projectile.Center.Y + 13;
                    break;
                case 8:
                    hitbox.Width = Projectile.width / 2;
                    hitbox.Height = 14;
                    hitbox.X = (int)Projectile.Center.X - hitbox.Width;
                    hitbox.Y = (int)Projectile.Center.Y + 10;
                    break;
                case 9:
                    hitbox.Width = Projectile.width;
                    hitbox.Height = 50;
                    hitbox.X = (int)Projectile.Center.X - hitbox.Width / 2;
                    hitbox.Y = (int)Projectile.Center.Y;
                    break;
                case 10:
                    hitbox.Width = Projectile.width / 2;
                    hitbox.Height = 50;
                    hitbox.X = (int)Projectile.Center.X;
                    hitbox.Y = (int)Projectile.Center.Y - 20;
                    break;
                case 11:
                    hitbox.Width = 15;
                    hitbox.Height = 40;
                    hitbox.X = (int)Projectile.Center.X;
                    hitbox.Y = (int)Projectile.Center.Y - 20;
                    break;
                case 12:
                    hitbox.Width = 15;
                    hitbox.Height = 40;
                    hitbox.X = (int)Projectile.Center.X - 3;
                    hitbox.Y = (int)Projectile.Center.Y - 20;
                    break;
                case 13:
                    hitbox.Width = 15;
                    hitbox.Height = 40;
                    hitbox.X = (int)Projectile.Center.X - 6;
                    hitbox.Y = (int)Projectile.Center.Y - 20;
                    break;
                case 14:
                    hitbox.Width = Projectile.width;
                    hitbox.Height = 55;
                    hitbox.X = (int)Projectile.Center.X - hitbox.Width / 2;
                    hitbox.Y = (int)Projectile.Center.Y;
                    break;
                case 15:
                    hitbox.Width = Projectile.width / 2;
                    hitbox.Height = 50;
                    hitbox.X = (int)Projectile.Center.X - hitbox.Width;
                    hitbox.Y = (int)Projectile.Center.Y - 20;
                    break;
            }
        }
        public void HandleHitboxLeft(ref Rectangle hitbox)
        {
            switch (Projectile.frame)
            {
                case 0:
                    hitbox.Width = 68;
                    hitbox.Height = 48;
                    hitbox.X = (int)Projectile.Center.X;
                    hitbox.Y = (int)Projectile.Center.Y - 28 - 24;
                    break;
                case 1:
                    hitbox.Width = 63;
                    hitbox.Height = 54;
                    hitbox.X = (int)Projectile.Center.X;
                    hitbox.Y = (int)Projectile.Center.Y - 28 - 34;
                    break;
                case 2:
                    hitbox.Width = 108;
                    hitbox.Height = 104;
                    hitbox.X = (int)Projectile.Center.X - hitbox.Width / 2 - 20;
                    hitbox.Y = (int)Projectile.Top.Y - 28 - 5;
                    break;
                case 3:
                    hitbox.Width = 84;
                    hitbox.Height = 62;
                    hitbox.X = (int)Projectile.Center.X - hitbox.Width;
                    hitbox.Y = (int)Projectile.Center.Y - 28;
                    break;
                case 4:
                    hitbox.Width = 31;
                    hitbox.Height = 50;
                    hitbox.X = (int)Projectile.Center.X - 10;
                    hitbox.Y = (int)Projectile.Center.Y - 28 + 15;
                    break;
                case 5:
                    hitbox.Width = 58;
                    hitbox.Height = 34;
                    hitbox.X = (int)Projectile.Center.X;
                    hitbox.Y = (int)Projectile.Center.Y - 28 + 15;
                    break;
                case 6:
                    hitbox.Width = Projectile.width / 2;
                    hitbox.Height = 14;
                    hitbox.X = (int)Projectile.Center.X;
                    hitbox.Y = (int)Projectile.Center.Y - 28 + 15;
                    break;
                case 7:
                    hitbox.Width = Projectile.width / 2;
                    hitbox.Height = 14;
                    hitbox.X = (int)Projectile.Center.X;
                    hitbox.Y = (int)Projectile.Center.Y - 28 + 13;
                    break;
                case 8:
                    hitbox.Width = Projectile.width / 2;
                    hitbox.Height = 14;
                    hitbox.X = (int)Projectile.Center.X;
                    hitbox.Y = (int)Projectile.Center.Y - 28 + 10;
                    break;
                case 9:
                    hitbox.Width = Projectile.width;
                    hitbox.Height = 50;
                    hitbox.X = (int)Projectile.Center.X - hitbox.Width / 2;
                    hitbox.Y = (int)Projectile.Center.Y - 28;
                    break;
                case 10:
                    hitbox.Width = Projectile.width / 2;
                    hitbox.Height = 50;
                    hitbox.X = (int)Projectile.Center.X - hitbox.Width;
                    hitbox.Y = (int)Projectile.Center.Y - 28 - 20;
                    break;
                case 11:
                    hitbox.Width = 15;
                    hitbox.Height = 40;
                    hitbox.X = (int)Projectile.Center.X - 4;
                    hitbox.Y = (int)Projectile.Center.Y - 28 - 20;
                    break;
                case 12:
                    hitbox.Width = 15;
                    hitbox.Height = 40;
                    hitbox.X = (int)Projectile.Center.X - 5;
                    hitbox.Y = (int)Projectile.Center.Y - 28 - 20;
                    break;
                case 13:
                    hitbox.Width = 15;
                    hitbox.Height = 40;
                    hitbox.X = (int)Projectile.Center.X - 6;
                    hitbox.Y = (int)Projectile.Center.Y - 28 - 20;
                    break;
                case 14:
                    hitbox.Width = Projectile.width;
                    hitbox.Height = 55;
                    hitbox.X = (int)Projectile.Center.X - hitbox.Width / 2;
                    hitbox.Y = (int)Projectile.Center.Y - 28;
                    break;
                case 15:
                    hitbox.Width = Projectile.width / 2;
                    hitbox.Height = 50;
                    hitbox.X = (int)Projectile.Center.X;
                    hitbox.Y = (int)Projectile.Center.Y - 28 - 20;
                    break;
            }
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (Projectile.direction == 1) HandleHitboxRight(ref hitbox);
            else HandleHitboxLeft(ref hitbox);
            rectangle = hitbox;
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D tex2 = Helper.GetTexture("Items/Weapons/Melee/IgnosP_Glow");
            SpriteEffects effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Main.spriteBatch.Reload(BlendState.Additive);
            if (Projectile.timeLeft < 14)
                Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition - new Vector2(0, Projectile.direction == -1 ? 28 : 0), new Rectangle(0, Projectile.frame * 128, Projectile.width, 128), Color.OrangeRed * (Projectile.ai[0] / 5), Projectile.rotation, Projectile.Size / 2, Projectile.scale, effects, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            SpriteEffects effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            if (Projectile.timeLeft < 14)
            {
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition - new Vector2(0, Projectile.direction == -1 ? 28 : 0), new Rectangle(0, Projectile.frame * 128, Projectile.width, 128), Color.White, Projectile.rotation, Projectile.Size / 2, Projectile.scale, effects, 0);
            }
            return false;
        }
        public override void AI()
        {
            Projectile.ai[1]--;
            if (Projectile.ai[1] < -100 && Projectile.ai[0] > 0)
            {
                Projectile.ai[1] = 0;
                Projectile.ai[0]--;
            }
            Lighting.AddLight(Projectile.Center, TorchID.Orange);
            if (++Projectile.frameCounter >= 3 - Projectile.ai[0])
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 15)
                {
                    Projectile.frame = 0;
                }
            }

            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems || !player.channel)
            {
                Projectile.Kill();
                return;
            }
            //if (player.direction != player.oldDirection)
            //    Projectile.frame = 0;
            float rot = player.direction == -1 ? MathHelper.ToRadians(180) : 0;
            Projectile.direction = player.direction;

            Vector2 pos = player.Center - new Vector2(-10, 17).RotatedBy(Projectile.rotation);

            player.itemRotation = (rot);
            player.ChangeDir(Main.MouseWorld.X < player.Center.X ? -1 : 1);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rot - MathHelper.PiOver2);

            Projectile.rotation = rot;
            Projectile.Center = pos;
            player.itemTime = 2;
            if (Projectile.timeLeft < 14)
                Projectile.timeLeft = 2;
            //player.heldProj = Projectile.whoAmI;
            player.itemAnimation = 2;
        }
    }
}
