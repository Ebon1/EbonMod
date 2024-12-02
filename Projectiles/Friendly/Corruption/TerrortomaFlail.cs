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
using EbonianMod.Common.Systems.Misc;
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
            Projectile.extraUpdates = 1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
        }
        public float AITimer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public float ChannelCheck
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public float AITimer2
        {
            get => Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        Verlet verlet;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (ChannelCheck < 30)
                ChannelCheck++;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (ChannelCheck <= 30)
                modifiers.FinalDamage.Base -= ChannelCheck / 3;
        }
        public override void AI()
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptGibs, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, 100, default, 1.5f);
            dust.noGravity = true;

            Player player = Main.player[Projectile.owner];

            if (verlet != null)
                verlet.Update(player.Center, Projectile.Center);
            Projectile.timeLeft = 2;
            if (player.dead)
            {
                Projectile.Kill();
                return;
            }
            if (!player.channel)
                ChannelCheck = 35;
            AITimer++;
            if (AITimer == 1)
            {
                verlet = new Verlet(Projectile.Center, 8, 15, stiffness: 30);
                Projectile eater = Main.projectile[Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TerrortomaFlail_Clingers>(), Projectile.damage, 0, player.whoAmI, Projectile.whoAmI)];
                Projectile smasher = Main.projectile[Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TerrortomaFlail_Clingers>(), Projectile.damage, 0, player.whoAmI, Projectile.whoAmI)];
                Projectile summoner = Main.projectile[Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TerrortomaFlail_Clingers>(), Projectile.damage, 0, player.whoAmI, Projectile.whoAmI)];
                eater.frame = 2;
                smasher.frame = 0;
                summoner.frame = 1;
            }

            if (ChannelCheck < 30)
            {
                Vector2 moveTo = (player.Center + new Vector2(150, 0).RotatedBy(MathHelper.ToRadians(AITimer * 7))) - Projectile.Center;
                Projectile.velocity = (moveTo) * 0.15f;
            }
            else
            {
                AITimer2++;
                if (AITimer < 10)
                    Projectile.velocity *= 0.96f;
                if (AITimer2 == 10)
                    Projectile.velocity = Helper.FromAToB(Projectile.Center, Main.MouseWorld) * 20;
                if (AITimer2 > 30 && AITimer2 < 40)
                    Projectile.velocity *= 0.76f;
                if (AITimer2 > 40)
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(Projectile.Center, player.Center) * 40f, 0.15f);
                if (AITimer2 > 40 && Projectile.Center.Distance(player.Center) < 50)
                    Projectile.Kill();
            }

            player.itemAnimation = 10;
            player.itemTime = 10;

            player.ChangeDir(Projectile.Center.X > player.Center.X ? 1 : -1);
            Projectile.direction = Projectile.Center.X > player.Center.X ? 1 : -1;
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
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

            if (verlet != null)
            {
                VerletDrawData data = new VerletDrawData()
                {
                    texPath = "EbonianMod/Projectiles/Friendly/Corruption/TerrortomaFlail_Chain"
                };
                verlet.Draw(Main.spriteBatch, "EbonianMod/Projectiles/Friendly/Corruption/TerrortomaFlail_Chain");
                verlet.Update(player.Center, Projectile.Center);
            }
            return true;
        }
    }
}