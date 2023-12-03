using EbonianMod.Projectiles;
using EbonianMod;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Terraria.DataStructures;
using EbonianMod.Projectiles.VFXProjectiles;
using EbonianMod.Dusts;
using EbonianMod.Projectiles.Exol;
using System.IO;
using FullSerializer.Internal;
using System.Reflection.Metadata;

namespace EbonianMod.NPCs.Exol
{
    public class Ignos : ModNPC
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
                new FlavorTextBestiaryInfoElement("Type: Soul"),
                new FlavorTextBestiaryInfoElement("One of the greatest warriors known, and one of the few able to go toe to toe with Inferos. Even in death, his very soul yearns for a battle as grand as that of Inferos, if not grander, before he can finally rest."),
            });
        }
        public override void SetDefaults()
        {
            NPC.width = 134;
            NPC.height = 148;
            NPC.damage = 0;
            NPC.defense = 30;
            NPC.lifeMax = 35000;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Ignos");
            }
        }
        public float AIState { get => NPC.ai[0]; set => NPC.ai[0] = value; }
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
        const int Spawn = 0, SwordSlashes = 1, RykardSpiral = 2, SwordSlashesVariant2 = 3, HoldSwordUpAndChannelSoulVortex = 4, StabSwordAndThrowRock = 5, InfernalEye = 6, GreatArrowRain = 7, GiantExplosiveArrow = 8, FireGiantLavaExplosion = 9;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            PlayerDetection();
            AITimer++;
            switch (AIState)
            {
                case Spawn:
                    {
                        Reset();
                        AIState = HoldSwordUpAndChannelSoulVortex;
                    }
                    break;
                case SwordSlashes:
                    {
                        IdleMovement(new Vector2(0, -200));
                        if (AITimer % 3 == 0 && AITimer >= 30 && AITimer <= 100)
                        {
                            Projectile.NewProjectile(null, player.Center - new Vector2((AITimer - 30 - 50) * 40, 0), new Vector2(Main.rand.NextFloat(-0.1f, 0.1f), -1), ModContent.ProjectileType<IgnosSlashTelegraph>(), 30, 0);
                        }
                        if (AITimer == 75 || AITimer == 145)
                            Projectile.NewProjectile(null, player.Center, new Vector2(0, -1), ModContent.ProjectileType<IgnosSlashTelegraph>(), 30, 0);
                        if (AITimer % 3 == 0 && AITimer >= 90 && AITimer <= 160)
                        {
                            Projectile.NewProjectile(null, player.Center + new Vector2((AITimer - 90 - 50) * 40, 0), new Vector2(Main.rand.NextFloat(-0.1f, 0.1f), -1), ModContent.ProjectileType<IgnosSlashTelegraph>(), 30, 0);
                        }
                        if (AITimer >= 180)
                        {
                            Reset();
                            AIState = RykardSpiral;
                        }
                    }
                    break;
                case RykardSpiral:
                    {
                        if (AITimer < 40)
                        {
                            Vector2 pos = NPC.Center + 300 * Main.rand.NextVector2Unit();
                            Dust a = Dust.NewDustPerfect(pos, ModContent.DustType<GenericAdditiveDust>(), Helper.FromAToB(pos, NPC.Center) * Main.rand.NextFloat(10, 20), newColor: Color.OrangeRed, Scale: Main.rand.NextFloat(0.05f, 0.15f));
                            a.noGravity = false;
                            a.customData = 1;
                        }
                        if (AITimer == 45)
                        {
                            Projectile a = Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<FlameExplosion>(), 30, 0);
                            for (int i = 0; i < 8; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 8);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.One.RotatedBy(angle), ModContent.ProjectileType<RykardSkullSpiral>(), 30, 0);
                            }
                        }
                    }
                    break;
                case SwordSlashesVariant2:
                    {
                        if (AITimer < 15)
                            IdleMovement();
                        else
                            NPC.velocity *= 0.9f;
                        if (AITimer > 15 && AITimer % 3 == 0 && AITimer2 < 15)
                        {
                            AITimer2++;
                            float angle = Helper.CircleDividedEqually(AITimer2, 15);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, 200).RotatedBy(angle), Vector2.One.RotatedBy(angle), ModContent.ProjectileType<IgnosSlashTelegraph>(), 30, 0);
                        }
                    }
                    break;
                case HoldSwordUpAndChannelSoulVortex:
                    {
                        if (AITimer < 30)
                        {
                            IdleMovement();
                            vortexAlpha = Math.Min(vortexAlpha + 0.04f, 1);
                        }
                        else
                            NPC.velocity *= 0.85f;
                        if (AITimer == 40)
                        {
                            Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<IgArena>(), 0, 0);
                        }
                        if (AITimer > 40 && AITimer % 5 == 0 && AITimer < 80)
                        {
                            Vector2 vel = Main.rand.NextVector2Circular(5, 5);
                            Projectile.NewProjectile(null, NPC.Center - new Vector2(0, 100), vel, ModContent.ProjectileType<ESkullEmoji>(), 30, 0);
                        }
                        if (AITimer == 150)
                            Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center - new Vector2(0, 100), Vector2.Zero, ModContent.ProjectileType<FlameExplosion>(), 30, 0);
                        if (AITimer == 160)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 8);
                                Projectile.NewProjectile(null, NPC.Center - new Vector2(0, 100), Main.rand.NextFloat(5, 10) * Vector2.UnitX.RotatedBy(angle), ModContent.ProjectileType<ESkullEmoji>(), 30, 0);
                            }
                        }
                        if (AITimer > 200)
                        {
                            vortexAlpha = Math.Max(vortexAlpha - 0.04f, 0);
                        }
                    }
                    break;
                case StabSwordAndThrowRock:
                    {

                    }
                    break;
                case InfernalEye:
                    {

                    }
                    break;
                case GreatArrowRain:
                    {

                    }
                    break;
                case GiantExplosiveArrow:
                    {

                    }
                    break;
                case FireGiantLavaExplosion:
                    {

                    }
                    break;
            }
        }
        float rot;
        float vortexAlpha;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Helper.GetExtraTexture("vortex3");
            spriteBatch.Reload(BlendState.Additive);
            rot -= 0.025f;
            Vector2 scale = new Vector2(1f, 0.25f);
            spriteBatch.Reload(EbonianMod.SpriteRotation);
            EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(-rot);
            EbonianMod.SpriteRotation.Parameters["scale"].SetValue(scale * 0.75f);
            Vector4 col = (Color.OrangeRed * vortexAlpha).ToVector4();
            col.W = vortexAlpha;
            EbonianMod.SpriteRotation.Parameters["uColor"].SetValue(col);
            spriteBatch.Draw(tex, NPC.Center - new Vector2(0, 100) - Main.screenPosition, null, Color.White, 0, tex.Size() / 2, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(tex, NPC.Center - new Vector2(0, 100) - Main.screenPosition, null, Color.White, 0, tex.Size() / 2, 1, SpriteEffects.None, 0);
            spriteBatch.Reload(BlendState.AlphaBlend);
            spriteBatch.Reload(effect: null);
            return true;
        }
        void Reset()
        {
            NPC.noTileCollide = true;
            NPC.rotation = 0;
            NPC.velocity.X = 0;
            NPC.velocity.Y = 0;
            AITimer = 0;
            AITimer2 = 0;
            AITimer3 = 0;
            NPC.damage = 0;
        }
        void IdleMovement(Vector2 offset = default)
        {
            Player player = Main.player[NPC.target];
            //NPC.velocity *= 0.975f;
            NPC.velocity = Vector2.Clamp(Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 150) + offset, false) * 0.1f, -Vector2.One * 10, Vector2.One * 10);
        }
        void PlayerDetection()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            if (player.dead || !player.active || !player.ZoneUnderworldHeight)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (NPC.HasValidTarget)
                {
                    AIState = Spawn;
                    AITimer = 0;
                }
                if (player.dead || !player.active || !player.ZoneUnderworldHeight)
                {
                    NPC.velocity.Y = 30;
                    NPC.timeLeft = 10;
                    NPC.active = false;
                }
                return;
            }
        }
    }
}
