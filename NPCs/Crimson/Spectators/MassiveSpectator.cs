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
    public class MassiveSpectator : ModNPC
    {
        public override void SetStaticDefaults()
        {

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "EbonianMod/NPCs/Crimson/Spectators/MassiveSpectator_Bestiary",
                Position = new Vector2(7f, 24f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 32f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return (spawnInfo.Player.ZoneCrimson && Main.hardMode && !NPC.AnyNPCs(Type) && !NPC.AnyNPCs(NPCType<Cecitior.Cecitior>())) ? 0.5f : 0;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = NPCType<Cecitior.Cecitior>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);


            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                new FlavorTextBestiaryInfoElement("Type: Organ"),
                new FlavorTextBestiaryInfoElement("These towering eyes seem to be connected to something at the very base of the Crimson. They only appeared very recently, so not much else is known about them."),
            });
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemType<Panopticon>(), 40));
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 1000;
            NPC.damage = 0;
            NPC.noTileCollide = true;
            NPC.defense = 3;
            NPC.knockBackResist = 0;
            NPC.width = 46;
            NPC.height = 44;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.buffImmune[24] = true;
            SoundStyle hit = EbonianSounds.fleshHit;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = hit;
            NPC.netAlways = true;
            NPC.hide = true;
            NPC.chaseable = false;
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
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/Gnasher0").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/CrimsonGoreChunk3").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/CrimsonGoreChunk1").Type, NPC.scale);
                for (int i = 0; i < 5; i++)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/CrimsonGoreChunk2").Type, NPC.scale);
                for (int i = 0; i < 3; i++)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/CrimsonGoreChunk2").Type, NPC.scale);
                for (int i = 0; i < verlet.points.Count; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), verlet.points[i].position, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/CrimsonGoreChunk2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), verlet.points[i].position, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/CrimorrhageChain").Type, NPC.scale);
                }
                NPC.NewNPCDirect(NPC.GetSource_Death(), NPC.Center + new Vector2(0, -800), NPCType<Cecitior.Cecitior>());
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ProjectileType<BloodShockwave2>(), 0, 0, 0);
            }
        }
        bool found = false;
        public override void AI()
        {
            NPC.timeLeft = 10;
            NPC.despawnEncouraged = false;
            NPC.chaseable = false;
            if (NPC.AnyNPCs(NPCType<Cecitior.Cecitior>())) NPC.active = false;
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            if (!found && (stalkBase == Vector2.Zero || Helper.TRay.CastLength(Helper.TRay.Cast(NPC.Center, Vector2.UnitY, 400), Vector2.UnitY, 32, true) > 16))
            {
                Vector2 direction = Vector2.UnitY.RotatedBy(MathHelper.PiOver4 + MathHelper.PiOver4 * 0.25f);
                int attempts = 0;
                while (Helper.TRay.CastLength(NPC.Center, direction, 400) >= 399 && attempts++ <= 300)
                {
                    NPC.Center += Vector2.UnitY * 4;
                    NPC.Center += Vector2.UnitX * Main.rand.NextFloat(-10, 10);
                    direction = Vector2.UnitY;
                }
                if (Helper.TRay.CastLength(Helper.TRay.Cast(NPC.Center, direction, 400), Vector2.UnitY, 32) < 16)
                {
                    stalkBase = Helper.TRay.Cast(NPC.Center, direction, 400) + new Vector2(0, 40);
                    found = true;
                }
                return;
            }
            NPC.rotation = NPC.Center.FromAToB(player.Center).ToRotation() + MathHelper.Pi;
            NPC.velocity = NPC.Center.FromAToB(player.Center - new Vector2(NPC.ai[1] * NPC.ai[3], NPC.ai[2]), false) * 0.005f;
            NPC.Center = Vector2.Clamp(NPC.Center, stalkBase - new Vector2(200), stalkBase + new Vector2(200));
            if (NPC.Center.Distance(stalkBase) > 200)
            {
                NPC.ai[1] = Main.rand.NextFloat(20, 100);
                NPC.ai[2] = Main.rand.NextFloat(30, 100);
                NPC.ai[3] = Main.rand.NextFloatDirection();
            }
            if (verlet != null)
                verlet.Update(stalkBase, NPC.Center + new Vector2(13, 0).RotatedBy(NPC.rotation));
        }
    }
}
