using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.CameraModifiers;

namespace EbonianMod.Common.CameraModifiers
{
    public class FocusCameraModifier : ICameraModifier
    {
        private int _framesToLast;
        private int _framesLasted;
        public Vector2 _pos;
        private float _lerpMult, _snappingRate;
        private Func<float, float> _easingFunction;

        public string UniqueIdentity { get; private set; }

        public bool Finished { get; private set; }

        public FocusCameraModifier(Vector2 pos, int frames, float lerpMult = 1, Func<float, float> easingFunction = null, float snappingRate = 1, string uniqueIdentity = null)
        {
            _pos = pos - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
            _lerpMult = lerpMult;
            _snappingRate = snappingRate;
            _easingFunction = easingFunction;
            _framesToLast = frames;
            UniqueIdentity = uniqueIdentity;
        }

        public void Update(ref CameraInfo cameraInfo)
        {
            float lerpT = Clamp(MathF.Sin(Pi * Utils.GetLerpValue(0, _framesToLast, _framesLasted)) * _lerpMult, 0, 1);
            if (_easingFunction != null)
            {
                lerpT = _easingFunction.Invoke(Clamp(MathF.Sin(Pi * Utils.GetLerpValue(0, _framesToLast, _framesLasted)) * _lerpMult, 0, 1));
            }
            Main.NewText(_framesLasted, Color.Lerp(Color.Green, Color.Red, lerpT));
            Vector2 pos = Vector2.Lerp(cameraInfo.CameraPosition, _pos, lerpT);

            cameraInfo.CameraPosition = Vector2.Lerp(cameraInfo.CameraPosition, pos, _snappingRate);
            _framesLasted++;
            if (_framesLasted >= _framesToLast)
                Finished = true;
        }
    }
}
