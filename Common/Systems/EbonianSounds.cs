using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        None = Default with { SoundPath = ebonianSoundPath + "sheep", Volume = 0f },
            bloodSpit = Default with { SoundPath = ebonianSoundPath + "bloodSpit" },
        bowPull = Default with { SoundPath = ebonianSoundPath + "bowPull" },
        bowRelease = Default with { SoundPath = ebonianSoundPath + "bowRelease" },
        chargedBeam = Default with { SoundPath = ebonianSoundPath + "chargedBeam" },
        chargedBeamImpactOnly = Default with { SoundPath = ebonianSoundPath + "chargedBeamImpactOnly", PitchVariance = 0f },
        cursedToyCharge = Default with { SoundPath = ebonianSoundPath + "cursedToyCharge", PitchVariance = 0f },
        chomp0 = Default with { SoundPath = ebonianSoundPath + "chomp0", Volume = 1.3f },
        chomp1 = Default with { SoundPath = ebonianSoundPath + "chomp1", Volume = 1.3f },
        chomp2 = Default with { SoundPath = ebonianSoundPath + "chomp2", MaxInstances = 1 },
        eggplosion = Default with { SoundPath = ebonianSoundPath + "eggplosion" },
        eruption = Default with { SoundPath = ebonianSoundPath + "eruption" },
        exolDash = Default with { SoundPath = ebonianSoundPath + "exolDash" },
        exolRoar = Default with { SoundPath = ebonianSoundPath + "exolRoar" },
        exolSummon = Default with { SoundPath = ebonianSoundPath + "exolSummon" },
        flesh0 = Default with { SoundPath = ebonianSoundPath + "flesh0" },
        flesh1 = Default with { SoundPath = ebonianSoundPath + "flesh1" },
        flesh2 = Default with { SoundPath = ebonianSoundPath + "flesh2" },
        garbageAwaken = Default with { SoundPath = ebonianSoundPath + "garbageAwaken" },
        garbageSignal = Default with { SoundPath = ebonianSoundPath + "garbageSignal" },
        genericExplosion = Default with { SoundPath = ebonianSoundPath + "genericExplosion" },
        heartbeat = Default with { SoundPath = ebonianSoundPath + "heartbeat" },
        nuke = Default with { SoundPath = ebonianSoundPath + "nuke" },
        reiFail = Default with { SoundPath = ebonianSoundPath + "reiFail" },
        reiFail2 = Default with { SoundPath = ebonianSoundPath + "reiFail2" },
        reiTP = Default with { SoundPath = ebonianSoundPath + "reiTP" },
        rolleg = Default with { SoundPath = ebonianSoundPath + "rolleg" },
        terrortomaDash = Default with { SoundPath = ebonianSoundPath + "terrortomaDash" },
        cecitiorDie = Default with { SoundPath = ebonianSoundPath + "NPCHit/cecitiorDie" },
        fleshHit = Default with { SoundPath = ebonianSoundPath + "NPCHit/fleshHit" },
        terrortomaFlesh = Default with { SoundPath = ebonianSoundPath + "terrortomaFlesh" },
        cecitiorBurp = Default with { SoundPath = ebonianSoundPath + "cecitiorBurp" },
        clawSwipe = Default with { SoundPath = ebonianSoundPath + "clawSwipe" },
        trumpet = Default with { SoundPath = ebonianSoundPath + "trumpet", PitchVariance = 0.4f, Variants = new int[] { 0, 1 } },
        terrortomaLaugh = Default with { SoundPath = ebonianSoundPath + "terrortomaLaugh", Variants = new int[] { 0, 1 }, PitchVariance = 0.25f, Volume = 1.1f },
        blink = Default with { SoundPath = ebonianSoundPath + "blink" },
        xSpirit = Default with { SoundPath = ebonianSoundPath + "xSpirit" },
        magicSlash = Default with { SoundPath = ebonianSoundPath + "magicSlash" },
        sheep = Default with { SoundPath = ebonianSoundPath + "sheep" },
        vaccum = Default with { SoundPath = ebonianSoundPath + "vaccum", IsLooped = true, PitchVariance = 0f },
        ghizasWheel = Default with { SoundPath = ebonianSoundPath + "ghizasWheel", IsLooped = true, PitchVariance = 0f },
        garbageDeath = Default with { SoundPath = ebonianSoundPath + "NPCHit/garbageDeath" },
        evilOutro = Default with { SoundPath = ebonianSoundPath + "Music/Outros/evilOutro", PitchVariance = 0 };
        public override void Load()
        {
            bloodSpit = Default with { SoundPath = ebonianSoundPath + "bloodSpit" };
            bowPull = Default with { SoundPath = ebonianSoundPath + "bowPull" };
            bowRelease = Default with { SoundPath = ebonianSoundPath + "bowRelease" };
            chargedBeam = Default with { SoundPath = ebonianSoundPath + "chargedBeam" };
            chargedBeamImpactOnly = Default with { SoundPath = ebonianSoundPath + "chargedBeamImpactOnly", PitchVariance = 0f };
            cursedToyCharge = Default with { SoundPath = ebonianSoundPath + "cursedToyCharge", PitchVariance = 0f };
            chomp0 = Default with { SoundPath = ebonianSoundPath + "chomp0", Volume = 1.3f };
            chomp1 = Default with { SoundPath = ebonianSoundPath + "chomp1", Volume = 1.3f };
            chomp2 = Default with { SoundPath = ebonianSoundPath + "chomp2", MaxInstances = 1 };
            eggplosion = Default with { SoundPath = ebonianSoundPath + "eggplosion" };
            eruption = Default with { SoundPath = ebonianSoundPath + "eruption" };
            exolDash = Default with { SoundPath = ebonianSoundPath + "exolDash" };
            exolRoar = Default with { SoundPath = ebonianSoundPath + "exolRoar" };
            exolSummon = Default with { SoundPath = ebonianSoundPath + "exolSummon" };
            flesh0 = Default with { SoundPath = ebonianSoundPath + "flesh0" };
            flesh1 = Default with { SoundPath = ebonianSoundPath + "flesh1" };
            flesh2 = Default with { SoundPath = ebonianSoundPath + "flesh2" };
            garbageAwaken = Default with { SoundPath = ebonianSoundPath + "garbageAwaken" };
            garbageSignal = Default with { SoundPath = ebonianSoundPath + "garbageSignal" };
            genericExplosion = Default with { SoundPath = ebonianSoundPath + "genericExplosion" };
            heartbeat = Default with { SoundPath = ebonianSoundPath + "heartbeat" };
            nuke = Default with { SoundPath = ebonianSoundPath + "nuke" };
            reiFail = Default with { SoundPath = ebonianSoundPath + "reiFail" };
            reiFail2 = Default with { SoundPath = ebonianSoundPath + "reiFail2" };
            reiTP = Default with { SoundPath = ebonianSoundPath + "reiTP" };
            rolleg = Default with { SoundPath = ebonianSoundPath + "rolleg" };
            terrortomaDash = Default with { SoundPath = ebonianSoundPath + "terrortomaDash" };
            cecitiorDie = Default with { SoundPath = ebonianSoundPath + "NPCHit/cecitiorDie" };
            fleshHit = Default with { SoundPath = ebonianSoundPath + "NPCHit/fleshHit" };
            terrortomaFlesh = Default with { SoundPath = ebonianSoundPath + "terrortomaFlesh" };
            cecitiorBurp = Default with { SoundPath = ebonianSoundPath + "cecitiorBurp" };
            clawSwipe = Default with { SoundPath = ebonianSoundPath + "clawSwipe" };
            trumpet = Default with { SoundPath = ebonianSoundPath + "trumpet", PitchVariance = 0.4f, Variants = new int[] { 0, 1 } };
            terrortomaLaugh = Default with { SoundPath = ebonianSoundPath + "terrortomaLaugh", Variants = new int[] { 0, 1 }, PitchVariance = 0.25f, Volume = 1.1f };
            blink = Default with { SoundPath = ebonianSoundPath + "blink" };
            xSpirit = Default with { SoundPath = ebonianSoundPath + "xSpirit" };
            magicSlash = Default with { SoundPath = ebonianSoundPath + "magicSlash" };
            sheep = Default with { SoundPath = ebonianSoundPath + "sheep" };
            vaccum = Default with { SoundPath = ebonianSoundPath + "vaccum", IsLooped = true, PitchVariance = 0f };
            ghizasWheel = Default with { SoundPath = ebonianSoundPath + "ghizasWheel", IsLooped = true, PitchVariance = 0f };
            garbageDeath = Default with { SoundPath = ebonianSoundPath + "NPCHit/garbageDeath" };
            evilOutro = Default with { SoundPath = ebonianSoundPath + "Music/Outros/evilOutro", PitchVariance = 0 };
        }
    }
}
