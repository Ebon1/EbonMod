using EbonianMod.Projectiles.Friendly.Corruption;
using EbonianMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using EbonianMod.Dusts;
using System.Xml.Linq;
using System.Collections;
using Microsoft.Xna.Framework.Graphics;

namespace EbonianMod.Items.Weapons.Melee
{
    public class Chainsword : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(3); // armor penetration
        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.width = Item.height = 80;
            Item.crit = 30;
            Item.damage = 20;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.noUseGraphic = true;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.channel = true;
            Item.DamageType = DamageClass.Melee;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Green;
            Item.shootSpeed = 1f;
            Item.ArmorPenetration = 3;
            Item.shoot = ProjectileType<ChainswordP>();
        }
        public override bool? CanAutoReuseItem(Player player)
        {
            return false;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, Vector2.UnitX * player.direction, type, damage, knockback, player.whoAmI, 0, -player.direction);
            return false;
        }
    }
    public class ChainswordP : HeldSword
    {
        public override void SetExtraDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 90;
            swingTime = 150;
            Projectile.extraUpdates = 3;
            holdOffset = 30;
            Projectile.tileCollide = false;
            glowBlend = BlendState.AlphaBlend;
            Projectile.usesLocalNPCImmunity = false;
        }
        public override float Ease(float x)
        {
            return x < 0.5 ? 4 * x * x * x : 1 - MathF.Pow(-2 * x + 2, 3) / 2;
        }
        float lerpProg = 1, swingProgress, rotation, timer;
        public override void ExtraAI()
        {
            Player player = Main.player[Projectile.owner];
            int direction = (int)Projectile.ai[1];
            float defRot = Projectile.velocity.ToRotation();
            float start = defRot - (MathHelper.PiOver2 + MathHelper.PiOver4);
            float end = defRot + (MathHelper.PiOver2 + MathHelper.PiOver4);
            if (lerpProg >= 0)
                rotation = MathHelper.Lerp(rotation, direction == 1 ? start + MathHelper.Pi * 3 / 2 * swingProgress : end - MathHelper.Pi * 3 / 2 * swingProgress, lerpProg);
            Vector2 position = player.GetFrontHandPosition(stretch, rotation - MathHelper.PiOver2) +
                rotation.ToRotationVector2() * holdOffset * ScaleFunction(swingProgress);
            Projectile.Center = position;
            Projectile.rotation = (position - player.Center).ToRotation() + MathHelper.PiOver4;
            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            player.heldProj = Projectile.whoAmI;
            if (player.gravDir != -1)
                player.SetCompositeArmFront(true, stretch, rotation - MathHelper.PiOver2);
            if (Projectile.timeLeft == swingTime - 11)
                SoundEngine.PlaySound(SoundID.Item22, Projectile.Center);
            if (lerpProg < 0.9f && lerpProg != -1)
            {
                holdOffset = baseHoldOffset + Main.rand.NextFloat(-3, 3f);
                glowAlpha = MathHelper.Lerp(glowAlpha, 1, 0.1f);
                Projectile.extraUpdates = 0;
                if (++timer % 3 != 0)
                {
                    Projectile.timeLeft++;
                    return;
                }
            }
            else
            {
                holdOffset = baseHoldOffset + Main.rand.NextFloat(-.3f, .3f);
                glowAlpha = MathHelper.Lerp(glowAlpha, 0, 0.2f);
                Projectile.extraUpdates = 3;
            }
            Lighting.AddLight(Projectile.Center, glowAlpha, glowAlpha * 0.6f, 0);
            lerpProg = MathHelper.Lerp(lerpProg, 1.05f, 0.275f);
            swingProgress = MathHelper.Lerp(swingProgress, Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft)), MathHelper.Clamp(lerpProg, -.1f, 1));
            Projectile.scale = 1 + Clamp(MathF.Sin(Pi * swingProgress) * 2, 0, 0.4f);
            //          if (lerpProg < 0)
            //                Projectile.timeLeft++;
            //if (lerpProg < 0.2f)
            //lerpProg += 0.025f;
            //if (lerpProg < 1)
            //  lerpProg -= 0.2f;
        }
        public override bool? CanDamage()
        {
            return Projectile.ai[0] < 1 && swingProgress > 0.35f && swingProgress < 0.65f;
        }
        public override void OnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 pos = Projectile.Center + Projectile.velocity * 5;
            Vector2 vel = Helper.FromAToB(player.Center, pos).RotatedBy(Projectile.ai[1]).RotatedByRandom(MathHelper.PiOver4);
            if (swingProgress.CloseTo(0.5f, 0.2f))
                lerpProg = MathHelper.Lerp(lerpProg, 0.005f, 0.85f);
            if (Projectile.extraUpdates == 3)
            {

                for (int i = 0; i < 70; i++)
                {
                    if (Main.rand.NextBool())
                        Dust.NewDustPerfect(pos + Helper.FromAToB(Projectile.Center, pos).RotatedByRandom(MathHelper.PiOver4 * 0.7f) * Main.rand.NextFloat(5, 35), DustType<LineDustFollowPoint>(), vel * Main.rand.NextFloat(5, 8), 0, Color.Lerp(Color.White, Color.Orange, Main.rand.NextFloat()), Main.rand.NextFloat(0.05f, 0.1f));
                    else
                        Dust.NewDustPerfect(pos + Helper.FromAToB(Projectile.Center, pos).RotatedByRandom(MathHelper.PiOver4 * 0.7f) * Main.rand.NextFloat(5, 35), DustType<SparkleDust>(), vel * Main.rand.NextFloat(5, 8), 0, Color.Lerp(Color.White, Color.Orange, Main.rand.NextFloat()), Main.rand.NextFloat(0.025f, 0.075f));
                }

            }
            for (int i = 0; i < 10; i++)
            {
                if (Main.rand.NextBool())
                    Dust.NewDustPerfect(pos + Helper.FromAToB(Projectile.Center, pos).RotatedByRandom(MathHelper.PiOver4 * 0.7f) * Main.rand.NextFloat(5, 35), DustType<LineDustFollowPoint>(), vel * Main.rand.NextFloat(5, 8), 0, Color.Lerp(Color.White, Color.Orange, Main.rand.NextFloat()), Main.rand.NextFloat(0.05f, 0.1f));
                else
                    Dust.NewDustPerfect(pos + Helper.FromAToB(Projectile.Center, pos).RotatedByRandom(MathHelper.PiOver4 * 0.7f) * Main.rand.NextFloat(5, 35), DustType<SparkleDust>(), vel * Main.rand.NextFloat(5, 8), 0, Color.Lerp(Color.White, Color.Orange, Main.rand.NextFloat()), Main.rand.NextFloat(0.025f, 0.075f));
            }
        }
    }
}
