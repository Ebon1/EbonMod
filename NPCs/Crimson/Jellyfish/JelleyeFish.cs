using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;

namespace EbonianMod.NPCs.Crimson.Jellyfish
{
    public class JelleyeFish : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 1000;
            NPC.damage = 0;
            NPC.noTileCollide = true;
            NPC.defense = 20;
            NPC.knockBackResist = 0;
            NPC.width = 82;
            NPC.height = 78;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.netAlways = true;
            NPC.value = Item.buyPrice(0, 20);
        }
        Verlet[] eyeVerlets = new Verlet[5];
        Verlet[] tentVerlets = new Verlet[5];
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < eyeVerlets.Length; i++)
            {
                eyeVerlets[i] = new Verlet(NPC.Center + new Vector2((i - 1) * 3, 0), 4, 10 + i * 6 + Main.rand.Next(5, 12), Main.rand.NextFloat(1, 5) * 2, lastPointLocked: false, stiffness: 150);
            }
            for (int i = 0; i < tentVerlets.Length; i++)
            {
                tentVerlets[i] = new Verlet(NPC.Center + new Vector2((i - 3) * 4, 0), 4, 25 + i * 3, Main.rand.NextFloat(1, 5), lastPointLocked: false, stiffness: 100);
            }
        }
        public override void AI()
        {
            NPC.TargetClosest(false);
            Player player = Main.player[NPC.target];
            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.Center.FromAToB(Main.MouseWorld) * 5, 0.2f);
            NPC.rotation = Helper.LerpAngle(NPC.rotation, NPC.velocity.ToRotation() + PiOver2, 0.2f);

            for (int i = 0; i < eyeVerlets.Length; i++)
                if (eyeVerlets[i] != null)
                {
                    eyeVerlets[i].Update(NPC.Center + new Vector2(MathF.Abs(i - 1) * 10, -4).RotatedBy(NPC.rotation - PiOver2), NPC.Center);
                }
            for (int i = 0; i < tentVerlets.Length; i++)
                if (tentVerlets[i] != null)
                {
                    tentVerlets[i].Update(NPC.Center + new Vector2((i - 2) * 8, -4).RotatedBy(NPC.rotation - PiOver2), NPC.Center);
                }

            for (int i = 0; i < eyeVerlets.Length; i++)
            {
                for (int j = 0; j < eyeVerlets.Length; j++)
                {
                    if (i == j) continue;
                    if (eyeVerlets[i].lastP.position.Distance(eyeVerlets[j].lastP.position) < 16)
                    {
                        Vector2 vel = Helper.FromAToB(eyeVerlets[i].lastP.position, eyeVerlets[j].lastP.position);
                        if (vel == Vector2.Zero)
                            vel = Vector2.UnitX;

                        Vector2 vel2 = Helper.FromAToB(eyeVerlets[j].lastP.position, eyeVerlets[i].lastP.position);
                        if (vel2 == Vector2.Zero)
                            vel2 = -Vector2.UnitX;
                        //eyeVerlets[i].lastP.position += vel;

                        //eyeVerlets[j].lastP.position += vel2;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            for (int i = 0; i < tentVerlets.Length; i++)
                if (tentVerlets[i] != null)
                    tentVerlets[i].Draw(spriteBatch, new VerletDrawData(Texture + "_Chain", null, Texture + "_Tip"));

            for (int i = 0; i < eyeVerlets.Length; i++)
                if (eyeVerlets[i] != null)
                    eyeVerlets[i].Draw(spriteBatch, new VerletDrawData(Texture + "_Chain", null, Texture + "_Eye"));

            return true;
        }
    }
}
