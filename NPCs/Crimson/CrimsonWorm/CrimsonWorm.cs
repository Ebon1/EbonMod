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

namespace EbonianMod.NPCs.Crimson.CrimsonWorm
{
    public class CrimsonWormHead : WormHead
    {
        //public override string Texture => "EbonianMod/NPCs/Crimson/CrimsonWorm/CrimsonWormBody";
        public override bool extraAiAsIndex => true;
        public override int TailType => ModContent.NPCType<CrimsonWormTail>();
        public override int BodyType => ModContent.NPCType<CrimsonWormBody>();
        public override bool byHeight => true;
        public override bool useNormalMovement => !(NPC.ai[3] < 402 && NPC.ai[3] > 198) && !(NPC.ai[3] > 800);
        public override void SetStaticDefaults()
        {

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
            Main.npcFrameCount[Type] = 8;
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
            NPC.dontTakeDamage = true;
            NPC.Size = new Vector2(72, 64);
            NPC.aiStyle = -1;

        }
        public override void ExtraAI()
        {
            if (NPC.life > NPC.lifeMax)
                NPC.life = NPC.lifeMax;
            NPC.lifeMax = 11;
            Player player = Main.player[NPC.target];
            NPC.ai[3]++;
            if (NPC.ai[3] < 200 && NPC.ai[3] > 180)
                NPC.velocity *= 0.9f;
            if (NPC.ai[3] >= 200 && NPC.ai[3] <= 400 && NPC.ai[3] % 100 == 0)
            {
                NPC.velocity = Helper.FromAToB(NPC.Center, player.Center) * 13f;
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
                SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);
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
                if (NPC.ai[3] >= 400)
                {

                    NPC.ai[3] = -2000;
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
            if (NPC.ai[3] < 462 && NPC.ai[3] > 198 || NPC.ai[3] > 800 || NPC.ai[3] < -900 || NPC.IsABestiaryIconDummy)
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
            CanFly = true;
            MoveSpeed = 5.5f;
            Acceleration = 0.15f;
        }
        public override void HitEffect(NPC.HitInfo hitinfo)
        {
            if (hitinfo.Damage > NPC.life)
            {
                EbonianSystem.ScreenShakeAmount = 5;
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/CrimsonWormSkull").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/CrimsonWormJaw").Type, NPC.scale);
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
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/CrimsonGoreChunk" + Main.rand.Next(4, 7)).Type, NPC.scale);

                else if (NPC.ai[2] > 6)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/CrimsonGoreChunk" + Main.rand.Next(7, 10)).Type, NPC.scale);

                else
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/CrimsonGoreChunk" + Main.rand.Next(1, 4)).Type, NPC.scale);


                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.Blood, Main.rand.NextVector2Unit().X, Main.rand.NextVector2Unit().Y, Scale: Main.rand.NextFloat(1, 2f));
                }
            }
            if (hitinfo.Damage > NPC.life && isDed)
            {
                for (int i = 1; i < 4; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/Bone" + i).Type, NPC.scale);
                }
            }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return null;
        }
        public override bool byHeight => true;
        public override void SetStaticDefaults()
        {

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
            Main.npcFrameCount[Type] = 4;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!isDed)
            {
                Texture2D tex = TextureAssets.Npc[Type].Value;
                if (NPC.ai[2] <= 6 && NPC.ai[2] > 3)
                    tex = Helper.GetTexture("NPCs/Crimson/CrimsonWorm/CrimsonWormBody2");
                if (NPC.ai[2] > 6)
                    tex = Helper.GetTexture("NPCs/Crimson/CrimsonWorm/CrimsonWormBody3");
                spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
            }
            else
            {
                Texture2D tex = Helper.GetTexture("NPCs/Crimson/CrimsonWorm/CrimsonWormBody_Destroyed");
                spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
            }
            return false;
        }
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
            if (hitinfo.Damage > NPC.life)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/CrimsonGoreChunk7").Type, NPC.scale);
            }
        }
        public override bool byHeight => true;
        public override void SetStaticDefaults()
        {

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
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
            Texture2D tex = TextureAssets.Npc[Type].Value;
            spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2 + new Vector2(0, 4), NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void ExtraAI()
        {
            NPC.realLife = -1;
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
