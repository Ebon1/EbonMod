using Terraria;
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using static Terraria.ModLoader.PlayerDrawLayer;
using EbonianMod.Dusts;
using EbonianMod.Projectiles.Terrortoma;
using Terraria.Audio;

namespace EbonianMod.NPCs.Corruption
{
    public class TheTrueGluttonEXNeoGod : ModNPC
    {
        public override string Texture => "EbonianMod/NPCs/Corruption/Glutton";
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 9;
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(50f, 70),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 12f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }
        public override void SetBestiary(BestiaryDatabase dataProjectile, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Type: Mentally Deranged"),
                new FlavorTextBestiaryInfoElement("i am going to pour cement into your ears."),
            });
        }
        public override void SetDefaults()
        {
            NPC.width = 150;
            NPC.height = 150;
            NPC.damage = 0;
            NPC.defense = 15;
            NPC.lifeMax = 1000;
            NPC.dontTakeDamage = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 60f;
            NPC.knockBackResist = 0;
            NPC.aiStyle = 0;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.boss = true;
            Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/ambience");
        }
        public const int ActualWidth = 170;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            NPC.frame.Width = ActualWidth;
            NPC.frame.X = ActualWidth;
            if (NPC.frameCounter % 5 == 0)
            {
                if (AIState == 0)
                {
                    NPC.frame.Y = 0;
                }
                else
                {
                    if (NPC.frame.Y < 8 * frameHeight)
                        NPC.frame.Y += frameHeight;
                    else
                        NPC.frame.Y = 0;
                }
            }

        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 pos, Color lightColor)
        {
            Texture2D drawTexture = Helper.GetTexture("NPCs/Corruption/Glutton");
            Vector2 origin = new Vector2((drawTexture.Width / 2) * 0.5F, (drawTexture.Height / Main.npcFrameCount[NPC.type]) * 0.5F);

            Vector2 drawPos = new Vector2(
                NPC.position.X - pos.X + (NPC.width / 2) - (Helper.GetTexture("NPCs/Corruption/Glutton").Width / 2) * NPC.scale / 2f + origin.X * NPC.scale,
                NPC.position.Y - pos.Y + NPC.height - Helper.GetTexture("NPCs/Corruption/Glutton").Height * NPC.scale / Main.npcFrameCount[NPC.type] + 4f + origin.Y * NPC.scale + NPC.gfxOffY);

            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(drawTexture, drawPos, NPC.frame, lightColor, NPC.rotation, origin, NPC.scale, effects, 0);
            return false;
        }
        float alpha;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D a = Helper.GetExtraTexture("Vignette_big");
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            spriteBatch.Draw(a, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * alpha);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
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
        public float AITimer2
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }

        private const int Rant = 0;
        private const int MentalBreakDown = 1;
        private const int Tantrum = 2;
        private const int PhoneCall = 3;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);
            if (NPC.collideY || NPC.Grounded())
            {
                if (alpha < 1)
                    alpha += 0.05f;
                if (AIState == Rant)
                {
                    if (AITimer == 1)
                        EbonianSystem.ChangeCameraPos(NPC.Center, 400, 1);
                    AITimer++;
                    if (AITimer == 100)
                    {

                        Helper.SetDialogue(100, "Some guy told me... \"Hit or miss, I guess they never miss, huh?\"", Color.LawnGreen);
                    }
                    else if (AITimer == 200)
                    {

                        Helper.SetDialogue(100, "That phrase... I get the hit or miss part. Because when you use a Projectile, you can hit, or you can miss.", Color.LawnGreen);
                    }
                    else if (AITimer == 300)
                    {
                        AITimer = 0;
                        AIState = MentalBreakDown;
                    }
                }
                else if (AIState == MentalBreakDown)
                {
                    AITimer++;
                    Music = 12;
                    if (AITimer == 100)
                    {
                        EbonianSystem.ChangeCameraPos(NPC.Center, 500, 1.7f);
                        Helper.SetDialogue(100, "BUT WHAT THE HECK DOES THE 'GUESS THEY NEVER MISS' PART MEAN?!", Color.LawnGreen);
                    }
                    else if (AITimer == 200)
                    {

                        Helper.SetDialogue(100, "HOW CAN YOU NEVER MISS IF IT'S HIT OR MISS?!?", Color.LawnGreen);
                    }
                    else if (AITimer == 300)
                    {
                        Helper.SetDialogue(100, "YOU THINK YOU'RE NEVER GONNA MISS BECAUSE YOU'RE SO GREAT OR SOMETHING?!? HUH?!?", Color.LawnGreen);
                    }
                    else if (AITimer == 400)
                    {

                        Helper.SetDialogue(100, "#### NO, YOU MIGHT MISS!!!", Color.LawnGreen);
                        AIState = Tantrum;
                        AITimer = 0;
                    }
                }
                else if (AIState == Tantrum)
                {
                    AITimer++;
                    Music = 12;
                    if (AITimer == 100)
                    {
                        EbonianSystem.ChangeCameraPos(NPC.Center, 600, 2f);


                        Helper.SetDialogue(100, "YOU ############, YOU STUPID ############", Color.LawnGreen);
                    }
                    else if (AITimer == 200)
                    {


                        Helper.SetDialogue(100, "TELL ME HOW YOU CAN HIT OR MISS, BUT YOU NEVER MISS, HUH?!?", Color.LawnGreen);
                    }
                    else if (AITimer == 300)
                    {


                        Helper.SetDialogue(100, "YOU SON OF A #####!!!", Color.LawnGreen);
                    }
                    else if (AITimer == 400)
                    {


                        Helper.SetDialogue(100, "TELL ME, IF YOU CAN HIT OR MISS, HOW CAN YOU NEVER MISS, HUUUUH-", Color.LawnGreen);
                    }
                    else if (AITimer == 500)
                    {
                        for (int i = 0; i < 40; i++)
                        {
                            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CursedTorch, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                        }
                        Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Bottom, Vector2.Zero, ModContent.ProjectileType<TExplosion>(), 0, 0).scale = 2f;
                        SoundEngine.PlaySound(new("EbonianMod/Sounds/Eggplosion"));
                        /*Vector2 spawnAt = new Vector2(NPC.Center.X, NPC.Center.Y + 80);
                        var funny = Main.npc[NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y, ModContent.NPCType<FatDudeDeath>())];
                        Item.NewItem(NPC.getRect(), ItemID.PlatinumCoin);
                        if (NPC.AnyNPCs(ModContent.NPCType<NPCs.Boss.ExolBoss>()))
                        {
                            ExolBoss.DoAFunny();
                        }
                        funny.localAI[0] = 69;
                        NPC.life = 0;*/
                    }
                    else if (AITimer == 501)
                    {
                        NPC.dontTakeDamage = false;
                        NPC.HitInfo hit = new NPC.HitInfo()
                        {
                            Damage = 9999999
                        };
                        NPC.StrikeNPC(hit);
                    }
                }
            }
        }
    }
}