using EbonianMod.Common.Systems.Misc.Dialogue;
using EbonianMod.Items.Weapons.Magic;
using EbonianMod.Projectiles.ArchmageX;
using EbonianMod.Projectiles.VFXProjectiles;
using EbonianMod.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
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
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                PortraitPositionYOverride = 40,
                PortraitPositionXOverride = -50,
                Position = new Vector2(-50, 40)
            });
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Staff"),
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.NPCs.ArchmageStaffNPC.Bestiary"),
            });
        }
        public override void SetDefaults()
        {
            NPC.Size = new Vector2(162, 80);
            NPC.dontTakeDamage = true;
            NPC.lifeMax = short.MaxValue;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            NPC.rotation = MathHelper.PiOver2;
            NPC.chaseable = false;
            NPC.townNPC = true;
            NPC.friendly = true;
            TownNPCStayingHomeless = true;
        }
        public override void ModifyTypeName(ref string typeName)
        {
            if (!NPC.IsABestiaryIconDummy)
                typeName = " ";
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
                                if (Main.tile[i + 31, j + k].TileType != TileID.TallGateClosed && Main.tile[i + 31, j + k].TileType != TileID.TallGateOpen && k < 6 && k > 0)
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

            NPC.shimmerWet = false;
            NPC.shimmering = false;
            NPC.buffImmune[BuffID.Shimmer] = true;
            rantFactor = Lerp(rantFactor, 0, 0.01f);
            if (d != null && d.timeLeft > 0 && d.Center != Vector2.Zero)
            {

                for (int i = 0; i < d.visibleText.Length; i++)
                {
                    d.charVisibleCenter[i] = NPC.Center - new Vector2(0, 60) + Main.rand.NextVector2Circular(rantFactor, rantFactor);
                    d.Center = NPC.Center - new Vector2(0, 60) + Main.rand.NextVector2Circular(rantFactor * 4, rantFactor * 4);
                    d.VisibleCenter = NPC.Center - new Vector2(0, 60) + Main.rand.NextVector2Circular(rantFactor * 4, rantFactor * 4);
                }
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
                        d = DialogueSystem.NewDialogueBox(60, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.DotDotDot").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 190)
                        d = DialogueSystem.NewDialogueBox(140, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.PostMartianLine1").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 340)
                        d = DialogueSystem.NewDialogueBox(140, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.PostMartianLine2").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 490)
                        d = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.DotDotDot").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 600)
                        d = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 60), String.Concat(Enumerable.Repeat(Language.GetText("Mods.EbonianMod.Dialogue.DotDotDot").Value, 5)), Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 720)
                        d = DialogueSystem.NewDialogueBox(140, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.PostMartianLine3").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);
                    if (NPC.ai[1] == 870)
                        d = DialogueSystem.NewDialogueBox(180, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.PostMartianLine4").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);
                    if (NPC.ai[1] == 1060)
                        d = DialogueSystem.NewDialogueBox(200, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.PostMartianLine5").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 5, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 1260)
                        d = DialogueSystem.NewDialogueBox(200, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.PostMartianLine6").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 5, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 1480)
                        d = DialogueSystem.NewDialogueBox(150, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.PostMartianLine7").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 5, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 1650)
                        d = DialogueSystem.NewDialogueBox(200, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.PostMartianLine8").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 5, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 1850)
                    {
                        rantFactor = 40;
                        d = DialogueSystem.NewDialogueBox(200, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.PostMartianLine9").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 5, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
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
                                d = DialogueSystem.NewDialogueBox(120, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.PsstLine").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 8f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_DarkMageCastHeal.WithPitchOffset(0.9f), 3);
                                NPC.ai[0] = 1;
                            }
                            if (NPC.ai[0] == 130)
                            {
                                d = DialogueSystem.NewDialogueBox(120, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.YouLine").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 8f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2); NPC.ai[0] = 1;
                                NPC.ai[0] = 131;
                            }
                            if (NPC.ai[0] == 260)
                            {
                                d = DialogueSystem.NewDialogueBox(120, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ComeHereLine").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 8f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2); NPC.ai[0] = 1;
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
                                d = DialogueSystem.NewDialogueBox(60, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffLine1").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            if (NPC.ai[1] == 190)
                                d = DialogueSystem.NewDialogueBox(140, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffLine2").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            if (NPC.ai[1] == 340)
                                d = DialogueSystem.NewDialogueBox(140, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffLine3").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            if (NPC.ai[1] == 490)
                                d = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.DotDotDot").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            if (NPC.ai[1] == 600)
                                d = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffLine4").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            if (NPC.ai[1] == 720)
                                d = DialogueSystem.NewDialogueBox(140, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffLine5").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);
                            if (NPC.ai[1] == 870)
                                d = DialogueSystem.NewDialogueBox(180, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffLine6").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);
                            if (NPC.ai[1] == 1060)
                            {
                                rantFactor = 5;
                                d = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffLine7").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 5, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            }
                            if (NPC.ai[1] == 1180)
                                d = DialogueSystem.NewDialogueBox(110, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffLine8").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.25f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);

                            if (NPC.ai[1] == 1300)
                            {
                                rantFactor = 5;
                                d = DialogueSystem.NewDialogueBox(150, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffLine9").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            }
                            if (NPC.ai[1] == 1460)
                            {
                                rantFactor = 8;
                                d = DialogueSystem.NewDialogueBox(135, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffLine10").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            }
                            if (NPC.ai[1] == 1630)
                                d = DialogueSystem.NewDialogueBox(200, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffLine11").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 2, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            if (NPC.ai[1] == 1820)
                                d = DialogueSystem.NewDialogueBox(120, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffLine12").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            if (NPC.ai[1] == 1950)
                                d = DialogueSystem.NewDialogueBox(120, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.DotDotDot").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            if (NPC.ai[1] == 2150)
                                d = DialogueSystem.NewDialogueBox(150, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffLine13").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            if (NPC.ai[1] == 2350)
                            {
                                rantFactor = 5;
                                d = DialogueSystem.NewDialogueBox(120, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffLine14").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            }
                            if (NPC.ai[1] == 2490)
                            {
                                rantFactor = 15;
                                Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 6, 6, 30, 1000));
                                d = DialogueSystem.NewDialogueBox(150, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffLine15").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.3f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            }

                            if (NPC.ai[1] == 2630)
                            {
                                rantFactor = 20;
                                d = DialogueSystem.NewDialogueBox(150, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffLine16").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.2f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            }
                            if (NPC.ai[1] == 2800)
                            {
                                rantFactor = 30;
                                d = DialogueSystem.NewDialogueBox(150, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffLine17").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.1f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            }
                            if (NPC.ai[1] == 3050)
                                d = DialogueSystem.NewDialogueBox(120, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.DotDotDot").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                            if (NPC.ai[1] == 3200)
                                d = DialogueSystem.NewDialogueBox(120, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffLine18").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);
                            if (NPC.ai[1] == 3370)
                                d = DialogueSystem.NewDialogueBox(200, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffLine19").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);
                            if (NPC.ai[1] == 3600)
                            {
                                GetInstance<EbonianSystem>().timesDiedToXareus = -1;
                                d = DialogueSystem.NewDialogueBox(140, NPC.Center - new Vector2(0, 60), Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffLine20").Value, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
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
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffMotivation1").Value);
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffMotivation2").Value);
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffMotivation3").Value);
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffMotivation4").Value);
                            }
                            else
                            {
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffMotivation5").Value);
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffMotivation6").Value);
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffMotivation7").Value);
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffMotivation8").Value);
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffMotivation9").Value);
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffMotivation10").Value);
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.ArchstaffMotivation11").Value);
                            }
                            d = DialogueSystem.NewDialogueBox(180, NPC.Center - new Vector2(0, 60), chat, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 3, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);
                            NPC.ai[2] = 51;
                        }

                    }
                    else
                    {
                        NPC.ai[2]--;
                        if (NPC.ai[2] == 2000)
                            d = DialogueSystem.NewDialogueBox(180, NPC.Center - new Vector2(0, 60), "", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);

                        if (NPC.ai[2] == 1770)
                            d = DialogueSystem.NewDialogueBox(200, NPC.Center - new Vector2(0, 60), "", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 2, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);

                        if (NPC.ai[2] == 1580)
                            d = DialogueSystem.NewDialogueBox(180, NPC.Center - new Vector2(0, 60), "", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 2, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);

                        if (NPC.ai[2] == 1390)
                            d = DialogueSystem.NewDialogueBox(180, NPC.Center - new Vector2(0, 60), "", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 2, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);

                        if (NPC.ai[2] == 1200)
                            d = DialogueSystem.NewDialogueBox(180, NPC.Center - new Vector2(0, 60), "", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 2, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);


                        if (NPC.ai[2] == 1000)
                        {
                            d = DialogueSystem.NewDialogueBox(180, NPC.Center - new Vector2(0, 60), "", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 2, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);
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
            chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.InteractionLine").Value);
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
            button = Language.GetText("Mods.EbonianMod.Dialogue.ArchstaffDialogue.Grab").Value;
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
            Texture2D tex = Helper.GetTexture("Items/Weapons/Magic/StaffOfX");
            Texture2D bloom = Helper.GetTexture("Items/Weapons/Magic/StaffOfX_Bloom");
            Texture2D interact = Helper.GetTexture("Items/Weapons/Magic/StaffOfX_InteractionHover");
            Texture2D streak = ExtraTextures2.scratch_02;
            if (NPC.IsABestiaryIconDummy)
            {
                NPC.rotation = PiOver4;
                return true;
            }
            else
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
                        EbonianSystem.stickZoomLerpVal = MathHelper.SmoothStep(EbonianSystem.stickZoomLerpVal, Clamp(MathHelper.SmoothStep(1f, 0, (Main.LocalPlayer.Center.Distance(NPC.Center) / (800f) - EbonianSystem.stickLerpOffset)), 0, 1), 0.2f);
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
                if (seed == 0)
                    seed = Main.rand.Next(100024018);
                Vector2 position = NPC.Center + new Vector2(0, MathF.Sin(Main.GlobalTimeWrappedHourly * .15f) * 16)
                    + (!NPC.IsABestiaryIconDummy ? Main.rand.NextVector2Circular(rantFactor, rantFactor) : Vector2.Zero) - Main.screenPosition;
                UnifiedRandom rand = new UnifiedRandom(seed);

                Main.spriteBatch.SaveCurrent();
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                float _alpha = (NPC.IsABestiaryIconDummy ? 1 : staffAlpha);

                int max = 20;
                for (int k = 0; k < max; k++)
                {
                    float factor = (MathF.Sin(Main.GlobalTimeWrappedHourly + rand.Next(10000) * .02f) + 1) * 0.5f * staffAlpha;
                    float alpha = MathHelper.Clamp(MathHelper.Lerp(0.4f, -0.1f, factor) * 2, 0, 0.5f);
                    float angle = Helper.CircleDividedEqually(k, max);
                    float scale = rand.NextFloat(0.5f, 1.5f) * factor;
                    Vector2 offset = new Vector2(rand.NextFloat(50) * factor * scale, 0).RotatedBy(angle);
                    for (int l = 0; l < 2; l++)
                        Main.spriteBatch.Draw(streak, position + new Vector2(rand.NextFloat(-80, 80), rand.NextFloat(-20, 20)) + offset, null, Color.Violet * (alpha * _alpha), angle, new Vector2(0, streak.Height / 2), new Vector2(alpha, factor * 2) * scale, SpriteEffects.None, 0);
                }

                Main.spriteBatch.Draw(bloom, position, null, Color.Violet * ((0.5f + MathHelper.Clamp(MathF.Sin(Main.GlobalTimeWrappedHourly * .5f), 0, 0.4f)) * _alpha), MathHelper.PiOver4, bloom.Size() / 2, 1, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(bloom, position, null, Color.Violet * ((0.05f + (MathF.Sin(Main.GlobalTimeWrappedHourly * .55f) + .5f) * 0.3f) * _alpha), MathHelper.PiOver4, bloom.Size() / 2, 1.1f, SpriteEffects.None, 0);
                Main.spriteBatch.ApplySaved();

                Main.spriteBatch.Draw(tex, position, null, Color.White * _alpha, MathHelper.PiOver4, tex.Size() / 2, 1, SpriteEffects.None, 0);
            }
            return false;
        }
    }
    public class ShackMusic : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/ambience2");
        public override bool IsBiomeActive(Player player)
        {
            return EbonianSystem.stickZoomLerpVal > 0.1f;
        }
    }
}
