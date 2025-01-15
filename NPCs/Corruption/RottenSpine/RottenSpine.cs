using EbonianMod.Common.Systems;
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
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.Corruption.RottenSpine
{
    public class RottenSpineHead : WormHead
    {
        public override bool byHeight => false;
        public override bool extraAiAsIndex => true;
        //public override bool HasCustomBodySegments => true;
        public override void SetStaticDefaults()
        {

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "EbonianMod/NPCs/Corruption/RottenSpine/RottenSpine",
                Position = new Vector2(7f, 24f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 12f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneCorrupt && spawnInfo.Player.ZoneRockLayerHeight && Main.hardMode)
            {
                return .15f;
            }
            else
            {
                return 0;
            }
        }
        public override bool useNormalMovement => true;
        public override void OnKill()
        {
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/RottenSpineGore1").Type, NPC.scale);
        }
        public override void ExtraAI()
        {
            Player player = Main.player[NPC.target];

            if (++NPC.ai[2] % 35 == 0 && NPC.ai[2] % 550 > 200)
            {
                SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
                Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center + NPC.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2) * NPC.height, -Vector2.UnitY.RotatedBy(NPC.rotation) * 10, ProjectileType<TFlameThrower>(), 10, 0).tileCollide = true; ;
            }

            if (NPC.ai[2] % 550 < 200)
            {
                ForcedTargetPosition = player.Center + Helper.FromAToB(player.Center, NPC.Center) * 1000;
            }
            else
                ForcedTargetPosition = null;
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
        public override int BodyType => NPCType<RottenSpineBody>();

        public override int TailType => NPCType<RottenSpineTail>();

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Helper.GetTexture("NPCs/Corruption/RottenSpine/RottenSpineHead");
            spriteBatch.Draw(tex, NPC.Center - (Vector2.UnitY * tex.Height / 3).RotatedBy(NPC.rotation) - screenPos, null, drawColor, NPC.rotation, tex.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void Init()
        {

            MinSegmentLength = 9;
            MaxSegmentLength = 9;
            MoveSpeed = 5.5f;
            Acceleration = 0.07f;
            CanFly = false;
        }
    }
    public class RottenSpineBody : WormBody
    {
        public override bool byHeight => false;
        public override void HitEffect(NPC.HitInfo hitinfo)
        {
            if (hitinfo.Damage > NPC.life && NPC.life <= 0)
                if (NPC.ai[3] == 3)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/RottenSpineGore3").Type, NPC.scale);
                else
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/RottenSpineGore2").Type, NPC.scale);
        }
        public override void SetStaticDefaults()
        {

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy) return false;
            Texture2D main = Helper.GetTexture("NPCs/Corruption/RottenSpine/RottenSpineBody");
            Texture2D alt = Helper.GetTexture("NPCs/Corruption/RottenSpine/RottenSpineCoupling");
            Texture2D tex = NPC.ai[3] == 3 ? alt : main; // god bless this code

            if (FollowingNPC == HeadSegment)
                spriteBatch.Draw(main, Vector2.Lerp(NPC.Center, FollowingNPC.Center, 0.5f) - screenPos, null, drawColor, Helper.LerpAngle(NPC.rotation, FollowingNPC.rotation, 0.5f), tex.Size() / 2, NPC.scale, SpriteEffects.None, 0);

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
            Acceleration = 0.07f;

        }
    }
    public class RottenSpineTail : WormTail
    {
        public override bool byHeight => false;
        public override void HitEffect(NPC.HitInfo hitinfo)
        {
            if (hitinfo.Damage > NPC.life && NPC.life <= 0)
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/RottenSpineGore1").Type, NPC.scale);
        }
        public override void SetStaticDefaults()
        {

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
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
            Texture2D tex = Helper.GetTexture("NPCs/Corruption/RottenSpine/RottenSpineTail");
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, drawColor, NPC.rotation, tex.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void ExtraAI()
        {
            NPC.rotation = FollowingNPC.rotation;
            NPC.Center = FollowingNPC.Center - NPC.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2) * (FollowingNPC.height + 6);
            if (++NPC.ai[2] % 35 == 0 && HeadSegment.ai[2] % 550 < 200)
            {
                SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
                Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center + NPC.velocity, Vector2.UnitY.RotatedBy(NPC.rotation) * 10, ProjectileType<TFlameThrower>(), 10, 0).tileCollide = true; ;
            }
        }
        public override void Init()
        {
            MoveSpeed = 5.5f;
            Acceleration = 0.07f;

        }
    }
}
