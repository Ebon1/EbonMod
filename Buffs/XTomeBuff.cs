using Terraria;
using Terraria.ModLoader;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using EbonianMod.Items.Weapons.Summoner;
namespace EbonianMod.Buffs
{
    public class XTomeBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            EbonianPlayer modPlayer = player.GetModPlayer<EbonianPlayer>();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<XTomeSummon>()] > 0)
            {
                modPlayer.xMinion = true;
            }
            if (!modPlayer.xMinion)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
            else
            {
                player.buffTime[buffIndex] = 18000;
            }
        }
    }
}