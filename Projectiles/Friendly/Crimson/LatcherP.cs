using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using EbonianMod.Projectiles.Friendly.Corruption;
using Terraria.DataStructures;

using EbonianMod.Projectiles.VFXProjectiles;
using Terraria.Audio;
using EbonianMod.Projectiles.Friendly.Crimson;
using System.IO;
using System.Collections.Generic;
using EbonianMod.NPCs.Corruption;
using EbonianMod.NPCs.Cecitior;
using EbonianMod.Projectiles.Cecitior;

namespace EbonianMod.Projectiles.Friendly.Crimson
{
    public class LatcherP : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 3000;
        }
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;
            Projectile.hide = true;
        }

        float Rotation=0;

        public override void OnSpawn(IEntitySource source)
        {
            Rotation = Helper.FromAToB(Main.player[Projectile.owner].Center, Projectile.Center).ToRotation();
        }
        
        bool IsAttached;
        Vector2 PositionOffset;
        NPC Target;
        float Speed=0.25f;

        public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
        {
            if(IsAttached == false)
            {
                PositionOffset = Projectile.Center-target.Center;
                Projectile.velocity = Vector2.Zero;
                Target = target;
                IsAttached = true;
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }
        
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Rotation = Helper.LerpAngle(Rotation, Helper.FromAToB(player.Center, Main.MouseWorld).ToRotation(), 0.02f);
            Projectile.ai[0]++;
            if(IsAttached == true)
            {
                Speed *= 1.08f;
                Speed = Clamp(Speed, 0, 23);
                Projectile.timeLeft = 2;
                player.velocity += Helper.FromAToB(player.Center, Projectile.Center, true) * Speed;
                Projectile.Center = Target.Center + PositionOffset;
                if(Vector2.Distance(player.Center, Target.Center) < 100)
                {
                    for(int i=0; i < (int)Speed/2; i++)
                    {
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Target.Center, Main.rand.NextFloat(0, -Pi).ToRotationVector2() * Main.rand.NextFloat(3, 6), ProjectileType<Gibs>(), 15, 0);
                    }
                    SoundEngine.PlaySound(EbonianSounds.chomp1, Projectile.Center);
                    player.velocity *= 0.4f;
                    Target.SimpleStrikeNPC((int)player.velocity.Length() * Main.rand.Next(7, 8), 0, false);
                    Projectile.Kill();
                }
                if(Target == null)
                {
                    Projectile.Kill();
                }
            }
            else if(Projectile.ai[0] > 10)
            {
                Projectile.ai[1]++;
                Projectile.velocity += Helper.FromAToB(Projectile.Center, player.Center) * (float)Projectile.ai[1]/2;
                if(Vector2.Distance(player.Center, Projectile.Center) < 100)
                {
                    Projectile.Kill();                   
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 neckOrigin = new Vector2(player.Center.X, player.Center.Y) + Rotation.ToRotationVector2() * 40;
            Vector2 center = Projectile.Center;
            Vector2 distToProj = neckOrigin - Projectile.Center;
            float projRotation = distToProj.ToRotation() - 1.57f;
            float distance = distToProj.Length();
            while (distance > 20 && !float.IsNaN(distance))
            {
                distToProj.Normalize();
                distToProj *= 20;
                center += distToProj;
                distToProj = neckOrigin - center;
                distance = distToProj.Length();

                //Draw chain
                Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Projectiles/Friendly/Crimson/LatcherP_Chain").Value, center - Main.screenPosition,
                    null, Lighting.GetColor((int)center.X / 16, (int)center.Y / 16), projRotation,
                    Mod.Assets.Request<Texture2D>("Projectiles/Friendly/Crimson/LatcherP_Chain").Value.Size() / 2, 1f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Projectiles/Friendly/Crimson/LatcherP_Chain").Value, Projectile.Center - Main.screenPosition,
                null, lightColor, projRotation,
                Mod.Assets.Request<Texture2D>("Projectiles/Friendly/Crimson/LatcherP_Chain").Value.Size() / 2, 1f, SpriteEffects.None, 0);

            Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Projectiles/Friendly/Crimson/LatcherP").Value, Projectile.Center + ((Projectile.height - 5) * Projectile.rotation.ToRotationVector2()) - Main.screenPosition,
                null, lightColor, Projectile.rotation,
                Mod.Assets.Request<Texture2D>("Projectiles/Friendly/Crimson/LatcherP").Value.Size() / 2, 1f, SpriteEffects.None, 0);
            /*if (verlet != null)
            {
                verlet.Update(Projectile.Center, Main.player[Projectile.owner].Center);
                verlet.Draw(Main.spriteBatch, "Projectiles/Friendly/Crimson/LatcherP_Chain");
            }*/
            return false;
        }
    }
    public class LatcherPCecitior : ModProjectile
    {
        public override string Texture => "EbonianMod/Projectiles/Friendly/Crimson/LatcherP";
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.hide = true;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 3000;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            if (Projectile.localAI[0] == 0)
            {
                Projectile.timeLeft = 200;
                Projectile.ai[1] = 1;
            }
            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.velocity = Vector2.Zero;
            if (Projectile.localAI[0] == 0 && Projectile.ai[1] == 0)
            {
                Projectile.localAI[0] = target.whoAmI;
                Projectile.timeLeft = 100;
                Projectile.ai[1] = 2;
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }
        public override void AI()
        {
            if (Helper.TRay.CastLength(Projectile.Center, Vector2.UnitY, 30, true) < 20)
            {
                Projectile.velocity = Vector2.Zero;
                if (Projectile.localAI[0] == 0)
                {
                    Projectile.timeLeft = 200;
                    Projectile.ai[1] = 1;
                }
            }
            if (Projectile.ai[1] == 0)
                Projectile.rotation = Projectile.velocity.ToRotation();
            if (!NPC.AnyNPCs(NPCType<NPCs.Cecitior.Cecitior>()))
                Projectile.Kill();
            NPC player = Main.npc[(int)Projectile.ai[0]];
            if (player.ai[0] != 8)
                Projectile.Kill();
            if (Projectile.ai[1] == 1)
            {
                player.velocity = Helper.FromAToB(player.Center, Projectile.Center) * 25;
                if (player.Center.Distance(Projectile.Center) < 50)
                {
                    Projectile.ai[2] = 1;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0, 0), ProjectileType<FatSmash>(), 0, 0, 0, 0);

                    for (int i = -6; i < 6; i++)
                    {
                        if (i == 0) continue;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(i * 3, Lerp(-3, -5, MathF.Abs(i) / 6)), ProjectileType<CecitiorTeeth>(), 20, 0, 0, 0);
                    }
                    player.velocity = Projectile.rotation.ToRotationVector2().RotatedByRandom(PiOver4) * -10f;
                    Projectile.Kill();
                    SoundEngine.PlaySound(EbonianSounds.cecitiorSlam, Projectile.Center);
                }
            }
            else if (Projectile.ai[1] == 2)
            {
                Player playerr = Main.player[(int)Projectile.localAI[0]];
                playerr.velocity = Helper.FromAToB(playerr.Center, player.Center, false) / 10;
                Projectile.velocity = Helper.FromAToB(Projectile.Center, player.Center) * 20;

            }
            else
            {
                if (Projectile.velocity.Length() < 24)
                    Projectile.velocity *= 1.15f;
                if (Projectile.timeLeft < 100)
                {
                    Projectile.Center = Vector2.Lerp(Projectile.Center, player.Center, 0.4f);

                    if (player.Center.Distance(Projectile.Center) < 50)
                        Projectile.Kill();
                }
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {

            NPC player = Main.npc[(int)Projectile.ai[0]];
            Vector2 neckOrigin = Projectile.Center;
            Vector2 center = player.Center;
            Vector2 distToProj = neckOrigin - player.Center;
            float projRotation = distToProj.ToRotation() - 1.57f;
            float distance = distToProj.Length();
            while (distance > 20 && !float.IsNaN(distance))
            {
                distToProj.Normalize();
                distToProj *= 20;
                center += distToProj;
                distToProj = neckOrigin - center;
                distance = distToProj.Length();

                //Draw chain
                Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Projectiles/Friendly/Crimson/LatcherP_Chain").Value, center - Main.screenPosition,
                    null, Lighting.GetColor((int)center.X / 16, (int)center.Y / 16), projRotation,
                    Mod.Assets.Request<Texture2D>("Projectiles/Friendly/Crimson/LatcherP_Chain").Value.Size() / 2, 1f, SpriteEffects.None, 0);
            }
            return true;
        }
    }
}
