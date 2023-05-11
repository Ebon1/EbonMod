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
using System.IO;
using Terraria.GameContent.Bestiary;
using EbonianMod.Misc;
using System.Linq;

namespace EbonianMod.NPCs.Cecitior
{
    public class CecitiorEye : ModNPC
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<Cecitior>()], quickUnlock: true);
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Type: Organic Construct"),
                new FlavorTextBestiaryInfoElement("Ocular organs used by Cecitor. The creature is haphazardly created, leaving many flaws in its existence, such as the lack of defended eyes.\n\nOne has to wonder how much of a pain it would be to get lemon there."),
            });
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gazer");
            Main.npcFrameCount[NPC.type] = 19;
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 1000;
            NPC.dontTakeDamage = false;
            NPC.damage = 0;
            NPC.noTileCollide = true;
            NPC.defense = 10;
            NPC.knockBackResist = 0;
            NPC.width = 32;
            NPC.height = 32;
            NPC.npcSlots = 1f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.buffImmune[24] = true;
            NPC.netAlways = true;
        }
        Verlet verlet;
        int timer;
        float randRot;
        public override void OnSpawn(IEntitySource source)
        {
            randRot = Main.rand.NextFloat(MathHelper.Pi * 2);
            timer = Main.rand.Next(60);
            NPC.frame.Y = NPC.frame.Height * 13;
            verlet = new(NPC.Center, 7, 16, 1, true, true, 13);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC center = Main.npc[(int)NPC.ai[0]];
            if (NPC.IsABestiaryIconDummy || !center.active || center.type != ModContent.NPCType<Cecitior>())
                return true;

            if (verlet != null)
            {
                if (leftSiders.Contains((int)NPC.ai[1]))
                    verlet.Update(NPC.Center, new Vector2(center.localAI[0], center.localAI[1]));
                else
                    verlet.Update(NPC.Center, center.Center);
                verlet.gravity = (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.25f;
                verlet.Draw(spriteBatch, "NPCs/Cecitior/CecitiorChain");
            }
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC center = Main.npc[(int)NPC.ai[0]];
            if (NPC.IsABestiaryIconDummy || !center.active || center.type != ModContent.NPCType<Cecitior>())
                return;
            Texture2D a = Helper.GetTexture("NPCs/Cecitior/CecitiorChain_base");
            Texture2D b = Helper.GetTexture("NPCs/Cecitior/CecitiorEye");
            if (verlet != null)
                spriteBatch.Draw(a, verlet.firstP.position - new Vector2(0, 20).RotatedBy(Helper.FromAToB(verlet.firstP.position, verlet.points[5].position, reverse: true).ToRotation() - 1.57f) - screenPos, null, drawColor, Helper.FromAToB(verlet.firstP.position, verlet.points[5].position, reverse: true).ToRotation() - 1.57f, a.Size() / 2, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(b, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
            {

                ++NPC.frameCounter;
                if (NPC.frameCounter % 5 == 0 && NPC.frameCounter < 550)
                {
                    if (NPC.frame.Y < frameHeight * 15)
                        NPC.frame.Y += frameHeight;
                    else
                        NPC.frame.Y = 0;
                }
                if (NPC.frameCounter % 5 == 0 && NPC.frameCounter > 550)
                {
                    if (NPC.frame.Y < frameHeight * 15)
                        NPC.frame.Y += frameHeight;
                    else
                        NPC.frame.Y = frameHeight * 13;
                }
                if (NPC.frameCounter > 650)
                    NPC.frameCounter = 0;
            }
            if (timer < 0)
            {
                NPC center = Main.npc[(int)NPC.ai[0]];
                if (center.ai[3] == 1) //frantically looking
                {
                    ++NPC.frameCounter;
                    if (NPC.frameCounter % 5 == 0 && NPC.frameCounter < 550)
                    {
                        if (NPC.frame.Y < frameHeight * 15)
                            NPC.frame.Y += frameHeight;
                        else
                            NPC.frame.Y = 0;
                    }
                    if (NPC.frameCounter % 5 == 0 && NPC.frameCounter > 550)
                    {
                        if (NPC.frame.Y < frameHeight * 15)
                            NPC.frame.Y += frameHeight;
                        else
                            NPC.frame.Y = frameHeight * 13;
                    }
                    if (NPC.frameCounter > 650)
                        NPC.frameCounter = 0;
                }
                else //stare at player
                {
                    ++NPC.frameCounter;
                    if (NPC.frameCounter % 5 == 0)
                    {
                        if (NPC.frame.Y < frameHeight * 18 && NPC.frame.Y >= frameHeight * 16)
                            NPC.frame.Y += frameHeight;
                        else if (NPC.frame.Y < frameHeight * 16 || NPC.frame.Y >= frameHeight * 18)
                            NPC.frame.Y = 16 * frameHeight;
                    }
                }
            }
        }

        public float Index
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }
        public float AIState
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }
        public float AITimer
        {
            get => NPC.ai[3];
            set => NPC.ai[3] = value;
        }
        int[] leftSiders = new int[] { 0, 5, 4 };
        public override void AI()
        {
            timer--;
            NPC center = Main.npc[(int)NPC.ai[0]];
            Player player = Main.player[center.target];
            if (!center.active || center.type != ModContent.NPCType<Cecitior>())
            {
                NPC.life = 0;
                return;
            }
            if (center.ai[3] == 1)
            {
                NPC.rotation = randRot;
                if (leftSiders.Contains((int)NPC.ai[1]))
                {
                    float angle = Helper.CircleDividedEqually(NPC.ai[1], 6) + MathHelper.ToRadians(15);
                    NPC.velocity = Helper.FromAToB(NPC.Center, new Vector2(center.localAI[0], center.localAI[1]) + new Vector2(100).RotatedBy(angle), false) / 10f;
                }
            }
            else
            {
                NPC.velocity = Vector2.Zero; // REMOVE LATER    
                NPC.rotation = Helper.FromAToB(NPC.Center, player.Center).ToRotation() + MathHelper.Pi;
            }
            switch (center.ai[0])
            {
                case 1:
                    float angle = Helper.CircleDividedEqually(NPC.ai[1], 6) + MathHelper.ToRadians(15);
                    NPC.velocity = Helper.FromAToB(NPC.Center, center.Center + new Vector2(100).RotatedBy(angle), false) / 10f;
                    break;
            }
        }
    }
}
