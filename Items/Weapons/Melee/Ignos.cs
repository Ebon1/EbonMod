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
using EbonianMod.Projectiles.Exol;

namespace EbonianMod.Items.Weapons.Melee
{
    public class Ignos : ModItem
    {
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
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.ai[1] <= 0)
            {
                Projectile.ai[1] = 100;
                target.AddBuff(BuffID.OnFire, 100);
                Projectile a = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), player.Center, Vector2.UnitX * Projectile.direction * 0.25f, ModContent.ProjectileType<EFireBreath2>(), damage, knockback, Projectile.owner);
                a.friendly = true;
                a.hostile = false;
                a.localAI[0] = 200;
                a.localAI[1] = 3;
                a.scale = 0.25f;
            }
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 16;
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
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            SpriteEffects effects = SpriteEffects.None;
            if (Projectile.timeLeft < 15)
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * 128, Projectile.width, 128), Color.White, Projectile.rotation, Projectile.Size / 2, Projectile.scale, effects, 0);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.ai[0] = 1;
        }
        public override void AI()
        {
            Projectile.ai[1]--;
            Lighting.AddLight(Projectile.Center, TorchID.Orange);
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 15)
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

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rot - MathHelper.PiOver2);

            Projectile.rotation = rot;
            Projectile.Center = pos;
            player.itemTime = 2;
            Projectile.timeLeft = 2;
            player.heldProj = Projectile.whoAmI;
            player.itemAnimation = 2;
        }
    }
}
