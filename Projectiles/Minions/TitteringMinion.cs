using EbonianMod.Common.Systems;
using EbonianMod.Items.Weapons.Magic;
using EbonianMod.Projectiles.Enemy.Corruption;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Projectiles.Minions
{
    public class TitteringMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 10;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(28, 30);
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 100;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1;
            Projectile.ignoreWater = true;
        }
        public override bool? CanDamage() => false;
        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;
            Projectile.rotation = 0;
            Projectile.ai[2] = -Projectile.minionPos * 10;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            EbonianPlayer modPlayer = player.GetModPlayer<EbonianPlayer>();
            if (player.dead)
                modPlayer.titteringMinion = false;
            if (modPlayer.titteringMinion)
                Projectile.timeLeft = 10;
            if (!player.active) return;
            if (Projectile.frameCounter++ % 5 == 0) Projectile.frame = (Projectile.frame < 9 ? Projectile.frame + 1 : 0);
            Vector2 targetPos = Projectile.position;
            Vector2 targetVel = Projectile.velocity;
            int index = -1;
            float targetDist = 600;
            bool target = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                //if (Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                {
                    targetDist = Vector2.Distance(Projectile.Center, targetPos);
                    targetPos = npc.Center;
                    targetVel = npc.velocity;
                    index = npc.whoAmI;
                    target = true;
                }
            }
            else
            {
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.active)
                        if (npc.CanBeChasedBy(this, false))
                        {
                            float distance = Vector2.Distance(npc.Center, Projectile.Center);
                            if ((distance < targetDist || !target))// && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                            {
                                targetDist = distance;
                                targetPos = npc.Center;
                                targetVel = npc.velocity;
                                index = npc.whoAmI;
                                target = true;
                            }
                        }
                }
            }
            if (player.Center.Distance(Projectile.Center) > 1000)
                Projectile.Center = player.Center;
            float off = 0;
            if (player.ownedProjectileCounts[Type] > 1)
                off = Helper.CircleDividedEqually(Projectile.minionPos, player.ownedProjectileCounts[Type]);

            if (target && targetDist < 600)
            {

                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(Projectile.Center, targetPos + new Vector2(0, -200).RotatedBy(off + MathF.Sin(Projectile.minionPos * 4 + Main.GlobalTimeWrappedHourly * 3) * 1.5f), false) / 40, 0.1f);
                Projectile.rotation = Helper.LerpAngle(Projectile.rotation, Helper.FromAToB(Projectile.Center, targetPos).ToRotation() + MathHelper.Pi, 0.25f);

                if (Projectile.ai[2] % 60 == 0)
                    SoundEngine.PlaySound(EbonianSounds.bloodSpit);
                if (Projectile.ai[2]++ % 60 == 50)
                {
                    Projectile p = Projectile.NewProjectileDirect(null, Projectile.Center, Helper.FromAToB(Projectile.Center, targetPos) * 10, ProjectileType<CursedToyP>(), Projectile.damage, 0);
                    p.DamageType = DamageClass.Summon;
                    p.tileCollide = false;
                    Projectile.velocity = -Helper.FromAToB(Projectile.Center, targetPos) * 4;
                    for (int i = 0; i < 15; i++)
                        Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, Helper.FromAToB(Projectile.Center, targetPos).RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(3, 6));
                }
            }
            else
            {

                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(Projectile.Center, player.Center + new Vector2(0, -200).RotatedBy(off + MathF.Sin(Projectile.minionPos * 4 + Main.GlobalTimeWrappedHourly * 3) * 1.5f), false) / 40, 0.1f);
                Projectile.rotation = Helper.LerpAngle(Projectile.rotation, Projectile.velocity.ToRotation() + MathHelper.Pi, 0.25f);
            }
            /*//Projectile.Center = player.Center;*/
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Helper.GetTexture(Texture);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * 32, 28, 32), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(28, 30) / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
