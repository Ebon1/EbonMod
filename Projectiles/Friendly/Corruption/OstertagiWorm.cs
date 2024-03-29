using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Projectiles.Friendly.Corruption
{
    public class OstertagiWorm : ModProjectile
    {
        public override string Texture => "EbonianMod/NPCs/Corruption/Rotling/RotlingHead";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8 * 5;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 14;
            Projectile.aiStyle = 0;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 400;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindNPCs.Add(index);
        Vector2 startPos, startVel;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
                if (targetHitbox.Intersects(new Rectangle((int)Projectile.oldPos[i].X, (int)Projectile.oldPos[i].Y, 10, 8)))
                    return true;
            return false;
        }
        public override void AI()
        {
            if (Projectile.Center.Distance(Main.LocalPlayer.Center) < Main.screenWidth)
                Projectile.timeLeft = 200;
            if (startPos == Vector2.Zero)
            {
                startPos = Projectile.Center;
                startVel = Projectile.velocity;
            }
            Projectile.rotation = Helper.FromAToB(Projectile.oldPos[1], Projectile.position).ToRotation() + MathHelper.PiOver2;
            if (Projectile.ai[0] == 0)
                Projectile.ai[0] = Main.rand.NextFloat(0.1f, 0.3f);
            Projectile.SineMovement(startPos, startVel, 0.2f, 60);

            if (Main.rand.NextBool(10))
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.CorruptGibs, Main.rand.NextVector2Circular(4, 4), 0, default, Main.rand.NextFloat(1, 2)).noGravity = !Main.rand.NextBool(10);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D body = ModContent.Request<Texture2D>(Texture.Replace("Head", "Body")).Value;
            Texture2D tail = ModContent.Request<Texture2D>(Texture.Replace("Head", "Tail")).Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (i != Projectile.oldPos.Length - 1)
                    Main.spriteBatch.Draw(body, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Lighting.GetColor(Projectile.oldPos[i].ToTileCoordinates()), Projectile.oldRot[i], body.Size() / 2, 1f, SpriteEffects.None, 0);
                if (i == Projectile.oldPos.Length - 1)
                    Main.spriteBatch.Draw(tail, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Lighting.GetColor(Projectile.oldPos[i].ToTileCoordinates()), Projectile.oldRot[i], body.Size() / 2, 1f, SpriteEffects.None, 0);
            }
            return true;
        }
    }
}
