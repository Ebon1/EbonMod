﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using System.IO.Pipes;
using System.IO;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;

namespace EbonianMod.NPCs.Corruption.FleshBricks
{
    public class FleshBrickSmall : ModNPC
    {
        public override string Texture => "EbonianMod/NPCs/Corruption/FleshBricks/FleshBrickSmall0";
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
            NPCID.Sets.TrailCacheLength[Type] = 5;
            NPCID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Type: Infected Creature"),
                new FlavorTextBestiaryInfoElement("Sedimentaries are theorized to be walls of the corruption's casms at an early stage in development. Like everything else in the corruption, even the mud and stone you walk on can be traced to the hive-organism itself."),
            });
        }
        public override void SetDefaults()
        {
            NPC.Size = new(56, 40);
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.defense = 5;
            NPC.lifeMax = 100;
            NPC.damage = 30;
            NPC.ai[3] = Main.rand.Next(3);
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.frameCounter++ % 5 == 0)
            {
                if (NPC.frame.Y < 5 * frameHeight)
                    NPC.frame.Y += frameHeight;
                else
                    NPC.frame.Y = 0;
            }
        }
        public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
        {
            if (NPC.rotation == MathHelper.PiOver2)
            {
                npcHitbox.Width = 40;
                npcHitbox.Height = 56;
            }
            return base.ModifyCollisionData(victimHitbox, ref immunityCooldownSlot, ref damageMultiplier, ref npcHitbox);
        }
        float widthMod = 1f, heightMod = 1f;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            if (NPC.ai[3] >= 0 && NPC.ai[3] < 3)
                texture = Helper.GetTexture("NPCs/Corruption/FleshBricks/FleshBrickSmall" + NPC.ai[3]);
            for (int i = 0; i < 5; i++)
            {
                spriteBatch.Draw(texture, NPC.oldPos[i] + NPC.Size / 2 - screenPos, NPC.frame, drawColor * 0.05f, NPC.rotation, NPC.Size / 2, new Vector2(widthMod, heightMod), SpriteEffects.None, 0);
            }
            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, new Vector2(widthMod, heightMod), SpriteEffects.None, 0);
            return false;
        }
        public override bool CheckDead()
        {
            for (int i = 0; i < 5; i++)
                Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(3, 3), ModContent.Find<ModGore>("EbonianMod/CorruptionBrickGibs" + i).Type, NPC.scale);
            return true;
        }
        public float AIState
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }
        public float AITimer
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }
        public float NextState
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }
        const int Halt = 0, X = 1, Y = 2;
        public override void OnSpawn(IEntitySource source)
        {
            NPC.ai[3] = Main.rand.Next(3);
            NPC.dontTakeDamage = true;
            if (Main.rand.NextBool())
                NPC.rotation = MathHelper.PiOver2;
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            AITimer++;
            switch (AIState)
            {
                case Halt:
                    if (AITimer < 5 && NPC.velocity.Length() > 1)
                    {
                        if (Main.rand.NextBool())
                            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CorruptGibs, NPC.velocity.X, NPC.velocity.Y);
                    }
                    widthMod = MathHelper.Lerp(widthMod, 1, 0.1f);
                    heightMod = MathHelper.Lerp(heightMod, 1, 0.1f);
                    if (NPC.Center.X - player.Center.X < 100 && NPC.Center.X - player.Center.X > -100)
                        NextState = Y;
                    else if (NPC.Center.Y - player.Center.Y < 100 && NPC.Center.Y - player.Center.Y > -100)
                        NextState = X;
                    else
                        NextState = Main.rand.Next(1, 3);
                    NPC.velocity *= 0.8f;
                    if (AITimer > 10)
                    {
                        NPC.dontTakeDamage = false;
                        NPC.velocity = Vector2.Zero;
                        if (NextState != 0)
                            AIState = NextState;
                        else
                            AIState = X;
                        AITimer = 0;
                    }
                    break;
                case X:
                    heightMod = 1f - (NPC.velocity.Length() * 0.03f);
                    widthMod = 1f + (NPC.velocity.Length() * 0.03f);
                    if (NPC.velocity.Length() < 10)
                        NPC.velocity.X += Helper.FromAToB(NPC.Center, player.Center, false).X * 0.004f;
                    if (AITimer > 20)
                    {
                        AIState = Halt;
                        AITimer = 0;
                    }
                    break;
                case Y:
                    heightMod = 1f + (NPC.velocity.Length() * 0.03f);
                    widthMod = 1f - (NPC.velocity.Length() * 0.03f);
                    if (NPC.velocity.Length() < 10)
                        NPC.velocity.Y += Helper.FromAToB(NPC.Center, player.Center, false).Y * 0.004f;
                    if (AITimer > 20)
                    {
                        AIState = Halt;
                        AITimer = 0;
                    }
                    break;
            }
        }
    }
}
