using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace EbonianMod.Projectiles.Friendly.Corruption
{
    public class TerrortomaFlail : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
        }
        private int aaaaaaaaaa = 0;
        public override void AI()
        {
            var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 93, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, 100, default, 1.5f);
            dust.noGravity = true;
            dust.velocity /= 2f;

            var player = Main.player[Projectile.owner];

            if (player.dead)
            {
                Projectile.Kill();
                return;
            }

            player.itemAnimation = 10;
            player.itemTime = 10;

            int newDirection = Projectile.Center.X > player.Center.X ? 1 : -1;
            player.ChangeDir(newDirection);
            Projectile.direction = newDirection;

            var vectorToPlayer = player.MountedCenter - Projectile.Center;
            float currentChainLength = vectorToPlayer.Length();

            if (Projectile.ai[0] == 0)
            {
                float maxChainLength = 350f;
                Projectile.tileCollide = true;

                if (currentChainLength > maxChainLength)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.netUpdate = true;
                }
                else if (!player.channel)
                {
                    if (Projectile.velocity.Y < 0)
                        Projectile.velocity.Y *= 0.9f;

                    Projectile.velocity.Y += 1f;
                    Projectile.velocity.X *= 0.9f;
                }
            }
            else if (Projectile.ai[0] == 1f)
            {
                float elasticFactorA = 14f / player.GetAttackSpeed(DamageClass.Melee);
                float elasticFactorB = 0.9f / player.GetAttackSpeed(DamageClass.Melee);
                float maxStretchLength = 560f;

                if (Projectile.ai[1] == 1f)
                    Projectile.tileCollide = false;

                if (!player.channel || currentChainLength > maxStretchLength || !Projectile.tileCollide)
                {
                    Projectile.ai[1] = 1f;

                    if (Projectile.tileCollide)
                        Projectile.netUpdate = true;

                    Projectile.tileCollide = false;

                    if (currentChainLength < 20f)
                        Projectile.Kill();
                }

                if (!Projectile.tileCollide)
                    elasticFactorB *= 2f;

                int restingChainLength = 140;

                if (currentChainLength > restingChainLength || !Projectile.tileCollide)
                {
                    var elasticAcceleration = vectorToPlayer * elasticFactorA / currentChainLength - Projectile.velocity;
                    elasticAcceleration *= elasticFactorB / elasticAcceleration.Length();
                    Projectile.velocity *= 0.98f;
                    Projectile.velocity += elasticAcceleration;
                }
                else
                {
                    if (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) < 6f)
                    {
                        Projectile.velocity.X *= 0.96f;
                        Projectile.velocity.Y += 0.2f;
                    }
                    if (player.velocity.X == 0)
                        Projectile.velocity.X *= 0.96f;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2; ;

            if (aaaaaaaaaa == 0)
            {
                Projectile eater = Main.projectile[Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TerrortomaFlail_Clingers>(), Projectile.damage, 0, player.whoAmI, Projectile.whoAmI)];
                Projectile smasher = Main.projectile[Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TerrortomaFlail_Clingers>(), Projectile.damage, 0, player.whoAmI, Projectile.whoAmI)];
                Projectile summoner = Main.projectile[Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TerrortomaFlail_Clingers>(), Projectile.damage, 0, player.whoAmI, Projectile.whoAmI)];
                eater.frame = 2;
                smasher.frame = 0;
                summoner.frame = 1;
                aaaaaaaaaa = 1;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bool shouldMakeSound = false;

            if (oldVelocity.X != Projectile.velocity.X)
            {
                if (Math.Abs(oldVelocity.X) > 4f)
                {
                    shouldMakeSound = true;
                }

                Projectile.position.X += Projectile.velocity.X;
                Projectile.velocity.X = -oldVelocity.X * 0.2f;
            }

            if (oldVelocity.Y != Projectile.velocity.Y)
            {
                if (Math.Abs(oldVelocity.Y) > 4f)
                {
                    shouldMakeSound = true;
                }

                Projectile.position.Y += Projectile.velocity.Y;
                Projectile.velocity.Y = -oldVelocity.Y * 0.2f;
            }

            Projectile.ai[0] = 1f;

            if (shouldMakeSound)
            {
                Projectile.netUpdate = true;
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Player player = Main.player[Projectile.owner];

            Vector2 neckOrigin = player.Center;
            Vector2 ccenter = Projectile.Center;
            Vector2 distToProj = neckOrigin - Projectile.Center;
            float projRotation = distToProj.ToRotation() - 1.57f;
            float distance = distToProj.Length();
            while (distance > 8 && !float.IsNaN(distance))
            {
                distToProj.Normalize();
                distToProj *= 8;
                ccenter += distToProj;
                distToProj = neckOrigin - ccenter;
                distance = distToProj.Length();

                //Draw chain
                Main.EntitySpriteDraw(ModContent.Request<Texture2D>("EbonianMod/Projectiles/Friendly/Corruption/TerrortomaFlail_Chain").Value, ccenter - Main.screenPosition,
                    new Rectangle(0, 0, 16, 8), Lighting.GetColor((int)ccenter.X / 16, (int)ccenter.Y / 16), projRotation,
                    new Vector2(16 * 0.5f, 8 * 0.5f), 1f, SpriteEffects.None, 0);
            }
            return true;
        }
    }
}