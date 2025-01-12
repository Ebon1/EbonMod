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
    public class FireDust : ModDust
    {
        public override string Texture => "EbonianMod/Extras/Empty";
        public override void OnSpawn(Dust dust)
        {
            dust.alpha = 255;
            dust.noLight = true;
            dust.noGravity = true;
            dust.customData = Main.rand.Next(1, 3);
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
        public static void DrawAll(SpriteBatch sb)
        {
            foreach (Dust d in Main.dust)
            {
                if (d.type == ModContent.DustType<FireDust>() && d.active)
                {
                    Texture2D tex = ModContent.Request<Texture2D>("EbonianMod/Extras/Extras2/fire_0" + d.customData).Value;
                    sb.Draw(tex, d.position - Main.screenPosition, null, Color.White, 0, tex.Size() / 2, d.scale * 0.85f * 2, SpriteEffects.None, 0);
                    sb.Draw(tex, d.position - Main.screenPosition, null, Color.OrangeRed, 0, tex.Size() / 2, d.scale * 2, SpriteEffects.None, 0);
                }
            }
        }
    }
    public class ColoredFireDust : ModDust
    {
        public override string Texture => "EbonianMod/Extras/Empty";
        public override void OnSpawn(Dust dust)
        {
            dust.alpha = 255;
            dust.noLight = true;
            dust.noGravity = true;
            if (dust.scale > 0.25f)
                dust.scale = 0.25f;
            dust.customData = Main.rand.Next(1, 3);
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
        public static void DrawAll(SpriteBatch sb)
        {
            foreach (Dust d in Main.dust)
            {
                if (d.type == ModContent.DustType<ColoredFireDust>() && d.active)
                {
                    Texture2D tex = ModContent.Request<Texture2D>("EbonianMod/Extras/Extras2/fire_0" + d.customData).Value;
                    sb.Draw(tex, d.position - Main.screenPosition, null, Color.White * d.scale * 5, 0, tex.Size() / 2, d.scale * 0.85f * 2, SpriteEffects.None, 0);
                    sb.Draw(tex, d.position - Main.screenPosition, null, d.color * d.scale * 10, 0, tex.Size() / 2, d.scale * 2, SpriteEffects.None, 0);
                }
            }
        }
    }
    public class SmokeDustAkaFireDustButNoGlow : ModDust
    {
        public override string Texture => "EbonianMod/Extras/Empty";
        public override void OnSpawn(Dust dust)
        {
            dust.alpha = 255;
            dust.noLight = true;
            dust.noGravity = true;
            //if (dust.scale > 0.35f)
            dust.rotation = Main.rand.NextFloat(MathHelper.Pi * 2);
            dust.scale = 0;
            dust.customData = Main.rand.Next(1, 3);
            base.OnSpawn(dust);

        }
        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.scale += 0.01f;
            dust.velocity *= 0.95f;
            if (dust.scale >= 0.35f)
                dust.active = false;
            return false;
        }
        public static void DrawAll(SpriteBatch sb)
        {
            foreach (Dust d in Main.dust)
            {
                if (d.type == ModContent.DustType<SmokeDustAkaFireDustButNoGlow>() && d.active)
                {
                    float alpha = MathHelper.Lerp(1, 0, d.scale * 2.857142857142857f);
                    Texture2D tex = ModContent.Request<Texture2D>("EbonianMod/Extras/Extras2/fire_0" + d.customData).Value;
                    sb.Draw(tex, d.position - Main.screenPosition, null, d.color * alpha, d.rotation, tex.Size() / 2, d.scale * 2, SpriteEffects.None, 0);
                }
            }
        }
    }
}
