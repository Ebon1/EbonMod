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
    public class GenericAdditiveDust : ModDust
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
            dust.velocity *= 0.95f;
            if (dust.scale <= 0)
                dust.active = false;
            return false;
        }
        public static void DrawAll(SpriteBatch sb)
        {
            foreach (Dust d in Main.dust)
            {
                if (d.type == DustType<GenericAdditiveDust>() && d.active)
                {
                    Texture2D tex = Request<Texture2D>("EbonianMod/Extras/explosion").Value;
                    if (d.customData != null)
                        sb.Draw(tex, d.position - Main.screenPosition, null, Color.White * d.scale * 5, 0, tex.Size() / 2, d.scale * 0.85f * 2, SpriteEffects.None, 0);
                    sb.Draw(tex, d.position - Main.screenPosition, null, d.color * (d.customData != null ? ((int)d.customData == 2 ? d.scale * 10 : 1) : 1), 0, tex.Size() / 2, d.scale * 2, SpriteEffects.None, 0);
                }
            }
        }
    }
    public class SparkleDust : ModDust
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
            dust.velocity *= 0.95f;
            if (dust.scale <= 0)
                dust.active = false;
            return false;
        }
        public static void DrawAll(SpriteBatch sb)
        {
            foreach (Dust d in Main.dust)
            {
                if (d.type == DustType<SparkleDust>() && d.active)
                {
                    Texture2D tex = Request<Texture2D>("EbonianMod/Extras/crosslight").Value;
                    if (d.customData != null)
                        sb.Draw(tex, d.position - Main.screenPosition, null, Color.White * d.scale * 5, 0, tex.Size() / 2, d.scale * 0.85f * 2, SpriteEffects.None, 0);
                    sb.Draw(tex, d.position - Main.screenPosition, null, d.color * (d.customData != null ? ((int)d.customData == 2 ? d.scale * 10 : 1) : 1), 0, tex.Size() / 2, d.scale * 2, SpriteEffects.None, 0);
                }
            }
        }
    }
    public class LineDustFollowPoint : ModDust
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
            dust.rotation = dust.velocity.ToRotation() - MathHelper.PiOver2;
            if (dust.customData != null && dust.customData.GetType() == typeof(Vector2))
            {
                dust.velocity = Vector2.Lerp(dust.velocity, Helper.FromAToB(dust.position, (Vector2)dust.customData, false) / 25, 0.05f + dust.scale);
                if (dust.position.Distance((Vector2)dust.customData) < 100)
                    dust.scale -= 0.01f;
            }
            if (dust.scale <= 0)
                dust.active = false;
            return false;
        }
        public static void DrawAll(SpriteBatch sb)
        {
            foreach (Dust d in Main.dust)
            {
                if (d.type == DustType<LineDustFollowPoint>() && d.active)
                {
                    Texture2D tex = Request<Texture2D>("EbonianMod/Extras/Extras2/trace_01").Value;
                    sb.Draw(tex, d.position - Main.screenPosition, null, d.color * (d.scale * 10), d.rotation, tex.Size() / 2, d.scale * 2, SpriteEffects.None, 0);
                }
            }
        }
    }
}
