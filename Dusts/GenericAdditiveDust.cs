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
using Terraria.Graphics.CameraModifiers;
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
        public static void DrawAll(SpriteBatch sb, Dust d)
        {
            if (d.type == DustType<GenericAdditiveDust>() && d.active)
            {
                Texture2D tex = ExtraTextures.explosion;
                if (d.customData != null)
                    sb.Draw(tex, d.position - Main.screenPosition, null, Color.White * d.scale * 5, 0, tex.Size() / 2, d.scale * 0.85f * 2, SpriteEffects.None, 0);
                sb.Draw(tex, d.position - Main.screenPosition, null, d.color * (d.customData != null ? ((int)d.customData == 2 ? d.scale * 10 : 1) : 1), 0, tex.Size() / 2, d.scale * 2, SpriteEffects.None, 0);
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
        public static void DrawAll(SpriteBatch sb, Dust d)
        {
            if (d.type == DustType<SparkleDust>() && d.active)
            {
                Texture2D tex = ExtraTextures.crosslight;
                if (d.customData != null)
                    sb.Draw(tex, d.position - Main.screenPosition, null, Color.White * d.scale * 5, 0, tex.Size() / 2, d.scale * 0.85f * 2, SpriteEffects.None, 0);
                sb.Draw(tex, d.position - Main.screenPosition, null, d.color * (d.customData != null ? ((int)d.customData == 2 ? d.scale * 10 : 1) : 1), 0, tex.Size() / 2, d.scale * 2, SpriteEffects.None, 0);
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
            dust.scale -= 0.0025f;
            dust.rotation = dust.velocity.ToRotation() - MathHelper.PiOver2;
            if (dust.customData != null && dust.customData.GetType() == typeof(Vector2))
            {
                dust.velocity = Vector2.Lerp(dust.velocity, Helper.FromAToB(dust.position, (Vector2)dust.customData, false) / 25, 0.05f + dust.scale);
                if (dust.position.Distance((Vector2)dust.customData) < 100)
                    dust.scale -= 0.01f;
            }
            else
                dust.velocity *= 0.98f;
            if (dust.scale <= 0)
                dust.active = false;
            dust.fadeIn = Lerp(dust.fadeIn, 1, 0.1f);
            return false;
        }
        public static void DrawAll(SpriteBatch sb, Dust d)
        {
            if (d.type == DustType<LineDustFollowPoint>() && d.active)
            {
                Texture2D tex = ExtraTextures2.trace_01;

                for (float i = 0; i < Clamp(10 * d.fadeIn * d.scale * 5, 0, 5); i++)
                    sb.Draw(tex, d.position - d.velocity * 2 * i - Main.screenPosition, null, d.color * (d.scale * 10 * SmoothStep(1, 0, i / 10f)), d.rotation, tex.Size() / 2, new Vector2(1, Clamp(d.velocity.Length() * 0.25f, 0, 2)) * d.scale * 2, SpriteEffects.None, 0);

                sb.Draw(tex, d.position - Main.screenPosition, null, d.color * (d.scale * 10), d.rotation, tex.Size() / 2, new Vector2(1, Clamp(d.velocity.Length() * 0.25f, 0, 2)) * d.scale * 2, SpriteEffects.None, 0);
            }
        }
    }
    public class IntenseDustFollowPoint : ModDust
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
            dust.rotation = dust.velocity.ToRotation() - MathHelper.PiOver2;
            if (dust.customData != null && dust.customData.GetType() == typeof(Vector2) && dust.fadeIn >= 0.9f)
            {
                dust.velocity = Vector2.Lerp(dust.velocity, Helper.FromAToB(dust.position, (Vector2)dust.customData, false) / 5, 0.1f + dust.scale);
                if (dust.position.Distance((Vector2)dust.customData) < 30)
                {
                    if (dust.frame.Y == 0)
                    {
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(dust.position, dust.velocity.SafeNormalize(Vector2.Zero), 3, 6, 10, 1000));
                        dust.frame.Y = 1;
                    }
                    dust.velocity *= 0.7f;
                    dust.scale -= 0.02f;
                }
            }
            else
                dust.velocity *= 0.98f;
            if (dust.scale <= 0)
                dust.active = false;
            dust.fadeIn = Lerp(dust.fadeIn, 1, 0.1f);
            return false;
        }
        public static void DrawAll(SpriteBatch sb, Dust d)
        {
            if (d.type == DustType<IntenseDustFollowPoint>() && d.active)
            {
                Texture2D tex = ExtraTextures.crosslight;
                Texture2D tex2 = ExtraTextures.flare;

                for (float i = 0; i < Clamp(30 * d.scale * 10, 0, 30); i++)
                    sb.Draw(tex2, d.position + d.velocity * 2 - d.velocity * 0.15f * i - Main.screenPosition, null, d.color * (d.scale * 10 * SmoothStep(1, 0, i / 30f)) * d.fadeIn, d.rotation, tex2.Size() / 2, new Vector2(1, Clamp(d.velocity.Length() * 0.25f, 1, 3)) * d.scale, SpriteEffects.None, 0);

                sb.Draw(tex, d.position + d.velocity * 2 - Main.screenPosition, null, d.color * (d.scale * 10) * d.fadeIn, d.rotation, tex.Size() / 2, new Vector2(1, Clamp(d.velocity.Length() * 0.25f, 0, 2)) * d.scale * 3, SpriteEffects.None, 0);
            }
        }
    }
}
