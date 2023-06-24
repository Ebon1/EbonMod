using EbonianMod.NPCs.Corruption;
using EbonianMod.Projectiles.Terrortoma;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.Crimson
{
    public class CrimeraHead : WormHead
    {
        //public override bool HasCustomBodySegments => true;
        public override bool byHeight => false;
        public override void SetStaticDefaults()
        {

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "EbonianMod/NPCs/Crimson/Crimera",
                Position = new Vector2(7f, 24f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 12f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneCorrupt && Main.hardMode)
            {
                return .15f;
            }
            else
            {
                return 0;
            }
        }

        public override void OnKill()
        {
            //Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/CrimeraGore1").Type, NPC.scale);
        }
        public override bool useNormalMovement => !(NPC.ai[2] > 300 && NPC.ai[2] < 650);
        float offset;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(offset);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            offset = reader.ReadSingle();
        }
        public override void ExtraAI()
        {
            Player player = Main.player[NPC.target];
            NPC.ai[2]++;
            if (NPC.ai[2] == 300)
                offset = Main.rand.NextFloat(1.5f, 3);
            if (NPC.ai[2] > 300 && NPC.ai[2] < 650)
            {
                NPC.damage = 0;
                Vector2 pos = player.Center + new Vector2(300, 0).RotatedBy(MathHelper.ToRadians(NPC.ai[2] * offset));
                NPC.velocity = Helper.FromAToB(NPC.Center, pos, false) * 0.025f;
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
                if (NPC.ai[2] % 10 == 0 && NPC.ai[2] > 340)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ProjectileID.BloodShot, 10, 0);
                //NPC.rotation = MathHelper.Lerp(NPC.rotation, Helper.FromAToB(NPC.Center, player.Center).ToRotation() - MathHelper.PiOver2, 0.3f);
                //NPC.rotation = MathHelper.Lerp(NPC.rotation, MathHelper.Clamp(Helper.FromAToB(NPC.Center, player.Center).ToRotation() + MathHelper.PiOver2, FollowerNPC.rotation - MathHelper.ToRadians(30), FollowerNPC.rotation + MathHelper.ToRadians(30)), 0.5f);
            }
            if (NPC.ai[2] > 800)
            {
                NPC.damage = 30;
                NPC.ai[2] = (int)(-200 * offset);
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                new FlavorTextBestiaryInfoElement("Type: Infected Creature"),
                new FlavorTextBestiaryInfoElement("A Crimera that has begun to evolve in the Crimson's time of need. It's limbs can now tunnel, and it can hurl up some of the dead blood cells which it's body generates at a breakneck pace."),
            });
        }
        public override void SetDefaults()
        {

            NPC.buffImmune[BuffID.Ichor] = true;
            // Head is 10 defence, body 20, tail 30.
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.Size = new Vector2(66, 68);
            NPC.damage = 30;
            NPC.aiStyle = -1;
        }
        /*public override int SpawnBodySegments(int segmentCount)
        {
            var source = NPC.GetSource_FromThis();
            NPC.ai[3]++;
            int latestNPC = SpawnSegment(source, BodyType, NPC.whoAmI, NPC.ai[3] == 3 ? 1 : 0);
            latestNPC = SpawnSegment(source, BodyType, latestNPC, NPC.ai[3] == 3 ? 1 : 0);
            return latestNPC;
        }*/
        public override int BodyType => ModContent.NPCType<CrimeraBody>();

        public override int TailType => ModContent.NPCType<CrimeraTail>();

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Helper.GetTexture("NPCs/Crimson/CrimeraHead");
            spriteBatch.Draw(tex, NPC.Center - (Vector2.UnitY * tex.Height / 3).RotatedBy(NPC.rotation) - screenPos, null, drawColor, NPC.rotation, new Vector2(tex.Width / 2, tex.Height / 4), NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void Init()
        {

            MinSegmentLength = 3;
            MaxSegmentLength = 3;
            MoveSpeed = 7.5f;
            Acceleration = 0.035f;
            CanFly = true;
        }
    }
    public class CrimeraBody : WormBody
    {
        public override bool byHeight => false;
        public override void HitEffect(NPC.HitInfo hitinfo)
        {
            //if (damage > NPC.life)
            //if (FollowingNPC.type == ModContent.NPCType<CrimeraBody>() && FollowerNPC.type == ModContent.NPCType<CrimeraBody>())
            //Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/CrimeraGore3").Type, NPC.scale);
            //else
            //Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/CrimeraGore2").Type, NPC.scale);
        }
        public override void SetStaticDefaults()
        {

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Helper.GetTexture("NPCs/Crimson/CrimeraBody");
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, drawColor, NPC.rotation, tex.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void SetDefaults()
        {

            NPC.buffImmune[BuffID.Ichor] = true;
            // Head is 10 defence, body 20, tail 30.
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.Size = new Vector2(30, 16);
            NPC.aiStyle = -1;
        }
        public override void Init()
        {
            MoveSpeed = 7.5f;
            Acceleration = 0.035f;

        }
    }
    public class CrimeraTail : WormTail
    {
        public override bool byHeight => false;
        public override void HitEffect(NPC.HitInfo hitinfo)
        {
            //if (damage > NPC.life)
            //Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/CrimeraGore1").Type, NPC.scale);
        }
        public override void SetStaticDefaults()
        {

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }
        public override void SetDefaults()
        {

            NPC.buffImmune[BuffID.Ichor] = true;
            // Head is 10 defence, body 20, tail 30.
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.Size = new Vector2(66 / 1.5f, 68 / 1.5f);
            NPC.aiStyle = -1;
            NPC.damage = 0;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Helper.GetTexture("NPCs/Crimson/CrimeraTail");
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, drawColor, NPC.rotation, new Vector2(tex.Width / 2, tex.Height + 2), NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        /*public override void ExtraAI()
        {
            if (++NPC.ai[2] % 30 == 0)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitY.RotatedBy(NPC.rotation) * 4, ModContent.ProjectileType<TFlameThrower>(), 10, 0);
            }
        }*/
        public override void Init()
        {
            MoveSpeed = 7.5f;
            Acceleration = 0.035f;

        }
    }
}
