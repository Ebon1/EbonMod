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
        public override string Texture => "EbonianMod/Projectiles/Friendly/Crimson/Fangs";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cecitior Fang");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 500;
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(4);
        }
        public override void AI()
        {
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.timeLeft > 490 && Projectile.velocity.Length() > 0.05f)
                Projectile.velocity *= 0.9f;
            if (Projectile.timeLeft < 450 && Projectile.velocity.Length() < 25)
                Projectile.velocity *= 1.15f;
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;
            }
        }
    }
}