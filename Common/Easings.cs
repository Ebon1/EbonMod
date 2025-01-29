using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbonianMod.Common
{
    public static class Easings
    {

        public static Func<float, float> Rigid1 = (x) => 1;
        public static Func<float, float> Rigid0 = (x) => 0;


        // Functions taken from https://eaSings.net/
        public static Func<float, float> InOutSine = (x) => -(MathF.Cos(Pi * x) - 1) / 2;
        public static Func<float, float> InOutQuad = (x) => x < 0.5 ? 2 * x * x : 1 - MathF.Pow(-2 * x + 2, 2) / 2;
        public static Func<float, float> InOutCubic = (x) => x < 0.5 ? 4 * x * x * x : 1 - MathF.Pow(-2 * x + 2, 3) / 2;
        public static Func<float, float> InOutQuart = (x) => x < 0.5 ? 8 * x * x * x * x : 1 - MathF.Pow(-2 * x + 2, 4) / 2;
        public static Func<float, float> InOutQuint = (x) => x < 0.5 ? 16 * x * x * x * x * x : 1 - MathF.Pow(-2 * x + 2, 5) / 2;
        public static Func<float, float> InOutExpo = (x) =>
        {
            return x == 0
  ? 0
  : x == 1
  ? 1
  : x < 0.5 ? MathF.Pow(2, 20 * x - 10) / 2
  : (2 - MathF.Pow(2, -20 * x + 10)) / 2;
        };

        public static Func<float, float> InOutCirc = (x) =>
        {
            return x < 0.5
   ? (1 - MathF.Sqrt(1 - MathF.Pow(2 * x, 2))) / 2
   : (MathF.Sqrt(1 - MathF.Pow(-2 * x + 2, 2)) + 1) / 2;
        };


        public const float c1 = 1.70158f;
        public const float c2 = c1 * 1.525f;

        public const float c5 = (2 * Pi) / 4.5f;

        public static Func<float, float> InOutBack = (x) =>
        {
            return x < 0.5
              ? (MathF.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
              : (MathF.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
        };
        public static Func<float, float> InOutElastic = (x) =>
        {
            return x == 0
 ? 0
 : x == 1
 ? 1
 : x < 0.5
 ? -(MathF.Pow(2, 20 * x - 10) * MathF.Sin((20 * x - 11.125f) * c5)) / 2
 : (MathF.Pow(2, -20 * x + 10) * MathF.Sin((20 * x - 11.125f) * c5)) / 2 + 1;
        };


        public const float n1 = 7.5625f;
        public const float d1 = 2.75f;

        public static Func<float, float> OutBounce = (x) =>
        {
            if (x < 1 / d1)
            {
                return n1 * x * x;
            }
            else if (x < 2 / d1)
            {
                return n1 * (x -= 1.5f / d1) * x + 0.75f;
            }
            else if (x < 2.5 / d1)
            {
                return n1 * (x -= 2.25f / d1) * x + 0.9375f;
            }
            else
            {
                return n1 * (x -= 2.625f / d1) * x + 0.984375f;
            }
        };

        public static Func<float, float> InOutBounce = (x) =>
        {
            return x < 0.5f
  ? (1 - OutBounce(1 - 2 * x)) / 2
  : (1 + OutBounce(2 * x - 1)) / 2;
        };
    }
}
