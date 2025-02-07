using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using EbonianMod.Dusts;
using EbonianMod.Projectiles.ArchmageX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.DataStructures;
using EbonianMod.Projectiles.VFXProjectiles;

namespace EbonianMod.Items.Weapons.Summoner
{
    public class ArchmageXTome : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 14;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.width = 26;
            Item.height = 28;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3;
            Item.value = Item.buyPrice(0, 8, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item44;
            Item.shoot = ProjectileType<XTomeSummon>();
            Item.buffType = BuffType<Buffs.XTomeBuff>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;

            return false;
        }
    }
    public class XTomeSummon : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetStaticDefaults()
        {

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;

            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 70;
            Projectile.minion = true;
            Projectile.minionSlots = 1;
            Projectile.Size = new(1, 1);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[ProjectileType<XAmethystShard>()].Value;
            Texture2D glow = Helper.GetTexture("Projectiles/ArchmageX/XAmethystShard_Glow");
            Main.spriteBatch.Reload(BlendState.Additive);
            for (int i = 0; i < 2; i++)
                Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + Projectile.minionPos, glow.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + Projectile.minionPos, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
        public override bool? CanDamage() => false;
        Vector2 basePos;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            EbonianPlayer modPlayer = player.GetModPlayer<EbonianPlayer>();
            if (player.dead)
            {
                modPlayer.xMinion = false;
            }
            if (modPlayer.xMinion)
            {
                Projectile.timeLeft = 2;
            }
            if (++Projectile.ai[1] % 100 == 0)
            {
                Projectile.NewProjectile(null, player.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), ProjectileType<XAnimeSlash>(), 0, 0, -1, 0, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.1f, 0.3f));
            }
            Vector2 targetPos = Projectile.position;
            Vector2 targetVel = Projectile.velocity;
            int index = -1;
            float targetDist = 1000;
            bool target = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (Collision.CanHitLine(player.position, player.width, player.height, npc.position, npc.width, npc.height))
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
                        if ((distance < targetDist || !target) && Collision.CanHitLine(player.position, player.width, player.height, npc.position, npc.width, npc.height))
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
                Vector2 vel = targetVel;
                if (vel.Length() > 0)
                    vel.Normalize();
                int atts = 0;
                Projectile.ai[0]++;
                if (vel.Length() > 0)
                    basePos = targetPos + vel.RotatedByRandom(MathHelper.PiOver4) * 150;
                else
                    basePos = targetPos + Main.rand.NextVector2Unit() * 150;
                while (++atts < 200 && !Collision.CanHit(Projectile, player))
                    basePos = targetPos + Main.rand.NextVector2Unit() * 150;
                if ((Projectile.ai[0] + Projectile.minionPos * 7) % 50 == 0)
                {
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), basePos + targetVel * 2, Helper.FromAToB(basePos + targetVel * 2, targetPos + targetVel) * 13, ProjectileType<XTomeP>(), Projectile.damage, 0f, player.whoAmI, index);
                }
            }
            Projectile.Center = player.Center + Vector2.UnitY.RotatedBy((player.ownedProjectileCounts[Type] > 1 ? Helper.CircleDividedEqually(Projectile.minionPos, player.ownedProjectileCounts[Type]) : 0) + ToRadians(player.GetModPlayer<EbonianPlayer>().consistentTimer * 3)) * (50 + MathF.Sin(player.GetModPlayer<EbonianPlayer>().consistentTimer * .1f + Projectile.minionPos) * 5);
        }
    }
    public class XTomeP : ModProjectile
    {
        public override string Texture => "EbonianMod/Projectiles/ArchmageX/XSpirit";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 2;

        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 30;
            Projectile.Size = new(34, 38);
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.timeLeft;
        }


        public override bool PreKill(int timeLeft)
        {
            return base.PreKill(timeLeft);
        }
        float vfxOffset;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Texture2D glow = Helper.GetTexture(Texture + "_Glow");
            Texture2D fireball = ExtraTextures.fireball;
            Main.spriteBatch.Reload(BlendState.Additive);
            vfxOffset += 0.05f;
            if (vfxOffset >= 1)
                vfxOffset = 0;
            vfxOffset = MathHelper.Clamp(vfxOffset, float.Epsilon, 1 - float.Epsilon);
            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float mult = (1f - fadeMult * i);
                if (i > 0 && Projectile.oldPos[i] != Vector2.Zero)
                {
                    Color col = Color.Lerp(Color.Indigo * Projectile.ai[2] * 0.5f, Color.Gray * Projectile.ai[2], (float)(i / Projectile.oldPos.Length)) * 2 * Projectile.ai[2];

                    float __off = vfxOffset;
                    float _off = MathF.Abs(__off + mult);
                    vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(25 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() + MathHelper.PiOver2), col, new Vector2(_off, 0)));
                    vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(25 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() - MathHelper.PiOver2), col, new Vector2(_off, 1)));
                }
            }
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (vertices.Count > 2)
            {
                Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, ExtraTextures.LintyTrail, false);
            }
            Main.spriteBatch.ApplySaved();
            Main.EntitySpriteDraw(fireball, Projectile.Center - Main.screenPosition, null, Color.Indigo * Projectile.ai[2] * 0.5f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, new Vector2(fireball.Width / 2, fireball.Height / 4), 1.2f + (((MathF.Sin(Main.GlobalTimeWrappedHourly * 5) + 1) / 2) * 0.4f), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(fireball, Projectile.Center - Main.screenPosition, null, Color.Indigo * Projectile.ai[2] * 0.5f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, new Vector2(fireball.Width / 2, fireball.Height / 4), 1.2f, SpriteEffects.None, 0);
            //Main.spriteBatch.Reload(BlendState.AlphaBlend);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.ai[2], Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.None, 0);
            //Main.spriteBatch.Reload(BlendState.Additive);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.ai[2] * 0.5f, Projectile.rotation, glow.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            return false;
        }
        public override void AI()
        {
            float progress = Utils.GetLerpValue(30, 0, Projectile.timeLeft);
            Projectile.rotation = Helper.LerpAngle(Projectile.rotation, Projectile.velocity.ToRotation(), 0.4f);
            Projectile.ai[2] = MathHelper.Clamp(MathF.Sin(progress * MathHelper.Pi), 0, 1);

            if (Projectile.ai[0] != -1)
            {
                NPC npc = Main.npc[(int)Projectile.ai[0]];
                if (npc.active && !npc.friendly && npc.CanBeChasedBy(this))
                {
                    if (Projectile.timeLeft > 15)
                        Projectile.velocity = Vector2.SmoothStep(Projectile.velocity, Helper.FromAToB(Projectile.Center, npc.Center) * 13, 0.2f);
                }
            }
        }
    }
}
