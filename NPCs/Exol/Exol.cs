using EbonianMod.Projectiles;
using EbonianMod;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Terraria.DataStructures;
using System.Collections.Generic;
using Terraria.Graphics.Shaders;
using static Terraria.ModLoader.PlayerDrawLayer;
using EbonianMod.Projectiles.VFXProjectiles;
using EbonianMod.Dusts;
using EbonianMod.Projectiles.Exol;
using System.Net.Http.Headers;
using EbonianMod.NPCs.Corruption;
using System.IO;

namespace EbonianMod.NPCs.Exol
{
    public class Exol : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            //Main.npcFrameCount[Type] = 8;
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "EbonianMod/NPCs/Exol/Exol_bestiary",
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
                new FlavorTextBestiaryInfoElement("Type: Unknown"),
                new FlavorTextBestiaryInfoElement("A boiling, blazing monster who terrorizes hell through sheer spite and rage. Its purpose and origin are unclear and most are content not knowing."),
            });
        }
        public override void SetDefaults()
        {
            NPC.width = 134;
            NPC.height = 148;
            NPC.damage = 0;
            NPC.defense = 20;
            NPC.lifeMax = 30000;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Exol");
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D a = Helper.GetTexture("NPCs/Exol/Exol_Pulse");
            Texture2D b = TextureAssets.Npc[Type].Value;
            Texture2D c = Helper.GetExtraTexture("Sprites/Exol_Glow");
            Texture2D d = Helper.GetTexture("NPCs/Exol/Exol_eye");
            Texture2D e = Helper.GetTexture("NPCs/Exol/Exol_bestiary");
            Texture2D f = Helper.GetTexture("NPCs/Exol/Exol_Trail");
            if (!NPC.hide)
            {
                Player player = Main.player[NPC.target];
                var fadeMult = 1f / NPCID.Sets.TrailCacheLength[NPC.type];
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {

                    spriteBatch.Reload(BlendState.Additive);
                    Color color = Color.Lerp(Color.OrangeRed, Color.White, 1f - fadeMult * i);
                    Main.spriteBatch.Draw(f, NPC.oldPos[i] - Main.screenPosition + new Vector2(NPC.width / 2f, NPC.height / 2f), null, drawColor * (1f - fadeMult * i), NPC.rotation, f.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
                    spriteBatch.Reload(BlendState.AlphaBlend);
                }
                Main.EntitySpriteDraw(b, NPC.Center - Main.screenPosition, null, drawColor, NPC.rotation, b.Size() / 2, 1, SpriteEffects.None, 0);
                spriteBatch.Reload(BlendState.Additive);
                Main.EntitySpriteDraw(c, NPC.Center - Main.screenPosition, null, Color.White * 0.75f, NPC.rotation, c.Size() / 2, 1, SpriteEffects.None, 0);
                spriteBatch.Reload(BlendState.AlphaBlend);
                Vector2 ogEyePos = NPC.Center + new Vector2(4, 5);
                Vector2 eyePos = ogEyePos;
                Vector2 fromTo = Helper.FromAToB(ogEyePos, pointOfInterest);

                //forced point of interest, remove later once ai is finished.
                if (AIState != Death)
                    pointOfInterest = player.Center;

                float dist = MathHelper.Clamp(Helper.FromAToB(ogEyePos, pointOfInterest, false).Length() * 0.1f, 0, 11);
                float distY = MathHelper.Clamp(Helper.FromAToB(ogEyePos, pointOfInterest, false).Length() * 0.1f, 0, 2);
                //if (AIState != Death)
                {
                    eyePos.X += dist * fromTo.X;
                    eyePos.Y += distY * fromTo.Y;
                }
                /*else
                {
                    eyePos += new Vector2(Main.rand.NextFloat(-11, 11), Main.rand.NextFloat(-2, 2));
                }*/
                Main.EntitySpriteDraw(d, eyePos - Main.screenPosition, null, Color.White, NPC.rotation, d.Size() / 2, 1, SpriteEffects.None, 0);
            }
            if (AIState == Death)
            {
                spriteBatch.Reload(BlendState.Additive);
                if (!NPC.hide)
                    for (int i = 0; i < 5; i++)
                        Main.EntitySpriteDraw(a, NPC.Center - Main.screenPosition, null, Color.White * deathAlpha, NPC.rotation, a.Size() / 2, 1, SpriteEffects.None, 0);

                spriteBatch.Reload(BlendState.AlphaBlend);
            }

            return false;
        }
        float deathAlpha;
        bool ded;
        Vector2 pointOfInterest;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(pointOfInterest);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            pointOfInterest = reader.ReadVector2();
        }
        public override bool CheckDead()
        {
            if (NPC.life <= 0 && !ded)
            {
                NPC.life = 1;
                AIState = Death;
                NPC.frameCounter = 0;
                NPC.immortal = true;
                NPC.dontTakeDamage = true;
                EbonianSystem.ChangeCameraPos(NPC.Center, 300);
                EbonianSystem.ScreenShakeAmount = 20;
                ded = true;
                AITimer = AITimer2 = 0;
                NPC.velocity = Vector2.Zero;
                NPC.life = 1;
                return false;
            }
            return true;
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
        public float AITimer3
        {
            get => NPC.ai[3];
            set => NPC.ai[3] = value;
        }
        const int Death = -1, Spawn = 0, Idle = 1, FlameThrower = 2, RocksAtPlayer = 3, RockFall = 4, RandomLasers = 5, SpinDash = 6, RepeatedBanging = 7;
        Vector2 lastPos;
        SoundStyle summon = new("EbonianMod/Sounds/ExolSummon");
        SoundStyle roar = new("EbonianMod/Sounds/ExolRoar")
        {
            PitchVariance = 0.25f,
        };
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            if (player.dead || !player.active || !player.ZoneUnderworldHeight)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (NPC.HasValidTarget)
                {
                    AIState = Spawn;
                    AITimer = 0;
                }
                if (player.dead || !player.active || !player.ZoneUnderworldHeight)
                {
                    NPC.velocity.Y = 30;
                    NPC.timeLeft = 10;
                    NPC.active = false;
                }
                return;
            }
            if (Main.rand.NextBool(5))
                if (NPC.life < NPC.lifeMax / 4)
                {
                    Helper.DustExplosion(NPC.Center, Vector2.One, 2, Color.Gray * 0.45f, false, false, 0.6f, 0.5f, new(Main.rand.NextFloat(-4, 4), -10));
                }
                else if (NPC.life < NPC.lifeMax / 3)
                {
                    Helper.DustExplosion(NPC.Center, Vector2.One, 2, Color.Gray * 0.35f, false, false, 0.4f, 0.5f, -Vector2.UnitY * Main.rand.NextFloat(6, 8));
                }
                else if (NPC.life < NPC.lifeMax / 2)
                {
                    Helper.DustExplosion(NPC.Center, Vector2.One, 2, Color.Gray * 0.25f, false, false, 0.2f, 0.5f, -Vector2.UnitY * Main.rand.NextFloat(4, 8));
                }

            if (AIState == Death)
            {
                AITimer++;
                if (AITimer == 180)
                {

                    Projectile a = Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GenericExplosion>(), 0, 0);
                }
                if (AITimer == 200)
                {

                    EbonianMod.FlashAlpha = 1;
                    //Projectile a = Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ScreenFlash>(), 0, 0);
                    //a.ai[0] = 1;
                }
                if (AITimer < 40)
                {
                    pointOfInterest = NPC.Center;
                }
                else if (AITimer < 150 & AITimer > 40)
                {
                    pointOfInterest = NPC.Center + Main.rand.NextVector2CircularEdge(250, 250);
                }
                else
                {

                    pointOfInterest = NPC.Center + Vector2.UnitY * 100;
                }
                if (AITimer > 212)
                {
                    NPC.hide = true;
                }
                if (AITimer == 312)
                {

                    NPC.immortal = false;
                    NPC.life = 0;
                    NPC.checkDead();
                }
                if (AITimer > 50)
                {
                    if (deathAlpha < 1)
                        deathAlpha += 0.005f;
                    if (Main.rand.NextBool(deathAlpha < 0.5 ? 5 : 2) && !NPC.hide)
                    {
                        Vector2 pos = NPC.Center + 300 * Main.rand.NextVector2Unit();
                        Dust a = Dust.NewDustPerfect(pos, ModContent.DustType<GenericAdditiveDust>(), Helper.FromAToB(pos, NPC.Center) * Main.rand.NextFloat(10, 20), newColor: Color.Gold * deathAlpha, Scale: Main.rand.NextFloat(0.15f, 0.35f));
                        a.noGravity = false;
                        a.customData = 1;
                    }
                }
                //if (AITimer == 190)
                //  EbonianPlayer.Instance.FlashScreen(NPC.Center, 35);
            }
            else if (AIState == Spawn)
            {
                AITimer++;
                if (AITimer == 1)
                {
                    //Helper.SetBossTitle(180, "Exol", Color.OrangeRed);
                    EbonianSystem.ChangeCameraPos(NPC.Center, 180);
                    SoundEngine.PlaySound(summon);
                    EbonianSystem.ScreenShakeAmount = 15f;
                }
                if (AITimer >= 150)
                {
                    AITimer = 0;
                    AIState = FlameThrower;
                }
            }
            else if (AIState == FlameThrower)
            {
                NPC.damage = 0;
                AITimer++;
                if (AITimer == 1)
                {
                    EbonianSystem.ScreenShakeAmount = 5f;
                    SoundEngine.PlaySound(roar);
                }
                if (AITimer < 30)
                {
                    Vector2 pos = new Vector2(player.position.X, player.position.Y - 335);
                    Vector2 target = pos;
                    Vector2 moveTo = target - NPC.Center;
                    NPC.velocity = (moveTo) * 0.18f;
                }
                if (AITimer == 30)
                {
                    lastPos = player.Center;
                    NPC.velocity = Vector2.Zero;
                    Vector2 vector16 = NPC.DirectionTo(player.Center) * 7f;
                    for (float i = (-9); i <= 9; i++)
                    {
                        Projectile projectile2 = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, 7.5f * Utils.RotatedBy(NPC.DirectionTo(lastPos), (double)(MathHelper.ToRadians(32.5f) * (float)i), default(Vector2)), ModContent.ProjectileType<TelegraphLine>(), 0, 1f, Main.myPlayer)];
                        projectile2.timeLeft = 20;
                    }
                }
                if (AITimer == 60)
                {
                    NPC.velocity = Vector2.Zero;
                    Vector2 vector16 = NPC.DirectionTo(player.Center) * 7f;
                    for (float i = (-9); i <= 9; i++)
                    {

                        Projectile projectile = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, 9.5f * Utils.RotatedBy(NPC.DirectionTo(lastPos), (double)(MathHelper.ToRadians(32.5f) * (float)i), default(Vector2)), ModContent.ProjectileType<EFireBreath>(), 20, 1f, Main.myPlayer)];
                        projectile.tileCollide = false;
                        projectile.friendly = false;
                        projectile.hostile = true;
                        projectile.ai[1] = 0.7f;
                        projectile.timeLeft = 230;
                    }
                }

                if (AITimer >= 100)
                {
                    NPC.rotation = 0;
                    NPC.velocity.X = 0;
                    NPC.velocity.Y = 0;
                    AITimer = 0;
                    AITimer2 = 0;
                    AITimer3 = 0;
                    AIState = RocksAtPlayer;
                }
            }
            else if (AIState == RocksAtPlayer)
            {
                AITimer++;
                if (AITimer == 1)
                    SoundEngine.PlaySound(roar);
                Vector2 pos = new Vector2(player.position.X, player.position.Y - 335);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.18f;
                if (AITimer < 100 && AITimer % 20 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(Main.screenPosition.X + Main.screenWidth * Main.rand.NextFloat(), Main.screenPosition.Y + Main.screenHeight), -Vector2.UnitY * (10 + Main.rand.NextFloat(4, 25)), ModContent.ProjectileType<EBoulder>(), 30, 0);
                }
                if (AITimer >= 100)
                {
                    NPC.rotation = 0;
                    NPC.velocity.X = 0;
                    NPC.velocity.Y = 0;
                    AITimer = 0;
                    AITimer2 = 0;
                    AITimer3 = 0;
                    AIState = RockFall;
                }
            }
            else if (AIState == RockFall)
            {
                AITimer++;
                if (AITimer == 1)
                    SoundEngine.PlaySound(roar);
                if (AITimer == 30)
                {
                    NPC.noTileCollide = false;
                    NPC.velocity = -Vector2.UnitY * 30;
                }
                if (NPC.collideY || NPC.collideX || NPC.Center.Y < Main.UnderworldLayer * 16)
                {
                    EbonianSystem.ScreenShakeAmount = 10;
                    NPC.velocity = Vector2.UnitY * 5;
                }
                if (AITimer > 60)
                    NPC.velocity *= 0.9f;
                if (AITimer % 10 == 0 && AITimer > 60)
                {
                    Vector2 pos = new Vector2(Main.screenPosition.X + Main.screenWidth * Main.rand.NextFloat(), Main.screenPosition.Y - 300);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, Vector2.UnitY * 10, ModContent.ProjectileType<EBoulder2>(), 30, 0);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, Vector2.UnitY * 10, ModContent.ProjectileType<TelegraphLine>(), 0, 0);
                }
                if (AITimer >= 500)
                {
                    NPC.noTileCollide = true;
                    NPC.rotation = 0;
                    NPC.velocity.X = 0;
                    NPC.velocity.Y = 0;
                    AITimer = 0;
                    AITimer2 = 0;
                    AITimer3 = 0;
                    AIState = RandomLasers;
                }
            }
        }
    }
}