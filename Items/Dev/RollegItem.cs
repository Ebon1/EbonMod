using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;
using EbonianMod.Items.Weapons.Ranged;
using EbonianMod.Projectiles.Dev;
using EbonianMod.Buffs;

namespace EbonianMod.Items.Dev
{
    public class RollegItem : ModItem
    {
        public override string Texture => Helper.Placeholder;
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = DamageClass.Summon;
            Item.useTime = Item.useAnimation = 30;
            Item.shoot = ModContent.ProjectileType<Rolleg>();
            Item.shootSpeed = 0f;
            Item.UseSound = new("EbonianMod/Sounds/rolleg");
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.autoReuse = false;
            Item.buffType = ModContent.BuffType<RollegB>();
            Item.rare = 7;
            Item.buffTime = 69; //LMAO HAHA XD
        }
    }
}
