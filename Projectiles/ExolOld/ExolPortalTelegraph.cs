using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using ExolRebirth.NPCs.Exol;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace ExolRebirth.Projectiles.ExolOld
{
    public class ExolPortalTelegraph : ModProjectile
    {
        public override string Texture => "ExolRebirth/Projectiles/ExolOld/ExolDashPortal";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exol Portal effect");
            Main.projFrames[Projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 136;
            Projectile.height = 120;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 30;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * ((225 - Projectile.alpha) / 225f);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }
        public static int dir = 5;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || !NPC.AnyNPCs(ModContent.NPCType<NPCs.Exol.Exol>()))
            {
                Projectile.Kill();
            }
            Projectile.timeLeft = 2;
            Projectile.ai[1] += 2f * (float)Math.PI / 600f * dir;
            Projectile.ai[1] %= 2f * (float)Math.PI;
            Projectile.Center = player.Center + 350 * new Vector2((float)Math.Cos(Projectile.ai[1]), (float)Math.Sin(Projectile.ai[1]));

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 8)
                {
                    Projectile.frame = 0;
                }
            }
            {
                Lighting.AddLight(Projectile.Center, 0.25f, 0, 0.5f);
            }

        }
    }
    /*public class ExolPortalTelegraph2 : ModProjectile
	{
        public override string Texture => "ExolRebirth/Projectiles/ExolOld/ExolDashPortal";
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Exol Portal effect");
            Main.projFrames[Projectile.type] = 8;
		}

		public override Color? GetAlpha(Color lightColor) {
            if (Projectile.timeLeft <= 20) {
                return new Color(0, 0, 100) * ((105 - Projectile.alpha) / 105f);
            }
			return Color.White * ((225 - Projectile.alpha) / 225f);
		}
		public override void SetDefaults()
		{
			Projectile.width = 103;
			Projectile.height = 103;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
            Projectile.timeLeft = 70;
		}
		public override void AI()
        {
            Player player = Main.player[Projectile.owner];
			Projectile.Center = new Vector2(player.Center.X, player.Center.Y + 375);
			if (++Projectile.frameCounter >= 5) {
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 8) {
                    Projectile.frame = 0;
				}
			}	
        {
            Lighting.AddLight(Projectile.Center, 0.25f, 0, 0.5f);
        }

        }
	}
	public class ExolPortalTelegraph3 : ModProjectile
	{
        public override string Texture => "ExolRebirth/Projectiles/ExolOld/ExolDashPortal";
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Exol Portal effect");
            Main.projFrames[Projectile.type] = 8;
		}
		public override Color? GetAlpha(Color lightColor) {
            if (Projectile.timeLeft <= 20) {
                return new Color(0, 0, 100) * ((105 - Projectile.alpha) / 105f);
            }
			return Color.White * ((225 - Projectile.alpha) / 225f);
		}

		public override void SetDefaults()
		{
			Projectile.width = 103;
			Projectile.height = 103;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
            Projectile.timeLeft = 110;
		}
		public override void AI()
        {
            Player player = Main.player[Projectile.owner];
			Projectile.Center = new Vector2(player.Center.X - 375, player.Center.Y);
			if (++Projectile.frameCounter >= 5) {
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 8) {
                    Projectile.frame = 0;
				}
			}	
        {
            Lighting.AddLight(Projectile.Center, 0.25f, 0, 0.5f);
        }

        }
	}
	public class ExolPortalTelegraph4 : ModProjectile
	{
        public override string Texture => "ExolRebirth/Projectiles/ExolOld/ExolDashPortal";
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Exol Portal effect");
            Main.projFrames[Projectile.type] = 8;
		}

		public override Color? GetAlpha(Color lightColor) {
            if (Projectile.timeLeft <= 20) {
                return new Color(0, 0, 100) * ((105 - Projectile.alpha) / 105f);
            }
			return Color.White * ((225 - Projectile.alpha) / 225f);
		}
		public override void SetDefaults()
		{
			Projectile.width = 103;
			Projectile.height = 103;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
            Projectile.timeLeft = 150;
		}
		public override void AI()
        {
            Player player = Main.player[Projectile.owner];
			Projectile.Center = new Vector2(player.Center.X + 375, player.Center.Y);
			if (++Projectile.frameCounter >= 5) {
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 8) {
                    Projectile.frame = 0;
				}
			}	
        {
            Lighting.AddLight(Projectile.Center, 0.25f, 0, 0.5f);
        }

        }
	}*/
}