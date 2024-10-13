using EbonianMod.Common.Systems.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace EbonianMod.Items.Accessories
{
    public class TinyBrain : ModNPC //the class name is a reference to my brain.
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }
        public static Vector2 cen;
        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 40;
            NPC.damage = 0;
            NPC.defense = 5;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0;
            NPC.aiStyle = 0;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.lifeMax = 400;
            NPC.friendly = true;
            NPC.dontTakeDamage = false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hitinfo, int damage)
        {
            if (projectile.timeLeft > 2 && projectile.velocity != Vector2.Zero)
                projectile.Kill();
        }

        public override bool CheckDead()
        {
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/TinyBrainGore").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/TinyBrainGore2").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/TinyBrainGore3").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/TinyBrainGore4").Type, NPC.scale);
            return true;
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
        }
        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[NPC.target];
            verlet = new Verlet(player.Center, 8, 14, stiffness: 20);
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.timeLeft = 2;
            NPC.ai[0] += 4;
            switch (NPC.localAI[0])
            {
                case 0:
                    NPC.Center = player.Center + new Vector2(0, 60 + MathF.Sin(NPC.ai[0] * 0.05f) * 5).RotatedBy(MathHelper.ToRadians(NPC.ai[0]));
                    break;
                case 1:
                    NPC.Center = player.Center + new Vector2(0, -60 + MathF.Sin(NPC.ai[0] * 0.05f) * 5).RotatedBy(MathHelper.ToRadians(NPC.ai[0]));
                    break;
                case 2:
                    NPC.Center = player.Center + new Vector2(80 + MathF.Sin(NPC.ai[0] * 0.05f) * 5, 0).RotatedBy(MathHelper.ToRadians(NPC.ai[0]));
                    break;
                case 3:
                    NPC.Center = player.Center + new Vector2(-80 + MathF.Sin(NPC.ai[0] * 0.05f) * 5, 0).RotatedBy(MathHelper.ToRadians(NPC.ai[0]));
                    break;
                case 4:
                    NPC.Center = player.Center + new Vector2(60 + MathF.Sin(NPC.ai[0] * 0.05f) * 5, 40 + MathF.Sin(NPC.ai[0] * 0.05f) * 5).RotatedBy(MathHelper.ToRadians(NPC.ai[0]));
                    break;
                case 5:
                    NPC.Center = player.Center + new Vector2(-60 + MathF.Sin(NPC.ai[0] * 0.05f) * 5, 40 + MathF.Sin(NPC.ai[0] * 0.05f) * 5).RotatedBy(MathHelper.ToRadians(NPC.ai[0]));
                    break;
                case 6:
                    NPC.Center = player.Center + new Vector2(60 + MathF.Sin(NPC.ai[0] * 0.05f) * 5, -40 + MathF.Sin(NPC.ai[0] * 0.05f) * 5).RotatedBy(MathHelper.ToRadians(NPC.ai[0]));
                    break;
                case 7:
                    NPC.Center = player.Center + new Vector2(-60 + MathF.Sin(NPC.ai[0] * 0.05f) * 5, -40 + MathF.Sin(NPC.ai[0] * 0.05f) * 5).RotatedBy(MathHelper.ToRadians(NPC.ai[0]));
                    break;

            }
            EbonianPlayer modPlayer = player.GetModPlayer<EbonianPlayer>();
            if (!modPlayer.brainAcc)
            {
                NPC.life = 0;
            }
        }

        public override void FindFrame(int frameHeight)
        {

            NPC.frameCounter++;
            if (NPC.frameCounter < 10)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
            else if (NPC.frameCounter < 20)
            {
                NPC.frame.Y = 1 * frameHeight;
            }
            else if (NPC.frameCounter < 30)
            {
                NPC.frame.Y = 2 * frameHeight;
            }
            else if (NPC.frameCounter < 40)
            {
                NPC.frame.Y = 3 * frameHeight;
            }
            else
            {
                NPC.frameCounter = 0;
            }
        }
        Verlet verlet;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 pos, Color drawColor)
        {
            if (verlet != null)
            {
                verlet.Update(Main.player[NPC.target].Center, NPC.Center);
                verlet.Draw(spriteBatch, new VerletDrawData("Items/Accessories/BrainAcc_Chain"));
            }
            return true;
        }
    }
}