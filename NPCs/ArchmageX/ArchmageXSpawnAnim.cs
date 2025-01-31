using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using EbonianMod.Projectiles.ArchmageX;
using Terraria.Audio;
using EbonianMod.Common.Systems;
using Terraria.ID;
using Terraria.GameContent.ObjectInteractions;
using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework.Graphics;
using static System.Net.Mime.MediaTypeNames;

namespace EbonianMod.NPCs.ArchmageX
{
    public class ArchmageXSpawnAnim : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetStaticDefaults()
        {
            EbonianMod.projectileFinalDrawList.Add(Type);
        }
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(2, 2);
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 400;
        }
        Rectangle GetArenaRect()
        {
            Vector2 sCenter = Projectile.Center;
            float LLen = Helper.TRay.CastLength(sCenter, -Vector2.UnitX, 29f * 16);
            float RLen = Helper.TRay.CastLength(sCenter, Vector2.UnitX, 29f * 16);
            Vector2 U = Helper.TRay.Cast(sCenter, -Vector2.UnitY, 380);
            Vector2 D = Helper.TRay.Cast(Projectile.Center, Vector2.UnitY, 380);
            sCenter.Y = U.Y + Helper.FromAToB(U, D, false).Y * 0.5f;
            Vector2 L = sCenter;
            Vector2 R = sCenter;
            if (LLen > RLen)
            {
                R = Helper.TRay.Cast(sCenter, Vector2.UnitX, 29f * 16);
                L = Helper.TRay.Cast(R, -Vector2.UnitX, 34.5f * 32);
            }
            else
            {
                R = Helper.TRay.Cast(L, Vector2.UnitX, 34.5f * 32);
                L = Helper.TRay.Cast(sCenter, -Vector2.UnitX, 29f * 16);
            }
            Vector2 TopLeft = new Vector2(L.X, U.Y);
            Vector2 BottomRight = new Vector2(R.X, D.Y);
            Rectangle rect = new Rectangle((int)L.X, (int)U.Y, (int)Helper.FromAToB(TopLeft, BottomRight, false).X, (int)Helper.FromAToB(TopLeft, BottomRight, false).Y);
            return rect;
        }
        public override void OnKill(int timeLeft)
        {
            EbonianSystem.xareusFightCooldown = 500;
            for (int i = 0; i < 15; i++)
                Projectile.NewProjectile(null, Projectile.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(10, 20), ProjectileType<XAnimeSlash>(), 0, 0, -1, 0, Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(0.1f, 0.3f));

            NPC.NewNPCDirect(null, Projectile.Center, NPCType<ArchmageX>());
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (lightColor != Color.Transparent) return false;
            Texture2D tex3 = ExtraTextures.crosslight;
            Main.spriteBatch.Reload(BlendState.Additive);
            for (int i = 0; i < 4; i++)
            {
                Main.spriteBatch.Draw(tex3, Projectile.Center - Main.screenPosition, null, Color.White * (glareAlpha), 0, tex3.Size() / 2, (glareAlpha) * .6f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex3, Projectile.Center - Main.screenPosition, null, Color.White * (glareAlpha * 0.5f), 0, tex3.Size() / 2, (glareAlpha) * 1.6f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex3, Projectile.Center - Main.screenPosition, null, Color.White * (glareAlpha * 0.1f) * 10, 0, tex3.Size() / 2, (glareAlpha) * 2f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        float glareAlpha;
        public override void AI()
        {
            if (Projectile.timeLeft > 20)
                glareAlpha = MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(400, 0, Projectile.timeLeft));
            else
                glareAlpha = MathHelper.Lerp(glareAlpha, 0, 0.05f);
            Player player = Main.LocalPlayer;
            if (GetArenaRect().Size().Length() > 100)
            {
                if (player.Distance(GetArenaRect().Center()) > 1200)
                {
                    Helper.TPNoDust(GetArenaRect().Center(), player);
                }
                else
                {
                    while (player.Center.X < GetArenaRect().X)
                        player.Center += Vector2.UnitX * 2;

                    while (player.Center.X > GetArenaRect().X + GetArenaRect().Width)
                        player.Center -= Vector2.UnitX * 2;

                    while (player.Center.Y < GetArenaRect().Y)
                        player.Center += Vector2.UnitY * 2;
                }
            }

            Projectile.scale = MathHelper.Lerp(Projectile.scale, 4, 0.01f);
            int fac = 12;
            if (Projectile.timeLeft < 140)
                fac = 1;
            else if (Projectile.timeLeft < 205)
                fac = 3;
            else if (Projectile.timeLeft < 275)
                fac = 5;
            else if (Projectile.timeLeft < 300)
                fac = 7;
            else if (Projectile.timeLeft < 320)
                fac = 8;
            else if (Projectile.timeLeft < 360)
                fac = 9;
            if (Projectile.timeLeft % fac == 0 && Projectile.timeLeft > 35)
            {
                Projectile.NewProjectile(null, Projectile.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.5f, 2) * Projectile.scale, ProjectileType<XCloudVFXExtra>(), 0, 0);
            }


            if (fac < 9 && Projectile.timeLeft % fac == 0 && Projectile.timeLeft > 5)
                for (int i = 0; i < 2; i++)
                {
                    Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2, MathHelper.Clamp(10 - fac, 3, 10)) * (Projectile.scale * 0.5f);
                    Projectile.NewProjectile(null, Projectile.Center - vel * 2, vel, ProjectileType<XAnimeSlash>(), 0, 0, -1, 0, Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(0.1f, 0.3f));
                }


            if (Projectile.timeLeft < 300)
            {
                if (Projectile.timeLeft % 50 == 0)
                {
                    Vector2 pos = Projectile.Center + new Vector2(Main.rand.NextFloat(-280, 280), 0);
                    float off = Main.rand.NextFloat(-1, 1);
                    //Projectile.NewProjectile(null, pos, Vector2.UnitY.RotatedBy(off), ProjectileType<XLightningBolt>(), 0, 0, ai1: 1);
                    //Projectile.NewProjectile(null, pos, -Vector2.UnitY.RotatedBy(off), ProjectileType<XLightningBolt>(), 0, 0, ai1: 1);
                }
            }
            if (Projectile.timeLeft == 399)
            {
                SoundEngine.PlaySound(EbonianSounds.buildup, Projectile.Center);
                //EbonianSystem.ChangeCameraPos(Projectile.Center, 460, null, 2, InOutQuint);
            }
            if (Projectile.timeLeft == 299)
                EbonianSystem.ChangeZoom(120, new ZoomInfo(1.4f, 1.1f, InOutCirc, Rigid1, true));
            if (Projectile.timeLeft == 199)
                EbonianSystem.ChangeZoom(220, new ZoomInfo(2, 1.1f, InOutCirc, Rigid1, true, true));
            if (Projectile.timeLeft == 130)
                SoundEngine.PlaySound(EbonianSounds.BeamWindUp.WithPitchOffset(-0.5f), Projectile.Center);

            if (Projectile.timeLeft == 40)
            {
                EbonianSystem.xareusFightCooldown = 0;
                Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileType<ArchmageChargeUp>(), 0, 0);
            }
            if (Projectile.timeLeft == 25)
                EbonianSystem.ChangeZoom(40, new ZoomInfo(2, 1.1f, Rigid1, InOutSine, true));
        }
    }
    public class SpawnAnimMusic : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Event;
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/ambience");
        public override bool IsBiomeActive(Player player)
        {
            return player.ownedProjectileCounts[ProjectileType<ArchmageXSpawnAnim>()] > 0;
        }
    }
}
