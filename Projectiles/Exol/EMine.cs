using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Projectiles.Exol
{
    internal class EMine : ModProjectile
    {
        public override string Texture => "EbonianMod/Projectiles/Exol/EBoulder";
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.hostile = true;
            Projectile.timeLeft = 450;
        }
        public override bool? CanDamage() => Projectile.ai[0] == 0;
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center + oldVelocity, Vector2.Zero, ModContent.ProjectileType<Noise>(), 0, 0);
                Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center + oldVelocity, Vector2.Zero, ModContent.ProjectileType<NoiseOverlay>(), 0, 0);
                Projectile.ai[0] = 1;
                Projectile.Center += oldVelocity;
                Projectile.velocity = Vector2.Zero;
                pos = Projectile.Center;
                Projectile.timeLeft = 285;
            }
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = Helper.GetTexture("Projectiles/Exol/EBoulder_Bloom");
            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.OrangeRed * Projectile.ai[2], Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
        }
        public Vector2 pos;
        public override void AI()
        {
            if (Projectile.ai[0] != 0)
            {
                if (Projectile.ai[2] < 1)
                    Projectile.ai[2] += 0.01f;
                if (Projectile.ai[2] > 0.65f)
                {
                    Projectile.Center = pos + Main.rand.NextVector2Circular(3 * Projectile.ai[2], 2 * Projectile.ai[2]);
                }
                if (Projectile.timeLeft == 20)
                    Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<QuickFlare>(), 0, 0);
            }
            else
            {
                if (Projectile.velocity.Length() < 20)
                    Projectile.velocity *= 1.05f;
            }
        }
        public override void Kill(int timeLeft)
        {
            if (Projectile.ai[0] != 0)
            {
                for (int i = 0; i < 25; i++)
                {
                    Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(), Main.rand.Next(new int[] { GoreID.Smoke1, GoreID.Smoke2, GoreID.Smoke3 }), Main.rand.NextFloat(0.25f, 1f));
                }
                Helper.DustExplosion(Projectile.Center, Projectile.Size, 0, Color.OrangeRed);
                SoundEngine.PlaySound(new SoundStyle("EbonianMod/Sounds/genericexplosion"), Projectile.Center);
                Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FlameExplosion>(), Projectile.damage * 2, 0);
                Projectile a = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileID.DaybreakExplosion, Projectile.damage * 2, 0);
                a.hostile = true;
                a.friendly = false;
            }
        }
    }
}
