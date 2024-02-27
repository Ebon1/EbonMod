using EbonianMod.Common.Systems;
using EbonianMod.Items.Misc;
using EbonianMod.Misc;
using EbonianMod.NPCs.Cecitior;
using EbonianMod.Projectiles.Friendly.Corruption;
using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.Crimson.Spectators
{
    public class MassiveSpectator : ModNPC
    {
        public override void SetStaticDefaults()
        {

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "EbonianMod/NPCs/Crimson/Spectators/MassiveSpectator_Bestiary",
                Position = new Vector2(7f, 24f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 32f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                new FlavorTextBestiaryInfoElement("Type: Organ"),
                new FlavorTextBestiaryInfoElement("These towering eyes are the largest spectators ever recorded. It would seem they are connected to something at the very base of the Crimson. These spectators have never been seen before you found them. One may theorize they were created to directly observe you, your weaponry, your tactics and more."),
            });
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 1000;
            NPC.damage = 0;
            NPC.noTileCollide = true;
            NPC.defense = 20;
            NPC.knockBackResist = 0;
            NPC.width = 46;
            NPC.height = 44;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.buffImmune[24] = true;
            SoundStyle hit = EbonianSounds.fleshHit;
            NPC.HitSound = hit;
            NPC.netAlways = true;
            NPC.hide = true;
        }
        Verlet verlet;
        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        }
        public override void OnSpawn(IEntitySource source)
        {
            verlet = new Verlet(NPC.Center, 16, 20, -0.25f, true, true, 30);

            NPC.ai[1] = Main.rand.NextFloat(20, 100);
            NPC.ai[2] = Main.rand.NextFloat(30, 100);
            NPC.ai[3] = Main.rand.NextFloatDirection();
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (verlet != null)
                verlet.Draw(spriteBatch, Texture + "_Vein", null, Texture + "_VeinBase", useRotEnd: true, endRot: NPC.rotation + MathHelper.PiOver2);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, drawColor, NPC.rotation, texture.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            spriteBatch.Draw(glow, NPC.Center - Main.screenPosition, null, Color.White, NPC.rotation, texture.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        Vector2 stalkBase;
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                SoundEngine.PlaySound(EbonianSounds.cecitiorDie, NPC.Center);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), ModContent.Find<ModGore>("EbonianMod/Gnasher0").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), ModContent.Find<ModGore>("EbonianMod/CrimsonGoreChunk3").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), ModContent.Find<ModGore>("EbonianMod/CrimsonGoreChunk1").Type, NPC.scale);
                for (int i = 0; i < 5; i++)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), ModContent.Find<ModGore>("EbonianMod/CrimsonGoreChunk2").Type, NPC.scale);
                for (int i = 0; i < 3; i++)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), ModContent.Find<ModGore>("EbonianMod/CrimsonGoreChunk2").Type, NPC.scale);
                for (int i = 0; i < verlet.points.Count; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), verlet.points[i].position, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), ModContent.Find<ModGore>("EbonianMod/CrimsonGoreChunk2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), verlet.points[i].position, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), ModContent.Find<ModGore>("EbonianMod/CrimorrhageChain").Type, NPC.scale);
                }
                NPC.NewNPCDirect(NPC.GetSource_Death(), NPC.Center + new Vector2(0, -1300), ModContent.NPCType<Cecitior.Cecitior>());
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BloodShockwave2>(), 0, 0, 0);
            }
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            if (stalkBase == Vector2.Zero)
            {
                Vector2 direction = Vector2.UnitY.RotatedBy(MathHelper.PiOver4 + MathHelper.PiOver4 * 0.25f);
                int attempts = 0;
                while (Helper.TRay.CastLength(NPC.Center, direction, 200) >= 200 && attempts++ <= 100)
                {
                    if (attempts == 1)
                        direction = Vector2.UnitY.RotatedBy(-1 * MathHelper.PiOver4 - MathHelper.PiOver4 * 0.25f);
                    else
                        direction = Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4 + MathHelper.PiOver4 * 0.25f);
                }
                stalkBase = Helper.TRay.Cast(NPC.Center, direction, 200) + new Vector2(0, 40);
            }
            NPC.rotation = NPC.Center.FromAToB(player.Center).ToRotation() + MathHelper.Pi;
            //if (player.Distance(stalkBase) < 320)
            //{
            NPC.velocity = NPC.Center.FromAToB(player.Center - new Vector2(NPC.ai[1] * NPC.ai[3], NPC.ai[2]), false) * 0.005f;
            NPC.Center = Vector2.Clamp(NPC.Center, stalkBase - new Vector2(300), stalkBase + new Vector2(300));
            if (NPC.Center.Distance(stalkBase) > 300)
            {
                NPC.ai[1] = Main.rand.NextFloat(20, 100);
                NPC.ai[2] = Main.rand.NextFloat(30, 100);
                NPC.ai[3] = Main.rand.NextFloatDirection();
            }
            //}
            /*else if (NPC.Distance(stalkBase) < 320)
            {
                NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(NPC.ai[1] * NPC.ai[3], NPC.ai[2]), false) * 0.005f;
            }*/
            //else
            if (verlet != null)
                verlet.Update(stalkBase, NPC.Center + new Vector2(23, 0).RotatedBy(NPC.rotation));
        }
    }
}
