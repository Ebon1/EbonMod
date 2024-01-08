using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.GameContent;
using EbonianMod.Projectiles.Terrortoma;
using EbonianMod.Projectiles.Minions;

namespace EbonianMod.Projectiles.Friendly.Corruption
{
    public class TerrorStaffP : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(32);
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.aiStyle = 2;
        }
        /*public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            DrawData data = new DrawData(tex, Projectile.Center - Main.screenPosition, null, lightColor, 0, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            MiscDrawingMethods.DrawWithDye(Main.spriteBatch, data, ItemID.GreenFlameDye, Projectile);
            return false;
        }*/
        public override void Kill(int timeLeft)
        {
            //Helper.DustExplosion(Projectile.Center, Projectile.Size, 0, Color.Green);

            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Main.rand.NextVector2Unit() * 10, ModContent.ProjectileType<EbonFlyMinion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Helper.TRay.CastLength(Projectile.Center, -Vector2.UnitY, 1000) < Projectile.height)
            {
                Projectile.velocity.Y = 0;
                return false;
            }
            if ((Helper.TRay.CastLength(Projectile.Center, Vector2.UnitX, 1000) < Projectile.width || Helper.TRay.CastLength(Projectile.Center, -Vector2.UnitX, 1000) < Projectile.width) && Helper.TRay.CastLength(Projectile.Center, Vector2.UnitY, 1000) > Projectile.height)
            {
                Projectile.velocity.X = 0;
                return false;
            }
            Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Helper.TRay.Cast(Projectile.Center, Vector2.UnitY, Main.screenWidth), Vector2.Zero, ModContent.ProjectileType<TExplosion>(), 0, 0);
            Terraria.Audio.SoundEngine.PlaySound(EbonianSounds.eggplosion, Projectile.Center);
            return true;
        }
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 5 == 0)
            {
                if (Projectile.frame == 0) Projectile.frame++;
                else Projectile.frame = 0;
            }
            Projectile.velocity.Y += 0.25f;
        }
    }
}
