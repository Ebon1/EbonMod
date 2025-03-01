using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace EbonianMod.Common.Systems.Verlets
{

    public struct VerletDrawData
    {
        public string texPath, baseTex, endTex;
        public int maxVariants, variantSeed;
        public bool useColor, useRot, useRotEnd, useRotFirst, scaleCalcForDist, clampScaleCalculationForDistCalculation, textureVariation;
        public float scale, rot, endRot, firstRot;
        public Color color;
        public VerletDrawData(string _texPath, string _baseTex = null, string _endTex = null, bool _useColor = false, Color _color = default, float _scale = 1, float _rot = 0,
            bool _useRot = false, bool _useRotEnd = false, bool _useRotFirst = false, float _endRot = 0, float _firstRot = 0, bool _scaleCalcForDist = false,
            bool _clampScaleCalculationForDistCalculation = true, bool _textureVariation = false, int _maxVariants = -1, int _variantSeed = -1)
        {
            texPath = _texPath;
            baseTex = _baseTex;
            endTex = _endTex;
            variantSeed = _variantSeed;
            useColor = _useColor;
            useRot = _useRot;
            useRotEnd = _useRotEnd;
            useRotFirst = _useRotFirst;
            color = _color;
            scale = _scale;
            rot = _rot;
            endRot = _endRot;
            firstRot = _firstRot;
            scaleCalcForDist = _scaleCalcForDist;
            clampScaleCalculationForDistCalculation = _clampScaleCalculationForDistCalculation;
            textureVariation = _textureVariation;
            maxVariants = _maxVariants;
        }
    }
    public class Verlet
    {

        public int stiffness { get; set; }
        public List<VerletSegment> segments { get; set; }
        public List<VerletPoint> points { get; set; }
        public VerletPoint firstP { get; set; }
        public VerletPoint lastP { get; set; }
        public float drag { get; set; }
        public float gravity { get; set; }
        public Vector2 gravityDirection { get; set; }
        public float startRot => segments[0].Rotation();
        public float endRot => segments[segments.Count - 1].Rotation();
        public Vector2 startPos => segments[0].pointA.position;
        public Vector2 endPos => segments[segments.Count - 1].pointB.position;

        public Verlet(Vector2 start, float length, int count, /*float drag = 0.9f,*/ float gravity = 0.2f, bool firstPointLocked = true, bool lastPointLocked = true, int stiffness = 6, bool collide = false, float colLength = 1)
        {

            this.gravity = gravity;
            gravityDirection = Vector2.UnitY;
            this.stiffness = stiffness;

            Load(start, length, count, firstPointLocked, lastPointLocked, collide: collide, colLength: colLength);
        }
        private void Load(Vector2 startPosition, float length, int count, bool firstPointLocked = true, bool lastPointLocked = true, Vector2 offset = default, bool collide = false, float colLength = 1)
        {
            segments = new List<VerletSegment>();
            points = new List<VerletPoint>();


            for (int i = 0; i < count; i++)
            {
                points.Add(new VerletPoint(startPosition + (offset == default ? Vector2.Zero : offset * i), gravity, gravityDirection/*, drag*/, collide, colLength));
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
        public void Update(Vector2 start, Vector2 end, float lerpT = 1f)
        {
            if (firstP.locked)
                firstP.position = Vector2.Lerp(firstP.position, start, lerpT);
            if (lastP.locked)
                lastP.position = Vector2.Lerp(lastP.position, end, lerpT);
            foreach (VerletPoint point in points)
            {
                point.Update();
                point.gravity = gravity;
                if (point.gravityDirection != gravityDirection)
                {
                    if (gravityDirection.Length() > 0)
                        gravityDirection.Normalize();
                    point.gravityDirection = gravityDirection;
                }
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
        public void Draw(SpriteBatch sb, string texPath, string baseTex = null, string endTex = null, bool useColor = false, Color color = default, float scale = 1, float rot = 0, bool useRot = false, bool useRotEnd = false, bool useRotFirst = false, float endRot = 0, float firstRot = 0, bool scaleCalcForDist = false, bool clampScaleCalculationForDistCalculation = true, bool textureVariation = false, int maxVariants = -1, int variantSeed = -1)
        {
            UnifiedRandom rand = new UnifiedRandom(variantSeed);
            foreach (VerletSegment segment in segments)
            {
                if (baseTex != null || endTex != null ? segment != segments.First() && segment != segments.Last() : true)
                {
                    int variant = rand.Next(maxVariants > 0 ? maxVariants : 2);
                    if (useColor)
                        segment.DrawSegments(sb, texPath + (textureVariation ? variant.ToString() : ""), color, true, scale: scale, rot, useRot, scaleCalcForDist, clampScaleCalculationForDistCalculation);
                    else
                        segment.DrawSegments(sb, texPath + (textureVariation ? variant.ToString() : ""), scale: scale, rot: rot, useRot: useRot, scaleCalcForDist: scaleCalcForDist, clampScaleCalculationForDistCalculation: clampScaleCalculationForDistCalculation);
                }
                else if (endTex != null && segment == segments.Last())
                {
                    if (useColor)
                        segment.Draw(sb, endTex, color, true, scale: scale, endRot, useRotEnd);
                    else
                        segment.Draw(sb, endTex, scale: scale, rot: endRot, useRot: useRotEnd);
                }
                else if (baseTex != null && segment == segments.First())
                {
                    if (useColor)
                        segment.Draw(sb, baseTex, color, true, scale: scale, firstRot, useRotFirst);
                    else
                        segment.Draw(sb, baseTex, scale: scale, rot: firstRot, useRot: useRotFirst);

                }
            }
        }
        public void Draw(SpriteBatch sb, VerletDrawData drawData)
        {
            UnifiedRandom rand = new UnifiedRandom(drawData.variantSeed);
            foreach (VerletSegment segment in segments)
            {
                if (drawData.baseTex != null || drawData.endTex != null ? segment != segments.First() && segment != segments.Last() : true)
                {
                    int variant = rand.Next(drawData.maxVariants > 0 ? drawData.maxVariants : 2);
                    if (drawData.useColor)
                        segment.DrawSegments(sb, drawData.texPath + (drawData.textureVariation ? variant.ToString() : ""), drawData.color, true, scale: drawData.scale, drawData.rot, drawData.useRot, drawData.scaleCalcForDist, drawData.clampScaleCalculationForDistCalculation);
                    else
                        segment.DrawSegments(sb, drawData.texPath + (drawData.textureVariation ? variant.ToString() : ""), scale: drawData.scale, rot: drawData.rot, useRot: drawData.useRot, scaleCalcForDist: drawData.scaleCalcForDist, clampScaleCalculationForDistCalculation: drawData.clampScaleCalculationForDistCalculation);
                }
                else if (drawData.endTex != null && segment == segments.Last())
                {
                    if (drawData.useColor)
                        segment.Draw(sb, drawData.endTex, drawData.color, true, scale: drawData.scale, drawData.endRot, drawData.useRotEnd);
                    else
                        segment.Draw(sb, drawData.endTex, scale: drawData.scale, rot: drawData.endRot, useRot: drawData.useRotEnd);
                }
                else if (drawData.baseTex != null && segment == segments.First())
                {
                    if (drawData.useColor)
                        segment.Draw(sb, drawData.baseTex, drawData.color, true, scale: drawData.scale, drawData.firstRot, drawData.useRotFirst);
                    else
                        segment.Draw(sb, drawData.baseTex, scale: drawData.scale, rot: drawData.firstRot, useRot: drawData.useRotFirst);
                }
            }
        }
    }
    public class VerletPoint
    {
        public Vector2 position, lastPos, gravityDirection;
        public bool locked;
        public float gravity;
        public bool collide;
        public float colLength;
        public VerletPoint(Vector2 position, float gravity, Vector2 gravityDirection/*, float drag*/, bool collide = false, float colLength = 1)
        {
            this.position = position;
            this.gravity = gravity;
            this.collide = collide;
            this.colLength = colLength;
            this.gravityDirection = gravityDirection;
        }

        public void Update()
        {
            {
                /*if (!isLast)
                {
                    lastPos = position;
                    position += new Vector2(0, gravity);
                }
                if (isLast)
                {*/
                if (collide)
                {
                    if (Helper.TRay.CastLength(position, gravityDirection, colLength) >= colLength || !Collision.SolidCollision(position, (int)colLength, (int)colLength))
                    {
                        lastPos = position;
                        position += gravityDirection * gravity;
                    }
                }
                else
                {
                    lastPos = position;
                    position += gravityDirection * gravity;
                }
            }
        }
    }
    public class VerletSegment
    {
        public bool cut = false;
        public float len;
        public float Rotation()
        {
            return (pointA.position - pointB.position).ToRotation() - 1.57f;
        }
        public VerletPoint pointA, pointB;
        public VerletSegment(float len, VerletPoint pointA, VerletPoint pointB)
        {
            this.len = len;
            cut = false;
            this.pointA = pointA;
            this.pointB = pointB;
        }

        public void Constrain()
        {
            if (cut)
                return;
            Vector2 vel = pointB.position - pointA.position;
            float distance = vel.Length();
            float fraction = (len - distance) / Math.Max(distance, 1) / 2;
            vel *= fraction;

            if (!pointA.locked)
                pointA.position -= vel;
            if (!pointB.locked)
                pointB.position += vel;
        }
        public void Draw(SpriteBatch sb, string texPath, Color color = default, bool useColor = false, float scale = 1, float rot = 0, bool useRot = false)
        {
            if (cut)
                return;
            Texture2D tex = Helper.GetTexture(texPath);
            sb.Draw(tex, pointB.position - Main.screenPosition, null, useColor ? color : Lighting.GetColor((int)pointB.position.X / 16, (int)(pointB.position.Y / 16.0)), useRot ? rot : Rotation(), tex.Size() / 2, scale, SpriteEffects.None, 0);
        }
        public void DrawSegments(SpriteBatch sb, string texPath, Color color = default, bool useColor = false, float scale = 1, float rot = 0, bool useRot = false, bool scaleCalcForDist = false, bool clampScaleCalculationForDistCalculation = true)
        {
            if (cut)
                return;
            Texture2D tex = Helper.GetTexture(texPath);
            Vector2 center = pointB.position;
            Vector2 distVector = pointA.position - pointB.position;
            float distance = distVector.Length();
            int attempts = 0;
            while (distance > tex.Height * (scaleCalcForDist ? clampScaleCalculationForDistCalculation ? Clamp(scale, 0, 1f) : scale : 1f) && !float.IsNaN(distance) && ++attempts < 100)
            {
                distVector.Normalize();
                distVector *= tex.Height * (scaleCalcForDist ? clampScaleCalculationForDistCalculation ? Clamp(scale, 0, 1f) : scale : 1f);
                center += distVector;
                distVector = pointA.position - center;
                distance = distVector.Length();
                sb.Draw(tex, center - Main.screenPosition, null, useColor ? color : Lighting.GetColor((int)pointB.position.X / 16, (int)(pointB.position.Y / 16.0)), useRot ? rot : Rotation(), tex.Size() / 2, scale, SpriteEffects.None, 0);
            }
            Draw(sb, texPath, color, useColor, scale);
        }
    }
}
