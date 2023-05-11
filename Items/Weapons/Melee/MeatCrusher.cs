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
using EbonianMod.Worldgen.Subworlds;
using SubworldLibrary;

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
            Item.noUseGraphic = false;
            Item.autoReuse = false;
            Item.DamageType = DamageClass.Melee;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = 5;
        }
        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            player.itemAnimation -= 5;
            player.itemTime -= 5;
            EbonianSystem.ScreenShakeAmount = 2;
            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Item), target.Center, new Vector2(Main.rand.NextFloat(-0.5f, 0.5f) * i, Main.rand.NextFloat(-2, -3)), ModContent.ProjectileType<VileMeatChunk>(), damage, 0, player.whoAmI);
            }
        }
    }
}
