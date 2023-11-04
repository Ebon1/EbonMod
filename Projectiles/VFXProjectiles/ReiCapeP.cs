using EbonianMod.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using EbonianMod.Effects.Prims;
using Microsoft.Xna.Framework.Graphics;

namespace EbonianMod.Projectiles.VFXProjectiles
{
    public class ReiCapeP : ModProjectile
    {
        public override string Texture => Helper.Empty;
        Verlet[] verlet = new Verlet[9];
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 9; i++)
                verlet[i] = new Verlet(Main.LocalPlayer.Center - new Vector2(0, 5), 1, 20, 0.5f, true, false, 20, false);
        }
        public override void AI()
        {
            Projectile.Center = Main.LocalPlayer.Center;
            if (verlet[0] != null)
            {
                for (int i = 0; i < 9; i++)
                {
                    verlet[i].Update(Main.LocalPlayer.Center - new Vector2((i - 4) * 1.75f, 5), Projectile.Center);
                    verlet[i].lastP.position -= Vector2.UnitX * (i - 4) * 1.1f;
                }
            }
        }
        public override void PostDraw(Color lightColor)
        {
            if (verlet[0] != null)
            {
                for (int k = 0; k < 9; k++)
                {
                    /*Verlet _verletI = verlet[k];
                    for (int i = 0; i < _verletI.segments.Count - 1; i++)
                    {
                        BeamPacket packet = new BeamPacket();
                        packet.Pass = "Texture";
                        Vector2 start = _verletI.segments[i].pointA.position;
                        bool outline = lightColor != Color.Transparent;
                        Vector2 end = _verletI.segments[i + 1].pointB.position + (outline ? Helper.FromAToB(start, _verletI.segments[i + 1].pointB.position) : Vector2.Zero);
                        if (i >= _verletI.segments.Count - 2)
                            end = _verletI.segments[i].pointB.position;
                        float width = 1 + (outline ? 0.5f : 0);
                        Vector2 offset = (start - end).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * width;

                        BeamPacket.SetTexture(0, Helper.GetExtraTexture("Line"));
                        float off = -Main.GlobalTimeWrappedHourly % 1;
                        Color BeamColor = outline ? Color.Black : Color.White;
                        packet.Add(start + offset * 3, BeamColor, new Vector2(0 + off, 0));
                        packet.Add(start - offset * 3, BeamColor, new Vector2(0 + off, 1));
                        packet.Add(end + offset * 3, BeamColor, new Vector2(1 + off, 0));

                        packet.Add(start - offset * 3, BeamColor, new Vector2(0 + off, 1));
                        packet.Add(end - offset * 3, BeamColor, new Vector2(1 + off, 1));
                        packet.Add(end + offset * 3, BeamColor, new Vector2(1 + off, 0));
                        packet.Send();
                    }*/
                    if (lightColor != Color.Transparent)
                        verlet[k].Draw(Main.spriteBatch, "Extras/Line", useColor: true, color: Color.Black, scale: 3.5f, endTex: "Extras/Empty");
                    verlet[k].Draw(Main.spriteBatch, "Extras/Line", scale: 3, endTex: "Extras/Empty");
                }
            }
        }
    }
}
