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
        SlotId cachedSound;
        public override void AI()
        {
            bool t = true;
            Lighting.AddLight(NPC.TopLeft, TorchID.Ichor);
            Lighting.AddLight(NPC.BottomRight, TorchID.Cursed);
            if (Main.dayTime)
            {
                Main.time = 31400;
                Main.UpdateTime_StartNight(ref t);
            }
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
            if (!cachedSound.IsValid || !SoundEngine.TryGetActiveSound(cachedSound, out var activeSound) || !activeSound.IsPlaying)
            {
                cachedSound = SoundEngine.PlaySound(selected, NPC.Center);
            }
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

            AITimer++;
            switch (AIState)
            {

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
    }
}
