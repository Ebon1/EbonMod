using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace EbonianMod.Common.Systems.Verlets
{
    public class SpawnableVerlet
    {
        public Verlet verlet;
        public VerletDrawData drawData;
        public Vector2 velocity;
        public int timeLeft, maxTime;
        public SpawnableVerlet(Verlet v, VerletDrawData vdd, Vector2 vel, int _timeLeft = 120)
        {
            verlet = v;
            drawData = vdd;
            velocity = vel;
            timeLeft = _timeLeft;
            maxTime = _timeLeft;
        }
    }
    public class S_VerletSystem : ModSystem
    {
        public static List<SpawnableVerlet> verlets = new List<SpawnableVerlet>(100);
        public override void PostUpdateEverything()
        {
            for (int i = 0; i < verlets.Count; i++)
            {
                if (verlets[i].timeLeft > -1)
                {
                    if (verlets[i].verlet != null)
                    {
                        if (verlets[i].verlet.gravity > 5)
                            verlets[i].velocity = Vector2.Lerp(verlets[i].velocity, new Vector2(verlets[i].velocity.X * 0.99f, 10), 0.05f);
                        verlets[i].verlet.gravity = Lerp(verlets[i].verlet.gravity, 10, 0.05f);
                        for (int j = 0; j < verlets[i].verlet.points.Count; j++)
                        {
                            UnifiedRandom rand = new UnifiedRandom(i + j);
                            verlets[i].verlet.points[j].collide = true;
                            if (verlets[i].verlet.points[j].colLength < verlets[i].verlet.gravity)
                                verlets[i].verlet.points[j].colLength = verlets[i].verlet.gravity * 2;
                            Vector2 velocity = verlets[i].velocity;
                            if (Helper.TRay.CastLength(verlets[i].verlet.points[j].position, velocity.SafeNormalize(Vector2.UnitY), verlets[i].verlet.points[j].colLength * 2) >= verlets[i].verlet.points[j].colLength * 1.8f || !Collision.SolidCollision(verlets[i].verlet.points[j].position, (int)verlets[i].verlet.points[j].colLength, (int)verlets[i].verlet.points[j].colLength))
                                verlets[i].verlet.points[j].position += verlets[i].velocity * (rand.NextFloat(0.75f, 1f) * (j / (float)verlets[i].verlet.points.Count));

                            verlets[i].verlet.points[j].locked = false;
                        }
                        verlets[i].verlet.Update(verlets[i].verlet.startPos, verlets[i].verlet.endPos);
                    }
                    verlets[i].timeLeft--;
                }
                else
                {
                    verlets.RemoveAt(i);
                }
            }
        }
    }
}