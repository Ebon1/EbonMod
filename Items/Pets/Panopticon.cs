using EbonianMod.Items.Pets.Hightoma;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
namespace EbonianMod.Items.Pets
{
    public class Panopticon : ModItem
    {
        public override void AddRecipes()
        {
            CreateRecipe().Register();
        }
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            Tooltip.SetDefault("Summons the Flesh Panopticon\nLIVE PANOPTICON REACTION");
        }
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<PanopticonP>(), ModContent.BuffType<PanopticonB>());
            Item.rare = ItemRarityID.Cyan;
            Item.useStyle = 10;
            Item.UseSound = SoundID.NPCHit54;
            Item.noUseGraphic = true;
            Item.useTime = Item.useAnimation = 80;
        }
    }
    public class PanopticonP : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[Projectile.type] = true;
            Main.projFrames[Type] = 32;
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.EyeOfCthulhuPet);
            Projectile.aiStyle = -1;
            Projectile.Size = new Vector2(34);
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (++Projectile.frameCounter % 5 == 0)
            {
                if (Projectile.frame < 31)
                    Projectile.frame++;
                else
                    Projectile.frame = 0;
            }
            if (player.active && player.HasBuff(ModContent.BuffType<PanopticonB>()))
                Projectile.timeLeft = 10;
            Projectile.ai[1] += MathHelper.ToRadians(1);
            Projectile.velocity = Helper.FromAToB(Projectile.Center, player.Center - (Vector2.UnitX * 100).RotatedBy(Projectile.ai[1]), false) * 0.1f;
        }

    }
    public class PanopticonB : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Panopticon");
            Description.SetDefault("This prison, to orbit around... ME!?");
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;

            int projType = ModContent.ProjectileType<PanopticonP>();


            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[projType] <= 0)
            {
                var entitySource = player.GetSource_Buff(buffIndex);

                Projectile.NewProjectile(entitySource, player.Center, Vector2.Zero, projType, 0, 0f, player.whoAmI);
            }
        }
    }
}
