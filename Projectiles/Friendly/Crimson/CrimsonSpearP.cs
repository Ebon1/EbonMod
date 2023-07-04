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
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = swingTime;
            SoundEngine.PlaySound(SoundID.Item1);
        }
        public override void PostDraw(Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D slash = Helper.GetExtraTexture("Extras2/twirl_03");
            Texture2D spear = Helper.GetExtraTexture("Extras2/trace_05");
            if (Projectile.localAI[0] != 0 && Projectile.localAI[0] <= 25)
            {
                float mult = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
                float alpha = (float)Math.Sin(mult * Math.PI);
                Vector2 pos = player.Center + Projectile.velocity * 25f;
                Main.spriteBatch.Reload(BlendState.Additive);
                if (Projectile.ai[1] == 0)
                    Main.spriteBatch.Draw(spear, Projectile.Center + new Vector2(0, 40).RotatedBy(Projectile.rotation + MathHelper.PiOver4) - Main.screenPosition, null, Color.Gold * alpha, Projectile.rotation + MathHelper.PiOver4, spear.Size() / 2, Projectile.scale * 0.75f, SpriteEffects.None, 0);
                else
                    Main.spriteBatch.Draw(slash, pos - Main.screenPosition, null, Color.Gold * alpha, Projectile.velocity.ToRotation() - MathHelper.PiOver2, slash.Size() / 2, Projectile.scale * 0.43f, Projectile.ai[1] == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
                Main.spriteBatch.Reload(BlendState.AlphaBlend);
            }
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox = new System.Drawing.RectangleF(Projectile.Center.X, Projectile.Center.Y, 59, 59).ToRectangle();
        }
        public override void ExtraAI()
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.localAI[0] != 0)
                swingTime = (int)Projectile.localAI[0];
            if (Projectile.ai[1] == 0)
                holdOffset = 50;
            baseHoldOffset = holdOffset;
            if (Projectile.timeLeft > swingTime)
                Projectile.timeLeft = swingTime;
        }
        public override bool PreKill(int timeLeft)
        {
            float ai = Projectile.ai[1] + 1;
            if (ai > 1)
                ai = -1;
            Player player = Main.player[Projectile.owner];
            if (player.active && player.channel && !player.dead && !player.CCed && !player.noItems && Projectile.ai[2] == 0)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    Vector2 dir = Vector2.Normalize(Main.MouseWorld - player.Center);
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), player.Center, dir, Projectile.type, Projectile.damage, Projectile.knockBack, player.whoAmI, 0, ai);
                    proj.rotation = Projectile.rotation;
                    proj.Center = Projectile.Center;
                    if (Projectile.localAI[0] == 0)
                        proj.localAI[0] = swingTime - 1;
                    if (Projectile.localAI[0] > 22)
                        proj.localAI[0] = Projectile.localAI[0] - 1;
                    else if (Projectile.localAI[0] != 0 && Projectile.localAI[0] <= 22)
                        proj.localAI[0] = 22;
                }
            }
            Projectile.active = false;
            return false;
        }
    }
}
