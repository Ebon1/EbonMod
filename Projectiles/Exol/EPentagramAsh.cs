using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using EbonianMod.Projectiles.VFXProjectiles;

namespace EbonianMod.Projectiles.Exol
{
    public class EPentagramAsh : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = 0;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Magic;

            Projectile.timeLeft = 300;
            Projectile.scale = 1f;
        }
        public override void PostDraw(Color lightColor)
        {

            Texture2D tex = ModContent.Request<Texture2D>("EbonianMod/Extras/Extras2/fire_01").Value;
            Texture2D tex2 = ModContent.Request<Texture2D>("EbonianMod/Extras/Extras2/fire_02").Value;
            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Draw(Projectile.ai[0] % 3 == 0 ? tex : tex2, Projectile.Center - Main.screenPosition, null, (Projectile.timeLeft < 250 - Projectile.ai[0] ? Color.OrangeRed : Color.Gray * 0.15f) * Projectile.scale, (Main.GameUpdateCount + Projectile.ai[1] + Projectile.ai[0] * 4) * 0.03f, tex.Size() / 2, 0.2f * Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(Projectile.ai[0] % 3 == 0 ? tex2 : tex, Projectile.Center - Main.screenPosition, null, (Projectile.timeLeft < 250 - Projectile.ai[0] ? Color.OrangeRed : Color.Gray * 0.15f) * Projectile.scale, (Main.GameUpdateCount - Projectile.ai[1] - Projectile.ai[0] * 4) * -0.03f, tex.Size() / 2, 0.2f * Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }
        public override bool? CanDamage()
        {
            return Projectile.timeLeft < 250 - Projectile.ai[0] && Projectile.timeLeft > 40 && Projectile.scale > 0.75f;
        }
        int side = 1;
        public override void OnSpawn(IEntitySource source)
        {
            NPC npc = Main.npc[(int)Projectile.localAI[0]];
            if (npc == null || !npc.active) return;
            if (npc.Center.X < Main.maxTilesX * 8) side = -1;
        }
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.localAI[0]];
            if (npc == null || !npc.active) Projectile.Kill();
            Projectile.Center = Vector2.Lerp(Projectile.Center, npc.Center + new Vector2(Projectile.ai[1], Projectile.ai[2]).RotatedBy(MathHelper.ToRadians(npc.ai[1] * 0.3f * side)), 0.2f);
            if (Projectile.timeLeft > 250 - Projectile.ai[0])
            {
                // if (Projectile.timeLeft % 5 == 0)
                //Helper.DustExplosion(Projectile.Center, Vector2.One, 2, Color.Gray * 0.45f, false, false, 0.6f, 1, Vector2.Zero);
            }
            else
            {
                if (Projectile.localAI[1] == 0 && npc.ai[1] < 310 && npc.ai[0] == 9)
                {
                    Projectile.localAI[1] = 1;
                    Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FlameExplosion2>(), 0, 0);
                }
                if (Main.rand.NextBool(3))
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                    Helper.DustExplosion(Projectile.Center, Vector2.One, 2, Color.OrangeRed * 0.25f, false, false, 0.6f, 1, Main.rand.NextVector2Unit());
                }
            }
            float progress = Utils.GetLerpValue(0, 300, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 3, 0, 1);
        }
    }
}
