using Terraria.ModLoader;

namespace EbonianMod.Projectiles.Enemy.Desert
{
    public class Coins : ModProjectile
    {
        public override string Texture => Helper.Empty;

        public override void SetDefaults()
        {
            Projectile.height = 13;
            Projectile.width = 12;

            Projectile.hostile = true;
            Projectile.aiStyle = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Coin = 2;

            if (Main.rand.NextBool(5))
            {
                Coin = 1;
            }
        }

        int Coin;
        int frame;
        int frameCD;

        public override void AI()
        {
            //Projectile.rotation = Projectile.velocity.ToRotation();

            if (frameCD > 0)
            {
                frameCD--;
            }

            if (frameCD == 0)
            {
                if (Coin == 0)
                {
                    if (frame < 12 * 7)
                    {
                        frame += 12;
                    }
                    else
                    {
                        frame = 0;
                    }
                }

                if (Coin == 1)
                {
                    if (frame < 14 * 8)
                    {
                        frame += 14;
                    }
                    else
                    {
                        frame = 0;
                    }
                }

                if (Coin == 2)
                {
                    if (frame < 16 * 9)
                    {
                        frame += 16;
                    }
                    else
                    {
                        frame = 0;
                    }
                }

                frameCD = 3;
            }

            if (Main.rand.NextBool(16))
            {
                Dust.NewDust(Projectile.Center, 1, 1, Coin == 0 ? DustID.CopperCoin : Coin == 1 ? DustID.SilverCoin : DustID.GoldCoin, SpeedX: 0f, SpeedY: 0f, Scale: 0.1f);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDust(Projectile.Center, 1, 1, Coin == 0 ? DustID.CopperCoin : Coin == 1 ? DustID.SilverCoin : DustID.GoldCoin, SpeedX: Main.rand.NextFloat(-0.5f, 0.5f), SpeedY: Main.rand.NextFloat(-0.5f, 0.5f), Scale: 0.1f);
            }
            //SoundEngine.PlaySound(SoundID.Coins, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Helper.GetTexture(Mod.Name + "/Projectiles/Enemy/Desert/CoinSheet");
            Rectangle sourceRect = new Rectangle(0, 0, 0, 0);

            if (Coin == 0)
            {
                sourceRect = new Rectangle(0, frame, 12, 12);
            }

            if (Coin == 1)
            {
                sourceRect = new Rectangle(12, frame, 12, 14);
            }

            if (Coin == 2)
            {
                sourceRect = new Rectangle(24, frame, 12, 16);
            }

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, sourceRect, Color.White, Projectile.rotation, sourceRect.Size() / 2f, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
