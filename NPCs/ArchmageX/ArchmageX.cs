using EbonianMod.Common.Systems.Misc.Dialogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.ArchmageX
{
    public class ArchmageX : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.Size = new Vector2(50, 78);
            NPC.lifeMax = 4500;
            NPC.defense = 5;
            NPC.damage = 0;
            NPC.boss = true;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0;
            NPC.lavaImmune = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Helper.GetTexture(Texture);
            Texture2D arms = Helper.GetTexture(Texture + "_Arms");
            Texture2D head = Helper.GetTexture(Texture + "_Head");
            spriteBatch.Draw(arms, NPC.Center - screenPos, null, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(head, NPC.Center + new Vector2(NPC.direction == -1 ? 6 : 12, -38).RotatedBy(NPC.rotation) - screenPos, headFrame, drawColor, headRotation, new Vector2(36, 42) / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
        float headRotation;
        Rectangle headFrame = new Rectangle(0, 0, 36, 42);
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
        public float Mana
        {
            get => NPC.ai[3];
            set => NPC.ai[3] = value;
        }
        public const int Despawn = -3, Death = -2, Idle = -1, Spawn = 0, PhantasmalSpirit = 1, ShadowflamePuddles = 2, SpectralOrbs = 3, MagnificentFireballs = 4, AmethystStorm = 5, HelicopterBlades = 6, AmethystBulletHell = 7, GiantAmethyst = 8, Micolash = 9, PhantasmalBlast = 10, AmethystCloseIn = 11, ExplosionAtFeet = 12, ShadowflameRift = 13, TheSheepening = 14;
        public void FacePlayer()
        {
            Player player = Main.player[NPC.target];
            NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
            NPC.spriteDirection = NPC.direction;
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (NPC.HasValidTarget)
                {
                    AITimer = 0;
                }
                if (!player.active || player.dead)
                {
                    AIState = Despawn;
                }
            }
            if (AIState != Spawn)
                AITimer++;
            switch (AIState)
            {
                case Despawn:
                    {

                    }
                    break;
                case Death:
                    {

                    }
                    break;
                case Spawn:
                    {
                        if (Helper.TRay.CastLength(NPC.Center, Vector2.UnitY, 70, true) < 70)
                            AITimer++;
                        FacePlayer();
                        if (AITimer == 10)
                        {
                            DialogueSystem.NewDialogueBox(150, NPC.Center - new Vector2(0, 80), "Dialogue.", Color.Purple, -1, 0.6f, default, 2.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite);
                        }
                    }
                    break;
                case Idle:
                    {

                    }
                    break;
            }
        }
    }
}
