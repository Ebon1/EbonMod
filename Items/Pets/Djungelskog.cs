using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Map;
using System.Collections.Generic;

namespace EbonianMod.Items.Pets
{
    public class DjungelskogI : ModItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Strange looking teddy bear");
            //Tooltip.SetDefault("Summons a djungelskog.\n\"Den här stora brunbjörnen möter dig alltid med en öppen famn. Kramgo som få och med en härlig jättemage där du kan vila tryggt. En riktig mjukis med andra ord.\"");
        }

        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ProjectileType<Djungelskog>();
            Item.width = 16;
            Item.height = 30;
            Item.UseSound = SoundID.Item2;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.rare = ItemRarityID.Green;
            Item.noMelee = true;
            Item.value = Item.sellPrice(0, 5, 50, 0);
            Item.buffType = BuffType<DjungelskogB>();
        }

        public override void UseStyle(Player player, Rectangle rec)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600, true);
            }
        }
    }
    public class Djungelskog : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 12;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(499);
            Projectile.width = 32;
            Projectile.height = 28;
            Projectile.hide = true;
            AIType = 499;
        }
        public int currentframe;
        public int frametimer;
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Texture2D texture = Request<Texture2D>("EbonianMod/Items/Pets/Djungelskog").Value;
            int frameHeight = Request<Texture2D>("EbonianMod/Items/Pets/Djungelskog").Value.Height / 17;
            int startY = frameHeight * currentframe;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            origin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - 20 : 20);

            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                sourceRectangle, drawColor, 0, origin, Projectile.scale, spriteEffects, 0);

            return false;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation = 0;
            if (Projectile.frame == 0 || Projectile.frame == 1)
            {
                frametimer++;
                if (frametimer == 5)
                {
                    currentframe++;
                    frametimer = 0;
                }
                if (currentframe > 2)
                {
                    currentframe = 0;
                }
            }
            else if (Projectile.frame > 1 && Projectile.frame < 8)
            {
                frametimer++;
                if (frametimer == 5)
                {
                    currentframe++;
                    frametimer = 0;
                }
                if (currentframe > 6 || currentframe < 3)
                {
                    currentframe = 3;
                }
            }
            else if (Projectile.ai[0] != 0)
            {
                frametimer++;
                if (frametimer == 5)
                {
                    currentframe++;
                    frametimer = 0;
                }
                if (currentframe > 16 || currentframe < 7)
                {
                    currentframe = 7;
                }
            }
            if (player.HasBuff(BuffType<DjungelskogB>()))
            {
                Projectile.timeLeft = 2;
            }
        }
    }
    public class DjungelskogB : ModBuff
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Djungelskog");
            //Description.SetDefault("Alla mjukdjur är bra på att kramas, trösta, busa och lyssna. Dessutom är de pålitliga och säkerhetstestade.");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;

            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ProjectileType<Djungelskog>()] < 1)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ProjectileType<Djungelskog>(), 0, 0, 0);
            }
            else
                player.buffTime[buffIndex] = 18000;
        }
    }
}