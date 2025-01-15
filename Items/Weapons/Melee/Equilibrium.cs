using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using EbonianMod.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;
using Steamworks;
using Terraria.Audio;
using System.Xml.Serialization;
using EbonianMod.Items.Misc;
using EbonianMod.Projectiles.VFXProjectiles;
using EbonianMod.Common.Systems;
using EbonianMod.Items.Materials;

namespace EbonianMod.Items.Weapons.Melee
{
    public class Equilibrium : ModItem
    {
        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.width = 100;
            Item.height = 100;
            Item.damage = 65;
            Item.useAnimation = 32;
            Item.useTime = 32;
            Item.noUseGraphic = true;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.channel = true;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Lime;
            Item.shootSpeed = 1f;
            Item.shoot = ProjectileType<EquilibriumP>();
        }
        int dir = 1;
        public override bool? CanAutoReuseItem(Player player)
        {
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemType<Serration>()).AddIngredient(ItemType<MeatCrusher>()).AddIngredient(ItemID.BrokenHeroSword).AddTile(TileID.MythrilAnvil).Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            dir = -dir;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, dir);
            return false;
        }
    }
    public class EquilibriumP : HeldSword
    {
        public override void SetExtraDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            swingTime = 70 * 5;
            Projectile.extraUpdates = 4;
            holdOffset = 0;
            Projectile.tileCollide = false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public override float Ease(float x)
        {
            return x == 0
    ? 0
    : x == 1
    ? 1
    : x < 0.5 ? MathF.Pow(2, 20 * x - 10) / 2
    : (2 - MathF.Pow(2, -20 * x + 10)) / 2;
        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (player.active && player.channel && !player.dead && !player.CCed && !player.noItems)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    Vector2 dir = Vector2.Normalize(Main.MouseWorld - player.Center);
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), player.Center, dir, Projectile.type, Projectile.damage, Projectile.knockBack, player.whoAmI, 0, (-Projectile.ai[1]));
                    proj.rotation = Projectile.rotation;
                    proj.Center = Projectile.Center;
                }
            }
        }
        bool _hit;
        public override void OnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (!_hit)
            {
                SoundEngine.PlaySound(EbonianSounds.FleshImpact, Projectile.Center);
                Vector2 dir = Vector2.Normalize(Main.MouseWorld - player.Center);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity * 50, dir, ProjectileType<EquilibriumP2>(), Projectile.damage, Projectile.knockBack, player.whoAmI, 0, (Projectile.ai[1]));
                proj.rotation = Projectile.rotation;
                proj.Center = Projectile.Center + Projectile.velocity * 50;
                proj.timeLeft = 60 * 5 - 15 * 5;
                //if (Projectile.ai[0] == 3)
                {
                    Projectile a = Projectile.NewProjectileDirect(null, Projectile.Center + Projectile.velocity * 50, Vector2.Zero, Projectile.ai[1] == 1 ? ProjectileType<OstertagiExplosion>() : ProjectileType<BloodExplosionWSprite>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    a.hostile = false;
                    a.friendly = true;
                }
                _hit = true;
            }
        }
        public override bool? CanDamage() => (Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft)).CloseTo(0.5f, 0.4f));
        public override void ExtraAI()
        {
            Projectile.ai[2] = 1;
            Player player = Main.player[Projectile.owner];
            int direction = (int)Projectile.ai[1];
            float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            float defRot = Projectile.velocity.ToRotation();
            float start = defRot - (MathHelper.PiOver2 + MathHelper.PiOver4);
            float end = defRot + (MathHelper.PiOver2 + MathHelper.PiOver4);
            float rotation = direction == 1 ? start + MathHelper.Pi * 3 / 6 * swingProgress : end - MathHelper.Pi * 3 / 6 * swingProgress;
            rotation = Projectile.velocity.ToRotation();
            Vector2 position = player.GetFrontHandPosition(stretch, rotation - MathHelper.PiOver2); //+
                                                                                                    //rotation.ToRotationVector2() * holdOffset * ScaleFunction(swingProgress);
            Projectile.Center = player.GetFrontHandPosition(stretch, rotation - MathHelper.PiOver2) + Projectile.velocity.ToRotation().ToRotationVector2() * holdOffset;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);

            if (Projectile.timeLeft == swingTime - 25 * 5)
                SoundEngine.PlaySound(EbonianSounds.HeavySwing, Projectile.Center);

            if (Projectile.timeLeft <= 18 * 5)
            {
                if (player.active && player.channel && !player.dead && !player.CCed && !player.noItems)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Vector2 dir = Vector2.Normalize(Main.MouseWorld - player.Center);
                        Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), player.Center, dir, Projectile.type, Projectile.damage, Projectile.knockBack, player.whoAmI, (Projectile.ai[0] > 3 ? 0 : Projectile.ai[0] + 1), (-Projectile.ai[1]));
                        proj.rotation = Projectile.rotation;
                        proj.Center = Projectile.Center;
                        proj.timeLeft = swingTime - 18 * 5;
                        Projectile.active = false;
                    }
                }
            }
            Projectile.scale = MathHelper.Clamp(MathF.Sin(swingProgress * MathF.PI) * 0.1f + 1, 0.975f, 1.05f);
            float angle = MathHelper.Lerp(-MathHelper.PiOver2 - MathHelper.PiOver4 * 0.5f, MathHelper.Pi + MathHelper.PiOver4 * 0.5f, -swingProgress + 1);
            float rot = angle + (Projectile.ai[1] == 1 ? 0 : MathHelper.Pi) + MathHelper.PiOver2;

            Arm(player);
            if (swingProgress.CloseTo(0.5f, 0.45f) && swingProgress < 0.85f && Projectile.timeLeft % 2 == 0)
                oldP.Add(rot);
        }
        void Arm(Player player)
        {

            int direction = (int)Projectile.ai[1];
            float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            float defRot = Projectile.velocity.ToRotation();
            float start = defRot - (MathHelper.PiOver2 + MathHelper.PiOver4);
            float end = defRot + (MathHelper.PiOver2 + MathHelper.PiOver4);
            float rot = direction == 1 ? start + MathHelper.Pi * 3 / 2 * swingProgress : end - MathHelper.Pi * 3 / 2 * swingProgress;
            player.SetCompositeArmFront(true, stretch, rot - MathHelper.PiOver2);
        }
        List<float> oldP = new List<float>(30);
        public override bool PreDraw(ref Color lightColor)
        {
            float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            float swingProgressInv = MathHelper.Lerp(1, 0, swingProgress);
            float angle = MathHelper.Lerp(-MathHelper.PiOver2 - MathHelper.PiOver4 * 0.5f, MathHelper.Pi + MathHelper.PiOver4 * 0.5f, -swingProgress + 1);
            float f = MathHelper.Clamp(MathF.Sin((Projectile.ai[1] == 1 ? swingProgress : swingProgressInv) * MathF.PI) * 3, 0, 1);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 orig = new Vector2(texture.Width / 2, texture.Height / 2);
            float rot = angle + (Projectile.ai[1] == 1 ? 0 : MathHelper.Pi) + MathHelper.PiOver2;
            //Vector2 scale = new Vector2(Projectile.scale * Projectile.scale, .75f * Projectile.scale + (Projectile.ai[1] == 1 ? swingProgress : swingProgressInv) * 0.15f) * .2f;

            Vector2 scale = new Vector2(Projectile.scale * Projectile.scale + f * 0.05f, .65f * Projectile.scale + f * 0.15f) * .2f;



            if (oldP.Count > 2)
            {
                float fadeMult = 1 / (float)oldP.Count;
                for (int i = 0; i < oldP.Count; i++)
                {
                    Main.spriteBatch.Reload(EbonianMod.SpriteRotation);

                    EbonianMod.SpriteRotation.Parameters["scale"].SetValue(scale * 1.15f);
                    EbonianMod.SpriteRotation.Parameters["uColor"].SetValue((Color.White * (fadeMult * i * fadeMult * i * f * 0.15f)).ToVector4());
                    EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(oldP[i]);
                    Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.velocity.ToRotation(), orig, Projectile.scale * (4f), Projectile.ai[1] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                    Main.spriteBatch.Reload(effect: null);
                }
            }
            Main.spriteBatch.Reload(EbonianMod.SpriteRotation);

            EbonianMod.SpriteRotation.Parameters["scale"].SetValue(scale);
            EbonianMod.SpriteRotation.Parameters["uColor"].SetValue((Color.White).ToVector4());
            EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(rot);
            for (float i = -2; i < 3; i++)
            {
                Main.EntitySpriteDraw(texture, Projectile.Center + Projectile.velocity.RotatedBy(MathHelper.PiOver2) * i * .5f - Main.screenPosition, null, lightColor, Projectile.velocity.ToRotation(), orig, Projectile.scale * (5f), Projectile.ai[1] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(effect: null);


            if (oldP.Count < 20) return false;

            swingProgress = Ease(Utils.GetLerpValue(0f, swingTime + 50, Projectile.timeLeft + 50));
            swingProgressInv = MathHelper.Lerp(1, 0, swingProgress);
            f = MathHelper.Clamp(MathF.Sin((Projectile.ai[1] == 1 ? swingProgress : swingProgressInv) * MathF.PI) * .5f, 0, 1) * 2;

            Texture2D tex = Helper.GetExtraTexture("Extras2/slash_06");

            Main.spriteBatch.Reload(EbonianMod.SpriteRotation);
            EbonianMod.SpriteRotation.Parameters["scale"].SetValue(scale);
            EbonianMod.SpriteRotation.Parameters["uColor"].SetValue((Color.Black * (f * f * 0.2f)).ToVector4());
            EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(oldP[oldP.Count - 20] - MathHelper.PiOver2);
            for (float i = -5; i < 5; i++)
                Main.EntitySpriteDraw(tex, Projectile.Center + Projectile.velocity.RotatedBy(MathHelper.PiOver2) * i * 4 - Main.screenPosition, null, Color.White, Projectile.velocity.ToRotation(), tex.Size() / 2, Projectile.scale * (4.5f), Projectile.ai[1] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            Main.spriteBatch.Reload(effect: null);

            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Reload(EbonianMod.SpriteRotation);
            EbonianMod.SpriteRotation.Parameters["scale"].SetValue(scale);
            EbonianMod.SpriteRotation.Parameters["uColor"].SetValue((Color.Lerp(Color.Gold, Color.Maroon, 2 - f * 2) * (f * 0.25f)).ToVector4());
            EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(oldP[oldP.Count - 20] - MathHelper.PiOver2);
            for (float i = -5; i < 5; i++)
                Main.EntitySpriteDraw(tex, Projectile.Center + Projectile.velocity.RotatedBy(MathHelper.PiOver2) * i * 4 - Main.screenPosition, null, Color.White, Projectile.velocity.ToRotation(), tex.Size() / 2, Projectile.scale * (5f), Projectile.ai[1] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            Main.spriteBatch.Reload(effect: null);

            Main.spriteBatch.Reload(EbonianMod.SpriteRotation);
            EbonianMod.SpriteRotation.Parameters["scale"].SetValue(scale);
            EbonianMod.SpriteRotation.Parameters["uColor"].SetValue((Color.Lerp(Color.GreenYellow, Color.SandyBrown, 2 - f * 2) * (f * 0.225f)).ToVector4());
            EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(oldP[oldP.Count - 20] - MathHelper.PiOver2);
            for (float i = -5; i < 5; i++)
                Main.EntitySpriteDraw(tex, Projectile.Center + Projectile.velocity.RotatedBy(MathHelper.PiOver2) * i * 4 - Main.screenPosition, null, Color.White, Projectile.velocity.ToRotation(), tex.Size() / 2, Projectile.scale * (4f), Projectile.ai[1] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            Main.spriteBatch.Reload(effect: null);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player player = Main.player[Projectile.owner];
            int direction = (int)Projectile.ai[1];
            float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            float defRot = Projectile.velocity.ToRotation();
            float start = defRot - (MathHelper.PiOver2 + MathHelper.PiOver4);
            float end = defRot + (MathHelper.PiOver2 + MathHelper.PiOver4);
            float rot = direction == 1 ? start + MathHelper.Pi * 3 / 2 * swingProgress : end - MathHelper.Pi * 3 / 2 * swingProgress;
            Vector2 _start = player.Center;
            Vector2 _end = player.Center + rot.ToRotationVector2() * (Projectile.height * 0.8f);
            float a = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), _start, _end, projHitbox.Width / 4, ref a) && Collision.CanHitLine(player.TopLeft, player.width, player.height, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height);
        }
    }
    public class EquilibriumP2 : HeldSword
    {
        public override string Texture => "EbonianMod/Items/Weapons/Melee/EquilibriumP";
        public override void SetExtraDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            swingTime = 60 * 5;
            Projectile.extraUpdates = 4;
            holdOffset = 0;
            Projectile.tileCollide = false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public override float Ease(float x)
        {
            return x == 0
    ? 0
    : x == 1
    ? 1
    : x < 0.5 ? MathF.Pow(2, 20 * x - 10) / 2
    : (2 - MathF.Pow(2, -20 * x + 10)) / 2;
        }
        public override bool? CanDamage() => (Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft)).CloseTo(0.5f, 0.4f));
        public override void ExtraAI()
        {
            Projectile.ai[2] = 1;
            float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            if (Projectile.timeLeft == swingTime - 25 * 5)
                SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);

            Projectile.scale = MathHelper.Clamp(MathF.Sin(swingProgress * MathF.PI) * 0.1f + 1, 1.075f, 1.15f);
            float angle = MathHelper.Lerp(-MathHelper.PiOver2 - MathHelper.PiOver4 * 0.5f, MathHelper.Pi + MathHelper.PiOver4 * 0.5f, -swingProgress + 1);
            float rot = angle + (Projectile.ai[1] == 1 ? 0 : MathHelper.Pi) + MathHelper.PiOver2;
            if (swingProgress.CloseTo(0.5f, 0.45f) && swingProgress < 0.85f)
                oldP.Add(rot);
        }
        List<float> oldP = new List<float>(30);
        public override bool PreDraw(ref Color lightColor)
        {
            float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            float swingProgressInv = MathHelper.Lerp(1, 0, swingProgress);
            float angle = MathHelper.Lerp(-MathHelper.PiOver2 - MathHelper.PiOver4 * 0.5f, MathHelper.Pi + MathHelper.PiOver4 * 0.5f, -swingProgress + 1);
            float f = MathHelper.Clamp(MathF.Sin((Projectile.ai[1] == 1 ? swingProgress : swingProgressInv) * MathF.PI) * 3, 0, 1);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 orig = new Vector2(texture.Width / 2, texture.Height / 2);
            float rot = angle + (Projectile.ai[1] == 1 ? 0 : MathHelper.Pi) + MathHelper.PiOver2;
            //Vector2 scale = new Vector2(Projectile.scale * Projectile.scale, .75f * Projectile.scale + (Projectile.ai[1] == 1 ? swingProgress : swingProgressInv) * 0.15f) * .2f;

            Vector2 scale = new Vector2(Projectile.scale * Projectile.scale + f * 0.05f, .65f * Projectile.scale + f * 0.15f) * .2f;



            if (oldP.Count > 2)
            {
                float fadeMult = 1 / (float)oldP.Count;
                for (int i = 0; i < oldP.Count; i++)
                {
                    Main.spriteBatch.Reload(EbonianMod.SpriteRotation);

                    EbonianMod.SpriteRotation.Parameters["scale"].SetValue(scale * 1.15f);
                    EbonianMod.SpriteRotation.Parameters["uColor"].SetValue((Color.White * (fadeMult * i * fadeMult * i * f * 0.15f * (0.5f * f))).ToVector4());
                    EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(oldP[i]);
                    Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.velocity.ToRotation(), orig, Projectile.scale * (4f), Projectile.ai[1] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                    Main.spriteBatch.Reload(effect: null);
                }
            }
            Main.spriteBatch.Reload(EbonianMod.SpriteRotation);

            EbonianMod.SpriteRotation.Parameters["scale"].SetValue(scale);
            EbonianMod.SpriteRotation.Parameters["uColor"].SetValue((Color.White * (0.5f * f)).ToVector4());
            EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(rot);
            Main.EntitySpriteDraw(texture, Projectile.Center + Projectile.velocity.RotatedBy(MathHelper.PiOver2) - Main.screenPosition, null, Color.Maroon * 0.2f, Projectile.velocity.ToRotation(), orig, Projectile.scale * (5f), Projectile.ai[1] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            Main.spriteBatch.Reload(effect: null);


            if (oldP.Count < 20) return false;

            swingProgress = Ease(Utils.GetLerpValue(0f, swingTime + 50, Projectile.timeLeft + 50));
            swingProgressInv = MathHelper.Lerp(1, 0, swingProgress);
            f = MathHelper.Clamp(MathF.Sin((Projectile.ai[1] == 1 ? swingProgress : swingProgressInv) * MathF.PI) * .5f, 0, 1) * 2;

            Texture2D tex = Helper.GetExtraTexture("Extras2/slash_06");

            Main.spriteBatch.Reload(EbonianMod.SpriteRotation);
            EbonianMod.SpriteRotation.Parameters["scale"].SetValue(scale);
            EbonianMod.SpriteRotation.Parameters["uColor"].SetValue((Color.Black * (f * f * 0.2f) * (0.5f * f)).ToVector4());
            EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(oldP[oldP.Count - 20] - MathHelper.PiOver2);
            Main.EntitySpriteDraw(tex, Projectile.Center + Projectile.velocity.RotatedBy(MathHelper.PiOver2) - Main.screenPosition, null, Color.White, Projectile.velocity.ToRotation(), tex.Size() / 2, Projectile.scale * (4.5f), Projectile.ai[1] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            Main.spriteBatch.Reload(effect: null);

            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Reload(EbonianMod.SpriteRotation);
            EbonianMod.SpriteRotation.Parameters["scale"].SetValue(scale);
            EbonianMod.SpriteRotation.Parameters["uColor"].SetValue((Color.Lerp(Color.Gold, Color.Maroon, 2 - f * 2) * (f * 0.25f) * (0.5f * f)).ToVector4());
            EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(oldP[oldP.Count - 20] - MathHelper.PiOver2);
            Main.EntitySpriteDraw(tex, Projectile.Center + Projectile.velocity.RotatedBy(MathHelper.PiOver2) - Main.screenPosition, null, Color.White, Projectile.velocity.ToRotation(), tex.Size() / 2, Projectile.scale * (5f), Projectile.ai[1] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            Main.spriteBatch.Reload(effect: null);

            Main.spriteBatch.Reload(EbonianMod.SpriteRotation);
            EbonianMod.SpriteRotation.Parameters["scale"].SetValue(scale);
            EbonianMod.SpriteRotation.Parameters["uColor"].SetValue((Color.Lerp(Color.GreenYellow, Color.SandyBrown, 2 - f * 2) * (f * 0.225f) * (0.5f * f)).ToVector4());
            EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(oldP[oldP.Count - 20] - MathHelper.PiOver2);
            Main.EntitySpriteDraw(tex, Projectile.Center + Projectile.velocity.RotatedBy(MathHelper.PiOver2) - Main.screenPosition, null, Color.White, Projectile.velocity.ToRotation(), tex.Size() / 2, Projectile.scale * (4f), Projectile.ai[1] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            Main.spriteBatch.Reload(effect: null);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int direction = (int)Projectile.ai[1];
            float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            float defRot = Projectile.velocity.ToRotation();
            float start = defRot - (MathHelper.PiOver2 + MathHelper.PiOver4);
            float end = defRot + (MathHelper.PiOver2 + MathHelper.PiOver4);
            float rot = direction == 1 ? start + MathHelper.Pi * 3 / 2 * swingProgress : end - MathHelper.Pi * 3 / 2 * swingProgress;
            Vector2 _start = Projectile.Center;
            Vector2 _end = Projectile.Center + rot.ToRotationVector2() * (Projectile.height * 0.8f);
            float a = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), _start, _end, projHitbox.Width / 4, ref a);
        }
    }
}
