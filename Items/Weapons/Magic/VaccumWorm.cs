using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Utilities;
using EbonianMod.Dusts;
using Terraria.Audio;
using EbonianMod.Common.Systems;
using ReLogic.Utilities;
using EbonianMod.Items.Materials;

namespace EbonianMod.Items.Weapons.Magic
{
    public class VaccumWorm : ModItem
    {
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Magic;
            Item.damage = 1;
            Item.useTime = 1;
            Item.mana = 1;
            Item.useAnimation = 10;
            Item.shoot = ModContent.ProjectileType<VaccumWormP>();
            Item.shootSpeed = 1f;
            Item.rare = ItemRarityID.Green;
            Item.useStyle = 5;
            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.ShadowScale, 20).AddIngredient(ItemID.WormTooth, 5).AddTile(TileID.Anvils).Register();
        }
        public override bool? CanAutoReuseItem(Player player) => false;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            velocity.Normalize();
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 350);
            return false;
        }
    }
    public class VaccumWormP : ModProjectile
    {
        public override string Texture => "EbonianMod/Items/Weapons/Magic/VaccumWorm";
        float holdOffset = 20;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.Size = new Vector2(36, 44);
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
        }
        public override bool? CanDamage() => false;
        float[] windAlpha = new float[12];
        SlotId slot;
        public override void OnSpawn(IEntitySource source)
        {
            slot = SoundEngine.PlaySound(EbonianSounds.vaccum);
        }
        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(slot, out var sound))
            {
                sound.Stop();
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems || !player.channel)
            {
                Projectile.Kill();
                return;
            }
            if (player.itemTime < 2)
            {
                player.itemTime = 2;
                player.itemAnimation = 2;
            }
            Projectile.timeLeft = 10;
            Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
            Vector2 pos = player.RotatedRelativePoint(player.MountedCenter);
            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            player.itemRotation = (Projectile.velocity.ToRotation() + Projectile.ai[0]) * player.direction;
            pos += (Projectile.velocity.ToRotation()).ToRotationVector2() * holdOffset;
            Projectile.Center = pos;
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - MathHelper.PiOver2);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(player.Center, Main.MouseWorld), 0.1f - MathHelper.Lerp(0.09f, 0f, Projectile.ai[0] / 350)).SafeNormalize(Vector2.UnitX);

            List<NPC> npcs = new List<NPC>();
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.CanBeChasedBy(Projectile) && !npc.boss && !NPCID.Sets.ShouldBeCountedAsBoss[npc.type])
                {
                    if (npc.Center.Distance(player.Center) < 330)
                    {
                        npcs.Add(npc);
                    }
                }
            }
            if (npcs.Any() && Main.rand.NextBool())
            {
                npcs = npcs.OrderBy(x => x.Distance(player.Center)).ToList();
                for (int i = 0; i < npcs.Count; i++)
                {
                    if (!npcs[i].active || npcs[i] == null)
                        continue;
                    if (npcs[i].knockBackResist > 0)
                    {
                        npcs[i].velocity = Helper.FromAToB(npcs[i].Center, Projectile.Center + Projectile.velocity * 20, false) / 10 * npcs[i].knockBackResist;
                    }
                    npcs[i].StrikeNPC(new NPC.HitInfo()
                    {
                        Damage = 1,
                        Crit = false
                    });
                    if (Projectile.ai[2]++ % 2 == 0)
                        player.CheckMana(1, true);

                    Vector2 _pos = npcs[i].Center + Main.rand.NextVector2Circular(npcs[i].width / 2, npcs[i].height / 2);
                    if (Main.rand.NextBool(4))
                    {
                        Dust d = Dust.NewDustPerfect(_pos, ModContent.DustType<LineDustFollowPoint>(), Helper.FromAToB(_pos, Projectile.Center) * Main.rand.NextFloat(3, 7), 0, Color.LawnGreen, Main.rand.NextFloat(0.06f, .2f));
                        d.noGravity = true;
                        d.customData = Projectile.Center + Projectile.velocity * 20;
                    }
                    break;
                }
            }


            Vector2 d_pos = Projectile.Center + Projectile.velocity * 20 + (Projectile.velocity * Main.rand.NextFloat(30, 100)).RotatedByRandom(MathHelper.PiOver2);
            if (Main.rand.NextBool(4))
            {
                Dust d = Dust.NewDustPerfect(d_pos, ModContent.DustType<LineDustFollowPoint>(), Helper.FromAToB(d_pos, Projectile.Center) * Main.rand.NextFloat(3, 7), 0, Color.LawnGreen, Main.rand.NextFloat(0.06f, .2f));
                d.noGravity = true;
                d.customData = Projectile.Center + Projectile.velocity * 20;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D[] texture = new Texture2D[] { Helper.GetExtraTexture("Extras2/slash_06"), Helper.GetExtraTexture("Extras2/twirl_01"), Helper.GetExtraTexture("Extras2/twirl_02"), Helper.GetExtraTexture("Extras2/twirl_03"), };
            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Reload(EbonianMod.SpriteRotation);
            for (int i = 0; i < windAlpha.Length; i++)
            {
                UnifiedRandom rand = new UnifiedRandom(804914729 + i);
                windAlpha[i] = MathHelper.Lerp(windAlpha[i], -0.05f, rand.NextFloat(0.01f, 0.2f));
                if (windAlpha[i] <= 0.01f)
                    windAlpha[i] = Main.rand.NextFloat(0.6f, 1.1f);
                Vector2 scale = new Vector2(1, 0.25f) * 0.5f;
                EbonianMod.SpriteRotation.Parameters["scale"].SetValue(scale * 0.75f);
                EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(-Main.GameUpdateCount * rand.NextFloat(0.03f, 0.05f) * (i + 1));
                Vector4 col = (Color.Lerp(Color.PaleGreen, Color.Lime, windAlpha[i])).ToVector4();
                EbonianMod.SpriteRotation.Parameters["uColor"].SetValue(col);
                Texture2D tex = texture[rand.Next(texture.Length)];
                Main.spriteBatch.Draw(tex, Projectile.Center + Main.rand.NextVector2Circular(3, 3) + Projectile.velocity * (20 + (rand.NextFloat(65) + (windAlpha[i] * 30)) * windAlpha[i]) - Main.screenPosition, null, Color.White * 0.5f * windAlpha[i], Projectile.velocity.ToRotation() + MathHelper.PiOver2, tex.Size() / 2, windAlpha[i] * rand.NextFloat(0.9f, 1f), SpriteEffects.FlipVertically, 0);
            }
            Main.spriteBatch.Reload(effect: null);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return true;
        }
    }
}
