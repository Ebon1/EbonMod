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
using EbonianMod.Dusts;
using Terraria.ID;

namespace EbonianMod.Projectiles.VFXProjectiles
{
    public class ReiCapeP : ModProjectile
    {
        public override string Texture => Helper.Empty;
        Verlet[] verlet = new Verlet[9];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            for (int i = 0; i < 9; i++)
                verlet[i] = new Verlet(player.RotatedRelativePoint(player.MountedCenter) - new Vector2(0, 5), 1, 20, 0.5f, true, false, 20, false);

            for (int i = 0; i < smoke.Length; i++)
            {
                Smoke dust = smoke[i];
                dust.position = new Vector2(0, player.height / 2 - 10);
                dust.velocity = new Vector2(-player.velocity.X * Main.rand.NextFloat(0, 0.1f) + Main.rand.NextFloat(0, 2f) * -player.direction, Main.rand.NextFloat(-2f, -0.25f));
                dust.scale = Main.rand.NextFloat(0.01f, 0.05f);
            }
        }
        public override void SetDefaults()
        {
            Projectile.Size = Vector2.One * 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }
        public struct Smoke
        {
            public float scale;
            public Vector2 position; //is actually offset
            public Vector2 velocity;
        }
        public Smoke[] smoke = new Smoke[250];
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        void UpdateSmoke()
        {
            Player player = Main.player[Projectile.owner];
            for (int i = 0; i < smoke.Length; i++)
            {
                smoke[i].position -= smoke[i].velocity;
                smoke[i].scale -= 0.0005f;
                smoke[i].velocity *= 0.95f;
                if (smoke[i].scale < 0.005f)
                    smoke[i].velocity *= 0.85f;
                if (smoke[i].scale <= 0)
                {
                    smoke[i].position = new Vector2(0, player.height / 2 - 10);
                    smoke[i].velocity = new Vector2(-player.velocity.X * Main.rand.NextFloat(0, 0.2f) + Main.rand.NextFloat(0, 2f) * -player.direction, Main.rand.NextFloat(-2f, -0.25f) + MathHelper.Lerp(0, 1f, MathHelper.Clamp(player.velocity.X * (player.velocity.X < 0 ? -1 : 1) * 0.1f, 0, 1f)));
                    smoke[i].scale = Main.rand.NextFloat(0.01f, 0.05f);
                }
            }
        }
        void DrawSmoke(SpriteBatch sb)
        {
            Player player = Main.player[Projectile.owner];
            //sb.Reload(MiscDrawingMethods.Subtractive);
            for (int i = 0; i < smoke.Length; i++)
            {
                Smoke d = smoke[i];
                Texture2D tex = ModContent.Request<Texture2D>("EbonianMod/Extras/explosion").Value;
                sb.Draw(tex, player.RotatedRelativePoint(player.MountedCenter) - d.position - Main.screenPosition, null, Color.White * d.scale * 10, 0, tex.Size() / 2, d.scale, SpriteEffects.None, 0);
            }
            //sb.Reload(BlendState.AlphaBlend);
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Main.player[Projectile.owner].GetModPlayer<EbonianPlayer>().rei || Main.player[Projectile.owner].GetModPlayer<EbonianPlayer>().reiV)
                Projectile.timeLeft = 10;
            UpdateSmoke();
            //for (int i = 0; i < 2; i++)
            //    Dust.NewDustPerfect(player.RotatedRelativePoint(player.MountedCenter) - new Vector2(0, player.height / 2 - 10), ModContent.DustType<ReiSmoke>(), new Vector2(-player.velocity.X * Main.rand.NextFloat(-0.1f, 0.1f) + Main.rand.NextFloat(-0.5f, 2f) * -player.direction, Main.rand.NextFloat(-2f, -0.25f))).scale = Main.rand.NextFloat(0.01f, 0.05f);
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + new Vector2(-5, 19);
            Projectile.rotation = player.velocity.ToRotation();
            if (verlet[0] != null)
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 2; j++)
                        verlet[i].Update(player.RotatedRelativePoint(player.MountedCenter) - new Vector2((i - 4) * 1.5f, 5), Projectile.Center);
                    verlet[i].lastP.position -= Vector2.UnitX * (i - 4) * 1.1f;
                    verlet[i].firstP.position = player.RotatedRelativePoint(player.MountedCenter) - new Vector2((i - 4) * 1.5f, 5);
                }
            }
        }
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            if (verlet[0] != null)
            {
                for (int i = 0; i < 9; i++)
                {
                    verlet[i].firstP.position = player.RotatedRelativePoint(player.MountedCenter) - new Vector2((i - 4) * 1.5f, 5);
                }
            }
            return true;
        }
        public override void PostAI()
        {
            Player player = Main.player[Projectile.owner];
            if (verlet[0] != null)
            {
                for (int i = 0; i < 9; i++)
                {
                    verlet[i].firstP.position = player.RotatedRelativePoint(player.MountedCenter) - new Vector2((i - 4) * 1.5f, 5);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            if (Main.gameInactive || Main.gamePaused)
                return true;
            if (verlet[0] != null)
            {
                for (int k = 0; k < 9; k++)
                {
                    float len = new Vector2(player.velocity.X, 0).Length();
                    verlet[k].Draw(Main.spriteBatch, "Extras/Line", scale: MathHelper.Lerp(2.5f, 0.9f, MathHelper.Clamp(len * 0.2f, 0, 1f)), endTex: "Extras/Empty", scaleCalcForDist: true, clampScaleCalculationForDistCalculation: true);
                }
            }
            return true;
        }
        public override bool PreDrawExtras()
        {
            Player player = Main.player[Projectile.owner];
            Lighting.AddLight(player.Center, TorchID.Purple);

            //if (Main.gamePaused || Main.gameInactive)
            //  return true;
            if (verlet[0] != null)
            {
                for (int k = 0; k < 9; k++)
                {
                    float len = new Vector2(player.velocity.X, 0).Length();
                    verlet[k].Draw(Main.spriteBatch, "Extras/Line", useColor: true, color: Color.Black, scale: MathHelper.Lerp(2.5f, 0.85f, MathHelper.Clamp(len * 0.2f, 0, 1f)) + 1, endTex: "Extras/Empty", scaleCalcForDist: true, clampScaleCalculationForDistCalculation: true);
                }
            }
            return true;
        }
        public override void PostDraw(Color lightColor)
        {
            if (lightColor == new Color(69, 420, 0, 1))
                DrawSmoke(Main.spriteBatch);
        }
    }
    public class ReiCapeTrail : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.Size = Vector2.One * 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Main.player[Projectile.owner].GetModPlayer<EbonianPlayer>().rei)
                Projectile.timeLeft = 10;
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + new Vector2(5 * Projectile.ai[0], 19);
            Projectile.rotation = player.velocity.ToRotation();
            if (player.GetModPlayer<EbonianPlayer>().reiBoostCool == 59)
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                    Projectile.oldPos[i] = Projectile.Center;
        }
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            (default(ReiTrail)).Draw(Projectile);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
    public class ReiExplosion : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.height = 200;
            Projectile.width = 200;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 100;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Helper.GetExtraTexture("explosion");
            Texture2D tex2 = Helper.GetExtraTexture("flameEye2");
            Main.spriteBatch.Reload(BlendState.Additive);
            float alpha = MathHelper.Lerp(2, 0, Projectile.ai[0]);
            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, Color.Cyan * alpha, Projectile.rotation, tex2.Size() / 2, Projectile.ai[0], SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, tex2.Size() / 2, Projectile.ai[0] * 0.3f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Mushroom);
            Projectile.ai[0] += 0.05f;
            if (Projectile.ai[0] > 1)
                Projectile.Kill();
        }
    }
}
