using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Items.Weapons.Ranged
{
    public class PipebombI : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Grenade);
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 9;
            Item.shoot = ModContent.ProjectileType<PipebombP>();
        }
    }
    public class PipebombP : ModProjectile
    {
        public override string Texture => "EbonianMod/Projectiles/Garbage/Pipebomb";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 25;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = 14;
            AIType = ProjectileID.BouncyGlowstick;
            Projectile.width = 18;
            Projectile.timeLeft = 300;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.hostile = false;
        }

        float savedP;
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = true;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            Texture2D tex = Helper.GetTexture(Texture + "_Bloom");
            Main.spriteBatch.Reload(BlendState.Additive);
            var fadeMult = 1f / Projectile.oldPos.Count();
            for (int i = 0; i < Projectile.oldPos.Count(); i++)
            {
                float mult = (1 - i * fadeMult);
                Main.spriteBatch.Draw(tex, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.Maroon * (mult * 0.4f), Projectile.oldRot[i], tex.Size() / 2, Projectile.scale * 1.1f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void AI()
        {
            if (savedP == 0)
                savedP = Main.player[Projectile.owner].Center.Y;
            Dust.NewDustPerfect(Projectile.Center - new Vector2(-8, 15).RotatedBy(Projectile.rotation), DustID.Torch);
        }
        public override void OnKill(int timeLeft)
        {
            Projectile a = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FlameExplosionWSprite>(), Projectile.damage / 3, 0, Projectile.owner);
            a.friendly = true;
            a.hostile = Projectile.ai[2] == 0;
        }
    }
}

