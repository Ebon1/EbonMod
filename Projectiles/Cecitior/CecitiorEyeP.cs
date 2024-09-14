using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ID;

namespace EbonianMod.Projectiles.Cecitior
{
    internal class CecitiorEyeP : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 0;
            Main.projFrames[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 200 * 2;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor * 0.5f * (Projectile.timeLeft < 40 ? Projectile.timeLeft * 0.1f : 1);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D a = TextureAssets.Projectile[Type].Value;
            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(a, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, new Rectangle(0, Projectile.frame * 34, 32, 34), lightColor * (Projectile.timeLeft < 40 ? Projectile.timeLeft * 0.1f : 1) * 0.4f * (1f - fadeMult * i), Projectile.rotation, Projectile.Size / 2, Projectile.scale * (1f - fadeMult * i), SpriteEffects.None, 0);
            }
            return true;
        }
        public override void AI()
        {
            foreach (Projectile npc in Main.projectile)
            {
                if (npc.active && npc.type == Type && npc.whoAmI != Projectile.whoAmI)
                {
                    if (npc.Center.Distance(Projectile.Center) < npc.width * npc.scale)
                    {
                        Projectile.velocity += Helper.FromAToB(Projectile.Center, npc.Center, true, true) * 0.5f;
                    }
                    if (npc.Center == Projectile.Center)
                    {
                        Projectile.velocity = Main.rand.NextVector2Unit() * 5;
                    }
                }
            }

            if (++Projectile.frameCounter % 5 == 0 && Projectile.frame < 2)
                Projectile.frame++;
            else
                Projectile.frame = 0;
            if (Projectile.timeLeft > 155 * 2)
                Projectile.ai[1] = Projectile.velocity.ToRotation() + MathHelper.Pi;
            Projectile.rotation = MathHelper.Lerp(Projectile.rotation, Projectile.ai[1], 0.45f);
            Vector2 move = Vector2.Zero;
            float distance = 5050f;
            bool target = false;
            for (int k = 0; k < 200; k++)
            {
                if (Main.player[k].active)
                {
                    Vector2 newMove = Main.player[k].Center - Projectile.Center;
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < distance)
                    {
                        move = newMove;
                        distance = distanceTo;
                        target = true;
                    }
                }
            }
            if (++Projectile.ai[0] % 5 * 3 == 0 && target && Projectile.timeLeft > 45 * 2 && Projectile.timeLeft < 155 * 2)
            {
                Projectile.ai[1] = Projectile.velocity.ToRotation() + MathHelper.Pi;
                AdjustMagnitude(ref move);
                Projectile.velocity = (Projectile.velocity.Length() * Projectile.velocity + move) / Projectile.velocity.Length();
                AdjustMagnitude(ref Projectile.velocity);
            }
            if (Projectile.timeLeft < 100 * 2)
            {
                Projectile.velocity *= 0.99f;
            }
            if (Projectile.timeLeft < 60 * 2)
            {
                Projectile.velocity *= 0.95f;
            }
        }

        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > Projectile.velocity.Length())
            {
                vector *= Projectile.velocity.Length() / magnitude;
            }
        }
    }
}
