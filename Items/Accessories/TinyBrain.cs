using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace EbonianMod.Items.Accessories
{
    public class TinyBrain : ModNPC //the class name is a reference to my brain.
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
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
            NPC.lifeMax = 50;
            NPC.friendly = true;
            NPC.dontTakeDamage = false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }
        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
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
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.timeLeft = 2;
            NPC.ai[0] += 1;
            if (++NPC.ai[1] >= 200 * (NPC.localAI[0] + 1))
            {
                player.Heal((int)(player.statLifeMax * 0.05f));
                Helper.QuickDustLine(NPC.Center, player.Center, 100, Color.Green);
                NPC.life = 0;
                NPC.checkDead();
            }
            switch (NPC.localAI[0])
            {
                case 0:
                    NPC.Center = player.Center + new Vector2(0, 60).RotatedBy(MathHelper.ToRadians(NPC.ai[0]));
                    break;
                case 1:
                    NPC.Center = player.Center + new Vector2(0, -60).RotatedBy(MathHelper.ToRadians(NPC.ai[0]));
                    break;
                case 2:
                    NPC.Center = player.Center + new Vector2(80, 0).RotatedBy(MathHelper.ToRadians(NPC.ai[0]));
                    break;
                case 3:
                    NPC.Center = player.Center + new Vector2(-80, 0).RotatedBy(MathHelper.ToRadians(NPC.ai[0]));
                    break;
                case 4:
                    NPC.Center = player.Center + new Vector2(60, 40).RotatedBy(MathHelper.ToRadians(NPC.ai[0]));
                    break;
                case 5:
                    NPC.Center = player.Center + new Vector2(-60, 40).RotatedBy(MathHelper.ToRadians(NPC.ai[0]));
                    break;
                case 6:
                    NPC.Center = player.Center + new Vector2(60, -40).RotatedBy(MathHelper.ToRadians(NPC.ai[0]));
                    break;
                case 7:
                    NPC.Center = player.Center + new Vector2(-60, -40).RotatedBy(MathHelper.ToRadians(NPC.ai[0]));
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
                while (distance > 4 && !float.IsNaN(distance))
                {
                    distToProj.Normalize();
                    distToProj *= 4;
                    center += distToProj;
                    distToProj = neckOrigin - center;
                    distance = distToProj.Length();

                    //Draw chain
                    spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Items/Accessories/BrainAcc_Chain").Value, center - pos,
                        new Rectangle(0, 0, 8, 4), Lighting.GetColor((int)center.X / 16, (int)center.Y / 16), projRotation,
                        new Vector2(8 * 0.5f, 4 * 0.5f), 1f, SpriteEffects.None, 0);
                }
            }
            return true;
        }
    }
}