using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.Crimson
{
    public class JuvenileBrain : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Direction = 1,
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Evil"),
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.NPCs.JuvenileBrain.Bestiary"),
            });
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.TissueSample, 5, 1, 4));
            npcLoot.Add(ItemDropRule.Common(ItemType<Items.Weapons.Magic.GoreSceptre>(), 45));
        }
        public override void HitEffect(NPC.HitInfo hit)
        {

            Vector2 startP = NPC.position;
            float len = Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X);


            if (NPC.Center.X < Main.player[NPC.target].Center.X)
                startP.X = Main.player[NPC.target].Center.X + len;
            else
                startP.X = Main.player[NPC.target].Center.X - len;

            if (alpha > 0.9f && NPC.ai[3] < 0)
            {
                NPC.ai[3] = 40;
                NPC.direction = NPC.spriteDirection = -NPC.direction;
                NPC.position = startP + NPC.velocity;
            }
            if ((hit.Damage >= NPC.life && NPC.life <= 0))
            {
                for (int i = 0; i < 4; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/TinyBrainGore").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/TinyBrainGore2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/TinyBrainGore3").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/TinyBrainGore4").Type, NPC.scale);
                }
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/SanguinaryBrainGore").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/SanguinaryBrainGore2").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/SanguinaryBrainGore3").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/SanguinaryBrainGore4").Type, NPC.scale);
            }
        }
        public override void SetDefaults()
        {
            NPC.width = 86 / 2;
            NPC.height = 76;
            NPC.aiStyle = 5;
            AIType = 205;
            NPC.damage = 20;
            NPC.knockBackResist = 0f;
            NPC.defense = 7;
            NPC.lifeMax = 100;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 200f;
            NPC.knockBackResist = 0.4f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
        }


        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZoneCrimson && NPC.downedBoss2 ? 0.05f : 0;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 pos, Color drawColor)
        {
            Microsoft.Xna.Framework.Color color9 = Lighting.GetColor((int)((double)NPC.position.X + (double)NPC.width * 0.5) / 16, (int)(((double)NPC.position.Y + (double)NPC.height * 0.5) / 16.0));
            Vector2 orig = new Vector2((float)(Request<Texture2D>("EbonianMod/NPCs/Crimson/JuvenileBrain").Value.Width / 2), (float)(Request<Texture2D>("EbonianMod/NPCs/Crimson/JuvenileBrain").Value.Height / Main.npcFrameCount[NPC.type] / 2));
            SpriteEffects spriteEffects2;
            Rectangle frame6 = NPC.frame;
            Color col = drawColor * ((MathF.Sin(Main.GlobalTimeWrappedHourly * 3) + 2) * 0.5f);
            for (int i = 0; i < 4; i++)
            {
                Vector2 startP = NPC.position;
                float len = Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X);

                bool realOne = i % 2 != 0;

                if (NPC.Center.X < Main.LocalPlayer.Center.X)
                    realOne = i % 2 == 0;

                if (i == 0 || i == 2)
                {
                    startP.X = Main.player[NPC.target].Center.X + len;
                    spriteEffects2 = NPC.direction * (realOne ? -1 : 1) < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                }
                else
                {
                    startP.X = Main.player[NPC.target].Center.X - len;
                    spriteEffects2 = NPC.direction * (realOne ? -1 : 1) < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                }

                startP.X -= (float)(NPC.width / 2);
                float _alpha = realOne ? alpha : 1;
                Main.spriteBatch.Draw(Request<Texture2D>("EbonianMod/NPCs/Crimson/JuvenileBrain").Value, new Vector2(startP.X - pos.X + (float)(NPC.width / 2) - (float)Request<Texture2D>("EbonianMod/NPCs/Crimson/JuvenileBrain").Value.Width * NPC.scale / 2f + orig.X * NPC.scale, startP.Y - pos.Y + (float)NPC.height - (float)Request<Texture2D>("EbonianMod/NPCs/Crimson/JuvenileBrain").Value.Height * NPC.scale / (float)Main.npcFrameCount[NPC.type] + 4f + orig.Y * NPC.scale + NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(frame6), col * _alpha, NPC.rotation * .1f, orig, NPC.scale, spriteEffects2, 0);

                if (Main.LocalPlayer.HasBuff(BuffID.Hunter) && !NPC.IsABestiaryIconDummy)
                    if (realOne)
                        Main.spriteBatch.Draw(Request<Texture2D>("EbonianMod/NPCs/Crimson/JuvenileBrain").Value, new Vector2(startP.X - pos.X + (float)(NPC.width / 2) - (float)Request<Texture2D>("EbonianMod/NPCs/Crimson/JuvenileBrain").Value.Width * NPC.scale / 2f + orig.X * NPC.scale, startP.Y - pos.Y + (float)NPC.height - (float)Request<Texture2D>("EbonianMod/NPCs/Crimson/JuvenileBrain").Value.Height * NPC.scale / (float)Main.npcFrameCount[NPC.type] + 4f + orig.Y * NPC.scale + NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(frame6), NPC.HunterPotionColor(), NPC.rotation * .1f, orig, NPC.scale, spriteEffects2, 0);

            }
            return false;
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
        public override void FindFrame(int frameHeight)
        {
            if (AITimer <= 405 || NPC.IsABestiaryIconDummy)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter < 5)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
                else if (NPC.frameCounter < 10)
                {
                    NPC.frame.Y = 1 * frameHeight;
                }
                else if (NPC.frameCounter < 15)
                {
                    NPC.frame.Y = 2 * frameHeight;
                }
                else if (NPC.frameCounter < 20)
                {
                    NPC.frame.Y = 3 * frameHeight;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }
            else
            {
                if (NPC.velocity.Length() < 5)
                {
                    NPC.frameCounter++;
                    if (NPC.frameCounter < 5)
                    {
                        NPC.frame.Y = 0 * frameHeight;
                    }
                    else if (NPC.frameCounter < 10)
                    {
                        NPC.frame.Y = 1 * frameHeight;
                    }
                    else if (NPC.frameCounter < 15)
                    {
                        NPC.frame.Y = 2 * frameHeight;
                    }
                    else if (NPC.frameCounter < 20)
                    {
                        NPC.frame.Y = 3 * frameHeight;
                    }
                    else
                    {
                        NPC.frameCounter = 0;
                    }
                }
                else
                {
                    NPC.frameCounter++;
                    if (NPC.frameCounter < 5)
                    {
                        NPC.frame.Y = 4 * frameHeight;
                    }
                    else if (NPC.frameCounter < 10)
                    {
                        NPC.frame.Y = 5 * frameHeight;
                    }
                    else
                    {
                        NPC.frameCounter = 0;
                    }
                }
            }
        }
        float alpha = 1;
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            Vector2 startP = NPC.position;
            float len = Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X);
            return null;

            if (NPC.ai[2] % 40 < 20)
                startP.X = Main.player[NPC.target].Center.X + len;
            else
                startP.X = Main.player[NPC.target].Center.X - len;


            if (alpha < 0.5f) return null;

            position = startP + NPC.Size / 2 + new Vector2(-NPC.width / 2, NPC.height / 2);

            return true;
        }
        public override void AI()
        {
            NPC.ai[3]--;
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            if (AITimer < 406)
            {
                NPC.FaceTarget();
            }
            else
            {
                NPC.spriteDirection = NPC.velocity.X > 0 ? 1 : -1;
                NPC.direction = NPC.velocity.X > 0 ? 1 : -1;
            }
            AITimer++;
            if (AITimer >= 370)
            {
                if (AITimer >= 395 && AITimer < 410)
                {
                    alpha = Lerp(alpha, 0f, 0.4f);
                    if (AITimer < 405)
                    {
                        NPC.velocity -= Helper.FromAToB(NPC.Center, player.Center) * 0.5f;
                    }
                    if (AITimer == 405)
                    {
                        NPC.velocity = Helper.FromAToB(NPC.Center, player.Center) * 20;
                        SoundEngine.PlaySound(SoundID.ForceRoar.WithPitchOffset(0.7f), NPC.Center);
                    }
                }
                if (AITimer > 410)
                    NPC.velocity *= 0.99f;
                if (AITimer > 435)
                    NPC.velocity *= 0.95f;
                if (AITimer >= 450)
                {
                    NPC.velocity = Vector2.Zero;
                    AITimer = 0;
                }
            }
            else
            {
                alpha = Lerp(alpha, 1, 0.1f);
            }

        }
    }
}
