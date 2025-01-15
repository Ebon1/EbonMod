using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace EbonianMod.Projectiles.Friendly.Corruption
{
    public class VilethornF1 : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_7";
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.aiStyle = 4;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * ((255 - Projectile.alpha) / 255f);
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
            if (Projectile.ai[0] == 0)
            {
                Projectile.alpha -= 50;
                if (Projectile.alpha > 0)
                {
                    return;
                }
                Projectile.alpha = 0;
                Projectile.ai[0] = 1f;
                if (Projectile.ai[1] == 0)
                {
                    Projectile.ai[1] += 1f;
                    Projectile.position += Projectile.velocity * 1f;
                }
                int num62 = ProjectileType<VilethornF1>();
                if (Projectile.ai[1] >= 55f)
                {
                    num62 = ProjectileType<VilethornF2>();
                }
                //float rot = MathHelper.ToRadians(Projectile.ai[1] * (Projectile.ai[2] == 0 ? 1 : Projectile.ai[2]));
                Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.UnitY);
                bool found = false;
                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && !npc.dontTakeDamage && !npc.friendly && npc.Center.Distance(Projectile.Center) < 1500)
                    {
                        found = true;
                        vel = Vector2.Lerp(vel, Helper.FromAToB(Projectile.Center, npc.Center), Projectile.ai[1] / 55).SafeNormalize(Vector2.UnitY);
                        break;
                    }
                }
                if (!found)
                {
                    vel = vel.RotatedBy(MathHelper.ToRadians(Projectile.ai[1] * (Projectile.ai[2] == 0 ? 1 : Projectile.ai[2])));
                }
                int num63 = Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.position + Projectile.velocity + Projectile.Size / 2, Projectile.velocity.Length() * vel, num62, Projectile.damage, 1, Projectile.owner);
                Main.projectile[num63].damage = Projectile.damage;
                Main.projectile[num63].ai[1] = Projectile.ai[1] + 1f;
                Main.projectile[num63].ai[2] = Projectile.ai[2];
                NetMessage.SendData(27, -1, -1, null, num63);
            }
            if (Projectile.alpha < 170 && Projectile.alpha + 5 >= 170)
            {
                for (int num73 = 0; num73 < 3; num73++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 18, Projectile.velocity.X * 0.025f, Projectile.velocity.Y * 0.025f, 170, default(Color), 1.2f);
                }
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 14, 0, 0, 170, default(Color), 1.1f);
            }
            Projectile.alpha += 2;
            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }
        }
    }
    public class VilethornF2 : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_8";
        public override void SetStaticDefaults()
        {
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * ((255 - Projectile.alpha) / 255f);
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.aiStyle = 4;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
            if (Projectile.ai[0] == 0)
            {
                Projectile.alpha -= 50;
                if (Projectile.alpha > 0)
                {
                    return;
                }
                Projectile.alpha = 0;
                Projectile.ai[0] = 1f;
                if (Projectile.ai[1] == 0)
                {
                    Projectile.ai[1] += 1f;
                    Projectile.position += Projectile.velocity * 1f;
                }
            }
            if (Projectile.alpha < 170 && Projectile.alpha + 5 >= 170)
            {
                for (int num73 = 0; num73 < 3; num73++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 18, Projectile.velocity.X * 0.025f, Projectile.velocity.Y * 0.025f, 170, default(Color), 1.2f);
                }
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 14, 0, 0, 170, default(Color), 1.1f);
            }
            Projectile.alpha += 5;
            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }
        }
    }
}