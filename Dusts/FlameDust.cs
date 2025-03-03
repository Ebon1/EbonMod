using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbonianMod.Dusts
{
    public class FlameDust : ModDust
    {
        public override string Texture => "EbonianMod/Extras/Empty";
        public override void OnSpawn(Dust dust)
        {
            dust.alpha = 255;
            dust.noLight = true;
            dust.noGravity = true;
            base.OnSpawn(dust);

        }
        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.scale -= 0.01f;
            dust.velocity *= 0.95f;
            if (dust.scale <= 0)
                dust.active = false;
            return false;
        }
        public static void DrawAll(SpriteBatch sb, Dust d)
        {
            if (d.type == DustType<FlameDust>() && d.active)
            {
                Texture2D tex = ExtraTextures.explosion;
                sb.Draw(tex, d.position - Main.screenPosition, null, Color.White * Clamp(d.scale, 0, 2), 0, tex.Size() / 2, d.scale * 0.1f, SpriteEffects.None, 0);
            }
        }
    }
}
