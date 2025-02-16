using Terraria.ModLoader;
using Terraria;
using EbonianMod.Projectiles.Enemy.Desert;

namespace EbonianMod.NPCs.Desert
{
    public class CoinBag : ModNPC
    {
        public override string Texture => Helper.Empty;

        public override void SetDefaults()
        {
            NPC.width = 64;
            NPC.height = 64;

            //these to be tuned
            NPC.damage = 30;
            NPC.defense = 0;
            NPC.lifeMax = 300;

            NPC.HitSound = SoundID.CoinPickup;
            NPC.DeathSound = SoundID.NPCDeath52;
            NPC.value = 100000f; //10 gold? idk i dont use floats for value
        }

        int frame = 0;
        int frameCD = 0;

        int hopCD = 0;

        int attacking = 0;

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player t = Main.player[NPC.target];

            if (attacking > 0)
            {
                attacking--;
            }

            if (hopCD > 0)
            {
                hopCD--;
            }

            if (frameCD > 0)
            {
                frameCD--;
            }

            Vector2 tileCoords = new Vector2(NPC.position.X, NPC.Center.Y + 32);
            if (NPC.collideY)
            {
                NPC.velocity.X = 0f;

                if (frameCD == 0)
                {
                    if (attacking == 0)
                    {
                        if (hopCD == 0)
                        {
                            if (frame > 64 * 2)
                            {
                                frame = 0;
                            }

                            if (frame < 64 * 2)
                            {
                                frame += 64;
                            }
                            else
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X, NPC.Center.Y - 16), new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, -2f)), ModContent.ProjectileType<Coins>(), NPC.damage, 0f, Main.myPlayer);
                                SoundEngine.PlaySound(SoundID.Coins, NPC.Center);

                                NPC.velocity.Y = -6f;
                                NPC.velocity.X = NPC.direction == 1 ? 6f : -6f;

                                hopCD = 80;
                            }
                        }
                        else
                        {
                            if (frame == 64 * 2)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X, NPC.Center.Y - 16), new Vector2(Main.rand.NextFloat(-1f, 1f), -0.5f), ModContent.ProjectileType<Coins>(), NPC.damage, 0f, Main.myPlayer);
                                SoundEngine.PlaySound(SoundID.Coins, NPC.Center);
                            }

                            if (frame < 64 * 6)
                            {
                                frame += 64;
                            }
                            else
                            {
                                if (Vector2.Distance(t.Center, NPC.Center) < 175)
                                {
                                    attacking = 150;
                                    frame = 0;
                                }
                            }
                        }


                        frameCD = 5;
                    }
                    else
                    {
                        if (frame == 64 * 2 || frame == 64 * 3)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X, NPC.Center.Y - 16), new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-4f, -6f)), ModContent.ProjectileType<Coins>(), NPC.damage, 0f, Main.myPlayer);
                            SoundEngine.PlaySound(SoundID.Coins, NPC.Center);
                        }

                        if (frame < 64 * 3)
                        {
                            frame += 64;
                        }
                        else
                        {
                            frame = 0;
                        }

                        frameCD = 5;
                    }
                }
            }
        }

        public override bool CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X, NPC.Center.Y - 16), new Vector2(Main.rand.NextFloat(-1f, 1f), -0.5f), ModContent.ProjectileType<Coins>(), NPC.damage, 0f, Main.myPlayer);
            SoundEngine.PlaySound(SoundID.Coins, NPC.Center);

            if (NPC.life <= 0)
            {
                int r = Main.rand.Next(5, 7);
                int smokeR = Main.rand.Next(1, 4);
                for (int i = 0; i < r; i++)
                {
                    Gore.NewGorePerfect(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)), smokeR == 1 ? GoreID.Smoke1 : smokeR == 2 ? GoreID.Smoke2 : GoreID.Smoke3, Scale: 1.2f);
                }

            }
        }

        public override void OnKill()
        {
            Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X, NPC.Center.Y - 16), new Vector2(4f, -3f), ModContent.ProjectileType<Coins>(), NPC.damage, 0f, Main.myPlayer);
            Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X, NPC.Center.Y - 16), new Vector2(2f, -4f), ModContent.ProjectileType<Coins>(), NPC.damage, 0f, Main.myPlayer);
            Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X, NPC.Center.Y - 16), new Vector2(0f, -5f), ModContent.ProjectileType<Coins>(), NPC.damage, 0f, Main.myPlayer);
            Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X, NPC.Center.Y - 16), new Vector2(-2f, -4f), ModContent.ProjectileType<Coins>(), NPC.damage, 0f, Main.myPlayer);
            Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X, NPC.Center.Y - 16), new Vector2(-4f, -3f), ModContent.ProjectileType<Coins>(), NPC.damage, 0f, Main.myPlayer);
            SoundEngine.PlaySound(SoundID.Coins, NPC.Center);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex;

            if (attacking > 0)
            {
                tex = Helper.GetTexture(Mod.Name + "/NPCs/Desert/CoinBagShake");
            }
            else
            {
                tex = Helper.GetTexture(Mod.Name + "/NPCs/Desert/CoinBagHop");
            }

            Rectangle sourceRect = new Rectangle(0, frame, 64, 64);

            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition, sourceRect, drawColor, NPC.rotation, sourceRect.Size() / 2f, NPC.scale, NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);

            return false;
        }
    }
}
