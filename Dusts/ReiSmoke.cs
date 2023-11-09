using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace EbonianMod.Dusts
{
    public class ReiSmoke : ModDust
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
            dust.scale -= 0.0005f;
            dust.velocity *= 0.95f;
            if (dust.scale < 0.005f)
                dust.velocity *= 0.85f;
            if (dust.scale <= 0)
                dust.active = false;
            return false;
        }
        public static void DrawAll(SpriteBatch sb)
        {
            foreach (Dust d in Main.dust)
            {
                if (d.type == ModContent.DustType<ReiSmoke>() && d.active)
                {
                    Texture2D tex = ModContent.Request<Texture2D>("EbonianMod/Extras/explosion").Value;
                    sb.Draw(tex, d.position - Main.screenPosition, null, Color.White * d.scale * 10, 0, tex.Size() / 2, d.scale, SpriteEffects.None, 0);
                }
            }
        }
    }
}
