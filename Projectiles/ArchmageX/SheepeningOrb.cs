using EbonianMod.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Humanizer.In;
using static System.Net.Mime.MediaTypeNames;

namespace EbonianMod.Projectiles.ArchmageX
{
    public class SheepeningOrb : ModProjectile
    {
        int MaxTime = 3000;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 100;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = MaxTime;
            Projectile.extraUpdates = 3;
            Projectile.Size = new(32, 32);
        }
        float alpha;
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<XExplosionTiny>(), 0, 0);
            if (Projectile.timeLeft > 60)
                Projectile.timeLeft = 60;
            return false;
        }
        public override void AI()
        {
            if (Projectile.velocity.Length() < 20)
                Projectile.velocity *= 1.025f;
            if (Projectile.timeLeft > 60)
                alpha = MathHelper.Lerp(alpha, 1, 0.05f);
            else
                alpha = MathHelper.Lerp(alpha, 0, 0.1f);
        }
        struct Orb
        {
            public Vector2 pos, velocity;
            public Vector2[] oldPos;
            public bool[] behind;
            public float dir;
        };
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < orbs.Length; i++)
            {
                orbs[i].velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1f, 2.75f);
                orbs[i].oldPos = new Vector2[15];
                orbs[i].behind = new bool[15];
                orbs[i].dir = Main.rand.NextFloatDirection();
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<Sheepened>(), (Main.expertMode ? Main.masterMode ? 16 : 13 : 10) * 60);
        }
        Orb[] orbs = new Orb[3];
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Texture2D trail = Helper.GetExtraTexture("fireball");

            Main.spriteBatch.Reload(BlendState.Additive);
            OrbLogic(true);

            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            for (int i = (Projectile.velocity.Length() < 2 ? 12 : 0); i < Projectile.oldPos.Length; i++)
            {
                float mult = (1f - fadeMult * i);
                Main.spriteBatch.Draw(trail, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.Indigo * 0.5f * alpha * mult, Projectile.oldRot[i] + MathHelper.PiOver2, trail.Size() / 2, Projectile.scale * 0.5f * mult, SpriteEffects.None, 0);
            }

            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            Main.spriteBatch.Reload(effect: EbonianMod.PullingForce);
            EbonianMod.PullingForce.Parameters["uOpacity"].SetValue(alpha * 2);
            EbonianMod.PullingForce.Parameters["uIntensity"].SetValue(alpha * 2);
            EbonianMod.PullingForce.Parameters["uOffset"].SetValue(0.5f);
            EbonianMod.PullingForce.Parameters["uSpeed"].SetValue(4f);
            EbonianMod.PullingForce.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 4f);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Main.GameUpdateCount * -0.005f, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Main.GameUpdateCount * 0.005f, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(effect: null);

            Main.spriteBatch.Reload(BlendState.Additive);
            OrbLogic(false);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            return false;
        }
        void OrbLogic(bool behind)
        {
            Texture2D orbTex = Helper.GetExtraTexture("fireball");
            if (orbs[0].velocity != Vector2.Zero)
            {
                for (int i = 0; i < orbs.Length; i++)
                {
                    for (int num16 = orbs[i].oldPos.Length - 1; num16 > 0; num16--)
                    {
                        orbs[i].oldPos[num16] = orbs[i].oldPos[num16 - 1];
                    }
                    orbs[i].oldPos[0] = orbs[i].pos;
                    orbs[i].velocity = orbs[i].velocity.RotatedBy(MathHelper.ToRadians(orbs[i].velocity.Length() * 2 * orbs[i].dir));
                    orbs[i].pos += orbs[i].velocity;
                    if ((orbs[i].pos + Projectile.Center + orbs[i].velocity).Distance(Projectile.Center) > 32)
                        orbs[i].velocity = -orbs[i].velocity;

                    var fadeMult = 1f / orbs[i].oldPos.Length;
                    for (int j = 0; j < orbs[i].oldPos.Length; j++)
                    {
                        float mult = (1f - fadeMult * j);
                        if (orbs[i].oldPos[j].Distance(Projectile.Center) > 16)
                        {
                            orbs[i].behind[j] = Main.rand.NextBool();
                        }
                        if ((orbs[i].behind[j] && behind) || (!orbs[i].behind[j] && !behind))
                        {
                            float rot = orbs[i].velocity.ToRotation() + MathHelper.PiOver2;
                            if (j > 1) rot = Helper.FromAToB(orbs[i].oldPos[j], orbs[i].oldPos[j - 1]).ToRotation() + MathHelper.PiOver2;
                            Main.spriteBatch.Draw(orbTex, Projectile.Center + orbs[i].oldPos[j] - Main.screenPosition, null, Color.Lerp(Color.Indigo, Color.Violet, (MathF.Sin(Main.GlobalTimeWrappedHourly) + 1) / 2) * (alpha * mult), rot, orbTex.Size() / 2, Projectile.scale * 0.1f * mult, SpriteEffects.None, 0);
                        }
                    }
                }
            }
        }
    }
}
