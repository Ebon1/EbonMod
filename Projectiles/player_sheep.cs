using EbonianMod.Common.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Projectiles
{
    public class player_sheep : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
            //EbonianMod.projectileFinalDrawList.Add(Type);
        }
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 1000;
            Projectile.extraUpdates = 3;
            Projectile.Size = new(38, 28);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
            return true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(EbonianSounds.sheep);
            for (int i = 0; i < 30; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            bool sheep = player.GetModPlayer<EbonianPlayer>().sheep;
            if (!sheep) Projectile.Kill();
            else Projectile.timeLeft = Projectile.extraUpdates * 2;
            Projectile.Center = player.Bottom + new Vector2(0, -Projectile.height / 2);
            Projectile.direction = Projectile.spriteDirection = player.direction;
            if (Helper.TRay.CastLength(Projectile.Bottom, Vector2.UnitY, 50, true) > Projectile.height / 2 - 4 || player.velocity.Y == 0)
            {
                Projectile.frame = 4;
            }
            else
            {
                if (player.velocity.X < -1 || player.velocity.X > 1)
                {
                    if (++Projectile.frameCounter % 15 == 0)
                    {
                        Projectile.frame++;
                        if (Projectile.frame > 3)
                            Projectile.frame = 0;
                    }
                }
                else
                    Projectile.frame = 0;
            }
        }
    }
}
