using EbonianMod.Common.Systems;
using EbonianMod.Projectiles.Terrortoma;
using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Projectiles.Cecitior
{
    public class CecitiorBombThing : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(32);
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.aiStyle = 2;
        }
        /*public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.Black * 0.1f, Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None);
            Main.spriteBatch.Reload(BlendState.Additive);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.Gold * 0.7f, Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White * 0.3f, Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }*/
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Helper.TRay.Cast(Projectile.Center, Vector2.UnitY, Main.screenWidth), Vector2.Zero, ProjectileType<IchorExplosion>(), Projectile.damage, 0);
            Terraria.Audio.SoundEngine.PlaySound(EbonianSounds.eggplosion, Projectile.Center);
            return true;
        }
        public override void AI()
        {
            if (Projectile.timeLeft % 5 == 0)
                for (int i = 0; i < 3; i++)
                {
                    Vector2 offset = Main.rand.NextVector2CircularEdge(15, 15);
                    Dust.NewDustPerfect(Projectile.Center + offset, DustID.IchorTorch, Helper.FromAToB(Projectile.Center + offset, Projectile.Center));
                }
        }
    }
}
