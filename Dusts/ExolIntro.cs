using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using ExolRebirth.NPCs.Exol;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace ExolRebirth.Dusts
{
    internal class ExolIntro : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.velocity = Vector2.Zero;
        }
        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return new Color(lightColor.R, lightColor.G, lightColor.B, 25);
        }
        public override bool Update(Dust dust)
        {
            float num56 = dust.scale * 1.4f;
            Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), num56 * 0.1f, num56 * 0.4f, num56);
            dust.rotation += MathHelper.ToRadians(5);
            dust.scale -= 0.05f;
            dust.position = Exol.centerOfExol + Vector2.UnitX.RotatedBy(dust.rotation, Vector2.Zero) * dust.scale * 100;
            if (dust.scale < 0.25f)
            {
                dust.active = false;
            }
            return false;
        }
    }
}