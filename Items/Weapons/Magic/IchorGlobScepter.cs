using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RealmOne.Items.Misc;
using RealmOne.Projectiles.Magic;
using RealmOne.RealmPlayer;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace RealmOne.Items.Weapons.Magic
{
	public class IchorGlobScepter : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Haemocele Glob Scepter");
			Tooltip.SetDefault("Shoots out an icky glob of ichor that splits into 3 exploding chunks of ichor");
            ItemGlowy.AddItemGlowMask(Item.type, "RealmOne/Items/Weapons/Magic/IchorGlobScepter_Glow");

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
			Item.shootSpeed = 3f;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Magic;
			Item.channel = true;

		}
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture = Request<Texture2D>("RealmOne/Items/Weapons/Magic/IchorGlobScepter_Glow", AssetRequestMode.ImmediateLoad).Value;
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
            for (int i = 0; i < 80; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                var d = Dust.NewDustPerfect(Main.LocalPlayer.Center, DustID.Ichor, speed * 4, Scale: 1.3f);
                ;
                d.noGravity = true;
                d.noLight = false;
            }
            Vector2 mouse = Main.MouseWorld;
            Projectile.NewProjectile(source, mouse.X, mouse.Y, 0f, 0f, type, damage, knockback, player.whoAmI);
            return false;
        }


        public override bool CanUseItem(Player player)
        {

            return player.ownedProjectileCounts[Item.shoot] < 1;

        }
      

    }
}

