using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;
using EbonianMod.Items.Weapons.Melee;
using Terraria.GameContent;
using Terraria.Audio;

namespace EbonianMod.Projectiles.Friendly
{
    public class WeakCursedBullet : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/EbonianGatlingBullet";
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 500;
            Projectile.Size = new(30, 10);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Reload(BlendState.Additive);
            Texture2D tex = Helper.GetExtraTexture("EbonianGatlingBullet");
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.LawnGreen, Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
        {
            target.AddBuff(BuffID.CursedInferno, 20);
        }
        public override void AI()
        {
            if (Projectile.timeLeft % 5 == 0)
                Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch);
            Projectile.velocity *= 1.025f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
    public class BloodBullet : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 500;
            Projectile.Size = new(30, 10);
        }
        /*public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Reload(BlendState.Additive);
            Texture2D tex = Helper.GetExtraTexture("EbonianGatlingBullet");
            for (int i = 0; i < 3; i++)
                spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.DarkRed, Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Reload(BlendState.AlphaBlend);
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Black, Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }*/
        public override void AI()
        {
            if (Projectile.timeLeft % 5 == 0)
                Dust.NewDustPerfect(Projectile.Center, DustID.Blood);
            Projectile.velocity *= 1.025f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
    public class IchorBlast : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/explosion";
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 500;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.Size = new(48, 48);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Reload(BlendState.Additive);
            Texture2D tex = Helper.GetExtraTexture("explosion");
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Gold, Projectile.rotation, tex.Size() / 2, Projectile.scale * 0.1f, SpriteEffects.None, 0);
            spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = new Vector2(Projectile.velocity.X, -oldVelocity.Y);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
        {
            target.AddBuff(BuffID.Ichor, 50);
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.Center.Distance(target.Center) < 600 && npc != target && !npc.friendly && !npc.buffImmune[BuffID.Ichor] && npc.realLife == -1)
                {
                    Helper.QuickDustLine(Projectile.Center, npc.Center, 100f, Color.Gold);
                    npc.AddBuff(BuffID.Ichor, 90);
                }
            }
        }
        public override void AI()
        {
            if (Projectile.timeLeft % 5 == 0)
                for (int i = 0; i < 5; i++)
                {
                    Vector2 offset = Main.rand.NextVector2CircularEdge(30, 30);
                    Dust.NewDustPerfect(Projectile.Center + offset, DustID.IchorTorch, Helper.FromAToB(Projectile.Center + offset, Projectile.Center));
                }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
}
