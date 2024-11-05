using EbonianMod.Common.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using EbonianMod.Common.Systems.Misc;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using EbonianMod.Projectiles.Cecitior;
using EbonianMod.Projectiles.Friendly.Corruption;
using Mono.Cecil;
using EbonianMod.Projectiles.Terrortoma;
using EbonianMod.Projectiles.VFXProjectiles;
using EbonianMod.NPCs.Corruption.Ebonflies;
using EbonianMod.Items.Misc;
using EbonianMod.Projectiles.Enemy.Corruption;
using EbonianMod.Projectiles.Conglomerate;
using EbonianMod.Dusts;
using Terraria.GameContent;
using EbonianMod.Items.Weapons.Magic;
using EbonianMod.Projectiles.Friendly.Crimson;

namespace EbonianMod.NPCs.Conglomerate
{
    public class Conglomerate : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 102;
            NPC.lifeMax = 7000;
            NPC.damage = 40;
            NPC.noTileCollide = true;
            NPC.defense = 23;
            NPC.knockBackResist = 0;
            NPC.rarity = 999;
            NPC.npcSlots = 1f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.aiStyle = -1;
            SoundStyle death = EbonianSounds.cecitiorDie;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = death;
            NPC.buffImmune[BuffID.CursedInferno] = true;
            NPC.buffImmune[BuffID.Ichor] = true;
            NPC.netAlways = true;
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/EvilMinibossSlow");
        }
        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter % 5 == 0)
            {
                hookFrame++;
                if (hookFrame > 7 || hookFrame < 1)
                    hookFrame = 1;
            }
        }
        Vector2 openOffset;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy) return false;
            Texture2D tex = Helper.GetTexture(Texture);
            Texture2D glow = Helper.GetTexture(Texture + "_Glow");

            Texture2D part = Helper.GetTexture("NPCs/Conglomerate/Conglomerate_PartT");
            Texture2D part2 = Helper.GetTexture("NPCs/Conglomerate/Conglomerate_PartC");
            Texture2D partTeeth = Helper.GetTexture("NPCs/Conglomerate/Conglomerate_PartT_Teeth");
            Texture2D part2Teeth = Helper.GetTexture("NPCs/Conglomerate/Conglomerate_PartC_Teeth");
            Texture2D partGlow = Helper.GetTexture("NPCs/Conglomerate/Conglomerate_PartT_Glow");
            Texture2D part2Glow = Helper.GetTexture("NPCs/Conglomerate/Conglomerate_PartC_Glow");
            if (tendonVerlet[0] != null)
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
                            offset2 = new Vector2(-5, 20);
                            break;
                        case 3:
                            scale = 3;
                            offset = new Vector2(-15, -10);
                            offset2 = new Vector2(-15, -20);
                            break;
                        case 4:
                            scale = 1;
                            offset = new Vector2(1, 20);
                            offset2 = new Vector2(-10, 32);
                            break;
                    }
                    tendonVerlet[i].Update(NPC.Center + offset2.RotatedBy(NPC.rotation) - openOffset, NPC.Center + openOffset + new Vector2(30, 4) + offset.RotatedBy(openRotation));
                    if (tendonVerlet[i].segments[10].cut)
                        tendonVerlet[i].Draw(spriteBatch, "Extras/Line", useColor: true, color: Color.Lerp(Color.Maroon, Color.DarkGreen, (float)i / 5) * 0.25f, scale: scale);
                }
            }
            DrawVerlets(spriteBatch);
            if (openOffset.Length() > 1 || !Helper.CloseTo(openRotation, 0, MathHelper.ToRadians(5)))
            {
                spriteBatch.Draw(partTeeth, NPC.Center + new Vector2(30, 0) + openOffset - screenPos, null, drawColor, openRotation, part.Size() / 2, NPC.scale, SpriteEffects.None, 0);
                spriteBatch.Draw(part2Teeth, NPC.Center - openOffset - screenPos, null, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
                if (tendonVerlet[0] != null)
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
                        if (!tendonVerlet[i].segments[10].cut)
                            tendonVerlet[i].Draw(spriteBatch, "Extras/Line", useColor: true, color: Color.Lerp(Color.Maroon, Color.DarkGreen, (float)i / 5) * 0.25f, scale: scale);
                    }
                }
                spriteBatch.Draw(part, NPC.Center + new Vector2(30, 0) + openOffset - screenPos, null, drawColor, openRotation, part.Size() / 2, NPC.scale, SpriteEffects.None, 0);
                spriteBatch.Draw(part2, NPC.Center - openOffset - screenPos, null, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
                spriteBatch.Draw(partGlow, NPC.Center + new Vector2(30, 0) + openOffset - screenPos, null, Color.White, openRotation, part.Size() / 2, NPC.scale, SpriteEffects.None, 0);
                spriteBatch.Draw(part2Glow, NPC.Center - openOffset - screenPos, null, Color.White, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
                spriteBatch.Draw(glow, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
            }
            return false;
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
        const int Intro = 0, Idle = 1,
            // Generic Attacks
            BloodAndWormSpit = 2, HomingEyeAndVilethornVomit = 3, BloatedEbonflies = 4, CursedFlameRain = 5, DashAndChomp = 6, ExplodingBalls = 7,
            BayleLaser = 8, SpinAroundThePlayerAndDash = 9, SpikesCloseIn = 10, BothHalvesRainCloseIn = 16, DashSpam = 12, OstertagiExplosion = 13,
            IchorBomb = 14, LastPrismLaser = 15,
            // Phase 2
            ClawAtFlesh = 16, GrabAttack = 17, ClingerHipfire = 18, EyeBeamPlusHomingEyes = 19, ClawTantrum = 20, SpineDashFollowedByMainDash = 21,
            ClingerWaveFire = 22, SpineWormVomit = 23, ClawPlucksBombsFromSpine = 24, BitesEyeToRainBlood = 25, ClingerComboType1 = 26, ClingerComboType2 = 27,
            ClingerComboType3 = 28, SmallDeathRay = 29, EyeSendsWavesOfHomingEyes = 30, BigBomb = 31, EyeSpin = 32, SlamSpineAndEyeTogether = 33,
            BodySlamTantrum = 34, ClawTriangle = 35, SpineChargedFlame = 36,

            Death = -2, PhaseTransition = -1;
        SlotId savedSound;
        Vector2[] disposablePos = new Vector2[10];
        float rotation, openRotation;
        bool phase2, open;
        public override void AI()
        {
            bool t = true;
            if (NPC.life < NPC.lifeMax / 2 + NPC.lifeMax / 4 && !phase2)
            {
                Reset();
                foreach (Projectile projectile in Main.projectile)
                {
                    if (projectile.hostile && projectile.active)
                        projectile.Kill();
                }
                phase2 = true;
                AIState = PhaseTransition;
            }
            if (Main.dayTime)
            {
                Main.time = 31400;
                Main.UpdateTime_StartNight(ref t);
            }
            MiscFX();
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)// || !player.ZoneCorrupt)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (NPC.HasValidTarget)
                {
                    if (AIState != Intro)
                        AIState = Idle;
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

            if (open)
            {
                NPC.damage = 0;
            }
            else
            {
                openOffset = Vector2.Lerp(openOffset, Vector2.Zero, 0.5f);
                if (openOffset.Length() < 1.5f && openOffset.Length() > 1f && openRotation.CloseTo(0, MathHelper.ToRadians(10)))
                {
                    SoundEngine.PlaySound(EbonianSounds.chomp1, NPC.Center);
                    SoundEngine.PlaySound(EbonianSounds.fleshHit, NPC.Center);
                    EbonianSystem.ScreenShakeAmount = 5;
                }
                if (openOffset != Vector2.Zero)
                    if (player.Center.Distance(NPC.Center - openOffset) < 75 || player.Center.Distance(NPC.Center + openOffset) < 75)
                        player.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), 150, 0);
            }

            NPC.rotation = Helper.LerpAngle(NPC.rotation, rotation, 0.05f);
            AITimer++;
            switch (AIState)
            {
                case Death:
                    {
                        rotation = Helper.FromAToB(NPC.Center, player.Center).ToRotation() - MathHelper.PiOver2;
                        if (AITimer == 1)
                        {
                            SoundEngine.PlaySound(EbonianSounds.BeamWindUp, NPC.Center);
                            disposablePos[0] = NPC.Center;
                        }
                        if (AITimer < 60 && AITimer > 1)
                            NPC.Center = disposablePos[0] + Main.rand.NextVector2Circular(10, 10);
                        if (AITimer == 40)
                            Projectile.NewProjectile(null, NPC.Center + new Vector2(7, -14).RotatedBy(NPC.rotation), Vector2.Zero, ModContent.ProjectileType<ChargeUp>(), 0, 0);
                        if (AITimer == 60)
                            Projectile.NewProjectile(null, NPC.Center + new Vector2(7, -14).RotatedBy(NPC.rotation), Vector2.Zero, ModContent.ProjectileType<GreenChargeUp>(), 0, 0);

                        if (AITimer == 80)
                        {

                        }
                    }
                    break;
                case PhaseTransition:
                    {
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/EvilMinibossMessy");
                    }
                    break;
                case Intro:
                    {
                        if (AITimer >= 10)
                        {
                            AIState = BothHalvesRainCloseIn;
                            Reset();
                        }
                    }
                    break;
                case BloodAndWormSpit:
                    {
                        rotation = Helper.FromAToB(NPC.Center, player.Center).ToRotation() - MathHelper.PiOver2;
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Zero, 0.2f);
                        Vector2 vel = NPC.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2).RotatedByRandom(MathHelper.PiOver2 * 0.7f);
                        if (AITimer == 20)
                        {
                            AITimer2 = Main.rand.Next(2);
                        }
                        if (AITimer == 30)
                        {
                            if (AITimer2 == 0)
                            {
                                NPC.velocity = -vel * 5;
                                Projectile a = Projectile.NewProjectileDirect(null, NPC.Center + vel * 20, vel * Main.rand.NextFloat(4, 6), ModContent.ProjectileType<OstertagiWorm>(), 30, 0, 0, Main.rand.NextFloat(0.15f, 0.5f));
                                a.friendly = false;
                                a.hostile = true;
                            }
                            else
                                Projectile.NewProjectile(null, NPC.Center + vel * 20, vel * 6, ModContent.ProjectileType<TerrorVilethorn1>(), 25, 0);
                        }
                        if (AITimer >= 80 && AITimer % (phase2 ? 5 : 7) == 0 && AITimer < 216)
                        {
                            if (AITimer % 2 == 0)
                            {
                                float angleScale = MathHelper.SmoothStep(0.5f, 0, (float)(AITimer - 165) / 55);
                                SoundEngine.PlaySound(EbonianSounds.xSpirit.WithPitchOffset(0.4f).WithVolumeScale(1.2f), NPC.Center);
                                Projectile.NewProjectile(null, NPC.Center + new Vector2(7, -14).RotatedBy(NPC.rotation), vel.RotatedByRandom(MathHelper.PiOver2 * angleScale) * Main.rand.NextFloat(.5f, 1f), ModContent.ProjectileType<RegorgerBolt>(), 10, 0);
                            }
                            NPC.velocity = -vel * Main.rand.NextFloat(2f, 5);

                            if (AITimer2 == 0)
                            {
                                Projectile a = Projectile.NewProjectileDirect(null, NPC.Center + vel * 20, vel * Main.rand.NextFloat(4, 6), ModContent.ProjectileType<OstertagiWorm>(), 30, 0, 0, Main.rand.NextFloat(0.15f, 0.5f));
                                a.friendly = false;
                                a.hostile = true;
                            }
                            else
                                Projectile.NewProjectile(null, NPC.Center + vel * 20, vel * 6, ModContent.ProjectileType<TerrorVilethorn1>(), 25, 0);
                        }
                        if (AITimer >= 240)
                        {
                            AIState = HomingEyeAndVilethornVomit;
                            Reset();
                        }
                    }
                    break;
                case HomingEyeAndVilethornVomit:
                    {
                        if (AITimer < 100 && AITimer > 20)
                        {
                            open = true;
                            if (openOffset.X < 25)
                            {
                                openOffset.X += 3.5f;
                            }
                            openRotation = MathHelper.ToRadians(MathF.Sin((AITimer + 50) * 4) * 10);

                            rotation = MathHelper.ToRadians(MathF.Sin(-(AITimer + 50) * 4) * 10);
                            NPC.rotation = MathHelper.ToRadians(MathF.Sin(-(AITimer + 50) * 4) * 10);
                            if (AITimer % 10 == 0)
                                Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<ConglomerateScream>(), 0, 0);
                        }
                        else
                        {
                            open = false;
                            openOffset = Vector2.Lerp(openOffset, Vector2.Zero, 0.2f);
                            rotation = Helper.LerpAngle(rotation, 0, 0.2f);
                            openRotation = Helper.LerpAngle(openRotation, 0, 0.2f);
                        }
                        if (AITimer > 50 && AITimer % (phase2 ? 10 : 15) == 0 && AITimer < 200)
                        {
                            Vector2 pos = player.Center + new Vector2(700 * (Main.rand.NextBool() ? 1 : -1), 0).RotatedByRandom(MathHelper.PiOver4 * 0.5f);
                            Projectile.NewProjectile(null, pos, Helper.FromAToB(pos, player.Center), ModContent.ProjectileType<CecitiorEyeP>(), 25, 0);
                            pos = player.Center + new Vector2(0, 700 * (Main.rand.NextBool() ? 1 : -1)).RotatedByRandom(MathHelper.PiOver4);
                            Projectile.NewProjectile(null, pos, Helper.FromAToB(pos, player.Center), ModContent.ProjectileType<TerrorVilethorn1>(), 25, 0);
                        }
                        if (AITimer >= 230)
                        {
                            AIState = BloatedEbonflies;
                            Reset();
                        }
                    }
                    break;
                case BloatedEbonflies:
                    {
                        if (AITimer == 40)
                        {
                            EbonianSystem.ScreenShakeAmount = 10f;
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<OstertagiExplosion>(), 0, 0);
                        }

                        if (AITimer == 70 || AITimer == 80 || AITimer == 140)
                        {
                            EbonianSystem.ScreenShakeAmount = 10f;
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<OstertagiExplosion>(), 0, 0);
                            for (int i = -3; i < 4; i++)
                                NPC.NewNPCDirect(null, NPC.Center + Vector2.One.RotatedBy(i) * 30, ModContent.NPCType<BloatedEbonfly>());
                        }

                        if (AITimer >= 150)
                        {
                            AIState = CursedFlameRain;
                            Reset();
                        }
                    }
                    break;
                case CursedFlameRain:
                    {
                        rotation = -Vector2.UnitY.ToRotation() - MathHelper.PiOver2;
                        if (AITimer <= 160)
                        {
                            Vector2 pos = new Vector2(player.position.X, player.position.Y - 75);
                            Vector2 target = pos;
                            Vector2 moveTo = target - NPC.Center;
                            NPC.velocity = (moveTo) * 0.0445f;
                            if (++AITimer2 % 40 == 0)
                            {
                                for (int i = 0; i <= 5 + (Main.expertMode ? 5 : 0); i++)
                                {
                                    Projectile projectile = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat(-10f, 10f), -10), ModContent.ProjectileType<TFlameThrower2>(), 30, 1f, Main.myPlayer)];
                                    projectile.tileCollide = false;
                                    projectile.hostile = true;
                                    projectile.friendly = false;
                                    projectile.timeLeft = 230;
                                }
                                Projectile projectileb = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -10), ModContent.ProjectileType<TFlameThrower2>(), 30, 1f, Main.myPlayer)];
                                projectileb.tileCollide = false;
                                projectileb.hostile = true;
                                projectileb.friendly = false;
                                projectileb.timeLeft = 230;

                                for (int i = 0; i <= 5 + (Main.expertMode ? 5 : 0); i++)
                                {
                                    Projectile projectile = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat(-10f, 10f), -6), ModContent.ProjectileType<CIchor>(), 30, 1f, Main.myPlayer)];
                                    projectile.tileCollide = false;
                                    projectile.hostile = true;
                                    projectile.friendly = false;
                                    projectile.timeLeft = 230;
                                }
                                Projectile projectilec = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -6), ModContent.ProjectileType<CIchor>(), 30, 1f, Main.myPlayer)];
                                projectilec.tileCollide = false;
                                projectilec.hostile = true;
                                projectilec.friendly = false;
                                projectilec.timeLeft = 230;
                            }
                        }
                        else NPC.velocity *= 0.9f;

                        if (AITimer >= 180)
                        {
                            AIState = DashAndChomp;
                            Reset();
                        }
                    }
                    break;
                case DashAndChomp:
                    {
                        //do thing
                        if (AITimer < 25)
                        {
                            open = true;
                            openOffset += Vector2.UnitX * 8;
                            NPC.velocity = Helper.FromAToB(NPC.Center, player.Center, false) / 10f;
                        }
                        if (AITimer > 25 && AITimer < 50)
                            NPC.velocity = Helper.FromAToB(NPC.Center, player.Center, false) / 5f;
                        if (AITimer == 50)
                        {
                            Projectile.NewProjectile(null, NPC.Center + openOffset, Vector2.Zero, ModContent.ProjectileType<ConglomerateScream>(), 0, 0);
                            Projectile.NewProjectile(null, NPC.Center - openOffset, Vector2.Zero, ModContent.ProjectileType<ConglomerateScream>(), 0, 0);
                            NPC.velocity = Vector2.Zero;
                        }
                        if (AITimer == 60)
                            open = false;
                        if (AITimer > 80 && AITimer < 90)
                        {
                            open = true;
                            openOffset += Vector2.UnitX * 15;
                            NPC.velocity = Helper.FromAToB(NPC.Center, player.Center, false) / 5f;
                        }
                        if (AITimer == 90)
                        {
                            Projectile.NewProjectile(null, NPC.Center + openOffset, Vector2.Zero, ModContent.ProjectileType<ConglomerateScream>(), 0, 0);
                            Projectile.NewProjectile(null, NPC.Center - openOffset, Vector2.Zero, ModContent.ProjectileType<ConglomerateScream>(), 0, 0);
                            NPC.velocity = Vector2.Zero;
                        }
                        if (AITimer == 110)
                            open = false;
                        if (AITimer > 130)
                            rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
                        if (AITimer == 120)
                        {
                            rotation = Helper.FromAToB(NPC.Center, player.Center).ToRotation() - MathHelper.PiOver2;
                            disposablePos[0] = player.Center;
                        }
                        if (AITimer == 130)
                        {
                            SoundEngine.PlaySound(EbonianSounds.terrortomaDash, NPC.Center);
                            for (int i = 0; i < 20; i++)
                            {
                                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CursedTorch, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Ichor, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                            }
                            NPC.velocity = Helper.FromAToB(NPC.Center, disposablePos[0]) * 2.5f;
                        }
                        if (AITimer > 140 && AITimer < 150)
                        {
                            NPC.velocity += Helper.FromAToB(NPC.Center, player.Center) * 2.5f;
                            NPC.damage = 120;
                        }
                        if (AITimer2 > 180 && AITimer < 220)
                        {
                            NPC.velocity *= 0.8f;
                            disposablePos[0] = player.Center;
                        }

                        if (AITimer == 220)
                        {
                            SoundEngine.PlaySound(EbonianSounds.terrortomaDash, NPC.Center);
                            for (int i = 0; i < 20; i++)
                            {
                                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CursedTorch, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Ichor, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                            }
                            NPC.velocity = Helper.FromAToB(NPC.Center, disposablePos[0]) * 2.5f;
                        }
                        if (AITimer > 230 && AITimer < 240)
                        {
                            NPC.velocity += Helper.FromAToB(NPC.Center, player.Center) * 3f;
                            NPC.damage = 120;
                        }
                        if (AITimer2 > 260)
                            NPC.velocity *= 0.8f;

                        if (AITimer >= 300)
                        {
                            AIState = ExplodingBalls;
                            Reset();
                        }
                    }
                    break;
                case ExplodingBalls:
                    {
                        rotation = -Vector2.UnitY.ToRotation() - MathHelper.PiOver2;
                        if (AITimer > 50 && AITimer % (phase2 ? 2 : 4) == 0 && AITimer < 90)
                        {
                            Vector2 pos = player.Center - new Vector2(Main.rand.NextFloat(-1200, 1200), 1000);
                            Projectile.NewProjectile(null, NPC.Center + -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * 30, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(10, 20), (Main.rand.NextBool() ? ModContent.ProjectileType<TerrorStaffPEvil>() : ModContent.ProjectileType<CecitiorBombThing>()), 20, 0);
                        }
                        if (AITimer >= 110)
                        {
                            AIState = BayleLaser;
                            Reset();
                        }
                    }
                    break;
                case BayleLaser:
                    {
                        if (AITimer > 30)
                        {
                            if (!(AITimer < 165 && AITimer > 140) && AITimer < 320)
                                disposablePos[0] = Vector2.Lerp(disposablePos[0], player.Center, 0.05f);
                            if (AITimer < 320)
                                rotation = Helper.FromAToB(NPC.Center, disposablePos[0]).ToRotation() - MathHelper.PiOver2;
                            else
                                rotation = Vector2.UnitY.ToRotation() - MathHelper.PiOver2 + MathHelper.ToRadians(20);
                        }
                        if (AITimer == 30)
                        {
                            disposablePos[0] = player.Center;
                        }
                        if (AITimer == 45)
                            SoundEngine.PlaySound(EbonianSounds.BeamWindUp, NPC.Center);
                        if (AITimer == 100)
                            Projectile.NewProjectile(null, NPC.Center + new Vector2(7, -14).RotatedBy(NPC.rotation), Vector2.Zero, ModContent.ProjectileType<ChargeUp>(), 0, 0);
                        if (AITimer == 110)
                            Projectile.NewProjectile(null, NPC.Center + new Vector2(7, -14).RotatedBy(NPC.rotation), Vector2.Zero, ModContent.ProjectileType<GreenChargeUp>(), 0, 0);

                        if (AITimer == 140)
                        {
                            SoundEngine.PlaySound(EbonianSounds.chargedBeamWindUp, NPC.Center);
                            Projectile.NewProjectile(NPC.InheritSource(NPC), NPC.Center, (rotation + MathHelper.PiOver2).ToRotationVector2(), ModContent.ProjectileType<VileTearTelegraph>(), 0, 0);
                        }
                        if (AITimer == 165)
                        {
                            EbonianSystem.ScreenShakeAmount = 10f;
                            SoundStyle s = EbonianSounds.chargedBeamImpactOnly;
                            SoundEngine.PlaySound(s.WithPitchOffset(0.2f), NPC.Center);
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<ConglomerateScream>(), 0, 0);
                            Projectile.NewProjectile(null, NPC.Center + new Vector2(7, -14).RotatedBy(NPC.rotation), Vector2.Zero, ModContent.ProjectileType<OstertagiExplosion>(), 0, 0);
                            Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, disposablePos[0]), ModContent.ProjectileType<CBeam>(), 100, 0, -1, 0, 0, NPC.whoAmI);
                        }
                        if (AITimer > 160 && AITimer < 220)
                            NPC.velocity = Helper.FromAToB(disposablePos[0], NPC.Center) * 0.5f;
                        if ((AITimer > 220 && AITimer < 240) || (AITimer > 340 && AITimer < 360))
                            NPC.velocity *= 0.9f;
                        if (AITimer == 320)
                            NPC.velocity = -Vector2.UnitY.RotatedBy(-MathHelper.PiOver4) * 8f;
                        if (AITimer > 320 && AITimer < 390)
                        {
                            for (int i = 0; i < 1 + (AITimer * 0.025f); i++)
                            {
                                Vector2 pos = NPC.Center + Helper.FromAToB(NPC.Center, NPC.Center + (NPC.rotation + MathHelper.PiOver2).ToRotationVector2() * 10).RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(100, 600);
                                Dust.NewDustPerfect(pos, ModContent.DustType<LineDustFollowPoint>(), Helper.FromAToB(pos, NPC.Center).RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(15, 35), 0, Color.Lerp(Color.LimeGreen, Color.Maroon, Main.rand.NextFloat()), Main.rand.NextFloat(0.06f, .15f)).customData = NPC.Center;
                            }
                        }
                        if (AITimer == 360)
                        {
                            SoundEngine.PlaySound(EbonianSounds.chargedBeamWindUp, NPC.Center);
                            Projectile.NewProjectile(NPC.InheritSource(NPC), NPC.Center, (rotation + MathHelper.PiOver2).ToRotationVector2(), ModContent.ProjectileType<VileTearTelegraph>(), 0, 0);
                        }
                        if (AITimer == 385)
                        {
                            EbonianSystem.ScreenShakeAmount = 20f;
                            SoundStyle s = EbonianSounds.chargedBeamImpactOnly;
                            SoundEngine.PlaySound(s.WithPitchOffset(0.25f), NPC.Center);
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<ConglomerateScream>(), 0, 0);
                            Projectile.NewProjectile(null, NPC.Center + new Vector2(7, -14).RotatedBy(NPC.rotation), Vector2.Zero, ModContent.ProjectileType<OstertagiExplosion>(), 0, 0);
                            Projectile.NewProjectile(null, NPC.Center, (rotation + MathHelper.PiOver2).ToRotationVector2(), ModContent.ProjectileType<CBeam>(), 100, 0, -1, 0, 0, NPC.whoAmI);
                        }
                        if (AITimer > 385 && AITimer < 470)
                        {
                            disposablePos[0] = new Vector2((player.Center.X < NPC.Center.X ? 1 : -1), 0);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(15, 0), 0.05f);
                        }
                        if (AITimer > 470 && AITimer < 520)
                        {
                            NPC.rotation += MathHelper.ToRadians(MathHelper.SmoothStep(0.25f, 11f, (AITimer - 470) / 80) * disposablePos[0].X);
                            NPC.velocity *= 0.9f;
                        }

                        if (AITimer >= 550)
                        {
                            AIState = SpinAroundThePlayerAndDash;
                            Reset();
                        }
                    }
                    break;
                case SpinAroundThePlayerAndDash:
                    {
                        if (NPC.velocity.Length() > .1f)
                            rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
                        else
                            rotation = Helper.FromAToB(NPC.Center, player.Center).ToRotation() - MathHelper.PiOver2;

                        AITimer2++;

                        if (AITimer2 == 1)
                            disposablePos[0] = Main.rand.NextVector2Unit();
                        else if (AITimer2 < 60)
                        {
                            NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center + new Vector2(600, 0).RotatedBy(disposablePos[0].ToRotation() + MathHelper.ToRadians(AITimer2 * 4)), false) * 0.03f, 0.15f);
                        }
                        if (AITimer2 > 60 && AITimer2 < 70)
                            NPC.velocity *= 0.9f;
                        if (AITimer2 == 70)
                        {
                            SoundEngine.PlaySound(EbonianSounds.terrortomaDash, NPC.Center);
                            for (int i = 0; i < 20; i++)
                            {
                                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CursedTorch, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Ichor, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                            }
                            for (int i = 0; i < 3; i++)
                                Projectile.NewProjectile(null, NPC.Center + Main.rand.NextVector2CircularEdge(5, 5), Helper.FromAToB(NPC.Center, player.Center), ModContent.ProjectileType<CecitiorEyeP>(), 25, 0);
                            NPC.velocity = Helper.FromAToB(NPC.Center, player.Center) * 2.5f;
                        }
                        if (AITimer2 > 70 && AITimer2 < 80)
                        {
                            NPC.velocity += Helper.FromAToB(NPC.Center, player.Center) * 2.5f;
                            NPC.damage = 120;
                        }
                        if (AITimer2 > 130)
                        {
                            NPC.velocity *= 0.8f;
                        }
                        if (AITimer2 > 140)
                        {
                            NPC.damage = 0;
                            AITimer2 = 0;
                        }
                        if (AITimer >= 300)
                        {
                            AIState = SpikesCloseIn;
                            Reset();
                        }
                    }
                    break;
                case SpikesCloseIn:
                    {
                        Vector2 pos = new Vector2(player.position.X, player.position.Y - 175);
                        Vector2 target = pos;
                        Vector2 moveTo = target - NPC.Center;
                        NPC.velocity = (moveTo) * 0.0445f;
                        if (AITimer >= 20 && AITimer <= 25)
                        {
                            for (int i = -1; i < 2; i++)
                            {
                                if (i == 0) continue;
                                Vector2 _pos = NPC.Center + new Vector2(i * (AITimer - 20) * 200, -100 + MathHelper.SmoothStep(-50, 150, MathF.Abs(i * (AITimer - 20)) / 5f));
                                Vector2 vel = Helper.FromAToB(_pos, player.Center);
                                Projectile.NewProjectile(null, _pos, vel, ModContent.ProjectileType<CecitiorTeeth>(), 35, 0);
                                Projectile.NewProjectile(null, _pos, Vector2.Zero, ModContent.ProjectileType<CFlareExplosion>(), 0, 0);
                            }
                        }
                        if (AITimer >= 40 && AITimer <= 48)
                        {
                            for (int i = -1; i < 2; i++)
                            {
                                if (i == 0) continue;
                                Vector2 _pos = NPC.Center + new Vector2(i * (AITimer - 40) * 100, -150 + MathHelper.SmoothStep(-100, 150, MathF.Abs(i * (AITimer - 40)) / 7f));
                                Vector2 vel = Helper.FromAToB(_pos, player.Center) * 3;
                                Projectile.NewProjectile(null, _pos, vel, ModContent.ProjectileType<CecitiorTeeth>(), 35, 0);
                                Projectile.NewProjectile(null, _pos, Vector2.Zero, ModContent.ProjectileType<CFlareExplosion>(), 0, 0);
                            }
                        }
                        if (AITimer >= 100 && AITimer <= 110)
                        {
                            for (int i = -1; i < 2; i++)
                            {
                                if (i == 0) continue;
                                Vector2 _pos = NPC.Center + new Vector2(i * (AITimer - 100) * 200, -200 + MathHelper.SmoothStep(-100, 200, MathF.Abs(i * (AITimer - 100)) / 10f));
                                Vector2 vel = Helper.FromAToB(_pos, player.Center);
                                Projectile.NewProjectile(null, _pos, vel, ModContent.ProjectileType<CecitiorTeeth>(), 35, 0);
                                Projectile.NewProjectile(null, _pos, Vector2.Zero, ModContent.ProjectileType<CFlareExplosion>(), 0, 0);
                            }
                        }
                        if (AITimer >= 130)
                        {
                            AIState = BothHalvesRainCloseIn;
                            Reset();
                        }
                    }
                    break;
                case BothHalvesRainCloseIn:
                    {
                        Vector2 pos = new Vector2(player.position.X + 100, player.position.Y - 175);
                        Vector2 target = pos;
                        Vector2 moveTo = target - NPC.Center;
                        NPC.velocity = (moveTo) * 0.0745f;
                        if (AITimer < 100)
                        {
                            open = true;
                            rotation = -MathHelper.PiOver2;
                            openRotation = Helper.LerpAngle(openRotation, MathHelper.PiOver2, 0.05f);
                            openOffset = Vector2.Lerp(openOffset, new Vector2(600, 0), 0.1f);
                        }
                        if (AITimer > 100 && AITimer < 200)
                        {
                            openOffset = Vector2.Lerp(new Vector2(600, 0), new Vector2(50, 0), (AITimer - 100) / 100);
                            if (AITimer % 10 == 0)
                                Projectile.NewProjectile(null, NPC.Center + openOffset, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4 * 1.25f) * 5, ModContent.ProjectileType<CFlesh>(), 30, 0);
                            if (AITimer % 10 == 5)
                                Projectile.NewProjectile(null, NPC.Center - openOffset, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4 * 1.25f) * 5, ModContent.ProjectileType<CFlesh>(), 30, 0);
                        }
                        if (AITimer >= 200)
                        {
                            rotation = 0;
                            openRotation = Helper.LerpAngle(openRotation, 0, 0.25f);
                        }
                        if (AITimer == 220)
                            open = false;
                        if (AITimer >= 250)
                        {
                            AIState = DashSpam;
                            Reset();
                        }
                    }
                    break;
                case DashSpam:
                    {

                        if (AITimer < 100 && AITimer > 20)
                        {
                            NPC.damage = 150;
                            if (AITimer % 10 == 0)
                                Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<ConglomerateScream>(), 0, 0);
                        }

                        if (NPC.velocity.Length() > 0)
                            rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
                        if (AITimer > 100 && AITimer % 40 == 0 && AITimer < 211)
                        {
                            SoundEngine.PlaySound(EbonianSounds.terrortomaDash, NPC.Center);
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<ConglomerateScream>(), 0, 0);
                            NPC.velocity = Helper.FromAToB(NPC.Center, player.Center) * 23;
                        }
                        if (AITimer > 231)
                            NPC.velocity *= 0.9f;
                        if (AITimer >= 250)
                        {
                            AIState = OstertagiExplosion;
                            Reset();
                        }
                    }
                    break;
                case OstertagiExplosion:
                    {
                        if (AITimer == 1)
                        {
                            SoundEngine.PlaySound(EbonianSounds.BeamWindUp, NPC.Center);
                        }
                        if (AITimer == 55)
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<GreenChargeUp>(), 0, 0);
                        if (AITimer == 120)
                        {
                            Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<OstertagiExplosion>(), 0, 0, 0);
                            for (int i = 0; i < 20; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 20);
                                Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, angle.ToRotationVector2() * Main.rand.NextFloat(6, 10), ModContent.ProjectileType<OstertagiWorm>(), 30, 0, 0);
                                a.friendly = false;
                                a.hostile = true;
                            }
                            EbonianSystem.ScreenShakeAmount = 10;
                        }
                        if (AITimer == 140)
                        {
                            Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<OstertagiExplosion>(), 0, 0, 0);
                            for (int i = 0; i < 20; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 20) + MathHelper.PiOver4;
                                Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, angle.ToRotationVector2() * Main.rand.NextFloat(6, 10), ModContent.ProjectileType<OstertagiWorm>(), 30, 0, 0);
                                a.friendly = false;
                                a.hostile = true;
                            }
                            EbonianSystem.ScreenShakeAmount = 10;
                        }

                        if (AITimer >= 220)
                        {
                            AIState = IchorBomb;
                            Reset();
                        }

                    }
                    break;
                case IchorBomb:
                    {

                        open = true;
                        if (AITimer < 35)
                        {
                            NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 200), false) / 20f;
                            openOffset.X++;
                            openRotation += MathHelper.ToRadians(2);
                            rotation -= MathHelper.ToRadians(2);
                        }
                        if (AITimer == 35)
                        {
                            NPC.velocity = Vector2.Zero;
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<ConglomerateScream>(), 0, 0);
                        }
                        if (AITimer == 50)
                        {
                            for (int i = -1; i < 2; i++)
                            {
                                if (i == 0) continue;
                                Projectile.NewProjectile(null, NPC.Center, new Vector2(i * 10, -7), ModContent.ProjectileType<CIchorBomb>(), 40, 0);
                            }
                        }
                        if (AITimer >= 70)
                        {
                            openOffset.X--;
                            openRotation = Helper.LerpAngle(openRotation, 0, 0.25f);
                            NPC.rotation = Helper.LerpAngle(NPC.rotation, 0, 0.25f);
                            rotation = Helper.LerpAngle(rotation, 0, 0.25f);
                        }
                        if (AITimer >= 80)
                        {
                            openOffset = Vector2.Zero;
                            open = false;
                        }

                        if (AITimer >= 190)
                        {
                            AIState = LastPrismLaser;
                            Reset();
                        }
                    }
                    break;
                case LastPrismLaser:
                    {
                        if (AITimer >= 20)
                        {
                            AIState = BloodAndWormSpit;
                            Reset();
                        }
                    }
                    break;
            }
        }
        void Reset()
        {
            AITimer = 0;
            AITimer2 = 0;
            open = false;
            openOffset = Vector2.Zero;
            openRotation = 0;
            AITimer3 = 0;
            rotation = 0;
            NPC.velocity = Vector2.Zero;
            for (int i = 0; i < disposablePos.Length; i++) disposablePos[i] = Vector2.Zero;
            NPC.damage = 0;
        }
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
                tendonVerlet[i] = new(NPC.Center, 2, 18, 1 * scale, true, true, (int)(2.5f * scale));
            }
        }
        public struct VerletStruct
        {
            public Vector2 position;
            public Vector2[] oldPosition = new Vector2[25];
            public float[] oldRotation = new float[25];
            public Verlet verlet;
            public VerletStruct(Vector2 _position, Verlet _verlet)
            {
                position = _position;
                verlet = _verlet;
            }
        }
        Verlet[] tendonVerlet = new Verlet[5];
        public VerletStruct spineVerlet, clingerVerlet, eyeVerlet, armVerlet;
        public VerletStruct[] clawVerlet = new VerletStruct[3];
        public VerletStruct[] gutVerlets = new VerletStruct[10];
        int hookFrame = 1;
        void SpawnVerlets()
        {
            spineVerlet = new VerletStruct(NPC.Center, new Verlet(NPC.Center, 14, 20, -0.25f, stiffness: 40));
            clingerVerlet = new VerletStruct(NPC.Center, new Verlet(NPC.Center, 10, 20, 0.25f, stiffness: 40));
            eyeVerlet = new VerletStruct(NPC.Center, new Verlet(NPC.Center, 10, 20, 0.15f, stiffness: 40));
            armVerlet = new VerletStruct(NPC.Center, new Verlet(NPC.Center, 12, 20, 0f, stiffness: 50));
            for (int i = 0; i < 3; i++)
                clawVerlet[i] = new VerletStruct(NPC.Center, new Verlet(NPC.Center, 12, 20, 0f, stiffness: 50));
        }
        void DrawVerlets(SpriteBatch spriteBatch)
        {
            if (spineVerlet.verlet != null)
            {
                spineVerlet.verlet.Update(NPC.Center + new Vector2(22, -10), spineVerlet.position);
                spineVerlet.verlet.Draw(spriteBatch, new VerletDrawData(Texture + "_SpineSegment", null, Texture + "_Spine"));
            }
            if (clingerVerlet.verlet != null)
            {
                clingerVerlet.verlet.Update(NPC.Center + new Vector2(22, 10), clingerVerlet.position);
                clingerVerlet.verlet.Draw(spriteBatch, new VerletDrawData(Texture + "_ClingerSegment", null, Texture + "_Clinger"));
            }
            if (eyeVerlet.verlet != null)
            {
                eyeVerlet.verlet.Update(NPC.Center + new Vector2(-22, -10), eyeVerlet.position);
                eyeVerlet.verlet.Draw(spriteBatch, new VerletDrawData(Texture + "_EyeSegment", null, Texture + "_Eye"));
            }
            if (armVerlet.verlet != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (clawVerlet[i].verlet != null)
                    {
                        clawVerlet[i].verlet.Update(armVerlet.position, clawVerlet[i].position);
                        clawVerlet[i].verlet.Draw(spriteBatch, new VerletDrawData("NPCs/Cecitior/Hook/CecitiorHook_0", null, "NPCs/Cecitior/Hook/CecitiorHook_" + hookFrame));
                    }
                }
                armVerlet.verlet.Update(NPC.Center + new Vector2(-22, -10), armVerlet.position);
                armVerlet.verlet.Draw(spriteBatch, new VerletDrawData("NPCs/Cecitior/Hook/CecitiorHook_0", null, "Gores/Crimorrhage3"));
            }
        }
        void MiscFX()
        {
            Lighting.AddLight(NPC.TopLeft, TorchID.Ichor);
            Lighting.AddLight(NPC.BottomRight, TorchID.Cursed);
            SoundStyle selected = EbonianSounds.flesh0;
            switch (Main.rand.Next(3))
            {
                case 0:
                    selected = EbonianSounds.flesh1;
                    break;
                case 1:
                    selected = EbonianSounds.flesh2;
                    break;
            }
            if (!savedSound.IsValid || !SoundEngine.TryGetActiveSound(savedSound, out var activeSound) || !activeSound.IsPlaying)
            {
                savedSound = SoundEngine.PlaySound(selected, NPC.Center);
            }
        }
    }
}
