using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;
using EbonianMod.Projectiles.Dev;

namespace EbonianMod.Buffs
{
    public class RollegB : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Neko Shroomie");
            Description.SetDefault("Ooaoaaoa mmm))");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<EbonianPlayer>().rolleg = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Rolleg>()] > 0)
                player.buffTime[buffIndex] = 2;
        }
    }
}
