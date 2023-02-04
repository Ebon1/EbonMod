using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using EbonianMod.Effects.Prims;
using System;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace EbonianMod.Projectiles
{
    public class TelegraphLine : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Telegraph line");
        }
        public override string Texture => "EbonianMod/Extras/Empty";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 30;
        }
        int MAX_TIME;
        bool RunOnce;
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(Projectile.whoAmI);
        }
        float alpha;
        public override void AI()
        {
            if (Projectile.ai[1] == 1)
            {
                NPC npc = Main.npc[(int)Projectile.ai[0]];
                Projectile.Center = npc.Center;
            }
            if (!RunOnce)
            {
                Projectile.velocity.Normalize();
                MAX_TIME = Projectile.timeLeft;
                RunOnce = true;
            }
            float progress = Utils.GetLerpValue(0, MAX_TIME, Projectile.timeLeft);
            alpha = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 2, 0, 1);
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Utils.DrawLine(Main.spriteBatch, Projectile.Center, Projectile.Center + Projectile.velocity * Main.screenWidth, Color.White * 0f, Color.Red * alpha, 2);
            return false;
        }
        /*private void DrawBeam(SpriteBatch spriteBatch, Texture2D texture, Vector2 startPosition, Vector2 endPosition, Vector2 drawScale, Color beamColor)
        {
            Utils.LaserLineFraming lineFraming = new Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw);
            DelegateMethods.c_1 = beamColor;
            Utils.DrawLaser(spriteBatch, texture, startPosition, endPosition, drawScale, lineFraming);
        }*/
    }
}

