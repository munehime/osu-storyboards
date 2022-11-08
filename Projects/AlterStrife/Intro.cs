using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Mapset;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Storyboarding.Util;
using StorybrewCommon.Subtitles;
using StorybrewCommon.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StorybrewScripts
{
    public class Intro : StoryboardObjectGenerator
    {
        private double BeatDuration;

        public override void Generate()
        {
            BeatDuration = GetBeatDuration(0);

            AddBackground("sb/bg/96796803_p1.jpg", 0, 42733);
            GenerateParticles(-4000, 42733, BeatDuration * 24, 64);
        }

        private void AddBackground(string filePath, double startTime, double endTime)
        {
            var bitmap = GetMapsetBitmap(filePath);
            var sprite = GetLayer("bg").CreateSprite(filePath);
            sprite.Scale(startTime, (480.0 / 2261) * 1.3);
            sprite.Fade(startTime, startTime + BeatDuration * 32, 0, 1);
            sprite.MoveY(startTime, endTime, 200, 280);
            sprite.Rotate(startTime, endTime, -Math.PI / 90, Math.PI / 90);
        }

        private void GenerateParticles(double startTime, double endTime, double particleDuration, int particleCount, string filePath = "sb/d.png", float scale = 0.02f, double opacity = 1)
        {
            var bitmap = GetMapsetBitmap(filePath);
            var bitmapScale = bitmap.Height * scale;
            using (var pool = new OsbSpritePool(GetLayer("particles"), filePath, OsbOrigin.Centre, (sprite, starttime, endtime) =>
            {
                sprite.Fade(startTime - 1, startTime, 0, opacity);
                sprite.Fade(endTime, 0);
            }))
            {
                var timestep = particleDuration / particleCount;
                for (double starttime = startTime - BeatDuration * 12; starttime < endTime; starttime += timestep)
                {
                    var moveSpeed = Random(240, 360);
                    var endtime = starttime + Math.Ceiling(480f / moveSpeed) * particleDuration;
                    var sprite = pool.Get(starttime, endtime);

                    var startX = Random(-107, 747f);
                    sprite.MoveX(starttime, endtime, startX, startX + Random(-50, 50f));
                    sprite.MoveY(OsbEasing.InQuart, starttime, endtime, 480 + bitmapScale, -bitmapScale);
                    sprite.Scale(starttime, Random(scale, scale * 2.5));
                    sprite.Additive(starttime, endtime);
                }
            }
        }

        public double GetBeatDuration(double time)
            => Beatmap.GetTimingPointAt((int)time).BeatDuration;
    }
}
