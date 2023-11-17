using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;
using EbonianMod.Dusts;

namespace EbonianMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Face)]
    public class ReiMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Face.Sets.PreventHairDraw[Item.faceSlot] = true;
        }
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = ItemRarityID.Lime;
        }
        public override void UpdateVanity(Player player)
        {
            EbonianPlayer modPlayer = player.GetModPlayer<EbonianPlayer>();
            modPlayer.reiV = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ReiCapeP>()] < 1)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<ReiCapeP>(), 0, 0, player.whoAmI);
            }
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ReiCapeTrail>()] < 1)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<ReiCapeTrail>(), 0, 0, player.whoAmI);
            }
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            EbonianPlayer modPlayer = player.GetModPlayer<EbonianPlayer>();
            modPlayer.rei = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ReiCapeP>()] < 1)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<ReiCapeP>(), 0, 0, player.whoAmI);
            }
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ReiCapeTrail>()] < 1)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<ReiCapeTrail>(), 0, 0, player.whoAmI);
            }
        }
    }
}
