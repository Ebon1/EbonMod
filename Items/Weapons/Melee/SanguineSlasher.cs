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

namespace EbonianMod.Items.Weapons.Melee
{
    public class SanguineSlasher : ModItem
    {
        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.width = 48;
            Item.height = 66;
            Item.crit = 45;
            Item.damage = 34;
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
            Item.rare = ItemRarityID.LightRed;
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<SanguineSlasherP>();
        }
        int dir = 1;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            dir = -dir;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, dir);
            return false;
        }
    }
    public class SanguineSlasherP : HeldSword
    {
        public override string Texture => "EbonianMod/Items/Weapons/Melee/SanguineSlasher";
        public override void SetExtraDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 66;
            swingTime = 30;
            holdOffset = 35;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
        {
            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, new Vector2(Main.rand.NextFloat(-2, 2), -3), ModContent.ProjectileType<TFang>(), (int)(Projectile.damage * 0.3f), 0, 0);
            }
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
