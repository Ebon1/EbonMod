using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using EbonianMod.NPCs.Garbage;
using Microsoft.Xna.Framework;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;

namespace EbonianMod.Items.Consumables.BossItems
{
    public class GarbageRemote : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dusty Quantum Computer");
            Tooltip.SetDefault("Allows communication with the only AI to ever reach singularity.");
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 12;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 1;
            Item.value = 1000000;
            Item.rare = ItemRarityID.Red;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().Register();
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<HotGarbage>()) && player.ownedProjectileCounts[ModContent.ProjectileType<GarbageRemoteP>()] <= 0;
        }

        public override bool? UseItem(Player player)
        {
            //NPC.NewNPCDirect(player.GetSource_FromThis(), player.Center + new Microsoft.Xna.Framework.Vector2(300, -200), ModContent.NPCType<HotGarbage>());
            Terraria.Audio.SoundEngine.PlaySound(new SoundStyle("EbonianMod/Sounds/GarbageSignal").WithVolumeScale(3), player.position);
            Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<GarbageRemoteP>(), 0, 0, player.whoAmI);
            return true;
        }
    }
    public class GarbageRemoteP : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.height = 20;
            Projectile.width = 20;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 350;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            NPC.NewNPCDirect(Projectile.GetSource_FromThis(), player.Center + new Microsoft.Xna.Framework.Vector2(300, -Main.screenHeight), ModContent.NPCType<HotGarbage>());
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;

            /*if (Projectile.ai[0] < 1 && Projectile.timeLeft < 400 && Projectile.timeLeft > 200)
                Projectile.ai[0] += 0.05f;
            else if (Projectile.ai[0] > 0 && Projectile.timeLeft < 200)
                Projectile.ai[0] -= 0.025f;*/

        }
    }
}
