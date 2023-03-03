using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using EbonianMod.Projectiles.VFXProjectiles;

namespace EbonianMod.NPCs.Garbage
{
    public class HotGarbage : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 13;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }
        public override void SetDefaults()
        {
            NPC.width = 80;
            NPC.height = 56;
            NPC.damage = 0;
            NPC.defense = 10;
            NPC.lifeMax = 3500;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.knockBackResist = 0f;
            NPC.DeathSound = new Terraria.Audio.SoundStyle("EbonianMod/Sounds/NPCHit/GarbageDeath");
            NPC.aiStyle = -1;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.boss = true;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Garbage");
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Type: Dumpster"),
                new FlavorTextBestiaryInfoElement("Literal flaming garbage. Why do you need a book to tell you that? Maybe you should jump in."),
            });
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 pos, Color lightColor)
        {
            Texture2D drawTexture = Helper.GetTexture("NPCs/Garbage/HotGarbage");
            Texture2D glow = Helper.GetTexture("NPCs/Garbage/HotGarbage_Glow");
            Texture2D fire = Helper.GetTexture("NPCs/Garbage/HotGarbage_Fire");
            Vector2 origin = new Vector2((drawTexture.Width / 2) * 0.5F, (drawTexture.Height / Main.npcFrameCount[NPC.type]) * 0.5F);

            Vector2 drawPos = new Vector2(
                NPC.position.X - pos.X + (NPC.width / 2) - (Helper.GetTexture("NPCs/Garbage/HotGarbage").Width / 2) * NPC.scale / 2f + origin.X * NPC.scale,
                NPC.position.Y - pos.Y + NPC.height - Helper.GetTexture("NPCs/Garbage/HotGarbage").Height * NPC.scale / Main.npcFrameCount[NPC.type] + 4f + origin.Y * NPC.scale + NPC.gfxOffY);

            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(drawTexture, drawPos, NPC.frame, lightColor, NPC.rotation, origin, NPC.scale, effects, 0);
            spriteBatch.Draw(glow, drawPos, NPC.frame, Color.White, NPC.rotation, origin, NPC.scale, effects, 0);
            if (AIState != Intro && AIState != Idle)
                spriteBatch.Draw(fire, drawPos + new Vector2(NPC.width * -NPC.direction + (NPC.direction == 1 ? 9 : 0), 2).RotatedBy(NPC.rotation), new Rectangle(0, NPC.frame.Y - 58 * 3, 70, 58), Color.White, NPC.rotation, origin, NPC.scale, effects, 0);
            return false;
        }
        public override void FindFrame(int f)
        {
            int frameHeight = 58;
            NPC.frame.Width = 80;
            NPC.frame.Height = 58;
            NPC.frame.X = AIState == Intro && !NPC.IsABestiaryIconDummy ? 0 : 80;
            NPC.frameCounter++;

            if (NPC.IsABestiaryIconDummy)
            {
                if (NPC.frameCounter % 5 == 0)
                {
                    if (NPC.frame.Y < 2 * frameHeight)
                    {
                        NPC.frame.Y += frameHeight;
                    }
                    else
                    {
                        NPC.frame.Y = 0;
                    }
                }
            }

            if (AIState == Intro && !NPC.IsABestiaryIconDummy)
            {
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
                else if (NPC.frameCounter < 25)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
                else if (NPC.frameCounter < 30)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }
                else if (NPC.frameCounter < 35)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }
                else if (NPC.frameCounter < 40)
                {
                    NPC.frame.Y = 7 * frameHeight;
                }
                else if (NPC.frameCounter < 45)
                {
                    NPC.frame.Y = 8 * frameHeight;
                }
                else if (NPC.frameCounter < 50)
                {
                    NPC.frame.Y = 9 * frameHeight;
                }
                else if (NPC.frameCounter < 55)
                {
                    NPC.frame.Y = 10 * frameHeight;
                }
                else if (NPC.frameCounter < 60)
                {
                    NPC.frame.Y = 11 * frameHeight;
                }
                else if (NPC.frameCounter < 65)
                {
                    NPC.frame.Y = 12 * frameHeight;
                }
                else
                {
                    NPC.frameCounter = 0;
                    if (!NPC.IsABestiaryIconDummy)
                    {
                        NPC.Center += new Vector2(2 * NPC.direction, 0);
                        NPC.frame.X = 80;
                        NPC.frame.Y = 0;
                        AIState = Idle;
                        AITimer = 0;
                        NextAttack = 2;
                    }
                }
            }
            else if (AIState == Idle)
            {
                if (NPC.frameCounter % 5 == 0)
                {
                    if (NPC.frame.Y < 2 * frameHeight)
                    {
                        NPC.frame.Y += frameHeight;
                    }
                    else
                    {
                        NPC.frame.Y = 0;
                    }
                }
            }
            else if (AIState == WarningForDash || AIState == SlamPreperation)
            {
                if (NPC.frameCounter % 5 == 0)
                {
                    if (NPC.frame.Y < 5 * frameHeight)
                    {
                        NPC.frame.Y += frameHeight;
                    }
                    else if (NPC.frame.Y >= 5 * frameHeight || NPC.frame.Y < 3 * frameHeight)
                    {
                        NPC.frame.Y = 3 * frameHeight;
                    }
                }
            }
            else if (AIState == SlamSlamSlam || AIState == Dash)
            {
                if (NPC.frameCounter % 5 == 0)
                {
                    if (NPC.frame.Y < 9 * frameHeight)
                    {
                        NPC.frame.Y += frameHeight;
                    }
                    else if (NPC.frame.Y >= 9 * frameHeight || NPC.frame.Y < 6 * frameHeight)
                    {
                        NPC.frame.Y = 6 * frameHeight;
                    }
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
        public float AITimer3
        {
            get => NPC.ai[3];
            set => NPC.ai[3] = value;
        }
        const int Death = -1, Intro = 0, Idle = 1, WarningForDash = 2, Dash = 3, SlamPreperation = 4, SlamSlamSlam = 5;
        int NextAttack = 2;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (NPC.HasValidTarget)
                {
                    AIState = Intro;
                    AITimer = 0;
                }
                if (!player.active || player.dead)
                {
                    if (NPC.timeLeft > 10)
                    {
                        NPC.timeLeft = 10;
                    }
                    NPC.timeLeft = 1;
                    return;
                }
            }
            if (AIState == Intro)
            {
                if (!NPC.collideY)
                {
                    NPC.frameCounter = 0;
                }
                else
                {
                    AITimer++;
                    if (AITimer == 1)
                    {
                        EbonianSystem.ChangeCameraPos(NPC.Center, 95);
                    }
                    if (AITimer < 30)
                    {

                        NPC.frameCounter = 0;
                    }
                }
            }
            else if (AIState == Idle)
            {
                AITimer++;
                NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.35f);
                NPC.spriteDirection = NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                if (NPC.collideY && NPC.collideX)
                    NPC.velocity.Y = -10;
                NPC.velocity.X = Helper.FromAToB(NPC.Center, player.Center).X * 3;
                if (AITimer >= 100) //change this back to 300 once testing is over.
                {
                    NPC.velocity.X = 0;
                    AITimer = 0;
                    AIState = NextAttack;
                }
            }
            else if (AIState == WarningForDash)
            {
                AITimer++;
                NPC.spriteDirection = NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                if (AITimer >= 100)
                {
                    NPC.velocity.X = 0;
                    AITimer = 0;
                    AITimer2 = 0;
                    AIState = Dash;
                }
            }
            else if (AIState == Dash)
            {
                //old code i refuse to even look at, it works.
                NPC.damage = 30;
                AITimer++;
                int num899 = 80;
                int num900 = 80;
                Vector2 position5 = new Vector2(NPC.Center.X - (float)(num899 / 2), NPC.position.Y + (float)NPC.height - (float)num900);
                if (Collision.SolidCollision(position5, num899, num900))
                {
                    NPC.velocity.Y = -5.75f;
                }
                if (AITimer3 < 22)
                {
                    NPC.velocity.X = 9.5f * NPC.direction;
                }
                if (AITimer3 >= 22)
                {
                    NPC.velocity *= 0.96f;
                }
                if (AITimer3 >= 22 && AITimer3 < 40)
                {
                    for (int i = -2; i < 2; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(2, 4) * i, NPC.height / 2 - 2), new Vector2(-NPC.direction, -2), ModContent.ProjectileType<GarbageFlame>(), 15, 0);
                    }
                }
                if (AITimer3 >= 40 && AITimer % 5 == 0)
                {
                    NPC.spriteDirection = Main.player[NPC.target].Center.X > NPC.Center.X ? 1 : -1;
                    NPC.direction = Main.player[NPC.target].Center.X > NPC.Center.X ? 1 : -1;
                }
                if (++AITimer3 >= 65)
                {
                    AITimer3 = 0;
                }
                if (AITimer >= 65 * 3)
                {
                    NPC.velocity = Vector2.Zero;
                    NextAttack = SlamPreperation;
                    AIState = Idle;
                    AITimer = 0;
                    AITimer2 = 0;
                    NPC.damage = 0;
                    AITimer3 = 0;
                    NPC.direction = 1;
                }
            }
            else if (AIState == SlamPreperation)
            {
                AITimer++;
                NPC.rotation += MathHelper.ToRadians(-0.9f * 4 * NPC.direction);
                if (AITimer >= 25)
                {
                    NPC.velocity.X = 0;
                    AITimer = 0;
                    AITimer2 = 0;
                    AIState = SlamSlamSlam;
                }
            }
            else if (AIState == SlamSlamSlam)
            {
                AITimer++;
                if (AITimer < 50)
                    NPC.velocity.Y--;
                if (AITimer >= 50 && AITimer < 200)
                {
                    NPC.direction = NPC.spriteDirection = 1;
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, MathHelper.ToRadians(90), 0.15f);
                    if (AITimer % 5 == 0)
                        NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 500)) * 5;
                }
                if (AITimer == 200)
                {

                    for (int i = -3; i < 3; i++)
                    {
                        for (int j = 0; j < 2; j++)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(3 * i, 10), ModContent.ProjectileType<GarbageFlame>(), 15, 0, ai0: 1);
                    }
                    NPC.velocity = new Vector2(0, 65);
                }
                if (NPC.collideY && AITimer2 == 0 && AITimer >= 200 && NPC.Center.Y => player.Center.Y)
                {
                    Projectile a = Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GenericExplosion>(), 0, 0);
                    AITimer2 = 1;
                }
                if (AITimer2 >= 1)
                    AITimer2++;
                if (AITimer2 >= 100)
                {
                    NPC.velocity = Vector2.Zero;
                    AITimer = 0;
                    NextAttack = WarningForDash;
                    AIState = Idle;
                }
            }
        }
    }
    public class GarbageFlame : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flame");
        }

        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.aiStyle = 14;
            AIType = ProjectileID.StickyGlowstick;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 650;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (Projectile.Center.Y => Main.LocalPlayer.Center.Y)
                fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public override void AI()
        {
            if (Projectile.velocity.Y > 2.8f && Projectile.ai[0] == 0)
            {
                Projectile.velocity *= 0.87f;
            }
            if (Main.rand.Next(5) == 0)
            {
                for (int dustNumber = 0; dustNumber < 5; dustNumber++)
                {
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 6, 0, 0, 0, default(Color), 1f)];
                    dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(4.1887903213500977) * new Vector2(Projectile.width / 2, Projectile.height / 2) * 0.8f * (0.8f + Main.rand.NextFloat() * 0.2f);
                    dust.velocity.X = Main.rand.NextFloat(-0.5f, 0.5f);
                    dust.velocity.Y = -2f;
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(0.65f, 1.25f);
                }
            }
        }
    }
}