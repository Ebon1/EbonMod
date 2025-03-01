using EbonianMod.Common.Systems.Verlets;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbonianMod.Items.Pets
{
    public class CecitiorPet : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ProjectileType<CecitiorPetP>();
            Item.width = 16;
            Item.height = 30;
            Item.UseSound = SoundID.Item2;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.rare = ItemRarityID.Green;
            Item.noMelee = true;
            Item.master = true;
            Item.value = Item.sellPrice(0, 5, 50, 0);
            Item.buffType = BuffType<CecitiorPetB>();
        }

        public override void UseStyle(Player player, Rectangle rec)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600, true);
            }
        }
    }
    public class CecitiorPetP : ModProjectile
    {
        public override string Texture => "EbonianMod/NPCs/Cecitior/CecitiorEye";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 19;
            Main.projPet[Projectile.type] = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }
        public override void SetDefaults()
        {
            Projectile.Size = new(32);
            Projectile.CloneDefaults(499);
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
        }
        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;
        Verlet verlet;
        public override void OnSpawn(IEntitySource source)
        {
            verlet = new(Projectile.Center, 7, 16, 1, true, true, 13);
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.HasBuff(BuffType<CecitiorPetB>()))
            {
                Projectile.timeLeft = 2;
            }
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(Projectile.Center, player.Center - new Vector2(player.direction * 50, 120), false) / 20, 0.1f);

            Projectile.rotation = Helper.LerpAngle(Projectile.rotation, Helper.FromAToB(Projectile.Center, Main.MouseWorld).ToRotation() + Pi, 0.1f);


            if (Projectile.frameCounter % 5 == 0)
            {
                if (Projectile.frame < 18 && Projectile.frame >= 16)
                    Projectile.frame++;
                else if (Projectile.frame < 16 || Projectile.frame >= 18)
                    Projectile.frame = 16;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.isAPreviewDummy) return true;
            Player player = Main.player[Projectile.owner];
            if (verlet != null)
            {
                verlet.Update(Projectile.Center, player.Center);
                verlet.gravity = (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.25f;
                verlet.Draw(Main.spriteBatch, "NPCs/Cecitior/CecitiorChain");
            }
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            if (Projectile.isAPreviewDummy) return;
            Player player = Main.player[Projectile.owner];
            Texture2D a = Helper.GetTexture("NPCs/Cecitior/CecitiorChain_base");
            Texture2D b = Helper.GetTexture("NPCs/Cecitior/CecitiorEye");
            if (verlet != null)
                Main.spriteBatch.Draw(a, verlet.firstP.position - new Vector2(0, 20).RotatedBy(Helper.FromAToB(verlet.firstP.position, verlet.points[5].position, reverse: true).ToRotation() - 1.57f) - Main.screenPosition, null, lightColor, Helper.FromAToB(verlet.firstP.position, verlet.points[5].position, reverse: true).ToRotation() - 1.57f, a.Size() / 2, 1, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(b, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * 34, 32, 34), lightColor, Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.None, 0);
        }
    }
    public class CecitiorPetB : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;

            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ProjectileType<CecitiorPetP>()] < 1)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ProjectileType<CecitiorPetP>(), 0, 0, 0);
            }
            else
                player.buffTime[buffIndex] = 18000;
        }
    }
}
