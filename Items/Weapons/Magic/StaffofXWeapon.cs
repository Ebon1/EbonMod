using EbonianMod.Projectiles.ArchmageX;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Items.Weapons.Magic
{
    public class StaffofXWeapon : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 31;
            Item.width = 40;
            Item.height = 40;
            Item.mana = 5;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.useStyle = 5;
            Item.knockBack = 10;
            Item.value = Item.buyPrice(0, 8, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item43;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<XBoltFriendly>();
            Item.shootSpeed = 12;
        }
    }
}
