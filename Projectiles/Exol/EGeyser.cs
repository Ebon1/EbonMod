using EbonianMod.NPCs.Garbage;
using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace EbonianMod.Projectiles.Exol
{
    public class EGeyser : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 44;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 350;
            Projectile.scale = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Warning").Value;
            Texture2D laser = Helper.GetExtraTexture("Extras2/trace_05");
            float scale = Math.Clamp(MathHelper.Lerp(0, 1, Projectile.scale * 2), 0, 1);
            //Rectangle frame = new Rectangle(0, Projectile.frame * 46, 30, 46);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(tex.Width / 2, Projectile.height), new Vector2(Projectile.scale, 1), SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.OrangeRed * Projectile.scale, Projectile.rotation, new Vector2(tex.Width / 2, Projectile.height), new Vector2(Projectile.scale, 1), SpriteEffects.None, 0);

            //Main.spriteBatch.Draw(laser, Projectile.Center + new Vector2(0, 40) - Main.screenPosition, null, Color.OrangeRed * Projectile.scale * Projectile.ai[1], Projectile.rotation, new Vector2(laser.Width / 2, laser.Height), new Vector2(Projectile.ai[1], 1), SpriteEffects.None, 0);
            //Main.spriteBatch.Draw(laser, Projectile.Center + new Vector2(0, 40) - Main.screenPosition, null, Color.OrangeRed * Projectile.scale * Projectile.ai[1], Projectile.rotation, new Vector2(laser.Width / 2, laser.Height), new Vector2(Projectile.ai[1], 1), SpriteEffects.FlipVertically, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override bool? CanDamage() => false;
        /*public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            return Projectile.ai[1] > 0.9f && Collision.CheckAABBvLineCollision(targetHitbox.Center(), targetHitbox.Size(), Projectile.Center, Projectile.Center - Vector2.UnitY * 512, 20, ref a);
        }*/
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.Center = Helper.TRay.Cast(Projectile.Center, Vector2.UnitY, 1000) - new Vector2(0, 44);
                Projectile.ai[0] = 1;
            }
            if (Projectile.timeLeft == 280)
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Bottom, Vector2.Zero, ModContent.ProjectileType<InferosShockwave2>(), 0, 0);
            if (Projectile.timeLeft < 250)
            {
                if (++Projectile.ai[1] % 40 == 0)
                {
                    Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, -Vector2.UnitY, ProjectileID.GeyserTrap, Projectile.damage, Projectile.knockBack).friendly = false;
                    if (Projectile.ai[1] > 40)
                        Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center - new Vector2(0, 300), -Vector2.UnitY, ProjectileID.GeyserTrap, Projectile.damage, Projectile.knockBack).friendly = false;
                }
            }

            float progress = Utils.GetLerpValue(0, 350, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 5, 0, 1);
        }
    }
}
