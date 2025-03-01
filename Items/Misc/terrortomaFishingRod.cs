using EbonianMod.Items.Materials;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbonianMod.Items.Misc
{
    //ebon balance these

    public class terrortomaFishingRod : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WoodFishingPole);

            Item.fishingPole = 30;
            Item.shootSpeed = 12;
            Item.shoot = ProjectileType<terrortomaFishingRodBobber>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int bobberAmount = 3;
            float spreadAmount = 75f;

            for (int index = 0; index < bobberAmount; ++index)
            {
                Vector2 bobberSpeed = velocity + new Vector2(Main.rand.NextFloat(-spreadAmount, spreadAmount) * 0.05f, Main.rand.NextFloat(-spreadAmount, spreadAmount) * 0.05f);

                Projectile.NewProjectile(source, position, bobberSpeed, type, 0, 0f, player.whoAmI);
            }
            return false;
        }

        public override void ModifyFishingLine(Projectile bobber, ref Vector2 lineOriginOffset, ref Color lineColor)
        {
            lineOriginOffset = new Vector2(41, -25);

            lineColor = new Color(93, 73, 75);
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.WoodFishingPole).AddIngredient<TerrortomaMaterial>(10).AddTile(TileID.MythrilAnvil).Register();
        }
    }

    public class cecitiorFishingRod : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WoodFishingPole);

            Item.fishingPole = 30;
            Item.shootSpeed = 12;
            Item.shoot = ProjectileType<cecitiorFishingRodBobber>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int bobberAmount = 3;
            float spreadAmount = 75f;

            for (int index = 0; index < bobberAmount; ++index)
            {
                Vector2 bobberSpeed = velocity + new Vector2(Main.rand.NextFloat(-spreadAmount, spreadAmount) * 0.05f, Main.rand.NextFloat(-spreadAmount, spreadAmount) * 0.05f);

                Projectile.NewProjectile(source, position, bobberSpeed, type, 0, 0f, player.whoAmI);
            }
            return false;
        }

        public override void ModifyFishingLine(Projectile bobber, ref Vector2 lineOriginOffset, ref Color lineColor)
        {
            lineOriginOffset = new Vector2(30, -25);

            lineColor = new Color(207, 160, 150);
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.WoodFishingPole).AddIngredient<CecitiorMaterial>(10).AddTile(TileID.MythrilAnvil).Register();
        }
    }

    public class terrortomaFishingRodBobber : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BobberWooden);

            DrawOriginOffsetY = 0;
        }
    }

    public class cecitiorFishingRodBobber : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BobberWooden);

            DrawOriginOffsetY = 0;
        }
    }
}
