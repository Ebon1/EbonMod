using EbonianMod.Common.Systems.Misc.Dialogue;
using EbonianMod.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
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
        }
        public override void SetDefaults()
        {
            NPC.Size = new Vector2(162, 80);
            NPC.dontTakeDamage = true;
            NPC.lifeMax = short.MaxValue;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            NPC.rotation = MathHelper.PiOver2;
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
                    if (Main.tile[i, j].TileType == (ushort)ModContent.TileType<ArchmageStaffTile>())
                    {
                        if (EbonianSystem.xareusFightCooldown <= 0)
                        {
                            Projectile.NewProjectile(null, new Vector2(i * 16, j * 16 - 100), Vector2.Zero, ModContent.ProjectileType<ArchmageXSpawnAnim>(), 0, 0, NPC.whoAmI);

                            for (int k = -23; k < 6; k++)
                            {
                                Main.tile[i - 31, j + k].TileType = ((ushort)ModContent.TileType<XHouseBrick>());
                                if (Main.tile[i + 31, j + k].TileType != TileID.TallGateClosed && Main.tile[i + 31, j + k].TileType != TileID.TallGateOpen)
                                {
                                    Main.tile[i + 31, j + k].TileType = ((ushort)ModContent.TileType<XHouseBrick>());
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
                                Main.tile[i + k, j + 5].TileType = ((ushort)ModContent.TileType<XHouseBrick>());
                                Main.tile[i + k, j - 23].TileType = ((ushort)ModContent.TileType<XHouseBrick>());

                                Tile tile = Main.tile[i + k, j - 23];
                                tile.HasTile = true;
                                Tile tile2 = Main.tile[i + k, j + 5];
                                tile2.HasTile = true;
                            }

                            for (int k = -31; k < 31; k++)
                            {
                                for (int l = -21; l < 6; l++)
                                    if (Main.tile[i + k, j + l].HasTile && Main.tileSolid[Main.tile[i + k, j + l].TileType] && !Main.tileSolidTop[Main.tile[i + k, j + l].TileType] &&
                                        Main.tile[i + k, j + l].TileType != ModContent.TileType<XHouseBrick>() && Main.tile[i + k, j + l].TileType != TileID.Platforms)
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
        public override void AI()
        {
            NPC.DiscourageDespawn(120);

            NPC.Center = sPos;

            EbonianPlayer p = Main.LocalPlayer.GetModPlayer<EbonianPlayer>();
            float dist = Main.LocalPlayer.Distance(NPC.Center);
            //if (p.timesDiedToXareus <= 0) // re add later
            {
                if (dist > 400 && dist < 700 && p.Player.Center.Y.CloseTo(NPC.Center.Y - 30, 100))
                {
                    if (NPC.ai[0] > 0)
                        NPC.ai[0]++;
                    if (NPC.ai[0] == 0)
                    {
                        DialogueSystem.NewDialogueBox(120, NPC.Center - new Vector2(0, 60), "Psst.", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 8f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_DarkMageCastHeal.WithPitchOffset(0.9f), 3);
                        NPC.ai[0] = 1;
                    }
                    if (NPC.ai[0] == 130)
                    {
                        DialogueSystem.NewDialogueBox(120, NPC.Center - new Vector2(0, 60), "You..", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 8f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    }
                    if (NPC.ai[0] == 260)
                    {
                        DialogueSystem.NewDialogueBox(120, NPC.Center - new Vector2(0, 60), "Come here..", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 8f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
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
                        DialogueSystem.NewDialogueBox(60, NPC.Center - new Vector2(0, 60), "Hmm..", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 190)
                        DialogueSystem.NewDialogueBox(140, NPC.Center - new Vector2(0, 60), "I can sense your power...", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 340)
                        DialogueSystem.NewDialogueBox(140, NPC.Center - new Vector2(0, 60), "Yes, Yes! Finally!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 490)
                        DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 60), "...", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 600)
                        DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 60), "You see..", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 720)
                        DialogueSystem.NewDialogueBox(140, NPC.Center - new Vector2(0, 60), "My master is a... TERRIBLE magician.", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);
                    if (NPC.ai[1] == 870)
                        DialogueSystem.NewDialogueBox(180, NPC.Center - new Vector2(0, 60), "It's quite humiliating being limited by such an... Idiot!.. 'Archmage', pfft.", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);
                    if (NPC.ai[1] == 1060)
                        DialogueSystem.NewDialogueBox(200, NPC.Center - new Vector2(0, 60), "Oh right! You! Please.. Please take me! My master REFUSES to utilize my grandeur!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 3);
                    if (NPC.ai[1] == 1260)
                        DialogueSystem.NewDialogueBox(140, NPC.Center - new Vector2(0, 60), "He's not here! He won't even notice!", Color.White, -1, 0.6f, Color.Magenta * 0.6f, 1.75f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    if (NPC.ai[1] == 1350)
                        NPC.ai[2] = 1;
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
            bool projExists = false;
            EbonianPlayer p = Main.LocalPlayer.GetModPlayer<EbonianPlayer>();
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<ArchmageXSpawnAnim>())
                {
                    projExists = true;
                    break;
                }
            }
            return !(NPC.AnyNPCs(ModContent.NPCType<ArchmageX>()) || EbonianSystem.xareusFightCooldown > 0 || projExists || (p.timesDiedToXareus <= 0 && NPC.ai[2] == 0));
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
                if (!NPC.AnyNPCs(ModContent.NPCType<ArchmageX>()) && EbonianSystem.xareusFightCooldown <= 0)
                    EbonianSystem.stickZoomLerpVal = MathHelper.SmoothStep(EbonianSystem.stickZoomLerpVal, MathHelper.SmoothStep(1f, 0, Main.LocalPlayer.Center.Distance(NPC.Center) / 800f), 0.2f);
            }
            if (NPC.AnyNPCs(ModContent.NPCType<ArchmageX>()) || EbonianSystem.xareusFightCooldown > 0)
                staffAlpha = MathHelper.Lerp(staffAlpha, 0f, 0.1f);
            else
                staffAlpha = MathHelper.Lerp(staffAlpha, 1f, 0.2f);
            Vector2 position = NPC.Center + new Vector2(0, MathF.Sin(Main.GlobalTimeWrappedHourly * .15f) * 16) - Main.screenPosition;
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
                    Main.spriteBatch.Draw(streak, position + new Vector2(rand.NextFloat(-80, 80), rand.NextFloat(-20, 20)) + offset, null, Color.Violet * (alpha * staffAlpha), angle, new Vector2(0, streak.Height / 2), new Vector2(alpha, factor * 2) * scale * 0.5f, SpriteEffects.None, 0);
            }

            Main.spriteBatch.Draw(bloom, position, null, Color.Violet * ((0.5f + MathHelper.Clamp(MathF.Sin(Main.GlobalTimeWrappedHourly * .5f), 0, 0.4f)) * staffAlpha), MathHelper.PiOver4, bloom.Size() / 2, 1, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(bloom, position, null, Color.Violet * ((0.05f + (MathF.Sin(Main.GlobalTimeWrappedHourly * .55f) + .5f) * 0.3f) * staffAlpha), MathHelper.PiOver4, bloom.Size() / 2, 1.1f, SpriteEffects.None, 0);
            Main.spriteBatch.ApplySaved();

            Main.spriteBatch.Draw(tex, position, null, Color.White * staffAlpha, MathHelper.PiOver4, tex.Size() / 2, 1, SpriteEffects.None, 0);
            return false;
        }
    }
}
