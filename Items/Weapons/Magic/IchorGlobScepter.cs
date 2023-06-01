using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace EbonMod.Items.Weapons.Magic
{
	public class IchorGlobScepter : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Haemocele Glob Scepter");
			Tooltip.SetDefault("Shoots out an icky glob of ichor that splits into 3 exploding chunks of ichor");
            // If you dont have an glowmask system
	    //ItemGlowy.AddItemGlowMask(Item.type, "RealmOne/Items/Weapons/Magic/IchorGlobScepter_Glow");

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
			Item.rare = ItemRarityID.Pink;
			Item.mana = 10;
			Item.noMelee = true;
			Item.staff[Item.type] = true;
			Item.shoot = ModContent.ProjectileType<IchorGlob>();
			Item.UseSound = SoundID.Item8;
			Item.shootSpeed = 19f;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Magic;

		}
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture = Request<Texture2D>("EbonMod/Items/Weapons/Magic/IchorGlobScepter_Glow", AssetRequestMode.ImmediateLoad).Value;
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
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{

			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 25f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
				position += muzzleOffset;

			for (int i = 0; i < 80; i++)
			{
				Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
				var d = Dust.NewDustPerfect(Item.Center, DustID.Ichor, speed * 5, Scale: 1f);
				
				d.noGravity = true;
			}

			return true;
		}

	
	
		
	}
	 public class IchorGlob : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ichor Glob");


        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = 0;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;

            Projectile.timeLeft = 160;
            Projectile.scale = 1f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.NavajoWhite;
        }
        public override void Kill(int timeLeft)
        {


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
            player.GetModPlayer<Screenshake>().SmallScreenshake = true;
            for (int i = 0; i < 3; i++)
            {
                float speedX = Projectile.velocity.X * Main.rand.NextFloat(.46f, .8f) + Main.rand.NextFloat(-7f, 8f);
                float speedY = Projectile.velocity.Y * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;

                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y, 4, 0, ModContent.ProjectileType<IchorGlobSmall>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y, -4, 0, ModContent.ProjectileType<IchorGlobSmall>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y, 0, 4, ModContent.ProjectileType<IchorGlobSmall>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            for (int i = 0; i < 100; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.Ichor, speed * 10, Scale: 3f);
                ;
                d.noGravity = true;
            }
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
            Projectile.rotation += 0.1f;
            Projectile.velocity *= 0.95f;
            Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Blood, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 1f);

        }

    }
    public class IchorGlobSmall : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ichor Glob Small");
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

			Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Blood, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 0.8f);
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
			
			for (int i = 0; i < 50; i++)
			{
				Vector2 speed = Main.rand.NextVector2CircularEdge(0.5f, 0.5f);
				var d = Dust.NewDustPerfect(Projectile.Center, DustID.Ichor, speed * 5, Scale: 1f);
				;
				d.noGravity = true;
			}

			SoundEngine.PlaySound(SoundID.NPCDeath11);
		}
	}
}

