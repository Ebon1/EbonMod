using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using EbonianMod.NPCs.Garbage;
using EbonianMod.Projectiles.VFXProjectiles;
using EbonianMod.NPCs.Exol;

namespace EbonianMod.Projectiles.Exol
{
    internal class EGroundHazard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }
        public override string Texture => Helper.Placeholder;
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 500;
            Projectile.scale = 0;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Helper.GetExtraTexture("gradation");
            Main.spriteBatch.Reload(BlendState.Additive);
            for (int i = 0; i < 2; i++)
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.OrangeRed * Projectile.scale * 0.75f, Projectile.rotation, new Vector2(tex.Width / 2, tex.Height - 100), new Vector2(4800, Projectile.scale), SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void AI()
        {
            if (Main.rand.NextBool(2))
            {
                Projectile a = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center + new Vector2(Main.maxTilesX * 16 * Main.rand.NextFloat(), -20), new Vector2(Main.rand.NextFloat(-1, 1), -Main.rand.NextFloat(3, 10)), ModContent.ProjectileType<Gibs>(), Projectile.damage, Projectile.knockBack, ai2: 1);
                a.friendly = false;
                a.hostile = true;
            }
            if (Projectile.ai[0] == 0)
            {
                Projectile.Center = Helper.TRay.Cast(new Vector2(10, Main.maxTilesY * 8), Vector2.UnitY, 1000);
                Projectile.ai[0] = 1;
            }
            if (Projectile.timeLeft < 250 && NPC.AnyNPCs(ModContent.NPCType<Ignos>()))
                Projectile.timeLeft = 250;
            float progress = Utils.GetLerpValue(0, 500, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 3, 0, 1);
        }
    }
}
