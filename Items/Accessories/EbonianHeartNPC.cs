using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using EbonianMod.Projectiles.Terrortoma;
using EbonianMod.Common.Systems.Misc;
using EbonianMod.Projectiles.VFXProjectiles;
using Terraria.GameContent;
using System.Collections.Generic;

namespace EbonianMod.Items.Accessories
{
    public class EbonianHeartNPC : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 40;
            NPC.damage = 0;
            NPC.defense = 132;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0;
            NPC.npcSlots = 0;
            NPC.aiStyle = 0;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.lifeMax = 69420;
            NPC.dontTakeDamage = true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 pos, Color drawColor)
        {
            Player player = Main.player[NPC.target];
            spriteBatch.Draw(Request<Texture2D>("EbonianMod/Items/Accessories/EbonianHeartNPC").Value, NPC.Center - pos,
                        NPC.frame, drawColor, NPC.rotation,
                        new Vector2(40 * 0.5f, 40 * 0.5f), 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Items/Accessories/EbonianHeartNPC_Glow").Value, NPC.Center - pos,
                        NPC.frame, Color.White, NPC.rotation,
                        new Vector2(40 * 0.5f, 40 * 0.5f), 1f, SpriteEffects.None, 0);
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.netUpdate = true;
            NPC.TargetClosest(true);

            Vector2 pos = player.Center + new Vector2(player.direction == 1 ? -40 : 40, -80);
            NPC.Center = Vector2.Lerp(NPC.Center, pos, 0.2f);
            if (++NPC.ai[0] % 40 == 0 && NPC.ai[0] > 0)
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.active && !npc.friendly && !npc.dontTakeDamage && npc.Center.Distance(NPC.Center) < 1000 && npc.type != NPCID.TargetDummy)
                    {
                        NPC.ai[1]++;
                        if (NPC.ai[1] > 6)
                        {
                            NPC.ai[0] = -450;
                            break;
                        }
                        Projectile p = Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Helper.FromAToB(NPC.Center, npc.Center) * 10, ProjectileType<EHeartP>(), 26, 0, player.whoAmI);
                        p.friendly = true;
                        p.hostile = false;
                        break;
                    }
                }
            NPC.ai[1] = 0;

            EbonianPlayer modPlayer = player.GetModPlayer<EbonianPlayer>();
            if (!modPlayer.heartAcc || player.dead || !player.active)
            {
                NPC.life = 0;
            }
        }

        public override void FindFrame(int frameHeight)
        {

            NPC.frameCounter++;
            if (NPC.frameCounter < 5)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
            else if (NPC.frameCounter < 10)
            {
                NPC.frame.Y = 1 * frameHeight;
            }
            else if (NPC.frameCounter < 15)
            {
                NPC.frame.Y = 2 * frameHeight;
            }
            else
            {
                NPC.frameCounter = 0;
            }
        }
        Verlet verlet;
        public override void OnSpawn(IEntitySource source)
        {
            verlet = new(NPC.Center, 10, 10, 1, true, true, 10);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 pos, Color drawColor)
        {
            if (NPC.ai[0] != -1)
            {
                Player player = Main.player[NPC.target];

                Vector2 neckOrigin = new Vector2(player.Center.X, player.Center.Y);
                Vector2 center = NPC.Center;
                Vector2 distToProj = neckOrigin - NPC.Center;
                float projRotation = distToProj.ToRotation() - 1.57f;
                float distance = distToProj.Length();
                /*while (distance > 6 && !float.IsNaN(distance))
                {
                    distToProj.Normalize();
                    distToProj *= 6;
                    center += distToProj;
                    distToProj = neckOrigin - center;
                    distance = distToProj.Length();

                    //Draw chain
                    spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Items/Accessories/HeartChain").Value, center - pos,
                        null, Lighting.GetColor((int)center.X / 16, (int)center.Y / 16), projRotation,
                        new Vector2(12 * 0.5f, 6 * 0.5f), 1f, SpriteEffects.None, 0);
                }*/
                if (verlet != null)
                {
                    verlet.Update(NPC.Center, player.Center);
                    verlet.Draw(spriteBatch, "Items/Accessories/HeartChain");
                }
            }
            return false;
        }
    }
    public class EHeartP : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 50;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        float vfxOffset;
        public override bool PreDraw(ref Color lightColor)
        {
            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            Main.spriteBatch.Reload(BlendState.Additive);
            float alpha = 1f;
            vfxOffset -= 0.015f;
            if (vfxOffset <= 0)
                vfxOffset = 1;
            vfxOffset = MathHelper.Clamp(vfxOffset, float.Epsilon, 1 - float.Epsilon);
            List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
            float s = 0;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float mult = (1f - fadeMult * i);

                if (mult < 0.5f)
                    s = MathHelper.Clamp(mult * 3.5f, 0, 0.5f) * 3;
                else
                    s = MathHelper.Clamp((-mult + 1) * 2, 0, 0.5f) * 3;

                if (i > 0 && Projectile.oldPos[i] != Vector2.Zero)
                {
                    Color col = Color.LawnGreen * mult * 2 * s;

                    float __off = vfxOffset;
                    if (__off > 1) __off = -__off + 1;
                    float _off = __off + mult;
                    vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(30 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() + MathHelper.PiOver2), col, new Vector2(_off, 0)));
                    vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(30 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() - MathHelper.PiOver2), col, new Vector2(_off, 1)));
                }
            }
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (vertices.Count > 2)
            {
                Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, ExtraTextures.FlamesSeamless, false);
            }
            Main.spriteBatch.ApplySaved();
            //Main.spriteBatch.Draw(TextureAssets.Projectile[ProjectileType<Gibs>()].Value, Projectile.Center - Main.screenPosition, null, Color.LawnGreen * alpha * 3, 0, TextureAssets.Projectile[ProjectileType<Gibs>()].Value.Size() / 2, 0.05f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hit)
        {
            target.AddBuff(BuffID.CursedInferno, Main.rand.Next(20, 100));
        }
        public override string Texture => "EbonianMod/Extras/Empty";
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(96);
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 350;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.extraUpdates = 2;
            AIType = 96;
        }

        public override void PostAI()
        {
            Helper.DustExplosion(Projectile.Center, Projectile.Size, 0, Color.LawnGreen, false, false, 0.04f);
        }
    }
}