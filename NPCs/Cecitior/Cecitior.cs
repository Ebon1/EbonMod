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
//using SubworldLibrary;
using EbonianMod.Items.Weapons.Melee;
//using EbonianMod.Worldgen.Subworlds;
using EbonianMod.Projectiles.Cecitior;
using EbonianMod.Projectiles.Friendly.Crimson;
using System.Reflection.Metadata;
using EbonianMod.Projectiles;
using ReLogic.Utilities;

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
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
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
            SoundStyle hit = new("EbonianMod/Sounds/NPCHit/fleshHit");
            SoundStyle death = new("EbonianMod/Sounds/NPCHit/cecitiordie");
            NPC.HitSound = hit;
            NPC.DeathSound = death;
            NPC.buffImmune[24] = true;
            NPC.netAlways = true;
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
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
                verlet[i] = new(NPC.Center, 2, 18, 1 * scale, true, true, (int)(2.5f * scale));
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
                    verlet[i].Update(NPC.Center + offset2 - openOffset, NPC.Center + openOffset + new Vector2(30, 4) + offset);
                    if (verlet[i].segments[10].cut)
                        verlet[i].Draw(spriteBatch, "Extras/Line", useColor: true, color: Color.Maroon * 0.25f * VerletAlpha, scale: scale);
                }
            }
            if (NPC.frame.Y == 6 * 102)//(openOffset.Length() > 0.25f || openOffset.Length() < -0.25f || openRotation != 0)
            {
                Texture2D tex = TextureAssets.Npc[Type].Value;
                Texture2D teeth = Helper.GetTexture("NPCs/Cecitior/CecitiorTeeth");
                Texture2D partTeeth = Helper.GetTexture("NPCs/Cecitior/CecitiorTeeth2");
                spriteBatch.Draw(teeth, NPC.Center - openOffset - new Vector2(0, -2) - screenPos, null, drawColor, NPC.rotation, teeth.Size() / 2, NPC.scale, SpriteEffects.None, 0);
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
                        if (!verlet[i].segments[10].cut)
                            verlet[i].Draw(spriteBatch, "Extras/Line", useColor: true, color: Color.Maroon * 0.25f * VerletAlpha, scale: scale);
                    }
                }
                Texture2D part = Helper.GetTexture("NPCs/Cecitior/Cecitior_Part");
                spriteBatch.Draw(part, NPC.Center + new Vector2(30, 4) + openOffset - screenPos, null, drawColor, openRotation, part.Size() / 2, NPC.scale, SpriteEffects.None, 0);
                spriteBatch.Draw(tex, NPC.Center - openOffset - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
                return false;
            }
            return true;
        }
        public override void FindFrame(int frameHeight)
        {
            if (openOffset.Length() > 1 || openOffset.Length() < -1 || openRotation != 0)
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
            writer.Write(lastAi);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            openOffset = reader.ReadVector2();
            open = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            openRotation = reader.ReadSingle();
            rotation = reader.ReadSingle();
            lastAi = reader.Read();
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
        float VerletAlpha = 1;
        private float AITimer2 = 0;
        int lastAi;
        Projectile tongue = null;
        const int PrePreDeath = -3, Death = -2, PreDeath = -1, Intro = 0, Idle = 1, EyeBehaviour = 2, Chomp = 3, Teeth = 4, EyeBehaviour2 = 5, LaserRain = 6, ThrowUpBlood = 7, Tongue = 8;
        SlotId cachedSound;
        Vector2 savedPos;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (AIState != Intro && AIState != PreDeath && AIState != PrePreDeath && AIState != Death && !NPC.AnyNPCs(eyeType))
            {
                savedPos = NPC.Center;
                NPC.velocity = Vector2.Zero;
                if (tongue != null)
                    tongue.Kill();
                AIState = PrePreDeath;
                AITimer = 0;
                AITimer2 = 0;
                EbonianSystem.ChangeCameraPos(NPC.Center, 180);
            }
            SoundStyle selected = EbonianMod.flesh0;
            switch (Main.rand.Next(3))
            {
                case 0:
                    selected = EbonianMod.flesh1;
                    break;
                case 1:
                    selected = EbonianMod.flesh2;
                    break;
            }
            if (!cachedSound.IsValid || !SoundEngine.TryGetActiveSound(cachedSound, out var activeSound) || !activeSound.IsPlaying)
            {
                cachedSound = SoundEngine.PlaySound(selected, NPC.Center);
            }
            int eyeCount = 0;
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.type == eyeType)
                    eyeCount++;
            }
            bool p2 = eyeCount <= 3;
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
            NPC.localAI[0] = openOffset.X;
            NPC.localAI[1] = openOffset.Y;
            if (open)
            {
                NPC.ai[3] = 1; //letting the eyes know its open.
                NPC.damage = 0;
            }
            else
            {
                NPC.ai[3] = 0;
                if (!(AIState == Chomp && AITimer2 == 1) && !(AIState == PreDeath && (AITimer2 == 1 || AITimer2 == 3)))
                    openOffset = Vector2.Lerp(openOffset, Vector2.Zero, 0.5f);
                //NPC.damage = 40;
                if ((openOffset.Length() < 2.5f && openOffset.Length() > 1f) || (openOffset.Length() > -2.5f && openOffset.Length() < -1f))
                {
                    SoundEngine.PlaySound(new SoundStyle("EbonianMod/Sounds/chomp" + Main.rand.Next(2)), NPC.Center);
                    SoundEngine.PlaySound(new SoundStyle("EbonianMod/Sounds/NPCHit/fleshHit"), NPC.Center);
                    EbonianSystem.ScreenShakeAmount = 5;
                }
                if (openOffset != Vector2.Zero && AIState != ThrowUpBlood && AIState != LaserRain && NPC.frame.Y == 6 * 102)
                    if (player.Center.Distance(NPC.Center - openOffset) < 75 || player.Center.Distance(NPC.Center + openOffset) < 75)
                        player.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), 20, 0);
            }
            //open = Main.mouseRight;
            //openRotation = NPC.rotation;
            NPC.rotation = MathHelper.Lerp(NPC.rotation, rotation, 0.35f);
            if (AIState == Death)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + openOffset, Vector2.UnitX * 5, ModContent.Find<ModGore>("EbonianMod/Cecitior1").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, -Vector2.UnitX * 5, ModContent.Find<ModGore>("EbonianMod/Cecitior2").Type, NPC.scale);

                NPC.dontTakeDamage = false;
                NPC.life = 0;
                NPC.checkDead();
            }
            else if (AIState == PrePreDeath)
            {
                if (rotation == 0 || AITimer > 0)
                    AITimer++;
                if (AITimer == 1)
                    open = false;
                if (AITimer == 0 || AITimer > 150)
                {
                    rotation = MathHelper.Lerp(rotation, 0, 0.3f);
                    openRotation = MathHelper.Lerp(openRotation, 0, 0.3f);
                    openOffset = Vector2.Lerp(openOffset, Vector2.Zero, 0.3f);
                }
                if (AITimer >= 100 && AITimer < 115)
                {
                    openOffset.X++;
                    openRotation += MathHelper.ToRadians(2);
                    rotation -= MathHelper.ToRadians(2);
                }
                if (AITimer > 115 && AITimer < 160)
                {
                    if (AITimer % 5 == 0)
                    {
                        Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.NextFloat(-1, 1), -1) * Main.rand.NextFloat(1, 15), 814, 10, 0);
                    }
                    if (AITimer % (AITimer < 130 ? 2 : 0) == 0)
                        for (int i = 0; i < 3; i++)
                        {
                            Dust.NewDustPerfect(NPC.Center - openOffset, DustID.Blood, new Vector2(Main.rand.NextFloat(0, 1), -1) * Main.rand.NextFloat(1, 15), Scale: Main.rand.NextFloat(1, 2)).noGravity = false;
                            Dust.NewDustPerfect(NPC.Center + openOffset, DustID.Blood, new Vector2(Main.rand.NextFloat(-1, 0), -1) * Main.rand.NextFloat(1, 15), Scale: Main.rand.NextFloat(1, 2)).noGravity = false;
                        }
                }
                NPC.Center = savedPos;
                if (AITimer < 100 && AITimer > 0)
                    NPC.Center += Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, (20 - AITimer * 0.2f).Safe());
                if (AITimer > 170)
                {
                    AIState = PreDeath;
                    AITimer = 0;
                    savedPos = NPC.Center;
                }
            }
            else if (AIState == PreDeath)
            {
                AITimer++;
                if (open)
                {
                    Dust.NewDustPerfect(NPC.Center - openOffset, DustID.Blood, new Vector2(Main.rand.NextFloat(0, 1), Main.rand.NextFloat(-0.5f, 0.5f)).RotatedBy(openRotation) * Main.rand.NextFloat(1, 10), Scale: Main.rand.NextFloat(1, 2));
                    Dust.NewDustPerfect(NPC.Center + openOffset, DustID.Blood, new Vector2(Main.rand.NextFloat(-1, 0), Main.rand.NextFloat(-0.5f, 0.5f)).RotatedBy(openRotation) * Main.rand.NextFloat(1, 10), Scale: Main.rand.NextFloat(1, 2));
                    if (AITimer2 == 0 || AITimer2 == 2)
                        openOffset += Vector2.UnitX * (4 + (AITimer2 * 0.5f - 1));
                    else
                        openOffset += Vector2.UnitY * (4 + (AITimer2 * 0.5f - 1));
                }
                if ((AITimer < 30 && AITimer >= 0) || (AITimer < 0 && AITimer % 5 == 0))
                {
                    open = true;
                    if (AITimer2 == 1 || AITimer2 == 3)
                    {
                        openRotation = MathHelper.Lerp(openRotation, MathHelper.ToRadians(90), 0.5f);
                        rotation = MathHelper.Lerp(rotation, MathHelper.ToRadians(90), 0.5f);
                    }
                    if (AITimer2 == 0)
                        NPC.velocity = Helper.FromAToB(NPC.Center, player.Center, false) / 10f;
                }
                if (AITimer >= 30 && AITimer < 70)
                {
                    if (AITimer2 == 0 || AITimer2 == 2)
                        NPC.velocity = Helper.FromAToB(NPC.Center, player.Center, false) / 5f;
                    else
                        NPC.velocity = Helper.FromAToB(NPC.Center, player.Center, false) / 5f;
                }
                if (AITimer == 70)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - openOffset, Vector2.Zero, ModContent.ProjectileType<BloodShockwave>(), 0, 0);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + openOffset, Vector2.Zero, ModContent.ProjectileType<BloodShockwave>(), 0, 0);
                    //SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center + openOffset);
                    NPC.velocity = Vector2.Zero;
                    savedPos = NPC.Center;
                }
                if (AITimer <= 80 && AITimer > 70)
                {
                    Vector2 offset = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, (20 - AITimer2 * 3).Safe());

                    NPC.Center = savedPos;
                    NPC.Center += offset;
                }
                if (AITimer == 80)
                {
                    open = false;
                    EbonianSystem.ScreenShakeAmount = AITimer2.Safe(0.5f) * 1.25f;
                    SoundEngine.PlaySound(new SoundStyle("EbonianMod/Sounds/chomp" + Main.rand.Next(2)), NPC.Center);
                    for (int i = 0; i < 15 + AITimer2 * 3; i++)
                        Dust.NewDustPerfect(NPC.Center, DustID.TreasureSparkle, Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 10));
                    for (int i = 0; i < 10 + AITimer2 * 3; i++)
                        Dust.NewDustPerfect(NPC.Center, DustID.Blood, Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 20), Scale: 2);
                }
                if (AITimer2 == 1 || AITimer2 == 3)
                {
                    if (openOffset.Y < 50 && AITimer > 30)
                    {
                        openRotation = 0;
                        //rotation = 0;
                        open = false;
                        NPC.frame.Y = 0;
                        openOffset = Vector2.Zero;
                    }
                }
                if (AITimer > 80)
                {
                    openOffset.Y = MathHelper.Lerp(openOffset.Y, 0, 0.35f);
                    //NPC.Center = Vector2.Lerp(NPC.Center, NPC.Center + openOffset, 0.45f);
                }
                if (AITimer >= 86)
                {
                    openRotation = 0;
                    rotation = 0;
                    if (AITimer2 < 3)
                    {
                        if (verlet[0] != null)
                        {
                            verlet[(int)AITimer2].segments[10].cut = true;
                            verlet[(int)AITimer2].stiffness = 1;
                        }
                        if (AITimer2 == 2)
                            AITimer = -30;
                        else
                            AITimer = 0;
                        AITimer2++;
                        savedPos = NPC.Center;
                        NPC.velocity = Vector2.Zero;
                    }
                    else
                    {
                        AITimer2 = 0;
                        AIState = Death;
                        NPC.velocity = Vector2.Zero;
                        AITimer = 0;
                    }
                }
            }
            else if (AIState == Intro)
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
                if (AITimer == 1 && AITimer2 == 0 && Main.rand.NextBool(3))
                {
                    AITimer = -250;
                    AITimer2 = 1;
                }
                NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 100), false) / 45f;
                if (AITimer >= 50)
                {
                    if (NPC.ai[2] == 1)
                    {
                        int rand = Main.rand.Next(2, 9);
                        int attempts = 0;
                        while (rand == lastAi && attempts < 75)
                        {
                            rand = Main.rand.Next(2, 9);
                            attempts++;
                        }
                        AIState = rand;
                    }
                    else
                        AIState = EyeBehaviour;
                    NPC.velocity = Vector2.Zero;
                    AITimer = 0;
                    AITimer2 = 0;
                }
            }
            else if (AIState == EyeBehaviour)
            {
                lastAi = (int)AIState;
                AITimer++;
                NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 350), false) / 20f;
                if (AITimer >= 220 - (p2 ? 121 : 0))
                {
                    if (NPC.ai[2] == 1)
                        AIState = Idle;
                    else
                        AIState = Chomp;
                    NPC.velocity = Vector2.Zero;
                    AITimer = 0;
                }
            }
            else if (AIState == Chomp)
            {
                lastAi = (int)AIState;
                AITimer++;
                if (open)
                {
                    if (AITimer2 == 1)
                        openOffset += Vector2.UnitY * 4;
                    else
                        openOffset += Vector2.UnitX * 4;
                }
                if (AITimer < 50)
                {
                    open = true;
                    if (AITimer2 == 1)
                    {
                        openRotation = MathHelper.Lerp(openRotation, MathHelper.ToRadians(90), 0.5f);
                        rotation = MathHelper.Lerp(rotation, MathHelper.ToRadians(90), 0.5f);
                    }
                    NPC.velocity = Helper.FromAToB(NPC.Center, player.Center, false) / 10f;
                }
                if (AITimer >= 50 && AITimer < 100)
                {
                    if (AITimer2 == 1)
                        NPC.velocity = Helper.FromAToB(NPC.Center, player.Center, false) / 5f;
                    else
                        NPC.velocity = Helper.FromAToB(NPC.Center, player.Center, false) / 5f;
                }
                if (AITimer == 100)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - openOffset, Vector2.Zero, ModContent.ProjectileType<BloodShockwave>(), 0, 0);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + openOffset, Vector2.Zero, ModContent.ProjectileType<BloodShockwave>(), 0, 0);
                    //SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center + openOffset);
                    NPC.velocity = Vector2.Zero;
                }
                if (AITimer == 115)
                {
                    open = false;
                }
                if (AITimer > 115)
                {
                    openOffset.Y = MathHelper.Lerp(openOffset.Y, 0, 0.5f);
                    //NPC.Center = Vector2.Lerp(NPC.Center, NPC.Center + openOffset, 0.6f);
                }
                if (AITimer2 == 1)
                {
                    if (openOffset.Y < 50 && AITimer > 50)
                    {
                        openRotation = 0;
                        //rotation = 0;
                        open = false;
                        NPC.frame.Y = 0;
                        openOffset = Vector2.Zero;
                    }
                }
                if (AITimer >= 135)
                {
                    openRotation = 0;
                    rotation = 0;
                    if (AITimer2 == 0 && p2)
                    {
                        AITimer2++;
                        AITimer = 30;
                        NPC.velocity = Vector2.Zero;
                    }
                    else
                    {
                        AITimer2 = 0;
                        if (NPC.ai[2] == 1)
                            AIState = Idle;
                        else
                            AIState = Teeth;
                        NPC.velocity = Vector2.Zero;
                        AITimer = 0;
                    }
                }
            }
            else if (AIState == Teeth)
            {
                lastAi = (int)AIState;
                AITimer++;
                if (AITimer < 20)
                    NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 200), false) / 10f;
                if (AITimer == 20)
                {
                    NPC.velocity = Vector2.Zero;
                    for (int i = 0; i < 15; i++)
                    {
                        float angle = Helper.CircleDividedEqually(i, 15);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(5).RotatedBy(angle), ModContent.ProjectileType<CecitiorTeeth>(), 15, 0);
                    }
                }
                if (AITimer == 30 && p2)
                {
                    NPC.velocity = Vector2.Zero;
                    for (int i = 0; i < 16; i++)
                    {
                        float angle = Helper.CircleDividedEqually(i, 16);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(5).RotatedBy(angle), ModContent.ProjectileType<CecitiorTeeth>(), 15, 0);
                    }
                }
                if (AITimer >= 50)
                {
                    if (NPC.ai[2] == 1)
                        AIState = Idle;
                    else
                        AIState = Tongue;
                    NPC.velocity = Vector2.Zero;
                    AITimer = 0;
                }
            }
            else if (AIState == Tongue)
            {
                lastAi = (int)AIState;
                AITimer++;
                open = true;
                if (AITimer < 15)
                {
                    AITimer2 = 10;
                    openOffset.X++;
                    openRotation -= MathHelper.ToRadians(2);
                    rotation += MathHelper.ToRadians(2);
                }
                if (AITimer == 70)
                {
                    tongue = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Helper.FromAToB(NPC.Center, player.Center) * 1.5f, ModContent.ProjectileType<LatcherPCecitior>(), 15, 0, -1, NPC.whoAmI);
                }
                if (AITimer < 20)
                    NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 200), false) / 10f;
                if (AITimer == 20)
                {
                    NPC.velocity = Vector2.Zero;
                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, new Vector2(0, 0), ModContent.ProjectileType<BloodShockwave>(), 0, 0, -1, NPC.whoAmI);
                }
                if (tongue != null)
                {
                    if (tongue.ai[1] == 1 && tongue.active)
                    {
                        NPC.damage = 15;
                        AITimer -= 0.5f;
                    }
                    else if (!tongue.active)
                    {
                        rotation = 0;
                        openRotation = 0;

                        NPC.damage = 0;
                        openOffset = Vector2.Zero;
                        open = false;
                        NPC.velocity *= 0.985f;
                        AITimer += 2;
                    }
                }
                if (AITimer >= 185)
                {
                    openOffset = Vector2.Zero;
                    open = false;
                }
                if (AITimer >= 200)
                {
                    NPC.damage = 0;
                    rotation = 0;
                    openRotation = 0;
                    tongue = null;
                    NPC.velocity = Vector2.Zero;
                    AITimer = 0;
                    if (NPC.ai[2] == 1)
                        AIState = Idle;
                    else
                        AIState = EyeBehaviour2;
                }
            }
            else if (AIState == EyeBehaviour2)
            {
                lastAi = (int)AIState;
                AITimer++;
                NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 200), false) / 10f;
                if (AITimer >= 200 - (p2 ? 95 : 0))
                {
                    if (NPC.ai[2] == 1)
                        AIState = Idle;
                    else
                        AIState = LaserRain;
                    NPC.velocity = Vector2.Zero;
                    AITimer = 0;
                }
            }
            else if (AIState == LaserRain)
            {
                lastAi = (int)AIState;
                AITimer++;
                if (p2)
                    AITimer++;
                open = true;
                if (AITimer < 15)
                {
                    if (p2)
                        AITimer2 = 5;
                    else
                        AITimer2 = 10;
                    openOffset.X++;
                    openRotation -= MathHelper.ToRadians(2);
                    rotation += MathHelper.ToRadians(2);
                }
                if (AITimer < 55)
                    NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 200), false) / 2;
                if (AITimer > 55)
                    NPC.velocity = Vector2.Zero;
                if (AITimer > 70 && AITimer < 85)
                {
                    AITimer2 -= 0.55f;
                    NPC.velocity = Vector2.Zero;
                    for (int i = -1; i < 2; i++)
                    {
                        if (i == 0)
                            continue;
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(i * AITimer2, 5), ModContent.ProjectileType<BloodLaser>(), 15, 0);
                    }
                }
                if (AITimer >= 85)
                {
                    openOffset = Vector2.Zero;
                    open = false;
                }

                if (AITimer >= 90)
                {
                    rotation = 0;
                    openRotation = 0;
                    if (NPC.ai[2] == 1)
                        AIState = Idle;
                    else
                        AIState = ThrowUpBlood;
                    NPC.velocity = Vector2.Zero;
                    AITimer = 0;
                    AITimer2 = 0;
                }
            }
            else if (AIState == ThrowUpBlood)
            {
                lastAi = (int)AIState;
                AITimer++;
                open = true;
                if (AITimer < 15)
                {
                    AITimer2 = 10;
                    NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 200), false) / 20f;
                    openOffset.X++;
                    openRotation += MathHelper.ToRadians(2);
                    rotation -= MathHelper.ToRadians(2);
                }
                if (AITimer >= 30 && AITimer <= 60 && AITimer % 10 == 0)
                {
                    AITimer2 -= 4;
                    NPC.velocity = Vector2.Zero;
                    if (p2)
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(AITimer2 * 0.5f, -2), ProjectileID.GoldenShowerHostile, 15, 0);
                    Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(AITimer2 * 2, -5), ModContent.ProjectileType<HeadGoreSceptreEVILSOBBINGRN>(), 15, 0);
                    a.friendly = false;
                    a.hostile = true;

                }
                if (AITimer >= 60)
                {
                    openOffset.X--;
                    openRotation -= MathHelper.ToRadians(2);
                    rotation += MathHelper.ToRadians(2);
                }
                if (AITimer >= 70)
                {
                    openOffset = Vector2.Zero;
                    open = false;
                }

                if (AITimer >= 75)
                {
                    rotation = 0;
                    openRotation = 0;
                    AIState = Idle;
                    NPC.ai[2] = 1;
                    NPC.velocity = Vector2.Zero;
                    AITimer = 0;
                    AITimer2 = 0;
                }
            }
        }
    }
}
