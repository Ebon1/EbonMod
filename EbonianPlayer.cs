using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using System;
using static Terraria.ModLoader.PlayerDrawLayer;
using EbonianMod.Items.Accessories;
using Terraria.Graphics.Effects;
using EbonianMod.Items.Weapons.Melee;

//using EbonianMod.Common.Systems.Worldgen.Subworlds;
using EbonianMod.NPCs.Crimson.Fleshformator;
using EbonianMod.NPCs;
using EbonianMod.Projectiles;
using EbonianMod.Tiles;
using EbonianMod.NPCs.ArchmageX;
using EbonianMod.NPCs.Conglomerate;

namespace EbonianMod
{
    public class EbonianPlayer : ModPlayer
    {
        public int bossTextProgress, bossMaxProgress, dialogueMax, dialogueProg, timeSlowProgress, timeSlowMax, fleshformators;
        public string bossName;
        public string bossTitle;
        public string dialogue;
        public int bossStyle;
        public Color bossColor, dialogueColor;
        public static EbonianPlayer Instance;
        public Vector2 stabDirection;
        public int reiBoostCool, reiBoostT;
        public bool rolleg, brainAcc, heartAcc, ToxicGland, doomMinion, rei, reiV, sheep;
        public override void ResetEffects()
        {
            reiBoostCool--;
            if (reiBoostCool > 0)
                reiBoostT--;
            rei = false;
            reiV = false;
            rolleg = false;
            doomMinion = false;
            brainAcc = false;
            heartAcc = false;
            if (!NPC.AnyNPCs(ModContent.NPCType<Fleshformator>()))
                fleshformators = 0;
            ToxicGland = false;
            sheep = false;
        }

        public int platformWhoAmI = -1;
        public int platformTimer = 0;
        public int platformDropTimer = 0;

        public Projectile Platform => Main.projectile[platformWhoAmI];

        public override void PreUpdateMovement()
        {
            if (platformWhoAmI != -1)
                Player.position.X += Platform.velocity.X;
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<TinyBrain>()))
            {
                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && npc.type == ModContent.NPCType<TinyBrain>())
                    {
                        npc.life = 0;
                        npc.checkDead();
                    }
                }
            }
        }
        public override void PostUpdateRunSpeeds()
        {
            if (Player.HeldItem.type == ModContent.ItemType<EbonianScythe>() && !Player.ItemTimeIsZero)
            {
                Player.maxRunSpeed += 2;
                Player.accRunSpeed += 2;
            }
            if (rei)
            {
                Player.maxRunSpeed += 2.5f;
                Player.accRunSpeed += 2.5f;
            }
            if (sheep)
            {
                Player.wingTimeMax = -1;
                Player.wingTime = -1;
                Player.wingsLogic = -1;
                Player.wings = -1;
                Player.mount.Dismount(Player);
                Player.gravity = Player.defaultGravity;
                Player.maxRunSpeed = 4.2f;
                Player.accRunSpeed = 4.2f;
                Player.jumpSpeed = 5.31f;
                Player.jumpHeight = 23;
                Player.dashType = 0;
                Player.channel = false;
                Player.blockExtraJumps = true;
            }
        }
        public override bool CanStartExtraJump(ExtraJump jump)
        {
            if (sheep)
                return false;
            return base.CanStartExtraJump(jump);
        }
        public override bool CanUseItem(Item item)
        {
            if (sheep)
                return false;
            return base.CanUseItem(item);
        }
        public override void PostUpdateMiscEffects()
        {
            EbonianMod.sys.UpdateParticles();
            Player.ManageSpecialBiomeVisuals("EbonianMod:Conglomerate", NPC.AnyNPCs(ModContent.NPCType<Conglomerate>()));
            #region "hell stuff"
            Player.ManageSpecialBiomeVisuals("EbonianMod:HellTint", Player.ZoneUnderworldHeight);
            #endregion
        }

        public override void OnEnterWorld()
        {
            Instance = Player.GetModPlayer<EbonianPlayer>();
        }
        public int flashTime;
        public int flashMaxTime;
        public float flashStr;
        public Vector2 flashPosition;
        public void FlashScreen(Vector2 pos, int time, float str = 2f)
        {
            flashStr = str;
            flashMaxTime = time;
            flashTime = time;
            flashPosition = pos;
        }
        public override void UpdateLifeRegen()
        {
            if (brainAcc)
                Player.lifeRegen += 5;
        }
        public override void PostUpdateBuffs()
        {
            if (sheep)
            {
                Player.height = Player.width;
                Player.position.Y += Player.width + 2;
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ModContent.ProjectileType<player_sheep>())
                        proj.Center = Player.Bottom + new Vector2(0, -14);
                }
            }
        }
        public override void PostUpdate()
        {
            for (int i = (int)Player.Center.X / 16 - 3; i < (int)Player.Center.X / 16 + 3; i++)
            {
                for (int j = (int)Player.Center.Y / 16 - 3; j < (int)Player.Center.Y / 16 + 3; j++)
                {
                    if (Main.tile[i, j].TileType == (ushort)ModContent.TileType<ArchmageStaffTile>())
                    {
                        if (EbonianSystem.xareusFightCooldown <= 0)
                        {
                            Projectile.NewProjectile(null, new Vector2(i * 16, j * 16 - 100), Vector2.Zero, ModContent.ProjectileType<ArchmageXSpawnAnim>(), 0, 0);
                            break;
                        }
                    }
                }
            }
            if (flashTime > 0)
            {
                flashTime--;
                if (!Filters.Scene["EbonianMod:ScreenFlash"].IsActive())
                    Filters.Scene.Activate("EbonianMod:ScreenFlash", flashPosition);
                Filters.Scene["EbonianMod:ScreenFlash"].GetShader().UseProgress((float)Math.Sin((float)flashTime / flashMaxTime * Math.PI) * flashStr);
                Filters.Scene["EbonianMod:ScreenFlash"].GetShader().UseTargetPosition(flashPosition);
            }
            else
            {
                if (Filters.Scene["EbonianMod:ScreenFlash"].IsActive())
                    Filters.Scene["EbonianMod:ScreenFlash"].Deactivate();
            }
            if (bossTextProgress > 0)
                bossTextProgress--;
            if (bossTextProgress == 0)
            {
                bossName = null;
                bossTitle = null;
                bossStyle = -1;
                bossMaxProgress = 0;
                bossColor = Color.White;
            }
            /*if (timeSlowProgress > 0)
                timeSlowProgress -= EbonianMod.timeSkips + 1;
            if (timeSlowProgress <= 0)
            {
                timeSlowProgress = 0;
                EbonianMod.timeSkips = 0;
            }
            if (timeSlowProgress < EbonianMod.timeSkips && EbonianMod.timeSkips > 0)
                EbonianMod.timeSkips--;*/
            if (dialogueProg > 0)
                dialogueProg--;
            if (dialogueProg == 0)
            {
                dialogue = null;
                dialogueMax = 0;
                dialogueColor = Color.White;
            }
        }
    }
}
