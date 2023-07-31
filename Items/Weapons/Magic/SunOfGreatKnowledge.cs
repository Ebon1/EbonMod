using EbonianMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Items.Weapons.Magic
{
    public class SunOfGreatKnowledge : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 35;
            Item.useAnimation = 35;
            Item.useStyle = 5;
            Item.value = Item.buyPrice(0, 1);
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.UseSound = SoundID.DD2_BetsyFireballShot;
            Item.value = 0;
            Item.rare = 2;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SunOfGreatKnowledgeP>();
            Item.shootSpeed = 1f;
        }
    }
    public class SunOfGreatKnowledgeP : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.aiStyle = 0;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;

            Projectile.timeLeft = 50;
            Projectile.scale = 1f;
        }
        public override void AI()
        {
            if (Projectile.velocity.Length() < 10)
                Projectile.velocity *= 1.05f;
            if (Projectile.timeLeft > 9)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<BlackWhiteDustExpand>(), Projectile.velocity);
                d.customData = "EbonianMod/Extras/Extras2/scorch_0" + Main.rand.Next(1, 4);


                Dust b = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<BlackWhiteDustExpand>(), -Projectile.velocity);
                b.customData = "EbonianMod/Extras/Extras2/scorch_0" + Main.rand.Next(1, 4);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[2] = 2;
        }
        public override void Kill(int timeLeft)
        {
            float offset = Main.rand.NextFloat(MathHelper.Pi);
            SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, Projectile.Center);
            for (int i = 0; i < 3 + Projectile.ai[2]; i++)
            {
                float angle = Helper.CircleDividedEqually(i, 3 + Projectile.ai[2]) + offset;
                Projectile a = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.UnitX.RotatedBy(angle) * Projectile.velocity.Length(), ModContent.ProjectileType<SunOfGreatKnowledgeP2>(), Projectile.damage + (int)(Projectile.ai[2] * 10), Projectile.knockBack, Projectile.owner, 15, 0, Projectile.ai[2] * 20);
                a.timeLeft = 80 + (int)(Projectile.ai[2] * 20);
            }
        }
    }
    public class SunOfGreatKnowledgeP2 : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.aiStyle = 0;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;

            Projectile.timeLeft = 80;
            Projectile.penetrate = 2;
            Projectile.scale = 1f;
        }
        public override void AI()
        {
            if (Projectile.timeLeft < 60 + Projectile.ai[2])
            {
                if (Main.myPlayer == Projectile.owner)
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(Projectile.Center, Main.MouseWorld) * Projectile.ai[0], 0.1f);
            }
            Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<BlackWhiteDust>(), Projectile.velocity);
            d.customData = "EbonianMod/Extras/Extras2/scorch_0" + Main.rand.Next(1, 4);
            d.scale = 0.25f;
        }
    }
}
