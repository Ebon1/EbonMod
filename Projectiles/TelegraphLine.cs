using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using ExolRebirth.Effects.Prims;
using System;
using Terraria.ModLoader;
using System.Collections.Generic;


namespace ExolRebirth.Projectiles
{
    public class TelegraphLine : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Telegraph line");
        }
        public override string Texture => "ExolRebirth/Extras/Empty";

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
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 3, 0, 1);
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Utils.DrawLine(Main.spriteBatch, Projectile.Center, Projectile.Center + Projectile.velocity * Main.screenWidth, Color.White * 0.5f, Color.White, 2);
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

