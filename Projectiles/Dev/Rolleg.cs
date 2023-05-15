using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;
using EbonianMod.Buffs;
using Terraria.Audio;
using System.Runtime.CompilerServices;

namespace EbonianMod.Projectiles.Dev
{
    public class Rolleg : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.Size = new(30, 44);
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 10;
            Projectile.tileCollide = true;
            Projectile.minionSlots = 0.5f;
            Projectile.minion = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.UnitY * -10 * Main.rand.NextFloat(0.65f, 1f);
            Projectile.frameCounter = -5;
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            player.AddBuff(ModContent.BuffType<RollegB>(), 69);
            Projectile.frame = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            if (Projectile.ai[0] == 2)
            {
                sb.Reload(BlendState.Additive);
                Texture2D cone2 = Helper.GetExtraTexture("cone2");
                float progress = Utils.GetLerpValue(0, 50, timer);
                float alpha = MathHelper.Clamp((float)Math.Sin(progress * MathHelper.Pi) * 3, 0, 1);
                sb.Draw(cone2, new Vector2(Projectile.Center.X - Main.screenPosition.X, -200), null, Main.DiscoColor * alpha, MathHelper.ToRadians(90), new Vector2(0, cone2.Height / 2), 1.1f, SpriteEffects.None, 0);
                sb.Draw(cone2, new Vector2(Projectile.Center.X - Main.screenPosition.X, -200), null, Color.White * alpha, MathHelper.ToRadians(90), new Vector2(0, cone2.Height / 2), 1f, SpriteEffects.None, 0);
                sb.Reload(BlendState.AlphaBlend);
            }
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
        {
            if (Projectile.ai[0] == 1)
            {
                target.StrikeNPC(hitinfo);
                Player player = Main.player[Projectile.owner];
                Helper.DustExplosion(Projectile.Center, Projectile.Size * 2f, 0, Color.Gold, false);
                SoundEngine.PlaySound(SoundID.Item62);
                Projectile.Center = player.Center;
                Projectile.ai[0] = 2;
                timer = 50;
                Projectile.velocity = Vector2.Zero;
            }
        }
        int timer;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            EbonianPlayer ebonianPlayer = player.GetModPlayer<EbonianPlayer>();
            if (ebonianPlayer.rolleg)
                Projectile.timeLeft = 2;
            if (Projectile.ai[0] != 2)
                Projectile.velocity.Y += 0.5f;
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X < 0 ? 1 : -1;
            Projectile.velocity.X = Helper.FromAToB(Projectile.Center, player.Center).X * 4 * Main.rand.NextFloat(0.65f, 1f);
            // ai 0 = aistate, ai 1 = target
            Projectile.frameCounter++;
            if (Projectile.frameCounter < 0)
                Projectile.frame = 0;
            if (Projectile.frameCounter == 0)
                Projectile.frame = 2;
            if (Projectile.frameCounter == 60)
                Projectile.frame = 1;
            if (Projectile.frameCounter >= 65)
            {
                Projectile.frame = 2;
                Projectile.frameCounter = 0;
            }
            switch (Projectile.ai[0])
            {
                case 0:
                    Projectile.rotation = MathHelper.Lerp(Projectile.rotation, 0, 0.1f);
                    if (Helper.TRay.CastLength(Projectile.Center, Vector2.UnitY, 1000) < Projectile.height * 2)
                    {
                        timer = 50;
                        foreach (NPC npc in Main.npc)
                        {
                            if (npc.active && !npc.friendly && !npc.dontTakeDamage && npc.Center.Distance(Projectile.Center) < 1000)
                            {
                                Projectile.ai[0] = 1;
                                Projectile.ai[1] = npc.whoAmI;
                                Projectile.frameCounter = -5;
                            }
                        }
                    }
                    break;
                case 1:
                    timer = 50;
                    NPC target = Main.npc[(int)Projectile.ai[1]];
                    if (!target.active)
                        Projectile.ai[0] = 0;
                    Projectile.rotation = MathHelper.Lerp(Projectile.rotation, Helper.FromAToB(Projectile.Center, target.Center).ToRotation() + MathHelper.PiOver2, 0.1f);
                    Projectile.velocity = Helper.FromAToB(Projectile.Center, target.Center) * 5f;
                    break;
                case 2:
                    timer--;
                    Projectile.rotation = MathHelper.Lerp(Projectile.rotation, 0, 0.1f);
                    if (timer > 0)
                        Projectile.velocity.Y -= 0.02f;

                    if (timer <= 0)
                    {
                        Projectile.ai[0] = 0;
                    }
                    break;
            }
        }
    }
}
