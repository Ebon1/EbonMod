using EbonianMod.Common.Systems.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.Crimson.BloodHunter
{
    public class BloodHunter : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.Size = new Vector2(94, 80);
            NPC.damage = 0;
            NPC.defense = 5;
            NPC.lifeMax = 400;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
        }
        public Vector2[] legOffsets = new Vector2[3];
        public Vector2 stingerTarget;
        public bool stingerTrailActive = false;
        public Verlet tail;
        public override void OnSpawn(IEntitySource source)
        {
            tail = new Verlet(NPC.Center, 20, 4, 0, true, true, 50, false);
            stingerTarget = NPC.Center + new Vector2(14 * NPC.direction, 32);
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);


            if (tail != null)
            {
                stingerTarget = Main.MouseWorld;
                tail.Update(NPC.Center - new Vector2(50 * -NPC.direction, 40), stingerTarget);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = TextureAssets.Npc[Type].Value;
            DrawBGLegs(spriteBatch, drawColor);

            SpriteEffects effect = NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(tex, NPC.Center - Main.screenPosition, null, drawColor, NPC.rotation, tex.Size() / 2, NPC.scale, effect, 0);

            DrawTail(spriteBatch, drawColor);

            DrawFGLegs(spriteBatch, drawColor);
            return false;
        }
        void DrawTail(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D tail0 = Helper.GetTexture(Texture + "_Tail0");
            Texture2D tail1 = Helper.GetTexture(Texture + "_Tail1");
            Texture2D tail2 = Helper.GetTexture(Texture + "_Tail2");
            Texture2D stinger = Helper.GetTexture(Texture + "_Stinger");
            if (tail != null)
            {
                float rot = Helper.FromAToB(tail.points[0].position, tail.points[1].position).ToRotation();
                spriteBatch.Draw(tail0, tail.points[0].position - Main.screenPosition, null, drawColor, rot, tail0.Size() / 2, NPC.scale, SpriteEffects.None, 0);

                rot = Helper.FromAToB(tail.points[1].position, tail.points[2].position).ToRotation();
                spriteBatch.Draw(tail1, tail.points[1].position - Main.screenPosition, null, drawColor, rot, tail1.Size() / 2, NPC.scale, SpriteEffects.None, 0);

                rot = Helper.FromAToB(tail.points[2].position, tail.points[3].position).ToRotation();
                spriteBatch.Draw(tail2, tail.points[2].position - Main.screenPosition, null, drawColor, rot, tail2.Size() / 2, NPC.scale, SpriteEffects.None, 0);

                rot = 0;
                spriteBatch.Draw(stinger, tail.points[3].position - Main.screenPosition, null, drawColor, rot, stinger.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            }
        }
        void DrawBGLegs(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D bgLeg0 = Helper.GetTexture(Texture + "_BGLeg0");
            Texture2D bgLeg1 = Helper.GetTexture(Texture + "_BGLeg1");
            Texture2D bgLeg2 = Helper.GetTexture(Texture + "_BGLeg2");
        }
        void DrawFGLegs(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D fgLeg0 = Helper.GetTexture(Texture + "_FGLeg0");
            Texture2D fgLeg1 = Helper.GetTexture(Texture + "_FGLeg1");
            Texture2D fgLeg2 = Helper.GetTexture(Texture + "_FGLeg2");
        }
    }
}
