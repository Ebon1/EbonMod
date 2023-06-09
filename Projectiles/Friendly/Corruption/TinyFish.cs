using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;
using EbonianMod.Items.Weapons.Melee;
using Terraria.GameContent;
using Terraria.Audio;


namespace EbonianMod.Projectiles.Friendly.Corruption
{
    public class TinyFish : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 500;
            Projectile.Size = new(30, 10);
            Projectile.penetrate = -1;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity = -Projectile.oldVelocity;
            Projectile.aiStyle = 2;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.timeLeft < 470)
            {
                Projectile.aiStyle = 2;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = new Vector2(Main.rand.NextFloat(-1.5f, 1.5f), -Projectile.oldVelocity.Y * 0.8f);
            Projectile.aiStyle = 2;
            return false;
        }

    }
}
