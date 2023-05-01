using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using EbonianMod.Projectiles.Friendly.Corruption;
using Terraria.DataStructures;
using EbonianMod.Projectiles.Exol;
using EbonianMod.Projectiles.VFXProjectiles;
using Terraria.Audio;
using System.IO;
using Terraria.GameContent.Bestiary;
using EbonianMod.Misc;
using System.Linq;

namespace EbonianMod.NPCs.Cecitior
{
    public class CecitiorEye : ModNPC
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<Cecitior>()], quickUnlock: true);
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Type: Organic Construct"),
                new FlavorTextBestiaryInfoElement("Ocular organs used by Cecitor. The creature is haphazardly created, leaving many flaws in its existence, such as the lack of defended eyes.\n\nOne has to wonder how much of a pain it would be to get lemon there."),
            });
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gazer");
            Main.npcFrameCount[NPC.type] = 19;
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 12323;
            NPC.dontTakeDamage = true;
            NPC.damage = 0;
            NPC.noTileCollide = true;
            NPC.defense = 10;
            NPC.knockBackResist = 0;
            NPC.width = 32;
            NPC.height = 32;
            NPC.npcSlots = 1f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.buffImmune[24] = true;
            NPC.netAlways = true;
        }
        Verlet verlet;
        int timer;
        public override void OnSpawn(IEntitySource source)
        {
            timer = Main.rand.Next(40);
            verlet = new(NPC.Center, 7, 16, 1, true, true, 10);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC center = Main.npc[(int)NPC.ai[0]];
            if (NPC.IsABestiaryIconDummy || !center.active || center.type != ModContent.NPCType<Cecitior>())
                return true;

            if (verlet != null)
            {
                verlet.Update(NPC.Center, center.Center);
                verlet.gravity = (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.25f;
                verlet.Draw(spriteBatch, "NPCs/Cecitior/CecitiorChain");
            }
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC center = Main.npc[(int)NPC.ai[0]];
            if (NPC.IsABestiaryIconDummy || !center.active || center.type != ModContent.NPCType<Cecitior>())
                return;
            Texture2D a = Helper.GetTexture("NPCs/Cecitior/CecitiorChain_base");
            Texture2D b = Helper.GetTexture("NPCs/Cecitior/CecitiorEye");
            if (verlet != null)
                spriteBatch.Draw(a, verlet.firstP.position - new Vector2(0, 20).RotatedBy(Helper.FromAToB(verlet.firstP.position, verlet.points[5].position, reverse: true).ToRotation() - 1.57f) - screenPos, null, drawColor, Helper.FromAToB(verlet.firstP.position, verlet.points[5].position, reverse: true).ToRotation() - 1.57f, a.Size() / 2, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(b, NPC.Center - Main.screenPosition, NPC.frame, Color.White, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
        }
        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter % 5 == 0 && timer < 0)
            {
                if (NPC.frame.Y < frameHeight * 15)
                    NPC.frame.Y += frameHeight;
                else
                    NPC.frame.Y = 0;
            }
        }
        public override void AI()
        {
            timer--;
            Player player = Main.player[NPC.target];
            NPC center = Main.npc[(int)NPC.ai[0]];
            if (!center.active || center.type != ModContent.NPCType<Cecitior>())
            {
                NPC.life = 0;
                return;
            }
        }
    }
}
