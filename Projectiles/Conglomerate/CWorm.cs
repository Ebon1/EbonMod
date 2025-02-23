using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbonianMod.Projectiles.Conglomerate
{
    public class CWorm : ModProjectile
    {
        public override string Texture => "EbonianMod/NPCs/Corruption/DankDigger/DankDiggerHead";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8 * 5;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 32;
            Projectile.aiStyle = 0;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 100;
            Projectile.timeLeft = 400;
            Projectile.extraUpdates = 1;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindNPCs.Add(index);
        Vector2 startPos, startVel;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
                if (targetHitbox.Intersects(new Rectangle((int)Projectile.oldPos[i].X, (int)Projectile.oldPos[i].Y, 20, 20)))
                    return true;
            return false;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Cursed);
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

            Projectile.ai[2] = Lerp(0, 100, 0.1f);

            Projectile.SineMovement(startPos, startVel, 0.2f, Projectile.ai[2]);

            if (Main.rand.NextBool(10))
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.CorruptGibs, Main.rand.NextVector2Circular(4, 4), 0, default, Main.rand.NextFloat(1, 2)).noGravity = !Main.rand.NextBool(10);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D body = Request<Texture2D>(Texture.Replace("Head", "Body")).Value;
            Texture2D tail = Request<Texture2D>(Texture.Replace("Head", "Tail")).Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (i != Projectile.oldPos.Length - 1)
                    Main.spriteBatch.Draw(body, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, new Color(Lighting.GetSubLight(Projectile.oldPos[i] + Projectile.Size / 2)) * 1.25f, Projectile.oldRot[i], body.Size() / 2, 1f, SpriteEffects.None, 0);
                else
                    Main.spriteBatch.Draw(tail, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, new Color(Lighting.GetSubLight(Projectile.oldPos[i] + Projectile.Size / 2)) * 1.25f, Projectile.oldRot[i], tail.Size() / 2, 1f, SpriteEffects.None, 0);
            }
            lightColor = Color.White;
            return true;
        }
    }
}
