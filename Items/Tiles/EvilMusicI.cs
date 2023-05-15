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
using Terraria.GameContent.Creative;
using EbonianMod.Tiles;
using Terraria.Utilities;

namespace EbonianMod.Items.Tiles
{
    public class EvilMusicI : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Music Box (Terrortoma & Cecitior)");
            Tooltip.SetDefault("Plays Forces of Evil by Yuri O");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/EvilMiniboss"), ModContent.ItemType<EvilMusicI>(), ModContent.TileType<EvilMusic>());
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<EvilMusic>();
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.LightRed;
            Item.value = 100000;
            Item.accessory = true;
        }
        public override bool? PrefixChance(int pre, UnifiedRandom rand)
        {
            return false;
        }
    }
}
