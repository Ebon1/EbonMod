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
}

