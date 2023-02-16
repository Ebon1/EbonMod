using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using EbonianMod.Projectiles;
using EbonianMod.Items;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using EbonianMod.Projectiles.Terrortoma;

namespace EbonianMod.Projectiles.Friendly.Corruption
{
    public class TerrortomaFlail_Clingers : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Clinger");
            Main.projFrames[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.damage = 0;
            Projectile.tileCollide = false;
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
        }
        private int AITimer = 0;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile center = Main.projectile[(int)Projectile.ai[0]];
            if (!center.active || center.type != ModContent.ProjectileType<TerrortomaFlail>())
            {
                Projectile.Kill();
            }
            if (Projectile.frame == 2)
            {
                Vector2 toPlayer = Main.MouseWorld - Projectile.Center;
                Projectile.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                Vector2 pos = center.Center + new Vector2(85, 85).RotatedBy(center.rotation);
                Vector2 target = pos;
                Vector2 moveTo = target - Projectile.Center;
                Projectile.velocity = (moveTo) * 0.05f;
                AITimer++;
                if (AITimer % 100 == 0)
                {
                    float rotation = (float)Math.Atan2(Projectile.Center.Y - Main.MouseWorld.Y, Projectile.Center.X - Main.MouseWorld.X);
                    Projectile projectilee = Main.projectile[Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center.X, Projectile.Center.Y, (float)((Math.Cos(rotation) * 12f) * -1), (float)((Math.Sin(rotation) * 12f) * -1), ModContent.ProjectileType<TFlameThrower3>(), Projectile.damage, 1f, Main.myPlayer)];
                    projectilee.friendly = true;
                    projectilee.hostile = false;
                }
            }
            else if (Projectile.frame == 1)
            {
                Vector2 pos = center.Center + new Vector2(-85, 85).RotatedBy(center.rotation);
                Vector2 target = pos;
                Vector2 moveTo = target - Projectile.Center;
                Projectile.velocity = (moveTo) * 0.05f;
                if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Minions.EbonFlyMinion>()] < 18)
                {
                    AITimer++;
                    if (AITimer >= 180)
                    {
                        Projectile fly = Main.projectile[Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Minions.EbonFlyMinion>(), Projectile.damage, 1f, Main.myPlayer)];
                        AITimer = 0;
                    }
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
                Vector2 pos = Main.MouseWorld;
                Vector2 target = pos;
                Vector2 moveTo = target - Projectile.Center;
                Projectile.velocity = (moveTo) * 0.09f;
            }
        }
        public override bool PreDraw(ref Color drawColor)
        {
            Player player = Main.player[Projectile.owner];
            Projectile center = Main.projectile[(int)Projectile.ai[0]];

            Vector2 neckOrigin = center.Center;
            Vector2 ccenter = Projectile.Center;
            Vector2 distToProj = neckOrigin - Projectile.Center;
            float projRotation = distToProj.ToRotation() - 1.57f;
            float distance = distToProj.Length();
            while (distance > 4 && !float.IsNaN(distance))
            {
                distToProj.Normalize();
                distToProj *= 4;
                ccenter += distToProj;
                distToProj = neckOrigin - ccenter;
                distance = distToProj.Length();

                Main.EntitySpriteDraw(Mod.Assets.Request<Texture2D>("Projectiles/Friendly/Corruption/TerrortomaFlail_TinyChain").Value, ccenter - Main.screenPosition,
                    new Rectangle(0, 0, 6, 4), Lighting.GetColor((int)ccenter.X / 16, (int)ccenter.Y / 16), projRotation,
                    new Vector2(6 * 0.5f, 4 * 0.5f), 1f, SpriteEffects.None, 0);
            }
            return true;

        }
    }
}