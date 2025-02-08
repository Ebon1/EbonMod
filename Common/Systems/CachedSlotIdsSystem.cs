using EbonianMod.NPCs.Cecitior;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbonianMod.Common.Systems
{
    public struct LoopedSound
    {
        public SlotId slotId;
        public Func<bool> condition;
        public int attatchedNpcType;
        public string key;
        public LoopedSound(string key, SlotId slotId, int attatchedNpcType = 0, Func<bool> condition = default)
        {
            this.key = key;
            this.slotId = slotId;
            this.attatchedNpcType = attatchedNpcType;
            if (condition == default)
            {
                if (attatchedNpcType > 0)
                    this.condition = () => NPC.AnyNPCs(attatchedNpcType);
                else throw new Exception("LoopedSound lacks proper condition");
            }
            else
            {
                this.condition = condition;
            }
        }
    }
    public class CachedSlotIdsSystem : ModSystem
    {
        public static List<LoopedSound> loopedSounds = new();
        public override void PostUpdateEverything()
        {
            if (loopedSounds.Any())
                foreach (LoopedSound loopedSound in loopedSounds)
                {
                    if (loopedSound.attatchedNpcType > 0)
                    {
                        foreach (NPC npc in Main.ActiveNPCs)
                        {
                            if (npc.type == loopedSound.attatchedNpcType)
                            {
                                if (SoundEngine.TryGetActiveSound(loopedSound.slotId, out var _activeSound)) _activeSound.Position = npc.Center;
                                break;
                            }
                        }
                    }
                    if (!loopedSound.condition.Invoke())
                    {
                        if (SoundEngine.TryGetActiveSound(loopedSound.slotId, out var _activeSound))
                            _activeSound.Stop();
                        else
                        {
                            loopedSounds.Remove(loopedSound);
                            break;
                        }
                    }
                }
        }
        public static void ClearSound(LoopedSound loopedSound)
        {
            if (SoundEngine.TryGetActiveSound(loopedSound.slotId, out var _activeSound))
                _activeSound.Stop();

            if (loopedSounds.Contains(loopedSound))
                loopedSounds.Remove(loopedSound);
        }
        public static void UnloadSounds()
        {
            if (loopedSounds.Any())
            {
                foreach (LoopedSound loopedSound in loopedSounds)
                {
                    if (SoundEngine.TryGetActiveSound(loopedSound.slotId, out var _activeSound))
                        _activeSound.Stop();
                }
                loopedSounds.Clear();
            }
        }
        public override void OnWorldUnload() => UnloadSounds();
        public override void Load() => UnloadSounds();
        public override void Unload() => UnloadSounds();
    }
}
