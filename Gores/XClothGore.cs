using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;

namespace EbonianMod.Gores
{
    public class XCloth0 : ModGore
    {
        // Code from Spooky Mod
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            ChildSafety.SafeGore[gore.type] = true;
            gore.velocity = new Vector2(Main.rand.NextFloat() - 0.5f, Main.rand.NextFloat() * MathHelper.TwoPi);
            UpdateType = 910;
        }
        public override bool Update(Gore gore)
        {
            return base.Update(gore);
        }
    }
    public class XCloth1 : XCloth0 { }
    public class XCloth2 : XCloth0 { }
    public class XCloth3 : XCloth0 { }
    public class XCloth4 : XCloth0 { }
}
