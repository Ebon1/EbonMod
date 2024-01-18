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
            MaxInstances = 3,
            PitchVariance = 0.2f,
        };
        public const string ebonianSoundPath = "EbonianMod/Sounds/";
        public static SoundStyle
            bloodSpit = Default with { SoundPath = ebonianSoundPath + "bloodSpit" },
        bowPull = Default with { SoundPath = ebonianSoundPath + "bowPull" },
        bowRelease = Default with { SoundPath = ebonianSoundPath + "bowRelease" },
        chargedBeam = Default with { SoundPath = ebonianSoundPath + "chargedBeam" },
        chomp0 = Default with { SoundPath = ebonianSoundPath + "chomp0" },
        chomp1 = Default with { SoundPath = ebonianSoundPath + "chomp1" },
        chomp2 = Default with { SoundPath = ebonianSoundPath + "chomp2" },
        eggplosion = Default with { SoundPath = ebonianSoundPath + "eggplosion" },
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
        garbageDeath = Default with { SoundPath = ebonianSoundPath + "NPCHit/garbageDeath" };
        public override void Load()
        {
            bloodSpit = Default with { SoundPath = ebonianSoundPath + "bloodSpit" };
            bowPull = Default with { SoundPath = ebonianSoundPath + "bowPull" };
            bowRelease = Default with { SoundPath = ebonianSoundPath + "bowRelease" };
            chargedBeam = Default with { SoundPath = ebonianSoundPath + "chargedBeam" };
            chomp0 = Default with { SoundPath = ebonianSoundPath + "chomp0" };
            chomp1 = Default with { SoundPath = ebonianSoundPath + "chomp1" };
            chomp2 = Default with { SoundPath = ebonianSoundPath + "chomp2" };
            eggplosion = Default with { SoundPath = ebonianSoundPath + "eggplosion" };
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
            garbageDeath = Default with { SoundPath = ebonianSoundPath + "NPCHit/garbageDeath" };
        }
    }
}
