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
            NPC.NewNPCDirect(null, Projectile.Center, ModContent.NPCType<ArchmageX>());
        }
        public override void AI()
        {
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

            EbonianSystem.xareusFightCooldown = 500;
            Projectile.scale = MathHelper.Lerp(Projectile.scale, 4, 0.01f);
            int fac = 12;
            if (Projectile.timeLeft < 175)
                fac = 1;
            else if (Projectile.timeLeft < 225)
                fac = 3;
            else if (Projectile.timeLeft < 275)
                fac = 5;
            else if (Projectile.timeLeft < 300)
                fac = 7;
            else if (Projectile.timeLeft < 360)
                fac = 9;
            if (Projectile.timeLeft % fac == 0)
            {
                Projectile.NewProjectile(null, Projectile.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(2.5f, 4) * Projectile.scale, ModContent.ProjectileType<XCloudVFXExtra>(), 0, 0);
            }

            if (Projectile.timeLeft < 300)
            {
                if (Projectile.timeLeft % 50 == 0)
                {
                    Vector2 pos = Projectile.Center + new Vector2(Main.rand.NextFloat(-280, 280), 0);
                    SoundEngine.PlaySound(EbonianSounds.xSpirit.WithPitchOffset(-1f), pos);
                    float off = Main.rand.NextFloat(-1, 1);
                    Projectile.NewProjectile(null, pos, Vector2.UnitY.RotatedBy(off), ModContent.ProjectileType<XLightningBolt>(), 0, 0);
                    Projectile.NewProjectile(null, pos, -Vector2.UnitY.RotatedBy(off), ModContent.ProjectileType<XLightningBolt>(), 0, 0);
                }
            }
            if (Projectile.timeLeft == 399)
                EbonianSystem.ChangeCameraPos(Projectile.Center, 120, 1f);
            if (Projectile.timeLeft == 299)
                EbonianSystem.ChangeCameraPos(Projectile.Center, 120, 1.3f);
            if (Projectile.timeLeft == 199)
                EbonianSystem.ChangeCameraPos(Projectile.Center, 120, 1.6f);
            if (Projectile.timeLeft == 99)
                EbonianSystem.ChangeCameraPos(Projectile.Center, 100, 2f);
            if (Projectile.timeLeft == 130)
                SoundEngine.PlaySound(EbonianSounds.BeamWindUp.WithPitchOffset(-0.5f), Projectile.Center);
        }
    }
    public class SpawnAnimMusic : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Event;
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/ambience");
        public override bool IsBiomeActive(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<ArchmageXSpawnAnim>()] > 0;
        }
    }
}
