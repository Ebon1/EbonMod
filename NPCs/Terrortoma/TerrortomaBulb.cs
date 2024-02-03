using EbonianMod.Common.Systems;
using EbonianMod.Items.Misc;
using EbonianMod.Projectiles.Friendly.Corruption;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.Terrortoma
{
    public class TerrortomaBulb : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
        }
        public override void SetDefaults()
        {
            NPC.Size = new Vector2(116, 104);
            NPC.damage = 0;
            NPC.defense = 3;
            NPC.lifeMax = 1000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = EbonianSounds.terrortomaFlesh;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.behindTiles = true;
        }
        public override void DrawBehind(int index)
        {
            NPC.behindTiles = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.Center = Helper.TRay.Cast(NPC.Center, Vector2.UnitY, 1500) - new Vector2(0, 35);
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                SoundEngine.PlaySound(EbonianSounds.cecitiorDie, NPC.Center);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), ModContent.Find<ModGore>("EbonianMod/Terrortoma2").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), ModContent.Find<ModGore>("EbonianMod/VileSlimeGore").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), ModContent.Find<ModGore>("EbonianMod/VileSlimeGore2").Type, NPC.scale);
                for (int i = 0; i < 5; i++)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), ModContent.Find<ModGore>("EbonianMod/VileSlimeGore4").Type, NPC.scale);
                for (int i = 0; i < 3; i++)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), ModContent.Find<ModGore>("EbonianMod/EbonCrawlerGore2").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), ModContent.Find<ModGore>("EbonianMod/EbonCrawlerGore1").Type, NPC.scale);
                NPC.NewNPCDirect(NPC.GetSource_Death(), NPC.Center + new Vector2(0, 40), ModContent.NPCType<Terrortoma>());
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<OstertagiExplosion>(), 0, 0, 0);
                for (int i = 0; i < 5; i++)
                    Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(2, 3), ModContent.ProjectileType<OstertagiWorm>(), 20, 0, 0);
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.frameCounter++ % 5 == 0)
            {
                if (NPC.frame.Y < frameHeight * 3)
                    NPC.frame.Y += frameHeight;
                else
                    NPC.frame.Y = 0;
            }
        }
        SlotId cachedSound;
        public override void AI()
        {
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
        }
    }
}
