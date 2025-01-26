using EbonianMod.Items.Consumables.BossItems;
using EbonianMod.Items.Pets;
using EbonianMod.Items.Tiles.Trophies;
using EbonianMod.Items.Weapons.Ranged;
using EbonianMod.NPCs.ArchmageX;
using EbonianMod.NPCs.Cecitior;
using EbonianMod.NPCs.Garbage;
using EbonianMod.NPCs.Terrortoma;
using System;
using System.Collections.Generic;

namespace EbonianMod.Common.Systems.Misc
{
    public class BossLogSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            BossCheckListIntegration();
        }
        private void BossCheckListIntegration()
        {
            if (!ModLoader.TryGetMod("BossChecklist", out Mod bossCheckListMod))
            {
                return;
            }

            if (bossCheckListMod.Version < new Version(2, 2, 2))
            {
                return;
            }
            HotGarbage(bossCheckListMod);
            ArchmageX(bossCheckListMod);
            Cecitior(bossCheckListMod);
            Terrortoma(bossCheckListMod);
        }
        private void HotGarbage(Mod bossCheckListMod)
        {
            float weight = 1.8f;
            Func<bool> downed = () => GetInstance<DownedBossSystem>().downedGarbage;

            Func<LocalizedText> spawnInfo = () => Language.GetText("Mods.EbonianMod.NPCs.HotGarbage.BossChecklistIntegration.SpawnInfo").WithFormatArgs(ItemType<GarbageRemote>());

            var customPortrait = (SpriteBatch sb, Rectangle rect, Color color) =>
            {
                Texture2D texture = ModContent.Request<Texture2D>("EbonianMod/Extras/Sprites/BossLogPreviews/HotGarbagePreviewA").Value;
                Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), (float)((rect.Height / 0.82) - (texture.Height / 0.82)));
                sb.Draw(texture, centered, color);
            };

            Dictionary<string, object> dictionary = new()
            {
                ["displayName"] = Language.GetText("Mods.EbonianMod.NPCs.HotGarbage.DisplayName"),
                ["spawnInfo"] = spawnInfo,
                ["spawnItems"] = ItemType<GarbageRemote>(),
                ["customPortrait"] = customPortrait
            };

            bossCheckListMod.Call(
                "LogBoss",
                Mod,
                "HotGarbage",
                weight,
                downed,
                NPCType<HotGarbage>(),
                dictionary);
        }

        private void ArchmageX(Mod bossCheckListMod)
        {
            float weight = 4.5f;
            Func<bool> downed = () => GetInstance<DownedBossSystem>().downedXareusCheck;

            Func<LocalizedText> spawnInfo = () => Language.GetText("Interact with the Archstaff in the Mysterious Shack, after unlocking the basement with a [i:" + ItemID.LargeAmethyst + "].");

            var customPortrait = (SpriteBatch sb, Rectangle rect, Color color) =>
            {
                Texture2D texture = ModContent.Request<Texture2D>("EbonianMod/Extras/Sprites/BossLogPreviews/ArchmageXPreview").Value;
                Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), (float)((rect.Height / 0.8) - (texture.Height / 0.8)));
                sb.Draw(texture, centered, color);
            };

            Dictionary<string, object> dictionary = new()
            {
                ["displayName"] = Language.GetText("Mods.EbonianMod.NPCs.ArchmageX.BossLogDisplayName"),
                ["spawnInfo"] = spawnInfo,
                ["customPortrait"] = customPortrait
            };

            bossCheckListMod.Call(
                "LogBoss",
                Mod,
                "ArchmageX",
                weight,
                downed,
                NPCType<ArchmageX>(),
                dictionary);
        }

        private void Cecitior(Mod bossCheckListMod)
        {
            float weight = 8.4f;
            Func<bool> downed = () => GetInstance<DownedBossSystem>().downedCecitior;

            Func<LocalizedText> spawnInfo = () => Language.GetText("Kill a Massive Spectator, commonly found in Crimson.");

            var customPortrait = (SpriteBatch sb, Rectangle rect, Color color) =>
            {
                Texture2D texture = ModContent.Request<Texture2D>("EbonianMod/Extras/Sprites/BossLogPreviews/CecitiorPreview").Value;
                Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), (float)((rect.Height / 0.6) - (texture.Height / 0.6)));
                sb.Draw(texture, centered, color);
            };

            Dictionary<string, object> dictionary = new()
            {
                ["displayName"] = Language.GetText("Mods.EbonianMod.NPCs.Cecitior.DisplayName"),
                ["spawnInfo"] = spawnInfo,
                ["customPortrait"] = customPortrait
            };

            bossCheckListMod.Call(
                "LogBoss",
                Mod,
                "Cecitior",
                weight,
                downed,
                NPCType<Cecitior>(),
                dictionary);
        }

        private void Terrortoma(Mod bossCheckListMod)
        {
            float weight = 8.6f;
            Func<bool> downed = () => GetInstance<DownedBossSystem>().downedTerrortoma;

            Func<LocalizedText> spawnInfo = () => Language.GetText("Destroy a Giant Cocoon, commonly found in the Corruption.");

            var customPortrait = (SpriteBatch sb, Rectangle rect, Color color) =>
            {
                Texture2D texture = ModContent.Request<Texture2D>("EbonianMod/Extras/Sprites/BossLogPreviews/TerrortomaPreview").Value;
                Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), (float)((rect.Height / 0.7) - (texture.Height / 0.7)));
                sb.Draw(texture, centered, color);
            };

            Dictionary<string, object> dictionary = new()
            {
                ["displayName"] = Language.GetText("Mods.EbonianMod.NPCs.Terrortoma.DisplayName"),
                ["spawnInfo"] = spawnInfo,
                ["customPortrait"] = customPortrait
            };

            bossCheckListMod.Call(
                "LogBoss",
                Mod,
                "Terrortoma",
                weight,
                downed,
                NPCType<Terrortoma>(),
                dictionary);
        }
    }
}