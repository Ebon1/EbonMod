using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbonianMod.Common.TextureLoading
{
    public class TextureLoader : ModSystem
    {
        public override void Load()
        {
            ExtraTextures.LoadExtraTextures1();
            ExtraTextures2.LoadExtraTextures2();
            ExtraSpriteTextures.LoadExtraSpriteTextures();
        }
    }
}

