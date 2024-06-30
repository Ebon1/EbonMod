using EbonianMod.Dusts;
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

namespace EbonianMod.Projectiles.ArchmageX
{
    public class XTelegraphLine : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(5, 5);
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 40;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Reload(BlendState.Additive);
            if (Projectile.ai[0] > 0)
            {
                Texture2D tex = Helper.GetExtraTexture("laser4");
                Vector2 pos = Projectile.Center;
                Vector2 scale = new Vector2(1f, Projectile.ai[1]);
                float eAlpha = MathHelper.Lerp(1, 0, Projectile.ai[2]);
                for (float i = 0; i < Projectile.ai[0]; i++)
                {
                    float x = MathHelper.Clamp(MathHelper.SmoothStep(1, 0, (i / Projectile.ai[0]) * 5), 0, 1);
                    float f = MathHelper.Lerp(50, 0, x);
                    float alpha = MathHelper.Lerp(1, 0, x);
                    Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, Color.Lerp(Color.Indigo, Color.Violet, i / Projectile.ai[0]) * (Projectile.ai[1] * alpha), Projectile.rotation, new Vector2(0, tex.Height / 2), scale, SpriteEffects.None, 0);
                    pos += Projectile.rotation.ToRotationVector2();

                    for (int j = -1; j < 2; j++)
                    {
                        if (j == 0) continue;

                        Main.spriteBatch.Draw(tex, pos + Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2) * (f * Projectile.ai[2] * j) - Main.screenPosition, null, Color.Lerp(Color.Indigo, Color.Violet, i / Projectile.ai[0]) * (eAlpha * alpha), Projectile.rotation, new Vector2(0, tex.Height / 2), scale, SpriteEffects.None, 0);
                    }
                }
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        public override void AI()
        {
            float progress = Utils.GetLerpValue(0, 40, Projectile.timeLeft);
            Projectile.ai[1] = MathHelper.Clamp(MathF.Sin(progress * MathF.PI) * 0.5f, 0, 1);
            Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], 1, 0.1f);

            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[0] == 0)
                Projectile.ai[0] = Helper.TRay.CastLength(Projectile.Center, Projectile.rotation.ToRotationVector2(), 2000);
        }
    }
}
