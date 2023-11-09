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
            for (int i = 0; i < 9; i++)
                verlet[i] = new Verlet(Main.player[Projectile.owner].Center - new Vector2(0, 5), 1, 20, 0.5f, true, false, 20, false);
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
            for (int i = 0; i < 5; i++)
                Dust.NewDustPerfect(player.Center - new Vector2(0, player.height / 2 - 10), ModContent.DustType<ReiSmoke>(), new Vector2(-player.velocity.X * Main.rand.NextFloat(-0.1f, 0.1f) + Main.rand.NextFloat(-0.5f, 2f) * -player.direction, Main.rand.NextFloat(-2f, -0.25f))).scale = Main.rand.NextFloat(0.01f, 0.05f);
            Projectile.Center = Main.player[Projectile.owner].Center + new Vector2(-5, 19);
            Projectile.rotation = player.velocity.ToRotation();
            if (verlet[0] != null)
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 2; j++)
                        verlet[i].Update(Main.player[Projectile.owner].Center - new Vector2((i - 4) * 1.5f, 5), Projectile.Center);
                    verlet[i].lastP.position -= Vector2.UnitX * (i - 4) * 1.1f;
                    verlet[i].firstP.position = Main.player[Projectile.owner].Center - new Vector2((i - 4) * 1.5f, 5);
                }
            }
        }
        public override bool PreAI()
        {
            if (verlet[0] != null)
            {
                for (int i = 0; i < 9; i++)
                {
                    verlet[i].firstP.position = Main.player[Projectile.owner].Center - new Vector2((i - 4) * 1.5f, 5);
                }
            }
            return true;
        }
        public override void PostAI()
        {
            if (verlet[0] != null)
            {
                for (int i = 0; i < 9; i++)
                {
                    verlet[i].firstP.position = Main.player[Projectile.owner].Center - new Vector2((i - 4) * 1.5f, 5);
                }
            }
        }
        public override void PostDraw(Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            if (lightColor != Color.Transparent)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                (default(ReiTrail)).Draw(Projectile);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
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
                    float len = new Vector2(player.velocity.X, 0).Length();
                    if (lightColor != Color.Transparent)
                        verlet[k].Draw(Main.spriteBatch, "Extras/Line", useColor: true, color: Color.Black, scale: MathHelper.Lerp(2.5f, 0.85f, MathHelper.Clamp(len * 0.2f, 0, 1f)) + 1, endTex: "Extras/Empty", scaleCalcForDist: true, clampScaleCalculationForDistCalculation: true);
                    verlet[k].Draw(Main.spriteBatch, "Extras/Line", scale: MathHelper.Lerp(2.5f, 0.9f, MathHelper.Clamp(len * 0.2f, 0, 1f)), endTex: "Extras/Empty", scaleCalcForDist: true, clampScaleCalculationForDistCalculation: true);
                    //Main.spriteBatch.Draw(Helper.GetExtraTexture("Line"), Main.player[Projectile.owner].Center - new Vector2((k - 4) * 1.75f, 5) - Main.screenPosition, null, Color.White, Helper.FromAToB(Main.player[Projectile.owner].Center - new Vector2((k - 4) * 1.75f, 5), verlet[k].firstP.position).ToRotation(), Helper.GetExtraTexture("Line").Size() / 2, 3.5f, SpriteEffects.None, 0);
                }
            }
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
            Projectile.Center = Main.player[Projectile.owner].Center + new Vector2(5, 19);
            Projectile.rotation = player.velocity.ToRotation();
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
}
