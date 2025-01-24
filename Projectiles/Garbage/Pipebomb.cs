using EbonianMod.Projectiles.VFXProjectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EbonianMod.Projectiles.Garbage
{
    public class Pipebomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 25;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = 2;
            AIType = ProjectileID.Shuriken;
            Projectile.width = 18;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 100;
            Projectile.height = 36;
        }
        float savedP;
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (Projectile.Center.Y >= savedP - 100)
                fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            Texture2D tex = Helper.GetTexture(Texture + "_Bloom");
            Main.spriteBatch.Reload(BlendState.Additive);
            var fadeMult = 1f / Projectile.oldPos.Count();
            for (int i = 0; i < Projectile.oldPos.Count(); i++)
            {
                float mult = (1 - i * fadeMult);
                Main.spriteBatch.Draw(tex, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.Maroon * (mult * 0.8f), Projectile.oldRot[i], tex.Size() / 2, Projectile.scale * 1.1f, SpriteEffects.None, 0);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Maroon * (0.5f), Projectile.rotation, tex.Size() / 2, Projectile.scale * (1 + (MathF.Sin(Main.GlobalTimeWrappedHourly * 3f) + 1) * 0.5f), SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void AI()
        {
            if (savedP == 0)
                savedP = Main.LocalPlayer.Center.Y;
            Projectile.tileCollide = Projectile.Center.Y > savedP - 20;
            if (Projectile.tileCollide)
            {
                Projectile.aiStyle = 14;
                AIType = ProjectileID.StickyGlowstick;
            }
            Dust.NewDustPerfect(Projectile.Center - new Vector2(-8, 15).RotatedBy(Projectile.rotation), DustID.Torch);
            if (Projectile.timeLeft == 30)
            {
                Projectile.NewProjectileDirect(NPC.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileType<CircleTelegraph>(), 0, 0);
                Projectile.velocity = Vector2.Zero;
                Projectile.aiStyle = 0;
            }
        }
        public override void Kill(int timeLeft)
        {
            Projectile a = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileType<FlameExplosionWSprite>(), Projectile.damage, 0, Projectile.owner);
        }
    }
}
