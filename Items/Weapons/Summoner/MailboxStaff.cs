using EbonianMod.Buffs;
using EbonianMod.Items.Weapons.Ranged;
using EbonianMod.Projectiles.Garbage;
using EbonianMod.Projectiles.Minions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static tModPorter.ProgressUpdate;

namespace EbonianMod.Items.Weapons.Summoner
{
    public class MailboxStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
            Item.staff[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 29;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.width = 26;
            Item.height = 28;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3;
            Item.sentry = true;
            Item.value = Item.buyPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item8;
            Item.shoot = ProjectileType<MailboxSentry>();
            Item.shootSpeed = 1;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
            while (Collision.SolidCollision(position - new Vector2(8, 8), 16, 16))
                position.Y -= 8;
        }
    }
    public class MailboxSentry : ModProjectile
    {
        public override string Texture => "EbonianMod/Projectiles/Garbage/Mailbox";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.sentry = true;
            Projectile.scale = 0;
            Projectile.hide = true;
            Projectile.DamageType = DamageClass.Summon;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => true;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            float scale = Math.Clamp(MathHelper.Lerp(0, 1, Projectile.scale * 2), 0, 1);
            Rectangle frame = new Rectangle(0, Projectile.frame * 46, 30, 46);
            Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(0, Projectile.gfxOffY) - Main.screenPosition, frame, Color.White, Projectile.rotation, new Vector2(tex.Width / 2, Projectile.height), new Vector2(Projectile.scale, 1), Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;
        }
        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;
            Player player = Main.player[Projectile.owner];
            if (Projectile.ai[0] == 0)
            {
                player.UpdateMaxTurrets();
                Projectile.Center = Helper.TRay.Cast(Projectile.Center, Vector2.UnitY, 1000, true) - new Vector2(0, 37);
                Projectile.ai[0] = 1;
            }
            Collision.StepDown(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
            Vector2 targetPos = Projectile.position;
            Vector2 targetVel = Projectile.velocity;
            int index = -1;
            float targetDist = 400;
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
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC npc = Main.npc[k];
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
            if (target)
            {
                Projectile.ai[1]++;

                if (Projectile.ai[1] == 100)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        Dust.NewDustPerfect(Projectile.Center - new Vector2(0, 6), DustID.Smoke, Helper.FromAToB(Projectile.Center, targetPos).RotatedByRandom(PiOver4) * Main.rand.NextFloat(2, 10));
                    }
                    Projectile.direction = Projectile.spriteDirection = Helper.FromAToB(Projectile.Center, targetPos).X > 0 ? 1 : -1;
                    Projectile.frame = 1;
                    SoundEngine.PlaySound(SoundID.Item156, Projectile.Center);
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Helper.FromAToB(Projectile.Center, targetPos) * 10, ProjectileType<PipebombPSentry>(), Projectile.damage, 0, Projectile.owner);
                }

                if (Projectile.ai[1] >= 115)
                {
                    Projectile.frame = 0;
                    Projectile.ai[1] = 0;
                }
            }
            else
            {
                Projectile.frame = 0;
                Projectile.ai[1] = 0;
            }
            Projectile.scale = Lerp(Projectile.scale, 1, 0.1f);
        }
    }
    public class PipebombPSentry : PipebombP
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Summon;
            Projectile.hostile = false;
        }
    }
}
