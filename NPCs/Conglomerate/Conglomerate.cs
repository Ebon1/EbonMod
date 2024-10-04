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
            Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Conglomerate");
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
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy) return false;
            Texture2D tex = Helper.GetTexture(Texture);
            Texture2D glow = Helper.GetTexture(Texture + "_Glow");
            DrawVerlets(spriteBatch);
            spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(glow, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
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
            RandomFocusedBeams = 8, SpinAroundThePlayerAndDash = 9, BodySlamTantrum = 10, GoldenShower = 11, SpikesCloseIn = 12, HomingEyeRain = 13, VerticalDashSpam = 14,
            // Phase 2
            ClawTriangle = 15, GrabAttack = 16, SpineChargedFlame = 17, ClingerHipfire = 18, EyeBeamPlusHomingEyes = 19, ClawTantrum = 20, SpineDashFollowedByMainDash = 21,
            ClingerWaveFire = 22, SpineWormVomit = 23, ClawPlucksBombsFromSpine = 24, BitesEyeToRainBlood = 25, ClingerComboType1 = 26, ClingerComboType2 = 27,
            ClingerComboType3 = 28, SmallDeathRay = 29, EyeSendsWavesOfHomingEyes = 30, BigBomb = 31, EyeSpin = 32, SlamSpineAndEyeTogether = 33,

            Death = -2;
        SlotId savedSound;
        Vector2[] disposablePos = new Vector2[10];
        float rotation;
        bool phase2;
        public override void AI()
        {
            bool t = true;
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
            NPC.rotation = Helper.LerpAngle(NPC.rotation, rotation, 0.1f);
            AITimer++;
            switch (AIState)
            {
                case Intro:
                    {
                        if (AITimer >= 10)
                        {
                            AIState = BloodAndWormSpit;
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
                                Projectile.NewProjectile(null, NPC.Center + vel * 20, vel * 6, ModContent.ProjectileType<CecitiorTeeth>(), 25, 0);
                        }
                        if (AITimer >= 80 && AITimer % (phase2 ? 5 : 7) == 0 && AITimer < 216)
                        {
                            NPC.velocity = -vel * Main.rand.NextFloat(2f, 5);

                            if (AITimer2 == 0)
                            {
                                Projectile a = Projectile.NewProjectileDirect(null, NPC.Center + vel * 20, vel * Main.rand.NextFloat(4, 6), ModContent.ProjectileType<OstertagiWorm>(), 30, 0, 0, Main.rand.NextFloat(0.15f, 0.5f));
                                a.friendly = false;
                                a.hostile = true;
                            }
                            else
                                Projectile.NewProjectile(null, NPC.Center + vel * 20, vel * 6, ModContent.ProjectileType<CecitiorTeeth>(), 25, 0);
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
                        if (AITimer < 50)
                        {
                            if (AITimer % 10 == 0)
                                Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<CecitiorBelch>(), 0, 0);
                        }
                        if (AITimer > 50 && AITimer % (phase2 ? 10 : 15) == 0 && AITimer < 150)
                        {
                            Vector2 pos = player.Center + new Vector2(700 * (Main.rand.NextBool() ? 1 : -1), 0).RotatedByRandom(MathHelper.PiOver4 * 0.5f);
                            Projectile.NewProjectile(null, pos, Helper.FromAToB(pos, player.Center), ModContent.ProjectileType<CecitiorEyeP>(), 25, 0);
                            pos = player.Center + new Vector2(0, 700 * (Main.rand.NextBool() ? 1 : -1)).RotatedByRandom(MathHelper.PiOver4);
                            Projectile.NewProjectile(null, pos, Helper.FromAToB(pos, player.Center), ModContent.ProjectileType<TerrorVilethorn1>(), 25, 0);
                        }
                        if (AITimer >= 170)
                        {
                            AIState = BloatedEbonflies;
                            Reset();
                        }
                    }
                    break;
                case BloatedEbonflies:
                    {
                        if (AITimer == 40)
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<OstertagiExplosion>(), 0, 0);

                        if (AITimer == 70)
                            for (int i = -3; i < 4; i++)
                                NPC.NewNPCDirect(null, player.Center - new Vector2(i * 100, 700), ModContent.NPCType<BloatedEbonfly>());

                        if (AITimer >= 100)
                        {
                            AIState = CursedFlameRain;
                            Reset();
                        }
                    }
                    break;
                case CursedFlameRain:
                    {
                        rotation = -Vector2.UnitY.ToRotation() - MathHelper.PiOver2;
                        if (AITimer <= 150)
                        {
                            Vector2 pos = new Vector2(player.position.X, player.position.Y - 75);
                            Vector2 target = pos;
                            Vector2 moveTo = target - NPC.Center;
                            NPC.velocity = (moveTo) * 0.0445f;
                            if (++AITimer2 % 60 == 0)
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
                                    Projectile projectile = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat(-10f, 10f), -6), ProjectileID.GoldenShowerHostile, 30, 1f, Main.myPlayer)];
                                    projectile.tileCollide = false;
                                    projectile.hostile = true;
                                    projectile.friendly = false;
                                    projectile.timeLeft = 230;
                                }
                                Projectile projectilec = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -6), ProjectileID.GoldenShowerHostile, 30, 1f, Main.myPlayer)];
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

                        if (AITimer >= 10)
                        {
                            AIState = ExplodingBalls;
                            Reset();
                        }
                    }
                    break;
                case ExplodingBalls:
                    {
                        rotation = -Vector2.UnitY.ToRotation() - MathHelper.PiOver2;
                        if (AITimer > 50 && AITimer % (phase2 ? 5 : 10) == 0 && AITimer < 150)
                        {
                            Vector2 pos = player.Center - new Vector2(Main.rand.NextFloat(-1200, 1200), 1000);
                            Projectile.NewProjectile(null, NPC.Center + -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * 30, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(10, 20), (Main.rand.NextBool() ? ModContent.ProjectileType<TerrorStaffPEvil>() : ModContent.ProjectileType<CecitiorBombThing>()), 20, 0);
                        }
                        if (AITimer >= 180)
                        {
                            AIState = RandomFocusedBeams;
                            Reset();
                        }
                    }
                    break;
                case RandomFocusedBeams:
                    {
                        if (AITimer > 30)
                        {
                            disposablePos[0] = Vector2.Lerp(disposablePos[0], player.Center, 0.05f);
                            rotation = Helper.FromAToB(NPC.Center, disposablePos[0]).ToRotation() - MathHelper.PiOver2;
                        }
                        if (AITimer == 30)
                        {
                            disposablePos[0] = player.Center;
                        }
                        if (AITimer == 45)
                            SoundEngine.PlaySound(EbonianSounds.BeamWindUp, NPC.Center);
                        if (AITimer == 100)
                        {
                            Projectile.NewProjectile(null, NPC.Center + new Vector2(7, -14).RotatedBy(NPC.rotation), Vector2.Zero, ModContent.ProjectileType<GreenChargeUp>(), 0, 0);
                        }
                        if (AITimer == 110)
                            Projectile.NewProjectile(null, NPC.Center + new Vector2(7, -14).RotatedBy(NPC.rotation), Vector2.Zero, ModContent.ProjectileType<GreenChargeUp>(), 0, 0);
                        if (AITimer == 145)
                            Projectile.NewProjectile(null, NPC.Center + new Vector2(7, -14).RotatedBy(NPC.rotation), Vector2.Zero, ModContent.ProjectileType<OstertagiExplosion>(), 0, 0);
                        if (AITimer > 145 && AITimer <= 240)
                        {
                            if (AITimer % 2 == 0)
                            {
                                EbonianSystem.ScreenShakeAmount = 5;
                                SoundEngine.PlaySound(EbonianSounds.xSpirit.WithPitchOffset(0.4f).WithVolumeScale(1.2f), NPC.Center);
                                Projectile.NewProjectile(null, NPC.Center + new Vector2(7, -14).RotatedBy(NPC.rotation), Helper.FromAToB(NPC.Center + new Vector2(7, -14).RotatedBy(NPC.rotation), disposablePos[0]).RotatedByRandom(MathHelper.PiOver4 * 0.35f) * Main.rand.NextFloat(1f, 2f), ModContent.ProjectileType<RegorgerBolt>(), 10, 0);
                            }
                        }
                    }
                    break;
            }
        }
        void Reset()
        {
            AITimer = 0;
            AITimer2 = 0;
            AITimer3 = 0;
            rotation = 0;
            NPC.velocity = Vector2.Zero;
            for (int i = 0; i < disposablePos.Length; i++) disposablePos[i] = Vector2.Zero;
            NPC.damage = 0;
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
