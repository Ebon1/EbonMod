using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EbonianMod.Buffs;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace EbonianMod.Projectiles.ExolOld
{
    public class ExolPortal : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exol Portal effect");
            Main.projFrames[Projectile.type] = 8;
        }
        public int KillTimer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 188;
            Projectile.height = 178;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 8)
                {
                    Projectile.Kill();
                }
            }
            {
                Lighting.AddLight(Projectile.position, 0.25f, 0, 0.5f);
            }

        }
    }
}