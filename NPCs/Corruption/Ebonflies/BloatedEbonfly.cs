using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using EbonianMod.Projectiles.VFXProjectiles;
using EbonianMod.Items.Misc;
using EbonianMod.Tiles.Paintings;
using EbonianMod.Items.Tiles;
using Terraria.GameContent.ItemDropRules;

namespace EbonianMod.NPCs.Corruption.Ebonflies
{
    public class BloatedEbonfly : ModNPC
    {
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemType<WaspPaintingI>(), 90));
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Type: Infected Creature"),
                new FlavorTextBestiaryInfoElement("A strange mutation of the Ebonfly. Eating something that has been lit ablaze by the cursed inferno, it has extended its natural lifespan, but also become far more volatile."),
            });
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            Texture2D tex = Helper.GetTexture("NPCs/Corruption/Ebonflies/BloatedEbonfly_Glow");
            Texture2D tex2 = Helper.GetTexture("NPCs/Corruption/Ebonflies/BloatedEbonfly");
            Texture2D tex3 = Helper.GetTexture("NPCs/Corruption/Ebonflies/BloatedEbonfly_Glow2");
            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(tex2, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, effects, 0);
            Main.EntitySpriteDraw(tex, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.Size / 2, NPC.scale, effects, 0);
            Main.spriteBatch.Reload(BlendState.Additive);
            Main.EntitySpriteDraw(tex3, NPC.Center - screenPos, NPC.frame, Color.LawnGreen * glowAlpha, NPC.rotation, NPC.Size / 2, NPC.scale, effects, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneCorrupt)
            {
                return .4f;
            }
            else
            {
                return 0;
            }
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = 5;
            AIType = 205;
            NPC.width = 40;
            NPC.height = 38;
            NPC.npcSlots = 0.1f;
            NPC.lifeMax = 200;
            NPC.damage = 0;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.buffImmune[24] = true;
            NPC.noTileCollide = false;
            NPC.defense = 0;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter % 5 == 0)
            {
                if (NPC.frame.Y < frameHeight * 5)
                    NPC.frame.Y += frameHeight;
                else
                    NPC.frame.Y = 0;
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.scale = Main.rand.NextFloat(0.8f, 1.2f);
            NPC.Center += Main.rand.NextVector2CircularEdge(40, 40);
            NPC.velocity = Main.rand.NextVector2Unit();
        }
        float glowAlpha = 0;
        Vector2 lastPos;
        public override void PostAI()
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.whoAmI != NPC.whoAmI)
                {
                    if (npc.Center.Distance(NPC.Center) < npc.width * npc.scale)
                    {
                        NPC.velocity += NPC.Center.FromAToB(npc.Center, true, true) * 0.25f;
                    }
                    if (npc.Center == NPC.Center)
                    {
                        NPC.velocity = Main.rand.NextVector2Unit() * 5;
                    }
                }
            }
            if (Main.LocalPlayer.Center.Distance(NPC.Center) < 900)
                if (++NPC.ai[3] > 100 * NPC.scale)
                {
                    NPC.aiStyle = -1;
                    AIType = 0;
                    NPC.velocity *= 0.99f;
                    if (NPC.ai[3] >= 120)
                        NPC.Center = lastPos + Main.rand.NextVector2Circular(4 * glowAlpha, 4 * glowAlpha);
                    else
                        lastPos = NPC.Center;
                    glowAlpha += 0.03f;
                    if (NPC.ai[3] > 150)
                    {
                        Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ProjectileType<OstertagiExplosion>(), 50, 0);
                        a.friendly = true;
                        a.hostile = true;
                        NPC.StrikeInstantKill();
                    }
                }
        }
        public override bool CheckDead()
        {
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore3").Type, NPC.scale);
            return true;
        }
    }
}
