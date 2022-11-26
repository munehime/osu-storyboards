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
    public class Gradient
    {
        public List<int[]> GenerateGradient(int[] startColor, int[] endColor, int amount)
        {
            var startColorLch = RGBToLCHuv(startColor);
            var endColorLch = RGBToLCHuv(endColor);

            var colors = new List<int[]>() { startColor };
            for (int i = 1; i < amount; i++)
            {
                var progress = (double)i / amount;
                var color = GetGradientColorAtProgress(startColorLch, endColorLch, progress);
                var rgb = LCHuvToRGB(color);
                colors.Add(rgb);
            }

            return colors;
        }

        private double[] RGBToLCHuv(int[] color)
        {
            double[] xyz = ColorConverter.RgbToXyz(color);
            double[] luv = ColorConverter.XyzToLuv(xyz);
            double[] lch = ColorConverter.LuvToLcHuv(luv);
            return lch;
        }

        private int[] LCHuvToRGB(double[] color)
        {
            double[] luv = ColorConverter.LcHuvToLuv(color);
            double[] xyz = ColorConverter.LuvToXyz(luv);
            int[] rgb = ColorConverter.XyzToRgb(xyz);
            return rgb;
        }

        private double[] GetGradientColorAtProgress(double[] startColor, double[] endColor, double percent)
        {
            double resultRed = startColor[0] + percent * (endColor[0] - startColor[0]);
            double resultGreen = startColor[1] + percent * (endColor[1] - startColor[1]);
            double resultBlue = startColor[2] + percent * (endColor[2] - startColor[2]);

            double[] result = new double[] { resultRed, resultGreen, resultBlue };
            return result;
        }
    }
}