using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.Utilities;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace EbonianMod.NPCs.Town
{
    [AutoloadHead]
    public class Thomas : ModNPC
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override void SetStaticDefaults()
        {
            NPC.Happiness.SetNPCAffection(NPCID.Wizard, AffectionLevel.Like);
            NPC.Happiness.SetNPCAffection(NPCID.WitchDoctor, AffectionLevel.Like);
            NPC.Happiness.SetNPCAffection(NPCID.Guide, AffectionLevel.Dislike);
            NPC.Happiness.SetNPCAffection(NPCID.Dryad, AffectionLevel.Hate);
            NPC.Happiness.SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Hate);
            NPC.Happiness.SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Hate);
            NPC.Happiness.SetBiomeAffection<ForestBiome>(AffectionLevel.Hate);
        }
        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 56;
            NPC.height = 52;
            NPC.lifeMax = 500;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            NPC.noGravity = true;
        }
        Vector2 dir;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            NPC.spriteDirection = NPC.direction = NPC.velocity.X < 0 ? -1 : 1;
            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.ToRotation() + (NPC.direction == -1 ? MathHelper.Pi : 0), 0.3f);
            NPC.ai[0]++;
            if (NPC.ai[0] < 90)
            {
                dir = Main.rand.NextVector2Unit();
                NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center) * 3.5f, 0.01f);
            }
            if (NPC.ai[0] > 90 && NPC.ai[0] < 300)
            {
                if (NPC.collideX || NPC.collideY)
                {
                    dir = -dir.RotatedByRandom(MathHelper.PiOver4);
                    NPC.velocity = dir * 5;
                }
                NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, NPC.Center + dir * 100) * 3.5f, 0.01f);
            }
            if (NPC.ai[0] > 300)
                NPC.velocity *= 0.9f;
            if (NPC.ai[0] > 380)
                NPC.ai[0] = 0;
        }
        public override ITownNPCProfile TownNPCProfile()
        {
            return new ThomasProfile();
        }
        public override List<string> SetNPCNameList()
        {
            return new List<string>
            {
                "Thomas"
            };
        }
        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            chat.Add("wake the dog up samurai, we got a city to burn");
            return chat;
        }
    }
    public class ThomasProfile : ITownNPCProfile
    {
        public int RollVariation() => 0;
        public string GetNameForVariant(NPC npc) => npc.getNewNPCName();
        public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
        {
            return ModContent.Request<Texture2D>("EbonianMod/NPCs/Town/Thomas");
        }
        public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("EbonianMod/NPCs/Town/Thomas_Head");
    }
}
