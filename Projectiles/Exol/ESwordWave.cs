using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Projectiles.Exol
{
    public class ESwordWave : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/Extras2/slash_03";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.Size = new Vector2(50, 50);
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        public override bool ShouldUpdatePosition()
        {
            return Projectile.ai[2] >= 1;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = 0;
        }
        public override void AI()
        {
            if (Projectile.ai[1] == 0)
                Projectile.ai[1] = Main.rand.Next(1, 3);
            Projectile.ai[2] += 0.03f;
            Projectile.scale = MathHelper.Min(Projectile.scale + 0.055f, 1);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.spriteBatch.Reload(BlendState.Additive);
            Rectangle rect = new Rectangle(0, 0, tex.Width, (int)(tex.Height * Projectile.scale));
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float fadeMult = 1f / ProjectileID.Sets.TrailCacheLength[Type];
                for (int j = 0; j < 10; j++)
                {
                    int k;
                    if (i == 0)
                        k = 0;
                    else
                        k = i - 1;
                    Vector2 lastP = Projectile.oldPos[k];

                    float alpha = MathHelper.Lerp((1f - fadeMult * i), (1f - fadeMult * k), (float)(j) / 10);
                    Vector2 pos = Vector2.Lerp(Projectile.oldPos[i], lastP, (float)(j) / 10);
                    Main.spriteBatch.Draw(tex, pos + (Projectile.Size / 2) - Main.screenPosition, rect, Color.OrangeRed * alpha * 0.5f, Projectile.rotation + (Projectile.ai[1] == 2 ? MathHelper.Pi : 0), tex.Size() / 2, 0.5f * alpha/*new Vector2(MathHelper.Clamp(Projectile.scale, 0, 0.5f), Projectile.scale / 2) * alpha*/, Projectile.ai[1] == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
                }

            }
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, rect, Color.OrangeRed, Projectile.rotation + (Projectile.ai[1] == 2 ? MathHelper.Pi : 0), tex.Size() / 2, 0.5f/*new Vector2(MathHelper.Clamp(Projectile.scale, 0, 0.5f), Projectile.scale / 2)*/, Projectile.ai[1] == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
    }
}
