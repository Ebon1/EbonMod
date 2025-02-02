using EbonianMod.Common.Systems;
using EbonianMod.Projectiles.Enemy.Corruption;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using EbonianMod.Items.Materials;
using EbonianMod.Projectiles.Terrortoma;
using Terraria.Utilities;

namespace EbonianMod.NPCs.Corruption
{
    public class VileTear : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;
            NPCID.Sets.TrailCacheLength[Type] = 5;
            NPCID.Sets.TrailingMode[Type] = 3;
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Rotation = -MathHelper.PiOver2
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Evil"),
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.NPCs.VileTear.Bestiary"),
            });
        }
        public override void SetDefaults()
        {
            NPC.width = 136;
            NPC.height = 136;
            NPC.damage = 10;
            NPC.defense = 14;
            NPC.lifeMax = 1600;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.DD2_DrakinHurt;
            NPC.DeathSound = SoundID.DD2_DrakinDeath;
            NPC.value = Item.buyPrice(0, 1);
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.buffImmune[BuffID.CursedInferno] = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(0, 25);
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZoneCorrupt && Main.hardMode ? 0.01f : 0;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.CursedFlame, 1, 10, 25));
            npcLoot.Add(ItemDropRule.Common(ItemType<Items.Weapons.Melee.CorruptionClaw>(), 3));
            npcLoot.Add(ItemDropRule.Common(ItemType<TerrortomaMaterial>(), 2, 1, 3));
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D tex = Helper.GetTexture("NPCs/Corruption/VileTear");
            Texture2D tex2 = Helper.GetTexture("NPCs/Corruption/VileTear_Glow");
            var fadeMult = 1f / NPC.oldPos.Length;
            if (AIState == 1)
                for (int i = 0; i < NPC.oldPos.Length; i++)
                {
                    Main.EntitySpriteDraw(tex, NPC.oldPos[i] + NPC.Size / 2 - screenPos, NPC.frame, drawColor * NPC.ai[3] * (1f - fadeMult * i), NPC.oldRot[i], NPC.Size / 2, (Vector2.One + scaleOffset) * NPC.scale, effects, 0);
                }
            Main.EntitySpriteDraw(tex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, (Vector2.One + scaleOffset) * NPC.scale, effects, 0);
            Main.EntitySpriteDraw(tex2, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.Size / 2, (Vector2.One + scaleOffset) * NPC.scale, effects, 0);


            if (Main.LocalPlayer.HasBuff(BuffID.Hunter) && !NPC.IsABestiaryIconDummy)
            {
                Main.EntitySpriteDraw(tex, NPC.Center - screenPos, NPC.frame, NPC.HunterPotionColor(), NPC.rotation, NPC.Size / 2, (Vector2.One + scaleOffset) * NPC.scale, effects, 0);
                Main.EntitySpriteDraw(tex2, NPC.Center - screenPos, NPC.frame, NPC.HunterPotionColor(), NPC.rotation, NPC.Size / 2, (Vector2.One + scaleOffset) * NPC.scale, effects, 0);
            }
            return false;
        }
        public override void HitEffect(NPC.HitInfo hitinfo)
        {
            if (hitinfo.Damage > NPC.life && NPC.life <= 0)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int i = 0; i < 4; i++)
                        Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/CorruptionBrickGibs2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/CorruptionBrickGibs4").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/CorruptionBrickGibs0").Type, NPC.scale);
                }
            }
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
        public float AITimer2
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }
        public float AITimer3;
        public override void FindFrame(int frameHeight)
        {
            //NPC.frame.Y = frameHeight * (AIState == 1 && AITimer > 90 && AITimer < 150 ? 0 : 1);
        }
        Vector2 p;
        Vector2 scaleOffset;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            if (player.Distance(NPC.Center) > 1800) return;
            //NPC.spriteDirection = NPC.direction = NPC.velocity.X > 0 ? -1 : 1;
            NPC.spriteDirection = NPC.direction = -1;
            switch (AIState)
            {
                case 0:
                    AITimer++;
                    NPC.damage = 10;
                    if (AITimer < 200)
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center) * 3.5f, 0.01f);
                    if (AITimer > 200)
                        NPC.velocity *= 0.9f;
                    if (AITimer > 1)
                        NPC.rotation = Helper.LerpAngle(NPC.rotation, NPC.velocity.ToRotation() + MathHelper.Pi, 0.25f);
                    if (AITimer >= 230)
                    {
                        AITimer2 = Main.rand.Next(4);
                        AITimer = 0;
                        AIState++;
                    }
                    break;
                case 1:
                    AITimer++;
                    if (AITimer < 70)
                    {
                        p = NPC.Center + Helper.FromAToB(NPC.Center, player.Center) * 1000;
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center + new Vector2(0, -300).RotatedBy(MathF.Sin(AITimer + Main.GlobalTimeWrappedHourly * 3) * 1.5f), true) * 20, 0.025f);
                        NPC.rotation = Helper.LerpAngle(NPC.rotation, Helper.FromAToB(NPC.Center, player.Center).ToRotation() + MathHelper.Pi, 0.25f);
                    }
                    if (AITimer > 50 && AITimer < 70)
                    {
                        NPC.rotation = Helper.LerpAngle(NPC.rotation, Helper.FromAToB(NPC.Center, player.Center).ToRotation() + MathHelper.Pi, 0.25f);
                        NPC.ai[3] = MathHelper.Lerp(NPC.ai[3], 1, 0.1f);
                    }

                    if (AITimer == 30)
                    {
                        NPC.damage = 40;
                        NPC.velocity *= 0.5f;
                        //Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center), ProjectileType<VileTearTelegraph>(), 0, 0);
                        SoundEngine.PlaySound(EbonianSounds.cursedToyCharge.WithPitchOffset(-0.4f), NPC.Center);
                    }
                    if (AITimer == 70)
                    {
                        NPC.velocity = Vector2.Zero;
                        SoundEngine.PlaySound(EbonianSounds.terrortomaDash.WithPitchOffset(-0.25f), NPC.Center);
                        scaleOffset = new Vector2(0.3f, -0.3f);
                        if (AITimer3 == 0)
                        {
                            for (int i = 0; i < 15; i++)
                                Dust.NewDustPerfect(NPC.Center, DustID.CursedTorch, Helper.FromAToB(NPC.Center + new Vector2(32, 0).RotatedBy(NPC.rotation), p).RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(3, 6));
                            for (int i = -1; i < 2; i++)
                                Projectile.NewProjectile(null, NPC.Center - new Vector2(32, -40 * i).RotatedBy(NPC.rotation), Helper.FromAToB(NPC.Center + new Vector2(32, 50 * i).RotatedBy(NPC.rotation), p), ProjectileType<RegorgerBolt>(), 20, 0);
                        }
                    }

                    if (AITimer >= 70 && AITimer < 89)
                    {
                        scaleOffset = Vector2.Lerp(scaleOffset, Vector2.Zero, 0.1f);
                        NPC.velocity += Helper.FromAToB(NPC.Center, p) * 1.4f;
                    }
                    if (AITimer > 70 && AITimer < 100 && AITimer3 == 1)
                        Projectile.NewProjectile(null, NPC.Center + Main.rand.NextVector2Circular(30, 30) + new Vector2(NPC.width / 3, 0).RotatedBy(NPC.rotation).RotatedByRandom(MathHelper.PiOver2), -NPC.velocity.RotatedByRandom(MathHelper.PiOver4) * 0.2f, ProjectileType<TFlameThrower>(), 20, 0);
                    if (AITimer > 100)
                    {
                        scaleOffset = Vector2.Zero;
                        NPC.ai[3] = MathHelper.Lerp(NPC.ai[3], 0, 0.1f);
                        NPC.damage = 10;
                        NPC.velocity *= 0.9f;
                    }

                    if (AITimer > 120)
                    {
                        NPC.velocity = Vector2.Zero;
                        AITimer = 0;
                        AITimer3 = (AITimer3 == 0 ? 1 : 0);
                        AIState++;
                    }
                    break;
                case 2:
                    AITimer++;
                    NPC.rotation = Helper.LerpAngle(NPC.rotation, Helper.FromAToB(NPC.Center, player.Center).ToRotation() + MathHelper.Pi, 0.25f);
                    if (AITimer > 30)
                    {
                        AITimer2++;
                        if (AITimer2 < 4)
                        {
                            AIState = 1;
                        }
                        else
                        {
                            AIState = 0;
                        }
                        AITimer = 0;
                    }
                    break;
            }
        }
    }
}
