using EbonianMod.Common.Systems;
using EbonianMod.Common.Systems.Misc;
using EbonianMod.Items.Misc;
using EbonianMod.Items.Pets;
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
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.Crimson.Spectators
{
    public class MiniSpectator : ModNPC
    {
        public override void SetStaticDefaults()
        {

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                //CustomTexturePath = "EbonianMod/NPCs/Crimson/Spectators/MiniSpectator_Bestiary",
                Position = new Vector2(7f, 24f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 32f,
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return (spawnInfo.Player.ZoneCrimson) ? 0.15f : 0;
        }
        /*public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                new FlavorTextBestiaryInfoElement("Type: Organ"),
                new FlavorTextBestiaryInfoElement("These towering eyes seem to be connected to something at the very base of the Crimson. They only appeared very recently, so not much else is known about them."),
            });
        }*/
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 500;
            NPC.damage = 0;
            NPC.noTileCollide = true;
            NPC.defense = 20;
            NPC.knockBackResist = 0;
            NPC.width = 24;
            NPC.height = 20;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.buffImmune[24] = true;
            SoundStyle hit = EbonianSounds.fleshHit;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = hit;
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
            verlet = new Verlet(NPC.Center, 10, Main.rand.Next(15, 25), Main.rand.NextFloat(-1, -0.5f), true, true, 4);

            NPC.ai[1] = Main.rand.NextFloat(20, 100);
            NPC.ai[2] = Main.rand.NextFloat(30, 100);
            NPC.ai[3] = Main.rand.NextFloatDirection();
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (verlet != null)
                verlet.Draw(spriteBatch, Texture + "_Vein", null, null, useRotEnd: true, endRot: NPC.rotation + MathHelper.PiOver2);
            Texture2D texture = Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, drawColor, NPC.rotation, texture.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            Texture2D glow = Request<Texture2D>(Texture + "_Glow").Value;
            spriteBatch.Draw(glow, NPC.Center - Main.screenPosition, null, Color.White, NPC.rotation, texture.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        Vector2 stalkBase;
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                SoundEngine.PlaySound(EbonianSounds.cecitiorDie, NPC.Center);
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/WormyGore").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/WormyGore2").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/WormyGore3").Type, NPC.scale);
                for (int i = 0; i < 3; i++)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/WormyGore2").Type, NPC.scale);

                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/WormyGore3").Type, NPC.scale);
                for (int i = 0; i < verlet.points.Count; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), verlet.points[i].position, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/JelleyeFishGore1").Type, NPC.scale);
                }
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ProjectileType<BloodShockwave2>(), 0, 0, 0);
            }
        }
        public override void AI()
        {
            NPC.timeLeft = 10;
            NPC.despawnEncouraged = false;
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            if (stalkBase == Vector2.Zero || Helper.TRay.CastLength(stalkBase, Vector2.UnitY, 32) > 16)
            {
                Vector2 direction = Vector2.UnitY.RotatedBy(MathHelper.PiOver4 + MathHelper.PiOver4 * 0.25f);
                int attempts = 0;
                while (attempts++ <= 200)
                {
                    if (Helper.TRay.CastLength(NPC.Center, Vector2.UnitY, 100) > 99)
                        NPC.Center += Vector2.UnitY;
                    if (attempts == 1)
                        direction = Vector2.UnitY.RotatedBy(-1 * MathHelper.PiOver4 - MathHelper.PiOver4 * 0.25f);
                    else
                        direction = Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4);
                }
                stalkBase = Helper.TRay.Cast(NPC.Center, direction, 800) + new Vector2(0, 40);
                return;
            }
            NPC.rotation = NPC.Center.FromAToB(player.Center).ToRotation() + MathHelper.Pi;
            NPC.velocity = NPC.Center.FromAToB(player.Center - new Vector2(NPC.ai[1] * NPC.ai[3], NPC.ai[2]), false) * 0.005f;
            NPC.Center = Vector2.Clamp(NPC.Center, stalkBase - new Vector2(200), stalkBase + new Vector2(200));
            if (NPC.Center.Distance(stalkBase) > 300)
            {
                NPC.ai[1] = Main.rand.NextFloat(20, 100);
                NPC.ai[2] = Main.rand.NextFloat(30, 100);
                NPC.ai[3] = Main.rand.NextFloatDirection();
            }
            if (verlet != null)
                verlet.Update(stalkBase, NPC.Center + new Vector2(4, 0).RotatedBy(NPC.rotation));
        }
    }
}
