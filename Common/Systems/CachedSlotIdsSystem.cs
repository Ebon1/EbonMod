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
        public LoopedSound(SlotId slotId, int attatchedNpcType = 0, Func<bool> condition = default)
        {
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
        public static Dictionary<string, LoopedSound> loopedSounds = new();
        public override void PostUpdateEverything()
        {
            if (loopedSounds.Any())
                foreach (KeyValuePair<string, LoopedSound> loopedSound in loopedSounds)
                {
                    if (loopedSound.Value.attatchedNpcType > 0)
                    {
                        foreach (NPC npc in Main.ActiveNPCs)
                        {
                            if (npc.type == loopedSound.Value.attatchedNpcType)
                            {
                                if (SoundEngine.TryGetActiveSound(loopedSound.Value.slotId, out var _activeSound)) _activeSound.Position = npc.Center;
                                break;
                            }
                        }
                    }
                    if (!loopedSound.Value.condition.Invoke())
                    {
                        if (SoundEngine.TryGetActiveSound(loopedSound.Value.slotId, out var _activeSound))
                            _activeSound.Stop();
                        else
                        {
                            loopedSounds.Remove(loopedSound.Key);
                            break;
                        }
                    }
                }
        }
        public static void ClearSound(string key)
        {
            if (loopedSounds.ContainsKey(key))
            {
                if (SoundEngine.TryGetActiveSound(loopedSounds[key].slotId, out var _activeSound))
                    _activeSound.Stop();
                loopedSounds.Remove(key);
            }
        }
        public static void ClearSound(KeyValuePair<string, LoopedSound> loopedSound)
        {
            if (SoundEngine.TryGetActiveSound(loopedSound.Value.slotId, out var _activeSound))
                _activeSound.Stop();

            if (loopedSounds.Contains(loopedSound))
                loopedSounds.Remove(loopedSound.Key);
        }
        public static void UnloadSounds()
        {
            if (loopedSounds.Any())
            {
                foreach (KeyValuePair<string, LoopedSound> loopedSound in loopedSounds)
                {
                    if (SoundEngine.TryGetActiveSound(loopedSound.Value.slotId, out var _activeSound))
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
