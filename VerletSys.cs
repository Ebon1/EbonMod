using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace EbonianMod
{
    public class VerletPoint
    {
        public Vector2 position, oldPos;
        public bool locked, followExol;
        public float alpha;
    }
    public class VerletStick
    {
        public VerletPoint pointA, pointB;
        public float length;
        public VerletStick(VerletPoint a, VerletPoint b)
        {
            pointA = a;
            pointB = b;
            length = Vector2.Distance(pointA.position, pointB.position);
        }
    }
    public class VerletSystem
    {
        public static int maxPoints = 300;
        public static List<VerletPoint> points;
        public static List<VerletStick> sticks;
        public static bool simulating;
        public static int[] order;
        public bool drawingStick;
        public bool autoStickMode;
        public int stickStartIndex;
        public static void Load()
        {
            if (points == null)
            {
                points = new List<VerletPoint>();
            }
            if (sticks == null)
            {
                sticks = new List<VerletStick>();
            }
        }
        public static void ClearAll()
        {
            sticks.Clear();
            points.Clear();
        }
        public void Simulate()
        {
            foreach (VerletPoint p in points)
            {
                if (!p.locked)
                {
                    Vector2 positionBeforeUpdate = p.position;
                    p.position += p.position - p.oldPos;
                    p.position += Vector2.UnitY * 6;
                    p.oldPos = positionBeforeUpdate;
                }
            }
            for (int i = 0; i < 5; i++)
            {
                for (int s = 0; s < sticks.Count; s++)
                {
                    VerletStick stick = sticks[order[s]];

                    Vector2 stickCentre = (stick.pointA.position + stick.pointB.position) / 2;
                    Vector2 stickDir = (stick.pointA.position - stick.pointB.position);
                    stickDir.Normalize();
                    float length = (stick.pointA.position - stick.pointB.position).Length();

                    if (length > stick.length)
                    {
                        if (!stick.pointA.locked)
                        {
                            stick.pointA.position = stickCentre + stickDir * stick.length / 2;
                        }
                        if (!stick.pointB.locked)
                        {
                            stick.pointB.position = stickCentre - stickDir * stick.length / 2;
                        }
                    }

                }
            }
        }
        public static void Update()
        {
            VerletSystem v = new VerletSystem();
            v.Simulate();
        }
        public static void DrawStuff(SpriteBatch spriteBatch)
        {
            foreach (VerletPoint p in points)
            {
                if (p == null) continue;
                spriteBatch.Draw(ModContent.Request<Texture2D>("EbonianMod/circle").Value, p.position - Main.screenPosition, null, (!p.locked ? Color.White * p.alpha : Color.Red * p.alpha), 0, new Vector2(16, 16), 1, SpriteEffects.None, 0f);
            }
            foreach (VerletStick s in sticks)
            {
                VerletSystem v = new VerletSystem();
                if (s == null) continue;
                Utils.DrawLine(spriteBatch, s.pointA.position, s.pointB.position, Color.White, Color.White, 5f);
            }
        }
        public static T[] ShuffleArray<T>(T[] array, Random prng)
        {

            int elementsRemainingToShuffle = array.Length;
            int randomIndex = 0;

            while (elementsRemainingToShuffle > 1)
            {
                randomIndex = prng.Next(0, elementsRemainingToShuffle);
                T chosenElement = array[randomIndex];

                elementsRemainingToShuffle--;
                array[randomIndex] = array[elementsRemainingToShuffle];
                array[elementsRemainingToShuffle] = chosenElement;
            }

            return array;
        }

        public static void CreateOrderArray()
        {
            order = new int[sticks.Count];
            for (int i = 0; i < order.Length; i++)
            {
                order[i] = i;
            }
            //ShuffleArray(order, new System.Random());
        }
    }
}
