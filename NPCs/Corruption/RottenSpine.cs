using EbonianMod.Projectiles.Terrortoma;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.Corruption
{
    public class RottenSpineHead : WormHead
    {
        public override bool byHeight => false;
        //public override bool HasCustomBodySegments => true;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rotten Spine");
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "EbonianMod/NPCs/Corruption/RottenSpine",
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
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/RottenSpineGore1").Type, NPC.scale);
        }
        public override void ExtraAI()
        {
            if (++NPC.ai[2] % 45 == 0)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -Vector2.UnitY.RotatedBy(NPC.rotation) * 4, ModContent.ProjectileType<TFlameThrower>(), 10, 0);
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Type: Infected Creature"),
                new FlavorTextBestiaryInfoElement("Corruption evolution has taken its worms in a curious direction, creating a purely aggressive worm with a head on either side. How it sustains itself is unknown."),
            });
        }
        public override void SetDefaults()
        {
            // Head is 10 defence, body 20, tail 30.
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.Size = new Vector2(66, 72);
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
        public override int BodyType => ModContent.NPCType<RottenSpineBody>();

        public override int TailType => ModContent.NPCType<RottenSpineTail>();

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Helper.GetTexture("NPCs/Corruption/RottenSpineHead");
            spriteBatch.Draw(tex, NPC.Center - (Vector2.UnitY * tex.Height / 3).RotatedBy(NPC.rotation) - screenPos, null, drawColor, NPC.rotation, tex.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void Init()
        {

            MinSegmentLength = 5;
            MaxSegmentLength = 5;
            MoveSpeed = 5.5f;
            Acceleration = 0.045f;
            CanFly = true;
        }
    }
    public class RottenSpineBody : WormBody
    {
        public override bool byHeight => false;
        public override void HitEffect(int hitDirection, double damage)
        {
            if (damage > NPC.life)
                if (FollowingNPC.type == ModContent.NPCType<RottenSpineBody>() && FollowerNPC.type == ModContent.NPCType<RottenSpineBody>())
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/RottenSpineGore3").Type, NPC.scale);
                else
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/RottenSpineGore2").Type, NPC.scale);
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rotten Spine");
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D main = Helper.GetTexture("NPCs/Corruption/RottenSpineBody");
            Texture2D alt = Helper.GetTexture("NPCs/Corruption/RottenSpineCoupling");
            Texture2D tex = FollowingNPC.type == ModContent.NPCType<RottenSpineBody>() && FollowerNPC.type == ModContent.NPCType<RottenSpineBody>() ? alt : main; // god bless this code
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, drawColor, NPC.rotation, tex.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void SetDefaults()
        {
            // Head is 10 defence, body 20, tail 30.
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.Size = new Vector2(22, 24);
            NPC.aiStyle = -1;
        }
        public override void Init()
        {
            MoveSpeed = 5.5f;
            Acceleration = 0.045f;

        }
    }
    public class RottenSpineTail : WormTail
    {
        public override bool byHeight => false;
        public override void HitEffect(int hitDirection, double damage)
        {
            if (damage > NPC.life)
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/RottenSpineGore1").Type, NPC.scale);
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rotten Spine Tail");
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }
        public override void SetDefaults()
        {
            // Head is 10 defence, body 20, tail 30.
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.Size = new Vector2(66 / 1.5f, 72 / 1.5f);
            NPC.aiStyle = -1;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Helper.GetTexture("NPCs/Corruption/RottenSpineTail");
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, drawColor, NPC.rotation, tex.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void ExtraAI()
        {
            if (++NPC.ai[2] % 30 == 0)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitY.RotatedBy(NPC.rotation) * 4, ModContent.ProjectileType<TFlameThrower>(), 10, 0);
            }
        }
        public override void Init()
        {
            MoveSpeed = 5.5f;
            Acceleration = 0.045f;

        }
    }
}
