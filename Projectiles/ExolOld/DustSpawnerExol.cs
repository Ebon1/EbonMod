using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace ExolRebirth.Projectiles.ExolOld
{
    public class DustSpawnerExol : ModProjectile
    {
        public override string Texture => "ExolRebirth/Extras/Empty";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spiteful Flame");
        }

        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile.velocity *= 1.005f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            for (int dustNumber = 0; dustNumber < 3; dustNumber++)
            {
                Dust dust = Main.dust[Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, ModContent.DustType<Dusts.ExolIntro2>(), 0, 0, 0, default(Color), 1f)];
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(4.1887903213500977) * new Vector2(Projectile.width / 2, Projectile.height / 2) * 0.8f * (0.8f + Main.rand.NextFloat() * 0.2f);
                dust.velocity.X = 0;
                dust.velocity.Y = 0;
                dust.noGravity = true;
                dust.scale = 1.25f;
            }
        }
    }
}