using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using EbonianMod.Dusts;
using Terraria.DataStructures;
using EbonianMod.Projectiles.VFXProjectiles;
using Terraria.Utilities;
using System.Reflection.Metadata;
using EbonianMod.NPCs.Conglomerate;
using System.ComponentModel.DataAnnotations.Schema;
using Terraria.Graphics.CameraModifiers;

namespace EbonianMod.Projectiles.Conglomerate
{
    public class CBeam : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 135;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override string Texture => "EbonianMod/Extras/Empty";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 7000;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
        }
        int damage;
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            if (Type == ProjectileType<CBeamSmall>() && Projectile.timeLeft < 80)
                return false;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[0], 60 * Projectile.ai[1], ref a) && Projectile.scale > 0.5f;
        }
        public bool RunOnce = true;
        public float flareAlpha = 1;
        public override bool PreAI()
        {
            NPC owner = Main.npc[(int)Projectile.ai[2]];

            if (owner.active && owner.type == ModContent.NPCType<NPCs.Conglomerate.Conglomerate>())
            {
                Projectile.velocity = (owner.rotation + PiOver2).ToRotationVector2();
                Projectile.Center = owner.Center;
            }
            return base.PreAI();
        }
        public Vector2 startVel;
        public override void AI()
        {
            if (RunOnce)
            {
                Projectile.velocity.Normalize();
                startVel = Projectile.velocity;
                Projectile.rotation = Projectile.velocity.ToRotation();
                RunOnce = false;
            }
            if (Projectile.timeLeft == 131)
            {
                Projectile.NewProjectile(null, Projectile.Center + Projectile.velocity * 30, Vector2.Zero, ModContent.ProjectileType<BlurScream>(), 0, 0, -1, 0, 0, Projectile.whoAmI);
                Projectile.NewProjectile(null, Projectile.Center + Projectile.velocity * 30, Vector2.Zero, ModContent.ProjectileType<CFlareExplosion>(), 0, 0, -1, 0, 0, Projectile.whoAmI);
                EbonianSystem.ScreenShakeAmount = 15;
            }
            if (Projectile.timeLeft % 2 == 0 && Projectile.timeLeft > 5)
            {
                if (EbonianSystem.ScreenShakeAmount < 13)
                    EbonianSystem.ScreenShakeAmount = 13;
                if (Projectile.timeLeft % 8 == 0)
                    Projectile.NewProjectile(null, Projectile.Center + Projectile.velocity * 30, Vector2.Zero, ModContent.ProjectileType<BlurScream>(), 0, 0, -1, 0, 0, Projectile.whoAmI);
                Projectile.NewProjectile(null, Projectile.Center + Projectile.velocity * 30, Vector2.Zero, ModContent.ProjectileType<CFlareExplosion>(), 0, 0, -1, 0, 0, Projectile.whoAmI);
            }
            NPC owner = Main.npc[(int)Projectile.ai[2]];

            if (owner.active && owner.type == ModContent.NPCType<NPCs.Conglomerate.Conglomerate>())
            {
                Projectile.velocity = (owner.rotation + PiOver2 + Projectile.ai[0]).ToRotationVector2();
                Projectile.Center = owner.Center;
            }
            if (Projectile.timeLeft < 90)
                Projectile.ai[1] = Lerp(Projectile.ai[1], 0, 0.025f);
            for (int i = 0; i < 5; i++)
            {
                float fac = Main.rand.NextFloat(0.2f, 1);

                Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(6) ? DustType<SparkleDust>() : DustType<LineDustFollowPoint>(), Projectile.velocity.RotatedByRandom(PiOver4 / fac) * Main.rand.NextFloat(25, 50), newColor: Color.Lerp(Color.Maroon, Color.LawnGreen, Main.rand.NextFloat()) * 5 * fac, Scale: Main.rand.NextFloat(0.1f, 0.2f) * fac);
            }
            flareAlpha = Lerp(flareAlpha, 0, 0.05f);
            Projectile.localAI[0] = SmoothStep(Projectile.localAI[0], 2048, 0.35f);
            startSize = Lerp(startSize, 0, 0.01f);
            //Projectile.velocity = -Projectile.velocity.RotatedBy(ToRadians(Projectile.ai[1]));
            Projectile.rotation = Projectile.velocity.ToRotation();
            float progress = Utils.GetLerpValue(0, 135, Projectile.timeLeft);
            if (Projectile.timeLeft < 135)
                Projectile.scale = Clamp((float)Math.Sin(progress * Math.PI) * 5 * (Projectile.scale + 0.5f), 0, 1);
        }
        public float visual1, visual2, visual3, startSize = 2f, coloredFlareAlpha = 1f, flareScaleMult = 1f, shakeFac = 10,
            additionalAlphaOffset = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            visual1 -= 0.035f;
            visual2 -= 0.02425f;
            visual3 -= 0.0446f;

            Texture2D texture = (Projectile.type == ProjectileType<CBeamSmall>() ? (Projectile.damage == 0 ? ExtraTextures.laser3 : ExtraTextures.trail_04) : ExtraTextures.LintyTrail);
            Texture2D texture2 = Projectile.type == ProjectileType<CBeamSmall>() ? (Projectile.damage == 0 ? ExtraTextures.laser3 : ExtraTextures.trail_04) : ExtraTextures.trail_04;
            Texture2D texture3 = Projectile.type == ProjectileType<CBeamSmall>() ? ExtraTextures.Tentacle : ExtraTextures.FlamesSeamless;
            Texture2D tex = ExtraTextures.explosion;
            Texture2D tex3 = ExtraTextures2.light_01;

            float progress = Utils.GetLerpValue(0, 135, Projectile.timeLeft);
            float i_progress = Clamp(SmoothStep(1, 0.2f, progress) * 50, 0, 1 / Clamp(startSize, 1, 2)) * (1 + Projectile.ai[1]);

            float alpha = (0.35f + MathF.Sin(Main.GlobalTimeWrappedHourly * 6 + Projectile.whoAmI) * 0.1f) * Projectile.scale;

            Texture2D tex2 = ExtraTextures.crosslight;
            if (Projectile.timeLeft < 134)
            {
                Main.spriteBatch.Reload(BlendState.Additive);

                Vector2 scale = new Vector2(1, 0.5f);
                startVel = Vector2.Lerp(startVel, Projectile.velocity, 0.5f);
                float rot = startVel.ToRotation() + PiOver2;
                for (int i = 0; i < (Type == ProjectileType<CBeamSmall>() ? 1 : 2); i++)
                {
                    Main.spriteBatch.Draw(tex2, Projectile.Center + Projectile.velocity * 40 - Main.screenPosition, null, Color.White * alpha * 1.4f * coloredFlareAlpha, rot, tex2.Size() / 2, scale * 2, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(tex2, Projectile.Center + Projectile.velocity * 40 - new Vector2(tex2.Width, 0).RotatedBy(Projectile.velocity.ToRotation() - PiOver2) - Main.screenPosition, new Rectangle(tex2.Width / 2, 0, tex.Width / 2, tex.Height), Color.Maroon * coloredFlareAlpha * alpha * 2, rot, tex2.Size() / 2, scale * 2, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(tex2, Projectile.Center + Projectile.velocity * 40 - Main.screenPosition, new Rectangle(0, 0, tex.Width / 2, tex.Height), Color.LawnGreen * alpha * 2 * coloredFlareAlpha, rot, tex2.Size() / 2, scale * 2, SpriteEffects.None, 0);
                }

                for (int i = 0; i < (Type == ProjectileType<CBeamSmall>() ? 1 : 2); i++)
                {
                    Main.spriteBatch.Draw(tex2, Projectile.Center + Projectile.velocity * 40 - Main.screenPosition, null, Color.White * flareAlpha * alpha * 2 * (Type == ProjectileType<CBeamSmall>() ? Clamp((startSize - 0.5f) * 2, 0, 1) * 5 : 1), rot + PiOver2, tex2.Size() / 2, scale * 5 * flareScaleMult, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(tex2, Projectile.Center + Projectile.velocity * 40 - Main.screenPosition, null, Color.Maroon * flareAlpha * alpha * 2 * (Type == ProjectileType<CBeamSmall>() ? Clamp((startSize - 0.5f) * 2, 0, 1) * 5 : 1), rot, tex2.Size() / 2, scale * 5 * flareScaleMult, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(tex2, Projectile.Center + Projectile.velocity * 40 - Main.screenPosition, null, Color.LawnGreen * flareAlpha * alpha * 2 * (Type == ProjectileType<CBeamSmall>() ? Clamp((startSize - 0.5f) * 2, 0, 1) * 5 : 1), rot, tex2.Size() / 2, scale * 5 * flareScaleMult, SpriteEffects.None, 0);
                }


                for (int i = 0; i < (Type == ProjectileType<CBeamSmall>() ? 1 : 3); i++)
                {
                    Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.Red, Color.LimeGreen, 0.5f) * alpha * coloredFlareAlpha * 0.5f, 0, tex.Size() / 2, 5 + Projectile.scale, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(tex3, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.Red, Color.LimeGreen, 0.5f) * alpha * coloredFlareAlpha * 0.5f, 0, tex3.Size() / 2, 5 + Projectile.scale, SpriteEffects.None, 0);
                }
                Main.spriteBatch.Reload(BlendState.AlphaBlend);

                DrawVertices(Projectile.Center, Projectile.velocity.ToRotation(), texture, texture2, i_progress, 5 * additionalAlphaOffset, (Type == ProjectileType<CBeamSmall>() ? 0.05f : 0.0025f), 1, visual1);
                DrawVertices(Projectile.Center, Projectile.velocity.ToRotation(), texture3, texture, i_progress, 5 * additionalAlphaOffset, (Type == ProjectileType<CBeamSmall>() ? 0.05f : 0.0025f), 1, visual2);
            }
            return false;
        }
        void DrawVertices(Vector2 pos, float rotation, Texture2D texture, Texture2D texture2, float i_progress, float alphaOffset, float quality = 0.002f, float max = 1, float visOff = 0)
        {
            Main.spriteBatch.SaveCurrent();
            List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
            List<VertexPositionColorTexture> vertices2 = new List<VertexPositionColorTexture>();
            List<VertexPositionColorTexture> vertices3 = new List<VertexPositionColorTexture>();
            Vector2 start = pos + (shakeFac == 0 ? Vector2.Zero : Main.rand.NextVector2Circular(shakeFac, shakeFac)) - Projectile.velocity * 75 - Main.screenPosition;
            Vector2 off = (rotation.ToRotationVector2() * (2548 * (Projectile.type == ProjectileType<CBeamSmall>() ? Lerp(0, Clamp(startSize, 1, 1.5f), Projectile.scale) : 1)));
            Vector2 end = start + off;
            float rot = Helper.FromAToB(start, end).ToRotation();

            float s = 0.5f;
            for (float i = 1 - max; i < max; i += quality)
            {
                if (i < max / 2)
                    s = Clamp(i * 5f, 0, 0.5f);
                else
                    s = Clamp((-i + max) * 2, 0, 0.5f);

                float _off = MathF.Abs(visOff + i);

                float sinFac = InOutCirc.Invoke((MathF.Sin(Main.GlobalTimeWrappedHourly * 16) + 1) * 0.5f);
                Color col = Color.Lerp(Color.Red, Color.Red * 1.2f, i) * (s * s * 3f * alphaOffset) * (Type == ProjectileType<CBeamSmall>() ? startSize : 1);
                float endSize = SmoothStep(420 + sinFac * 50, 600, InOutQuint.Invoke(i));

                if (Type == ProjectileType<CBeamSmall>())
                    endSize = Lerp(SmoothStep(320 + sinFac * 50, 600, InOutQuint.Invoke(i)), 0, Clamp(InOutSine.Invoke(i), 0, 1)) * Projectile.scale;

                vertices.Add(Helper.AsVertex(start + off * i + new Vector2(SmoothStep(Lerp(60 + sinFac * 10, 100 + sinFac * 50, i) * startSize, endSize, i * 3) * Clamp(startSize, 1, 2), 0).RotatedBy(rot + PiOver2) * i_progress, new Vector2(_off, 1), col * Projectile.scale));

                sinFac = InOutCirc.Invoke((MathF.Sin(Main.GlobalTimeWrappedHourly * 25) + 1) * 0.5f);
                col = Color.Lerp(Color.Green, Color.Green * 1.2f, i) * (s * s * 3f * alphaOffset) * (Type == ProjectileType<CBeamSmall>() ? startSize : 1);
                vertices.Add(Helper.AsVertex(start + off * i + new Vector2(SmoothStep(Lerp(60 + sinFac * 10, 100 + sinFac * 50, i) * startSize, endSize, i * 3) * Clamp(startSize, 1, 2), 0).RotatedBy(rot - PiOver2) * i_progress, new Vector2(_off, 0), col * Projectile.scale));

                _off = MathF.Abs((visOff == visual1 ? visual2 : visual1) + i);

                sinFac = InOutCirc.Invoke((MathF.Sin(Main.GlobalTimeWrappedHourly * 10) + 1) * 0.5f);
                col = Color.White * (s * s * alphaOffset * 0.8f * SmoothStep(1, 0, i));

                if (Projectile.type == ProjectileType<CBeamSmall>())
                    col = Color.White * (s * s * alphaOffset * 0.8f * SmoothStep(4, 0, i)) * startSize;
                vertices2.Add(Helper.AsVertex(start + off * i + new Vector2(Lerp(Lerp(10 + sinFac * 4, 200 + sinFac * 90, i) * startSize, endSize, i * 2) * Clamp(startSize, 1, 2), 0).RotatedBy(rot + PiOver2) * i_progress, new Vector2(_off, 1), col * Projectile.scale));

                sinFac = InOutCirc.Invoke((MathF.Sin(Main.GlobalTimeWrappedHourly * 9) + 1) * 0.5f);
                vertices2.Add(Helper.AsVertex(start + off * i + new Vector2(Lerp(Lerp(10 + sinFac * 4, 200 + sinFac * 90, i) * startSize, endSize, i * 2) * Clamp(startSize, 1, 2), 0).RotatedBy(rot - PiOver2) * i_progress, new Vector2(_off, 0), col * Projectile.scale));

                _off = MathF.Abs(visual3 + i);
                col = Color.White;
                endSize = SmoothStep(500 + sinFac * 50, 690, InOutQuint.Invoke(i));
                //if (Type == ProjectileType<CBeamSmall>())
                //  endSize = Lerp(SmoothStep(420 + sinFac * 50, 600, InOutQuint.Invoke(i)), 20, InOutSine.Invoke(i)) * Projectile.scale;

                if (Type != ProjectileType<CBeamSmall>())
                {
                    vertices3.Add(Helper.AsVertex(start + off * i + new Vector2(Lerp(Lerp(20, 280 + sinFac * 90, i) * startSize, endSize, i * 2) * Clamp(startSize, 1, 2), 0).RotatedBy(rot - PiOver2) * i_progress, new Vector2(_off, 0), col));
                    vertices3.Add(Helper.AsVertex(start + off * i + new Vector2(Lerp(Lerp(20, 280 + sinFac * 90, i) * startSize, endSize, i * 2) * Clamp(startSize, 1, 2), 0).RotatedBy(rot + PiOver2) * i_progress, new Vector2(_off, 1), col));
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (vertices.Count >= 3 && vertices2.Count >= 3 && (Type != ProjectileType<CBeamSmall>() ? vertices3.Count >= 3 : true))
            {
                for (int i = 0; i < (Type == ProjectileType<CBeamSmall>() ? 1 : 3); i++)
                {
                    Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, texture, false);
                    Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, texture2, false);
                }

                if (Type != ProjectileType<CBeamSmall>())
                {
                    EbonianMod.affectedByInvisibleMaskCache.Add(() =>
                    {
                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                        Helper.DrawTexturedPrimitives(vertices2.ToArray(), PrimitiveType.TriangleStrip, ExtraTextures.swirlyNoise, false);
                        Helper.DrawTexturedPrimitives(vertices2.ToArray(), PrimitiveType.TriangleStrip, Helper.GetExtraTexture("vein"), false);
                    });
                    EbonianMod.invisibleMaskCache.Add(() => Helper.DrawTexturedPrimitives(vertices3.ToArray(), PrimitiveType.TriangleStrip, ExtraTextures.laserMask, false));
                }
                Helper.DrawTexturedPrimitives(vertices2.ToArray(), PrimitiveType.TriangleStrip, texture, false);

            }
            Main.spriteBatch.ApplySaved();
        }
    }
    public class CBeamSmall : CBeam
    {
        public override void AI()
        {
            if (RunOnce)
            {
                if (Projectile.damage == 0)
                    additionalAlphaOffset = 0.1f;
                coloredFlareAlpha = 0;
                shakeFac = 2;
                if (Projectile.damage == 0)
                    flareAlpha = 0;
                else
                {
                    Projectile.extraUpdates = 1;
                    flareAlpha = 0.8f;
                }
                flareScaleMult = 0.35f;
                Projectile.velocity.Normalize();
                startVel = Projectile.velocity;
                Projectile.rotation = Projectile.velocity.ToRotation();
                RunOnce = false;
            }
            if (Projectile.timeLeft == 131)
            {
                if (Projectile.damage > 0)
                {
                    Projectile.NewProjectile(null, Projectile.Center + Projectile.velocity * 30, Vector2.Zero, ModContent.ProjectileType<ConglomerateScream>(), 0, 0, -1, 0, 0, Projectile.whoAmI);
                    Projectile.NewProjectile(null, Projectile.Center + Projectile.velocity * 30, Vector2.Zero, ModContent.ProjectileType<BlurScream>(), 0, 0, -1, 0, 0, Projectile.whoAmI);
                }
            }

            if (Projectile.timeLeft == 70 && Projectile.damage == 0)
            {
                if (EbonianSystem.conglomerateSkyFlash < 8)
                    EbonianSystem.conglomerateSkyFlash = 8f;
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Projectile.velocity, 13, 10, 30, 1000));
                SoundEngine.PlaySound(EbonianSounds.exolDash, Projectile.Center);
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity, Projectile.type, 30, 0, Projectile.owner, Projectile.ai[0], Projectile.ai[1] + 0.1f, Projectile.ai[2]);
            }
            NPC owner = Main.npc[(int)Projectile.ai[2]];

            if (owner.active && owner.type == ModContent.NPCType<NPCs.Conglomerate.Conglomerate>())
            {
                Projectile.velocity = (owner.rotation + PiOver2 + Projectile.ai[0]).ToRotationVector2();
                Projectile.Center = owner.Center;
            }
            if (Projectile.damage > 0 && Projectile.timeLeft > 70 && Projectile.timeLeft < 125)
                for (int i = 0; i < 2; i++)
                {
                    float fac = Main.rand.NextFloat(0.2f, 1);

                    Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * 200, DustType<LineDustFollowPoint>(), Projectile.velocity.RotatedByRandom((Projectile.ai[1] + 1) * (PiOver4 / fac)) * Main.rand.NextFloat(25, 50), newColor: Color.Lerp(Color.Maroon, Color.LawnGreen, Main.rand.NextFloat()) * 5 * fac, Scale: Main.rand.NextFloat(0.1f, 0.2f) * fac);
                }
            if (Projectile.timeLeft < 120 && Projectile.timeLeft > 20 && Projectile.damage > 0)
                flareAlpha = Lerp(flareAlpha, 0.25f, 0.1f);
            else if (Projectile.timeLeft <= 20)
                flareAlpha = Lerp(flareAlpha, 0, 0.1f);
            Projectile.localAI[0] = SmoothStep(Projectile.localAI[0], 2048, 0.35f);
            startSize = Lerp(startSize, 0, 0.02f);
            //Projectile.velocity = -Projectile.velocity.RotatedBy(ToRadians(Projectile.ai[1]));
            Projectile.rotation = Projectile.velocity.ToRotation();
            float progress = Utils.GetLerpValue(0, 135, Projectile.timeLeft);
            if (Projectile.timeLeft < 135)
                Projectile.scale = Clamp((float)Math.Sin(progress * Math.PI) * 1.5f * (Projectile.scale + 0.5f), 0, 1);
        }

    }
    public class CFlareExplosion : ModProjectile
    {
        public override string Texture => Helper.Placeholder;
        public override void SetDefaults()
        {
            Projectile.height = 1;
            Projectile.width = 1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;
            Projectile.tileCollide = false;
        }
        public override bool ShouldUpdatePosition() => false;
        int seed;
        public override void OnSpawn(IEntitySource source)
        {
            seed = Main.rand.Next(int.MaxValue);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (seed == 0) return false;
            Texture2D tex = ExtraTextures.cone5;
            UnifiedRandom rand = new UnifiedRandom(seed);
            float max = 40;
            Main.spriteBatch.Reload(BlendState.Additive);
            float ringScale = Lerp(1, 0, Clamp(vfxIncrement * 6.5f, 0, 1));
            if (ringScale > 0.01f)
            {
                for (float i = 0; i < max; i++)
                {
                    float angle = Helper.CircleDividedEqually(i, max);
                    float scale = rand.NextFloat(0.2f, 1.15f);
                    Vector2 offset = new Vector2(Main.rand.NextFloat(50, 100) * ringScale * scale, 0).RotatedBy(angle);
                    for (float j = 0; j < 2; j++)
                        Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.Lerp(Color.Red, Color.LimeGreen, rand.NextFloat()) * ringScale, angle, tex.Size() / 2, new Vector2(Clamp(vfxIncrement * 6.5f, 0, 1), ringScale) * scale * 0.4f, SpriteEffects.None, 0);
                }
            }

            float alpha = Lerp(0.5f, 0, Projectile.ai[1]) * 2;
            for (float i = 0; i < max; i++)
            {
                float angle = Helper.CircleDividedEqually(i, max);
                float scale = rand.NextFloat(0.2f, 1.15f);
                Vector2 offset = new Vector2(Main.rand.NextFloat(50) * Projectile.ai[1] * scale, 0).RotatedBy(angle);
                for (float j = 0; j < 2; j++)
                    Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.Lerp(Color.Red, Color.LimeGreen, rand.NextFloat()) * alpha * 2, angle, tex.Size() / 2, new Vector2(Projectile.ai[1], alpha) * scale * 1.5f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        float vfxIncrement;
        public override void AI()
        {
            vfxIncrement = Lerp(vfxIncrement, 0.2f, 0.18f);
            Projectile.ai[0] = Lerp(Projectile.ai[0], Projectile.ai[0] + vfxIncrement + (Projectile.ai[0] * 0.075f), 0.5f);
            Projectile.ai[1] = SmoothStep(0, 1.5f, Projectile.ai[0]);

            Projectile owner = Main.projectile[(int)Projectile.ai[2]];
            if (owner.active && owner.type == ModContent.ProjectileType<CBeam>())
            {
                Projectile.Center = owner.Center;
            }

            if (Projectile.timeLeft >= 190 && Projectile.timeLeft < 194)
            {
                UnifiedRandom rand = new UnifiedRandom(seed);
                float max = 10 + ((Projectile.timeLeft - 190) * 10);
                for (int i = ((Projectile.timeLeft - 190) * 10); i < max; i++)
                {
                    float angle = Helper.CircleDividedEqually(i, max);
                    float scale = rand.NextFloat(1f - (Projectile.timeLeft - 190) * 0.2f);
                    Vector2 offset = new Vector2(Main.rand.NextFloat(50) * scale, 0).RotatedBy(angle);
                    int jMax = rand.Next(3, 5);
                    //for (int j = 0; j < jMax; j++)
                    //  Dust.NewDustPerfect(Projectile.Center + offset * 0.5f, Main.rand.NextBool() ? DustID.IchorTorch : DustID.CursedTorch, Helper.FromAToB(Projectile.Center, Projectile.Center + offset).RotatedByRandom(PiOver4 * (j == 0 ? 0 : 1)) * (scale * 20)).noGravity = true;
                }
            }

            if (Projectile.ai[1] > 1.15f)
                Projectile.Kill();
        }
    }
}
