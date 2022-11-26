using OpenTK;
using StorybrewCommon.Animations;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Linq;

namespace StorybrewCommon.Util
{
    public static class ColorConverter
    {
        // Formula: https://www.easyrgb.com/en/math.php
        public static double[] RgbToXyz(int[] rgb)
        {
            // Normalize rgb to [0,1]
            double r = rgb[0] / 255.0;
            double g = rgb[1] / 255.0;
            double b = rgb[2] / 255.0;

            // The inverse function is linear below a corrected value of 0.04045 since gamma correction is linear at 0.0031308
            double invR = InverseCompand(r);
            double invG = InverseCompand(g);
            double invB = InverseCompand(b);

            // xyz scaled to [0,100]
            invR *= 100;
            invG *= 100;
            invB *= 100;

            // Linear rgb is then undergoes a forward transformation to xyz
            double x = invR * 0.4124 + invG * 0.3576 + invB * 0.1805;
            double y = invR * 0.2126 + invG * 0.7152 + invB * 0.0722;
            double z = invR * 0.0193 + invG * 0.1192 + invB * 0.9505;

            var xyz = new double[] { x + 0, y + 0, z + 0 };
            return xyz;
        }

        // Translate from https://github.com/vinaypillai/ac-colors/blob/565fa44e107ba6f9f0197cc5e3e1f94715aec9b1/dist/ac-colors.js#L460
        public static int[] XyzToRgb(double[] xyz)
        {
            // xyz is normalized to [0,1]
            double x = xyz[0] / 100.0;
            double y = xyz[1] / 100.0;
            double z = xyz[2] / 100.0;

            // xyz is multiplied by the reverse transformation matrix to linear rgb
            double invR = 3.2406254773200533 * x - 1.5372079722103187 * y - 0.4986285986982479 * z;
            double invG = -0.9689307147293197 * x + 1.8757560608852415 * y + 0.041517523842953964 * z;
            double invB = 0.055710120445510616 * x + -0.2040210505984867 * y + 1.0569959422543882 * z;

            // Linear rgb must be gamma corrected to normalized srgb. Gamma correction
            // is linear for values <= 0.0031308 to avoid infinite log slope near zero
            double cR = Compand(invR);
            double cG = Compand(invG);
            double cB = Compand(invB);

            // srgb is scaled to [0,255]
            // Add zero to prevent signed zeros (force 0 rather than -0)
            var rgb = new int[] { (int)Math.Round(cR * 255) + 0, (int)Math.Round(cG * 255) + 0, (int)Math.Round(cB * 255) + 0 };
            return rgb;
        }

        // Translate from https://github.com/vinaypillai/ac-colors/blob/565fa44e107ba6f9f0197cc5e3e1f94715aec9b1/dist/ac-colors.js#LL571
        public static double[] XyzToLuv(double[] xyz)
        {
            double x = xyz[0];
            double y = xyz[1];
            double z = xyz[2];

            double eps = 216.0 / 24389;
            double kap = 24389.0 / 27;

            // d65 = [95.05, 100, 108.9]
            double Xn = 95.05;
            double Yn = 100.0;
            double Zn = 108.9;

            double vR = 9 * Yn / (Xn + 15 * Yn + 3 * Zn);
            double uR = 4 * Xn / (Xn + 15 * Yn + 3 * Zn);

            var Luv = new double[3];
            // If XYZ = [0, 0, 0], avoid division by zero and return conversion
            if (x == 0 && y == 0 && z == 0)
            {
                Luv[0] = 0;
                Luv[1] = 0;
                Luv[2] = 0;
                return Luv;
            }

            double v1 = 9 * y / (x + 15 * y + 3 * z);
            double u1 = 4 * x / (x + 15 * y + 3 * z);
            double yR = y / Yn;

            double L = (yR > eps) ? 116 * Math.Pow(yR, 1.0 / 3) - 16 : kap * yR;
            double u = 13 * L * (u1 - uR);
            double v = 13 * L * (v1 - vR);

            Luv[0] = L + 0;
            Luv[1] = u + 0;
            Luv[2] = v + 0;
            return Luv;
        }

        public static double[] LuvToXyz(double[] luv)
        {
            double L = luv[0];
            double u = luv[1];
            double v = luv[2];

            double eps = 216.0 / 24389;
            double kap = 24389.0 / 27;

            // d65 = [95.05, 100, 108.9]
            double Xn = 95.05;
            double Yn = 100.0;
            double Zn = 108.9;

            double v0 = 9.0 * Yn / (Xn + 15.0 * Yn + 3.0 * Zn);
            double u0 = 4.0 * Xn / (Xn + 15.0 * Yn + 3.0 * Zn);

            double y = (L > kap * eps) ? Math.Pow((L + 16.0) / 116, 3) : L / kap;

            // If L is 0 (black), will evaluate to divide by 0, use 0
            double d = L != 0.0 ? y * (39 * L / (v + 13 * L * v0) - 5.0) : 0;
            double c = -1.0 / 3;
            double b = -5.0 * y;

            // If L is 0 (black), will evaluate to divide by 0, use 0
            double a = L != 0.0 ? (52 * L / (u + 13 * L * u0) - 1) / 3 : 0;
            double x = (d - b) / (a - c);
            double z = x * a + b;

            // x,y,z in [0,1] multiply by 100 to scale to [0,100]
            // Add zero to prevent signed zeros (force 0 rather than -0)
            var xyz = new double[3] { x * 100, y * 100, z * 100 };
            return xyz;
        }

        // Translate from https://github.com/vinaypillai/ac-colors/blob/565fa44e107ba6f9f0197cc5e3e1f94715aec9b1/dist/ac-colors.js#L634
        public static double[] LuvToLcHuv(double[] luv)
        {
            double maxZeroTolerance = Math.Pow(10.0, -12);

            double L = luv[0];
            double u = (Math.Abs(luv[1]) < maxZeroTolerance) ? 0 : luv[1];

            // Since atan2 behaves unpredictably for non-zero values of v near 0, round v within the given tolerance
            double v = (Math.Abs(luv[2]) < maxZeroTolerance) ? 0 : luv[2];
            double c = Math.Sqrt(u * u + v * v);

            // Math.atan2 returns angle in radians so convert to degrees
            double h = Math.Atan2(v, u) * 180 / Math.PI;

            // If hue is negative add 360
            h = (h >= 0) ? h : h + 360;

            var Lch = new double[3] { L, c, h };
            return Lch;
        }

        // Translate form https://github.com/vinaypillai/ac-colors/blob/565fa44e107ba6f9f0197cc5e3e1f94715aec9b1/dist/ac-colors.js#L654
        public static double[] LcHuvToLuv(double[] lchUV)
        {
            double L = lchUV[0];
            double c = lchUV[1];

            // Convert hue to radians for use with Math.cos and Math.sin
            double h = lchUV[2] / 180.0 * Math.PI;
            double u = c * Math.Cos(h);
            double v = c * Math.Sin(h);

            var Luv = new double[3] { L, u, v };
            return Luv;
        }

        private static double InverseCompand(double value)
            => value <= 0.04045 ? value / 12.92 : Math.Pow((value + 0.055) / 1.055, 2.4);

        private static double Compand(double value)
            => value <= 0.0031308 ? 12.92 * value : 1.055 * Math.Pow(value, 1 / 2.4) - 0.055;
    }
}
