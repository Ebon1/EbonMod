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

namespace EbonianMod.Items.Pets.Hightoma
{
    public class CursedCone : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ProjectileType<Hightoma>(), BuffType<HightomaB>());
            Item.rare = ItemRarityID.Master;
            Item.master = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.NPCHit54;
            Item.useTime = Item.useAnimation = 80;
        }
    }
    public class Hightoma : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.EyeOfCthulhuPet);
            Projectile.aiStyle = -1;
            Projectile.Size = new Vector2(36);
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.active && player.HasBuff(BuffType<HightomaB>()))
                Projectile.timeLeft = 10;
            //Projectile.direction = Projectile.spriteDirection = player.direction;
            Projectile.velocity = Helper.FromAToB(Projectile.Center, player.Center - new Vector2(player.direction * -50, 100), false) * 0.1f;
            Projectile.rotation += MathHelper.ToRadians(1);

        }
        public override bool PreDraw(ref Color lightColor)
        {
            //UnifiedRandom rand = new UnifiedRandom(912301);
            Texture2D chain = Helper.GetTexture("Items/Pets/Hightoma/Hightoma_Chain");
            for (int i = -1; i < 2; i++)
            {
                //  float rando = rand.NextFloat(MathHelper.Pi * 2);
                Texture2D clinger = Helper.GetTexture("Items/Pets/Hightoma/Hightoma_Clinger" + (i + 2));
                Vector2 neckOrigin = Projectile.Center;
                Vector2 center = Projectile.Center - new Vector2(i * 30, -45) - Projectile.velocity * MathF.Abs(i);
                Vector2 distToProj = neckOrigin - center;
                float projRotation = distToProj.ToRotation() - 1.57f;
                float distance = distToProj.Length();
                Vector2 _center = center;
                while (distance > chain.Height && !float.IsNaN(distance))
                {
                    distToProj.Normalize();
                    distToProj *= chain.Height;
                    center += distToProj;
                    distToProj = neckOrigin - center;
                    distance = distToProj.Length();

                    //Draw chain
                    Main.spriteBatch.Draw(chain, center - Main.screenPosition,
                        null, Lighting.GetColor((int)center.X / 16, (int)center.Y / 16), projRotation,
                        chain.Size() / 2, 1f, SpriteEffects.None, 0);
                }
                Main.spriteBatch.Draw(clinger, _center - Main.screenPosition,
                    null, Lighting.GetColor((int)_center.X / 16, (int)_center.Y / 16), 0,
                    clinger.Size() / 2, 1f, SpriteEffects.None, 0);
            }
            return true;
        }
    }
    public class HightomaB : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;

            int projType = ProjectileType<Hightoma>();


            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[projType] <= 0)
            {
                var entitySource = player.GetSource_Buff(buffIndex);

                Projectile.NewProjectile(entitySource, player.Center, Vector2.Zero, projType, 0, 0f, player.whoAmI);
            }
        }
    }
}
