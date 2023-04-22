using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using EbonianMod.Projectiles.Friendly.Corruption;
using Terraria.DataStructures;
using EbonianMod.Projectiles.Exol;
using EbonianMod.Projectiles.VFXProjectiles;
using Terraria.Audio;
using EbonianMod.Projectiles.Friendly.Crimson;

namespace EbonianMod.Items.Weapons.Magic
{
    public class Latcher : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Reels you into tiles and heavy enemies, reels light enemies towards you\nRight click to cancel.");
        }
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Harpoon);
            Item.shoot = ModContent.ProjectileType<LatcherP>();
            Item.DamageType = DamageClass.Magic;
            Item.mana = 25;
            Item.shootSpeed = 20;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<LatcherP>()] < 1;
        }
    }
}
