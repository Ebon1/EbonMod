using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs
{
    public class foreskin : ModNPC
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.Derpling);
            NPC.dontTakeDamage = true;
            NPC.boss = true;
            //Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/foreskins");
        }
        public override void AI()
        {
            NPC.rotation += 0.5f;
        }
    }
}
