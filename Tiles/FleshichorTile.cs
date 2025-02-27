using Terraria.ModLoader;


namespace EbonianMod.Tiles
{
    public class FleshichorTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            DustType = DustID.Blood;
            HitSound = SoundID.NPCDeath1;
            RegisterItemDrop(ItemType<Items.Tiles.FleshichorI>());

            AddMapEntry(Color.DarkRed);
        }
    }
}
