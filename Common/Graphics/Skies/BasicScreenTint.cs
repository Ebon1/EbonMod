﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Shaders;

namespace EbonianMod.Common.Graphics.Skies
{
    public class BasicScreenTint : ScreenShaderData
    {
        private int index;

        public BasicScreenTint(string passName)
            : base(passName)
        {
        }

        public override void Apply()
        {
            if (index != -1)
            {
                UseTargetPosition(Main.screenPosition);
            }
            base.Apply();
        }
    }
}
