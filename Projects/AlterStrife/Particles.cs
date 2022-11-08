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
    public class Particles : StoryboardObjectGenerator
    {
        private double BeatDuration;

        public override void Generate()
        {
            BeatDuration = GetBeatDuration(0);

            GenerateParticles(53400, 74733, BeatDuration * 16, 72);
            GenerateParticles(76067, 108067, BeatDuration * 16, 72);
            GenerateParticles(108067, 129400, BeatDuration * 8, 32);
            GenerateParticles(140400, 176733, BeatDuration * 6, 36);
            GenerateParticles(179067, 221733, BeatDuration * 18, 80);
            GenerateParticles(221733, 264400, BeatDuration * 16, 72);
            GenerateParticles(297066, 318565, BeatDuration * 16, 72);
            GenerateParticles(321232, 363898, BeatDuration * 8, 32);
            GenerateParticles(363898, 402898, BeatDuration * 16, 24);
            
            GenerateRaysParticles(53400, 73400, 100, 32);
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
                for (double starttime = startTime - (particleDuration + BeatDuration * 12); starttime < endTime; starttime += timestep)
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

        private void GenerateRaysParticles(double startTime, double endTime, double speed, int particleCount)
        {
            using (var pool = new OsbSpritePool(GetLayer("rays"), "sb/d.png", OsbOrigin.Centre, (sprite, starttime, endtime) => { }))
            {
                var starttime = startTime;
                while (starttime <= endTime)
                {
                    var lifeTime = Random(BeatDuration * 6, BeatDuration * 18);
                    var timeStep = lifeTime / particleCount;
                    var endtime = starttime + lifeTime;

                    var moveAngle = Random(-Math.PI, -Math.PI * 2);
                    var moveDistance = (float)speed * (float)lifeTime * 0.001f;

                    var startPosition = new Vector2(Random(-20, 125f), Random(-5, 0));
                    var endPosition = startPosition + new Vector2((float)Math.Cos(moveAngle), (float)Math.Sin(moveAngle)) * moveDistance;

                    var sprite = pool.Get(starttime, endtime);
                    sprite.Scale(starttime, Random(0.01, 0.07));
                    sprite.Fade(OsbEasing.InCubic, starttime, endtime, 1, 0);

                    sprite.Additive(starttime, endtime);
                    sprite.Move(starttime, endtime, startPosition, endPosition);

                    starttime += timeStep;
                }
            }
        }

        public double GetBeatDuration(double time)
            => Beatmap.GetTimingPointAt((int)time).BeatDuration;
    }
}
