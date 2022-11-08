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
using System.Drawing;
using System.Linq;
using System.IO;

namespace StorybrewScripts
{
    public class Backgrounds : StoryboardObjectGenerator
    {
        private double BeatDuration;

        private readonly Vector2 Center = new Vector2(320, 240);

        public override void Generate()
        {
            BeatDuration = GetBeatDuration(0);

            AddPostCreditsBackground("sb/bg/102061593_p0.png", 53400, 76067);
            AddFirstFunkyPartBackground("sb/bg/92794995_p0.jpg", 76067, 108067);
            AddSecondFunkyPartBackground("sb/bg/madoc.png", 140400, 177150);
            AddFirstSlowPartBackground("sb/bg/84715806_p0.jpg", 129400, 140400);
            AddSecondSlowPartBackground("sb/bg/74318061_p0.jpg", 179067, 221733);
            AddMiddleIntroBackground("sb/bg/100836852_p0.png", 221733, 264400);
            AddBuildUpBackground("sb/bg/87469040_p0.jpg", 264400, 297066);
            AddPostBuildUpBackground("sb/bg/99540624_p0.jpg", 297066, 318565);
            AddEndingBackground("sb/bg/93414470_p0.png", 363898, 402982);

            AddMovingBackground("sb/bg/85548000_p0.jpg", 108067, 129400, "stream_part.csv");
            AddMovingBackground("sb/bg/85548000_p0.jpg", 321232, 363898, "stream_part2.csv");

            AddSolidBackground(151483, 152067, "#000000", "solid_black");
            AddSolidBackground(162150, 162733, "#000000", "solid_black");
            AddSolidBackground(177150, 179067, "#000000", "solid_black");
        }

        private void AddPostCreditsBackground(string filepath, double startTime, double endTime)
        {
            Bitmap bitmap = GetMapsetBitmap(filepath);
            OsbSprite sprite = GetLayer("post_credits").CreateSprite(filepath);

            sprite.Scale(startTime, 854.0 / bitmap.Width);
            sprite.Fade(startTime, 1);
            sprite.Fade(endTime, 0);
            sprite.MoveY(startTime, endTime, 52, 428);
        }

        private void AddFirstSlowPartBackground(string filepath, double startTime, double endTime)
        {
            Bitmap bitmap = GetMapsetBitmap(filepath);
            OsbSprite sprite = GetLayer("first_slow_part").CreateSprite(filepath);

            sprite.Scale(startTime, 854.0 / bitmap.Width);
            sprite.MoveY(startTime, endTime, 188, 280);
            sprite.Fade(startTime, 1);
            sprite.Fade(endTime, 0);
        }

        private void AddSecondSlowPartBackground(string filepath, double startTime, double endTime)
        {
            Bitmap bitmap = GetMapsetBitmap(filepath);
            OsbSprite sprite = GetLayer("second_slow_part").CreateSprite(filepath);

            sprite.Scale(startTime, (854.0 / bitmap.Width) * 1.2);
            sprite.Fade(startTime, 1);
            sprite.Fade(endTime, 0);
            sprite.MoveX(startTime, endTime, 240, 370);
            sprite.MoveY(startTime, endTime, 218, 420);
            sprite.Rotate(startTime, endTime, -Math.PI / 90, Math.PI / 32);
        }

        private void AddMiddleIntroBackground(string filepath, double startTime, double endTime)
        {
            for (int i = 0; i < 9; i++)
            {
                string filepath_ = $"sb/s/s{i}.png";
                Bitmap bitmap_ = GetMapsetBitmap(filepath_);
                OsbSprite smoke = GetLayer("lines").CreateSprite(filepath_);

                double delay = BeatDuration * Random(0, 8f);
                smoke.Fade(252400 - BeatDuration + delay, Random(0.05, 0.55));
                smoke.Additive(252400 - BeatDuration + delay, 252400 + delay + BeatDuration * 1.5 * 20);

                for (int j = 0; j < 20; j++)
                {
                    smoke.MoveX((252400 - BeatDuration) + (delay + (BeatDuration * (j * 1.5))), Random(-107, 747f));
                }

                smoke.StartLoopGroup(252400 - BeatDuration + delay, 20);
                smoke.MoveY(0, BeatDuration * 1.5, -bitmap_.Height, 480 + bitmap_.Height);
                smoke.EndGroup();
            }

            for (int i = 0; i < 25; i++)
            {
                OsbSprite line = GetLayer("lines").CreateSprite("sb/p.png", OsbOrigin.TopCentre);

                double delay = BeatDuration * Random(0, 8f);
                double height = Random(25, 50f);
                line.ScaleVec(252400 + delay, Random(0.1f, 1.5f), height);
                for (int j = 0; j < 20; j++)
                {
                    line.MoveX(252400 + delay + (BeatDuration * (j * 1.5)), Random(-107, 747f));
                }

                line.StartLoopGroup(252400 + delay, 20);
                line.MoveY(0, BeatDuration * 1.5, -height, 480 + height);
                line.EndGroup();
            }

            Bitmap bitmap = GetMapsetBitmap(filepath);
            OsbSprite sprite = GetLayer("post_credits").CreateSprite(filepath);

            sprite.Scale(startTime, 854.0 / bitmap.Width);
            sprite.Fade(startTime, 1);
            sprite.Fade(endTime, 0);
            sprite.MoveY(startTime, 252400, -46, 160);
            // sprite.MoveY(OsbEasing.Out, 252400, endTime, 160, 520);
            sprite.MoveY(OsbEasing.OutQuart, 252400, endTime, 160, 1020);
            sprite.Fade(OsbEasing.Out, 252400, 252400 + BeatDuration * 3.75, 1, 0);
        }

        private void AddBuildUpBackground(string filepath, double startTime, double endTime)
        {
            Bitmap bitmap = GetMapsetBitmap(filepath);
            OsbSprite sprite = GetLayer("post_credits").CreateSprite(filepath);

            sprite.Scale(startTime, endTime, (854.0 / bitmap.Width) * 1.15, (854.0 / bitmap.Width) * 1.05);
            sprite.Fade(startTime, 1);
            sprite.Fade(endTime, 0);
            sprite.Rotate(startTime, endTime, Math.PI / 60, -Math.PI / 60);

            sprite.StartLoopGroup(startTime, 4);
            sprite.MoveY(OsbEasing.InOutSine, 0, BeatDuration * 16, 185, 300);
            sprite.MoveY(OsbEasing.InOutSine, BeatDuration * 16, BeatDuration * 32, 300, 185);
            sprite.EndGroup();
        }

        private void AddPostBuildUpBackground(string filepath, double startTime, double endTime)
        {
            AddParallaxBackground(filepath, startTime, endTime, "post_buildup", 8, 0.025);
        }

        private void AddFirstFunkyPartBackground(string filepath, double startTime, double endTime)
        {
            AddParallaxBackground(filepath, startTime, endTime, "first_funky_part", 8, 0.025);
        }

        private void AddSecondFunkyPartBackground(string filepath, double startTime, double endTime)
        {
            AddParallaxBackground(filepath, startTime, endTime, "second_funky_part", 8, 0.025);
        }

        private void AddEndingBackground(string filepath, double startTime, double endTime)
        {
            Bitmap bitmap = GetMapsetBitmap(filepath);
            OsbSprite sprite = GetLayer("ending").CreateSprite(filepath);

            sprite.Scale(startTime, 854.0 / bitmap.Width);
            sprite.Fade(startTime, 1);
            sprite.Fade(endTime, 0);

            sprite.StartLoopGroup(startTime, 8);
            sprite.MoveY(OsbEasing.InOutSine, 0, BeatDuration * 8, 215, 265);
            sprite.MoveY(OsbEasing.InOutSine, BeatDuration * 8, BeatDuration * 16, 265, 215);
            sprite.EndGroup();
        }

        private void AddMovingBackground(string filepath, double startTime, double endTime, string keyframesFilename)
        {
            string keyframesFilepath = $"keyframes/{keyframesFilename}";

            Bitmap bitmap = GetMapsetBitmap(filepath);
            OsbSprite sprite = GetLayer("moving_background").CreateSprite(filepath);
            sprite.Scale(startTime, (854.0 / bitmap.Width) * 1.1);
            sprite.Fade(startTime, 1);
            sprite.Fade(endTime, 0);

            List<Keyframe> keyframes = new List<Keyframe>();
            Vector2 startPosition = new Vector2(320, 240);
            double startAngle = 0d;
            int exponent = 0;
            double starttime = startTime;

            while (true)
            {
                double angle = Random(0, 2 * Math.PI);
                Vector2 radius = new Vector2(Random(-20f, 20f), Random(-20f, 20f));
                Vector2 endPosition = new Vector2(
                    320 + (float)(radius.X * Math.Cos(angle)),
                    240 + (float)(radius.Y * Math.Sin(angle))
                );

                double endAngle = startAngle + Random(-Math.PI / 120, Math.PI / 120);
                double endtime = starttime + BeatDuration * 4;

                sprite.Move(OsbEasing.InOutSine, starttime, endtime, startPosition, endPosition);
                sprite.Rotate(OsbEasing.InOutSine, starttime, endtime, startAngle, endAngle);

                keyframes.Add(new Keyframe(starttime, startPosition, startAngle));
                if (endtime >= endTime)
                {
                    keyframes.Add(new Keyframe(endtime, endPosition, endAngle));
                    break;
                }

                startPosition = endPosition;
                startAngle = endAngle;
                endAngle += Random(0, Math.PI / 120) * Math.Pow(-1, ++exponent);
                starttime = endtime;
            }

            string fullKeyframesFilepath = Path.Combine(ProjectPath, keyframesFilepath);
            if (!File.Exists(fullKeyframesFilepath))
            {
                using (StreamWriter writter = new StreamWriter(fullKeyframesFilepath))
                {
                    writter.WriteLine("time,positionX,positionY,angle");
                    foreach (Keyframe keyframe in keyframes)
                    {
                        writter.WriteLine("{0},{1},{2},{3}", keyframe.Time, keyframe.Position.X, keyframe.Position.Y,
                            keyframe.Angle);
                    }
                }
            }

            AddGlow(keyframesFilepath);
        }

        private void AddGlow(string keyframesFilepath)
        {
            List<Keyframe> keyframes = GetKeyframes(keyframesFilepath);
            Keyframe previousKeyframe = keyframes.FirstOrDefault();

            OsbSprite sprite = GetLayer("moving_background").CreateSprite("sb/hl.png");
            sprite.Scale(previousKeyframe.Time, 1.35);
            sprite.Fade(previousKeyframe.Time, 0.75);
            sprite.Additive(previousKeyframe.Time, keyframes.LastOrDefault().Time);

            foreach (Keyframe keyframe in keyframes.Skip(1))
            {
                Vector2 startPosition = new Vector2(
                    previousKeyframe.Position.X - 138,
                    previousKeyframe.Position.Y - 68
                );
                Vector2 endPosition = new Vector2(
                    keyframe.Position.X - 138,
                    keyframe.Position.Y - 68
                );

                sprite.Move(OsbEasing.InOutSine, previousKeyframe.Time, keyframe.Time, startPosition, endPosition);

                previousKeyframe = keyframe;
            }
        }

        private List<Keyframe> GetKeyframes(string filepath)
        {
            using (Stream stream = OpenProjectFile(filepath))
            using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8))
            {
                string header = reader.ReadLine();
                string line;
                List<Keyframe> keyframes = new List<Keyframe>();

                while ((line = reader.ReadLine()) != null)
                {
                    string[] args = line.Split(',');
                    double[] values = args.Select(arg => double.Parse(arg)).ToArray();

                    keyframes.Add(new Keyframe(values[0], new Vector2((float)values[1], (float)values[2]), values[3]));
                }

                return keyframes;
            }
        }

        private void AddParallaxBackground(string filepath, double startTime, double endTime, string layer = "parallax",
            double beatDivisor = 8, double moveAmount = 0.1, double backgroundScale = 1.1, double opacity = 1,
            OsbEasing easing = OsbEasing.InOutSine)
        {
            Bitmap bitmap = GetMapsetBitmap(filepath);
            OsbSprite sprite = GetLayer(layer).CreateSprite(filepath, OsbOrigin.Centre);

            sprite.Scale(startTime, (480.0 / bitmap.Height) * backgroundScale);
            sprite.Fade(startTime - 1, startTime, 0, opacity);
            sprite.Fade(endTime, 0);

            OsuHitObject previousHitObject = Beatmap.HitObjects.FirstOrDefault();
            foreach (OsuHitObject hitObject in Beatmap.HitObjects)
            {
                if (hitObject.StartTime < startTime - 5 || endTime - 5 <= hitObject.StartTime)
                {
                    previousHitObject = hitObject;
                    continue;
                }

                Vector2 oldVec = previousHitObject.PositionAtTime(previousHitObject.EndTime);
                Vector2 oldPos = GetTrackedLocation(oldVec, (float)moveAmount);
                Vector2 newVec = hitObject.PositionAtTime(hitObject.StartTime);
                Vector2 newPos = GetTrackedLocation(newVec, (float)moveAmount);

                sprite.Move(easing, previousHitObject.EndTime, hitObject.StartTime, oldPos.X, oldPos.Y, newPos.X,
                    newPos.Y);

                if (hitObject is OsuSlider)
                {
                    double timestep = Beatmap.GetTimingPointAt((int)hitObject.StartTime).BeatDuration / beatDivisor;
                    double starttime = hitObject.StartTime;
                    while (true)
                    {
                        double endtime = starttime + timestep;

                        bool isCompleted = hitObject.EndTime - endtime < 5;
                        if (isCompleted)
                        {
                            endtime = hitObject.EndTime;
                        }

                        oldVec = hitObject.PositionAtTime(starttime);
                        oldPos = GetTrackedLocation(oldVec, (float)moveAmount);
                        newVec = hitObject.PositionAtTime(endtime);
                        newPos = GetTrackedLocation(newVec, (float)moveAmount);

                        sprite.Move(easing, starttime + 1, endtime, oldPos.X, oldPos.Y, newPos.X, newPos.Y);

                        if (isCompleted)
                        {
                            break;
                        }

                        starttime += timestep;
                    }
                }

                previousHitObject = hitObject;
            }
        }

        private void AddSolidBackground(double startTime, double endTime, string color, string layer)
        {
            OsbSprite sprite = GetLayer(layer).CreateSprite("sb/p.png");

            sprite.ScaleVec(startTime, 854, 480);
            sprite.Color(startTime, color);
            sprite.Fade(startTime, 1);
            sprite.Fade(endTime, 0);
        }

        private Vector2 GetTrackedLocation(Vector2 position, float moveAmount)
        {
            return new Vector2(
                -(Center.X - position.X) * moveAmount + Center.X,
                -(Center.Y - position.Y) * moveAmount + Center.Y
            );
        }

        public double GetBeatDuration(double time)
            => Beatmap.GetTimingPointAt((int)time).BeatDuration;
    }
}