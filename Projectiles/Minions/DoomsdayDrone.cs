using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria.Audio;

namespace EbonianMod.Projectiles.Minions
{
    public class DoomsdayDrone : MinionAI2
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 10;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;

            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 58;
            Projectile.height = 82;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            inertia = 20f;
            shoot = 438;
            shootSpeed = 12f;
            shootCool = 30;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }
        public override void ExtraTargetAI(Vector2 pos)
        {
            if (++Projectile.localAI[0] % 50 == 0)
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Helper.FromAToB(Projectile.Center, pos), ModContent.ProjectileType<DroneBeam>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
        }
        public override void CheckActive()
        {
            Player player = Main.player[Projectile.owner];
            EbonianPlayer modPlayer = player.GetModPlayer<EbonianPlayer>();
            if (player.dead)
            {
                modPlayer.doomMinion = false;
            }
            if (modPlayer.doomMinion)
            {
                Projectile.timeLeft = 2;
            }
        }
        public override void SelectFrame()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 10;
            }
        }
    }
    public class DroneBeam : ModProjectile
    {
        int MAX_TIME = 60;
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = Main.screenWidth;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)MAX_TIME;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override string Texture => "EbonianMod/Extras/Empty";
        public override void SetStaticDefaults()
        {
        }
        int damage;
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath44);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.height, Projectile.width, ref a);
        }
        bool RunOnce;

        public override void AI()
        {
            if (RunOnce)
            {
                Projectile.velocity.Normalize();
                damage = Projectile.damage;
                MAX_TIME = Projectile.timeLeft;
                RunOnce = false;
            }
            if (Projectile.localAI[1] != 0)
            {
                Projectile.damage = 0;
                Projectile.timeLeft = MAX_TIME;
                Projectile.localAI[1]--;
            }
            else
            {
                Projectile.damage = damage;
            }

            Vector2 end = Projectile.Center + Projectile.velocity * /*Helper.TRay.CastLength(Projectile.Center, Projectile.velocity, */Main.screenWidth/*)*/;

            //Projectile.velocity = -Vector2.UnitY.RotatedBy(MathHelper.ToRadians(Projectile.ai[1]));

            float progress = Utils.GetLerpValue(0, MAX_TIME, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI), 0, 1);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.rotation += 0.3f;

            Helper.Reload(Main.spriteBatch, BlendState.Additive);
            Helper.Reload(Main.spriteBatch, SpriteSortMode.Immediate);

            float mult = (0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly/* * 2*/) * 0.1f);
            float scale = Projectile.scale * 2 * mult;
            Texture2D texture = ModContent.Request<Texture2D>("EbonianMod/Extras/Line").Value;
            Texture2D bolt = Helper.GetExtraTexture("laser4");
            Vector2 start = Projectile.Center;
            Vector2 end = Projectile.Center + Projectile.velocity * /*Helper.TRay.CastLength(Projectile.Center, Projectile.velocity,*/ Main.screenWidth;//);
            float num = Vector2.Distance(start, end);
            Vector2 vector = (end - start) / num;
            Vector2 vector2 = start;
            float rotation = vector.ToRotation();
            for (int i = 0; i < num; i++)
            {
                Main.spriteBatch.Draw(bolt, vector2 - Main.screenPosition, null, Color.White, rotation, bolt.Size() / 2, new Vector2(1, Projectile.scale), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(bolt, vector2 - Main.screenPosition, null, Color.Maroon, rotation, bolt.Size() / 2, new Vector2(1, Projectile.scale), SpriteEffects.None, 0f);
                vector2 = start + i * vector;
            }
            texture = ModContent.Request<Texture2D>("EbonianMod/Extras/Spotlight").Value;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.Maroon, 0, new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, 0, new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0f);

            texture = ModContent.Request<Texture2D>("EbonianMod/Extras/Spotlight").Value;
            for (int i = 0; i < 5; i++)
                Main.spriteBatch.Draw(texture, end - Main.screenPosition, null, Color.Maroon, Projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0f);



            Helper.Reload(Main.spriteBatch, BlendState.AlphaBlend);

            return false;
        }
    }
}