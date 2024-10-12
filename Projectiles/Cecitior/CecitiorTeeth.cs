using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent;
using System.Collections.Generic;

namespace EbonianMod.Projectiles.Cecitior
{
    public class CecitiorTeeth : ModProjectile
    {
        //public override string Texture => "EbonianMod/Projectiles/Friendly/Crimson/Fangs";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        /*public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.tileCollide)
            {
                Projectile.Center += Projectile.velocity;
                Projectile.velocity = Vector2.Zero;
                Projectile.damage = 0;
                Projectile.timeLeft = 20;
            }
            return false;
        }*/
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 18;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
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
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override bool PreDraw(ref Color lightColor)
        {
            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float mult = (1f - fadeMult * i);
                if (i > 0)
                    for (float j = 0; j < 5; j++)
                    {
                        Vector2 pos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[i - 1], (float)(j / 5));
                        Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, pos + TextureAssets.Projectile[Type].Value.Size() / 2 - Main.screenPosition, null, Color.White * mult, Projectile.rotation, TextureAssets.Projectile[Type].Value.Size() / 2, mult, SpriteEffects.None, 0);
                    }
            }
            return true;
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
    public class CecitiorTeethFriendly : ModProjectile
    {
        public override string Texture => "EbonianMod/Projectiles/Cecitior/CecitiorTeeth";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 18;
            Projectile.aiStyle = 0;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 500;
            Projectile.hide = true;
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.tileCollide)
            {
                Projectile.Center += Projectile.velocity;
                Projectile.velocity = Vector2.Zero;
                Projectile.damage = 0;
                Projectile.timeLeft = 20;
            }
            return false;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Ichor, 30);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft >= 499) return false;
            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float mult = (1f - fadeMult * i);
                if (i > 0)
                    for (float j = 0; j < 5; j++)
                    {
                        Vector2 pos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[i - 1], (float)(j / 5));
                        Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, pos + TextureAssets.Projectile[Type].Value.Size() / 2 - Main.screenPosition, null, Color.White * mult, Projectile.rotation, TextureAssets.Projectile[Type].Value.Size() / 2, mult, SpriteEffects.None, 0);
                    }
            }
            return true;
        }
        Vector2 baseVel;
        public override void AI()
        {
            if (Projectile.timeLeft == 499)
                baseVel = Projectile.velocity;
            Projectile.rotation = baseVel.ToRotation() + MathHelper.PiOver2;
            if (Projectile.velocity.Length() < 25)
                Projectile.velocity *= 1.15f;

        }
    }
}