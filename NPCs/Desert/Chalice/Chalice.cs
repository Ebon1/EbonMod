using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using EbonianMod.Projectiles.Cecitior;

namespace EbonianMod.NPCs.Desert
{
    public class Chalice : ModNPC
    {
        public override string Texture => Helper.Empty;

        public override void SetDefaults()
        {
            NPC.width = 28;
            NPC.height = 34;
            NPC.damage = 30;
            NPC.defense = 0;
            NPC.lifeMax = 100;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath52;
        }

        float Argument = 0;
        bool AimingMode;
        Vector2 Position;
        float AttackCooldown;
        float NeededRotation;
        float SpeedCoefficent;

        public override void OnSpawn(IEntitySource source)
        {
            Position = NPC.position;
            AttackCooldown = Main.rand.NextFloat(4f, 6f);
        }

        public override void AI()
        {
            AttackCooldown -= 0.02f;
            if(AttackCooldown <= 0)
            {
                AimingMode = true;
            }
            NPC.TargetClosest(true);
            Player TargetPlayer = Main.player[NPC.target];
            Position = new Vector2(Lerp(Position.X, TargetPlayer.position.X, 0.011f*SpeedCoefficent), Lerp(Position.Y, TargetPlayer.position.Y, 0.05f*SpeedCoefficent));
            NPC.position = new Vector2(Position.X + MathF.Sin(Argument) * 100, Position.Y - 120 + MathF.Cos(Argument) * 26);
            if(AimingMode == false)
            {
                SpeedCoefficent = Lerp(SpeedCoefficent, 1f, 0.1f);
                Argument += 0.035f;
                NPC.rotation = Lerp(NPC.rotation, (float)MathF.Cos(Argument)/3.5f, 0.05f);
                NeededRotation = NPC.DirectionTo(TargetPlayer.Center).ToRotation() + PiOver2 - (NPC.direction == 1 ? Pi*2 : 0);
            }
            else
            {
                SpeedCoefficent = Lerp(SpeedCoefficent, 0f, 0.04f);
                NPC.rotation = Lerp(NPC.rotation, NeededRotation, 0.1f);
                if(MathF.Abs(NPC.rotation - NeededRotation) < 0.04f)
                {
                    AttackCooldown = Main.rand.NextFloat(4f, 6f);
                    AimingMode = false;
                    Vector2 direction = new Vector2(MathF.Cos(NPC.rotation-PiOver2), MathF.Sin(NPC.rotation-PiOver2));
                    Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, direction*40, ProjectileType<CIchor>(), 20, 0);
                    a.friendly = false;
                    a.hostile = true;
                    a.tileCollide = true;
                }
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {

        }

        public override void OnKill()
        {

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = Helper.GetTexture(Mod.Name + "/NPCs/Desert/Chalice/Chalice");
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, drawColor, NPC.rotation, texture.Size() / 2, NPC.scale, NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;
        }
    }
}
