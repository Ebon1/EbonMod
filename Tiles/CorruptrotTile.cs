using Terraria.ModLoader;


namespace EbonianMod.Tiles
{
    public class CorruptrotTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            DustType = DustID.GreenBlood;
            HitSound = SoundID.NPCDeath1;
            RegisterItemDrop(ItemType<Items.Tiles.CorruptrotI>());

            AddMapEntry(Color.DarkGreen);
        }
    }
}
