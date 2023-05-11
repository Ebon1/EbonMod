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
using Terraria.GameContent;
using static System.Formats.Asn1.AsnWriter;
using SubworldLibrary;
using EbonianMod.Items.Weapons.Melee;
using EbonianMod.Worldgen.Subworlds;

namespace EbonianMod.NPCs.Cecitior
{
    [AutoloadBossHead]
    public class Cecitior : ModNPC
    {
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Type: Organic Construct"),
                new FlavorTextBestiaryInfoElement("A construct of flesh made from the remnants of the Brain of Cthulhu, Wall of Flesh, and many other crimson creatures. It appears to be an attempt to respond to the threat you pose to the crimson, with less than successful results."),
            });
        }
        public override void SetStaticDefaults()
        {
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.ShouldBeCountedAsBoss[Type] = true;
            Main.npcFrameCount[NPC.type] = 7;
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 4750;
            NPC.damage = 40;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.defense = 20;
            NPC.knockBackResist = 0;
            NPC.width = 118;
            NPC.height = 100;
            NPC.rarity = 999;
            NPC.npcSlots = 1f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.boss = false;
            SoundStyle hit = new("EbonianMod/Sounds/NPCHit/TerrorHit");
            SoundStyle death = new("EbonianMod/Sounds/NPCHit/TerrorDeath");
            NPC.HitSound = hit;
            NPC.DeathSound = death;
            NPC.buffImmune[24] = true;
            NPC.netAlways = true;
            //NPC.alpha = 255;
        }
        Verlet[] verlet = new Verlet[5];
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 5; i++)
            {
                float scale = 2;
                switch (i)
                {
                    case 1:
                        scale = 4;
                        break;
                    case 2:
                        scale = 2;
                        break;
                    case 3:
                        scale = 3;
                        break;
                    case 4:
                        scale = 1;
                        break;
                }
                verlet[i] = new(NPC.Center, 2, 20, 1 * scale, true, true, (int)(5 * scale));
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (verlet[0] != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 offset = Vector2.Zero;
                    Vector2 offset2 = Vector2.Zero;
                    float scale = 2;
                    switch (i)
                    {
                        case 1:
                            scale = 4;
                            offset = new Vector2(4, 28);
                            offset2 = new Vector2(-7, -10);
                            break;
                        case 2:
                            scale = 2;
                            offset = new Vector2(10, -20);
                            offset2 = new Vector2(1, 20);
                            break;
                        case 3:
                            scale = 3;
                            offset = new Vector2(-15, -10);
                            offset2 = new Vector2(10, -20);
                            break;
                        case 4:
                            scale = 1;
                            offset = new Vector2(1, 20);
                            offset2 = new Vector2(4, 32);
                            break;
                    }
                    verlet[i].Update(NPC.Center + offset2, NPC.Center + openOffset + new Vector2(30, 4) + offset);
                }
            }
            if (openOffset.Length() > 1 || openOffset.Length() < -1)
            {
                Texture2D tex = TextureAssets.Npc[Type].Value;
                Texture2D teeth = Helper.GetTexture("NPCs/Cecitior/CecitiorTeeth");
                Texture2D partTeeth = Helper.GetTexture("NPCs/Cecitior/CecitiorTeeth2");
                spriteBatch.Draw(teeth, NPC.Center - new Vector2(0, -2) - screenPos, null, drawColor, NPC.rotation, teeth.Size() / 2, NPC.scale, SpriteEffects.None, 0);
                spriteBatch.Draw(partTeeth, NPC.Center + new Vector2(30, 4) + openOffset - screenPos, null, drawColor, openRotation, partTeeth.Size() / 2, NPC.scale, SpriteEffects.None, 0);
                if (verlet[0] != null)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 offset = Vector2.Zero;
                        Vector2 offset2 = Vector2.Zero;
                        float scale = 2;
                        switch (i)
                        {
                            case 1:
                                scale = 4;
                                offset = new Vector2(4, 28);
                                offset2 = new Vector2(-7, -10);
                                break;
                            case 2:
                                scale = 2;
                                offset = new Vector2(10, -20);
                                offset2 = new Vector2(1, 20);
                                break;
                            case 3:
                                scale = 3;
                                offset = new Vector2(-15, -10);
                                offset2 = new Vector2(10, -20);
                                break;
                            case 4:
                                scale = 1;
                                offset = new Vector2(1, 20);
                                offset2 = new Vector2(4, 32);
                                break;
                        }
                        //verlet[i].Update(NPC.Center + offset2, NPC.Center + openOffset + new Vector2(30, 4) + offset);
                        verlet[i].Draw(spriteBatch, "Extras/Line", useColor: true, color: Color.Maroon * 0.25f, scale: scale);
                    }
                }
                Texture2D part = Helper.GetTexture("NPCs/Cecitior/Cecitior_Part");
                spriteBatch.Draw(part, NPC.Center + new Vector2(30, 4) + openOffset - screenPos, null, drawColor, openRotation, part.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            }
            return true;
        }
        public override void FindFrame(int frameHeight)
        {
            if (openOffset.Length() > 1.5f || openOffset.Length() < -1.5f)
                NPC.frame.Y = frameHeight * 6;
            else
            if (++NPC.frameCounter % 5 == 0)
            {
                if (NPC.frame.Y < frameHeight * 5)
                    NPC.frame.Y += frameHeight;
                else
                    NPC.frame.Y = 0;
            }
        }
        float rotation, openRotation;
        bool open;
        int eyeType = ModContent.NPCType<CecitiorEye>();
        Vector2 openOffset;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(openOffset);
            writer.Write(open);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(openRotation);
            writer.Write(rotation);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            openOffset = reader.ReadVector2();
            open = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            openRotation = reader.ReadSingle();
            rotation = reader.ReadSingle();
        }
        public float AIState
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        public float AITimer
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }
        private float AITimer2 = 0;
        const int Death = -1, Intro = 0, Idle = 1, EyeBehaviour = 2, Chomp = 3, Dash = 4, Teeth = 5, PISS = 6, DualSlamEachPiece = 7, ThrowUpBlood = 8;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (AIState != Intro && AIState != Death && !NPC.AnyNPCs(eyeType))
            {
                AITimer = Death;
                AITimer = 0;
                AITimer2 = 0;
            }
            if (!player.active || player.dead)// || !player.ZoneCorrupt)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (NPC.HasValidTarget)
                {
                    AIState = Intro;
                    AITimer = 0;
                }
                if (!player.active || player.dead)// || !player.ZoneCrimson)
                {
                    AIState = -12124;
                    NPC.velocity = new Vector2(0, 10f);
                    if (NPC.timeLeft > 60)
                    {
                        NPC.timeLeft = 60;
                    }
                    return;
                }
            }
            NPC.localAI[0] = NPC.Center.X + openOffset.X;
            NPC.localAI[1] = NPC.Center.Y + openOffset.Y;
            if (open)
            {
                NPC.ai[3] = 1; //letting the eyes know its open.
                NPC.damage = 0;
                openOffset += Vector2.UnitX * 4;
                if (player.Center.Distance(NPC.Center) < 75 || player.Center.Distance(NPC.Center + openOffset) < 75)
                    player.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), 20, 0);
            }
            else
            {
                NPC.ai[3] = 0;
                openOffset = Vector2.Lerp(openOffset, Vector2.Zero, 0.4f);
                NPC.damage = 40;
                if ((openOffset.Length() < 2.5f && openOffset.Length() > 1f) || (openOffset.Length() > -2.5f && openOffset.Length() < -1f))
                    EbonianSystem.ScreenShakeAmount = 5;
            }
            open = Main.mouseRight;
            openRotation = NPC.rotation;
            NPC.rotation = MathHelper.Lerp(NPC.rotation, rotation, 0.35f);
            if (AIState == Intro)
            {
                AITimer++;
                if (AITimer == 1)
                {
                    NPC.boss = true;
                    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/EvilMiniboss");
                    Helper.SetBossTitle(120, "Cecitior", Color.Gold, "Sightless Carcass", 0);
                    EbonianSystem.ChangeCameraPos(NPC.Center, 120);
                    for (int i = 0; i < 6; i++)
                    {
                        float angle = Helper.CircleDividedEqually(i, 6) + MathHelper.ToRadians(15);
                        NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.Center + new Vector2(100).RotatedBy(angle), ModContent.NPCType<CecitiorEye>(), 0, NPC.whoAmI, i);
                    }
                }
                if (AITimer >= 160)
                {
                    AITimer = 0;
                    AIState = Idle;
                }
            }
            else if (AIState == Idle)
            {
                AITimer++;
                NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 100), false) / 20f;
                if (AITimer >= 230)
                {
                    AIState = EyeBehaviour;
                    NPC.velocity = Vector2.Zero;
                    AITimer = 0;
                }
            }
            else if (AIState == EyeBehaviour)
            {

            }
        }
    }
}
