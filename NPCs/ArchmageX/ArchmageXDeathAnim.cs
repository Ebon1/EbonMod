using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using EbonianMod.Projectiles.ArchmageX;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.Utilities;
using Terraria.Audio;
using ReLogic.Utilities;
using EbonianMod.Common.UI.Dialogue;

namespace EbonianMod.NPCs.ArchmageX
{
    public class ArchmageDeath : ModProjectile
    {
        public override string Texture => "EbonianMod/NPCs/ArchmageX/ArchmageX";
        public override void SetStaticDefaults()
        {
            EbonianMod.projectileFinalDrawList.Add(Type);
        }
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(68, 76);
            Projectile.tileCollide = false;
            Projectile.aiStyle = 2;
            Projectile.timeLeft = 300;
        }
        public override Color? GetAlpha(Color lightColor) => Color.Transparent;
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, Projectile.Center);
            Projectile.velocity = new Vector2(Main.rand.NextFloat(-15f, 15f), -12f);

            Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0);
            Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileType<XExplosionTiny>(), 0, 0);

            Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileType<ArchmageHead>(), 0, 0);
            Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileType<ArchmageStaffGore>(), 0, 0);
            for (int i = 0; i < 2; i++)
                Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileType<ArchmageArm>(), 0, 0);

            for (int i = 0; i < 15; i++)
                Gore.NewGore(null, Projectile.Center, Main.rand.NextVector2Circular(13, 13), Find<ModGore>("EbonianMod/XFlesh" + Main.rand.Next(2).ToString()).Type);

            for (int i = 0; i < 5; i++)
                Gore.NewGore(null, Projectile.Center + Main.rand.NextVector2Circular(30, 30), new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-2, -1)), Find<ModGore>("EbonianMod/XCloth" + i).Type);


        }
    }
    public class ArchmageHead : ModProjectile
    {
        public override string Texture => "EbonianMod/NPCs/ArchmageX/ArchmageX_Head";
        public override string GlowTexture => "EbonianMod/NPCs/ArchmageX/ArchmageX_HeadGlow";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 10;
            EbonianMod.projectileFinalDrawList.Add(Type);
        }
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(34, 42);
            Projectile.tileCollide = false;
            Projectile.aiStyle = 2;
            Projectile.frame = 2;
            Projectile.timeLeft = 300;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Helper.GetTexture(Texture);
            Texture2D glow = Helper.GetTexture(GlowTexture);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * 42, 34, 42), lightColor, Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * 42, 34, 42), Color.White, Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity = new Vector2(Main.rand.NextFloat(-10f, 10f), -12f);
        }
        FloatingDialogueBox d = null;
        SlotId id;
        public override void AI()
        {
            if (Projectile.timeLeft == 250)
            {
                id = SoundEngine.PlaySound(EbonianSounds.xDeath, Projectile.Center);
                d = DialogueSystem.NewDialogueBox(250, Projectile.Center - new Vector2(FontAssets.DeathText.Value.MeasureString(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XDeath").Value).X * -0.5f + 80, 7), Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XDeath").Value, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 5f, true, DialogueAnimationIDs.ColorWhite);
            }
            if (Projectile.timeLeft == 170)
                SoundEngine.PlaySound(EbonianSounds.xareusOutro);
            if (id.IsValid)
            {
                if (SoundEngine.TryGetActiveSound(id, out var activeSound))
                {
                    activeSound.Position = Projectile.Center;
                }
            }
            if (d != null)
            {
                d.Center = Projectile.Center - new Vector2(FontAssets.DeathText.Value.MeasureString(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XDeath").Value).X * -0.5f + 80, 7);
                d.VisibleCenter = Projectile.Center - new Vector2(FontAssets.DeathText.Value.MeasureString(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XDeath").Value).X * -0.5f + 80, 7);
            }
        }
    }
    public class ArchmageArm : ModProjectile
    {
        public override string Texture => "EbonianMod/NPCs/ArchmageX/ArchmageX_Arm";
        public override void SetStaticDefaults()
        {
            EbonianMod.projectileFinalDrawList.Add(Type);
        }
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(26, 30);
            Projectile.tileCollide = false;
            Projectile.aiStyle = 2;
            Projectile.timeLeft = 300;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Helper.GetTexture(Texture);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity = new Vector2(Main.rand.NextFloat(-11f, 11f), -5f * Main.rand.NextFloat(1, 1.5f));
        }
    }
    public class ArchmageStaffGore : ModProjectile
    {
        public override string Texture => "EbonianMod/Items/Weapons/Magic/StaffOfXItem";
        public override void SetStaticDefaults()
        {
            EbonianMod.projectileFinalDrawList.Add(Type);
        }
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(26, 30);
            Projectile.tileCollide = false;
            Projectile.aiStyle = 2;
            Projectile.timeLeft = 300;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        float alpha;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Helper.GetTexture(Texture);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor * MathHelper.SmoothStep(1, 0, alpha), Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Lighting.AddLight(Projectile.Center, TorchID.Purple);
            Projectile.velocity = new Vector2(Main.rand.NextFloat(-11f, 11f), -5f * Main.rand.NextFloat(1, 1.5f));
        }
        public override void AI()
        {
            alpha = MathHelper.SmoothStep(alpha, 1, 0.09f);
        }
    }
}
