using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace EbonianMod.Dusts
{
    public class BlurDust : ModDust
    {
        public override string Texture => "EbonianMod/Extras/Empty";
        public override void OnSpawn(Dust dust)
        {
            dust.alpha = 255;
            dust.noLight = true;
            dust.noGravity = true;
            //if (dust.scale <= 1f && dust.scale >= 0.8f)
            //  dust.scale = 0.25f;
            base.OnSpawn(dust);

        }
        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.scale -= 0.005f;
            dust.rotation = dust.velocity.ToRotation();
            dust.velocity *= 0.95f;
            dust.fadeIn = Lerp(dust.fadeIn, 1, 0.1f);
            if (dust.scale <= 0)
                dust.active = false;
            return false;
        }
        public static void DrawAll(SpriteBatch sb, Dust d)
        {
            if (d.type == DustType<BlurDust>() && d.active)
            {
                Texture2D tex = ExtraTextures2.fire_01_normal;
                for (float i = 0; i < 30 * d.fadeIn; i++)
                {
                    sb.Draw(tex, d.position - d.velocity * d.scale * i * 2 - Main.screenPosition, null, d.color * d.scale * SmoothStep(2, 0, i / 30f), 0, tex.Size() / 2, d.scale * 0.85f * 2, SpriteEffects.None, 0);
                }
                sb.Draw(tex, d.position - Main.screenPosition, null, d.color * d.scale * d.fadeIn, 0, tex.Size() / 2, d.scale * 0.85f * 2, SpriteEffects.None, 0);
            }
        }
    }
}
