using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace EbonianMod.Projectiles.Cecitior
{
    public class CecitiorTeeth : ModProjectile
    {
        //public override string Texture => "EbonianMod/Projectiles/Friendly/Crimson/Fangs";
        public override void SetStaticDefaults()
        {
            //  Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 500;
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Ichor, 200);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.timeLeft > 490 && Projectile.velocity.Length() > 0.05f)
                Projectile.velocity *= 0.9f;
            if (Projectile.timeLeft < 450 && Projectile.velocity.Length() < 25)
                Projectile.velocity *= 1.15f;

        }
    }
}