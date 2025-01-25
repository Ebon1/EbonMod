using EbonianMod.NPCs.Cecitior;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ModLoader;
namespace EbonianMod.Bossbars
{
    public abstract class BossBar : ModBossBar
    {
        private int bossHeadIndex = -1;
        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
        {
            if (bossHeadIndex != -1)
            {
                return TextureAssets.NpcHeadBoss[bossHeadIndex];
            }
            return null;

        }

        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)
        {
            NPC npc = Main.npc[info.npcIndexToAimAt];
            if (!npc.active)
                return false;
            bossHeadIndex = npc.GetBossHeadTextureIndex();

            life = npc.life;
            lifeMax = npc.lifeMax;

            return true;
        }
    }
    public class TerrortomaBar : BossBar { }
    public class XBar : BossBar { }

    public class CecitiorBar : ModBossBar
    {

        private int bossHeadIndex = -1;
        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
        {
            if (bossHeadIndex != -1)
            {
                return TextureAssets.NpcHeadBoss[bossHeadIndex];
            }
            return null;

        }
        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)
        {
            NPC npc = Main.npc[info.npcIndexToAimAt];
            if (!npc.active)
                return false;
            bossHeadIndex = npc.GetBossHeadTextureIndex();


            float _shield = 0;
            float _shieldMax = 0;
            int count = 0;
            foreach (NPC _npc in Main.ActiveNPCs)
            {
                if (_npc.type == NPCType<CecitiorEye>())
                {
                    _shield += _npc.life;
                    _shieldMax += _npc.lifeMax;
                    count++;
                }
            }
            if (_shield > 0)
            {
                life = _shield;
                lifeMax = _shieldMax * 6 / count;
            }
            else
            {
                life = npc.life;
                lifeMax = npc.lifeMax;
            }

            return true;
        }
    }
}
