using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace EbonianMod.Misc
{
    //code somewhat (pretty much) taken from spirit mod
    public class Verlet //especially this part
    {
        //i tried to modify it to not be the exact same but theres not much i (a complete dumbass) can do
        public int stiffness { get; set; }
        public List<VerletSegment> segments { get; set; }
        public List<VerletPoint> points { get; set; }
        public VerletPoint firstP { get; set; }
        public VerletPoint lastP { get; set; }
        public float drag { get; set; }
        public float gravity { get; set; }
        public float startRot => segments[0].Rotation();
        public float endRot => segments[segments.Count - 1].Rotation();
        public Vector2 startPos => segments[0].pointA.position;
        public Vector2 endPos => segments[segments.Count - 1].pointB.position;

        public Verlet(Vector2 start, float length, int count, /*float drag = 0.9f,*/ float gravity = 0.2f, bool firstPointLocked = true, bool lastPointLocked = true, int stiffness = 6)
        {
            //this.drag = drag;
            this.gravity = gravity;
            this.stiffness = stiffness;

            Load(start, length, count, firstPointLocked, lastPointLocked);
        }
        private void Load(Vector2 startPosition, float length, int count, bool firstPointLocked = true, bool lastPointLocked = true, Vector2 offset = default)
        {
            segments = new List<VerletSegment>();
            points = new List<VerletPoint>();


            for (int i = 0; i < count; i++)
            {
                points.Add(new VerletPoint(startPosition + (offset == default ? Vector2.Zero : offset * i), gravity/*, drag*/));
            }


            for (int i = 0; i < count - 1; i++)
            {
                segments.Add(new VerletSegment(length, points[i], points[i + 1]));
            }



            firstP = points.First();
            firstP.locked = firstPointLocked;

            lastP = points.Last();
            lastP.locked = lastPointLocked;
        }
        public void Update(Vector2 start, Vector2 end)
        {
            if (firstP.locked)
                firstP.position = start;
            if (lastP.locked)
                lastP.position = end;
            /*points[0] = firstP;
            points[points.Count - 1] = lastP;*/
            foreach (VerletPoint point in points)
            {
                point.Update();
                point.gravity = gravity;
            }
            for (int i = 0; i < stiffness; i++)
                foreach (VerletSegment segment in segments)
                    segment.Constrain();
        }

        public Vector2[] Points()
        {
            List<Vector2> verticeslist = new List<Vector2>();
            foreach (VerletPoint point in points)
                verticeslist.Add(point.position);

            return verticeslist.ToArray();
        }
        public void Draw(SpriteBatch sb, string texPath, string baseTex = null, string endTex = null, bool useColor = false, Color color = default, float scale = 1)
        {
            foreach (VerletSegment segment in segments)
            {
                if ((baseTex != null || endTex != null) ? (segment != segments.First() && segment != segments.Last()) : true)
                {
                    if (useColor)
                        segment.DrawSegments(sb, texPath, color, true, scale: scale);
                    else
                        segment.DrawSegments(sb, texPath, scale: scale);
                }
                else if (endTex != null && segment == segments.Last())
                {
                    if (useColor)
                        segment.Draw(sb, endTex, color, true, scale: scale);
                    else
                        segment.Draw(sb, endTex, scale: scale);
                }
                else if (baseTex != null && segment == segments.First())
                {
                    if (useColor)
                        segment.Draw(sb, baseTex, color, true, scale: scale);
                    else
                        segment.Draw(sb, baseTex, scale: scale);

                }
            }
        }
    }
    public class VerletPoint
    {
        public Vector2 position, lastPos;
        public bool locked;
        public float gravity;
        public VerletPoint(Vector2 position, float gravity/*, float drag*/)
        {
            this.position = position;
            this.gravity = gravity;
            //this.drag = drag;
        }

        public void Update()
        {
            //Vector2 vel = (position - lastPos) * drag;
            lastPos = position;
            //position += vel;
            position += new Vector2(0, gravity);
        }
    }
    public class VerletSegment
    {
        public float len;
        public float Rotation()
        {
            return (this.pointA.position - this.pointB.position).ToRotation() - 1.57f;
        }
        public VerletPoint pointA, pointB;
        public VerletSegment(float len, VerletPoint pointA, VerletPoint pointB)
        {
            this.len = len;
            this.pointA = pointA;
            this.pointB = pointB;
        }

        public void Constrain()
        {
            Vector2 vel = pointB.position - pointA.position;
            float distance = vel.Length();
            float fraction = ((len - distance) / Math.Max(distance, 1)) / 2;
            vel *= fraction;

            if (!pointA.locked)
                pointA.position -= vel;
            if (!pointB.locked)
                pointB.position += vel;
        }
        public void Draw(SpriteBatch sb, string texPath, Color color = default, bool useColor = false, float scale = 1)
        {
            Texture2D tex = Helper.GetTexture(texPath);
            sb.Draw(tex, pointB.position - Main.screenPosition, null, useColor ? color : Lighting.GetColor((int)pointB.position.X / 16, (int)(pointB.position.Y / 16.0)), Rotation(), tex.Size() / 2, scale, SpriteEffects.None, 0);
        }
        public void DrawSegments(SpriteBatch sb, string texPath, Color color = default, bool useColor = false, float scale = 1)
        {
            Texture2D tex = Helper.GetTexture(texPath);
            Vector2 center = pointB.position;
            Vector2 distVector = pointA.position - pointB.position;
            float distance = distVector.Length();
            int attempts = 0;
            while (distance > tex.Height / 2 && !float.IsNaN(distance) && ++attempts < 100)
            {
                distVector.Normalize();
                distVector *= tex.Height / 2;
                center += distVector;
                distVector = pointA.position - center;
                distance = distVector.Length();
                sb.Draw(tex, center - Main.screenPosition, null, useColor ? color : Lighting.GetColor((int)pointB.position.X / 16, (int)(pointB.position.Y / 16.0)), Rotation(), tex.Size() / 2, scale, SpriteEffects.None, 0);
            }
            Draw(sb, texPath, color, useColor, scale);
        }
    }
}
