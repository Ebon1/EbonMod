using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace EbonianMod.Projectiles.Friendly.Crimson
{
    public class Fangs : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;

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

            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;
            }
        }
    }
}