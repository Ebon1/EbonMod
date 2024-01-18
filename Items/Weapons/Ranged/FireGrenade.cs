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
using EbonianMod.Projectiles.Friendly;
using EbonianMod.Projectiles.Exol;
using static Terraria.ModLoader.PlayerDrawLayer;
using System.IO;
using System.Collections.Generic;
using EbonianMod.Projectiles.VFXProjectiles;
using EbonianMod.Common.Systems;

namespace EbonianMod.Items.Weapons.Ranged
{
    public class FireGrenade : ModItem
    {
        public override void SetDefaults()
        {
            Item.Size = new Vector2(32);
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = 32;
            Item.useTime = 32;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.LightRed;
            Item.shootSpeed = 15f;
            Item.shoot = ModContent.ProjectileType<FireGrenadeP>();
        }
    }
    public class FireGrenadeP : ModProjectile
    {
        public override string Texture => "EbonianMod/Items/Weapons/Ranged/FireGrenade";
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(32);
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.aiStyle = 2;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[2] == 0)
            {
                Projectile.Center += oldVelocity;
                Projectile.damage = 0;
                Projectile.velocity = Vector2.Zero;
                Projectile.ai[2] = 1;
                Projectile.aiStyle = -1;
                if (Projectile.timeLeft > 80)
                    Projectile.timeLeft = 80;
                Projectile a = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Noise>(), 0, 0);
                a.timeLeft = 80;
                Projectile b = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<NoiseOverlay>(), 0, 0);
                b.timeLeft = 80;
            }
            return false;
        }
        public override bool? CanDamage() => Projectile.ai[2] == 0;
        public override void Kill(int timeLeft)
        {
            Helper.DustExplosion(Projectile.Center, Projectile.Size, 0, Color.OrangeRed);
            SoundEngine.PlaySound(EbonianSounds.genericExplosion, Projectile.Center);
            for (int i = 0; i < 25; i++)
            {
                Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(), Main.rand.Next(new int[] { GoreID.Smoke1, GoreID.Smoke2, GoreID.Smoke3 }), Main.rand.NextFloat(0.25f, 1f));
            }
            Projectile a = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FlameExplosion>(), (int)(Projectile.ai[2] + 1) * 100, 0, Projectile.owner);
            a.friendly = true;
            a.hostile = false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[1]);
            writer.Write(Projectile.localAI[0]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[1] = reader.ReadSingle();
            Projectile.localAI[0] = reader.ReadSingle();
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Orange);
            if (Projectile.ai[2] == 1 && Projectile.timeLeft % 3 == 0 && Projectile.timeLeft < 60 && Projectile.timeLeft > 20)
            {
                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && !npc.friendly && !npc.dontTakeDamage && npc.Center.Distance(Projectile.Center) < 150)
                        npc.StrikeNPC(new NPC.HitInfo() { Damage = 1, Crit = false, Knockback = 0 });
                }
            }
            if (Projectile.ai[2] == 1 && Projectile.timeLeft == 20)
                Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<QuickFlare>(), 0, 0);
        }
    }
}