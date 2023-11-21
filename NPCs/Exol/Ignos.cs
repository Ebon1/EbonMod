using EbonianMod.Projectiles;
using EbonianMod;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Terraria.DataStructures;
using EbonianMod.Projectiles.VFXProjectiles;
using EbonianMod.Dusts;
using EbonianMod.Projectiles.Exol;
using System.IO;

namespace EbonianMod.NPCs.Exol
{
    public class Ignos : ModNPC
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
                new FlavorTextBestiaryInfoElement("Type: Soul"),
                new FlavorTextBestiaryInfoElement("One of the greatest warriors known, and one of the few able to go toe to toe with Inferos. Even in death, his very soul yearns for a battle as grand as that of Inferos, if not grander, before he can finally rest."),
            });
        }
        public override void SetDefaults()
        {
            NPC.width = 134;
            NPC.height = 148;
            NPC.damage = 0;
            NPC.defense = 30;
            NPC.lifeMax = 35000;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Ignos");
            }
        }
        public float AIState { get => NPC.ai[0]; set => NPC.ai[0] = value; }
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
        public float AITimer3
        {
            get => NPC.ai[3];
            set => NPC.ai[3] = value;
        }
        const int Spawn = 0, SwordSlashes = 1, SwordSlashRain = 2, HoldSwordUpAndChannelSoulVortex = 3, StabSwordAndThrowRock = 4;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            PlayerDetection();
            IdleMovement();
            AITimer++;
            switch (AIState)
            {
                case Spawn:
                    {
                        Reset();
                    }
                    break;
            }
        }
        float rot;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Helper.GetExtraTexture("vortex2");
            spriteBatch.Reload(BlendState.Additive);
            rot -= 0.025f;
            Vector2 scale = new Vector2(1f, 0.25f);
            spriteBatch.Reload(EbonianMod.SpriteRotation);
            EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(rot);
            EbonianMod.SpriteRotation.Parameters["scale"].SetValue(scale * 0.5f);
            spriteBatch.Draw(tex, Main.MouseScreen, null, Color.White, 0, tex.Size() / 2, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(tex, Main.MouseScreen, null, Color.White, 0, tex.Size() / 2, 1, SpriteEffects.None, 0);
            spriteBatch.Reload(BlendState.AlphaBlend);
            spriteBatch.Reload(effect: null);
            return true;
        }
        void Reset()
        {
            NPC.noTileCollide = true;
            NPC.rotation = 0;
            NPC.velocity.X = 0;
            NPC.velocity.Y = 0;
            AITimer = 0;
            AITimer2 = 0;
            AITimer3 = 0;
            NPC.damage = 0;
        }
        void IdleMovement(bool over = false)
        {
            Player player = Main.player[NPC.target];
            //NPC.velocity *= 0.975f;
            NPC.velocity = Vector2.Clamp(Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 150), false) * 0.1f, -Vector2.One * 10, Vector2.One * 10);
        }
        void PlayerDetection()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            if (player.dead || !player.active || !player.ZoneUnderworldHeight)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (NPC.HasValidTarget)
                {
                    AIState = Spawn;
                    AITimer = 0;
                }
                if (player.dead || !player.active || !player.ZoneUnderworldHeight)
                {
                    NPC.velocity.Y = 30;
                    NPC.timeLeft = 10;
                    NPC.active = false;
                }
                return;
            }
        }
    }
}
