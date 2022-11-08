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
    public class Rays : StoryboardObjectGenerator
    {
        private double BeatDuration;

        public override void Generate()
        {
            BeatDuration = GetBeatDuration(0);

            AddRays(53400, 74733);
        }

        private void AddRays(double startTime, double endTime)
        {
            AddRay(startTime, endTime, 1, new Vector2(100, -130), 10000, 50);
            AddRay(startTime, endTime, 2, new Vector2(40, -120), 15000, 75, true);
            AddRay(startTime, endTime, 3, new Vector2(-10, -160), 20000, 65);
        }

        private void AddRay(double startTime, double endTime, int i, Vector2 initialPosition, double wiggleDuration, double wiggleDistance, bool flip = false)
        {
            OsbSprite sprite = GetLayer("").CreateSprite($"sb/r/r{i}.png", OsbOrigin.TopCentre, initialPosition);

            int loopCount = (int)Math.Ceiling(((endTime + BeatDuration * 12) - startTime) / wiggleDuration);
            sprite.StartLoopGroup(startTime, loopCount);
            sprite.MoveX(OsbEasing.InOutSine, 0, wiggleDuration * 0.5, sprite.InitialPosition.X, sprite.InitialPosition.X - 50);
            sprite.MoveX(OsbEasing.InOutSine, wiggleDuration * 0.5, wiggleDuration, sprite.InitialPosition.X - 50, sprite.InitialPosition.X);
            sprite.EndGroup();

            sprite.MoveY(startTime, endTime, sprite.InitialPosition.Y - 200, sprite.InitialPosition.Y);

            sprite.Fade(startTime, 1);
            sprite.Fade(endTime, 0);

            if (flip)
            {
                sprite.FlipH(sprite.StartTime, sprite.EndTime);
            }

            sprite.Additive(sprite.StartTime, sprite.EndTime);
        }

        public double GetBeatDuration(double time)
            => Beatmap.GetTimingPointAt((int)time).BeatDuration;
    }
}