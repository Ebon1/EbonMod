using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using EbonianMod.Projectiles.Friendly.Corruption;
using Terraria.DataStructures;

using EbonianMod.Projectiles.VFXProjectiles;
using Terraria.Audio;
using System.IO;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using EbonianMod.Items.Materials;
using Terraria.GameContent.ItemDropRules;
using EbonianMod.Items.Weapons.Magic;
using EbonianMod.Items.Tiles;

namespace EbonianMod.NPCs.Crimson.CrimsonWorm
{
    public class CrimsonWormHead : WormHead
    {
        //public override string Texture => "EbonianMod/NPCs/Crimson/CrimsonWorm/CrimsonWormBody";
        public override bool extraAiAsIndex => true;
        public override int TailType => NPCType<CrimsonWormTail>();
        public override int BodyType => NPCType<CrimsonWormBody>();
        public override bool byHeight => true;
        public override bool useNormalMovement => !(NPC.ai[3] < 302 && NPC.ai[3] > 198) && !(NPC.ai[3] > 800);
        public override void SetStaticDefaults()
        {

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "EbonianMod/NPCs/Crimson/CrimsonWorm/CrimsonWorm_Bestiary",
                PortraitPositionXOverride = -100
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);

            Main.npcFrameCount[Type] = 8;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                new FlavorTextBestiaryInfoElement("Type: Infected Creature"),
                new FlavorTextBestiaryInfoElement("goon."),
            });
        }
        /*public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Helper.GetTexture("NPCs/Crimson/CrimsonWorm/CrimsonWormHead");
            spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, new Vector2(NPC.Size.X / 2, NPC.height / 1.4f), NPC.scale, SpriteEffects.None, 0);
            return false;
        }*/
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void SetDefaults()
        {

            NPC.buffImmune[BuffID.Ichor] = true;
            // Head is 10 defence, body 20, tail 30.
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.lifeMax = 11;
            NPC.value = Item.buyPrice(0, 20);
            NPC.dontTakeDamage = true;
            NPC.Size = new Vector2(72, 64);
            NPC.aiStyle = -1;

        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemType<CecitiorMaterial>(), 2, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ItemType<WormPaintingI>(), 10));
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZoneCrimson && spawnInfo.Player.ZoneOverworldHeight && Main.hardMode ? 0.01f : 0;
        }
        public override void ExtraAI()
        {
            if (NPC.lifeMax != 11)
            {
                NPC.lifeMax = 11;
                NPC.life = 11;
            }
            if (NPC.life > NPC.lifeMax)
                NPC.life = NPC.lifeMax;
            Player player = Main.player[NPC.target];
            NPC.ai[3]++;
            if (NPC.ai[3] < 200 && NPC.ai[3] > 180)
                NPC.velocity *= 0.9f;
            if (NPC.ai[3] >= 200 && NPC.ai[3] <= 300 && NPC.ai[3] % 50 == 0)
            {
                NPC.velocity = Helper.FromAToB(NPC.Center, player.Center) * 8f;
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
                SoundStyle sound = SoundID.ForceRoar;
                sound.MaxInstances = 3;
                SoundEngine.PlaySound(sound, NPC.Center);
                for (int i = 0; i < 3; i++)
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(0.5f, 2), ProjectileType<HostileGibs>(), 10, 0).tileCollide = false;

            }
            if (NPC.life > 4)
            {
                if (NPC.ai[3] > 800 && NPC.ai[3] < 810)
                {
                    NPC.velocity = Vector2.UnitY * 25;
                    NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
                }
                if (NPC.ai[3] == 850)
                {

                    NPC.ai[3] = -1000;
                }
                if (NPC.ai[3] == 840)
                {
                    for (int i = -2; i < 5; i++)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(2.5f * i, -15), ProjectileID.BloodShot, 10, 0).tileCollide = false;
                    }
                    SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);
                    NPC.velocity = -Vector2.UnitY * 25;
                    NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
                }
            }
            else
            {
                if (NPC.ai[3] >= 200 && NPC.ai[3] % 30 == 0)
                {
                    SoundEngine.PlaySound(SoundID.NPCHit1, NPC.Center);
                    for (int i = 0; i < 5; i++)
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(0.5f, 2), ProjectileType<HostileGibs>(), 10, 0).tileCollide = false;

                    if (NPC.ai[3] > 250)
                        NPC.ai[3] = 0;
                }
            }


            if (NPC.life <= 0)
            {
                NPC.dontTakeDamage = false;
                NPC.checkDead();
            }
            NPC.realLife = -1;
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.ai[3] < 362 && NPC.ai[3] > 198 || NPC.ai[3] > 800 || NPC.ai[3] < -900 || NPC.IsABestiaryIconDummy)
            {
                if (NPC.frameCounter++ % 2 == 0)
                {
                    if (NPC.frame.Y < 7 * frameHeight)
                        NPC.frame.Y += frameHeight;
                    else
                        NPC.frame.Y = 0;
                }
            }
            else
            {
                if (NPC.frameCounter++ % 5 == 0)
                {
                    if (NPC.frame.Y < 7 * frameHeight)
                        NPC.frame.Y += frameHeight;
                    else
                        NPC.frame.Y = 0;
                }
            }
        }
        public override void Init()
        {
            MinSegmentLength = 13;
            MaxSegmentLength = 13;
            CanFly = false;
            MoveSpeed = 7.5f;
            Acceleration = 0.1f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = TextureAssets.Npc[Type].Value;
            spriteBatch.Draw(tex, NPC.Center + new Vector2(0, 2).RotatedBy(NPC.rotation) - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void HitEffect(NPC.HitInfo hitinfo)
        {
            if (hitinfo.Damage > NPC.life && NPC.life <= 0)
            {
                EbonianSystem.ScreenShakeAmount = 5;
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * 0.05f, Find<ModGore>("EbonianMod/CrimsonWormSkull").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * 0.05f, Find<ModGore>("EbonianMod/CrimsonWormJaw").Type, NPC.scale);
            }
        }
    }
    public class CrimsonWormBody : WormBody
    {
        public override void HitEffect(NPC.HitInfo hitinfo)
        {
            if (hitinfo.Damage > NPC.life && !isDed)
            {
                if (NPC.ai[2] <= 6 && NPC.ai[2] > 3)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/CrimsonGoreChunk" + Main.rand.Next(4, 7)).Type, NPC.scale);

                else if (NPC.ai[2] > 6)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/CrimsonGoreChunk" + Main.rand.Next(7, 10)).Type, NPC.scale);

                else
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/CrimsonGoreChunk" + Main.rand.Next(1, 4)).Type, NPC.scale);


                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.Blood, Main.rand.NextVector2Unit().X, Main.rand.NextVector2Unit().Y, Scale: Main.rand.NextFloat(1, 2f));
                }
            }
            if (hitinfo.Damage > NPC.life && isDed)
            {
                for (int i = 1; i < 4; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/Bone" + i).Type, NPC.scale);
                }
            }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return null;
        }
        public override bool byHeight => true;
        public override void DrawBehind(int index)
        {
            if (isDed)
                Main.instance.DrawCacheNPCsMoonMoon.Add(index);
            else
                Main.instance.DrawCacheProjsBehindNPCs.Add(index);
        }
        public override void SetStaticDefaults()
        {

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
            Main.npcFrameCount[Type] = 4;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (isDed)
            {
                Texture2D tex = Helper.GetTexture("NPCs/Crimson/CrimsonWorm/CrimsonWormBody_Destroyed");

                spriteBatch.Draw(tex, Vector2.Lerp(NPC.Center, FollowingNPC.Center, 0.5f) - screenPos, NPC.frame, drawColor, Helper.LerpAngle(NPC.rotation, FollowingNPC.rotation, 0.5f), NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);

                spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);


                //if (FollowerNPC.type == NPCType<CrimsonWormTail>())
                //  spriteBatch.Draw(tex, Vector2.Lerp(NPC.Center, FollowerNPC.Center, 0.5f) - screenPos, NPC.frame, drawColor, Helper.LerpAngle(NPC.rotation, FollowerNPC.rotation, 0.5f), NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
            }
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = TextureAssets.Npc[Type].Value;
            if (NPC.ai[2] <= 6 && NPC.ai[2] > 3)
                tex = Helper.GetTexture("NPCs/Crimson/CrimsonWorm/CrimsonWormBody2");
            if (NPC.ai[2] > 6)
                tex = Helper.GetTexture("NPCs/Crimson/CrimsonWorm/CrimsonWormBody3");
            if (isDed) return;
            scale = MathHelper.Lerp(scale, 1, 0.1f);
            if (timer2++ % 4 == 0)
            {
                timer++;
                if (timer == NPC.ai[2])
                    scale = 1.15f;
                if (timer >= 13)
                    timer = 0;
            }
            spriteBatch.Draw(tex, Vector2.Lerp(NPC.Center, FollowingNPC.Center, 0.5f) - screenPos, NPC.frame, drawColor, Helper.LerpAngle(NPC.rotation, FollowingNPC.rotation, 0.5f), NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);

            spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale * scale, SpriteEffects.None, 0);
        }
        float scale = 1, timer, timer2;
        public override void ExtraAI()
        {
            NPC head = Main.npc[(int)NPC.ai[3]];
            if (!head.active)
                return;
            if (!head.dontTakeDamage)
            {
                NPC.immortal = false;
                NPC.life = 0;
                NPC.checkDead();
            }
            if (head.ai[3] == 198 || head.ai[3] == 800)
                Init();

            NPC.hide = isDed;

            NPC.realLife = -1;
        }
        bool isDed;
        public override bool CheckDead()
        {
            NPC head = Main.npc[(int)NPC.ai[3]];
            if (!head.dontTakeDamage)
                return true;
            if (!isDed)
            {
                isDed = true;
                NPC.immortal = true;
                NPC.dontTakeDamage = true;
                NPC.life = 1;
                head.life--;
            }
            return false;
        }
        public override void SetDefaults()
        {

            NPC.buffImmune[BuffID.Ichor] = true;
            // Head is 10 defence, body 20, tail 30.
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.Size = new Vector2(72, 32);
            NPC.aiStyle = -1;
            NPC.lifeMax = 75;

        }
        public override void FindFrame(int frameHeight)
        {
            NPC head = Main.npc[(int)NPC.ai[3]];
            bool headIdle = !(head.ai[3] < 462 && head.ai[3] > 198 || head.ai[3] > 800 || head.ai[3] < -900);
            if (isDed)
            {
                if (NPC.frameCounter++ % (headIdle ? 5 : 2) == 0)
                {
                    if (NPC.frame.Y < 7 * frameHeight)
                        NPC.frame.Y += frameHeight;
                    else
                        NPC.frame.Y = 0;
                }
            }
            else if (!isDed)
            {
                if (NPC.frameCounter++ % (headIdle ? 5 : 2) == 0)
                {
                    if (NPC.frame.Y < 3 * frameHeight)
                        NPC.frame.Y += frameHeight;
                    else
                        NPC.frame.Y = 0;
                }
            }
            /*else if (isDed && headIdle)
            {
                NPC.frame.Y = head.frame.Y / 72 * frameHeight;
            }*/
            /*else if (!isDed && headIdle)
                if (NPC.frameCounter++ % 5 == 0)
                {
                    if (NPC.frame.Y < 3 * frameHeight)
                        NPC.frame.Y += frameHeight;
                }*/

        }
        public override void Init()
        {
            NPC.realLife = -1;
            MoveSpeed = 7.5f;
            Acceleration = 0.15f;
            if (NPC.ai[2] < 4)
                NPC.frame.Y = (int)(NPC.ai[2] * 34);
            else switch (NPC.ai[2])
                {
                    case 4:
                        NPC.frame.Y = 0 * 34;
                        break;
                    case 5:
                        NPC.frame.Y = 1 * 34;
                        break;
                    case 6:
                        NPC.frame.Y = 2 * 34;
                        break;
                    case 7:
                        NPC.frame.Y = 3 * 34;
                        break;
                    case 8:
                        NPC.frame.Y = 0 * 34;
                        break;
                    case 9:
                        NPC.frame.Y = 1 * 34;
                        break;
                    case 10:
                        NPC.frame.Y = 2 * 34;
                        break;
                    case 11:
                        NPC.frame.Y = 3 * 34;
                        break;
                    case 12:
                        NPC.frame.Y = 0 * 34;
                        break;
                    case 13:
                        NPC.frame.Y = 1 * 34;
                        break;
                }
        }
    }

    public class CrimsonWormTail : WormTail
    {
        public override void HitEffect(NPC.HitInfo hitinfo)
        {
            if (hitinfo.Damage > NPC.life && NPC.life <= 0)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/CrimsonGoreChunk7").Type, NPC.scale);
            }
        }
        public override bool byHeight => true;
        public override void SetStaticDefaults()
        {

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
            Main.npcFrameCount[Type] = 8;
        }
        public override void SetDefaults()
        {

            NPC.buffImmune[BuffID.Ichor] = true;
            // Head is 10 defence, body 20, tail 30.
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.dontTakeDamage = true;
            NPC.Size = new Vector2(68, 40);
            NPC.aiStyle = -1;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = TextureAssets.Npc[Type].Value;
            spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2 + new Vector2(0, 4), NPC.scale, SpriteEffects.None, 0);
        }
        public override void ExtraAI()
        {
            NPC.realLife = -1;
            NPC.rotation = FollowingNPC.rotation;
            NPC.Center = FollowingNPC.Center - NPC.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2) * (FollowingNPC.height + 6);
        }
        public override void FindFrame(int frameHeight)
        {
            NPC head = Main.npc[(int)NPC.ai[3]];
            bool headIdle = !(head.ai[3] < 462 && head.ai[3] > 198 || head.ai[3] > 800 || head.ai[3] < -900);
            if (NPC.frameCounter++ % (headIdle ? 5 : 2) == 0)
            {
                if (NPC.frame.Y < 7 * frameHeight)
                    NPC.frame.Y += frameHeight;
                else
                    NPC.frame.Y = 0;
            }
        }
        public override void Init()
        {
            NPC.realLife = -1;
            MoveSpeed = 7.5f;
            Acceleration = 0.15f;
        }
    }
}
