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
        private float _framesLasted;
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
            _easingFunction ??= (x) => x;
            float lerpT = _easingFunction.Invoke(Clamp(MathF.Sin(Pi * Utils.GetLerpValue(0, _framesToLast, _framesLasted)) * _lerpMult, 0, 1));
            cameraInfo.CameraPosition = Vector2.Lerp(cameraInfo.CameraPosition, Vector2.Lerp(cameraInfo.CameraPosition, _pos, lerpT), _snappingRate);

            float deltaTime = (float)Main.gameTimeCache.TotalGameTime.TotalSeconds - (float)Main.gameTimeCache.ElapsedGameTime.TotalSeconds;

            if (!Main.gameInactive && !Main.gamePaused)
                _framesLasted += 1f * deltaTime;
            if (_framesLasted >= _framesToLast)
                Finished = true;
        }
    }
}
