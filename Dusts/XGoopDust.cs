using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace EbonianMod.Dusts
{
    public class XGoopDust : ModDust
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
            dust.scale -= 0.04f;
            dust.velocity *= 0.95f;
            dust.rotation += MathHelper.ToRadians(MathHelper.Clamp(dust.scale * 2, 0, 2));
            if (dust.scale <= 0)
                dust.active = false;
            return false;
        }
        public static void DrawAll(SpriteBatch sb)
        {
            foreach (Dust d in Main.dust)
            {
                if ((d.type == DustType<XGoopDust>() || d.type == DustType<XGoopDust2>()) && d.active)
                {
                    Texture2D tex = ExtraTextures2.fire_01;
                    sb.Draw(tex, d.position - Main.screenPosition, null, (d.customData == null ? Color.White : d.color) * MathHelper.Clamp(d.scale * 2, 0, 0.5f), d.rotation, tex.Size() / 2, d.scale * 0.2f, SpriteEffects.None, 0);
                    sb.Draw(tex, d.position - Main.screenPosition, null, (d.customData == null ? Color.White : d.color) * MathHelper.Clamp(d.scale * 2, 0, 0.5f), -d.rotation, tex.Size() / 2, d.scale * 0.2f, SpriteEffects.None, 0);
                }
            }
        }
    }
    public class XGoopDust2 : ModDust
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
            dust.scale -= 0.01f;
            dust.velocity *= 0.95f;
            dust.rotation += MathHelper.ToRadians(MathHelper.Clamp(dust.scale * 2, 0, 2));
            if (dust.scale <= 0)
                dust.active = false;
            return false;
        }
    }
    public class XGoopDustDark : ModDust
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
                if (d.type == DustType<XGoopDustDark>() && d.active)
                {
                    Texture2D tex = ExtraTextures.Spotlight;
                    sb.Draw(tex, d.position - Main.screenPosition, null, (d.customData == null ? Color.White : d.color) * MathHelper.Clamp(d.scale * 2, 0, 1), 0, tex.Size() / 2, d.scale, SpriteEffects.None, 0);
                }
            }
        }
    }
}
