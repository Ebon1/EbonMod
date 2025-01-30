using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace EbonianMod.Common.Systems
{
    public class EbonianSounds : ModSystem
    {
        public static SoundStyle Default = new SoundStyle("EbonianMod/Sounds/reiFail")
        {
            MaxInstances = 10,
            PitchVariance = 0.3f,
        };


        public const string ebonianSoundPath = "EbonianMod/Sounds/";
        public static SoundStyle
        None,
        bloodSpit,
        bowPull,
        buzz,
        bowRelease,
        chargedBeam,
        chargedBeamWindUp,
        chargedBeamImpactOnly,
        cursedToyCharge,
        chomp0,
        chomp1,
        chomp2,
        cecitiorOpen,
        cecitiorClose,
        cecitiorCloseShort,
        cecitiorIdle,
        cecitiorSlam,
        cecitiorSlice,
        cecitiorSpit,
        eggplosion,
        eruption,
        exolDash,
        exolRoar,
        exolSummon,
        flesh0,
        flesh1,
        flesh2,
        buildup,
        garbageAwaken,
        garbageSignal,
        genericExplosion,
        heartbeat,
        nuke,
        reiFail,
        reiFail2,
        reiTP,
        rolleg,
        terrortomaDash,
        cecitiorDie,
        fleshHit,
        terrortomaFlesh,
        cecitiorBurp,
        clawSwipe,
        BeamWindUp,
        garbageLaser,
        trumpet,
        terrortomaLaugh,
        HeavySwing,
        FleshImpact,
        blink,
        xSpirit,
        xDeath,
        helicopter,
        firework,
        magicSlash,
        sheep,
            sheep_player,
        shriek,
        vaccum,
        ghizasWheel,
        garbageDeath,
        evilOutro,
        xareusOutro;
        public static void LoadSounds()
        {
            bloodSpit = Default with
            {
                SoundPath = ebonianSoundPath + "bloodSpit"
            };

            bowPull = Default with
            {
                SoundPath = ebonianSoundPath + "bowPull",
                Volume = GetInstance<EbonianClientConfig>().BowPullVolume
            };


            bowRelease = Default with
            {
                SoundPath = ebonianSoundPath + "bowRelease"
            };

            buzz = Default with
            {
                SoundPath = ebonianSoundPath + "buzz",
                PitchVariance = 0
            };


            chargedBeam = Default with
            {
                SoundPath = ebonianSoundPath + "chargedBeam"
            };


            chargedBeamWindUp = Default with
            {
                SoundPath = ebonianSoundPath + "chargedBeamWindUp",
                PitchVariance = 0f
            };


            chargedBeamImpactOnly = Default with
            {
                SoundPath = ebonianSoundPath + "chargedBeamImpactOnly",
                PitchVariance = 0f
            };


            cursedToyCharge = Default with
            {
                SoundPath = ebonianSoundPath + "cursedToyCharge",
                PitchVariance = 0f
            };


            chomp0 = Default with
            {
                SoundPath = ebonianSoundPath + "chomp0",
                Volume = 1.3f
            };


            chomp1 = Default with
            {
                SoundPath = ebonianSoundPath + "chomp1",
                Volume = 1.3f
            };


            chomp2 = Default with
            {
                SoundPath = ebonianSoundPath + "chomp2",
                MaxInstances = 1
            };


            cecitiorOpen = Default with
            {
                SoundPath = ebonianSoundPath + "Cecitior/cecitiorOpen",
                MaxInstances = 1
            };

            cecitiorCloseShort = Default with
            {
                SoundPath = ebonianSoundPath + "Cecitior/cecitiorCloseShort",
                MaxInstances = 1
            };


            cecitiorClose = Default with
            {
                SoundPath = ebonianSoundPath + "Cecitior/cecitiorClose",
                MaxInstances = 1
            };

            cecitiorIdle = Default with
            {
                SoundPath = ebonianSoundPath + "Cecitior/cecitiorIdle",
                PitchVariance = 0,
                MaxInstances = 1,
                IsLooped = true
            };

            cecitiorSlam = Default with
            {
                SoundPath = ebonianSoundPath + "Cecitior/cecitiorSlam",
                MaxInstances = 1,
                Variants = [0, 1, 2]
            };

            cecitiorSlice = Default with
            {
                SoundPath = ebonianSoundPath + "Cecitior/cecitiorSlice",
                MaxInstances = 1,
                Variants = [0, 1, 2]
            };

            cecitiorSpit = Default with
            {
                SoundPath = ebonianSoundPath + "Cecitior/cecitiorSpit",
                MaxInstances = 1,
                Variants = [0, 1, 2, 3]
            };

            eggplosion = Default with
            {
                SoundPath = ebonianSoundPath + "eggplosion"
            };


            eruption = Default with
            {
                SoundPath = ebonianSoundPath + "eruption"
            };


            exolDash = Default with
            {
                SoundPath = ebonianSoundPath + "exolDash"
            };


            exolRoar = Default with
            {
                SoundPath = ebonianSoundPath + "exolRoar"
            };


            exolSummon = Default with
            {
                SoundPath = ebonianSoundPath + "exolSummon"
            };


            flesh0 = Default with
            {
                SoundPath = ebonianSoundPath + "flesh0"
            };


            flesh1 = Default with
            {
                SoundPath = ebonianSoundPath + "flesh1"
            };


            flesh2 = Default with
            {
                SoundPath = ebonianSoundPath + "flesh2"
            };


            buildup = Default with
            {
                SoundPath = ebonianSoundPath + "buildup",
                PitchVariance = 0
            };


            garbageAwaken = Default with
            {
                SoundPath = ebonianSoundPath + "HotGarbage/garbageAwaken",
                PitchVariance = 0
            };


            garbageSignal = Default with
            {
                SoundPath = ebonianSoundPath + "HotGarbage/garbageSignal",
                PitchVariance = 0
            };


            genericExplosion = Default with
            {
                SoundPath = ebonianSoundPath + "genericExplosion"
            };


            heartbeat = Default with
            {
                SoundPath = ebonianSoundPath + "heartbeat"
            };


            nuke = Default with
            {
                SoundPath = ebonianSoundPath + "nuke"
            };


            reiFail = Default with
            {
                SoundPath = ebonianSoundPath + "reiFail"
            };


            reiFail2 = Default with
            {
                SoundPath = ebonianSoundPath + "reiFail2"
            };


            reiTP = Default with
            {
                SoundPath = ebonianSoundPath + "reiTP"
            };


            rolleg = Default with
            {
                SoundPath = ebonianSoundPath + "rolleg"
            };


            terrortomaDash = Default with
            {
                SoundPath = ebonianSoundPath + "Terrortoma/terrortomaDash"
            };


            cecitiorDie = Default with
            {
                SoundPath = ebonianSoundPath + "NPCHit/cecitiorDie"
            };


            fleshHit = Default with
            {
                SoundPath = ebonianSoundPath + "NPCHit/fleshHit"
            };


            terrortomaFlesh = Default with
            {
                SoundPath = ebonianSoundPath + "Terrortoma/terrortomaFlesh"
            };


            cecitiorBurp = Default with
            {
                SoundPath = ebonianSoundPath + "Cecitior/cecitiorBurp"
            };


            clawSwipe = Default with
            {
                SoundPath = ebonianSoundPath + "clawSwipe"
            };


            BeamWindUp = Default with
            {
                SoundPath = ebonianSoundPath + "BeamWindUp",
                PitchVariance = 0
            };


            garbageLaser = Default with
            {
                SoundPath = ebonianSoundPath + "HotGarbage/garbageLaser",
                PitchVariance = 0
            };


            trumpet = Default with
            {
                SoundPath = ebonianSoundPath + "trumpet",
                PitchVariance = 0.4f,
                Variants = new int[] { 0, 1 }
            };


            terrortomaLaugh = Default with
            {
                SoundPath = ebonianSoundPath + "Terrortoma/terrortomaLaugh",
                Variants = new int[] { 0, 1 },
                PitchVariance = 0.25f,
                Volume = 1.1f
            };


            HeavySwing = Default with
            {
                SoundPath = ebonianSoundPath + "HeavySwing",
                Variants = new int[] { 0, 1, 2 },
                PitchVariance = 0.25f,
                Volume = 1.1f
            };


            FleshImpact = Default with
            {
                SoundPath = ebonianSoundPath + "FleshImpact",
                Variants = new int[] { 0, 1, 2 },
                PitchVariance = 0.25f,
                Volume = .7f
            };


            blink = Default with
            {
                SoundPath = ebonianSoundPath + "blink"
            };


            xSpirit = Default with
            {
                SoundPath = ebonianSoundPath + "Xareus/xSpirit",
                Volume = 0.7f
            };

            xDeath = Default with
            {
                SoundPath = ebonianSoundPath + "Xareus/xDeath",
                PitchVariance = 0.05f
            };


            helicopter = Default with
            {
                SoundPath = ebonianSoundPath + "helicopter",
                PitchVariance = 0f
            };


            firework = Default with
            {
                SoundPath = ebonianSoundPath + "firework"
            };


            magicSlash = Default with
            {
                SoundPath = ebonianSoundPath + "magicSlash"
            };


            sheep = Default with
            {
                SoundPath = ebonianSoundPath + "sheep",
                PitchVariance = 0.5f,
                Variants = new int[] { 0, 1, 2 }
            };


            sheep_player = Default with
            {
                SoundPath = ebonianSoundPath + "sheep",
                PitchVariance = 0.5f,
                Variants = new int[] { 0, 1 }
            };


            shriek = Default with
            {
                SoundPath = ebonianSoundPath + "shriek"
            };


            vaccum = Default with
            {
                SoundPath = ebonianSoundPath + "vaccum",
                IsLooped = true,
                PitchVariance = 0f
            };


            ghizasWheel = Default with
            {
                SoundPath = ebonianSoundPath + "ghizasWheel",
                IsLooped = true,
                PitchVariance = 0f
            };


            garbageDeath = Default with
            {
                SoundPath = ebonianSoundPath + "NPCHit/garbageDeath",
                PitchVariance = 0
            };


            evilOutro = Default with
            {
                SoundPath = ebonianSoundPath + "Music/Outros/evilOutro",
                PitchVariance = 0,
                Volume = 0.5f,
                MaxInstances = 1,
                Type = SoundType.Music
            };

            xareusOutro = Default with
            {
                SoundPath = ebonianSoundPath + "Music/Outros/xareusOutro",
                PitchVariance = 0,
                Volume = 0.5f,
                MaxInstances = 1,
                Type = SoundType.Music
            };


        }
        public override void OnModLoad()
        {
            LoadSounds();
        }
    }
}
