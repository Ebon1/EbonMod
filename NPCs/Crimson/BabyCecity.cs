using EbonianMod.Misc;
using EbonianMod.NPCs.Corruption;
using EbonianMod.Projectiles.Terrortoma;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.Crimson
{
    public class BabyCecity : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.width = 68;
            NPC.height = 76;
            NPC.damage = 15;
            NPC.defense = 2;
            NPC.lifeMax = 55;
            NPC.HitSound = new SoundStyle("EbonianMod/Sounds/NPCHit/fleshHit");
            NPC.value = 60f;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.buffImmune[BuffID.Ichor] = true;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                new FlavorTextBestiaryInfoElement("Type: Infected Creature"),
                new FlavorTextBestiaryInfoElement("Crimorrhages appear to be an evolution of an Ichor Sticker that had its limbs overtaken by the spread of the biome. They appear to have once had more, but lost them to travelers and other Crimson beasts. Their sporadic movement and violent calls suggests something resembling anger due to their current state."),
            });
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
        }
        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter % 5 == 0)
            {
                if (NPC.frame.Y < frameHeight * 5)
                    NPC.frame.Y += frameHeight;
                else
                    NPC.frame.Y = 0;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!NPC.IsABestiaryIconDummy && verlet[0] != null)
                for (int i = 0; i < 2; i++)
                {
                    if (!Main.gamePaused)
                        verlet[i].Update(NPC.Center, ogPos[i]);
                    verlet[i].Draw(spriteBatch, "NPCs/Crimson/BabyCecity_Hook0", endTex: "NPCs/Crimson/BabyCecity_Hook2");
                }
            return true;
        }
        public float AIState
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }
        public float AITimer
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }
        public float AITimer2
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }
        Verlet[] verlet = new Verlet[2];
        Vector2[] dir = new Vector2[2];
        Vector2[] ogPos = new Vector2[2];
        //float[] len = new float[3];
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 2; i++)
            {
                verlet[i] = new Verlet(NPC.Center, 20, 22, gravity: 0.2f, lastPointLocked: true, stiffness: 30);
                dir[i] = -Helper.CircleDividedEqually(i + 1, 6).ToRotationVector2();
                ogPos[i] = Helper.TRay.Cast(NPC.Center, dir[i], 350);
            }
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit(), ModContent.Find<ModGore>("EbonianMod/Crimorrhage" + i).Type);
                }
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < verlet[i].segments.Count; j++)
                        Gore.NewGore(NPC.GetSource_Death(), verlet[i].segments[j].pointA.position, Main.rand.NextVector2Unit(), ModContent.Find<ModGore>("EbonianMod/CrimorrhageChain").Type);
                }
            }
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            NPC.direction = NPC.velocity.X < 0 ? -1 : 1;
            if (verlet[0] == null)
                return;
            NPC.rotation = Helper.FromAToB(NPC.Center, player.Center).ToRotation() - MathHelper.PiOver2;
            switch (AIState)
            {
                case 0:
                    if (player.Center.Distance(NPC.Center) < 1300)
                        AITimer++;
                    if (NPC.Center.Distance(verlet[0].lastP.position) < 370 && NPC.Center.Distance(verlet[1].lastP.position) < 370)
                    {
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center) * 15, 0.01f);
                    }
                    else NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, verlet[0].lastP.position) * 15, 0.01f);
                    if (AITimer >= 350)
                    {
                        AIState++;
                        AITimer = 0;
                    }
                    break;
                case 1:
                    AITimer++;
                    if (AITimer > 45)
                        AITimer2++;
                    NPC.velocity *= 0.9f;
                    if (AITimer2 == 10)
                    {
                        SoundStyle sound = new SoundStyle("EbonianMod/Sounds/bloodSpit");
                        SoundEngine.PlaySound(sound, NPC.Center);
                    }
                    if (AITimer2 == 30)
                    {
                        Vector2 vel = Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(0.2f);
                        for (int i = 0; i < 15; i++)
                        {
                            Dust.NewDustDirect(NPC.Center, NPC.width / 2, NPC.height / 2, DustID.IchorTorch, vel.X * Main.rand.NextFloat(5, 8), vel.Y * Main.rand.NextFloat(5, 8));
                        }
                        Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, vel * 7, ProjectileID.GoldenShowerHostile, 20, 0);
                        a.friendly = false;
                        a.hostile = true;

                    }
                    if (AITimer2 >= 60)
                    {
                        AITimer2 = 0;
                    }
                    if (AITimer >= 160)
                    {
                        AIState = 0;
                        AITimer = 0;
                    }
                    break;
            }
        }
    }
}
