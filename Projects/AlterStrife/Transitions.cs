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
    public class Transitions : StoryboardObjectGenerator
    {
        private double BeatDuration;

        private OsbSprite FlashSprite;

        public override void Generate()
        {
            BeatDuration = GetBeatDuration(0);
            FlashSprite = GetLayer("flash").CreateSprite("sb/p.png");
            FlashSprite.ScaleVec(0, 854, 480);
            FlashSprite.Fade(0, 0);

            AddDiagonalClose(41900, 42733);

            AddSplitScaleCloseUpDown(73400, 74733);
            AddSplitScaleCloseUpDown(151233, 152067);

            AddSplitScaleCloseLeftRight(162067, 162733);

            AddSplitScaleCloseHorizontal(107400, 108067);
            AddSplitScaleCloseHorizontal(317232, 318565);

            AddSplitScaleCloseVertical(295733, 297066);

            AddRotatingSquare(74733, 76067, true);
            AddRotatingSquare(318565, 321232, true);
            AddRotatingSquare2(176733, 179067);

            AddBlackClose(140067, 140400);
            AddBlackCloseWithFollowBeat(150067, 1.5);
            AddBlackCloseWithFollowBeat(160733, 1.5);
            AddBlackCloseWithFollowBeat(175733, 1.5);

            AddFlash(42733, 6);
            AddFlash(45400, 6, 0.75);
            AddFlash(48067, 6, 0.75);
            AddFlash(50733, 6, 0.75);
            AddFlash(53400, 8);
            AddFlash(76067, 8);
            AddFlash(108067, 8);
            AddFlash(129400, 8);
            AddFlash(140400, 8);
            AddFlash(152067, 8);
            AddFlash(162733, 8);
            AddFlash(179067, 8);
            AddFlash(221733, 6);
            AddFlash(264400, 6);
            AddFlash(267066, 6, 0.65);
            AddFlash(269733, 6, 0.65);
            AddFlash(272400, 6, 0.65);
            AddFlash(275066, 8);
            AddFlash(297066, 6);
            AddFlash(321232, 8);
            AddFlash(363898, 8);
            AddFlash(402898, 8);
        }

        private void AddDiagonalClose(double startTime, double endTime)
        {
            double angle = Math.PI / 3;
            Vector2 position = new Vector2(-107, 0);

            for (int i = 0; i < 2; i++)
            {
                OsbSprite sprite = GetLayer("diagonal_close").CreateSprite("sb/p.png", OsbOrigin.CentreLeft, position);
                sprite.ScaleVec(OsbEasing.OutQuart, startTime, endTime, 0, 1500, 422, 1500);
                sprite.Rotate(startTime, angle);

                position = new Vector2(747, 480);
                angle += Math.PI;
            }
        }

        private void AddSplitScaleCloseUpDown(double startTime, double endTime, double beatMultiply = 0.25)
        {
            double spriteAmount = Math.Round((endTime - startTime) / (BeatDuration * beatMultiply));
            double width = 854.0 / spriteAmount;
            double positionX = -107 + (float)width * 0.5f;
            double delay = 0;

            for (int i = 0; i < spriteAmount; i++)
            {
                OsbSprite sprite = GetLayer("split_scale_close_updown").CreateSprite("sb/p.png",
                    i % 2 == 0 ? OsbOrigin.TopCentre : OsbOrigin.BottomCentre,
                    new Vector2((float)positionX, i % 2 == 0 ? 0 : 480));
                sprite.ScaleVec(OsbEasing.OutExpo, startTime + delay, endTime, width, 0, width, 480);

                positionX += width;
                delay += BeatDuration * beatMultiply;
            }
        }

        private void AddSplitScaleCloseLeftRight(double startTime, double endTime, double beatMultiply = 0.25)
        {
            double spriteAmount = Math.Round((endTime - startTime) / (BeatDuration * beatMultiply));
            double height = 854.0 / spriteAmount;
            double positionY = (float)height * 0.5f;
            double delay = 0;

            for (int i = 0; i < spriteAmount; i++)
            {
                OsbSprite sprite = GetLayer("split_scale_close_leftright").CreateSprite("sb/p.png",
                    i % 2 == 0 ? OsbOrigin.CentreLeft : OsbOrigin.CentreRight,
                    new Vector2(i % 2 == 0 ? -107 : 747, (float)positionY));
                sprite.ScaleVec(OsbEasing.OutExpo, startTime + delay, endTime, 0, height, 854, height);

                positionY += height;
                delay += BeatDuration * beatMultiply;
            }
        }

        private void AddSplitScaleCloseHorizontal(double startTime, double endTime, double beatMultiply = 0.25)
        {
            double spriteAmount = Math.Round((endTime - startTime) / (BeatDuration * beatMultiply));
            double width = 854.0 / spriteAmount;
            double positionX = -107 + (float)width * 0.5f;
            double delay = 0;

            for (int i = 0; i < spriteAmount; i++)
            {
                OsbSprite sprite = GetLayer("split_scale_close_horizontal")
                    .CreateSprite("sb/p.png", OsbOrigin.Centre, new Vector2((float)positionX, 240));
                sprite.ScaleVec(OsbEasing.OutExpo, startTime + delay, endTime, 0, 480, width, 480);

                positionX += width;
                delay += BeatDuration * beatMultiply;
            }
        }

        private void AddSplitScaleCloseVertical(double startTime, double endTime, double beatMultiply = 0.25)
        {
            double spriteAmount = Math.Round((endTime - startTime) / (BeatDuration * beatMultiply));
            double height = 480.0 / spriteAmount;
            double positionY = (float)height * 0.5f;
            double delay = 0;

            for (int i = 0; i < spriteAmount; i++)
            {
                OsbSprite sprite = GetLayer("split_scale_close_horizontal")
                    .CreateSprite("sb/p.png", OsbOrigin.Centre, new Vector2(320, (float)positionY));
                sprite.ScaleVec(OsbEasing.OutExpo, startTime + delay, endTime, 854, 0, 854, height);

                positionY += height;
                delay += BeatDuration * beatMultiply;
            }
        }

        private void AddRotatingSquare(double startTime, double endTime, bool addSolidBg = false,
            string solidColor = "#000000")
        {
            if (addSolidBg)
            {
                OsbSprite solid = GetLayer("rotating_square").CreateSprite("sb/p.png");
                solid.ScaleVec(startTime, 854, 480);
                solid.Color(startTime, solidColor);
                solid.Fade(startTime, 1);
                solid.Fade(endTime, 0);
            }

            OsbSprite sprite = GetLayer("rotating_square").CreateSprite("sb/p.png");
            sprite.Scale(OsbEasing.OutExpo, startTime, startTime + BeatDuration * 2, 980, 120);
            sprite.Rotate(OsbEasing.OutExpo, startTime, startTime + BeatDuration * 2, 0, 5 * Math.PI / 3);

            if ((endTime - BeatDuration * 2) != (startTime + BeatDuration * 2))
            {
                sprite.Rotate(OsbEasing.InSine, startTime + BeatDuration * 2, endTime - BeatDuration * 2,
                    sprite.RotationAt(startTime + BeatDuration * 2),
                    sprite.RotationAt(startTime + BeatDuration * 2) + 3 * Math.PI / 4);
            }

            sprite.Scale(OsbEasing.OutExpo, endTime - BeatDuration * 2, endTime, 120, 980);
            sprite.Rotate(OsbEasing.OutQuad, endTime - BeatDuration * 2, endTime,
                sprite.RotationAt(endTime - BeatDuration * 2),
                sprite.RotationAt(endTime - BeatDuration * 2) + 3 * Math.PI / 4);
        }

        private void AddRotatingSquare2(double startTime, double endTime)
        {
            OsbSprite sprite = GetLayer("rotating_square2").CreateSprite("sb/p.png");

            sprite.ScaleVec(OsbEasing.OutExpo, startTime, endTime - BeatDuration * 3, Vector2.Zero, 150, 150);
            sprite.ScaleVec(OsbEasing.OutExpo, endTime - BeatDuration * 3, endTime - BeatDuration - 2, 150, 150, 20,
                854);
            sprite.ScaleVec(OsbEasing.InExpo, endTime - BeatDuration - 2, endTime, 20, 854, 480, 854);

            sprite.Rotate(OsbEasing.OutExpo, startTime, endTime - BeatDuration * 3, 0, 7 * Math.PI / 2);
            sprite.Fade(startTime, 1);
        }

        private void AddBlackClose(double startTime, double endTime)
        {
            for (int i = 0; i < 2; i++)
            {
                OsbSprite sprite = GetLayer("black_close").CreateSprite("sb/p.png",
                    i % 2 == 0 ? OsbOrigin.TopCentre : OsbOrigin.BottomCentre, new Vector2(320, i % 2 * 480));
                sprite.ScaleVec(OsbEasing.OutExpo, startTime, endTime, 854, 0, 854, 240);
                sprite.Color(startTime, "#000000");
            }
        }

        private void AddBlackCloseWithFollowBeat(double startTime, double beatMultiply = 1)
        {
            for (int i = 0; i < 2; i++)
            {
                OsbSprite sprite = GetLayer("black_close").CreateSprite("sb/p.png",
                    i % 2 == 0 ? OsbOrigin.TopCentre : OsbOrigin.BottomCentre, new Vector2(320, i % 2 * 480));
                sprite.Color(startTime, "#000000");

                for (int j = 0; j < 3; j++)
                {
                    sprite.ScaleVec(OsbEasing.OutExpo, startTime + (BeatDuration * beatMultiply) * j, startTime +
                        (BeatDuration * beatMultiply) * (j + 1), 854, 80 * j, 854, 80 * (j + 1));
                }
            }
        }

        private void AddFlash(double startTime, double beatMultiply = 4, double opacity = 1,
            OsbEasing easing = OsbEasing.Out)
        {
            FlashSprite.Fade(easing, startTime, startTime + BeatDuration * beatMultiply, opacity, 0);
        }

        public double GetBeatDuration(double time)
            => Beatmap.GetTimingPointAt((int)time).BeatDuration;
    }
}