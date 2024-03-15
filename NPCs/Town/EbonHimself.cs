using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using EbonianMod.Projectiles.VFXProjectiles;
using ReLogic.Graphics;
using Terraria.GameContent;
using EbonianMod.Projectiles;
using EbonianMod.Projectiles.Exol;
using System.Linq;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.Utilities;
using Terraria.GameContent.ItemDropRules;

using Terraria.GameContent.Personalities;
using System.Collections.Generic;
using ReLogic.Content;
using Terraria.ModLoader.IO;
using EbonianMod.NPCs.Exol;
using System.IO;
using EbonianMod.NPCs.Terrortoma;
using EbonianMod.NPCs.Garbage;

namespace EbonianMod.NPCs.Town
{
    [AutoloadHead]
    public class EbonHimself : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 10;
            NPC.Happiness.SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Love);
            NPC.Happiness.SetNPCAffection(NPCID.Wizard, AffectionLevel.Like);
            NPC.Happiness.SetNPCAffection(NPCID.GoblinTinkerer, AffectionLevel.Dislike);
            NPC.Happiness.SetNPCAffection(NPCID.BestiaryGirl, AffectionLevel.Hate);
            NPC.Happiness.SetBiomeAffection<DesertBiome>(AffectionLevel.Love);
        }
        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 38;
            NPC.height = 30;
            NPC.aiStyle = 7;
            NPC.damage = 1;
            NPC.defense = 99999;
            NPC.lifeMax = 999999;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,
                new FlavorTextBestiaryInfoElement("Type: Ebon"),
                new FlavorTextBestiaryInfoElement("Someone once decided to put an Agal on a cat, and it all went downhill from that point on")
            });
        }
        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            return Main.BestiaryTracker.Kills.GetKillCount(ModContent.NPCType<HotGarbage>().ToString()) > 0;
        }
        public override ITownNPCProfile TownNPCProfile()
        {
            return new EbonProfile();
        }
        public override List<string> SetNPCNameList()
        {
            return new List<string>
            {
                "Ebon"
            };
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Y < -1 || NPC.velocity.Y > 1)
            {
                NPC.frame.Y = frameHeight * 2;
            }
            else if (NPC.velocity.X > 0 || NPC.velocity.X < 0)
            {
                if (++NPC.frameCounter % 5 == 0)
                {
                    if (NPC.frame.Y < 9 * frameHeight)
                        NPC.frame.Y += frameHeight;
                    else if (NPC.frame.Y >= 9 * frameHeight || NPC.frame.Y < 2 * frameHeight)
                        NPC.frame.Y = 2 * frameHeight;
                }
            }
            else
                NPC.frame.Y = 0;
        }
        public override string GetChat()
        {
            if (!MetBefore)
            {
                MetBefore = true;
                return "Alsalam Alaykum brother";
            }
            WeightedRandom<string> chat = new WeightedRandom<string>();
            chat.Add("Visuals a problem? Additive blending's the solution!");
            chat.Add("I've been thinking about making a mod called \"Crimulan Mod\", I think it'd be pretty cool, idk.");
            chat.Add("uhhhhh, what do I name the boss I'm making? I'm thinking Phantasmal Spirit but that's pretty lame");
            chat.Add("This Cuh so Intelligent");
            chat.Add("yes yes, he should say \"It is I, Duke Rainbow Von Ran-bow the Third\" that'd be quite funny.");
            chat.Add("So you're telling me that a dumpster is not valid boss design? how about you kill yourself?");
            chat.Add("OH MY EXOL IS THAT EBON EBONIAN MOD!?");
            chat.Add("Uniquest of AIs");
            if (NPC.AnyNPCs(NPCID.BestiaryGirl))
                chat.Add("God I hate " + Main.npc[NPC.FindFirstNPC(NPCID.BestiaryGirl)].GivenName + ", LIKE AW MY GOSH!");
            chat.Add("so uh, i might just kill the mod, I'm just kinda losing interest in terraria modding and terraria itself in general and this is just getting boring. I might finish this update before killing the mod but for now I'm planning on just killing it.");
            chat.Add("Would i be milking it if i made duke rainbow the 1st?");
            chat.Add("How about you code in some women, or maybe resprite some grass.");
            return chat;
        }
        public int Option;
        public int[] TimesAskedAbout = new int[20];
        public bool MetBefore;
        public override void AI()
        {
            NPC.direction = NPC.spriteDirection = Main.LocalPlayer.Center.X > NPC.Center.X ? 1 : -1;
        }
        public override bool CheckDead()
        {
            CombatText.NewText(NPC.getRect(), Color.Red, "a mod developer does not fear death");
            NPC.life = NPC.lifeMax;
            return false;
        }
        public override void SetChatButtons(ref string button, ref string button2)
        {
            switch (Option)
            {
                case 0:
                    button = Language.GetTextValue("LegacyInterface.28");
                    break;
                case 1:
                    button = "Arabic?";
                    break;
                case 2:
                    button = "Who's Ebon?";
                    break;
                case 3:
                    button = "Who's Sten?";
                    break;
                case 4:
                    button = "Who's Max?";
                    break;
                case 5:
                    button = "Who's Yuri O?";
                    break;
                case 6:
                    button = "Who's Theonetruedog?";
                    break;
                case 7:
                    button = "Who's Decrypt?";
                    break;
                case 8:
                    button = "Who's Dreadsoul?";
                    break;
                case 9:
                    button = "Who's Lunatic? (Aka Strat Strat, Stratis, or Lunaitok)";
                    break;
                case 10:
                    button = "Who's Vade?";
                    break;
                case 11:
                    button = "Who's Taco?";
                    break;
                case 12:
                    button = "Who's Mjoon?";
                    break;
                case 13:
                    button = "Who's Sulphuric?";
                    break;
                case 14:
                    button = "Who's Enamoured?";
                    break;
                case 15:
                    button = "Who's Juice?";
                    break;
                case 16:
                    button = "Who's Mr.Gerd26?";
                    break;
                case 17:
                    button = "Who's Roll'eG?";
                    break;
                case 18:
                    button = "Who am I?";
                    break;
                case 19:
                    button = "What's the meaning of life?";
                    break;
                case 20:
                    button = "Will it ever stop?";
                    break;
            }
            button2 = "Cycle Options";
            Main.LocalPlayer.currentShoppingSettings.HappinessReport = "";
        }
        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
            if (firstButton)
            {
                switch (Option)
                {
                    case 1:
                        Main.npcChatText = "Mashallah, ana la afhamu kalamak.";
                        break;
                    case 2:
                        Main.npcChatText = "Horrid being.";
                        break;
                    case 3:
                        Main.npcChatText = "";
                        break;
                    case 4:
                        Main.npcChatText = "";
                        break;
                    case 5:
                        Main.npcChatText = "He's a ball thief.\n\nCheck your pants.";
                        break;
                    case 6:
                        Main.npcChatText = "He's a fat lazy dog who just sits in the corner all day and occasionally burps out an idea and passes out, I think you should beat him up";
                        break;
                    case 7:
                        Main.npcChatText = "";
                        break;
                    case 8:
                        Main.npcChatText = "";
                        break;
                    case 9:
                        Main.npcChatText = "Don't let his pfp deceive you, he's not a girl.\nHe is a very great designer but very lazy too.";
                        break;
                    case 10:
                        TimesAskedAbout[2]++;
                        switch (TimesAskedAbout[2])
                        {
                            case 1:
                                Main.npcChatText = "He's some motherfucker trapped in a basement somewhere and he sprites every million years, if you see him, please note to have an ultrakill plushie or something called a 'starlad' on you at all times in hopes of survival in the case you meet him.";
                                break;
                            case 2:
                                Main.npcChatText = "Vade once said ''It's vadin time'' and vade'd all over the place, truly the vade of all time.";
                                break;
                            case 3:
                                Main.npcChatText = "Please commission him, he's poor :thumbsup:";
                                break;
                        }
                        if (TimesAskedAbout[1] >= 3)
                            Main.npcChatText = "Please commission him, he's poor :thumbsup:";
                        break;
                    case 11:
                        Main.npcChatText = "";
                        break;
                    case 12:
                        TimesAskedAbout[0]++;
                        switch (TimesAskedAbout[0])
                        {
                            case 1:
                                Main.npcChatText = "...I think the less you know about him, the better.";
                                break;
                            case 2:
                                Main.npcChatText = "I'm telling you, he's a weirdo! that's all you need to know!";
                                break;
                            case 3:
                                Main.npcChatText = "Ok, stop, fine! Mjoon's a strange guy from a weird country in the southern hemisphere, who makes art and is obsessed with cloaked entities!";
                                break;
                        }
                        if (TimesAskedAbout[0] >= 3)
                            Main.npcChatText = "Ok, stop, fine! Mjoon's a strange guy from a weird country in the southern hemisphere, who makes art and is obsessed with cloaked entities!";
                        break;
                    case 13:
                        if (Main.rand.NextBool())
                            Main.npcChatText = "Idfk he just sorta appeared";
                        else
                            Main.npcChatText = "32.99108° S, 151.65298° E";
                        break;
                    case 14:
                        TimesAskedAbout[1]++;
                        switch (TimesAskedAbout[1])
                        {
                            case 1:
                                Main.npcChatText = "ah, the wormfucker... need i say more?";
                                break;
                            case 2:
                                Main.npcChatText = "... apparently i do need say more.";
                                break;
                        }
                        if (TimesAskedAbout[1] >= 3)
                            Main.npcChatText = "dumb stupid moth.";
                        break;
                    case 15:
                        Main.npcChatText = "My lawyer advised me not to get into it.";
                        break;
                    case 16:
                        Main.npcChatText = "";
                        break;
                    case 17:
                        Main.npcChatText = "";
                        break;
                    case 18:
                        Main.npcChatText = "192.0.30.1";
                        break;
                    case 19:
                        Main.npcChatText = "Life is inherently meaningless as it stands. No, there is no one given meaning to our existence. However, there is a meaning to every individual existence that you may find. Only then will you be fulfilled. - abominble shit goblin";
                        break;
                    case 20:
                        Main.npcChatText = "No.";
                        break;
                }
                if (Option != 0)
                    return;
                shop = "Shop";
            }
            else
            {
                if (Option < 20)
                    Option++;
                else
                    Option = 0;
            }
        }
    }
    public class EbonProfile : ITownNPCProfile
    {
        public int RollVariation() => 0;
        public string GetNameForVariant(NPC npc) => npc.getNewNPCName();
        public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
        {
            return ModContent.Request<Texture2D>("EbonianMod/NPCs/Town/EbonHimself");
        }
        public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("EbonianMod/NPCs/Town/EbonHimself_Head");
    }
}
