using Microsoft.Xna.Framework;
using System;
using Terraria;
using EbonianMod.Items.Accessories;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace EbonianMod.Projectiles.Minions
{
    public abstract class MinionAI2 : MinionAI //this code is pretty horrible, ill rewrite it later.
    {
        protected float idleAccel = 0.05f;
        protected float spacingMult = 1f;
        protected float viewDist = 400f;
        protected float chaseDist = 200f;
        protected float chaseAccel = 6f;
        protected float inertia = 40f;
        protected float shootCool = 90f;
        protected float shootSpeed;
        protected bool Shoots = true;
        protected float rotateValue;
        protected int rotateCool = 5;
        protected int shoot;
        protected bool stayAboveHead = false;

        public float rotation;
        public int lmaoClickBait;

        public virtual void CreateDust()
        {
        }

        public virtual void SelectFrame()
        {
        }
        public virtual void ExtraAI()
        {

        }
        public virtual void ExtraTargetAI(Vector2 pos)
        {

        }

        public override void Behavior()
        {
            Player player = Main.player[Projectile.owner];

            if (!stayAboveHead)
            {
                float spacing = (float)Projectile.width * spacingMult;
                for (int k = 0; k < 1000; k++)
                {
                    Projectile otherProj = Main.projectile[k];
                    if (k != Projectile.whoAmI && otherProj.active && otherProj.owner == Projectile.owner && otherProj.type == Projectile.type && Math.Abs(Projectile.position.X - otherProj.position.X) + Math.Abs(Projectile.position.Y - otherProj.position.Y) < spacing)
                    {
                        if (Projectile.position.X < Main.projectile[k].position.X)
                        {
                            Projectile.velocity.X -= idleAccel;
                        }
                        else
                        {
                            Projectile.velocity.X += idleAccel;
                        }
                        if (Projectile.position.Y < Main.projectile[k].position.Y)
                        {
                            Projectile.velocity.Y -= idleAccel;
                        }
                        else
                        {
                            Projectile.velocity.Y += idleAccel;
                        }
                    }
                }
            }
            Vector2 targetPos = Projectile.position;
            float targetDist = viewDist;
            bool target = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                {
                    targetDist = Vector2.Distance(Projectile.Center, targetPos);
                    targetPos = npc.Center;
                    target = true;
                }
            }
            else
            {
                for (int k = 0; k < 200; k++)
                {
                    NPC npc = Main.npc[k];
                    if (npc.CanBeChasedBy(this, false))
                    {
                        float distance = Vector2.Distance(npc.Center, Projectile.Center);
                        if ((distance < targetDist || !target) && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                        {
                            targetDist = distance;
                            targetPos = npc.Center;
                            target = true;
                        }
                    }
                }
            }
            if (!stayAboveHead)
            {
                if (Vector2.Distance(player.Center, Projectile.Center) > (target ? 1000f : 500f))
                {
                    Projectile.ai[0] = 1f;
                    Projectile.netUpdate = true;
                }
                if (Projectile.ai[0] == 1f)
                {
                    Projectile.tileCollide = false;
                }
                if (target && Projectile.ai[0] == 0)
                {
                    Vector2 direction = targetPos - Projectile.Center;
                    if (direction.Length() > chaseDist)
                    {
                        direction.Normalize();
                        Projectile.velocity = (Projectile.velocity * inertia + direction * chaseAccel) / (inertia + 1);
                    }
                    else
                    {
                        Projectile.velocity *= (float)Math.Pow(0.97, 40.0 / inertia);
                    }
                }
                else
                {
                    if (!Collision.CanHitLine(Projectile.Center, 1, 1, player.Center, 1, 1))
                    {
                        Projectile.ai[0] = 1f;
                    }
                    float speed = 6f;
                    if (Projectile.ai[0] == 1f)
                    {
                        speed = 15f;
                    }
                    Vector2 center = Projectile.Center;
                    Vector2 direction = player.Center - center;
                    Projectile.ai[1] = 3600f;
                    Projectile.netUpdate = true;
                    int num = 1;
                    for (int k = 0; k < Projectile.whoAmI; k++)
                    {
                        if (Main.projectile[k].active && Main.projectile[k].owner == Projectile.owner && Main.projectile[k].type == Projectile.type)
                        {
                            num++;
                        }
                    }
                    direction.X -= (float)((10 + num * 40) * player.direction);
                    direction.Y -= 70f;
                    float distanceTo = direction.Length();
                    if (distanceTo > 200f && speed < 9f)
                    {
                        speed = 9f;
                    }
                    if (distanceTo < 100f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                    {
                        Projectile.ai[0] = 0;
                        Projectile.netUpdate = true;
                    }
                    if (distanceTo > 2000f)
                    {
                        Projectile.Center = player.Center;
                    }
                    if (distanceTo > 48f)
                    {
                        direction.Normalize();
                        direction *= speed;
                        float temp = inertia / 2f;
                        Projectile.velocity = (Projectile.velocity * temp + direction) / (temp + 1);
                    }
                    else
                    {
                        Projectile.direction = Main.player[Projectile.owner].direction;
                        Projectile.velocity *= (float)Math.Pow(0.9, 40.0 / inertia);
                    }
                }
            }
            if (!stayAboveHead)
            {
                Projectile.rotation = Projectile.velocity.X * 0.05f;
            }
            SelectFrame();
            CreateDust();
            if (Projectile.velocity.X > 0)
            {
                Projectile.spriteDirection = Projectile.direction = -1;
            }
            else if (Projectile.velocity.X < 0)
            {
                Projectile.spriteDirection = Projectile.direction = 1;
            }
            if (Projectile.ai[1] > 0)
            {
                Projectile.ai[1] += 1f;
                if (Main.rand.NextBool(3))
                {
                    Projectile.ai[1] += 1f;
                }
            }
            if (Projectile.ai[1] > shootCool)
            {
                Projectile.ai[1] = 0;
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[0] == 0)
            {
                if (target)
                {
                    if (stayAboveHead)
                    {
                        rotation += rotateValue;
                        if (++lmaoClickBait >= rotateCool)
                        {
                            Vector2 velocity = new Vector2(10f, 0).RotatedBy(MathHelper.ToRadians(rotation));
                            if (velocity.Length() < 3) velocity = Vector2.Normalize(velocity) * 3f;
                            {
                                int Proj = Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, velocity, shoot, 50, 0, 0, 1);
                                Main.projectile[Proj].tileCollide = false;
                                Main.projectile[Proj].timeLeft = 260;
                                Main.projectile[Proj].netUpdate = true;
                                Main.projectile[Proj].friendly = true;
                                Main.projectile[Proj].hostile = false;
                            }
                            lmaoClickBait = 0;
                        }
                    }
                    if (!stayAboveHead)
                    {
                        if ((targetPos - Projectile.Center).X > 0)
                        {
                            Projectile.spriteDirection = Projectile.direction = -1;
                        }
                        else if ((targetPos - Projectile.Center).X < 0)
                        {
                            Projectile.spriteDirection = Projectile.direction = 1;
                        }
                    }
                    if (Projectile.ai[1] == 0)
                    {
                        Projectile.ai[1] = 1f;
                        if (Shoots)
                        {

                            if (Main.myPlayer == Projectile.owner)
                            {
                                ExtraTargetAI(targetPos);
                                Vector2 shootVel = targetPos - Projectile.Center;
                                if (shootVel == Vector2.Zero)
                                {
                                    shootVel = new Vector2(0, 1f);
                                }
                                shootVel.Normalize();
                                shootVel *= shootSpeed;
                                int proj = Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center.X, Projectile.Center.Y, shootVel.X, shootVel.Y, shoot, Projectile.damage, Projectile.knockBack, Main.myPlayer, 0, 0);
                                Main.projectile[proj].timeLeft = 300;
                                Main.projectile[proj].netUpdate = true;
                                Main.projectile[proj].friendly = true;
                                Main.projectile[proj].hostile = false;
                                if (stayAboveHead)
                                {
                                    Main.projectile[proj].tileCollide = false;
                                    Main.projectile[proj].penetrate = -1;
                                }
                                Projectile.netUpdate = true;
                            }
                        }
                    }
                }
            }
            ExtraAI();
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = true;
            return true;
        }
    }
}