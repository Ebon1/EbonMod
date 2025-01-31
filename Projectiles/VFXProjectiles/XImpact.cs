using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.Utilities;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace EbonianMod.Projectiles.VFXProjectiles
{
    public class XImpact : ModProjectile
    {
        public override string Texture => Helper.Placeholder;
        public override void SetDefaults()
        {
            Projectile.width = 300;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 50;
            Projectile.tileCollide = false;
            Projectile.hide = false;
            Projectile.penetrate = -1;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            //behindNPCsAndTiles.Add(index);
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? CanDamage() => Projectile.timeLeft > 45;
        int seed;
        public override void OnSpawn(IEntitySource source)
        {
            seed = Main.rand.Next(int.MaxValue);
            for (int k = 0; k < 50; k++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.GemAmethyst, Main.rand.NextVector2Unit(-MathHelper.Pi, MathHelper.Pi) * Main.rand.NextFloat(1, 30), 0, default, Main.rand.NextFloat(1, 3)).noGravity = true;
                Dust.NewDustPerfect(Projectile.Center, DustID.PurpleTorch, Main.rand.NextVector2Unit(-MathHelper.Pi, MathHelper.Pi) * Main.rand.NextFloat(1, 30), 100, default, Main.rand.NextFloat(1, 2)).noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex1 = Helper.GetExtraTexture("Extras2/scratch_02");
            Texture2D tex2 = Helper.GetExtraTexture("cone4");
            UnifiedRandom rand = new UnifiedRandom(seed);
            float max = 50;
            float alpha = MathHelper.Lerp(0.5f, 0, Projectile.ai[1]) * 2;
            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Reload(EbonianMod.SpriteRotation);
            EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(rand.NextFloat(MathHelper.Pi, MathHelper.TwoPi) * (rand.NextFloatDirection() > 0 ? 1 : -1) + Projectile.ai[1]);
            EbonianMod.SpriteRotation.Parameters["scale"].SetValue(new Vector2(1, rand.NextFloat(0.2f, 0.8f)));
            EbonianMod.SpriteRotation.Parameters["uColor"].SetValue(Color.Indigo.ToVector4() * alpha * 0.5f);
            for (float i = 0; i < max; i++)
            {
                Texture2D tex = Main.rand.NextBool() ? tex1 : tex2;
                float angle = Helper.CircleDividedEqually(i, max * 2) + MathHelper.Pi;
                float scale = rand.NextFloat(0, .6f) * 2;
                Vector2 offset = new Vector2(Main.rand.NextFloat(-0, 2) * Projectile.ai[1] * scale, 0).RotatedBy(angle);
                for (float j = 0; j < 2; j++)
                    Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.Indigo * alpha, angle, new Vector2(0, tex.Height / 2), new Vector2(Projectile.ai[1], alpha) * scale * 0.6f * 4, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(effect: null);
            for (float i = 0; i < max; i++)
            {
                Texture2D tex = Main.rand.NextBool() ? tex1 : tex2;
                float angle = Helper.CircleDividedEqually(i, max * 2) + MathHelper.Pi;
                float scale = rand.NextFloat(0, .8f) * 2;
                Vector2 offset = new Vector2(Main.rand.NextFloat(-0, 2) * Projectile.ai[1] * scale, 0).RotatedBy(angle);
                for (float j = 0; j < 2; j++)
                    Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.Indigo * alpha, angle, new Vector2(0, tex.Height / 2), new Vector2(Projectile.ai[1], alpha) * scale * 0.6f * 4, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            Lighting.AddLight(Projectile.Center, TorchID.UltraBright);
            return false;
        }
        public override void AI()
        {

            Projectile.ai[1] = MathHelper.Lerp(Projectile.ai[1], 1, 0.1f);
            if (Projectile.ai[1] > 1)
                Projectile.Kill();
        }
    }
    public class XImpact2 : ModProjectile
    {
        public override string Texture => Helper.Placeholder;
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.hostile = false;
            Projectile.hide = true;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? CanDamage() => Projectile.timeLeft > 45;
        int seed;
        public override void OnSpawn(IEntitySource source)
        {
            seed = Main.rand.Next(int.MaxValue);
            for (int k = 0; k < 20; k++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.GemAmethyst, Main.rand.NextVector2Unit(-MathHelper.Pi, MathHelper.Pi) * Main.rand.NextFloat(1, 30), 0, default, Main.rand.NextFloat(.5f, 1f)).noGravity = true;
                Dust.NewDustPerfect(Projectile.Center, DustID.PurpleTorch, Main.rand.NextVector2Unit(-MathHelper.Pi, MathHelper.Pi) * Main.rand.NextFloat(1, 30), 100, default, Main.rand.NextFloat(.5f, 1f)).noGravity = true;
            }
        }
        public override void OnKill(int timeLeft)
        {
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex1 = Helper.GetExtraTexture("Extras2/scratch_02");
            Texture2D tex2 = Helper.GetExtraTexture("cone4");
            UnifiedRandom rand = new UnifiedRandom(seed);
            float max = 35;
            float alpha = MathHelper.Lerp(0.5f, 0, Projectile.ai[1]) * 2;
            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Reload(EbonianMod.SpriteRotation);
            EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(rand.NextFloat(MathHelper.Pi, MathHelper.TwoPi) * (rand.NextFloatDirection() > 0 ? 1 : -1) + Projectile.ai[1]);
            EbonianMod.SpriteRotation.Parameters["scale"].SetValue(new Vector2(1, rand.NextFloat(0.2f, 0.8f)));
            EbonianMod.SpriteRotation.Parameters["uColor"].SetValue(Color.Indigo.ToVector4() * alpha * 0.5f);
            float off = MathHelper.SmoothStep(1.5f, 0.5f, Projectile.ai[2] / 17);
            for (float i = 0; i < max; i++)
            {
                Texture2D tex = Main.rand.NextBool() ? tex1 : tex2;
                float angle = Helper.CircleDividedEqually(i, max * 4) + MathHelper.Pi * 1.25f;
                float scale = rand.NextFloat(0.15f, .6f) * off;
                Vector2 offset = new Vector2(Main.rand.NextFloat(-0, 2) * Projectile.ai[1] * scale, 0).RotatedBy(angle);
                for (float j = 0; j < 2; j++)
                    Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.Indigo * alpha * 0.5f, angle, new Vector2(0, tex.Height / 2), new Vector2(Projectile.ai[1], alpha) * scale * 0.6f * 4, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(effect: null);
            for (float i = 0; i < max; i++)
            {
                Texture2D tex = Main.rand.NextBool() ? tex1 : tex2;
                float angle = Helper.CircleDividedEqually(i, max * 4) + MathHelper.Pi * 1.25f;
                float scale = rand.NextFloat(0.3f, .8f) * off;
                Vector2 offset = new Vector2(Main.rand.NextFloat(-0, 2) * Projectile.ai[1] * scale, 0).RotatedBy(angle);
                for (float j = 0; j < 2; j++)
                    Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.Indigo * alpha * 0.5f, angle, new Vector2(0, tex.Height / 2), new Vector2(Projectile.ai[1], alpha) * scale * 0.6f * 4, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Purple);
            Projectile.ai[1] = MathHelper.Lerp(Projectile.ai[1], 1, 0.1f);
            if (Projectile.ai[1].CloseTo(1, 0.05f))
                Projectile.Kill();
            if (Projectile.timeLeft == 29)
                if (++Projectile.ai[2] < 17)
                    Projectile.NewProjectile(null, Helper.TRay.Cast(Projectile.Center + Projectile.velocity * 30 - new Vector2(0, 50), Vector2.UnitY, 100, true), Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 0, Projectile.ai[2]);
        }
    }
}
