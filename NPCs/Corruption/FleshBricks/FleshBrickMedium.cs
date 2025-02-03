using System;
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
using Terraria.GameContent.Bestiary;

namespace EbonianMod.NPCs.Corruption.FleshBricks
{
    public class FleshBrickMedium : ModNPC
    {
        public override string Texture => "EbonianMod/NPCs/Corruption/FleshBricks/FleshBrickMedium0";
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
            Main.npcFrameCount[Type] = 6;
            NPCID.Sets.TrailCacheLength[Type] = 5;
            NPCID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Evil"),
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.NPCs.FleshBrickBig.Bestiary"),
            });
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.RottenChunk, 2, 1, 4));
        }
        public override void SetDefaults()
        {
            NPC.Size = new(68, 68);
            NPC.noGravity = true;
            NPC.knockBackResist = 0.8f;
            NPC.aiStyle = -1;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.defense = 5;
            NPC.lifeMax = 150;
            NPC.damage = 20;
            NPC.ai[3] = Main.rand.Next(2);
            NPC.value = Item.buyPrice(0, 0, 3);

        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZoneCorrupt && NPC.downedBoss2 ? 0.05f : 0;
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
        float widthMod = 1f, heightMod = 1f;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture1 = Helper.GetTexture("NPCs/Corruption/FleshBricks/FleshBrickMedium0");
            Texture2D texture2 = Helper.GetTexture("NPCs/Corruption/FleshBricks/FleshBrickMedium1");
            Texture2D texture = NPC.ai[3] == 1 ? texture1 : texture2;
            for (int i = 0; i < 5; i++)
            {
                spriteBatch.Draw(texture, NPC.oldPos[i] + NPC.Size / 2 - screenPos, NPC.frame, drawColor * 0.05f, NPC.rotation, NPC.Size / 2, new Vector2(widthMod, heightMod), SpriteEffects.None, 0);
            }
            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, new Vector2(widthMod, heightMod), SpriteEffects.None, 0);
            return false;
        }
        public override bool CheckDead()
        {
            for (int j = 0; j < 3; j++)
                for (int i = 0; i < 5; i++)
                    Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(3, 3), Find<ModGore>("EbonianMod/CorruptionBrickGibs" + i).Type, NPC.scale);
            NPC.NewNPCDirect(NPC.GetSource_Death(), NPC.Center, NPCType<FleshBrickSmall>());
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
            NPC.ai[3] = Main.rand.Next(2);
            NPC.dontTakeDamage = true;
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            AITimer++;
            if (NPC.Center.Distance(player.Center) > 1000) return;
            float off = Helper.TRay.CastLength(NPC.Center, NPC.Center.FromAToB(player.Center), 400) < 200 ? -1 : 1;
            float dir = 0;
            switch (AIState)
            {
                case Halt:
                    if (AITimer < 10 && NPC.velocity.Length() > 1)
                    {
                        Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CorruptGibs, NPC.velocity.X, NPC.velocity.Y);
                    }
                    widthMod = MathHelper.Lerp(widthMod, 1, 0.1f);
                    heightMod = MathHelper.Lerp(heightMod, 1, 0.1f);
                    if (NPC.Center.X - player.Center.X < NPC.width / 2 && NPC.Center.X - player.Center.X > -NPC.width / 2)
                        NextState = Y;
                    else if (NPC.Center.Y - player.Center.Y < NPC.height / 2 && NPC.Center.Y - player.Center.Y > -NPC.height / 2)
                        NextState = X;
                    else
                        NextState = Main.rand.Next(1, 3);
                    NPC.velocity *= 0.8f;
                    if (AITimer > 20)
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
                    heightMod = 1f - (NPC.velocity.Length() * 0.015f);
                    widthMod = 1f + (NPC.velocity.Length() * 0.015f);
                    if (off == -1)
                    {
                        if (player.Center.Y.CloseTo(NPC.Center.Y, NPC.width * 2))
                        {
                            if (!Collision.CanHit(NPC, player))
                                dir = -PiOver4 + (Helper.FromAToB(NPC.Center, player.Center).Y > 0 ? Pi : 0);
                            else if (!Collision.CanHit(NPC, player))
                                dir = PiOver4 + (Helper.FromAToB(NPC.Center, player.Center).Y > 0 ? Pi : 0);
                            else dir = 0;
                        }
                    }
                    if (NPC.velocity.Length() < 5)
                        NPC.velocity += new Vector2(Helper.FromAToB(NPC.Center, player.Center + NPC.Center.FromAToB(player.Center) * 200, false).X * 0.003f, 0).RotatedBy(dir);
                    if (NPC.Center.X.CloseTo(player.Center.X, NPC.height) && AITimer > 3)
                        AITimer = 31;
                    if (AITimer > 30)
                    {
                        AIState = Halt;
                        AITimer = 0;
                    }
                    break;
                case Y:
                    heightMod = 1f + (NPC.velocity.Length() * 0.015f);
                    widthMod = 1f - (NPC.velocity.Length() * 0.015f);
                    if (off == -1)
                    {
                        if (player.Center.X.CloseTo(NPC.Center.X, NPC.width * 2))
                        {
                            if (!Collision.CanHit(NPC, player))
                                dir = (Helper.FromAToB(NPC.Center, player.Center).X > 0 ? -1 : 1) * PiOver4 + (Helper.FromAToB(NPC.Center, player.Center).Y > 0 ? Pi : 0);
                            else dir = 0;
                        }
                    }
                    if (NPC.velocity.Length() < 5)
                        NPC.velocity += new Vector2(0, Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 64) + NPC.Center.FromAToB(player.Center - new Vector2(0, 64)) * 200, false).Y * 0.003f).RotatedBy(dir);
                    if (NPC.Center.Y.CloseTo(player.Center.Y, NPC.width) && AITimer > 3)
                        AITimer = 31;
                    if (AITimer > 30)
                    {
                        AIState = Halt;
                        AITimer = 0;
                    }
                    break;
            }
        }
    }
}
