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
    public class BlackWhiteDust : ModDust
    {
        public override string Texture => "EbonianMod/Extras/Empty";
        public override void OnSpawn(Dust dust)
        {
            dust.alpha = 255;
            dust.noLight = false;
            dust.noGravity = true;
            dust.rotation = Main.rand.NextFloat(MathHelper.Pi * 2);
            base.OnSpawn(dust);

        }
        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.scale -= 0.01f;
            dust.rotation += MathHelper.ToRadians(Main.rand.NextFloat(-5, 5));
            dust.velocity *= 0.95f;
            if (dust.scale <= 0)
                dust.active = false;
            return false;
        }
        public static void DrawAll(SpriteBatch sb)
        {
            foreach (Dust d in Main.dust)
            {
                if (d.type == ModContent.DustType<BlackWhiteDust>() || d.type == ModContent.DustType<BlackWhiteDustExpand>() && d.active)
                {
                    Texture2D tex = ModContent.Request<Texture2D>("EbonianMod/Extras/explosion").Value;

                    if (d.customData != null)
                        tex = ModContent.Request<Texture2D>((string)d.customData).Value;

                    float alpha = d.scale / d.scale.Safe(0.001f);
                    if (d.scale == 0) alpha = 0;
                    if (d.type == ModContent.DustType<BlackWhiteDustExpand>())
                        alpha = MathHelper.SmoothStep(1.5f, 0, d.scale * 2);
                    if (d.scale > 0)
                        sb.Draw(tex, d.position - Main.screenPosition, null, Color.White * alpha, d.rotation, tex.Size() / 2, d.scale, SpriteEffects.None, 0);
                }
            }
        }
    }
    public class BlackWhiteDustExpand : ModDust
    {
        public override string Texture => "EbonianMod/Extras/Empty";
        public override void OnSpawn(Dust dust)
        {
            dust.alpha = 255;
            dust.noLight = false;
            dust.noGravity = true;
            dust.scale = 0;
            dust.rotation = Main.rand.NextFloat(MathHelper.Pi * 2);
            base.OnSpawn(dust);

        }
        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.scale += 0.01f;
            dust.rotation += MathHelper.ToRadians(Main.rand.NextFloat(-5, 5));
            dust.velocity *= 0.95f;
            if (dust.scale >= 0.5f)
                dust.active = false;
            return false;
        }
    }
}
