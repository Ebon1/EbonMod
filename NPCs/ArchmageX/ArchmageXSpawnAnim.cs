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
            Projectile.timeLeft = 300;
        }
        public override void OnKill(int timeLeft)
        {
            NPC.NewNPCDirect(null, Projectile.Center, ModContent.NPCType<ArchmageX>());
        }
        public override void AI()
        {
            Projectile.scale = MathHelper.Lerp(Projectile.scale, 4, 0.01f);
            int fac = 10;
            if (Projectile.timeLeft < 50)
                fac = 1;
            else if (Projectile.timeLeft < 75)
                fac = 3;
            else if (Projectile.timeLeft < 125)
                fac = 4;
            else if (Projectile.timeLeft < 150)
                fac = 5;
            else if (Projectile.timeLeft < 175)
                fac = 7;
            if (Projectile.timeLeft % fac == 0)
            {
                Projectile.NewProjectile(null, Projectile.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(2.5f, 4) * Projectile.scale, ModContent.ProjectileType<XCloudVFXExtra>(), 0, 0);
            }
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
