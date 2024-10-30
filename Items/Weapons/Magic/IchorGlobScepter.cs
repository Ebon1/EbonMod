using EbonianMod.Items.Materials;
using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Terraria.ModLoader.ModContent;

namespace EbonianMod.Items.Weapons.Magic
{
    public class IchorGlobScepter : ModItem
    {
        public override void SetStaticDefaults()
        {
            //ItemGlowy.AddItemGlowMask(Item.type, "EbonianMod/Items/Weapons/Magic/IchorGlobScepter_Glow");

        }

        public override void SetDefaults()
        {
            Item.damage = 38;
            Item.width = 32;
            Item.height = 38;
            Item.maxStack = 1;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2f;
            Item.rare = ItemRarityID.LightRed;
            Item.mana = 10;
            Item.noMelee = true;
            Item.staff[Item.type] = true;
            Item.shoot = ModContent.ProjectileType<IchorGlob>();
            Item.UseSound = SoundID.Item8;
            Item.shootSpeed = 3f;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Magic;

        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.CrimsonRod).AddIngredient(ModContent.ItemType<CecitiorMaterial>(), 20).AddTile(TileID.MythrilAnvil).Register();
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture = Request<Texture2D>("EbonianMod/Items/Weapons/Magic/IchorGlobScepter_Glow", AssetRequestMode.ImmediateLoad).Value;
            spriteBatch.Draw
            (
                texture,
                new Vector2
                (
                    Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                    Item.position.Y - Main.screenPosition.Y + Item.height - texture.Height * 0.5f + 2f
                ),
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.NavajoWhite,
                rotation,
                texture.Size() * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }
    }
    public class IchorGlob : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 20;
            Projectile.aiStyle = 0;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;

            Projectile.timeLeft = 90;
            Projectile.scale = 1f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.NavajoWhite;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        int seed;
        public override bool PreDraw(ref Color lightColor)
        {

            if (seed == 0) seed = Main.rand.Next(int.MaxValue / 2);
            Texture2D tex = Helper.GetExtraTexture("Extras2/scratch_03");
            float max = 40;
            Main.spriteBatch.Reload(BlendState.Additive);
            UnifiedRandom rand = new UnifiedRandom(seed);
            float ringScale = MathHelper.Lerp(1, 0, MathHelper.Clamp(Projectile.ai[2] * 3.5f, 0, 1));
            if (ringScale > 0.01f)
            {
                for (float i = 0; i < max; i++)
                {
                    UnifiedRandom rand2 = new UnifiedRandom(seed + (int)i);
                    float angle = Helper.CircleDividedEqually(i, max);
                    float scale = rand.NextFloat(0.1f, .5f);
                    Vector2 offset = new Vector2(rand2.NextFloat(100, 200) * (ringScale + rand2.NextFloat(-0.2f, 0.5f)) * scale, 0).RotatedBy(angle);
                    Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.Maroon * ringScale, angle, tex.Size() / 2, new Vector2(MathHelper.Clamp(Projectile.ai[2] * 6.5f, 0, 1), ringScale) * scale * 0.2f, SpriteEffects.None, 0);
                }
            }
            rand = new UnifiedRandom(seed + 1);
            ringScale = MathHelper.Lerp(1, 0, MathHelper.Clamp(Projectile.ai[1] * 3.5f, 0, 1));
            if (ringScale > 0.01f)
            {
                for (float i = 0; i < max; i++)
                {
                    UnifiedRandom rand2 = new UnifiedRandom(seed + (int)i);
                    float angle = Helper.CircleDividedEqually(i, max);
                    float scale = rand.NextFloat(0.1f, .5f);
                    Vector2 offset = new Vector2(rand2.NextFloat(200, 400) * (ringScale + rand2.NextFloat(-0.2f, 0.5f)) * scale, 0).RotatedBy(angle);
                    for (float j = 0; j < 2; j++)
                        Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.Maroon * (ringScale * 0.7f), angle, tex.Size() / 2, new Vector2(MathHelper.Clamp(Projectile.ai[1] * 6.5f, 0, 1), ringScale) * scale * 0.2f, SpriteEffects.None, 0);
                }
            }
            rand = new UnifiedRandom(seed + 1);
            ringScale = MathHelper.Lerp(1, 0, MathHelper.Clamp(Projectile.ai[0] * 3.5f, 0, 1));
            if (ringScale > 0.01f)
            {
                for (float i = 0; i < max; i++)
                {
                    UnifiedRandom rand2 = new UnifiedRandom(seed + (int)i);
                    float angle = Helper.CircleDividedEqually(i, max);
                    float scale = rand.NextFloat(0.1f, .5f);
                    Vector2 offset = new Vector2(rand2.NextFloat(150, 300) * (ringScale + rand2.NextFloat(-0.2f, 0.5f)) * scale, 0).RotatedBy(angle);
                    for (float j = 0; j < 2; j++)
                        Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.Maroon * ringScale, angle, tex.Size() / 2, new Vector2(MathHelper.Clamp(Projectile.ai[0] * 6.5f, 0, 1), ringScale) * scale * 0.2f, SpriteEffects.None, 0);
                }
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return true;
        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath11);
            int radius = 150;

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC target = Main.npc[i];
                if (target.active && !target.friendly && Vector2.Distance(Projectile.Center, target.Center) < radius)
                {

                    target.SimpleStrikeNPC(damage: 25, 0);
                }
            }

            Player player = Main.player[Projectile.owner];
            //Screenshake system
            //  player.GetModPlayer<Screenshake>().SmallScreenshake = true;
            for (int i = 0; i < 3; i++)
            {
                float speedX = Projectile.velocity.X * Main.rand.NextFloat(.46f, .8f) + Main.rand.NextFloat(-7f, 8f);
                float speedY = Projectile.velocity.Y * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;

                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Helper.FromAToB(player.Center, Projectile.Center).RotatedBy(0) * 4, ModContent.ProjectileType<IchorGlobSmall>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Helper.FromAToB(player.Center, Projectile.Center).RotatedBy(MathHelper.PiOver4) * 4, ModContent.ProjectileType<IchorGlobSmall>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Helper.FromAToB(player.Center, Projectile.Center).RotatedBy(-MathHelper.PiOver4) * 4, ModContent.ProjectileType<IchorGlobSmall>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BloodExplosionWSprite>(), 0, 0);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Ichor, 400);

        }
        public override bool? CanHitNPC(NPC target)
        {
            return !target.friendly;
        }
        public override void AI()
        {
            if (Projectile.timeLeft < 40)
                Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], 1, 0.007f);
            if (Projectile.timeLeft < 30)
                Projectile.ai[1] = MathHelper.Lerp(Projectile.ai[1], 1, 0.011f);
            if (Projectile.timeLeft < 20)
                Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], 1, 0.021f);
            if (Projectile.timeLeft == 20)
                EbonianSystem.ScreenShakeAmount = 5;
            Projectile.rotation += 0.1f;
            Projectile.velocity *= 0.8f;
            Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Blood, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 1f);

        }

    }
    public class IchorGlobSmall : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;

            Projectile.aiStyle = 0;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 0.2f;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 80;
            Projectile.penetrate = 1;
            Projectile.alpha = 0;

        }
        public override void AI()
        {

            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 0.8f);
            Lighting.AddLight(Projectile.position, 0.1f, 0.1f, 0.1f);
            Lighting.Brightness(1, 1);
            Projectile.rotation += 0.3f;
            Projectile.velocity *= 0.97f;

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Ichor, 400);

        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.NavajoWhite;
        }

        public override void Kill(int timeleft)
        {



            //   Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, Mod.Find<ModGore>("LightbulbBulletGore1").Type, 1f);

            Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BloodExplosionWSprite>(), 0, 0);
            SoundEngine.PlaySound(SoundID.NPCDeath11);
        }
    }
}

