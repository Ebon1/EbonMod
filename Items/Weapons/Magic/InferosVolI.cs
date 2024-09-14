using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.ModLoader;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;

namespace EbonianMod.Items.Weapons.Magic
{
    public class InferosVolI : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 39;
            Item.width = 40;
            Item.height = 40;
            Item.mana = 25;
            Item.useTime = 120;
            Item.DamageType = DamageClass.Magic;
            Item.useAnimation = 120;
            Item.useStyle = 5;
            Item.useTurn = true;
            Item.knockBack = 10;
            Item.value = Item.sellPrice(gold: (int)2.5);
            Item.rare = 6;
            Item.UseSound = SoundID.AbigailUpgrade;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<InferosP>();
            Item.shootSpeed = 10f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.HellstoneBar, 35).AddTile(TileID.MythrilAnvil).Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 5; i++)
            {
                float angle = Helper.CircleDividedEqually(i, 5);
                Vector2 pos = player.Center + new Vector2(0f, -54).RotatedBy(angle /* +Main.GameUpdateCount * -0.0514f */);
                Projectile.NewProjectile(source, pos, angle.ToRotationVector2() * velocity.Length(), type, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }
    public class InferosP : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D a = ModContent.Request<Texture2D>("EbonianMod/Extras/Sprites/ExolPortal").Value;
            //Texture2D a = Helper.GetExtraTexture("explosion");
            //Main.spriteBatch.Reload(BlendState.Additive);
            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(a, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.OrangeRed * 0.5f * (1f - fadeMult * i) * alpha, 0, a.Size() / 2, 1f * (1f - fadeMult * i), SpriteEffects.None, 0);
                Main.spriteBatch.Draw(a, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.White * 0.5f * (1f - fadeMult * i) * alpha, 0, a.Size() / 2, 0.9f * (1f - fadeMult * i), SpriteEffects.None, 0);
            }
            //Main.spriteBatch.Reload(BlendState.AlphaBlend);
            Main.spriteBatch.Draw(a, Projectile.Center - Main.screenPosition, null, Color.Orange * alpha, 0, a.Size() / 2, 1f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(a, Projectile.Center - Main.screenPosition, null, Color.White * alpha, 0, a.Size() / 2, 0.9f, SpriteEffects.None, 0);

            return false;
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 2;
            Projectile.penetrate = -1;
        }
        float alpha = 1, alpha2 = 1;
        public override bool? CanDamage()
        {
            return Projectile.penetrate == -1;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
        {
            alpha2 -= 0.33333f;
        }
        public override void AI()
        {
            alpha = MathHelper.Lerp(alpha, alpha2, 0.1f);
            Vector2 move = Vector2.Zero;
            float distance = 1200;
            bool target = false;
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                if (Main.npc[k].active && !Main.npc[k].friendly && !Main.npc[k].dontTakeDamage && Main.npc[k].type != NPCID.TargetDummy)
                {
                    Vector2 newMove = Main.npc[k].Center - Projectile.Center;
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < 400)
                        Projectile.ai[1] = 10;
                    else
                        Projectile.ai[1] = 17;
                    if (distanceTo < distance)
                    {
                        move = newMove;
                        distance = distanceTo;
                        target = true;
                    }
                }
            }
            if (Projectile.timeLeft < 50)
            {
                if (target && Projectile.ai[0] == 0)
                {
                    Projectile.timeLeft = 2;
                    AdjustMagnitude(ref move);
                    Projectile.velocity = (Projectile.ai[1] * Projectile.velocity + move) / Projectile.ai[1];
                    AdjustMagnitude(ref Projectile.velocity);
                }
                else
                {
                    Projectile.ai[0] = 1;
                    alpha2 -= 0.025f;
                    Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(3));
                }
            }
            if (Projectile.timeLeft > 50)
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(3));
        }

        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > Projectile.ai[1])
            {
                vector *= Projectile.ai[1] / magnitude;
            }
        }
    }
    public class InferosDraw : PlayerDrawLayer
    {
        public override bool IsHeadLayer => false;
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Head);
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Texture2D InfernoTexture = ModContent.Request<Texture2D>("EbonianMod/Extras/Sprites/ExolPortal").Value;
            Texture2D rune = ModContent.Request<Texture2D>("EbonianMod/Extras/explosion").Value;
            Player player = Main.LocalPlayer;
            var position = player.Center;
            /*if (drawInfo.drawPlayer.HeldItem.type == ModContent.ItemType<InferosVolI>())
            {
                Main.spriteBatch.Reload(BlendState.Additive);
                Main.spriteBatch.Draw(rune, position - Main.screenPosition, null, Color.OrangeRed, Main.GameUpdateCount * -0.0514f, rune.Size() * 0.5f, 1f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(rune, position - Main.screenPosition, null, Color.White, Main.GameUpdateCount * -0.0514f, rune.Size() * 0.5f, 0.9f, SpriteEffects.None, 0);
                Main.spriteBatch.Reload(BlendState.AlphaBlend);
            }*/
            for (int i = 0; i < 5; i++)
            {
                float angle = Helper.CircleDividedEqually(i, 5);
                position = player.Center + new Vector2(0f, -54).RotatedBy(angle /* +Main.GameUpdateCount * -0.0514f */) - Main.screenPosition;
                position = new Vector2((int)position.X, (int)position.Y);

                if (drawInfo.drawPlayer.HeldItem.type == ModContent.ItemType<InferosVolI>())
                {
                    drawInfo.DrawDataCache.Add(new DrawData(
                    InfernoTexture,
                    position,
                    null,
                    Color.White,
                    Main.GameUpdateCount * 0.314f,
                    InfernoTexture.Size() * 0.5f,
                    1f,
                    SpriteEffects.None,
                    0
                ));
                }
            }
        }
    }
}
