using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Audio;
using EbonianMod.Common.Systems.Misc;

namespace EbonianMod.Projectiles.Friendly.Crimson
{
    public class SpinaxP : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        int swingTime = 40;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = swingTime;
            Projectile.timeLeft = swingTime;
            Projectile.Size = new Vector2(36, 44);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (verlet != null)
                foreach (VerletPoint pos in verlet.points)
                {
                    if (new Rectangle((int)pos.position.X, (int)pos.position.Y, 8, 8).Intersects(targetHitbox))
                        return true;
                }
            return projHitbox.Intersects(targetHitbox);
        }
        public virtual float Ease(float x)
        {
            return x == 0
  ? 0
  : x == 1
  ? 1
  : x < 0.5 ? MathF.Pow(2, 20 * x - 10) / 2
  : (2 - MathF.Pow(2, -20 * x + 10)) / 2;
        }
        public virtual float ScaleFunction(float progress)
        {
            return 0.7f + (float)Math.Sin(progress * Math.PI) * 0.5f;
        }
        Verlet verlet;
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            float off = 20 * ScaleFunction(swingProgress);
            if (Projectile.ai[1] != 1 && Projectile.ai[1] != 0)
                off = 20;
            if (verlet != null)
            {
                verlet.Update(player.Center + Helper.FromAToB(player.Center, Projectile.Center) * off, Projectile.Center);
                verlet.Draw(Main.spriteBatch, "Projectiles/Friendly/Crimson/SpinaxP_Chain");
            }
            Texture2D tex = ModContent.Request<Texture2D>(Texture + "_Handle").Value;
            Texture2D trail = ModContent.Request<Texture2D>(Texture + "_Trail").Value;
            Main.spriteBatch.Draw(tex, player.Center + Helper.FromAToB(player.Center, Projectile.Center) * off - Main.screenPosition, null, Color.White, Helper.FromAToB(player.Center, Projectile.Center).ToRotation() + MathHelper.PiOver4, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);


            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float mult = (1f - fadeMult * i);
                float mult2 = (1f - fadeMult * (i));
                if (i > 0)
                {
                    mult2 = (1f - fadeMult * (i - 1));
                    for (float j = 0; j < 5; j++)
                    {
                        Vector2 pos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[i - 1], (float)(j / 5));
                        Main.spriteBatch.Draw(trail, pos + Projectile.Size / 2 - Main.screenPosition, null, Color.Maroon * 0.25f * MathHelper.Lerp(mult2, mult, (float)(j / 5)), Projectile.oldRot[i] + MathHelper.PiOver4 + (Projectile.ai[1] == 0 ? 0 : MathHelper.PiOver2 * 3), trail.Size() / 2, MathHelper.Lerp(mult2, mult, (float)(j / 5)), Projectile.ai[1] == 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
                    }
                }
            }

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 orig = texture.Size() / 2;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), lightColor, Projectile.rotation + MathHelper.PiOver4 + (Projectile.ai[1] == 0 ? 0 : MathHelper.PiOver2 * 3), orig, Projectile.scale, Projectile.ai[1] == 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            verlet = new Verlet(player.Center, 8, 10);
        }
        Vector2 lastPos;
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (player.active && player.channel && !player.dead && !player.CCed && !player.noItems)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    Vector2 dir = Vector2.Normalize(Main.MouseWorld - player.Center);
                    float ai = (Projectile.ai[1] + 1);
                    if (ai > 2) ai = -1;
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), player.Center, dir, Projectile.type, Projectile.damage, Projectile.knockBack, player.whoAmI, 0, ai);
                    proj.rotation = Projectile.rotation;
                    proj.Center = Projectile.Center;
                }
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems)
            {
                Projectile.Kill();
                return;
            }
            foreach (VerletPoint pos in verlet.points)
            {
                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && !npc.friendly && !npc.dontTakeDamage)
                        if (new Rectangle((int)pos.position.X, (int)pos.position.Y, 8, 8).Intersects(npc.getRect()) && npc.immune[player.whoAmI] == 0)
                        {
                            npc.SimpleStrikeNPC(Projectile.damage, player.direction, false, 0, DamageClass.Melee);
                            npc.immune[player.whoAmI] = Projectile.timeLeft > swingTime ? swingTime : Projectile.timeLeft;
                        }
                }
            }
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Helper.FromAToB(player.Center, Projectile.Center).ToRotation() - MathHelper.PiOver2);
            if (Projectile.ai[1] == 1 || Projectile.ai[1] == 0)
            {
                if (Projectile.ai[0] == 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing, Projectile.Center);
                    Projectile.ai[0] = 1;
                }
                int direction = Projectile.ai[1] == 0 ? -1 : 1;
                float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
                float defRot = Projectile.velocity.ToRotation();
                float start = defRot - (MathHelper.PiOver2 + MathHelper.PiOver4);
                float end = defRot + (MathHelper.PiOver2 + MathHelper.PiOver4);
                float rotation = direction == 1 ? start + MathHelper.Pi * 3 / 2 * swingProgress : end - MathHelper.Pi * 3 / 2 * swingProgress;
                Vector2 position = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, rotation - MathHelper.PiOver2) +
                    rotation.ToRotationVector2() * 200 * ScaleFunction(swingProgress) + Projectile.velocity * 100;
                if (Projectile.timeLeft > 5 || Projectile.ai[1] == 0)
                    Projectile.Center = Vector2.Lerp(Projectile.Center, position, 0.15f);
                else if (Projectile.timeLeft <= 5 && Projectile.ai[1] == 1)
                    Projectile.Center = Vector2.Lerp(Projectile.Center, player.Center, 0.25f);
                Projectile.rotation = (position - player.Center).ToRotation();
                player.ChangeDir(Main.MouseWorld.X < player.Center.X ? -1 : 1);
            }
            if (Projectile.ai[1] == -1)
            {
                if (Projectile.ai[0] == 0)
                {
                    Projectile.rotation = 0;
                    player.ChangeDir(Main.MouseWorld.X < player.Center.X ? -1 : 1);
                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing, Projectile.Center);
                    Projectile.ai[0] = 1;
                }
                Projectile.rotation += MathHelper.ToRadians(Projectile.ai[2] * 2);
                Projectile.Center = player.Center + new Vector2(Projectile.ai[2] * 20, 0).RotatedBy(Projectile.rotation);

                float progress = Utils.GetLerpValue(0, swingTime, Projectile.timeLeft);
                float scale = MathHelper.Clamp((float)Math.Sin(progress * MathHelper.Pi) * 10, 0, 10);
                Projectile.ai[2] = scale;
            }
            if (Projectile.ai[1] == 2)
            {
                if (Projectile.ai[2] == 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing, Projectile.Center);
                    Projectile.timeLeft = 25;
                    swingTime = 25;
                    player.ChangeDir(Main.MouseWorld.X < player.Center.X ? -1 : 1);
                    lastPos = Main.MouseWorld;
                    Projectile.ai[2] = 1;
                }
                float progress = Utils.GetLerpValue(0, swingTime, Projectile.timeLeft);
                float scale = MathHelper.Clamp((float)Math.Sin(progress * MathHelper.Pi) * 0.75f, 0, 1);
                if (scale > 0.9f)
                    Projectile.ai[0] = 1;
                float scale2 = (MathHelper.Lerp(1, 0, scale) - 0.5f) * 2;
                if (lastPos != Vector2.Zero)
                    Projectile.Center = Vector2.SmoothStep(player.Center, player.Center + Helper.FromAToB(player.Center, lastPos) * 360 - new Vector2(0, scale2 * 200 * player.direction).RotatedBy(Projectile.rotation), scale);
                Projectile.rotation = Helper.FromAToB(player.Center, lastPos).ToRotation();
            }
        }
    }
}
