using EbonianMod.Buffs;
using EbonianMod.Common;
using EbonianMod.Common.Systems;
using EbonianMod.Common.Systems.Misc.Dialogue;
using EbonianMod.Dusts;
using EbonianMod.Items.BossTreasure;
using EbonianMod.Items.Pets;
using EbonianMod.Items.Tiles;
using EbonianMod.Items.Weapons.Magic;
using EbonianMod.Items.Weapons.Melee;
using EbonianMod.Items.Weapons.Ranged;
using EbonianMod.Items.Weapons.Summoner;
using EbonianMod.Projectiles.ArchmageX;
using EbonianMod.Projectiles.VFXProjectiles;
using EbonianMod.Tiles;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.Utilities;

namespace EbonianMod.NPCs.ArchmageX
{
    [AutoloadBossHead]
    public class ArchmageX : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.Size = new Vector2(50, 78);
            NPC.lifeMax = 9500;
            NPC.defense = 4;
            NPC.damage = 0;
            NPC.boss = true;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0;
            NPC.lavaImmune = true;
            NPC.dontTakeDamage = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Music = 0;
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance * bossAdjustment);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Type: 'Master' Magician"),
                new FlavorTextBestiaryInfoElement("The most magnificent and brilliant sorcerer to ever walk this earth... An unmatched master of wizardry! A true magician! The Archmage!!\n\nAll self-proclaimed, of course."),
            });
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(new CommonDrop(ItemType<PhantasmalGreatsword>(), 4));
            notExpertRule.OnSuccess(new CommonDrop(ItemType<XareusPotion>(), 4));
            notExpertRule.OnSuccess(new CommonDrop(ItemType<StaffofXWeapon>(), 4));
            notExpertRule.OnSuccess(new CommonDrop(ItemType<ArchmageXTome>(), 4));
            npcLoot.Add(notExpertRule);

            npcLoot.Add(ItemDropRule.Common(ItemType<XTrophy>(), 4));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemType<SheepPet>(), 4));

            npcLoot.Add(ItemDropRule.BossBag(ItemType<ArchmageBag>()));
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Helper.GetTexture(Texture);
            Texture2D singularArm = Helper.GetTexture(Texture + "_Arm");
            Texture2D head = Helper.GetTexture(Texture + "_Head");
            Texture2D headGlow = Helper.GetTexture(Texture + "_HeadGlow");
            Texture2D manaPot = TextureAssets.Projectile[ProjectileType<XManaPotion>()].Value;
            Texture2D staff = Helper.GetTexture("Items/Weapons/Magic/StaffOfXItem");
            Texture2D bigStaff = Helper.GetTexture("Items/Weapons/Magic/StaffOfX");
            Texture2D bigStaffBloom = Helper.GetTexture("Items/Weapons/Magic/StaffOfX_Bloom");
            Texture2D heli = Helper.GetExtraTexture("Sprites/ArchmageXHeli");
            Texture2D heliGlow = Helper.GetExtraTexture("Sprites/ArchmageXHeli_Glow");

            /*
            Vector2 staffP = NPC.Center + rightArmRot.ToRotationVector2().RotatedBy(MathHelper.Pi - MathHelper.PiOver4 * 0.8f + (MathHelper.ToRadians((headYOff + 2) * 4) * NPC.direction)) * 6;
            if (NPC.direction == 1)
                staffP = NPC.Center + new Vector2(NPC.width / 2 - 4, -2) + rightArmRot.ToRotationVector2().RotatedBy(MathHelper.Pi + MathHelper.PiOver4 * 0.8f + (MathHelper.ToRadians((headYOff + 2) * 4) * NPC.direction)) * -6;
            float staffRot = rightArmRot + MathHelper.Pi * (NPC.direction == 1 ? .5f : AIState == HelicopterBlades ? .8f : 1f);
            */

            Vector2 staffP = NPC.Center + rightArmRot.ToRotationVector2().RotatedBy(MathHelper.Pi - MathHelper.PiOver4 * 0.8f + (MathHelper.ToRadians((headYOff + 2) * 4) * NPC.direction)) * 0.25f;
            if (NPC.direction == 1)
                staffP = NPC.Center + new Vector2(NPC.width / 2 - 4, -10) + (rightArmRot + (MathHelper.ToRadians((headYOff + 2) * 5) * NPC.direction)).ToRotationVector2().RotatedBy(MathHelper.Pi + MathHelper.PiOver4 * 0.8f + (MathHelper.ToRadians((headYOff + 2) * 4) * NPC.direction)) * -.25f;
            float staffRot = rightArmRot + (MathHelper.ToRadians((headYOff + 2) * 5) * NPC.direction) + MathHelper.Pi * (NPC.direction == 1 ? .5f : AIState == HelicopterBlades ? .8f : 1f);


            Vector2 heliP = NPC.Center - new Vector2(singularArm.Width + (NPC.direction == -1 ? 36 : 46), 0).RotatedBy(NPC.direction == -1 ? (rightArmRot - MathHelper.PiOver2 * .87f - MathHelper.PiOver4 * 0.1f) : (rightArmRot - (MathHelper.Pi - MathHelper.PiOver4) + MathHelper.PiOver4 * 0.5f)) - new Vector2(0, NPC.direction == 1 ? 18 : 12);
            float heliR = Helper.FromAToB(NPC.Center, heliP).ToRotation() + MathHelper.PiOver2 * (NPC.direction == -1 ? 1.1f : 0.8f);

            spriteBatch.Draw(staff, staffP - screenPos, null, Color.White * staffAlpha, staffRot, new Vector2(0, staff.Height), NPC.scale, SpriteEffects.None, 0f);

            spriteBatch.Draw(bigStaff, staffP - screenPos, null, Color.White * bigStaffAlpha, staffRot, new Vector2(0, bigStaff.Height), NPC.scale, SpriteEffects.None, 0f);


            Vector2 scale = new Vector2(1f, 0.25f);
            Main.spriteBatch.Reload(BlendState.Additive);

            for (int i = 0; i < 6; i++)
                spriteBatch.Draw(bigStaff, staffP - screenPos, null, Color.White * bigStaffBloomAlpha, staffRot, new Vector2(0, bigStaff.Height), NPC.scale, SpriteEffects.None, 0f);


            Main.spriteBatch.Reload(EbonianMod.SpriteRotation);
            EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(MathHelper.ToRadians(Main.GlobalTimeWrappedHourly * 1200));
            EbonianMod.SpriteRotation.Parameters["scale"].SetValue(scale);
            Vector4 col = (Color.White * heliAlpha).ToVector4();
            col.W = heliAlpha * 0.25f;
            EbonianMod.SpriteRotation.Parameters["uColor"].SetValue(col);


            for (int i = 12; i > 0; i--)
            {
                Vector2 pos = heliP + new Vector2(i * 0.2f, 0).RotatedBy(heliR + MathHelper.PiOver2);
                Main.spriteBatch.Draw(heliGlow, pos - Main.screenPosition, null, Color.White * heliAlpha, heliR, heli.Size() / 2, 1f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            Main.spriteBatch.Reload(EbonianMod.SpriteRotation);
            EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(MathHelper.ToRadians(Main.GlobalTimeWrappedHourly * 1200));
            EbonianMod.SpriteRotation.Parameters["scale"].SetValue(scale);
            col.W = heliAlpha;
            EbonianMod.SpriteRotation.Parameters["uColor"].SetValue(col);
            for (int i = 12; i > 0; i--)
            {
                Vector2 pos = heliP + new Vector2(i * 0.2f, 0).RotatedBy(heliR + MathHelper.PiOver2);
                Main.spriteBatch.Draw(heli, pos - Main.screenPosition, null, Color.White * heliAlpha, heliR, heli.Size() / 2, 1f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(effect: null);



            spriteBatch.Draw(manaPot, NPC.Center - new Vector2(singularArm.Width + (NPC.direction == -1 ? 8 : 18), 0).RotatedBy(NPC.direction == -1 ? (rightArmRot - MathHelper.PiOver2) : (rightArmRot - (MathHelper.Pi - MathHelper.PiOver4))) - new Vector2(0, 18) - screenPos, null, Color.White * 0.9f * manaPotAlpha, 0, manaPot.Size() / 2, NPC.scale, SpriteEffects.None, 0f);

            spriteBatch.Draw(singularArm, NPC.Center - new Vector2(NPC.direction == -1 ? -14 : -6, 18).RotatedBy(NPC.rotation) - screenPos, null, drawColor, leftArmRot + (MathHelper.ToRadians((headYOff + 2) * 5) * -NPC.direction), new Vector2(NPC.direction == 1 ? singularArm.Width : 0, 0), NPC.scale, NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            spriteBatch.Draw(singularArm, NPC.Center - new Vector2(singularArm.Width - 2 + (NPC.direction == -1 ? 4 : 0), 0) - new Vector2(NPC.direction == 1 ? -42 : -24, 18).RotatedBy(NPC.rotation) - screenPos, null, drawColor, rightArmRot + (MathHelper.ToRadians((headYOff + 2) * 5) * NPC.direction) - (NPC.direction == -1 ? MathHelper.PiOver4 * 0.5f : 0), new Vector2(NPC.direction == -1 ? singularArm.Width : 0, 0), NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(head, NPC.Center + new Vector2(NPC.direction == -1 ? 6 : 12, -38 + headYOff * 0.5f).RotatedBy(NPC.rotation) - screenPos, headFrame, drawColor, headRotation, new Vector2(36, 42) / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            spriteBatch.Draw(headGlow, NPC.Center + new Vector2(NPC.direction == -1 ? 6 : 12, -38 + headYOff * 0.5f).RotatedBy(NPC.rotation) - screenPos, headFrame, Color.White, headRotation, new Vector2(36, 42) / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(headGlow, NPC.Center + new Vector2(NPC.direction == -1 ? 6 : 12, -38).RotatedBy(NPC.rotation) - screenPos, headFrame, Color.White, headRotation, new Vector2(36, 42) / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (GetArenaRect().Size().Length() > 1054) return;
            arenaAlpha = MathHelper.Lerp(arenaAlpha, 1, 0.1f);

            arenaVFXOffset += 0.005f;
            if (arenaVFXOffset >= 1)
                arenaVFXOffset = 0;
            arenaVFXOffset = MathHelper.Clamp(arenaVFXOffset, float.Epsilon, 1 - float.Epsilon);
            List<VertexPositionColorTexture> verticesR = new List<VertexPositionColorTexture>();
            Texture2D texture = Helper.GetExtraTexture("wavyLaser2");
            Vector2 startR = GetArenaRect().BottomRight() + Vector2.UnitY * 16 - Main.screenPosition;
            Vector2 offVert = (Helper.FromAToB(GetArenaRect().BottomRight() + Vector2.UnitY * 16, GetArenaRect().BottomRight() - Vector2.UnitY * 16 * 6, false));
            float rotVert = Helper.FromAToB(startR, startR + offVert).ToRotation();
            float s = 0f;
            float sLin = 0f;
            for (float i = 0; i < 1; i += 0.001f)
            {
                if (i < 0.5f)
                    s = MathHelper.Clamp(i * 3.5f, 0, 0.5f);
                else
                    s = MathHelper.Clamp((-i + 1) * 2, 0, 0.5f);

                if (i < 0.5f)
                    sLin = MathHelper.Clamp(i, 0, 0.5f);
                else
                    sLin = MathHelper.Clamp((-i + 1), 0, 0.5f);

                float cA = MathHelper.Lerp(s, sLin, i);
                float vertSize = MathHelper.SmoothStep(5, 18, s);

                float __off = arenaVFXOffset;
                if (__off > 1) __off = -__off + 1;
                float _off = __off + i;

                Color col = new Color(60, 2, 113) * (arenaAlpha * s);
                verticesR.Add(Helper.AsVertex(startR - new Vector2(-vertSize * 0.5f, vertSize) + offVert * i + new Vector2(vertSize, 0).RotatedBy(rotVert + MathHelper.PiOver2), new Vector2(_off, 0), col * 2));
                verticesR.Add(Helper.AsVertex(startR - new Vector2(-vertSize * 0.5f, vertSize) + offVert * i + new Vector2(vertSize, 0).RotatedBy(rotVert - MathHelper.PiOver2), new Vector2(_off, 1), col * 2));
            }
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (verticesR.Count >= 3)
            {
                Helper.DrawTexturedPrimitives(verticesR.ToArray(), PrimitiveType.TriangleStrip, texture, false);
            }
            Rectangle rect = new Rectangle(GetArenaRect().X - (int)Main.screenPosition.X, GetArenaRect().Y - (int)Main.screenPosition.Y, GetArenaRect().Width, GetArenaRect().Height);

            for (int i = 0; i < 5; i++)
                Main.spriteBatch.Draw(Helper.GetExtraTexture("pixel"), rect, Color.Indigo * arenaFlash);
            Main.spriteBatch.ApplySaved();

        }
        float leftArmRot, rightArmRot;
        float headRotation, headYOff, headOffIncrementOffset;
        float manaPotAlpha, staffAlpha = 1f, bigStaffAlpha, bigStaffBloomAlpha, heliAlpha, arenaVFXOffset, arenaAlpha;
        Rectangle headFrame = new Rectangle(0, 0, 36, 42);
        public void FacePlayer()
        {
            Player player = Main.player[NPC.target];
            NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
            NPC.spriteDirection = NPC.direction;
        }
        public override void OnSpawn(IEntitySource source)
        {
            sCenter = NPC.Center - new Vector2(0, 130);
        }

        public const int
            NeutralFace = 0,
            ShockedFace = 1 * 42,
            SadFace = 2 * 42,
            DisappointedFace = 3 * 42,
            AngryFace = 4 * 42,
            LightSmirkFace = 5 * 42,
            SmirkFace = 6 * 42,
            VeryShockedFace = 7 * 42,
            BlinkingFace = 8 * 42,
            AssholeFace = 9 * 42;
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
        public float AITimer3
        {
            get => NPC.ai[3];
            set => NPC.ai[3] = value;
        }
        public const int Phase2Transition = -5, Taunt = -4, Despawn = -3, Death = -2, Idle = -1, Spawn = 0,
            PhantasmalSpirit = 1, ShadowflamePuddles = 2, SpectralOrbs = 3, MagnificentFireballs = 4, SineLaser = 5, AmethystCloseIn = 6,
            HelicopterBlades = 7, GiantAmethyst = 8, Micolash = 9, TheSheepening = 10, ManaPotion = 11, PhantasmalBlast = 12, ShadowflameRift = 13,
            AmethystBulletHell = 14, AmethystStorm = 15, BullshitRain = 16, BONK = 17;
        public List<int> Phase1AttackPool = new List<int>()
        {
            ManaPotion,
            PhantasmalSpirit,
            AmethystCloseIn,
            TheSheepening,
            SpectralOrbs,
            SineLaser,
            AmethystBulletHell,
            Micolash,
            ShadowflamePuddles,
            MagnificentFireballs,
            AmethystStorm,
            //adding weight to the rng
            SineLaser,
            PhantasmalSpirit,
            ShadowflamePuddles,
            SpectralOrbs,
            MagnificentFireballs
        };
        public List<int> Phase2AttackPool = new List<int>()
        {
            ManaPotion,
            PhantasmalSpirit,
            AmethystCloseIn,
            GiantAmethyst,
            TheSheepening,
            SpectralOrbs,
            AmethystBulletHell,
            Micolash,
            ShadowflameRift,
            SineLaser,
            ShadowflamePuddles,
            MagnificentFireballs,
            HelicopterBlades,
            PhantasmalBlast,
            AmethystStorm,

        };
        public List<int> Phase3AttackPool = new List<int>()
        {
            ManaPotion,
            PhantasmalSpirit,
            AmethystCloseIn,
            GiantAmethyst,
            SineLaser,
            TheSheepening,
            SpectralOrbs,
            AmethystBulletHell,
            Micolash,
            ShadowflameRift,
            ShadowflamePuddles,
            MagnificentFireballs,
            HelicopterBlades,
            PhantasmalBlast,
            AmethystStorm,
            BONK,
            BullshitRain
        };
        public List<float> MeleeAttacks = new List<float>()
        {
            ManaPotion,
            PhantasmalSpirit,
            MagnificentFireballs,
            GiantAmethyst,
            AmethystStorm,
            HelicopterBlades,
            ShadowflamePuddles,
            SpectralOrbs,
            Micolash,
            AmethystBulletHell,
            AmethystCloseIn,

        };
        float Next = 1;
        float arenaFlash;
        void IdleAnimation()
        {
            if (NPC.collideY || NPC.velocity.Y.CloseTo(0))
            {
                rightArmRot = Helper.LerpAngle(rightArmRot, 0, 0.3f);
                leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
            }
            else
            {
                rightArmRot = Helper.LerpAngle(rightArmRot, -NPC.direction + MathHelper.ToRadians(-NPC.velocity.Y * .5f), 0.1f);
                leftArmRot = Helper.LerpAngle(leftArmRot, NPC.direction + MathHelper.ToRadians(NPC.velocity.Y * .5f), 0.1f);
            }
        }
        void PickAttack()
        {
            if (AIState != Taunt)
                oldAttack = AIState;
            if (doneAttacksBefore)
            {
                int attempts = 0;
                if (!phase2)
                {
                    Next = Main.rand.Next(Phase1AttackPool);
                    while (attempts++ < 10 && Next == AmethystStorm && oldAttack == Next)
                        Next = Main.rand.Next(Phase1AttackPool);
                }
                else
                {
                    if (phaseMult == 3)
                    {
                        Next = Main.rand.Next(Phase3AttackPool);
                        while (attempts++ < 10 && oldAttack == Next)
                            Next = Main.rand.Next(Phase3AttackPool);
                    }
                    else
                    {
                        Next = Main.rand.Next(Phase2AttackPool);
                        while (attempts++ < 20 && oldAttack == Next)
                            Next = Main.rand.Next(Phase2AttackPool);
                    }
                }
            }
            else
            {
                Next = oldAttack + 1;
                if (Next > (phase2 ? (phaseMult == 3 ? 17 : 15) : 11))
                {
                    doneAttacksBefore = true;
                    PickAttack();
                    return;
                }
            }
            AIState = Idle;
            AITimer = Main.rand.Next(-40, 20);
            if (Main.rand.NextBool((int)(6 + phaseMult * 3)) && oldAttack != Spawn && phaseMult != 3)
            {
                AIState = Taunt;
                AITimer = 0;
            }
            if (phaseMult == 3) Next = BONK;
        }
        bool phase2;
        float phaseMult;
        float oldAttack = Spawn;
        bool doneAttacksBefore;
        int blinkInterval;
        int frameBeforeBlink;
        SlotId helicopterSlot;
        FloatingDialogueBox currentDialogue;
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (ProjectileID.Sets.CultistIsResistantTo[projectile.type])
                modifiers.FinalDamage *= .7f;
        }
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage -= 0.4f;
            modifiers.FinalDamage *= (phaseMult == 3 ? .65f : .83f);
        }
        public override bool CheckDead()
        {
            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<ArchmageDeath>(), 0, 0);
            if (!GetInstance<EbonianSystem>().downedXareus)
            {
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.type == NPCType<ArchmageStaffNPC>())
                        npc.ai[2] = 2400;
                }
                EbonianSystem.xareusFightCooldown = 300;
                GetInstance<EbonianSystem>().downedXareus = true;
            }
            else
            {
                EbonianSystem.xareusFightCooldown = (int)GetInstance<EbonianServerConfig>().XareusDelay;
            }
            if (SoundEngine.TryGetActiveSound(helicopterSlot, out var sound))
            {
                sound.Stop();
            }
            return true;
        }
        public override void AI()
        {
            if (currentDialogue != null)
            {
                currentDialogue.Center = NPC.Center - new Vector2(0, 80);
                currentDialogue.VisibleCenter = NPC.Center - new Vector2(0, 80);
            }
            arenaFlash = MathHelper.Lerp(arenaFlash, 0, 0.05f);
            if (arenaFlash.CloseTo(0, 0.01f))
                arenaFlash = 0;
            if (NPC.life < NPC.lifeMax / 3 + 700)
            {
                if (phaseMult != 3)
                {
                    if (AIState == Idle)
                    {
                        doneAttacksBefore = false;
                        Next = PhantasmalSpirit;
                        NPC.defense += 7;
                        phaseMult = 3;
                    }
                }
            }
            else if (phase2)
                phaseMult = 2;
            else if (NPC.life < NPC.lifeMax * 0.75f)
                phaseMult = 1;
            else
                phaseMult = 0;
            EbonianSystem.xareusFightCooldown = 500;
            blinkInterval++;
            if (blinkInterval >= 170 && blinkInterval < 175)
            {
                if (headFrame.Y != BlinkingFace)
                {
                    frameBeforeBlink = headFrame.Y;
                }
                if (frameBeforeBlink == DisappointedFace || frameBeforeBlink == NeutralFace)
                    headFrame.Y = BlinkingFace;
            }
            if (blinkInterval == 176)
            {
                if (frameBeforeBlink == DisappointedFace || frameBeforeBlink == NeutralFace)
                    headFrame.Y = frameBeforeBlink;
                blinkInterval = Main.rand.Next(-250, 10);
            }
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    if (AIState != Despawn)
                        Reset();
                    AIState = Despawn;
                }
            }
            if (player.GetModPlayer<EbonianPlayer>().timesDiedToXareus <= 0)
            {
                player.GetModPlayer<EbonianPlayer>().timesDiedToXareus = 1;
            }
            if (GetArenaRect().Size().Length() > 100)
            {
                if (Main.LocalPlayer.Distance(GetArenaRect().Center()) > 1200)
                {
                    Helper.TPNoDust(GetArenaRect().Center(), Main.LocalPlayer);
                }
                else
                {
                    while (Main.LocalPlayer.Center.X < GetArenaRect().X)
                        Main.LocalPlayer.Center += Vector2.UnitX * 2;

                    while (Main.LocalPlayer.Center.X > GetArenaRect().X + GetArenaRect().Width)
                        Main.LocalPlayer.Center -= Vector2.UnitX * 2;

                    while (Main.LocalPlayer.Center.Y < GetArenaRect().Y)
                        Main.LocalPlayer.Center += Vector2.UnitY * 2;
                }

                if (NPC.Distance(GetArenaRect().Center()) > 1200)
                {
                    NPC.Center = GetArenaRect().Center();
                }
                else
                {
                    while (NPC.Center.X < GetArenaRect().X)
                        NPC.Center += Vector2.UnitX * 2;

                    while (NPC.Center.X > GetArenaRect().X + GetArenaRect().Width)
                        NPC.Center -= Vector2.UnitX * 2;

                    while (NPC.Center.Y < GetArenaRect().Y)
                        NPC.Center += Vector2.UnitY * 2;
                }
            }


            if (NPC.life < NPC.lifeMax / 2 + 2000)
            {
                if (!phase2 && AIState != Taunt)
                {
                    Next = 1;
                    oldAttack = 0;
                    Reset();
                    NPC.defense += 8;
                    AIState = Phase2Transition; //change to a transition attack later
                    doneAttacksBefore = false;
                    phase2 = true;
                }
            }
            if (NPC.direction != NPC.oldDirection)
                rightArmRot = 0;
            float rightHandOffsetRot = MathHelper.Pi - (NPC.direction == 1 ? MathHelper.PiOver4 : MathHelper.PiOver2);
            float leftHandOffsetRot = MathHelper.Pi - (NPC.direction == 1 ? MathHelper.PiOver2 : MathHelper.PiOver4);
            /*Vector2 staffP = NPC.Center + rightArmRot.ToRotationVector2().RotatedBy(MathHelper.Pi - MathHelper.PiOver4 * 0.8f + (MathHelper.ToRadians((headYOff + 2) * 4) * NPC.direction)) * 6;
            if (NPC.direction == 1)
                staffP = NPC.Center + new Vector2(NPC.width / 2 - 4, -2) + rightArmRot.ToRotationVector2().RotatedBy(MathHelper.Pi + MathHelper.PiOver4 * 0.8f + (MathHelper.ToRadians((headYOff + 2) * 4) * NPC.direction)) * -6;

            float staffRot = rightArmRot + MathHelper.Pi * (NPC.direction == 1 ? .5f : AIState == HelicopterBlades ? .8f : 1f);*/

            Vector2 staffP = NPC.Center + rightArmRot.ToRotationVector2().RotatedBy(MathHelper.Pi - MathHelper.PiOver4 * 0.8f + (MathHelper.ToRadians((headYOff + 2) * 4) * NPC.direction)) * 0.25f;
            if (NPC.direction == 1)
                staffP = NPC.Center + new Vector2(NPC.width / 2 - 4, -10) + (rightArmRot + (MathHelper.ToRadians((headYOff + 2) * 5) * NPC.direction)).ToRotationVector2().RotatedBy(MathHelper.Pi + MathHelper.PiOver4 * 0.8f + (MathHelper.ToRadians((headYOff + 2) * 4) * NPC.direction)) * -.25f;
            float staffRot = rightArmRot + (MathHelper.ToRadians((headYOff + 2) * 5) * NPC.direction) + MathHelper.Pi * (NPC.direction == 1 ? .5f : AIState == HelicopterBlades ? .8f : 1f);

            Vector2 staffTip = staffP + staffRot.ToRotationVector2().RotatedBy(-MathHelper.PiOver4) * 58;

            Lighting.AddLight(staffTip, TorchID.Purple);
            AITimer++;
            headYOff = MathHelper.Lerp(headYOff, MathF.Sin((AITimer + headOffIncrementOffset) * 0.05f) * 2, 0.2f);
            bigStaffBloomAlpha = Lerp(bigStaffBloomAlpha, 0, 0.1f);
            if (bigStaffBloomAlpha.CloseTo(0, 0.05f)) bigStaffBloomAlpha = 0;
            switch (AIState)
            {
                case Despawn:
                    {
                        rightArmRot = Helper.LerpAngle(rightArmRot, -NPC.direction + MathHelper.ToRadians(-65f), 0.1f);
                        leftArmRot = Helper.LerpAngle(leftArmRot, NPC.direction + MathHelper.ToRadians(65f), 0.1f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                        headFrame.Y = (NPC.life < NPC.lifeMax * 0.1f ? AngryFace : SmirkFace);
                        if (AITimer == 40)
                        {
                            WeightedRandom<string> chat = new WeightedRandom<string>();
                            if (!phase2)
                            {
                                chat.Add("Your skill in losing is truly unmatched!");
                                chat.Add("Just another moth to my glorious flame!");
                                chat.Add("Hah! Another victory for the brilliant Archmage!");
                                chat.Add("Would a purple laser pointer ALSO defeat you?");
                                chat.Add("A predictable failure, THIEF!");
                                if (NPC.life > NPC.lifeMax - 2000)
                                {
                                    chat.Add("Wait, you're actually THAT incompetent?! I pity you.");
                                    chat.Add("*Yawn*");
                                }
                            }
                            else
                            {
                                if (phaseMult == 3)
                                    chat.Add("GOOD GODDAMN RIDDANCE!");
                                else
                                    chat.Add("Phew... I mean, not even close!");
                            }
                            currentDialogue = DialogueSystem.NewDialogueBox(160, NPC.Center - new Vector2(0, 80), chat, Color.Violet, -1, 0.6f, Color.Indigo * 0.6f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        }
                        if (AITimer >= 100)
                        {
                            for (int i = 0; i < 15; i++)
                                Projectile.NewProjectile(null, NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(10, 20), ProjectileType<XAnimeSlash>(), 0, 0, -1, 0, Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(0.1f, 0.3f));

                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0);
                            NPC.active = false;
                        }
                    }
                    break;
                case Spawn:
                    {
                        FacePlayer();
                        if (AITimer == 1)
                        {
                            NPC.dontTakeDamage = true;
                            if (EbonianSystem.heardXareusIntroMonologue || GetInstance<EbonianSystem>().downedXareus)
                                AITimer = 339;
                        }
                        if (AITimer < 50 || AITimer > 350)
                        {
                            rightArmRot = Helper.LerpAngle(rightArmRot, -NPC.direction + MathHelper.ToRadians(-20f) * NPC.direction, 0.15f);
                            leftArmRot = Helper.LerpAngle(leftArmRot, NPC.direction + MathHelper.ToRadians(20f) * NPC.direction, 0.15f);
                        }
                        else IdleAnimation();

                        if (AITimer == 1)
                        {
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<XExplosionTiny>(), 0, 0);
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0);
                            //SoundEngine.PlaySound(SoundID.Item16, NPC.Center);
                        }
                        if (AITimer == 50)
                        {
                            headFrame.Y = AngryFace;
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "GET YOUR HANDS OFF, YYYOOOOUUUU!!!!!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        }
                        if (AITimer == 150)
                        {
                            headFrame.Y = SadFace;
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "my staff... my BELOVED STAFF, SO PRISTINE UNTIL NOW!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        }
                        if (AITimer == 350)
                        {
                            Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/xareus");
                            headFrame.Y = AngryFace;
                            if (!EbonianSystem.heardXareusIntroMonologue)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "YOU FIEND!!! I'LL DESTROY YOU IN EVERY WAY PHYSICALLY POSSIBLE!!!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                            else
                            {
                                WeightedRandom<string> chat = new WeightedRandom<string>();
                                chat.Add("Would you PLEASE stop that!?");
                                chat.Add("You again?!");
                                chat.Add("STOP TOUCHING THE STAFF!");
                                if (GetInstance<EbonianSystem>().downedXareus)
                                {
                                    chat.Add("The first time was just luck!");
                                    chat.Add("LEAVE ME ALONE!");
                                }
                                else
                                {
                                    if (!Main.dayTime)
                                    {
                                        chat.Add("Dude, please, it's like midnight, I'm so tired of winning...");
                                    }
                                    else
                                    {
                                        chat.Add("I thought the first time taught you a proper lesson!");
                                    }
                                }
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), chat, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                            }
                        }
                        if (AITimer >= 500)
                        {
                            NPC.dontTakeDamage = false;
                            EbonianSystem.heardXareusIntroMonologue = true;
                            Reset();
                            PickAttack();
                        }
                    }
                    break;
                case Phase2Transition:
                    {
                        if (AITimer == 1)
                        {
                            disposablePos[9] = GetArenaRect().Center();
                        }
                        if (AITimer < 340 || AITimer > 551)
                        {
                            if (AITimer == 1 || AITimer > 300)
                                FacePlayer();
                            NPC.dontTakeDamage = true;
                            if (AITimer < 340)
                                headFrame.Y = AngryFace;
                            else if (AITimer < 1100)
                                headFrame.Y = DisappointedFace;
                            else
                                headFrame.Y = BlinkingFace;

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            NPC.velocity.X = Helper.FromAToB(NPC.Center, disposablePos[9], false).X * .03f;
                            rightArmRot = Helper.LerpAngle(rightArmRot, -NPC.direction + MathHelper.ToRadians(-30f) * NPC.direction, 0.15f);
                            leftArmRot = Helper.LerpAngle(leftArmRot, NPC.direction + MathHelper.ToRadians(30f) * NPC.direction, 0.15f);
                        }
                        else
                        {
                            NPC.velocity *= 0.5f;
                            headFrame.Y = DisappointedFace;
                            if (AITimer >= 400 && AITimer < 551)
                            {
                                FacePlayer();
                                rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, player.Center, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                            }
                            else
                                IdleAnimation();
                        }
                        if (AITimer == 40)
                        {
                            Projectile.NewProjectile(null, staffTip, Vector2.Zero, ProjectileType<XExplosionInvis>(), 0, 0);
                            currentDialogue = DialogueSystem.NewDialogueBox(160, NPC.Center - new Vector2(0, 80), "None of this would've happened if you just kept your grubby hands away from my things!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        }
                        if (AITimer == 100)
                        {
                            //arenaFlash = 1;
                            float off = Main.rand.NextFloat(MathHelper.TwoPi);
                            for (int i = 0; i < 4; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 4) + off;
                                Vector2 vel = angle.ToRotationVector2();
                                Projectile.NewProjectile(null, NPC.Center - new Vector2(0, 130), vel, ProjectileType<XRift>(), 15, 0);
                            }
                        }
                        if (AITimer == 150)
                        {
                            foreach (Player _player in Main.ActivePlayers)
                            {
                                _player.AddBuff(BuffType<Sheepened>(), 1150);
                                for (float i = 0; i < 1; i += 0.1f)
                                {
                                    Projectile.NewProjectile(null, Vector2.Lerp(NPC.Center - new Vector2(0, 130), _player.Center, i), Vector2.Zero, ProjectileType<XExplosionTiny>(), 0, 0);
                                }
                                Projectile.NewProjectile(null, _player.Center, Vector2.Zero, ProjectileType<XExplosionInvis>(), 0, 0);
                            }
                        }
                        if (AITimer == 300)
                        {
                            SoundEngine.PlaySound(EbonianSounds.xSpirit, NPC.Center);
                            for (int i = 0; i < 5; i++)
                            {
                                if (i == 2) continue;
                                Vector2 _pos = GetArenaRect().Left();
                                Vector2 __pos = GetArenaRect().Right();
                                Vector2 pos = Vector2.Lerp(_pos + new Vector2(40, 0), __pos - new Vector2(16, 0), (float)i / 4);
                                Projectile.NewProjectileDirect(null, Helper.TRay.Cast(pos, Vector2.UnitY, GetArenaRect().Height + 10), -Vector2.UnitY, ProjectileType<XRift>(), 15, 0);
                            }
                        }
                        if (AITimer >= 480 && AITimer < 541 && AITimer % 20 == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item73.WithPitchOffset(-0.2f), staffTip);
                            Vector2 vel = Helper.FromAToB(staffTip, player.Center).RotatedByRandom(MathHelper.PiOver4 * 0.2f);
                            NPC.damage = 30;
                            Projectile.NewProjectile(null, staffTip + vel * 15f, vel * 7f, ProjectileType<XBolt>(), 15, 0);
                        }
                        else NPC.damage = 0;
                        if (AITimer == 570)
                        {
                            SoundEngine.PlaySound(EbonianSounds.xSpirit, NPC.Center);
                            for (int i = 1; i < 3; i++)
                            {
                                Vector2 pos = Main.rand.NextVector2FromRectangle(OffsetBoundaries(Vector2.One * 200));
                                disposablePos[i] = pos;
                                Projectile.NewProjectile(null, pos, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0);
                                Projectile.NewProjectile(null, pos, Vector2.Zero, ProjectileType<XExplosionTiny>(), 0, 0);
                            }

                        }
                        if (AITimer == 600)
                        {
                            SoundEngine.PlaySound(EbonianSounds.xSpirit, NPC.Center);
                            Projectile.NewProjectile(null, disposablePos[1], Vector2.Zero, ProjectileType<XExplosionInvis>(), 0, 0);
                            float off = Main.rand.NextFloat(MathHelper.TwoPi);
                            for (int j = 0; j < 4; j++)
                            {
                                float angle = Helper.CircleDividedEqually(j, 4) + off;
                                Projectile.NewProjectile(null, disposablePos[1], angle.ToRotationVector2() * (1 + j * 0.5f), ProjectileType<XBolt>(), 20, 0);
                            }
                        }
                        if (AITimer == 630)
                        {
                            SoundEngine.PlaySound(EbonianSounds.xSpirit, NPC.Center);
                            Projectile.NewProjectile(null, disposablePos[2], Vector2.Zero, ProjectileType<XExplosionInvis>(), 0, 0);
                            float off = Main.rand.NextFloat(MathHelper.TwoPi);
                            for (int j = 0; j < 8; j++)
                            {
                                float angle = Helper.CircleDividedEqually(j, 8) + off;
                                Projectile.NewProjectile(null, disposablePos[2], angle.ToRotationVector2() * (3 + j * 0.5f), ProjectileType<XBolt>(), 20, 0);
                            }
                        }
                        if (AITimer == 660)
                        {
                            Projectile.NewProjectile(null, GetArenaRect().Center() + new Vector2(250, -100), Vector2.Zero, ProjectileType<XCloud>(), 0, 0);
                            Projectile.NewProjectile(null, GetArenaRect().Center() + new Vector2(-250, -100), Vector2.Zero, ProjectileType<XCloud>(), 0, 0);
                        }
                        if (AITimer == 1030)
                        {
                            Projectile.NewProjectile(null, GetArenaRect().Center() + new Vector2(-250, -100), Vector2.Zero, ProjectileType<SheepeningOrb>(), 1, 0);
                            Projectile.NewProjectile(null, GetArenaRect().Center() + new Vector2(250, -100), Vector2.Zero, ProjectileType<SheepeningOrb>(), 1, 0);
                        }
                        if (AITimer == 1110)
                            currentDialogue = DialogueSystem.NewDialogueBox(160, NPC.Center - new Vector2(0, 80), "Phew... Almost lost my cool there..", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 2.3f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        if (AITimer >= 1260)
                        {
                            Reset();
                            NPC.dontTakeDamage = false;
                            oldAttack = 0;
                            Next = 1;
                            AIState = 1;
                        }
                    }
                    break;
                case Taunt:
                    {
                        if (AITimer < 80 + (NPC.life * 0.01f))
                        {
                            headFrame.Y = phase2 ? (NPC.life < NPC.lifeMax * 0.1f ? AngryFace : SmirkFace) : LightSmirkFace;
                            rightArmRot = Helper.LerpAngle(rightArmRot, -NPC.direction + MathHelper.ToRadians(-20f) * NPC.direction, 0.15f);
                            leftArmRot = Helper.LerpAngle(leftArmRot, NPC.direction + MathHelper.ToRadians(20f) * NPC.direction, 0.15f);
                        }
                        else
                        {
                            headFrame.Y = phase2 ? DisappointedFace : NeutralFace;
                            IdleAnimation();
                        }
                        FacePlayer();
                        AITimer2--;

                        NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                        if (AITimer == 40)
                        {
                            WeightedRandom<string> chat = new WeightedRandom<string>();
                            chat.Add("If I were you, I would probably try to steal my staff too!");
                            chat.Add("This is nothing but a slight inconvenience for the Grand Archmage!!");
                            chat.Add("If only you were as good at fighting as you were at opening doors!");
                            chat.Add("You're practically defeating YOURSELF!");
                            chat.Add("Are you intentionally missing? Or is my glory blinding you?");
                            chat.Add("You're unworthy of even witnessing my greatest magic!");
                            chat.Add("My greatest magics would break your mind BEFORE your body!");
                            chat.Add("I bet you can't even begin to comprehend the might of The Archmage!");
                            chat.Add("Your greatest skill is your ability to make bad decisions!");
                            chat.Add("If I were you, I'd bow and let me destroy you!");
                            chat.Add("I can't believe I'm wasting all my glory on this...");
                            chat.Add("Even if you stole it, you would never be worthy enough to wield my Magnificent Staff!");
                            chat.Add("I'm not sure whether your futile attempts to hurt me are depressing or amusing!");
                            chat.Add("I've yet to meet someone that can outsmart spells!");
                            chat.Add("You're not putting a scratch on me!");
                            chat.Add("Feel the power of my MAAAGIIIC!!!");
                            chat.Add("You truly are an amateur among amateurs!");
                            chat.Add("There is no mercy for thieves like you!");
                            chat.Add("If you surrender now, I still won't consider letting you go!");
                            chat.Add("There is no escape, weakling!");
                            chat.Add("My power is MAXIMUM!!!!! *cough*");
                            chat.Add("Unbelievable! I almost envy your persistence!");
                            chat.Add("Unlike you, I am destined for supremacy!");
                            chat.Add("My spells are my power!");
                            chat.Add("DIE!!! HUMAN!! ENEMY!!!!!! *cough, cough*");
                            if (NPC.HasBuff(BuffID.OnFire) || NPC.HasBuff(BuffID.OnFire3))
                            {
                                if (phase2)
                                    chat.Add("QUIT IT WITH THE FIRE! THIS ROBE IS NEW!", 1.4f);
                                else
                                {
                                    chat.Add("Fool! I actually ENJOY burning!", 1.4f);
                                    chat.Add("You think some fire is enough to take down the GRAND ARCHMAGE!?", 1.4f);
                                }
                            }
                            if (phase2)
                                chat.Add("Hey! QUIT IT! You're scratching up my robe!");
                            if (player.HeldItem != null)
                            {
                                if (player.HeldItem.DamageType == DamageClass.Magic)
                                {
                                    chat.Add("I remember a RAT that was better with magic!", 1.5);
                                    chat.Add("Even a bunny outmatches you in magic prowess!", 1.5);
                                    chat.Add("Maybe hone your spells before facing the Grand Archmage!", 1.5);
                                    chat.Add("You call THAT magic?! How pitiful!", 1.5);
                                    if (Item.staff[player.HeldItem.type])
                                    {
                                        chat.Add("My staff is bigger than yours!", 1.5);
                                        chat.Add("You'd be better off using that staff to club me!", 1.5);
                                    }
                                    chat.Add("For a sorcerer, your power disappoints me...", 1.5);
                                }
                                if (player.HeldItem.DamageType == DamageClass.Melee)
                                {
                                    chat.Add("A brute like you is unworthy to face someone like me!", 1.5);
                                    chat.Add("Your strikes are but empty threats to one such as myself!", 1.5);
                                    chat.Add("A measly blade is all you'll ever be good for!", 1.5);
                                    if (phase2)
                                        chat.Add("Oh, WHAT?! Is the sword too heavy for your flabby noodle arms?!?", 1.5);
                                }
                                if (player.HeldItem.DamageType == DamageClass.Ranged)
                                {
                                    chat.Add("Stay true to your aim, then maybe you'll stand a chance!", 1.5);
                                    chat.Add("Your firepower is nothing compared to the might of my magic!", 1.5);
                                    chat.Add("Your aim is a disgrace to a disgrace!", 1.5);
                                    chat.Add("Are you TRYING to hit the wall? Or just blinded by my glory?", 1.5);
                                    if (player.HeldItem.useAmmo == AmmoID.Bullet)
                                        chat.Add("You fool! I am bulletproof!", 1.5);
                                    if (player.HeldItem.useAmmo == AmmoID.Arrow)
                                        chat.Add("I would make a better archer than you!", 1.5);
                                    if (player.HeldItem.useAmmo == AmmoID.Rocket && player.ChooseAmmo(player.HeldItem).type == ItemID.MiniNukeI)
                                        chat.Add("WHAT THE FUCK?!", 3);
                                    if (phase2)
                                        chat.Add("STOP GOING FOR CHEAP SHOTS, YOU LITTLE BANDIT!", 1.5);
                                }
                                if (player.HeldItem.DamageType == DamageClass.SummonMeleeSpeed)
                                    chat.Add("How about I whip YOU into shape?!", 1.5);
                            }
                            if (player.numMinions > 0)
                            {
                                chat.Add("Coward! Strength in numbers is futile against ME!!!", 1.5);
                                chat.Add("You dare face me with your sinister spawns?!", 1.5);
                                chat.Add("You attack me with MINIONS? AT LEAST FACE ME YOURSELF!", 1.5);
                                chat.Add("How actually DARE you! WOULD YOU LIKE IT IF I SWARMED YOU WITH MINIONS?!", 1.5);
                                if (player.ownedProjectileCounts[ProjectileID.BabySlime] > 1)
                                    chat.Add("Hold up, slimes?! You're THAT ill-prepared?!", 1.5);
                            }
                            currentDialogue = DialogueSystem.NewDialogueBox(120, NPC.Center - new Vector2(0, 80), chat, Color.Violet, -1, 0.6f, Color.Indigo * 0.6f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        }
                        if (AITimer >= 80 + (NPC.life * 0.01f))
                        {
                            Reset();
                            AIState = Next;
                        }
                    }
                    break;
                case Idle:
                    {
                        IdleAnimation();
                        //
                        if (MeleeAttacks.Contains(Next))
                        {
                            FacePlayer();

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.Grounded())
                                NPC.velocity.Y = -9.5f;
                            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center + Helper.FromAToB(player.Center, NPC.Center) * 150, false).X * (phaseMult == 3 ? 0.09f : .02f), (phaseMult == 3 ? 0.15f : 0.1f));
                        }
                        else
                        {
                            if (AITimer == 1) disposablePos[9] = Main.rand.NextVector2FromRectangle(GetArenaRect());
                            Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                            if (disposablePos[9].X < NPC.Center.X)
                                pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);
                            if (NPC.Distance(pos) < 70)
                                AITimer++;

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.Grounded())
                                NPC.velocity.Y = -9.5f;
                            NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * (phaseMult == 3 ? 0.07f : .01f);
                            FacePlayer();
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                        }
                        AITimer2--;

                        NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);

                        if (phaseMult == 3)
                            headFrame.Y = AngryFace;
                        if (AITimer >= 35 + (NPC.life * 0.0035f))
                        {
                            Reset();
                            AIState = Next;
                        }
                    }
                    break;
                case PhantasmalSpirit:
                    {
                        FacePlayer();
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 40)
                        {
                            if (phaseMult == 3 && !doneAttacksBefore)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "Do you ever quit?!... PHANTASMAL SPIRITS!!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                            else if (oldAttack != AIState)
                            {
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? "PHANTASMAL SPIRITS!!!!!" : "Phantasmal Spirits!"), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                            }
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? "AGAIN!" : "And again!"), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        }



                        if (AITimer < 135) rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, player.Center, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                        else rightArmRot = Helper.LerpAngle(rightArmRot, 0, 0.2f);

                        if ((AITimer < 65 && AITimer > 50) || (AITimer < 125 && AITimer > 100))
                        {
                            headFrame.Y = ShockedFace;
                            Vector2 pos = staffTip + Helper.FromAToB(staffTip, player.Center).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(10, 50);
                            Dust.NewDustPerfect(pos, DustType<SparkleDust>(), Helper.FromAToB(staffTip, pos) * Main.rand.NextFloat(3, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                        }
                        NPC.velocity.X += -NPC.direction * .05f;
                        NPC.velocity.X *= 0.95f;
                        if (phase2 ? (AITimer >= 75 && AITimer % 20 == 0 && (phaseMult == 3 ? true : AITimer != 105)) : (AITimer == 75 || AITimer == 95 || AITimer == 135 || (phaseMult == 1 ? AITimer == 160 : false)))
                        {
                            NPC.velocity.X += -NPC.direction * 5;
                            if (phaseMult == 3)
                                headFrame.Y = AngryFace;
                            else
                                headFrame.Y = phase2 ? DisappointedFace : NeutralFace;
                            SoundEngine.PlaySound(EbonianSounds.xSpirit, staffTip);
                            NPC.damage = 30;
                            EbonianSystem.ScreenShakeAmount = 6;
                            for (int i = 0; i < 20; i++)
                            {
                                Vector2 pos = NPC.Center + Helper.FromAToB(staffTip, player.Center).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(10, 50);
                                if (i % 2 == 0)
                                    Dust.NewDustPerfect(pos, DustType<SparkleDust>(), Helper.FromAToB(staffTip, pos) * Main.rand.NextFloat(10, 15), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                                else
                                    Dust.NewDustPerfect(pos, DustType<LineDustFollowPoint>(), Helper.FromAToB(staffTip, pos) * Main.rand.NextFloat(10, 15), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                            }
                            if (!phase2)
                                Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(staffTip, player.Center).RotatedByRandom(MathHelper.PiOver4) * (phaseMult == 3 ? 4 : 5f), ProjectileType<XSpirit>(), 15, 0);
                            else
                                Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(staffTip, player.Center).RotatedByRandom(MathHelper.PiOver4 * 0.25f) * 6f, ProjectileType<XSpirit>(), 15, 0);

                        }
                        else
                            NPC.damage = 0;

                        if (phaseMult == 3 && AITimer % 40 == 0)
                        {
                            for (int i = -1; i < 2; i++)
                            {
                                if (i == 0) continue;
                                Projectile.NewProjectile(null, staffTip, Helper.FromAToB(staffTip, player.Center).RotatedBy(i * 0.3f), ProjectileType<XBolt>(), 15, 0);
                            }
                        }
                        if (AITimer >= 150)
                        {
                            Reset();
                            PickAttack();
                        }
                    }
                    break;
                case ShadowflamePuddles:
                    {
                        if (phaseMult == 3)
                            headFrame.Y = AngryFace;

                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 1)
                            AITimer2 = Main.rand.Next(4);
                        if (AITimer == 40)
                            if (oldAttack != AIState)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? "BURN!!" : "Shadowflame Eruption!"), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? "BURN, I SAID!" : "And again!"), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        if (AITimer < 180 && AITimer > 1)
                        {
                            rightArmRot = Helper.LerpAngle(rightArmRot, -Vector2.UnitY.ToRotation() - (NPC.direction == -1 ? (MathHelper.PiOver2 + MathHelper.PiOver4) : 0), 0.2f);
                        }
                        else
                        {
                            FacePlayer();
                            rightArmRot = Helper.LerpAngle(rightArmRot, 0, 0.2f);
                        }


                        if (AITimer2 > 0 && phaseMult != 3)
                        {
                            if (AITimer >= 60 && AITimer <= 180 && AITimer % (phase2 ? (phaseMult == 3 ? 10 : 15) : (phaseMult == 1 ? 20 : 25)) == 0)
                            {
                                for (int i = 0; i < 5; i++)
                                    Dust.NewDustPerfect(staffTip, DustType<XGoopDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.White, Main.rand.NextFloat(.1f, .6f));
                                Vector2 pos = new Vector2(GetArenaRect().X + GetArenaRect().Width * Main.rand.NextFloat(), NPC.Center.Y);
                                Projectile.NewProjectile(null, pos, AITimer % (phase2 ? (phaseMult == 3 ? 20 : 30) : 50) == 0 ? Vector2.UnitY : -Vector2.UnitY, ProjectileType<XShadowflame>(), 15, 0);
                            }
                        }
                        else//phase 2
                        {
                            if (AITimer == 100)
                            {
                                for (int i = 0; i < 5; i++)
                                    Dust.NewDustPerfect(staffTip, DustType<XGoopDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.White, Main.rand.NextFloat(.1f, .6f));
                                for (int i = 0; i < 7; i++)
                                {
                                    Vector2 _pos = GetArenaRect().Left();
                                    Vector2 __pos = GetArenaRect().Right();
                                    Vector2 pos = Vector2.Lerp(_pos + new Vector2(40, 0), __pos - new Vector2(16, 0), (float)i / 6);
                                    if (Helper.TRay.CastLength(pos, -Vector2.UnitY, GetArenaRect().Height + 10) <= GetArenaRect().Height)
                                    {
                                        Projectile p1 = Projectile.NewProjectileDirect(null, pos + (i > 1 && i < 6 && phaseMult == 3 ? Main.rand.NextFloat(-50, 50) * Vector2.UnitX : Vector2.Zero), Vector2.UnitY, ProjectileType<XShadowflame>(), 15, 0);
                                        p1.timeLeft = 868;
                                        p1.ai[1] = 2;
                                    }
                                    Projectile p2 = Projectile.NewProjectileDirect(null, pos + (i > 1 && i < 6 && phaseMult == 3 ? Main.rand.NextFloat(-49, 49) * Vector2.UnitX : Vector2.Zero), -Vector2.UnitY, ProjectileType<XShadowflame>(), 15, 0);
                                    if (i > 0)
                                    {
                                        p2.timeLeft = 868;
                                        p2.ai[1] = 2;
                                    }
                                }
                            }
                            if (AITimer == 200 && phaseMult == 3)
                            {
                                for (int i = 0; i < 5; i++)
                                    Dust.NewDustPerfect(staffTip, DustType<XGoopDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.White, Main.rand.NextFloat(.1f, .6f));
                                for (int i = 0; i < 7; i++)
                                {
                                    Vector2 _pos = GetArenaRect().Left();
                                    Vector2 __pos = GetArenaRect().Right();
                                    Vector2 pos = Vector2.Lerp(_pos + new Vector2(40, 0), __pos - new Vector2(16, 0), (float)i / 6);
                                    if (Helper.TRay.CastLength(pos, -Vector2.UnitY, GetArenaRect().Height + 10) <= GetArenaRect().Height)
                                    {
                                        Projectile p1 = Projectile.NewProjectileDirect(null, pos + (i > 1 && i < 6 && phaseMult == 3 ? Main.rand.NextFloat(-50, 50) * Vector2.UnitX : Vector2.Zero), Vector2.UnitY, ProjectileType<XShadowflame>(), 15, 0);
                                        p1.timeLeft = 868;
                                        p1.ai[1] = 2;
                                    }
                                    Projectile p2 = Projectile.NewProjectileDirect(null, pos + (i > 1 && i < 6 && phaseMult == 3 ? Main.rand.NextFloat(-49, 49) * Vector2.UnitX : Vector2.Zero), -Vector2.UnitY, ProjectileType<XShadowflame>(), 15, 0);
                                    if (i > 0)
                                    {
                                        p2.timeLeft = 868;
                                        p2.ai[1] = 2;
                                    }
                                }
                            }
                        }
                        if (AITimer >= (phaseMult == 3 ? 160 : 260))
                        {
                            Reset();
                            PickAttack();
                        }
                    }
                    break;
                case SpectralOrbs:
                    {
                        FacePlayer();
                        if (phaseMult == 3)
                            headFrame.Y = AngryFace;
                        else
                            headFrame.Y = phase2 ? SmirkFace : LightSmirkFace;
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f); rightArmRot = Helper.LerpAngle(rightArmRot, -Vector2.UnitY.ToRotation() - (NPC.direction == -1 ? (MathHelper.PiOver2 + MathHelper.PiOver4) : 0), 0.2f);
                        if (AITimer == 30)
                            if (oldAttack != AIState && oldAttack != AmethystBulletHell)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? "RAAAAGHH!" : "Glittering Amethysts!"), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);

                        if (AITimer == 50)
                        {
                            SoundEngine.PlaySound(EbonianSounds.xSpirit, NPC.Center);
                            float off = Main.rand.NextFloat(MathHelper.Pi * 2);
                            for (int i = 0; i < 3 + (AITimer / 60); i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 3 + (AITimer / 60)) + off;
                                Projectile.NewProjectileDirect(null, staffTip, Vector2.UnitX.RotatedBy(angle), ProjectileType<XAmethyst>(), 15, 0).timeLeft = 100;
                            }
                        }
                        if (AITimer >= 70 && AITimer <= (Main.expertMode ? 140 : 110) && AITimer % (phase2 ? (phaseMult == 3 ? 15 : 20) : (phaseMult == 1 ? 25 : 30)) == 0)
                        {
                            SoundEngine.PlaySound(EbonianSounds.xSpirit, NPC.Center);
                            float off = Main.rand.NextFloat(MathHelper.Pi * 2);
                            for (int i = 0; i < 3 + (AITimer / 60); i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 3 + (AITimer / 60)) + off;
                                Projectile.NewProjectileDirect(null, staffTip, Vector2.UnitX.RotatedBy(angle), ProjectileType<XAmethyst>(), 15, 0).timeLeft = 100;
                            }
                        }

                        if (AITimer >= 150)
                        {
                            Reset();
                            PickAttack();
                        }
                    }
                    break;
                case MagnificentFireballs:
                    {
                        if (phaseMult == 3)
                            headFrame.Y = AngryFace;
                        if (AITimer == 1)
                        {
                            disposablePos[9] = player.Center;
                            AITimer = Main.rand.Next(2, 20);
                        }
                        if (AITimer < 50)
                        {

                            Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                            if (disposablePos[9].X < NPC.Center.X)
                                pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.Grounded())
                                NPC.velocity.Y = -9.5f;
                            NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                            NPC.direction = disposablePos[9].X > NPC.Center.X ? -1 : 1;
                            NPC.spriteDirection = NPC.direction;
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                        }
                        else
                            NPC.velocity.X *= 0.9f;
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 30)
                            if (oldAttack != AIState)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), phaseMult == 3 ? (doneAttacksBefore ? "JUST DIE!!" : "SHIMMERING.. WHATEVER JUST DIE!") : "Shimmering Fusillade!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? "AGAIN!" : "And again!"), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        if (AITimer == 80)
                        {
                            Projectile.NewProjectile(null, staffTip, Vector2.Zero, ProjectileType<XExplosionInvis>(), 0, 0);
                        }
                        if (AITimer > 55)
                        {
                            FacePlayer();
                            rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, player.Center, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                        }
                        if (AITimer == 150 || AITimer == 220)
                        {
                            Vector2 pos = NPC.Center;
                            int attempts = 0;
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<XExplosionInvis>(), 0, 0);
                            while (++attempts < 100 && (pos.Distance(NPC.Center) < 250 || pos.Distance(player.Center) < 350 || !Collision.CanHit(NPC.position, NPC.width, NPC.height, pos - NPC.Size / 2, NPC.width, NPC.height)))
                            {
                                Rectangle rect = GetArenaRect();
                                rect.Width -= 120;
                                rect.X += 60;
                                rect.Height -= 120;
                                rect.Y += 60;
                                pos = Main.rand.NextVector2FromRectangle(rect);
                            }
                            pos = Helper.TRay.Cast(pos, Vector2.UnitY, 1000, true) - new Vector2(0, NPC.height / 2 + 16);
                            Projectile.NewProjectile(null, pos, Vector2.Zero, ProjectileType<XExplosionInvis>(), 0, 0);
                            NPC.Center = pos;
                        }
                        if ((AITimer <= 120 && AITimer >= 110) || AITimer == 160 || (AITimer <= 190 && AITimer >= 170) || (AITimer <= 250 && AITimer >= 240))
                        {
                            if (AITimer % (AITimer > 229 ? (phase2 ? (phaseMult == 3 ? 4 : 5) : (phaseMult == 1 ? 5 : 10)) : (phase2 ? (phaseMult == 3 ? 4 : 6) : 10)) == 0)
                            {
                                for (int i = 0; i < 5; i++)
                                    Dust.NewDustPerfect(staffTip, DustType<XGoopDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.White, Main.rand.NextFloat(.1f, .6f));
                                SoundEngine.PlaySound(SoundID.Item73.WithPitchOffset(-0.2f), staffTip);
                                Vector2 vel = Helper.FromAToB(staffTip, player.Center).RotatedByRandom(MathHelper.PiOver4 * MathHelper.Lerp(1.25f + phaseMult * 0.05f, 0.65f, AITimer / 300) * (AITimer == 110 ? 0 : (AITimer > 229 ? 1.2f : 0.65f)));
                                Projectile.NewProjectile(null, staffTip, vel * 7f, ProjectileType<XBolt>(), 15, 0);
                            }

                            NPC.damage = 30;

                        }
                        else
                            NPC.damage = 0;

                        if (phaseMult == 3 && AITimer == 260)
                        {
                            disposablePos[0] = NPC.Center;
                            disposablePos[1] = player.Center + player.velocity;
                            for (int i = -2; i < 3; i++)
                            {
                                disposablePos[i + 6] = Helper.FromAToB(disposablePos[0], disposablePos[1]).RotatedByRandom(MathF.Abs(i) * 0.75f);
                                Projectile.NewProjectileDirect(null, disposablePos[0], disposablePos[i + 6], ProjectileType<XSineLaser>(), 0, 0);
                            }
                        }

                        if (phaseMult == 3 && AITimer == 298)
                        {
                            for (int i = -2; i < 3; i++)
                            {
                                Projectile.NewProjectileDirect(null, disposablePos[0], disposablePos[i + 6], ProjectileType<XSineLaser>(), 15, 0);
                            }
                        }

                        if (AITimer >= 300)
                        {
                            Reset();
                            PickAttack();
                        }
                    }
                    break;
                case SineLaser:
                    {
                        if (phaseMult == 3)
                            headFrame.Y = AngryFace;
                        else
                            headFrame.Y = SmirkFace;
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 30)
                            if (oldAttack != AIState)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), phaseMult == 3 ? "SHADOWFLAME!!" : (phase2 ? "Helix" : "Wave") + " of Shadowflame!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? "AGAIN!" : "And again!"), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        if (AITimer < 50)
                        {
                            disposablePos[0] = player.Center;
                            FacePlayer();
                        }
                        rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, disposablePos[0], reverse: true).ToRotation() + rightHandOffsetRot + (AITimer >= 145 ? MathF.Sin(AITimer * 6) * 0.1f : 0), 0.2f);
                        Vector2 vel = Helper.FromAToB(NPC.Center, disposablePos[0]);
                        if (AITimer == 53)
                        {
                            SoundEngine.PlaySound(EbonianSounds.cursedToyCharge, NPC.Center);

                            for (int i = 0; i < 5; i++)
                                Dust.NewDustPerfect(staffTip, DustType<XGoopDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.White, Main.rand.NextFloat(.1f, .6f));

                            Projectile.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 0, 0, ai1: (phaseMult == 3 ? 55 : 70));
                            if (phase2)
                                Projectile.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 0, 0, ai1: -(phaseMult == 3 ? 55 : 70));
                            if (phaseMult == 3)
                                Projectile.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 0, 0);
                        }
                        if (AITimer == 92)
                        {
                            SoundEngine.PlaySound(EbonianSounds.exolDash, NPC.Center);

                            Projectile.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 15, 0, ai1: (phaseMult == 3 ? 55 : 70));
                            if (phase2)
                                Projectile.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 15, 0, ai1: -(phaseMult == 3 ? 55 : 70));
                            if (phaseMult == 3)
                                Projectile.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 15, 0);
                        }
                        if (AITimer > 110 && AITimer < 130)
                            disposablePos[0] = player.Center;
                        if (AITimer == 135 && phaseMult == 3)
                        {
                            FacePlayer();
                            SoundEngine.PlaySound(EbonianSounds.cursedToyCharge, NPC.Center);

                            for (int i = 0; i < 5; i++)
                                Dust.NewDustPerfect(staffTip, DustType<XGoopDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.White, Main.rand.NextFloat(.1f, .6f));


                            Projectile.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 0, 0, ai1: 70);
                            Projectile.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 0, 0, ai1: -70);
                            Projectile.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 0, 0);
                        }
                        if (AITimer == 172 && phaseMult == 3)
                        {
                            SoundEngine.PlaySound(EbonianSounds.exolDash, NPC.Center);

                            Projectile.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 15, 0, ai1: 70);
                            Projectile.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 15, 0, ai1: -70);
                            Projectile.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 15, 0);
                        }
                        if (AITimer >= 130 + (phaseMult == 3 ? 80 : 0))
                        {
                            Reset();
                            PickAttack();
                        }
                    }
                    break;
                case AmethystCloseIn:
                    {
                        if (phaseMult == 3)
                            headFrame.Y = AngryFace;
                        else
                            headFrame.Y = SmirkFace;
                        rightArmRot = Helper.LerpAngle(rightArmRot, MathHelper.ToRadians(AITimer * 8f), 0.15f);
                        NPC.direction = NPC.spriteDirection = ((NPC.Center + rightArmRot.ToRotationVector2().RotatedBy(MathHelper.PiOver4 * 0.5f) * 20).X > NPC.Center.X ? -1 : 1);
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 40)
                            if (oldAttack != AIState)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), phaseMult == 3 ? "THIS IS VERY EXPENSIVE!" : "Amethysts of Empowerment!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? "VERYY EXPENSIVE!" : "And again!"), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);

                        if (AITimer % (phase2 ? (phaseMult == 3 ? 10 : 15) : (phaseMult == 1 ? 20 : 25)) == 0 && AITimer > 29 && AITimer < 91)
                        {
                            float off = (phase2 ? AITimer * 0.18f : Main.rand.NextFloat(MathHelper.TwoPi));
                            for (int i = 0; i < 5; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 5) + off;
                                Vector2 pos = NPC.Center + new Vector2(phase2 ? 225 * phaseMult : 300, 0).RotatedBy(angle);
                                Projectile p = Projectile.NewProjectileDirect(null, pos, Helper.FromAToB(pos, NPC.Center) * 0.1f, ProjectileType<XAmethystCloseIn>(), 15, 0);
                                p.ai[0] = NPC.Center.X;
                                p.ai[1] = NPC.Center.Y;
                            }
                            Projectile.NewProjectileDirect(null, staffTip, Vector2.Zero, ProjectileType<XExplosionTiny>(), 0, 0);
                        }

                        if (AITimer >= 100)
                        {
                            Reset();
                            PickAttack();
                        }
                    }
                    break;
                case AmethystBulletHell: //phase 2
                    {
                        if (phaseMult == 3)
                            headFrame.Y = AngryFace;
                        else
                            headFrame.Y = phase2 ? SmirkFace : LightSmirkFace;
                        rightArmRot = Helper.LerpAngle(rightArmRot, -NPC.direction + MathHelper.ToRadians(-45f) * NPC.direction, 0.15f);
                        if (AITimer == 1) disposablePos[9] = player.Center;
                        if (AITimer < 50)
                        {

                            Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                            if (disposablePos[9].X < NPC.Center.X)
                                pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.Grounded())
                                NPC.velocity.Y = -9.5f;
                            NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                            FacePlayer();
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                        }
                        else
                            NPC.velocity.X *= 0.9f;
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        rightArmRot = Helper.LerpAngle(rightArmRot, 0, 0.2f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 40)
                            if (oldAttack != AIState && oldAttack != SpectralOrbs)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), phaseMult == 3 ? "AHAHAHA!" : "Glittering Amethysts!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);

                        if (AITimer > 60 && AITimer < 170)
                        {
                            if (AITimer % 15 == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Shatter, NPC.Center);
                                for (int i = 0; i < disposablePos.Length; i++)
                                {
                                    if (disposablePos[i] != Vector2.Zero)
                                        continue;
                                    disposablePos[i] = Main.rand.NextVector2FromRectangle(GetArenaRect());

                                    for (int j = 0; j < 5; j++)
                                    {
                                        Dust.NewDustPerfect(disposablePos[i], DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                                        Dust.NewDustPerfect(disposablePos[i], DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));

                                        Dust.NewDustPerfect(staffTip, DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                                        Dust.NewDustPerfect(staffTip, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f)).customData = disposablePos[i];
                                    }
                                    break;
                                }
                            }
                        }
                        if (AITimer > 60 && AITimer % 20 == 0)
                        {
                            for (int i = 0; i < disposablePos.Length; i++)
                            {
                                Projectile.NewProjectile(null, disposablePos[i], Helper.FromAToB(disposablePos[i], player.Center + Main.rand.NextVector2Circular(150, 150)) * 0.1f, ProjectileType<XAmethystCloseIn>(), 15, 0);

                                if (phaseMult == 3)
                                {
                                    Projectile.NewProjectile(null, disposablePos[i], Helper.FromAToB(disposablePos[i], player.Center) * 0.05f, ProjectileType<XAmethystCloseIn>(), 15, 0);
                                }
                                disposablePos[i] = Vector2.Zero;
                            }
                        }
                        if (AITimer >= 200)
                        {
                            Reset();
                            PickAttack();
                        }
                    }
                    break;
                case GiantAmethyst:
                    {
                        if (phaseMult == 3)
                            headFrame.Y = AngryFace;
                        else
                            headFrame.Y = phase2 ? SmirkFace : LightSmirkFace;
                        if (AITimer == 1) disposablePos[9] = player.Center;
                        if (AITimer < 50)
                        {
                            if (phaseMult == 1)
                            {
                                Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                                if (disposablePos[9].X < NPC.Center.X)
                                    pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);

                                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                                if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.Grounded())
                                    NPC.velocity.Y = -9.5f;
                                NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                                NPC.direction = disposablePos[9].X > NPC.Center.X ? -1 : 1;
                                NPC.spriteDirection = NPC.direction;
                                NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                            }
                            else if (phaseMult == 2)
                            {
                                FacePlayer();
                                disposablePos[9] = GetArenaRect().Center.ToVector2() - new Vector2(0, 16);
                                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                                NPC.velocity.X = Helper.FromAToB(NPC.Center, disposablePos[9], false).X * .03f;
                                rightArmRot = Helper.LerpAngle(rightArmRot, -NPC.direction + MathHelper.ToRadians(-30f) * NPC.direction, 0.15f);
                                leftArmRot = Helper.LerpAngle(leftArmRot, NPC.direction + MathHelper.ToRadians(30f) * NPC.direction, 0.15f);
                            }
                            else
                            {
                                rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, NPC.Center - Vector2.UnitY.RotatedBy(NPC.direction * 0.5f) * 100, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);

                                headFrame.Y = AngryFace;
                                NPC.velocity *= 0.9f;
                                NPC.noGravity = false;
                                heliAlpha = MathHelper.Lerp(heliAlpha, 0, 0.25f);
                                Dust d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(200, 200), DustType<LineDustFollowPoint>(), Helper.FromAToB(Main.rand.NextVector2FromRectangle(NPC.getRect()), NPC.Center) * Main.rand.NextFloat(3, 7), 0, Color.Indigo, Main.rand.NextFloat(0.06f, .2f));
                                d.noGravity = true;
                                d.customData = NPC.Center + Helper.FromAToB(NPC.Center, player.Center) * 20;
                            }
                        }
                        else
                            NPC.velocity.X *= 0.9f;
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        rightArmRot = Helper.LerpAngle(rightArmRot, -Vector2.UnitY.ToRotation() - (NPC.direction == -1 ? (MathHelper.PiOver2 + MathHelper.PiOver4) : 0), 0.2f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 40)
                            if (oldAttack != AIState)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), phaseMult == 3 ? "GET AWAY FROM ME!!" : "The Archmage's Great Amethysts!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? "I HATE YOU!!" : "And again!"), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        if (phaseMult < 3)
                        {
                            if (AITimer >= 60 && phase2 && AITimer % 35 == 0)
                            {
                                float off = Main.rand.NextFloat(Pi);
                                for (int i = 0; i < 3; i++)
                                {
                                    float angle = Helper.CircleDividedEqually(i, 3) + off;
                                    Projectile.NewProjectile(null, staffTip, angle.ToRotationVector2(), ProjectileType<XLargeAmethyst>(), 15, 0, ai2: 1);
                                }
                            }
                            if (!phase2 && AITimer >= 60 && AITimer % 25 == 0)
                            {
                                Projectile.NewProjectile(null, staffTip, Vector2.Zero, ProjectileType<XLargeAmethyst>(), 15, 0);

                                Projectile.NewProjectile(null, staffTip, Vector2.Zero, ProjectileType<XExplosionInvis>(), 0, 0);
                            }

                        }
                        else
                        {
                            if (AITimer > 90 && AITimer % 25 == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Shatter, staffTip);
                                for (int h = 0; h < 5; h++)
                                {
                                    Projectile.NewProjectile(null, staffTip, Main.rand.NextVector2Circular(7 + AITimer * 0.02f, 7 + AITimer * 0.02f), ProjectileType<XAmethystShard>(), 15, 0);
                                }

                                Projectile.NewProjectile(null, staffTip, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0);
                                float off = Main.rand.NextFloat(Pi);
                                for (int i = 0; i < 5; i++)
                                {
                                    Projectile.NewProjectileDirect(null, staffTip, Helper.FromAToB(staffTip, player.Center).RotatedBy(Helper.CircleDividedEqually(i, 5) + off) * 0.25f, ProjectileType<XBolt>(), 15, 0).tileCollide = false;
                                }
                            }
                        }
                        if (AITimer >= (phase2 ? (phaseMult == 3 ? 200 : 151) : (phaseMult == 1 ? 100 : 76)))
                        {
                            Reset();
                            PickAttack();
                        }
                    }
                    break;
                case Micolash:
                    {
                        rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, disposablePos[0], reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                        //if (AITimer == 40)
                        //  if (oldAttack != AIState)
                        //    currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "Think fast!", Color.Violet, -1, 0.Color.Indigo* 0.5f, default, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)),3);

                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        if (AITimer == 40)
                            if (oldAttack != AIState && AITimer3 == 0)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), phaseMult == 3 ? "DIE!!" : "Shadowflame Tendrils!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? "Just DIEEE!" : "And again!"), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);

                        if (phaseMult == 3 && AITimer3 > 0 && AITimer == 121)
                        {
                            string dialogue = "DIE!!!!";
                            if (AITimer3 == 2)
                                dialogue = "DIE!!!!!!!!!!!!!";
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), dialogue, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        }
                        if (AITimer < 122)
                        {
                            if (phaseMult == 3)
                                headFrame.Y = AngryFace;
                            else
                                headFrame.Y = AssholeFace;
                            FacePlayer();
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.02f, 0.1f);

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.Grounded())
                                NPC.velocity.Y = -9.5f;
                            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center - Helper.FromAToB(NPC.Center, player.Center) * 120, false).X * 0.07f, 0.1f);
                        }
                        else
                        {
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                            NPC.velocity.X *= 0.7f;
                        }
                        if (AITimer == 130)
                        {
                            SoundEngine.PlaySound(EbonianSounds.xSpirit.WithPitchOffset(-0.5f), NPC.Center);
                            for (int i = 0; i < 20; i++)
                            {
                                Dust.NewDustPerfect(staffTip, DustType<SparkleDust>(), Main.rand.NextVector2Circular(5, 5), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                                Dust.NewDustPerfect(staffTip, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
                            }
                        }
                        if (AITimer == 150)
                        {
                            for (int i = -1; i < 2; i++)
                                Projectile.NewProjectile(null, NPC.Center + Helper.FromAToB(NPC.Center, disposablePos[0]) * 60, Helper.FromAToB(NPC.Center, disposablePos[0]).RotatedBy(i), ProjectileType<XRiftSmall>(), 0, 0);

                            SoundEngine.PlaySound(EbonianSounds.xSpirit.WithPitchOffset(-0.5f), NPC.Center);
                            for (int i = 0; i < 20; i++)
                            {
                                Dust.NewDustPerfect(staffTip, DustType<SparkleDust>(), Main.rand.NextVector2Circular(15, 15), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                                Dust.NewDustPerfect(staffTip, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(20, 20), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
                            }
                        }
                        if (AITimer < 130)
                            disposablePos[0] = player.Center;
                        if (AITimer > 170 && AITimer < 202 && AITimer % 4 == 0)
                        {
                            if (phaseMult == 3)
                                headFrame.Y = AngryFace;
                            else
                                headFrame.Y = VeryShockedFace;
                        }

                        if (AITimer >= (phaseMult == 3 ? 220 : 250))
                        {
                            Reset();
                            AITimer3++;
                            if (AITimer3 >= (phase2 ? (phaseMult) : 0))
                            {
                                AITimer3 = 0;
                                PickAttack();
                            }
                            else
                                AITimer = (phaseMult == 3 ? 120 : 80);
                        }
                    }
                    break;
                case TheSheepening:
                    {
                        if (phaseMult == 3)
                            headFrame.Y = AngryFace;
                        else
                            headFrame.Y = SmirkFace;
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        if (AITimer == 1) disposablePos[9] = player.Center;
                        if (AITimer < 50)
                        {

                            Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                            if (disposablePos[9].X < NPC.Center.X)
                                pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);
                            NPC.direction = disposablePos[9].X < NPC.Center.X ? -1 : 1;
                            NPC.spriteDirection = NPC.direction;
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.Grounded())
                                NPC.velocity.Y = -9.5f;
                            NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                        }
                        else
                            NPC.velocity.X *= 0.9f;
                        if (AITimer < (phase2 ? 160 : 70) && AITimer > 50)
                        {
                            FacePlayer();
                            rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, player.Center, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                        }
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 40)
                        {
                            WeightedRandom<string> chat = new WeightedRandom<string>();
                            chat.Add("Bleat now!");
                            chat.Add("'Baa! Baa!'... That's you!");
                            chat.Add("The Sheepening!");
                            chat.Add("To the slaughter with you!");
                            chat.Add("Polymorph!");
                            if (oldAttack != AIState)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), phaseMult == 3 ? "GET OUT OF MY SIGHT!" : chat, Color.Violet, -1, 0.6f, Color.Indigo * 0.6f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? "GET OUT!!!" : "And again!"), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        }

                        if (AITimer == 50)
                        {

                            for (int i = 0; i < 15; i++)
                            {
                                Dust.NewDustPerfect(NPC.Center, DustType<SparkleDust>(), Main.rand.NextVector2Circular(17, 17), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                                Dust.NewDustPerfect(NPC.Center, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(20, 20), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
                            }
                        }
                        bool shouldAttack = AITimer == 70;
                        if (phase2)
                            shouldAttack = AITimer == 70 || (AITimer >= 120 && AITimer % 30 == 0 && AITimer <= 200);
                        if (shouldAttack)
                        {
                            SoundEngine.PlaySound(EbonianSounds.cursedToyCharge, NPC.Center);
                            Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center + player.velocity), ProjectileType<SheepeningOrb>(), 1, 0, player.whoAmI);

                            if (phaseMult == 3 && AITimer % 60 == 0)
                                for (int i = -1; i < 2; i++)
                                {
                                    if (i == 0) continue;
                                    Projectile.NewProjectileDirect(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center + player.velocity).RotatedBy(i * 0.75f), ProjectileType<SheepeningOrb>(), 1, 0, -1).localAI[0] = 1;
                                }

                        }
                        if (phaseMult == 3 && AITimer == 260)
                        {
                            for (int i = -2; i < 4; i++)
                                Projectile.NewProjectileDirect(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center + player.velocity).RotatedBy(i * 0.35f), ProjectileType<SheepeningOrb>(), 1, 0, -1).localAI[0] = 1;
                        }
                        if (AITimer >= (phase2 ? 260 + (phaseMult == 3 ? 50 : 0) : 160))
                        {
                            Reset();
                            PickAttack();
                        }
                    }
                    break;
                case ManaPotion:
                    {
                        if (phaseMult == 3)
                            headFrame.Y = AngryFace;
                        else
                            headFrame.Y = SmirkFace;
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        headFrame.Y = DisappointedFace;
                        FacePlayer();

                        if (AITimer == 30 && phaseMult == 3)
                        {
                            if (NPC.ai[3] <= 0)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "TAKE THIS!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                            else
                            {
                                WeightedRandom<string> chat = new WeightedRandom<string>();
                                if (NPC.ai[3] == 1)
                                    chat.Add("AND SOME OF THIS!");
                                else
                                {
                                    if (NPC.ai[3] % 2 == 0)
                                    {
                                        chat.Add("AND SOME OF THAT!");
                                        chat.Add("AND THAT!");
                                        chat.Add("AND A LITTLE BIT OF THAT!");
                                    }
                                    else
                                    {
                                        chat.Add("AND THIS!");
                                        chat.Add("AND A LITTLE BIT OF THIS!");
                                    }
                                }
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), chat, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                            }
                        }
                        if (AITimer < 40)
                        {
                            if (AITimer > 20)
                                manaPotAlpha = MathHelper.Lerp(0, 1, (AITimer - 20) / 20);
                            else
                                staffAlpha = MathHelper.Lerp(1, 0, AITimer / 19);
                            if (AITimer == 25 || AITimer == 5)
                            {
                                Vector2 pos = NPC.Center - new Vector2(26 + (NPC.direction == -1 ? 8 : 18), 0).RotatedBy(NPC.direction == -1 ? (rightArmRot - MathHelper.PiOver2) : (rightArmRot - (MathHelper.Pi - MathHelper.PiOver4))) - new Vector2(0, 18);
                                for (int i = 0; i < 10; i++)
                                {
                                    Dust.NewDustPerfect(pos, DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                                    Dust.NewDustPerfect(pos, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
                                }
                            }
                            AITimer2 = MathHelper.Lerp(AITimer2, MathHelper.PiOver4, 0.1f);
                            disposablePos[0] = player.Center;
                            rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, disposablePos[0], reverse: true).ToRotation() + rightHandOffsetRot - AITimer2 * NPC.direction, 0.2f);
                        }
                        if (AITimer == 60)
                        {
                            Vector2 pos = NPC.Center - new Vector2(26 + (NPC.direction == -1 ? 8 : 18), 0).RotatedBy(NPC.direction == -1 ? (rightArmRot - MathHelper.PiOver2) : (rightArmRot - (MathHelper.Pi - MathHelper.PiOver4))) - new Vector2(0, 18);
                            Projectile.NewProjectile(null, pos, Helper.FromAToB(pos, player.Center + player.velocity) * Main.rand.NextFloat(12, 17), ProjectileType<XManaPotion>(), 15, 0);
                            if (phase2)
                                for (int i = 0; i < 2 + (phaseMult == 3 ? 2 : 0); i++)
                                    Projectile.NewProjectile(null, pos, Helper.FromAToB(pos, player.Center + player.velocity).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(4, 10), ProjectileType<XManaPotion>(), 15, 0);

                            if (phaseMult == 3)
                                Projectile.NewProjectile(null, pos, new Vector2(Helper.FromAToB(pos, player.Center + player.velocity).X * Main.rand.NextFloat(12, 17), -10), ProjectileType<XManaPotion>(), 15, 0);
                        }
                        if (AITimer > 60)
                        {
                            AITimer2 = MathHelper.Lerp(AITimer2, -(MathHelper.PiOver2 * 0.75f), 0.25f);
                            if (AITimer > 75)
                                staffAlpha = MathHelper.Lerp(0, 1, 0.1f);
                            manaPotAlpha = 0;
                            rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, disposablePos[0], reverse: true).ToRotation() + rightHandOffsetRot - AITimer2 * NPC.direction, 0.2f);
                        }
                        if (AITimer >= 100)
                        {
                            Reset();
                            NPC.ai[3] = MathHelper.Clamp(NPC.ai[3] + 1, 0, 20);
                            if (Main.rand.Next((int)MathHelper.Clamp(phaseMult == 3 ? ((doneAttacksBefore ? 8 : 15) - (int)NPC.ai[3]) : 18, 1, 20)) < 4)
                            {
                                NPC.ai[3] = 0;
                                PickAttack();
                            }
                        }
                    }
                    break;
                case PhantasmalBlast: //phase 2
                    {
                        if (AITimer == 1) disposablePos[9] = player.Center;
                        if (AITimer < 50)
                        {

                            Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                            if (disposablePos[9].X < NPC.Center.X)
                                pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.Grounded())
                                NPC.velocity.Y = -9.5f;
                            NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                            FacePlayer();
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                        }
                        else
                        {
                            NPC.velocity.X *= 0.9f;
                        }
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 30)
                            if (oldAttack != AIState)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), phaseMult == 3 ? ("DESTROY " + (player.HasBuff<Sheepened>() || player.HasBuff(BuffID.WolfMount) ? "IT!" : (player.Male ? "HIM!" : "HER!"))) : "Phantasmal Blast!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? "COME ON!" : "And again!"), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);

                        if (AITimer == 50)
                        {
                            AITimer2 = 0.5f;
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<XExplosionInvis>(), 0, 0);
                        }

                        if (AITimer < 205 && AITimer > 50) rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, player.Center, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                        else rightArmRot = Helper.LerpAngle(rightArmRot, 0, 0.2f);

                        if (AITimer > 80 && AITimer < 140 && AITimer % 20 < 10)
                        {
                            headFrame.Y = AngryFace;
                            Vector2 pos = NPC.Center + Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(10, 50);
                            Dust.NewDustPerfect(pos, DustType<SparkleDust>(), Helper.FromAToB(NPC.Center, pos) * Main.rand.NextFloat(3, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                        }
                        if (AITimer > 80 && AITimer < 170 && AITimer % (phaseMult == 3 ? 10 : 15) == 0)
                        {
                            FacePlayer();
                            AITimer2 += 0.1f;
                            headFrame.Y = ShockedFace;
                            SoundEngine.PlaySound(EbonianSounds.xSpirit, NPC.Center);
                            EbonianSystem.ScreenShakeAmount = 6;
                            Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(MathHelper.PiOver2 * 0.7f * AITimer2) * Main.rand.NextFloat(7, 12) * (phaseMult == 3 ? 0.8f : 0.6f), ProjectileType<XSpiritNoHome>(), 15, 0, -1, 0.25f);
                        }
                        if (AITimer == 140)
                        {
                            disposablePos[0] = player.Center;
                            Vector2 vel = Helper.FromAToB(NPC.Center, disposablePos[0]);
                            if (phaseMult == 3)
                                Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center + player.velocity), ProjectileType<SheepeningOrb>(), 1, 0, player.whoAmI);
                            else
                            {
                                Projectile.NewProjectile(null, NPC.Center + vel * 15, vel, ProjectileType<XSineLaser>(), 0, 0, ai1: 7.5f);
                                Projectile.NewProjectile(null, NPC.Center + vel * 15, vel, ProjectileType<XSineLaser>(), 0, 0, ai1: -7.5f);
                            }
                        }
                        if (AITimer > 160 && AITimer < 200 && AITimer % 15 == 0 && phaseMult == 3)
                            Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center + player.velocity), ProjectileType<SheepeningOrb>(), 1, 0, player.whoAmI);
                        if (AITimer == 170 && phaseMult != 3)
                        {
                            headFrame.Y = ShockedFace;
                            for (int i = 0; i < 20; i++)
                            {
                                Vector2 pos = NPC.Center + Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(10, 50);
                                if (i % 2 == 0)
                                    Dust.NewDustPerfect(pos, DustType<SparkleDust>(), Helper.FromAToB(NPC.Center, pos) * Main.rand.NextFloat(10, 15), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                                else
                                    Dust.NewDustPerfect(pos, DustType<LineDustFollowPoint>(), Helper.FromAToB(NPC.Center, pos) * Main.rand.NextFloat(10, 15), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                            }
                            Vector2 vel = Helper.FromAToB(NPC.Center, disposablePos[0]);
                            SoundEngine.PlaySound(EbonianSounds.cursedToyCharge, NPC.Center);
                            Projectile.NewProjectileDirect(null, NPC.Center + vel * 15, vel, ProjectileType<XSineLaser>(), 15, 0, ai1: 7.5f).localAI[0] = 1;
                            Projectile.NewProjectileDirect(null, NPC.Center + vel * 15, vel, ProjectileType<XSineLaser>(), 15, 0, ai1: -7.5f).localAI[0] = 1;
                        }

                        if (AITimer >= 240)
                        {
                            Reset();
                            PickAttack();
                        }
                    }
                    break;
                case ShadowflameRift:
                    {
                        if (phaseMult == 3)
                            headFrame.Y = AngryFace;
                        else
                            headFrame.Y = SmirkFace;
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        //more portals in phase 2
                        if (AITimer == 80)
                        {
                            WeightedRandom<string> chat = new WeightedRandom<string>();
                            chat.Add("Hold up, I forgot to bring something...");
                            chat.Add("Don't mind me! I just forgot something...");
                            chat.Add("I seem to have forgotten something...");
                            chat.Add("Hehehehe..."); //phase 2
                            if (oldAttack != AIState)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), phaseMult == 3 ? "STAND STILL!" : chat, Color.Violet, -1, 0.6f, Color.Indigo * 0.6f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? "I SAID STAND STILL!" : "And again!"), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        }
                        if (AITimer == 1) disposablePos[9] = player.Center;
                        if (AITimer < 50)
                        {

                            Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                            if (disposablePos[9].X < NPC.Center.X)
                                pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.Grounded())
                                NPC.velocity.Y = -9.5f;
                            NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                            NPC.direction = disposablePos[9].X > NPC.Center.X ? -1 : 1;
                            NPC.spriteDirection = NPC.direction;
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                        }
                        else
                        {
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                            NPC.velocity.X *= 0.9f;
                        }
                        if (AITimer > 60 && AITimer < 160)
                        {
                            rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, disposablePos[0], reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                            if (AITimer < 160)
                                disposablePos[1] = player.Center - new Vector2(-player.velocity.X, 100).RotatedByRandom(MathHelper.TwoPi);

                            if (AITimer < 85)
                            {
                                disposablePos[0] = NPC.Center + new Vector2(NPC.direction * 70, 0);
                            }
                            if (AITimer % 2 == 0)
                            {
                                Dust.NewDustPerfect(disposablePos[0], DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                                Dust.NewDustPerfect(disposablePos[0], DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f)).customData = NPC.Center;
                                Dust.NewDustPerfect(NPC.Center, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f)).customData = disposablePos[0];

                                Dust.NewDustPerfect(disposablePos[1], DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                                Dust.NewDustPerfect(disposablePos[1], DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
                            }
                        }
                        if (AITimer > 60 && AITimer < 270 && AITimer % 5 == 0)
                        {
                            Dust.NewDustPerfect(disposablePos[0], DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f)).customData = NPC.Center;
                            Dust.NewDustPerfect(NPC.Center, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f)).customData = disposablePos[0];
                        }
                        if (AITimer == 85)
                        {
                            Projectile.NewProjectile(null, disposablePos[0], Helper.FromAToB(disposablePos[0], NPC.Center), ProjectileType<XRift>(), 0, 0);
                        }

                        if (phaseMult < 3)
                        {
                            if (AITimer == 140)
                            {
                                Projectile.NewProjectile(null, disposablePos[1] + Helper.FromAToB(disposablePos[1], player.Center) * 10, Helper.FromAToB(disposablePos[1], player.Center), ProjectileType<XRift>(), 15, 0);
                                if (phaseMult == 3)
                                    Projectile.NewProjectile(null, disposablePos[1], -Helper.FromAToB(disposablePos[1], player.Center), ProjectileType<XRift>(), 15, 0);
                            }

                            if (AITimer == 200 && phase2)
                            {
                                disposablePos[1] = player.Center - new Vector2(-player.velocity.X, 100).RotatedByRandom(MathHelper.TwoPi);
                                Projectile.NewProjectile(null, disposablePos[1] + Helper.FromAToB(disposablePos[1], player.Center) * 10, Helper.FromAToB(disposablePos[1], player.Center), ProjectileType<XRift>(), 15, 0);
                            }
                        }
                        else
                        {
                            if (AITimer > 140 && AITimer < 241 && AITimer % 30 == 0)
                            {
                                disposablePos[1] = Helper.TRay.Cast(Vector2.Clamp(player.Center, GetArenaRect().BottomLeft() + new Vector2(30, -30), GetArenaRect().TopRight() + new Vector2(-30, 30)), -Vector2.UnitY, 600);
                                Projectile.NewProjectile(null, disposablePos[1], Vector2.UnitY, ProjectileType<XRift>(), 15, 0);

                                disposablePos[1] = Helper.TRay.Cast(Vector2.Clamp(player.Center, GetArenaRect().BottomLeft() + new Vector2(30, -30), GetArenaRect().TopRight() + new Vector2(-30, 30)), Vector2.UnitY, 600);
                                Projectile.NewProjectile(null, disposablePos[1], -Vector2.UnitY, ProjectileType<XRift>(), 15, 0);
                            }
                        }

                        if (AITimer >= (phaseMult == 3 ? 270 : 320))
                        {
                            Reset();
                            PickAttack();
                        }
                    }
                    break;
                case HelicopterBlades: //phase 2
                    {
                        if (phaseMult == 3 && AITimer == 1)
                        {
                            AITimer = 450;
                        }
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);
                        if (AITimer < 400)
                            FacePlayer();
                        if (AITimer < 405)
                            rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, NPC.Center - Vector2.UnitY.RotatedBy(NPC.direction * 0.5f) * 100, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);

                        if (AITimer == 40)
                            helicopterSlot = SoundEngine.PlaySound(EbonianSounds.helicopter);
                        if (AITimer >= 40 && AITimer <= 440)
                        {
                            headFrame.Y = AssholeFace;
                            if (AITimer > 80 && AITimer % 10 == 0 && AITimer < 120 || AITimer == 170)
                            {
                                SoundEngine.PlaySound(SoundID.Item73.WithPitchOffset(-0.2f), NPC.Center);
                                Vector2 vel = Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(MathHelper.PiOver4 * 0.5f);
                                if (AITimer == 170)
                                    vel = Helper.FromAToB(NPC.Center, player.Center + player.velocity);
                                Projectile.NewProjectile(null, NPC.Center - new Vector2(14 + (NPC.direction == -1 ? 10 : 20), 0).RotatedBy(NPC.direction == -1 ? (rightArmRot - MathHelper.PiOver2 * 1.11f) : (rightArmRot - (MathHelper.Pi - MathHelper.PiOver4))) - new Vector2(25 * -NPC.direction, 24), vel * 7f, ProjectileType<XKnife>(), 15, 0);
                            }
                            heliAlpha = MathHelper.Lerp(heliAlpha, 1, 0.1f);
                            NPC.noGravity = true;
                            if (AITimer > 40 && AITimer < 170)
                                NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, Helper.TRay.Cast(player.Center, Vector2.UnitY, 300, true) - new Vector2(0, 150), false) / 35, 0.025f);
                            else if (AITimer >= 170 && AITimer < 240)
                                NPC.velocity *= 0.9f;
                            else if (AITimer > 240 && AITimer < 400)
                            {
                                if (AITimer % (phaseMult == 3 ? 15 : 30) == 0)
                                {
                                    SoundEngine.PlaySound(EbonianSounds.xSpirit, NPC.Center);
                                    Projectile.NewProjectile(null, staffTip, Helper.FromAToB(staffTip, player.Center + player.velocity).RotatedByRandom(MathHelper.PiOver4 * 0.5f) * 2, ProjectileType<XBolt>(), 15, 0);
                                }
                                NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, Helper.TRay.Cast(player.Center, Vector2.UnitY, 300, true) - new Vector2(MathF.Sin(AITimer * 3) * 200, 150), false) / 25, 0.05f);
                            }
                            else
                                NPC.velocity *= 0.9f;
                        }
                        if (AITimer > 400 && AITimer < 490)
                        {
                            rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, NPC.Center - Vector2.UnitY.RotatedBy(NPC.direction * 0.5f) * 100, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                            if (phaseMult == 3)
                                headFrame.Y = AngryFace;
                            else
                                headFrame.Y = phase2 ? SmirkFace : LightSmirkFace;
                            if (SoundEngine.TryGetActiveSound(helicopterSlot, out var sound))
                            {
                                sound.Volume = MathHelper.Lerp(sound.Volume, 0, 0.1f);
                            }
                            NPC.velocity *= 0.9f;
                            NPC.noGravity = false;
                            heliAlpha = MathHelper.Lerp(heliAlpha, 0, 0.25f);
                            Dust d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(200, 200), DustType<LineDustFollowPoint>(), Helper.FromAToB(Main.rand.NextVector2FromRectangle(NPC.getRect()), NPC.Center) * Main.rand.NextFloat(3, 7), 0, Color.Indigo, Main.rand.NextFloat(0.06f, .2f));
                            d.noGravity = true;
                            d.customData = NPC.Center + Helper.FromAToB(NPC.Center, player.Center) * 20;
                        }
                        if (AITimer == 470)
                        {
                            disposablePos[0] = Helper.FromAToB(NPC.Center, player.Center);
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0, ai2: 1);
                        }
                        if (phaseMult < 3)
                        {
                            if (AITimer > 499 && AITimer <= 530)
                            {
                                if (AITimer % 10 == 0)
                                {
                                    SoundEngine.PlaySound(SoundID.Item73.WithPitchOffset(-0.2f), NPC.Center);
                                    Projectile.NewProjectile(null, staffTip, Helper.FromAToB(staffTip, player.Center).RotatedByRandom(MathHelper.PiOver4) * 7f, ProjectileType<XKnife>(), 15, 0);
                                }
                            }
                        }
                        else
                        {
                            if (AITimer == 460)
                            {
                                if (oldAttack != AIState)
                                    currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "JUST LEAVE ME ALONE!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                            }
                            if (AITimer == 520)
                            {
                                for (int i = 0; i < 14; i++)
                                {
                                    float angle = Helper.CircleDividedEqually(i, 14);
                                    if (i % 2 == 0)
                                        Projectile.NewProjectileDirect(null, staffTip, angle.ToRotationVector2() * 0.5f, ProjectileType<XBolt>(), 15, 0);
                                    else
                                        Projectile.NewProjectileDirect(null, staffTip, angle.ToRotationVector2() * 0.5f, ProjectileType<XAmethyst>(), 15, 0).tileCollide = false;
                                }
                            }

                            if (AITimer == 560)
                            {
                                for (int i = 0; i < 18; i++)
                                {
                                    float angle = Helper.CircleDividedEqually(i, 18);
                                    if (i % 2 == 0)
                                        Projectile.NewProjectileDirect(null, staffTip, angle.ToRotationVector2(), ProjectileType<XBolt>(), 15, 0);
                                    else
                                        Projectile.NewProjectileDirect(null, staffTip, angle.ToRotationVector2(), ProjectileType<XAmethyst>(), 15, 0).tileCollide = false;
                                }
                            }
                        }
                        if (AITimer >= (phaseMult == 3 ? 620 : 550))
                        {
                            Reset();
                            PickAttack();
                        }
                    }
                    break;
                case AmethystStorm:
                    {
                        if (AITimer == 40)
                            if (oldAttack != AIState)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), phaseMult == 3 ? "YOU WILL BLEAT!" : "Shadowflame Mist!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? (player.HasBuff<Sheepened>() ? "NOT ENOUGH BLEATING!" : "I SAID.. YOU WILL BLEAT!") : "And again!"), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        if (AITimer < 260 && AITimer > 50)
                        {
                            if (phaseMult == 3)
                                headFrame.Y = AngryFace;
                            else
                                headFrame.Y = phase2 ? SmirkFace : DisappointedFace;
                            rightArmRot = Helper.LerpAngle(rightArmRot, -NPC.direction + MathHelper.ToRadians(-40f) * NPC.direction, 0.15f);
                            leftArmRot = Helper.LerpAngle(leftArmRot, NPC.direction + MathHelper.ToRadians(40f) * NPC.direction, 0.15f);
                        }
                        else
                        {
                            headFrame.Y = phase2 ? DisappointedFace : NeutralFace;
                            IdleAnimation();
                        }

                        if (AITimer == 1) disposablePos[9] = player.Center;
                        if (AITimer < 50)
                        {

                            Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                            if (disposablePos[9].X < NPC.Center.X)
                                pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);
                            NPC.direction = disposablePos[9].X > NPC.Center.X ? -1 : 1;
                            NPC.spriteDirection = NPC.direction;
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.Grounded())
                                NPC.velocity.Y = -9.5f;
                            NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                        }
                        else
                        {
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                            NPC.velocity.X *= 0.9f;
                        }
                        if (AITimer == 70)
                        {
                            if (phase2)
                            {
                                Projectile.NewProjectileDirect(null, GetArenaRect().Center() + new Vector2(250, -100), Vector2.Zero, ProjectileType<XCloud>(), 0, 0, player.whoAmI, ai1: (phaseMult == 3 ? 1 : 0)).timeLeft = 300;
                                Projectile.NewProjectileDirect(null, GetArenaRect().Center() + new Vector2(-250, -100), Vector2.Zero, ProjectileType<XCloud>(), 0, 0, player.whoAmI, -20, ai1: (phaseMult == 3 ? 1 : 0)).timeLeft = 300;
                                if (phaseMult == 3)
                                {
                                    Projectile.NewProjectileDirect(null, GetArenaRect().Center() + new Vector2(0, -150), Vector2.Zero, ProjectileType<XCloud>(), 0, 0, player.whoAmI, -30, ai1: (phaseMult == 3 ? 1 : 0)).timeLeft = 300;

                                    Projectile.NewProjectileDirect(null, GetArenaRect().Center() + new Vector2(420, -50), Vector2.Zero, ProjectileType<XCloud>(), 0, 0, player.whoAmI, -10, ai1: (phaseMult == 3 ? 1 : 0)).timeLeft = 300;
                                    Projectile.NewProjectileDirect(null, GetArenaRect().Center() + new Vector2(-420, -50), Vector2.Zero, ProjectileType<XCloud>(), 0, 0, player.whoAmI, 5, ai1: (phaseMult == 3 ? 1 : 0)).timeLeft = 300;
                                }
                            }
                            else
                                Projectile.NewProjectile(null, new Vector2(MathHelper.Clamp(NPC.Center.X, GetArenaRect().X + 40, GetArenaRect().X + GetArenaRect().Width - 40), MathHelper.Clamp(NPC.Center.Y - 120, GetArenaRect().Y + 70, GetArenaRect().Y + GetArenaRect().Width - 70)), Vector2.Zero, ProjectileType<XCloud>(), 0, 0, player.whoAmI);
                        }

                        if (AITimer >= 280 + (phaseMult == 3 ? 100 : 0))
                        {
                            Reset();
                            PickAttack();
                        }
                    }
                    break;
                case BullshitRain:
                    {
                        if (AITimer == 40)
                            if (oldAttack != AIState)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "STOP DODGING!!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "STOP!!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);

                        headFrame.Y = AngryFace;
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        if (AITimer == 1) disposablePos[9] = player.Center;
                        if (AITimer < 50)
                        {

                            Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                            if (disposablePos[9].X < NPC.Center.X)
                                pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.Grounded())
                                NPC.velocity.Y = -9.5f;
                            NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                            NPC.direction = disposablePos[9].X > NPC.Center.X ? -1 : 1;
                            NPC.spriteDirection = NPC.direction;
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                        }
                        else
                        {
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                            NPC.velocity.X *= 0.9f;
                        }
                        if (AITimer > 60 && AITimer < 160)
                        {
                            rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, disposablePos[0], reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                            if (AITimer < 160)
                                disposablePos[1] = player.Center - new Vector2(-player.velocity.X, 100).RotatedByRandom(MathHelper.TwoPi);

                            if (AITimer < 85)
                            {
                                disposablePos[0] = NPC.Center + new Vector2(NPC.direction * 70, 0);
                            }
                            if (AITimer % 2 == 0)
                            {
                                Dust.NewDustPerfect(disposablePos[0], DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                                Dust.NewDustPerfect(disposablePos[0], DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f)).customData = NPC.Center;
                                Dust.NewDustPerfect(NPC.Center, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f)).customData = disposablePos[0];

                                Dust.NewDustPerfect(disposablePos[1], DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                                Dust.NewDustPerfect(disposablePos[1], DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
                            }
                        }
                        if (AITimer > 60 && AITimer < 270 && AITimer % 5 == 0)
                        {
                            Dust.NewDustPerfect(disposablePos[0], DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f)).customData = NPC.Center;
                            Dust.NewDustPerfect(NPC.Center, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f)).customData = disposablePos[0];
                        }
                        if (AITimer == 85)
                        {
                            Projectile.NewProjectile(null, disposablePos[0], Helper.FromAToB(disposablePos[0], NPC.Center), ProjectileType<XRift>(), 0, 0);
                        }

                        if (AITimer == 100)
                        {
                            for (float i = 0; i <= 1; i += 0.2f)
                            {
                                Vector2 pos = Helper.TRay.Cast(new Vector2(GetArenaRect().X + 30 + (GetArenaRect().Width - 60) * i, GetArenaRect().Center.Y), -Vector2.UnitY, 700);
                                Projectile.NewProjectile(null, pos, Vector2.UnitY, ProjectileType<XRift>(), 0, 0);
                            }
                        }
                        if (AITimer > 170 && AITimer < 260 && AITimer % 10 == 0)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 pos = Helper.TRay.Cast(new Vector2(GetArenaRect().X + 30 + (GetArenaRect().Width - 60) * Main.rand.NextFloat(), GetArenaRect().Center.Y), -Vector2.UnitY, 700);

                                Vector2 vel = Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4 * 0.7f);
                                SoundEngine.PlaySound(EbonianSounds.xSpirit, pos);
                                for (int j = 0; j < 10; j++)
                                {
                                    if (j % 2 == 0)
                                        Dust.NewDustPerfect(pos, DustType<SparkleDust>(), vel.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(2, 5), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.175f));
                                    else
                                        Dust.NewDustPerfect(pos, DustType<LineDustFollowPoint>(), vel.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(2, 5), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.175f));
                                }

                                WeightedRandom<int> projType = new();
                                projType.Add(ProjectileType<XAmethystCloseIn>());
                                projType.Add(ProjectileType<XBolt>());
                                projType.Add(ProjectileType<XKnife>());
                                projType.Add(ProjectileType<XManaPotion>());
                                int _projType = projType;
                                Vector2 _vel = Vector2.UnitY.RotatedByRandom(PiOver4 * 0.3f);
                                if (_projType == ProjectileType<XKnife>())
                                    _vel *= 7;
                                else
                                    Projectile.NewProjectileDirect(null, pos + new Vector2(0, 32), _vel, _projType, 20, 0);
                            }
                        }

                        if (AITimer >= 280)
                        {
                            Reset();
                            PickAttack();
                        }
                    }
                    break;
                case BONK:
                    {
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        if (AITimer == 40)
                            if (oldAttack != AIState && AITimer3 == 0)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), doneAttacksBefore ? "I'M SO TIRED OF YOUR NONSENSE!" : "LOOK WHAT YOU'RE MAKING ME DO!!!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "GOD DAMN YOU!!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);

                        if (AITimer < 110)
                        {
                            rightArmRot = Helper.LerpAngle(rightArmRot, 1.75f * -NPC.direction, 0.3f);
                            headFrame.Y = AngryFace;
                            FacePlayer();
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.02f, 0.1f);

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.Grounded())
                                NPC.velocity.Y = -9.5f;
                            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center - Helper.FromAToB(NPC.Center, player.Center) * 120, false).X * 0.07f, 0.1f);
                        }
                        else
                        {
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                            NPC.velocity.X *= 0.7f;
                        }
                        if (AITimer < 110)
                            disposablePos[0] = player.Center;

                        if (AITimer == 80)
                        {
                            staffAlpha = 0;
                            for (int i = 0; i < 15; i++)
                                Projectile.NewProjectile(null, staffTip, Main.rand.NextVector2Unit() * Main.rand.NextFloat(10, 20), ProjectileType<XAnimeSlash>(), 0, 0, -1, 0, Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(0.1f, 0.3f));

                            Projectile.NewProjectile(null, staffTip, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0);
                            bigStaffAlpha = 1;
                            bigStaffBloomAlpha = 1;
                            AITimer2 = 60;
                        }


                        if (AITimer > 130 && AITimer2 > 0)
                        {
                            AITimer2--;
                            int direction = -NPC.direction;
                            if (lerpProg > 0)
                                swingProgress = MathHelper.Lerp(swingProgress, BigStaffEasingFunction(Utils.GetLerpValue(0, 60, AITimer2)), lerpProg);
                            float defRot = (Vector2.UnitX * NPC.direction).ToRotation();
                            float start = defRot - (MathHelper.PiOver2 + MathHelper.PiOver4);
                            float end = defRot + (MathHelper.PiOver2 + MathHelper.PiOver4);
                            if (lerpProg >= 0)
                                rightArmRot = Helper.LerpAngle(rightArmRot, (direction == 1 ? start + MathHelper.Pi * 3 / 2 * swingProgress : end - MathHelper.Pi * 3 / 2 * swingProgress) + MathHelper.PiOver4 - rightHandOffsetRot, 0.3f * lerpProg);
                            Vector2 position = NPC.Center +
                                (rightArmRot - PiOver4 + Pi).ToRotationVector2() * 170 * BigStaffScaleFunction(swingProgress);

                            if (lerpProg > -1 && swingProgress > 0.1f && swingProgress < 0.9f && AITimer % 4 < 2)
                            {
                                Projectile.NewProjectile(null, position - Helper.FromAToB(position, NPC.Center) * 20, Helper.FromAToB(staffTip + (rightArmRot - PiOver4).ToRotationVector2() * 50, disposablePos[0]) * Main.rand.NextFloat(1, 3), ProjectileType<XBolt>(), 20, 0);
                            }

                            if (AITimer > 164)
                                if (AITimer3 == 0 && Helper.TRay.CastLength(position, Vector2.UnitY, 100) < 30)
                                {
                                    AITimer3 = 1;
                                    AITimer2 = 15;
                                    AITimer = 190;
                                    lerpProg = -1;
                                    SoundEngine.PlaySound(SoundID.Item70, position);
                                    SoundEngine.PlaySound(EbonianSounds.xSpirit, position);
                                    WeightedRandom<string> chat = new();
                                    chat.Add("WHAT ARE YOU DOING?!");
                                    chat.Add("XAREUS?!");
                                    chat.Add("???!");
                                    chat.Add("HUH?!");
                                    DialogueSystem.NewDialogueBox(40, position - new Vector2(0, 40), chat, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 8f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                                    Projectile p = Projectile.NewProjectileDirect(null, Helper.TRay.Cast(position - new Vector2(0, 20), Vector2.UnitY, 80), Vector2.Zero, ProjectileType<XImpact>(), 20, 0);
                                    p.friendly = false;
                                    p.hostile = true;
                                }

                        }

                        if (AITimer == 240)
                        {
                            for (int i = 0; i < 15; i++)
                                Projectile.NewProjectile(null, staffTip, Main.rand.NextVector2Unit() * Main.rand.NextFloat(10, 20), ProjectileType<XAnimeSlash>(), 0, 0, -1, 0, Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(0.1f, 0.3f));

                            Projectile.NewProjectile(null, staffTip, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0);
                            bigStaffBloomAlpha = 1;
                            bigStaffAlpha = 0;
                            staffAlpha = 1;
                        }
                        if (AITimer >= 270)
                        {
                            Reset();
                            AITimer3 = 0;
                            PickAttack();
                        }
                    }
                    break;
            }
        }
        float swingProgress, lerpProg = 1;
        float BigStaffEasingFunction(float x)
        {
            return x == 0
    ? 0
    : x == 1
    ? 1
    : x < 0.5 ? MathF.Pow(2, 20 * x - 10) / 2
    : (2 - MathF.Pow(2, -20 * x + 10)) / 2;
        }
        float BigStaffScaleFunction(float progress)
        {
            return 0.7f + (float)Math.Sin(progress * Math.PI) * 0.5f;
        }
        // Sounds like something
        Vector2 sCenter;
        Rectangle GetArenaRect()
        {
            float LLen = Helper.TRay.CastLength(sCenter, -Vector2.UnitX, 29f * 16);
            float RLen = Helper.TRay.CastLength(sCenter, Vector2.UnitX, 29f * 16);
            Vector2 U = Helper.TRay.Cast(sCenter, -Vector2.UnitY, 380);
            Vector2 D = Helper.TRay.Cast(NPC.Center, Vector2.UnitY, 380);
            sCenter.Y = U.Y + Helper.FromAToB(U, D, false).Y * 0.5f;
            Vector2 L = sCenter;
            Vector2 R = sCenter;
            if (LLen > 400)
            {
                if (LLen > RLen)
                {
                    R = Helper.TRay.Cast(sCenter, Vector2.UnitX, 29f * 16);
                    L = Helper.TRay.Cast(R, -Vector2.UnitX, 34.5f * 32);
                }
                else
                {
                    R = Helper.TRay.Cast(L, Vector2.UnitX, 34.5f * 32);
                    L = Helper.TRay.Cast(sCenter, -Vector2.UnitX, 29f * 16);
                }
            }
            else
            {
                R = sCenter + new Vector2(495, 0);
                L = sCenter - new Vector2(464, 0);
            }
            Vector2 TopLeft = new Vector2(L.X, U.Y);
            Vector2 BottomRight = new Vector2(R.X, D.Y);
            Rectangle rect = new Rectangle((int)L.X, (int)U.Y, (int)Helper.FromAToB(TopLeft, BottomRight, false).X, (int)Helper.FromAToB(TopLeft, BottomRight, false).Y);
            return rect;
        }
        Rectangle OffsetBoundaries(Vector2 offset)
        {
            Rectangle rect = GetArenaRect();
            rect.Width -= (int)offset.X;
            rect.Height -= (int)offset.Y;
            rect.X += (int)offset.X / 2;
            rect.Y += (int)offset.Y / 2;
            return rect;
        }
        Vector2[] disposablePos = new Vector2[15];
        public override bool? CanFallThroughPlatforms()
        {
            Player player = Main.player[NPC.target];
            return (player.Center.Y > NPC.Center.Y + 50 && (AIState == Idle || (AIState == BONK && AITimer < 130) || AIState == Spawn || (AIState == Micolash && AITimer < 130))) || NPC.noGravity;
        }
        void Reset()
        {
            if (SoundEngine.TryGetActiveSound(helicopterSlot, out var sound))
            {
                sound.Stop();
            }
            headOffIncrementOffset = Main.rand.NextFloat(500);
            AITimer = 0;
            AITimer2 = 0;
            NPC.noGravity = false;
            manaPotAlpha = 0;
            staffAlpha = 1;
            heliAlpha = 0;
            NPC.velocity.X = 0;
            bigStaffAlpha = 0;
            bigStaffBloomAlpha = 0;
            headFrame.Y = phase2 ? DisappointedFace : NeutralFace;
            for (int i = 0; i < disposablePos.Length; i++) disposablePos[i] = Vector2.Zero;
            NPC.damage = 0;
            lerpProg = 1;
            swingProgress = 0;
        }
    }
}
