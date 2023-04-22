using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using EbonianMod.Projectiles.Friendly.Corruption;
using Terraria.DataStructures;
using EbonianMod.Projectiles.Exol;
using EbonianMod.Projectiles.VFXProjectiles;
using Terraria.Audio;
using EbonianMod.Projectiles.Friendly.Crimson;
using EbonianMod.Misc;

namespace EbonianMod.Projectiles.Friendly.Crimson
{
    public class LatcherP : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            if (Projectile.ai[0] == 0)
            {
                Projectile.timeLeft = 200;
                Projectile.ai[1] = 1;
            }
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.velocity = Vector2.Zero;
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = target.whoAmI;
                Projectile.timeLeft = 200;
                Projectile.ai[1] = 2;
            }
        }
        Verlet verlet;
        public override void OnSpawn(IEntitySource source)
        {
            verlet = new(Projectile.Center, 20, 10, 1, true, true, 10);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Player player = Main.player[Projectile.owner];
            if (Main.myPlayer == Projectile.owner && Main.mouseRight)
                Projectile.Kill();
            if (Projectile.ai[1] == 1)
            {
                player.velocity = Helper.FromAToB(player.Center, Projectile.Center) * 10;
            }
            else if (Projectile.ai[1] == 2)
            {
                NPC npc = Main.npc[(int)Projectile.ai[0]];
                if (npc.active && npc.life > 0 && player.Center.Distance(npc.Center) > npc.width)
                {
                    Projectile.Center = npc.Center;
                    if (npc.knockBackResist == 0f)
                        player.velocity = Helper.FromAToB(player.Center, Projectile.Center) * 10;
                    else
                        npc.velocity = Helper.FromAToB(npc.Center, player.Center) * 10;
                }
                else
                    Projectile.Kill();
            }
            else
            {
                if (Projectile.timeLeft < 100)
                    Projectile.Center = Vector2.Lerp(Projectile.Center, player.Center, 0.05f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {

            if (verlet != null)
            {
                verlet.Update(Projectile.Center, Main.player[Projectile.owner].Center);
                verlet.Draw(Main.spriteBatch, "Projectiles/Friendly/Crimson/LatcherP_Chain");
            }
            return true;
        }
    }
}
