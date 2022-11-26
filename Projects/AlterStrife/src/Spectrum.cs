using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Animations;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Linq;

namespace StorybrewScripts
{
    public class Spectrum : StoryboardObjectGenerator
    {
        [Configurable] public float Radius = 75;

        [Configurable] public int BeatDivisor = 16;

        [Configurable] public int BarCount = 96;

        [Configurable] public string SpritePath = "sb/p.png";

        [Configurable] public OsbOrigin SpriteOrigin = OsbOrigin.BottomLeft;

        [Configurable] public Vector2 Scale = new Vector2(1, 100);

        [Configurable] public int LogScale = 600;

        [Configurable] public double Tolerance = 0.2;

        [Configurable] public int CommandDecimals = 1;

        [Configurable] public float MinimalHeight = 0.05f;

        [Configurable] public OsbEasing FftEasing = OsbEasing.InExpo;

        private Bitmap Bitmap;

        private readonly Gradient Gradient = new Gradient();

        public override void Generate()
        {
            Bitmap = GetMapsetBitmap(SpritePath);

            GenerateSpectrum(76067, 108067, BarCount - 1);
            GenerateSpectrum(297066, 318565, BarCount - 1);
        }

        private void GenerateSpectrum(double startTime, double endTime, int barCount)
        {
            KeyframedValue<float>[] keyFramedValues = GetKeyframedValues(startTime, endTime);
            List<int[]> colors = GetBarsColor();

            double barWidth = (Math.PI * 2 * Radius) / barCount;
            for (var i = 0; i < barCount; i++)
            {
                KeyframedValue<float> keyframes = keyFramedValues[i];
                keyframes.Simplify1dKeyframes(Tolerance, h => h);

                double barAngle = Math.PI / 2 - (i * (Math.PI * 2) / barCount);
                Vector2 position = new Vector2(
                    320 + (float)Math.Cos(barAngle) * Radius,
                    240 + (float)Math.Sin(barAngle) * Radius
                );

                OsbSprite bar = GetLayer("").CreateSprite(SpritePath, SpriteOrigin, position);
                bar.CommandSplitThreshold = 5000;
                bar.Color(startTime, colors[i][0] / 255.0, colors[i][1] / 255.0, colors[i][2] / 255.0);
                bar.Rotate(startTime, Math.PI / 2 + barAngle);
                bar.Fade(startTime, 1);
                bar.Additive(startTime, endTime);

                double scaleX = Scale.X * barWidth / Bitmap.Width;
                scaleX = (float)Math.Floor(scaleX * 10) / 10.0f;

                bool hasScale = false;
                keyframes.ForEachPair(
                    (start, end) =>
                    {
                        hasScale = true;
                        bar.ScaleVec(start.Time, end.Time,
                            scaleX, start.Value,
                            scaleX, end.Value);
                    },
                    MinimalHeight,
                    s => (float)Math.Round(s, CommandDecimals)
                );

                if (!hasScale)
                {
                    bar.ScaleVec(startTime, scaleX, MinimalHeight);
                }
            }
        }

        private KeyframedValue<float>[] GetKeyframedValues(double startTime, double endTime)
        {
            KeyframedValue<float>[] keyframes = new KeyframedValue<float>[BarCount];
            for (int i = 0; i < BarCount; i++)
            {
                keyframes[i] = new KeyframedValue<float>(null);
            }

            double fftTimeStep = GetBeatDuration(startTime) / BeatDivisor;
            double fftOffset = fftTimeStep * 0.2;
            for (double time = startTime; time < endTime; time += fftTimeStep)
            {
                float[] fft = GetFft(time + fftOffset, BarCount, null, FftEasing);
                for (int i = 0; i < BarCount; i++)
                {
                    float height = (float)Math.Log10(1 + fft[i] * LogScale) * Scale.Y / Bitmap.Height;
                    if (height < MinimalHeight)
                    {
                        height = MinimalHeight;
                    }

                    keyframes[i].Add(time, height);
                }
            }

            return keyframes;
        }

        private List<int[]> GetBarsColor()
        {
            List<int[]> colors = new List<int[]>();

            // RBG: 80, 95,1 97
            // RGB: 46, 180, 231
            // RGB: 230, 170, 111
            // RGB: 225, 225, 51
            int amount = (int)Math.Ceiling((BarCount - 1) / 3.0);

            List<int[]> colors1 =
                Gradient.GenerateGradient(new int[] { 80, 95, 197 }, new int[] { 46, 180, 231 }, amount);
            List<int[]> colors2 =
                Gradient.GenerateGradient(new int[] { 46, 180, 231 }, new int[] { 230, 170, 111 }, amount);
            List<int[]> colors3 =
                Gradient.GenerateGradient(new int[] { 230, 170, 111 }, new int[] { 225, 225, 51 }, amount);

            foreach (int[] color in colors1)
                colors.Add(color);
            foreach (int[] color in colors2)
                colors.Add(color);
            foreach (int[] color in colors3)
                colors.Add(color);

            colors.Add(colors3.LastOrDefault());

            return colors;
        }

        public double GetBeatDuration(double time)
            => Beatmap.GetTimingPointAt((int)time).BeatDuration;
    }
}