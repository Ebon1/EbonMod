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
using EbonianMod.Projectiles.ArchmageX;
using EbonianMod.Buffs;


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
        public int reiBoostCool, reiBoostT, xTentCool, consistentTimer;
        public bool rolleg, brainAcc, heartAcc, ToxicGland, hotShield, rei, reiV, sheep, xTent;
        public bool doomMinion, xMinion, cClawMinion, titteringMinion;
        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (item.DamageType == DamageClass.Magic && xTent && xTentCool <= 0)
            {
                Projectile p = Projectile.NewProjectileDirect(source, position, Helper.FromAToB(position, Main.MouseWorld) * 8, ProjectileType<XAmethyst>(), 50, 0);
                p.DamageType = DamageClass.Magic;
                p.friendly = true;
                p.hostile = false;
                xTentCool = 60;
            }
            return base.Shoot(item, source, position, velocity, type, damage, knockback);
        }
        public override void ResetEffects()
        {

            if (NPC.AnyNPCs(NPCType<ArchmageX>()) || Player.ownedProjectileCounts[ProjectileType<ArchmageXSpawnAnim>()] > 0)
                Player.noBuilding = true;
            reiBoostCool--;
            xTentCool--;
            if (reiBoostCool > 0)
                reiBoostT--;
            rei = false;
            reiV = false;
            rolleg = false;
            hotShield = false;
            doomMinion = false;
            xMinion = false;
            titteringMinion = false;
            cClawMinion = false;
            brainAcc = false;
            xTent = false;
            heartAcc = false;
            if (!NPC.AnyNPCs(NPCType<Fleshformator>()))
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
            //if (platformWhoAmI != -1)
            //  Player.position.X += Platform.velocity.X;

        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (NPC.AnyNPCs(NPCType<TinyBrain>()))
            {
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.active && npc.type == NPCType<TinyBrain>())
                    {
                        npc.life = 0;
                        npc.checkDead();
                    }
                }
            }
            if (Player.HasBuff<Sheepened>())
            {
                modifiers.DisableSound();
                SoundEngine.PlaySound(SoundID.NPCHit1, Player.Center);
            }
        }
        public override void PostUpdateRunSpeeds()
        {
            if (NPC.AnyNPCs(NPCType<ArchmageX>()) || Player.ownedProjectileCounts[ProjectileType<ArchmageXSpawnAnim>()] > 0)
                Player.noBuilding = true;
            if (Player.HeldItem.type == ItemType<EbonianScythe>() && !Player.ItemTimeIsZero)
            {
                Player.maxRunSpeed += 2;
                Player.accRunSpeed += 2;
            }
            if (rei)
            {
                Player.maxRunSpeed += .5f;
                Player.accRunSpeed += .5f;
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
                Player.jumpSpeed = 6.1f;
                Player.jumpHeight = 26;
                Player.dashType = 0;
                Player.channel = false;
                Player.blockExtraJumps = true;
            }
            if (hotShield)
                Player.CancelAllBootRunVisualEffects();
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
            consistentTimer++;
            if (hotShield)
            {
                Player.CancelAllBootRunVisualEffects();
                Player.accRunSpeed *= 0.7f;
                Player.moveSpeed *= 0.7f;
            }
            EbonianMod.sys.UpdateParticles();
            Player.ManageSpecialBiomeVisuals("EbonianMod:XMartian", NPC.AnyNPCs(NPCType<ArchmageCutsceneMartian>()));
            //Player.ManageSpecialBiomeVisuals("EbonianMod:Conglomerate", NPC.AnyNPCs(NPCType<Conglomerate>()));
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

            if (NPC.AnyNPCs(NPCType<ArchmageX>()) || Player.ownedProjectileCounts[ProjectileType<ArchmageXSpawnAnim>()] > 0)
                Player.noBuilding = true;
            if (sheep)
            {
                Player.height = Player.width;
                Player.position.Y += Player.width + 2;
                foreach (Projectile proj in Main.ActiveProjectiles)
                {
                    if (proj.active && proj.type == ProjectileType<player_sheep>())
                    {
                        proj.Center = Player.Bottom + new Vector2(0, -14);
                        if (Player.velocity.Y < 0)
                            Collision.StepUp(ref proj.position, ref Player.velocity, proj.width, proj.height, ref proj.stepSpeed, ref proj.gfxOffY);
                        else
                            Collision.StepDown(ref proj.position, ref Player.velocity, proj.width, proj.height, ref proj.stepSpeed, ref proj.gfxOffY);

                    }
                }
            }
        }
        public override void PostUpdate()
        {
            if (hotShield)
            {
                Player.CancelAllBootRunVisualEffects();
            }
            if (NPC.AnyNPCs(NPCType<ArchmageX>()) || Player.ownedProjectileCounts[ProjectileType<ArchmageXSpawnAnim>()] > 0)
                Player.noBuilding = true;
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
