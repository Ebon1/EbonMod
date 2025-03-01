using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace EbonianMod.Common.Graphics
{
    public delegate void UpdateFunction(Particle particle);
    public delegate void DrawFunction(Particle particle, SpriteBatch spriteBatch, Vector2 position);
    public delegate void InitializeFunction(Particle particle);
    public class Particle
    {
        public BGParticleSys parent;
        public Vector2 position = Vector2.Zero;
        public Vector2 velocity = Vector2.Zero;
        public Color color = Color.White;
        public float alpha = 1f;
        public float scale = 1f;
        public float rotation = 0f;
        public Texture2D[] textures;
        public float[] ai;
        public bool dead;
        public UpdateFunction Update;
        public DrawFunction Draw;
    }
    public class BGParticleSys
    {
        readonly List<Particle> particles;
        public BGParticleSys()
        {
            particles = new();
        }

        public static void DefaultDrawHook(Particle part, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(part.textures[0], part.position, null, part.color * part.alpha, 0f,
                part.textures[0].Size() / 2, part.scale * 0.5f, SpriteEffects.None, 0f);
        }
        public void UpdateParticles()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                Particle part = particles[i];
                part.Update(part);
                part.position += part.velocity;
                if (part.dead)
                {
                    particles.RemoveAt(i);
                    i--;
                }
            }
        }
        public void DrawParticles()
        {
            foreach (Particle particle in particles)
            {
                particle.Draw(particle, Main.spriteBatch, particle.position);
            }
        }
        public void CreateParticle(UpdateFunction update, Texture2D[] textures, DrawFunction draw, Vector2 _velocity, InitializeFunction init = default)
        {
            Particle particle = new Particle
            {
                Update = update,
                Draw = draw,
                textures = textures,
                ai = new float[4],
                parent = this,
                velocity = _velocity
            };
            init?.Invoke(particle);
            particles.Add(particle);
        }
    }
}
