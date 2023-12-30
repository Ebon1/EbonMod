﻿using EbonianMod.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.Crimson
{
    public class MassiveSpectator : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 1000;
            NPC.damage = 0;
            NPC.noTileCollide = true;
            NPC.defense = 20;
            NPC.knockBackResist = 0;
            NPC.width = 46;
            NPC.height = 44;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.buffImmune[24] = true;
            SoundStyle hit = new("EbonianMod/Sounds/NPCHit/fleshHit");
            NPC.HitSound = hit;
            NPC.netAlways = true;
            NPC.hide = true;
        }
        Verlet verlet;
        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        }
        public override void OnSpawn(IEntitySource source)
        {
            verlet = new Verlet(NPC.Center, 16, 20, -0.25f, true, true, 30);

            NPC.ai[1] = Main.rand.NextFloat(20, 100);
            NPC.ai[2] = Main.rand.NextFloat(30, 100);
            NPC.ai[3] = Main.rand.NextFloatDirection();
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (verlet != null)
                verlet.Draw(spriteBatch, Texture + "_Vein", null, Texture + "_VeinBase", useRotEnd: true, endRot: NPC.rotation + MathHelper.PiOver2);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, drawColor, NPC.rotation, texture.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            spriteBatch.Draw(glow, NPC.Center - Main.screenPosition, null, Color.White, NPC.rotation, texture.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        Vector2 stalkBase;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            if (stalkBase == Vector2.Zero)
            {
                Vector2 direction = Vector2.UnitY.RotatedBy(MathHelper.PiOver4 + MathHelper.PiOver4 * 0.25f);
                int attempts = 0;
                while (Helper.TRay.CastLength(NPC.Center, direction, 200) >= 200 && attempts++ <= 100)
                {
                    if (attempts == 1)
                        direction = Vector2.UnitY.RotatedBy(-1 * MathHelper.PiOver4 - MathHelper.PiOver4 * 0.25f);
                    else
                        direction = Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4 + MathHelper.PiOver4 * 0.25f);
                }
                stalkBase = Helper.TRay.Cast(NPC.Center, direction, 200) + new Vector2(0, 40);
            }
            NPC.rotation = Helper.FromAToB(NPC.Center, player.Center).ToRotation() + MathHelper.Pi;
            //if (player.Distance(stalkBase) < 320)
            //{
            NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(NPC.ai[1] * NPC.ai[3], NPC.ai[2]), false) * 0.005f;
            NPC.Center = Vector2.Clamp(NPC.Center, stalkBase - new Vector2(300), stalkBase + new Vector2(300));
            if (NPC.Center.Distance(stalkBase) > 320)
            {
                NPC.ai[1] = Main.rand.NextFloat(20, 100);
                NPC.ai[2] = Main.rand.NextFloat(30, 100);
                NPC.ai[3] = Main.rand.NextFloatDirection();
            }
            //}
            /*else if (NPC.Distance(stalkBase) < 320)
            {
                NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(NPC.ai[1] * NPC.ai[3], NPC.ai[2]), false) * 0.005f;
            }*/
            //else
            if (verlet != null)
                verlet.Update(stalkBase, NPC.Center + new Vector2(23, 0).RotatedBy(NPC.rotation));
        }
    }
}