using EbonianMod.Projectiles.Friendly.Corruption;
using EbonianMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using EbonianMod.Items.Weapons.Melee;
using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using EbonianMod.Common.Systems;
using EbonianMod.Projectiles.ArchmageX;
using EbonianMod.Common.Systems.Misc.Dialogue;
using Terraria.Utilities;
using Terraria.Graphics.CameraModifiers;

namespace EbonianMod.Items.Weapons.Magic
{
    public class StaffOfX : ModItem
    {
        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.width = Item.height = 80;
            Item.crit = 30;
            Item.damage = 350;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.noUseGraphic = true;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.channel = true;
            Item.DamageType = DamageClass.Magic;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.HiddenAnimation;
            Item.rare = ItemRarityID.Yellow;
            Item.shootSpeed = 1f;
            Item.shoot = ProjectileType<StaffOfXP>();
        }
        public override bool? CanAutoReuseItem(Player player)
        {
            return false;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, Vector2.UnitX * player.direction, type, damage, knockback, player.whoAmI, 0, -player.direction, 1);
            return false;
        }
    }
    public class StaffOfXP : HeldSword
    {
        public override string Texture => "EbonianMod/Items/Weapons/Magic/StaffOfX";
        public override void SetExtraDefaults()
        {
            Projectile.width = 162;
            Projectile.height = 162;
            swingTime = 90;
            Projectile.ignoreWater = true;
            useHeld = false;
            minSwingTime = 72;
            holdOffset = 90;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 130;
            ProjectileID.Sets.TrailingMode[Type] = 2;
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
        public override Color? GetAlpha(Color lightColor) => Color.White;
        float visualOff;
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > swingTime - 1) return false;
            visualOff -= 0.05f;
            if (visualOff <= 0)
                visualOff = 1;
            visualOff = MathHelper.Clamp(visualOff, float.Epsilon, 1 - float.Epsilon);
            Player player = Main.player[Projectile.owner];

            Main.spriteBatch.Reload(BlendState.Additive);

            Texture2D bloom = Helper.GetTexture(Texture + "_Bloom");
            Main.EntitySpriteDraw(bloom, Projectile.Center + visualOffset - Main.screenPosition, null, Color.Indigo * alphaOff, Projectile.rotation + (Projectile.ai[1] == -1 ? 0 : MathHelper.PiOver2 * 3), bloom.Size() / 2, Projectile.scale, Projectile.ai[1] == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 orig = texture.Size() / 2;
            Main.EntitySpriteDraw(texture, Projectile.Center + visualOffset - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), Color.White * alphaOff, Projectile.rotation + (Projectile.ai[1] == -1 ? 0 : MathHelper.PiOver2 * 3), orig, Projectile.scale, Projectile.ai[1] == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);

            Main.spriteBatch.Reload(BlendState.Additive);
            Main.EntitySpriteDraw(bloom, Projectile.Center + visualOffset - Main.screenPosition, null, Color.White * bloomAlpha * alphaOff, Projectile.rotation + (Projectile.ai[1] == -1 ? 0 : MathHelper.PiOver2 * 3), bloom.Size() / 2, Projectile.scale, Projectile.ai[1] == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);


            return false;
        }
        float bloomAlpha = 1, alphaOff = 1;
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.ChangeDir(Main.MouseWorld.X < player.Center.X ? -1 : 1);
            if (player.active && player.channel && !player.dead && !player.CCed && !player.noItems)
            {
                Projectile p = Projectile.NewProjectileDirect(null, Projectile.Center, Vector2.UnitX * player.direction, Projectile.type, Projectile.damage, Projectile.knockBack, player.whoAmI, 0, -player.direction, 1);
                p.Center = Projectile.Center;
                p.rotation = Projectile.rotation;
                p.timeLeft = swingTime + 10;
            }
        }
        float lerpProg = 1, swingProgress, rotation;
        public override void ExtraAI()
        {
            if (Projectile.timeLeft < swingTime - 5)
                bloomAlpha = Lerp(bloomAlpha, 0, 0.1f);
            if (bloomAlpha.CloseTo(0, 0.05f)) bloomAlpha = 0;
            if (lerpProg != 1 && lerpProg != -1)
                lerpProg = MathHelper.SmoothStep(lerpProg, 1, 0.1f);
            if (Projectile.timeLeft == swingTime - 1)
            {
                for (int i = 0; i < 15; i++)
                    Projectile.NewProjectile(null, Projectile.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(10, 20), ProjectileType<XAnimeSlash>(), 0, 0, -1, 0, Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(0.1f, 0.3f));

                Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0);
            }
            if (swingProgress > 0.25f && swingProgress < 0.85f)
                if (Projectile.ai[0] == 0 && Helper.TRay.CastLength(Projectile.Center, Vector2.UnitY, 100) < 30)
                {
                    Projectile.ai[0] = 1;
                    bloomAlpha = 1;
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 10, 6, 30, 1000));
                    Projectile.timeLeft = 15;
                    SoundEngine.PlaySound(SoundID.Item70, Projectile.Center);
                    SoundEngine.PlaySound(EbonianSounds.xSpirit, Projectile.Center);
                    WeightedRandom<string> chat = new();
                    chat.Add("OW!");
                    chat.Add("QUIT THAT!");
                    chat.Add("I WASN'T DESTINED FOR THIS!");
                    chat.Add("WHAT ARE YOU DOING!");
                    chat.Add("STOP!");
                    chat.Add("STOP THAT!");
                    chat.Add("QUIT IT!");
                    chat.Add("STOP IT!");
                    chat.Add("OUCH!");
                    chat.Add("DAMN YOU!");
                    DialogueSystem.NewDialogueBox(40, Projectile.Center - new Vector2(0, 40), chat, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 8f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                    Projectile.NewProjectile(null, Helper.TRay.Cast(Projectile.Center, Vector2.UnitY, 80), Vector2.Zero, ProjectileType<XImpact>(), 0, 0);

                    lerpProg = -1;
                }

            if (Projectile.timeLeft == 45)
            {
                float targetDist = 400;
                bool target = false;

                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.active)
                        if (npc.CanBeChasedBy(this, false))
                        {
                            float distance = Vector2.Distance(npc.Center, Projectile.Center);
                            if ((distance < targetDist || !target))// && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                            {
                                targetDist = distance;
                                target = true;
                            }
                        }
                }
                if (!target || !Main.rand.NextBool(3))
                    return;
                bool failed = false;
                for (int i = Projectile.Center.ToTileCoordinates().X - 10; i < Projectile.Center.ToTileCoordinates().X + 10; i++)
                    for (int j = Projectile.Center.ToTileCoordinates().Y - 10; j < Projectile.Center.ToTileCoordinates().Y + 10; j++)
                        if (Main.tile[i, j].HasTile)
                            failed = true;

                if (!failed)
                {
                    WeightedRandom<string> chat = new();
                    chat.Add("YEAH!");
                    chat.Add("WOOO!");
                    chat.Add("TAKE THAT!");
                    chat.Add("AHAHAHA!");
                    chat.Add("WOOHOOO!");
                    chat.Add("DIEE!");
                    chat.Add("YEAH DIE!");
                    DialogueSystem.NewDialogueBox(40, Projectile.Center - new Vector2(0, 40), chat, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 8f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                }
            }
            if (Projectile.timeLeft <= 15)
            {
                alphaOff = Lerp(0, 1, (float)Projectile.timeLeft / 15);
            }
            Player player = Main.player[Projectile.owner];
            int direction = (int)Projectile.ai[1];
            if (lerpProg >= 0)
                swingProgress = MathHelper.Lerp(swingProgress, Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft)), lerpProg);
            float defRot = Projectile.velocity.ToRotation();
            float start = defRot - (MathHelper.PiOver2 + MathHelper.PiOver4);
            float end = defRot + (MathHelper.PiOver2 + MathHelper.PiOver4);
            if (lerpProg >= 0)
                rotation = MathHelper.Lerp(rotation, direction == 1 ? start + MathHelper.Pi * 3 / 2 * swingProgress : end - MathHelper.Pi * 3 / 2 * swingProgress, lerpProg);
            Vector2 position = player.GetFrontHandPosition(stretch, rotation - MathHelper.PiOver2) +
                rotation.ToRotationVector2() * holdOffset * ScaleFunction(swingProgress);
            if (Projectile.timeLeft > swingTime)
            {
                if (player.gravDir != -1)
                    player.SetCompositeArmFront(true, stretch, rotation - MathHelper.PiOver2);
                return;
            }
            Projectile.Center = position;
            Projectile.rotation = (position - player.Center).ToRotation() + MathHelper.PiOver4;
            if (player.gravDir != -1)
                player.SetCompositeArmFront(true, stretch, rotation - MathHelper.PiOver2);

            if (lerpProg > -1 && swingProgress > 0.15f && swingProgress < 0.85f && Projectile.timeLeft % 2 == 0)
            {
                Projectile.NewProjectile(null, Projectile.Center + rotation.ToRotationVector2() * Projectile.height * 0.5f, Helper.FromAToB(Projectile.Center + rotation.ToRotationVector2() * Projectile.height, Main.MouseWorld).RotatedByRandom(PiOver4 * MathF.Sin(MathHelper.Pi * swingProgress)) * 15, ProjectileType<XBoltFriendly2>(), Projectile.damage, 0, player.whoAmI);
            }
        }
        public override bool? CanDamage()
        {
            return Projectile.ai[0] < 1 && swingProgress > 0.35f && swingProgress < 0.65f;
        }
        bool _hit;
        public override void OnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }
    }
}
