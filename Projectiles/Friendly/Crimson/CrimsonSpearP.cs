using System;
using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Audio;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;

namespace EbonianMod.Projectiles.Friendly.Crimson
{
    public class CrimsonSpearP : HeldSword
    {
        public override void SetExtraDefaults()
        {
            Projectile.width = 118;
            Projectile.height = 118;
            swingTime = 30;
            holdOffset = 20;
            useHeld = false;
        }
        public override void OnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hit.Crit = Projectile.ai[2] != 0;
            if (Projectile.localAI[0] == 21)
            {
                target.AddBuff(BuffID.Ichor, 200);
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = swingTime;
        }
        public override void PostDraw(Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D slash = ExtraTextures2.twirl_03;
            Texture2D stab = ExtraTextures2.trace_05;
            Texture2D spin = ExtraTextures2.light_02;
            if (Projectile.localAI[0] != 0 && Projectile.localAI[0] <= 25)
            {
                float mult = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
                float alpha = (float)Math.Sin(mult * Math.PI);
                Vector2 pos = player.Center + Projectile.velocity * 25f;
                Main.spriteBatch.Reload(BlendState.Additive);
                if (Projectile.ai[2] == 0)
                {
                    if (Projectile.ai[1] == 0)
                        Main.spriteBatch.Draw(stab, Projectile.Center + new Vector2(0, 40).RotatedBy(Projectile.rotation + MathHelper.PiOver4) - Main.screenPosition, null, Color.Gold * alpha, Projectile.rotation + MathHelper.PiOver4, stab.Size() / 2, Projectile.scale * (Projectile.localAI[0] != 21 ? 0.75f : 1.2f) * 2, SpriteEffects.None, 0);
                    else
                        Main.spriteBatch.Draw(slash, pos - Main.screenPosition, null, Color.Gold * alpha, Projectile.velocity.ToRotation() - MathHelper.PiOver2, slash.Size() / 2, Projectile.scale * 0.43f * 2, Projectile.ai[1] == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
                }
                else
                {
                    Main.spriteBatch.Draw(spin, Projectile.Center - Main.screenPosition, null, Color.Gold * Projectile.localAI[1], Projectile.rotation * 0.95f, spin.Size() / 2, Projectile.scale * 0.43f * 2, Projectile.ai[1] == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
                }
                Main.spriteBatch.Reload(BlendState.AlphaBlend);
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            if (Projectile.ai[2] != 0)
                return targetHitbox.Center().Distance(Projectile.Center) < 115;
            if (Projectile.localAI[0] == 21)
                return Collision.CheckAABBvLineCollision(targetHitbox.Center(), targetHitbox.Size(), Projectile.Center, Projectile.Center + 200 * Projectile.velocity, 50, ref a);
            return targetHitbox.Center().Distance(Projectile.Center + 59 * Projectile.rotation.ToRotationVector2()) < 60;
        }
        public override void ExtraAI()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.timeLeft == swingTime - 1)
            {
                if (Projectile.ai[2] == 0)
                {
                    if (Projectile.localAI[0] != 21)
                        SoundEngine.PlaySound(Projectile.ai[1] == 0 ? SoundID.Item1 : SoundID.DD2_MonkStaffSwing, Projectile.Center);
                    else
                        SoundEngine.PlaySound(SoundID.Item73, Projectile.Center);
                }
            }
            if (Projectile.localAI[0] != 0)
                swingTime = (int)(Projectile.localAI[0] / player.GetAttackSpeed(DamageClass.Melee));
            if (Projectile.ai[1] == 0)
                holdOffset = 50;
            baseHoldOffset = holdOffset;
            if (Projectile.timeLeft > swingTime)
                Projectile.timeLeft = swingTime;

            if (Projectile.localAI[0] == 21 && Projectile.timeLeft == 18)
            {
                for (int i = 0; i < 30; i++)
                    Dust.NewDustPerfect(Projectile.Center, DustID.IchorTorch, Projectile.velocity.RotatedByRandom(0.2f) * Main.rand.NextFloat(10, 15f));
            }

            if (Projectile.ai[2] != 0)
            {
                Projectile.damage = Projectile.originalDamage * 2;
                Projectile.localNPCHitCooldown = 15;
                Projectile.ai[2]++;
                if (Projectile.ai[2] % 10 == 0 && Projectile.localAI[1] > 0.8f)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        float angle = Helper.CircleDividedEqually(i, 10) + Projectile.rotation;
                        Dust.NewDustPerfect(Projectile.Center, DustID.IchorTorch, Vector2.UnitX.RotatedBy(angle) * Main.rand.NextFloat(1, 5f));
                    }
                    SoundEngine.PlaySound(SoundID.Item169, Projectile.Center);
                }
                Projectile.ai[1] = 1;
                if (Projectile.localAI[1] < 1 && Projectile.ai[2] < 40)
                    Projectile.localAI[1] += 0.25f * Projectile.localAI[1].Safe();
                else if (Projectile.localAI[1] > 0 && Projectile.ai[2] > 80)
                    Projectile.localAI[1] -= 0.25f * Projectile.localAI[1].Safe();
                Projectile.rotation += MathHelper.ToRadians(20 * Projectile.localAI[1]);
                if (Projectile.ai[2] < 100)
                    if (player.active && player.channel && !player.dead && !player.CCed && !player.noItems)
                        Projectile.timeLeft = 2;
                Vector2 position = player.MountedCenter +
                    Projectile.velocity * (10 * Projectile.localAI[1]);
                if (player.gravDir != -1)
                    player.SetCompositeArmFront(true, stretch, Projectile.velocity.ToRotation() - MathHelper.PiOver2);

                Projectile.Center = position;
            }
        }
        public override bool PreKill(int timeLeft)
        {
            float ai = Projectile.ai[1] + 1;
            if (ai > 1)
                ai = -1;
            Player player = Main.player[Projectile.owner];
            if (player.active && player.channel && !player.dead && !player.CCed && !player.noItems)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    Vector2 dir = Vector2.Normalize(Main.MouseWorld - player.Center);
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), player.Center, dir, Projectile.type, player.HeldItem.damage, Projectile.knockBack, player.whoAmI, 0, ai);
                    proj.rotation = Projectile.rotation;
                    proj.Center = Projectile.Center;
                    if (Projectile.localAI[0] == 0)
                        proj.localAI[0] = swingTime - 1;
                    if (Projectile.localAI[0] > 22)
                        proj.localAI[0] = Projectile.localAI[0] - 1;
                    else if (Projectile.localAI[0] == 21)
                    {
                        proj.localAI[0] = 25;
                    }
                    else if (Projectile.localAI[0] != 0 && Projectile.localAI[0] <= 22)
                    {
                        proj.ai[2] = 1;
                        proj.localAI[0] = 22;
                        proj.originalDamage = Projectile.damage;
                        if (ai == 0)
                            proj.ai[1] = 1;
                    }

                    if (Projectile.ai[2] != 0)
                    {
                        proj.ai[2] = 0;
                        proj.ai[1] = 0;
                        proj.localAI[0] = 21;
                    }
                }
            }
            Projectile.active = false;
            return false;
        }
    }
}
