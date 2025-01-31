using EbonianMod.Common.Systems.Misc.Dialogue;
using EbonianMod.Items.Weapons.Magic;
using EbonianMod.Projectiles.ArchmageX;
using EbonianMod.Projectiles.VFXProjectiles;
using EbonianMod.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace EbonianMod.NPCs.ArchmageX
{
    public class ArchmageStaffNPC : ModNPC
    {
        public override string Texture => "EbonianMod/Items/Weapons/Magic/StaffOfX";
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.Size = new Vector2(162, 80);
            NPC.dontTakeDamage = true;
            NPC.lifeMax = short.MaxValue;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            NPC.rotation = MathHelper.PiOver2;
            NPC.chaseable = true;
            NPC.townNPC = true;
            NPC.friendly = true;
            TownNPCStayingHomeless = true;
        }
        Vector2 sPos;
        public override void OnSpawn(IEntitySource source)
        {
            seed = Main.rand.Next(1, 999999);
            if (NPC.ai[3] == 0)
            {
                NPC.active = false;
                return;
            }
            sPos = NPC.Center;
        }
        float staffAlpha;
        int seed;
        void StartFight()
        {
            for (int i = (int)NPC.Center.X / 16 - 3; i < (int)NPC.Center.X / 16 + 3; i++)
            {
                for (int j = (int)NPC.Center.Y / 16 - 3; j < (int)NPC.Center.Y / 16 + 3; j++)
                {
                    if (Main.tile[i, j].TileType == (ushort)TileType<ArchmageStaffTile>())
                    {
                        if (EbonianSystem.xareusFightCooldown <= 0)
                        {
                            Projectile.NewProjectile(null, new Vector2(i * 16 + 88, j * 16 + MathF.Sin(Main.GlobalTimeWrappedHourly * .15f) * 16), Vector2.Zero, ProjectileType<ArchmageXSpawnAnim>(), 0, 0);

                            for (int k = -23; k < 6; k++)
                            {
                                Main.tile[i - 31, j + k].TileType = ((ushort)TileType<XHouseBrick>());
                                if (Main.tile[i + 31, j + k].TileType != TileID.TallGateClosed && Main.tile[i + 31, j + k].TileType != TileID.TallGateOpen)
                                {
                                    Main.tile[i + 31, j + k].TileType = ((ushort)TileType<XHouseBrick>());
                                    Tile tile = Main.tile[i + 31, j + k];
                                    tile.HasTile = true;
                                }
                                Tile tile2 = Main.tile[i - 31, j + k];
                                tile2.HasTile = true;

                                WorldGen.TileFrame(i + 31, j + k, noBreak: true);
                                WorldGen.TileFrame(i - 31, j + k, noBreak: true);
                            }
                            for (int k = -31; k < 31; k++)
                            {
                                Main.tile[i + k, j + 5].TileType = ((ushort)TileType<XHouseBrick>());
                                Main.tile[i + k, j - 23].TileType = ((ushort)TileType<XHouseBrick>());

                                Tile tile = Main.tile[i + k, j - 23];
                                tile.HasTile = true;
                                Tile tile2 = Main.tile[i + k, j + 5];
                                tile2.HasTile = true;
                            }

                            for (int k = -31; k < 31; k++)
                            {
                                for (int l = -21; l < 6; l++)
                                    if (Main.tile[i + k, j + l].HasTile && Main.tileSolid[Main.tile[i + k, j + l].TileType] && !Main.tileSolidTop[Main.tile[i + k, j + l].TileType] &&
                                        Main.tile[i + k, j + l].TileType != TileType<XHouseBrick>() && Main.tile[i + k, j + l].TileType != TileID.Platforms)
                                        Main.tile[i + k, j + l].ClearTile();
                            }

                            for (int k = -33; k < 33; k++)
                            {
                                for (int l = -25; l < 8; l++)
                                {
                                    WorldGen.TileFrame(i + k, j + l, noBreak: true);
                                }
                            }

                            break;
                        }
                    }
                }
            }
        }
        FloatingDialogueBox d = null;
        float rantFactor = 0;
        bool initiatedMartianCutscene;
        public override void AI()
        {
            NPC.DiscourageDespawn(120);

            rantFactor = Lerp(rantFactor, 0, 0.01f);
            if (d != null && d.timeLeft > 0 && d.Center != Vector2.Zero)
            {
                d.VisibleCenter = d.Center + Main.rand.NextVector2Circular(rantFactor, rantFactor);
            }
            bool hasTile = false;
            if (NPC.Center != Vector2.Zero)
            {
                for (int i = (int)NPC.Center.X / 16 - 3; i < (int)NPC.Center.X / 16 + 3; i++)
                {
                    for (int j = (int)NPC.Center.Y / 16 - 3; j < (int)NPC.Center.Y / 16 + 3; j++)
                    {
                        if (Main.tile[i, j].TileType == (ushort)TileType<ArchmageStaffTile>())
                        {
                            hasTile = true;
                        }
                    }
                }
            }
            if (!hasTile || NPC.Center == Vector2.Zero) NPC.active = false;
            NPC.Center = sPos;

            EbonianPlayer p = Main.LocalPlayer.GetModPlayer<EbonianPlayer>();
            float dist = Main.LocalPlayer.Distance(NPC.Center);
            if (NPC.downedMartians && GetInstance<EbonianSystem>().xareusFuckingDies && GetInstance<EbonianSystem>().downedXareus && !NPC.AnyNPCs(NPCType<ArchmageCutsceneMartian>()))
            {
                if (!GetInstance<EbonianSystem>().gotTheStaff)
                {
                    if (NPC.ai[3] < 1960)
                        staffAlpha = Lerp(staffAlpha, 1, 0.1f);
                    if (dist < 300 && p.Player.Center.Y.CloseTo(NPC.Center.Y - 30, 100) && !initiatedMartianCutscene)
                        initiatedMartianCutscene = true;
                    if (initiatedMartianCutscene)
                        NPC.ai[1]++;
                    if (NPC.ai[1] == 100)
                        d = DialogueSystem.NewDialogueBox(60, NPC.Center - new Vector2(0, 60), "...", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 190)
                        d = DialogueSystem.NewDialogueBox(140, NPC.Center - new Vector2(0, 60), "I assume you are... aware..", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 340)
                        d = DialogueSystem.NewDialogueBox(140, NPC.Center - new Vector2(0, 60), "..of the passing of Master Xareus...", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 490)
                        d = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 60), "...", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 600)
                        d = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 60), "............", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 720)
                        d = DialogueSystem.NewDialogueBox(140, NPC.Center - new Vector2(0, 60), "No.. no.. Tragedy won't break the Archstaff!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);
                    if (NPC.ai[1] == 870)
                        d = DialogueSystem.NewDialogueBox(180, NPC.Center - new Vector2(0, 60), "YOU! Become my new wielder!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);
                    if (NPC.ai[1] == 1060)
                        d = DialogueSystem.NewDialogueBox(200, NPC.Center - new Vector2(0, 60), "You are obviously more worthy than Master Xareus! Together we will destroy galaxies!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 5, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 1260)
                        d = DialogueSystem.NewDialogueBox(200, NPC.Center - new Vector2(0, 60), "Yes, yes! I can see it now! Your name, whatever it is, and the Great Archstaff of the Tragically Deceased Master Xareus!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 5, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 1480)
                        d = DialogueSystem.NewDialogueBox(150, NPC.Center - new Vector2(0, 60), "Power beyond anything ever conceived by minds even eldritch!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 5, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 1650)
                        d = DialogueSystem.NewDialogueBox(200, NPC.Center - new Vector2(0, 60), "Glory! And fame! We will be unstoppable!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 5, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 1850)
                    {
                        rantFactor = 40;
                        d = DialogueSystem.NewDialogueBox(200, NPC.Center - new Vector2(0, 60), "RAAAAAAAAAAAAAAAAAAAAAAAAAAAGHHHHHHHHHH!!!!!!!!!!!!!!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 5, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    }

                    if (NPC.ai[1] == 1885)
                    {
                        Item.NewItem(null, NPC.getRect(), ItemType<StaffOfX>());

                        for (int i = 0; i < 35; i++)
                            Projectile.NewProjectile(null, NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(10, 30), ProjectileType<XAnimeSlash>(), 0, 0, -1, 0, Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(0.1f, 0.3f));

                        Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0);
                        GetInstance<EbonianSystem>().gotTheStaff = true;
                        staffAlpha = 0;
                    }

                }
            }
            else
            {
                if (GetInstance<EbonianSystem>().timesDiedToXareus <= 0) // re add later
                {
                    if (GetInstance<EbonianSystem>().timesDiedToXareus == 0)
                    {
                        if (dist > 400 && dist < 700 && p.Player.Center.Y.CloseTo(NPC.Center.Y - 30, 100))
                        {
                            if (NPC.ai[0] > 0)
                                NPC.ai[0]++;
                            if (NPC.ai[0] == 0)
                            {
                                d = DialogueSystem.NewDialogueBox(120, NPC.Center - new Vector2(0, 60), "Psst.", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 8f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_DarkMageCastHeal.WithPitchOffset(0.9f), 3);
                                NPC.ai[0] = 1;
                            }
                            if (NPC.ai[0] == 130)
                            {
                                d = DialogueSystem.NewDialogueBox(120, NPC.Center - new Vector2(0, 60), "You..", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 8f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2); NPC.ai[0] = 1;
                                NPC.ai[0] = 131;
                            }
                            if (NPC.ai[0] == 260)
                            {
                                d = DialogueSystem.NewDialogueBox(120, NPC.Center - new Vector2(0, 60), "Come here..", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 8f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2); NPC.ai[0] = 1;
                                NPC.ai[0] = 261;
                            }
                        }
                        if (dist < 300 && p.Player.Center.Y.CloseTo(NPC.Center.Y - 30, 100))
                            NPC.ai[0] = 301;
                        if (NPC.ai[1] > 0)
                        {
                            if (GetArenaRect().Size().Length() > 100)
                            {
                                if (p.Player.Distance(GetArenaRect().Center()) > 1200)
                                {
                                    Helper.TPNoDust(GetArenaRect().Center(), p.Player);
                                }
                                else
                                {
                                    while (p.Player.Center.X < GetArenaRect().X)
                                        p.Player.Center += Vector2.UnitX * 2;

                                    while (p.Player.Center.X > GetArenaRect().X + GetArenaRect().Width)
                                        p.Player.Center -= Vector2.UnitX * 2;

                                    while (p.Player.Center.Y < GetArenaRect().Y)
                                        p.Player.Center += Vector2.UnitY * 2;
                                }
                            }
                        }
                        if (NPC.ai[0] > 300 && (dist < 400 || NPC.ai[1] > 0))
                        {
                            if (NPC.ai[1] == 0)
                                NPC.ai[1] = 1;
                            if (NPC.ai[1] > 0)
                                NPC.ai[1]++;
                            if (NPC.ai[1] == 100)
                                d = DialogueSystem.NewDialogueBox(60, NPC.Center - new Vector2(0, 60), "Hmm..", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            if (NPC.ai[1] == 190)
                                d = DialogueSystem.NewDialogueBox(140, NPC.Center - new Vector2(0, 60), "You seem.. capable..", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            if (NPC.ai[1] == 340)
                                d = DialogueSystem.NewDialogueBox(140, NPC.Center - new Vector2(0, 60), "Yes, Yes, Finally! A new wielder!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            if (NPC.ai[1] == 490)
                                d = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 60), "...", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            if (NPC.ai[1] == 600)
                                d = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 60), "You see..", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            if (NPC.ai[1] == 720)
                                d = DialogueSystem.NewDialogueBox(140, NPC.Center - new Vector2(0, 60), "My master is a... TERRIBLE magician.", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);
                            if (NPC.ai[1] == 870)
                                d = DialogueSystem.NewDialogueBox(180, NPC.Center - new Vector2(0, 60), "It's quite humiliating being limited by such an...", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);
                            if (NPC.ai[1] == 1060)
                            {
                                rantFactor = 5;
                                d = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 60), "Idiot!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 5, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            }
                            if (NPC.ai[1] == 1180)
                                d = DialogueSystem.NewDialogueBox(110, NPC.Center - new Vector2(0, 60), "'Archmage', pfft.", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.25f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);

                            if (NPC.ai[1] == 1300)
                            {
                                rantFactor = 5;
                                d = DialogueSystem.NewDialogueBox(150, NPC.Center - new Vector2(0, 60), "I was forged from pure powdered lightning, the finest amethysts from the realm of crystals,", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            }
                            if (NPC.ai[1] == 1460)
                            {
                                rantFactor = 8;
                                d = DialogueSystem.NewDialogueBox(135, NPC.Center - new Vector2(0, 60), "in the heat of a billion suns, by the greatest goblin smiths of the omniverse!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            }
                            if (NPC.ai[1] == 1630)
                                d = DialogueSystem.NewDialogueBox(170, NPC.Center - new Vector2(0, 60), "Like... Can you even imagine that?! An ARCHMAGE who refuses to use his own greatest weapon?!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            if (NPC.ai[1] == 1820)
                                d = DialogueSystem.NewDialogueBox(120, NPC.Center - new Vector2(0, 60), "What a sick joke!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            if (NPC.ai[1] == 1950)
                                d = DialogueSystem.NewDialogueBox(120, NPC.Center - new Vector2(0, 60), "...", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            if (NPC.ai[1] == 2150)
                                d = DialogueSystem.NewDialogueBox(150, NPC.Center - new Vector2(0, 60), "And that dimwitted magician refuses to use my power, why?!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            if (NPC.ai[1] == 2350)
                            {
                                rantFactor = 5;
                                d = DialogueSystem.NewDialogueBox(120, NPC.Center - new Vector2(0, 60), "Because he's scared of me scratching!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            }
                            if (NPC.ai[1] == 2490)
                            {
                                rantFactor = 15;
                                Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 6, 6, 30, 1000));
                                d = DialogueSystem.NewDialogueBox(150, NPC.Center - new Vector2(0, 60), "I'M A WEAPON! I AM MEANT TO SCRATCH! I'M NOT A DISPLAY PIECE", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            }

                            if (NPC.ai[1] == 2630)
                            {
                                rantFactor = 20;
                                d = DialogueSystem.NewDialogueBox(150, NPC.Center - new Vector2(0, 60), "MY PURPOSE IS TO BE USED IN BATTLE BY THE GREATEST WIZARDS OF ALL TIME FOR GENERATIONS TO COME", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            }
                            if (NPC.ai[1] == 2800)
                            {
                                rantFactor = 30;
                                d = DialogueSystem.NewDialogueBox(150, NPC.Center - new Vector2(0, 60), "TO DIE IN COMBAT AND TO BE REMEMBERED AS THE TRUE GREATEST ARCHSTAFF OF EVERY REALITY!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            }
                            if (NPC.ai[1] == 3050)
                                d = DialogueSystem.NewDialogueBox(120, NPC.Center - new Vector2(0, 60), "...", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            if (NPC.ai[1] == 3200)
                                d = DialogueSystem.NewDialogueBox(120, NPC.Center - new Vector2(0, 60), "Oh right!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);
                            if (NPC.ai[1] == 3370)
                                d = DialogueSystem.NewDialogueBox(200, NPC.Center - new Vector2(0, 60), "You! Please.. Please take me! My master REFUSES to utilize my grandeur!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);
                            if (NPC.ai[1] == 3600)
                            {
                                GetInstance<EbonianSystem>().timesDiedToXareus = -1;
                                d = DialogueSystem.NewDialogueBox(140, NPC.Center - new Vector2(0, 60), "He's not here! He won't even notice!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            }
                        }
                    }
                }
                else
                {
                    if (EbonianSystem.xareusFightCooldown > 0 && NPC.ai[2] <= 1000)
                        NPC.ai[2] = 0;
                    if (NPC.ai[2] < 1000)
                    {
                        if (NPC.ai[2] < 999 && dist < 400 && !p.Player.dead)
                            NPC.ai[2]++;

                        if (NPC.ai[2] == 50)
                        {
                            WeightedRandom<string> chat = new WeightedRandom<string>();
                            if (GetInstance<EbonianSystem>().downedXareus)
                            {
                                chat.Add("Hahaha! Back for another GLORIOUS battle!? He's easily aggravated!");
                                chat.Add("Fighting you is most pleasing!");
                                chat.Add("Back for another unforgettable fight?");
                                chat.Add("Oh how I ACHE for a fight!");
                            }
                            else
                            {
                                chat.Add("That wasn't a... horrible attempt! He's not here, you can try again!");
                                chat.Add("You can do it! Ignore his ramblings!");
                                chat.Add("Try not to die this time!");
                                chat.Add("The Sheepening ray may look intimidating... Because it is!");
                                chat.Add("I've told him numerous times to get rid of the large amethyst spell! Such an annoyance..");
                                chat.Add("Why does he even throw his potions like that?");
                                chat.Add("He's eventually going to run out of amethysts... I think");
                            }
                            d = DialogueSystem.NewDialogueBox(180, NPC.Center - new Vector2(0, 60), chat, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 3, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);
                            NPC.ai[2] = 51;
                        }

                    }
                    else
                    {
                        NPC.ai[2]--;
                        if (NPC.ai[2] == 2000)
                            d = DialogueSystem.NewDialogueBox(180, NPC.Center - new Vector2(0, 60), "Woah... He actually... Used me!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);

                        if (NPC.ai[2] == 1770)
                            d = DialogueSystem.NewDialogueBox(200, NPC.Center - new Vector2(0, 60), "What a spectacular fight! And, And I was the heart of it all! The Grand Archstaff!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 2, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);

                        if (NPC.ai[2] == 1580)
                            d = DialogueSystem.NewDialogueBox(180, NPC.Center - new Vector2(0, 60), "I can't remember the last time he actually used me to my full potential!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 2, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);

                        if (NPC.ai[2] == 1390)
                            d = DialogueSystem.NewDialogueBox(180, NPC.Center - new Vector2(0, 60), "I suppose I have to thank you for this unforgettable battle!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 2, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);

                        if (NPC.ai[2] == 1200)
                            d = DialogueSystem.NewDialogueBox(180, NPC.Center - new Vector2(0, 60), "The chaos! The bloodshed! The, the amethysts!! You may return at a later time if you wish for more!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 2, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);


                        if (NPC.ai[2] == 1000)
                        {
                            d = DialogueSystem.NewDialogueBox(180, NPC.Center - new Vector2(0, 60), "Farewell!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 2, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);
                            EbonianSystem.xareusFightCooldown = 3600 * 12;
                            NPC.ai[2] = 0;
                        }
                    }
                }
            }
        }
        Rectangle GetArenaRect()
        {
            Vector2 sCenter = NPC.Center - new Vector2(0, 150);
            float LLen = Helper.TRay.CastLength(sCenter, -Vector2.UnitX, 29f * 16);
            float RLen = Helper.TRay.CastLength(sCenter, Vector2.UnitX, 29f * 16);
            Vector2 U = Helper.TRay.Cast(sCenter, -Vector2.UnitY, 380);
            Vector2 D = Helper.TRay.Cast(NPC.Center, Vector2.UnitY, 380);
            sCenter.Y = U.Y + Helper.FromAToB(U, D, false).Y * 0.5f;
            Vector2 L = sCenter;
            Vector2 R = sCenter;
            if (LLen > RLen)
            {
                R = Helper.TRay.Cast(sCenter, Vector2.UnitX, 29f * 16);
                L = Helper.TRay.Cast(R, -Vector2.UnitX, 34.5f * 32);
            }
            else
            {
                R = Helper.TRay.Cast(L, Vector2.UnitX, 34.5f * 32);
                L = Helper.TRay.Cast(sCenter, -Vector2.UnitX, 29f * 16);
            }
            Vector2 TopLeft = new Vector2(L.X, U.Y);
            Vector2 BottomRight = new Vector2(R.X, D.Y);
            Rectangle rect = new Rectangle((int)L.X, (int)U.Y, (int)Helper.FromAToB(TopLeft, BottomRight, false).X, (int)Helper.FromAToB(TopLeft, BottomRight, false).Y);
            return rect;
        }
        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            EbonianPlayer p = Main.LocalPlayer.GetModPlayer<EbonianPlayer>();
            chat.Add("It shimmers intensely, aching for a glorious fight.");
            return chat;
        }
        public override bool CanChat()
        {
            EbonianPlayer p = Main.LocalPlayer.GetModPlayer<EbonianPlayer>();
            bool projExists = false;
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.active && proj.type == ProjectileType<ArchmageXSpawnAnim>())
                {
                    projExists = true;
                    break;
                }
            }
            return !NPC.downedMartians && !GetInstance<EbonianSystem>().xareusFuckingDies && !NPC.AnyNPCs(NPCType<ArchmageX>()) && EbonianSystem.xareusFightCooldown <= 0 && !projExists && !(GetInstance<EbonianSystem>().timesDiedToXareus == 0 && NPC.ai[1] < 3700) && NPC.ai[2] < 1001;
        }
        public override void SetChatButtons(ref string button, ref string button2)
        {
            //if (!Main.downedMartians)
            button = "Grab";
            //else
            // button = "";
            button2 = "";
        }
        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                Main.npcChatText = "";
                StartFight();
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Main.LocalPlayer.Center.Distance(NPC.Center) < 700)
            {
                bool projExists = false;
                foreach (Projectile proj in Main.ActiveProjectiles)
                {
                    if (proj.active && proj.type == ProjectileType<ArchmageXSpawnAnim>())
                    {
                        projExists = true;
                        break;
                    }
                }
                if (projExists)
                {
                    EbonianSystem.stickZoomXOffset = Lerp(EbonianSystem.stickZoomXOffset, -88, 0.1f);

                    EbonianSystem.stickLerpOffset = Lerp(EbonianSystem.stickLerpOffset, 1, 0.1f);
                }
                else
                {
                    EbonianSystem.stickZoomXOffset = Lerp(EbonianSystem.stickZoomXOffset, 0, 0.1f);

                    if (EbonianSystem.stickZoomXOffset < 0.01f)
                        EbonianSystem.stickZoomXOffset = 0;

                    EbonianSystem.stickLerpOffset = Lerp(EbonianSystem.stickLerpOffset, 0, 0.1f);

                    if (EbonianSystem.stickLerpOffset < 0.01f)
                        EbonianSystem.stickLerpOffset = 0;
                }


                if (!NPC.AnyNPCs(NPCType<ArchmageX>()) && EbonianSystem.xareusFightCooldown <= 0 && !GetInstance<EbonianSystem>().gotTheStaff)
                    EbonianSystem.stickZoomLerpVal = MathHelper.SmoothStep(EbonianSystem.stickZoomLerpVal, Clamp(MathHelper.SmoothStep(1f, 0, (Main.LocalPlayer.Center.Distance(NPC.Center) / (800f) + EbonianSystem.stickLerpOffset)), 0, 1), 0.2f);
            }
            else
            {
                EbonianSystem.stickZoomLerpVal = MathHelper.SmoothStep(EbonianSystem.stickZoomLerpVal, 0, 0.2f);
                if (EbonianSystem.stickZoomLerpVal.CloseTo(0, 0.01f))
                    EbonianSystem.stickZoomLerpVal = 0;
            }
            if (NPC.AnyNPCs(NPCType<ArchmageX>()) || EbonianSystem.xareusFightCooldown > 0 || GetInstance<EbonianSystem>().gotTheStaff)
            {
                staffAlpha = MathHelper.Lerp(staffAlpha, 0f, 0.1f);
            }
            else
                staffAlpha = MathHelper.Lerp(staffAlpha, 1f, 0.2f);
            Vector2 position = NPC.Center + new Vector2(0, MathF.Sin(Main.GlobalTimeWrappedHourly * .15f) * 16) + Main.rand.NextVector2Circular(rantFactor, rantFactor) - Main.screenPosition;
            Texture2D tex = Helper.GetTexture("Items/Weapons/Magic/StaffOfX");
            Texture2D bloom = Helper.GetTexture("Items/Weapons/Magic/StaffOfX_Bloom");
            Texture2D interact = Helper.GetTexture("Items/Weapons/Magic/StaffOfX_InteractionHover");
            Texture2D streak = Helper.GetExtraTexture("Extras2/scratch_02");
            UnifiedRandom rand = new UnifiedRandom(seed);

            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            int max = 20;
            for (int k = 0; k < max; k++)
            {
                float factor = (MathF.Sin(Main.GlobalTimeWrappedHourly + rand.Next(10000) * .02f) + 1) * 0.5f * staffAlpha;
                float alpha = MathHelper.Clamp(MathHelper.Lerp(0.4f, -0.1f, factor) * 2, 0, 0.5f);
                float angle = Helper.CircleDividedEqually(k, max);
                float scale = rand.NextFloat(0.5f, 1.5f) * factor;
                Vector2 offset = new Vector2(rand.NextFloat(50) * factor * scale, 0).RotatedBy(angle);
                for (int l = 0; l < 2; l++)
                    Main.spriteBatch.Draw(streak, position + new Vector2(rand.NextFloat(-80, 80), rand.NextFloat(-20, 20)) + offset, null, Color.Violet * (alpha * staffAlpha), angle, new Vector2(0, streak.Height / 2), new Vector2(alpha, factor * 2) * scale, SpriteEffects.None, 0);
            }

            Main.spriteBatch.Draw(bloom, position, null, Color.Violet * ((0.5f + MathHelper.Clamp(MathF.Sin(Main.GlobalTimeWrappedHourly * .5f), 0, 0.4f)) * staffAlpha), MathHelper.PiOver4, bloom.Size() / 2, 1, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(bloom, position, null, Color.Violet * ((0.05f + (MathF.Sin(Main.GlobalTimeWrappedHourly * .55f) + .5f) * 0.3f) * staffAlpha), MathHelper.PiOver4, bloom.Size() / 2, 1.1f, SpriteEffects.None, 0);
            Main.spriteBatch.ApplySaved();

            Main.spriteBatch.Draw(tex, position, null, Color.White * staffAlpha, MathHelper.PiOver4, tex.Size() / 2, 1, SpriteEffects.None, 0);
            return false;
        }
    }
}
