using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using System.Collections.ObjectModel;
using Terraria.UI.Chat;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using ReLogic.Graphics;
using Terraria.Chat;
using Terraria.GameContent.UI.Chat;
using System.IO;
using System.Text;
using static Terraria.ModLoader.ModContent;
using EbonianMod.Projectiles;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;
using Terraria.Graphics.Shaders;
using System.ComponentModel;
using static tModPorter.ProgressUpdate;
using System.Linq;
using System.Runtime;

namespace EbonianMod
{
    public struct Text
    {
        public string text;
        public Rectangle rect;
        public string wrappedString;
        public Text(Rectangle rect, DynamicSpriteFont font, string text)
        {
            this.text = text;
            this.rect = rect;
            this.wrappedString = Helper.WrapText(font, this.text, this.rect.Width);
        }
    }

    public static class Helper
    {
        /// <summary>
        /// Avoids division by zero 
        /// </summary>
        public static float Safe(this float f, float x = 1)
        {
            return f + (f == 0 ? x : 0);
        }
        /// <summary>
        /// Finds the shortest path to a desired angle
        /// </summary>
        public static float ShortestPathToAngle(float from, float to)
        {
            float difference = (to - from) % MathHelper.TwoPi;
            return (2 * difference % MathHelper.TwoPi) - difference;
        }
        /// <summary>
        /// Lerps to the shortest path to a desired angle
        /// </summary>
        public static float LerpAngle(float from, float to, float t)
        {
            return from + ShortestPathToAngle(from, to) * t;
        }
        public static Rectangle ToRectangle(this System.Drawing.RectangleF rect)
        {
            return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }
        public static float ClosestTo(this IEnumerable<float> collection, float target)
        {
            // NB Method will return int.MaxValue for a sequence containing no elements.
            // Apply any defensive coding here as necessary.
            var closest = float.MaxValue;
            var minDifference = float.MaxValue;
            foreach (var element in collection)
            {
                var difference = Math.Abs(element - target);
                if (minDifference > difference)
                {
                    minDifference = difference;
                    closest = element;
                }
            }

            return closest;
        }
        public static int IndexOfClosestTo(this IEnumerable<float> collection, float target)
        {
            // NB Method will return int.MaxValue for a sequence containing no elements.
            // Apply any defensive coding here as necessary.
            int closest = 0;
            var minDifference = float.MaxValue;
            foreach (float element in collection)
            {
                var difference = Math.Abs(element - target);
                if (minDifference > difference)
                {
                    minDifference = difference;
                    closest = Array.IndexOf(collection.ToArray(), element);
                }
            }

            return closest;
        }
        public static float Closer(float a, float b, float compareValue)
        {

            float calcA = Math.Abs(a - compareValue);
            float calcB = Math.Abs(b - compareValue);

            if (calcA == calcB)
            {
                return 0;
            }

            if (calcA < calcB)
            {
                return a;
            }

            return b;
        }
        /// <summary>
        /// Extremely laggy grounded check, only use this for stuff like death animations where you absolutely dont want the npc to not be able to detect the ground
        /// </summary>
        /// 
        public static bool Grounded(this NPC NPC, float offset = .5f, float offsetX = 1f)
        {
            if (NPC.collideY)
                return true;
            if ((!Collision.CanHitLine(new Vector2(NPC.Center.X, NPC.Center.Y + NPC.height / 2), 1, 1, new Vector2(NPC.Center.X, NPC.Center.Y + (NPC.height * offset) / 2), 1, 1) || Collision.FindCollisionDirection(out int dir, NPC.Center, 1, NPC.height / 2)))
                return true;
            for (int i = 0; i < NPC.width * offsetX; i += (int)(1 / (offsetX == 0 ? 1 : offsetX))) //full sprite check
            {

                bool a = TRay.CastLength(NPC.BottomLeft + Vector2.UnitX * i, Vector2.UnitY, 1000) < NPC.height * offset;
                if (!a)
                    continue;
                return a;
            }
            return false; //give up
        }
        public static string RgbToHex(Color color)
        {
            int r = color.R;
            int g = color.G;
            int b = color.B;
            return string.Format("{0:X2}{1:X2}{2:X2}", r, g, b);
        }
        public static VertexPositionColorTexture AsVertex(Vector2 position, Color color, Vector2 texCoord)
        {
            return new VertexPositionColorTexture(new Vector3(position, 50), color, texCoord);
        }
        public static VertexPositionColorTexture AsVertex(Vector3 position, Color color, Vector2 texCoord)
        {
            return new VertexPositionColorTexture(position, color, texCoord);
        }
        private static int width;
        private static int height;
        private static Vector2 zoom;
        private static Matrix view;
        private static Matrix projection;
        private static bool CheckGraphicsChanged()
        {
            var device = Main.graphics.GraphicsDevice;
            bool changed = device.Viewport.Width != width
                           || device.Viewport.Height != height
                           || Main.GameViewMatrix.Zoom != zoom;

            if (!changed) return false;

            width = device.Viewport.Width;
            height = device.Viewport.Height;
            zoom = Main.GameViewMatrix.Zoom;

            return true;
        }

        /// <summary>
        /// used for uuhhhhh evenly distrubuted velocity or sum shit i forgor
        /// </summary>
        public static float CircleDividedEqually(float i, float max)
        {
            return 2f * (float)Math.PI / max * i;
        }
        public static Matrix GetMatrix()
        {
            if (CheckGraphicsChanged())
            {
                var device = Main.graphics.GraphicsDevice;
                int width = device.Viewport.Width;
                int height = device.Viewport.Height;
                Vector2 zoom = Main.GameViewMatrix.Zoom;
                view =
                    Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up)
                    * Matrix.CreateTranslation(width / 2, height / -2, 0)
                    * Matrix.CreateRotationZ(MathHelper.Pi)
                    * Matrix.CreateScale(zoom.X, zoom.Y, 1f);
                projection = Matrix.CreateOrthographic(width, height, 0, 1000);
            }

            return view * projection;
        }

        private static int GetPrimitiveCount(int vertexCount, PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.LineList:
                    return vertexCount / 2;
                case PrimitiveType.LineStrip:
                    return vertexCount - 1;
                case PrimitiveType.TriangleList:
                    return vertexCount / 3;
                case PrimitiveType.TriangleStrip:
                    return vertexCount - 2;
                default: return 0;
            }
        }
        public static void DrawPrimitives(VertexPositionColorTexture[] vertices, PrimitiveType type, bool drawBacksides = true)
        {
            if (vertices.Length < 6) return;
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            Effect effect = EbonianMod.TrailShader;
            effect.Parameters["WorldViewProjection"].SetValue(GetMatrix());
            effect.CurrentTechnique.Passes["Default"].Apply();
            if (drawBacksides)
            {
                short[] indices = new short[vertices.Length * 2];
                for (int i = 0; i < vertices.Length; i += 3)
                {
                    indices[i * 2] = (short)i;
                    indices[i * 2 + 1] = (short)(i + 1);
                    indices[i * 2 + 2] = (short)(i + 2);

                    indices[i * 2 + 5] = (short)i;
                    indices[i * 2 + 4] = (short)(i + 1);
                    indices[i * 2 + 3] = (short)(i + 2);
                }

                device.DrawUserIndexedPrimitives(type, vertices, 0, vertices.Length, indices, 0,
                    GetPrimitiveCount(vertices.Length, type) * 2);
            }
            else
            {
                device.DrawUserPrimitives(type, vertices, 0, GetPrimitiveCount(vertices.Length, type));
            }
        }
        public static void Reload(this SpriteBatch spriteBatch, SamplerState _samplerState = default)
        {
            if ((bool)spriteBatch.GetType().GetField("beginCalled", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch))
            {
                spriteBatch.End();
            }
            SpriteSortMode sortMode = SpriteSortMode.Deferred;
            SamplerState samplerState = _samplerState;
            BlendState blendState = (BlendState)spriteBatch.GetType().GetField("blendState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            DepthStencilState depthStencilState = (DepthStencilState)spriteBatch.GetType().GetField("depthStencilState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            RasterizerState rasterizerState = (RasterizerState)spriteBatch.GetType().GetField("rasterizerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            Effect effect = (Effect)spriteBatch.GetType().GetField("customEffect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            Matrix matrix = (Matrix)spriteBatch.GetType().GetField("transformMatrix", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, matrix);
        }
        public static void DrawTexturedPrimitives(VertexPositionColorTexture[] vertices, PrimitiveType type, Texture2D texture, bool drawBacksides = true)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            Effect effect = EbonianMod.TrailShader;
            effect.Parameters["WorldViewProjection"].SetValue(GetMatrix());
            effect.Parameters["tex"].SetValue(texture);
            effect.CurrentTechnique.Passes["Texture"].Apply();
            if (drawBacksides)
            {
                short[] indices = new short[vertices.Length * 2];
                for (int i = 0; i < vertices.Length; i += 3)
                {
                    indices[i * 2] = (short)i;
                    indices[i * 2 + 1] = (short)(i + 1);
                    indices[i * 2 + 2] = (short)(i + 2);

                    indices[i * 2 + 5] = (short)i;
                    indices[i * 2 + 4] = (short)(i + 1);
                    indices[i * 2 + 3] = (short)(i + 2);
                }

                device.DrawUserIndexedPrimitives(type, vertices, 0, vertices.Length, indices, 0,
                    GetPrimitiveCount(vertices.Length, type) * 2);
            }
            else
            {
                device.DrawUserPrimitives(type, vertices, 0, GetPrimitiveCount(vertices.Length, type));
            }
        }

        public static Vector2 GetRotation(List<Vector2> oldPos, int index)
        {
            if (oldPos.Count == 1)
                return oldPos[0];

            if (index == 0)
            {
                return Vector2.Normalize(oldPos[1] - oldPos[0]).RotatedBy(MathHelper.Pi / 2);
            }

            return (index == oldPos.Count - 1
                ? Vector2.Normalize(oldPos[index] - oldPos[index - 1])
                : Vector2.Normalize(oldPos[index + 1] - oldPos[index - 1])).RotatedBy(MathHelper.Pi / 2);
        }

        public static string WrapText(DynamicSpriteFont spriteFont, string text, float maxLineWidth)
        {
            string[] words = text.Split(' ');

            StringBuilder sb = new StringBuilder();

            float lineWidth = 0f;

            float spaceWidth = spriteFont.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = spriteFont.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb.ToString();
        }
        public static string BuffPlaceholder = "EbonianMod/Buffs/ExolStun";
        public static string Empty = "EbonianMod/Extras/Empty";
        public static string Placeholder = "EbonianMod/Extras/Placeholder";
        public static class TRay
        {
            public static Vector2 Cast(Vector2 start, Vector2 direction, float length, bool platformCheck = false)
            {
                direction = direction.SafeNormalize(Vector2.UnitY);
                Vector2 output = start;

                for (int i = 0; i < length; i++)
                {
                    if (Collision.CanHitLine(output, 0, 0, output + direction, 0, 0) && (platformCheck ? !Collision.SolidTiles(output, 1, 1, platformCheck) && Main.tile[(int)output.X / 16, (int)output.Y / 16].TileType == TileID.Platforms : true))
                    {
                        output += direction;
                    }
                    else
                    {
                        break;
                    }
                }

                return output;
            }
            public static float CastLength(Vector2 start, Vector2 direction, float length, bool platformCheck = false)
            {
                Vector2 end = Cast(start, direction, length, platformCheck);
                return (end - start).Length();
            }
        }
        public static Texture2D GetExtraTexture(string tex, bool altMethod = false)
        {
            if (altMethod)
                return GetTextureAlt("Extras/" + tex);
            return GetTexture("Extras/" + tex);
        }
        public static Texture2D GetTexture(string path)
        {
            if (path.Contains("EbonianMod/"))
                return ModContent.Request<Texture2D>(path).Value;
            else
                return ModContent.Request<Texture2D>("EbonianMod/" + path).Value;
        }
        public static Texture2D GetTextureAlt(string path)
        {
            return EbonianMod.Instance.Assets.Request<Texture2D>(path).Value;
        }
        public static Vector4 ColorToVector4(Color color)
        {
            return new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }
        public static Vector4 ColorToVector4(Vector4 color)
        {
            return new Vector4(color.X / 255f, color.Y / 255f, color.Z / 255f, color.W / 255f);
        }
        public static Vector4 ColorToVector4(Vector3 color)
        {
            return new Vector4(color.X / 255f, color.Y / 255f, color.Z / 255f, 1);
        }
        //public static Player[] activePlayers = new Player[Main.maxPlayers];
        //public static Player GetRandomPlayer()
        //{
        //   return Main.player[Main.rand.Next(activePlayers.Length)];
        //}
        public static SpriteSortMode previousSortMode;
        public static BlendState previousBlendState;
        public static SamplerState previousSamplerState;
        public static DepthStencilState previousDepthStencilState;
        public static RasterizerState previousRasterizerState;
        public static Effect previousEffect;
        public static Matrix previousMatrix;

        public static void SaveCurrent(this SpriteBatch spriteBatch)
        {
            previousSortMode = SpriteSortMode.Deferred;
            previousBlendState = (BlendState)spriteBatch.GetType().GetField("blendState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            previousSamplerState = (SamplerState)spriteBatch.GetType().GetField("samplerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            previousDepthStencilState = (DepthStencilState)spriteBatch.GetType().GetField("depthStencilState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            previousRasterizerState = (RasterizerState)spriteBatch.GetType().GetField("rasterizerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            previousEffect = (Effect)spriteBatch.GetType().GetField("customEffect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            previousMatrix = (Matrix)spriteBatch.GetType().GetField("transformMatrix", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
        }

        public static void ApplySaved(this SpriteBatch spriteBatch)
        {
            if ((bool)spriteBatch.GetType().GetField("beginCalled", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch))
            {
                spriteBatch.End();
            }
            SpriteSortMode sortMode = previousSortMode;
            BlendState blendState = previousBlendState;
            SamplerState samplerState = previousSamplerState;
            DepthStencilState depthStencilState = previousDepthStencilState;
            RasterizerState rasterizerState = previousRasterizerState;
            Effect effect = previousEffect;
            spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, previousMatrix);
        }
        public static void Reload(this SpriteBatch spriteBatch, SpriteSortMode sortMode = SpriteSortMode.Deferred)
        {
            if ((bool)spriteBatch.GetType().GetField("beginCalled", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch))
            {
                spriteBatch.End();
            }
            BlendState blendState = (BlendState)spriteBatch.GetType().GetField("blendState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            SamplerState samplerState = (SamplerState)spriteBatch.GetType().GetField("samplerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            DepthStencilState depthStencilState = (DepthStencilState)spriteBatch.GetType().GetField("depthStencilState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            RasterizerState rasterizerState = (RasterizerState)spriteBatch.GetType().GetField("rasterizerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            Effect effect = (Effect)spriteBatch.GetType().GetField("customEffect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            Matrix matrix = (Matrix)spriteBatch.GetType().GetField("transformMatrix", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, matrix);
        }
        public static void Reload(this SpriteBatch spriteBatch, BlendState blendState = default)
        {
            if ((bool)spriteBatch.GetType().GetField("beginCalled", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch))
            {
                spriteBatch.End();
            }
            SpriteSortMode sortMode = SpriteSortMode.Deferred;
            SamplerState samplerState = (SamplerState)spriteBatch.GetType().GetField("samplerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            DepthStencilState depthStencilState = (DepthStencilState)spriteBatch.GetType().GetField("depthStencilState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            RasterizerState rasterizerState = (RasterizerState)spriteBatch.GetType().GetField("rasterizerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            Effect effect = (Effect)spriteBatch.GetType().GetField("customEffect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            Matrix matrix = (Matrix)spriteBatch.GetType().GetField("transformMatrix", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, matrix);
        }
        public static void Reload(this SpriteBatch spriteBatch, Effect effect = null)
        {
            if ((bool)spriteBatch.GetType().GetField("beginCalled", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch))
            {
                spriteBatch.End();
            }
            SpriteSortMode sortMode = SpriteSortMode.Deferred;
            BlendState blendState = (BlendState)spriteBatch.GetType().GetField("blendState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            SamplerState samplerState = (SamplerState)spriteBatch.GetType().GetField("samplerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            DepthStencilState depthStencilState = (DepthStencilState)spriteBatch.GetType().GetField("depthStencilState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            RasterizerState rasterizerState = (RasterizerState)spriteBatch.GetType().GetField("rasterizerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            Matrix matrix = (Matrix)spriteBatch.GetType().GetField("transformMatrix", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, matrix);
        }
        public static void SineMovement(this Projectile projectile, Vector2 initialCenter, Vector2 initialVel, float frequencyMultiplier, float amplitude)
        {
            projectile.ai[1]++;
            float wave = (float)Math.Sin(projectile.ai[1] * frequencyMultiplier);
            Vector2 vector = new Vector2(initialVel.X, initialVel.Y).RotatedBy(MathHelper.ToRadians(90));
            vector.Normalize();
            wave *= projectile.ai[0];
            wave *= amplitude;
            Vector2 offset = vector * wave;
            projectile.Center = initialCenter + (projectile.velocity * projectile.ai[1]);
            projectile.Center = projectile.Center + offset;
        }
        public static Vector2 FromAToB(this Vector2 a, Vector2 b, bool normalize = true, bool reverse = false)
        {
            Vector2 baseVel = b - a;
            if (normalize)
                baseVel.Normalize();
            if (reverse)
            {
                Vector2 baseVelReverse = a - b;
                if (normalize)
                    baseVelReverse.Normalize();
                return baseVelReverse;
            }
            return baseVel;
        }
        public static bool CloseTo(this float f, float target, float range = 1f)
        {
            return f > target - range && f < target + range;
        }
        public static void DustExplosion(Vector2 pos, Vector2 size = default, int type = 0, Color color = default, bool sound = true, bool smoke = true, float scaleFactor = 1, float increment = 0.125f, Vector2 _vel = default)
        {

            int dustType = ModContent.DustType<Dusts.ColoredFireDust>();
            switch (type)
            {
                case 0:
                    dustType = ModContent.DustType<Dusts.ColoredFireDust>();
                    break;
                case 1:
                    dustType = ModContent.DustType<Dusts.FireDust>();
                    break;
                case 2:
                    dustType = ModContent.DustType<Dusts.SmokeDustAkaFireDustButNoGlow>();
                    break;
            }
            if (sound)
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item14, pos);
            for (float num614 = 0f; num614 < 1f; num614 += increment)
            {
                Vector2 velocity = Vector2.UnitY.RotatedBy(num614 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f);
                if (increment == 1 || type == 2)
                    velocity = _vel;
                Dust dust = Dust.NewDustPerfect(pos, dustType, velocity, 150, color, scaleFactor);
                dust.noGravity = true;
                dust.color = color;

            }
            if (smoke)
                for (int num905 = 0; num905 < 10; num905++)
                {
                    int num906 = Dust.NewDust(new Vector2(pos.X, pos.Y), (int)size.X, (int)size.Y, 31, 0f, 0f, 0, default(Color), 2.5f * scaleFactor);
                    Main.dust[num906].position = pos + Vector2.UnitX.RotatedByRandom(3.1415927410125732) * size.X / 2f;
                    Main.dust[num906].noGravity = true;
                    Dust dust2 = Main.dust[num906];
                    dust2.velocity *= 3f;
                }
            if (smoke)
                for (int num899 = 0; num899 < 4; num899++)
                {
                    int num900 = Dust.NewDust(new Vector2(pos.X, pos.Y), (int)size.X, (int)size.Y, 31, 0f, 0f, 100, default(Color), 1.5f * scaleFactor);
                    Main.dust[num900].position = pos + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * size.X / 2f;
                }
        }
        public static void Log(this Projectile obj)
        {
            Main.NewText("Friendly?" + obj.friendly);
            Main.NewText("Hostile?" + obj.hostile);
            Main.NewText("Object:" + obj.Name);
            Main.NewText("Timeleft:" + obj.timeLeft);
            Main.NewText("Damage:" + obj.damage);
            Main.NewText("AI: [" + obj.ai[0] + ", " + obj.ai[1] + "]");
            Main.NewText("Direction:" + obj.direction);
            Main.NewText("LocalAI: [" + obj.localAI[0] + ", " + obj.localAI[1] + "]");
            Main.NewText("Velocity:" + obj.velocity);
            Main.NewText("Owner:" + obj.owner);
        }
        public static void Log(this NPC obj)
        {
            Main.NewText("Friendly?" + obj.friendly);
            Main.NewText("Object:" + obj.TypeName);
            Main.NewText("Timeleft:" + obj.timeLeft);
            Main.NewText("Damage:" + obj.damage);
            Main.NewText("AI: [" + obj.ai[0] + ", " + obj.ai[1] + ", " + obj.ai[2] + ", " + obj.ai[3] + "]");
            Main.NewText("Direction:" + obj.direction);
            Main.NewText("LocalAI: [" + obj.localAI[0] + ", " + obj.localAI[1] + ", " + obj.localAI[2] + ", " + obj.localAI[3] + "]");
            Main.NewText("Velocity:" + obj.velocity);
        }
        public static void CollisionTPNoDust(Vector2 targetPosition, Player player)
        {
            int num = 150;
            Vector2 vector = player.position;
            Vector2 vector2 = player.velocity;
            for (int i = 0; i < num; i++)
            {
                vector2 = (vector + player.Size / 2f).DirectionTo(targetPosition).SafeNormalize(Vector2.Zero) * 12f;
                Vector2 vector3 = Collision.TileCollision(vector, vector2, player.width, player.height, fallThrough: true, fall2: true, (int)player.gravDir);
                vector += vector3;
            }
            _ = vector - player.position;
            TPNoDust(vector, player);
            NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, vector.X, vector.Y, 0);
        }
        public static void TPNoDust(Vector2 newPos, Player player)
        {
            try
            {
                player._funkytownAchievementCheckCooldown = 100;
                player.environmentBuffImmunityTimer = 4;
                player.RemoveAllGrapplingHooks();
                player.StopVanityActions();
                float num = MathHelper.Clamp(1f - player.teleportTime * 0.99f, 0.01f, 1f);
                Vector2 otherPosition = player.position;
                float num2 = Vector2.Distance(player.position, newPos);
                PressurePlateHelper.UpdatePlayerPosition(player);
                player.position = newPos;
                player.fallStart = (int)(player.position.Y / 16f);
                if (player.whoAmI == Main.myPlayer)
                {
                    bool flag = false;
                    if (num2 < new Vector2(Main.screenWidth, Main.screenHeight).Length() / 2f + 100f)
                    {
                        int time = 0;
                        Main.SetCameraLerp(0.1f, time);
                        flag = true;
                    }
                    else
                    {
                        NPC.ResetNetOffsets();
                        Main.BlackFadeIn = 255;
                        Lighting.Clear();
                        Main.screenLastPosition = Main.screenPosition;
                        Main.screenPosition.X = player.position.X + (float)(player.width / 2) - (float)(Main.screenWidth / 2);
                        Main.screenPosition.Y = player.position.Y + (float)(player.height / 2) - (float)(Main.screenHeight / 2);
                        Main.instantBGTransitionCounter = 10;
                        player.ForceUpdateBiomes();
                    }
                }
                PressurePlateHelper.UpdatePlayerPosition(player);
                player.ResetAdvancedShadows();
                for (int i = 0; i < 3; i++)
                {
                    player.UpdateSocialShadow();
                }
                player.oldPosition = player.position + player.BlehOldPositionFixer;
            }
            catch
            {
            }
        }
        public static void SetBossTitle(int progress, string name, Color color, string title = null, int style = -1)
        {
            EbonianPlayer player = Main.LocalPlayer.GetModPlayer<EbonianPlayer>();
            player.bossTextProgress = progress;
            player.bossMaxProgress = progress;
            player.bossName = name;
            player.bossTitle = title;
            player.bossColor = color;
            player.bossStyle = style;
        }
        /*public static void SetTimeSlow(int progress, int skips)
        {
            EbonianPlayer player = Main.LocalPlayer.GetModPlayer<EbonianPlayer>();
            player.timeSlowMax = progress;
            player.timeSlowProgress = progress;
            EbonianMod.timeSkips = skips;
        }*/
        public static string GetBossText()
        {
            var player = Main.LocalPlayer.GetModPlayer<EbonianPlayer>();
            if (player.bossTitle == null || player.bossTitle == "")
                return "";
            float progress = Utils.GetLerpValue(0, player.bossMaxProgress, player.bossTextProgress);
            float realProg = ((MathHelper.Clamp((1f - progress) * 3, 0, 1)));
            string text = player.bossTitle;
            int count = (int)(text.Length * realProg);
            string something = $"{text.Substring(0, count)}";
            return something;
        }
        public static string GetDialogueText()
        {
            var player = Main.LocalPlayer.GetModPlayer<EbonianPlayer>();
            float progress = Utils.GetLerpValue(0, player.dialogueMax, player.dialogueProg);
            float realProg = ((MathHelper.Clamp((1f - progress) * 5, 0, 1)));
            string text = player.dialogue;
            int count = (int)(text.Length * realProg);
            string something = $"{text.Substring(0, count)}";
            return something;
        }//OUTDATED
        public static void SetDialogue(int progress, string text, Color color)
        {
            EbonianPlayer player = Main.LocalPlayer.GetModPlayer<EbonianPlayer>();
            player.dialogueMax = progress;
            player.dialogueProg = progress;
            player.dialogue = text;
            player.dialogueColor = color;
        }//OUTDATED
        public static void DrawDialogue()
        {
            EbonianPlayer player = Main.LocalPlayer.GetModPlayer<EbonianPlayer>();
            if (player.dialogueProg > 0)
            {
                float progress = Utils.GetLerpValue(0, player.dialogueMax, player.dialogueProg);
                float alpha = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 3, 0, 1);
                string text = GetDialogueText();
                Main.spriteBatch.Reload(BlendState.Additive);
                Main.spriteBatch.Draw(ModContent.Request<Texture2D>("EbonianMod/Extras/textGlow").Value, new Vector2(Main.screenWidth / 2, (int)(Main.screenHeight * 0.2f)), null, player.dialogueColor * alpha * 0.5f, 0f, new Vector2(256) / 2, new Vector2(10, 3f), SpriteEffects.None, 0f);
                Main.spriteBatch.Reload(BlendState.AlphaBlend);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, text, new Vector2(100, Main.screenHeight * 0.2f), player.dialogueColor * alpha, 0, new Vector2(0.5f, 0.5f), new Vector2(1f, 1f), Main.screenWidth - 100);
                Main.spriteBatch.Reload(Main.DefaultSamplerState);
            }
        } //OUTDATED
        public static void QuickDustLine(Vector2 start, Vector2 end, float splits, Color color)
        {
            Dust.QuickDust(start, color).scale = 1f;
            Dust.QuickDust(end, color).scale = 1f;
            float num = 1f / splits;
            for (float amount = 0.0f; (double)amount < 1.0; amount += num)
                Dust.QuickDustSmall(Vector2.Lerp(start, end, amount), color).scale = 1f;
        }
        public static void QuickDustLine(this Dust dust, Vector2 start, Vector2 end, float splits, Color color1, Color color2)
        {
            Dust.QuickDust(start, color1).scale = 1f;
            Dust.QuickDust(end, color2).scale = 1f;
            float num = 1f / splits;
            for (float amount = 0.0f; (double)amount < 1.0; amount += num)
            {
                Color color = Color.Lerp(color1, color2, amount);
                Dust.QuickDustSmall(Vector2.Lerp(start, end, amount), color).scale = 1f;
            }
        }
        public static void DrawBossTitle()
        {
            var player = Main.LocalPlayer.GetModPlayer<EbonianPlayer>();
            if (player.bossTextProgress > 0)
            {
                float progress = Utils.GetLerpValue(0, player.bossMaxProgress, player.bossTextProgress);
                float alpha = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 3, 0, 1);
                string something = GetBossText();
                switch (player.bossStyle)
                {
                    case -69:
                        Main.spriteBatch.Draw(GetExtraTexture("textGlow"), new Vector2(Main.screenWidth / 2, Main.screenHeight * 0.2f), null, Color.Black * alpha, 0, GetExtraTexture("textGlow").Size() / 2, 10f, SpriteEffects.None, 0);
                        if (player.bossTitle != null)
                            DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.Value, something, new Vector2(Main.screenWidth / 2 - FontAssets.MouseText.Value.MeasureString(something).X / 2, Main.screenHeight * 0.2f), player.bossColor * alpha);
                        DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.MouseText.Value, player.bossName, new Vector2(Main.screenWidth / 2 - FontAssets.DeathText.Value.MeasureString(player.bossName).X / 2, Main.screenHeight * 0.175f), Color.SaddleBrown * alpha);
                        break;
                    case -2:
                        Main.spriteBatch.Draw(GetExtraTexture("textGlow"), new Vector2(Main.screenWidth / 2, Main.screenHeight * 0.2f), null, Color.Black * alpha, 0, GetExtraTexture("textGlow").Size() / 2, 10f, SpriteEffects.None, 0);
                        if (player.bossTitle != null)
                            DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.Value, something, new Vector2(Main.screenWidth / 2 - FontAssets.MouseText.Value.MeasureString(something).X / 2, Main.screenHeight * 0.2f), player.bossColor * alpha);
                        DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.MouseText.Value, player.bossName, new Vector2(Main.screenWidth / 2 - FontAssets.DeathText.Value.MeasureString(player.bossName).X / 2, Main.screenHeight * 0.175f), player.bossColor * alpha);
                        break;
                    case -1:
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.DeathText.Value, player.bossName, new Vector2(Main.screenWidth / 2 - FontAssets.DeathText.Value.MeasureString(player.bossName).X / 2, Main.screenHeight * 0.25f), player.bossColor * alpha, 0, new Vector2(0.5f, 0.5f), new Vector2(1f, 1f));
                        if (player.bossTitle != null)
                        {
                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, player.bossTitle, new Vector2(Main.screenWidth / 2 - FontAssets.MouseText.Value.MeasureString(player.bossTitle).X / 2, Main.screenHeight * 0.225f), player.bossColor * alpha, 0, new Vector2(0.5f, 0.5f), new Vector2(1f, 1f));
                        }
                        break;
                    case 0:
                        MiscDrawingMethods.DrawTerrorTitle();
                        break;

                        /*case 0:
                            BossTitles.DrawOracleTitle();
                            break;
                        case 1:
                            BossTitles.DrawVagrantTitle();
                            break;
                        case 2:
                            BossTitles.DrawSSWTitle();
                            break;*/
                }

            }
        }
    }
    public class MiscDrawingMethods
    {
        public static void DrawTerrorTitle()
        {
            var player = Main.LocalPlayer.GetModPlayer<EbonianPlayer>();
            if (player.bossTextProgress > 0)
            {
                float progress = Utils.GetLerpValue(0, player.bossMaxProgress, player.bossTextProgress);
                float alpha = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 3, 0, 1);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.DeathText.Value, player.bossName, new Vector2(Main.screenWidth / 2 - FontAssets.DeathText.Value.MeasureString(player.bossName).X / 2, Main.screenHeight * 0.25f), player.bossColor * alpha, 0, new Vector2(0.5f, 0.5f), new Vector2(1f, 1f));

                if (player.bossTitle != null)
                {
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, player.bossTitle, new Vector2(Main.screenWidth / 2 - FontAssets.MouseText.Value.MeasureString(player.bossTitle).X / 2, Main.screenHeight * 0.225f), player.bossColor * alpha, 0, new Vector2(0.5f, 0.5f), new Vector2(1f, 1f));
                }
            }
        }
        public static readonly BlendState Subtractive = new BlendState
        {
            ColorSourceBlend = Blend.SourceAlpha,
            ColorDestinationBlend = Blend.One,
            ColorBlendFunction = BlendFunction.ReverseSubtract,
            AlphaSourceBlend = Blend.SourceAlpha,
            AlphaDestinationBlend = Blend.One,
            AlphaBlendFunction = BlendFunction.ReverseSubtract
        };
        public readonly static BlendState AlphaSubtractive = new BlendState
        {
            ColorSourceBlend = Blend.SourceAlpha,
            AlphaSourceBlend = Blend.SourceAlpha,
            ColorDestinationBlend = Blend.One,
            AlphaDestinationBlend = Blend.One,
            ColorBlendFunction = BlendFunction.ReverseSubtract,
            AlphaBlendFunction = BlendFunction.ReverseSubtract
        };
        public static void DrawWithDye(SpriteBatch spriteBatch, DrawData data, int dye, Entity entity, bool Additive = false)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, Additive ? BlendState.Additive : BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
            //DrawData a = new(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2, 1, SpriteEffects.None, 0);
            GameShaders.Armor.GetShaderFromItemId(dye).Apply(entity, data);
            data.Draw(Main.spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
        }
        public static void DrawShinyText(DrawableTooltipLine line)
        {
            var font = FontAssets.MouseText.Value;

            DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, font, line.Text, new Vector2(line.X, line.Y), line.Color * 0.5f);
            MiscDrawingMethods.LocalDrawShinyText(line, 0.5f);
        }
        public static void LocalDrawShinyText(DrawableTooltipLine line, float opacity = 1)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, EbonianMod.CrystalShine, Main.UIScaleMatrix);
            var font = FontAssets.MouseText.Value;
            EbonianMod.CrystalShine.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            EbonianMod.CrystalShine.Parameters["uOpacity"].SetValue(opacity);
            DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, font, line.Text, new Vector2(line.X, line.Y), Color.White);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
        }
        public static void LocalDrawShinyText(string text, Vector2 pos, float opacity = 1)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, EbonianMod.CrystalShine, Main.UIScaleMatrix);
            var font = FontAssets.MouseText.Value;
            EbonianMod.CrystalShine.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            EbonianMod.CrystalShine.Parameters["uOpacity"].SetValue(opacity);
            DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, font, text, pos, Color.White);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
        }
    }
}