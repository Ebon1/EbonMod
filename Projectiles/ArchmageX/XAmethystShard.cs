using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using EbonianMod.Dusts;

namespace EbonianMod.Projectiles.ArchmageX
{
    public class XAmethystShard : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 2;
            Projectile.timeLeft = 600;
            Projectile.Size = new(10, 10);
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(3, 3), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(5, 5), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
            }
            SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            return false;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Texture2D glow = Helper.GetTexture(Texture + "_Glow");
            Main.spriteBatch.Reload(BlendState.Additive);
            for (int i = 0; i < 2; i++)
                Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, glow.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
