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
    public class Lyrics : StoryboardObjectGenerator
    {
        [Configurable]
        public string FontPath = "fonts/Paulo Goode - Torus Regular.otf";

        private readonly string FontDirectory = "sb/f/l";
        private readonly float FontScale = 0.3f;

        private FontGenerator Font;

        private double BeatDuration;

        public override void Generate()
        {
            BeatDuration = Beatmap.GetTimingPointAt(0).BeatDuration;
            Font = SetupFont();

            GenerateLine("Blue dusk, cold sight", 10733, 13067);
            GenerateLine("Lost in the dark light", 13067, 15400);
            GenerateLine("A single dried tear", 15733, 18400);
            GenerateLine("A petal hiding fear", 18400, 21066);
            GenerateLine("One star, finite", 21400, 23733);
            GenerateLine("Boundaryless stray lights", 23733, 26400);
            GenerateLine("Fading what’s clear", 26400, 29066);
            GenerateLine("The illusion won’t shear", 29066, 31566);
            GenerateLine("So close but remote", 31566, 34067);
            GenerateLine("To hear you once more", 34233, 36900);
            GenerateLine("But the light is now calling", 36900, 39733);
            GenerateLine("Wait once more", 39733, 41067);
            GenerateLine("And all will be gone", 41067, 46067);

            GenerateLine("Bright lights all around", 53400, 55233);
            GenerateLine("Will only drag me down", 55233, 57900);
            GenerateLine("Blinding fractured faces", 58067, 60733);
            GenerateLine("Refracting broken traces", 60733, 63400);
            GenerateLine("There is no remedy", 63400, 65733);
            GenerateLine("To the memories", 66067, 68400);
            GenerateLine("Will you stop creating", 68733, 71400);
            GenerateLine("With this hatred", 71400, 73067);
            GenerateLine("From my traces", 73067, 74733);
            GenerateLine("Again", 74733, 76400);

            GenerateLine("So far, but close", 97400, 98650);
            GenerateLine("Unhesitating", 98733, 99900);
            GenerateLine("A mirror falls", 99900, 101233);
            GenerateLine("With 2 forms but one soul", 101233, 103233);
            // GenerateLine("I thought you’re gone", 104067, 105233);
            GenerateLine("The dream is fading", 104067, 105400);
            GenerateLine("A path for 2 but only one won't fall", 105400, 111400);

            GenerateLine("Will they be gone?", 112067, 115400);
            GenerateLine("Sharded memories into spaces once gone", 115733, 122067);
            GenerateLine("Will they all fall?", 122733, 126067);
            GenerateLine("Collected, but rejected once more", 126400, 132733);

            GenerateLine("Blue dusk, cold sight", 221733, 224067);
            GenerateLine("Lost in the dark light", 224067, 226733);
            GenerateLine("A single dried tear", 226733, 229733);
            GenerateLine("A petal hiding fear", 229733, 232067);
            GenerateLine("One star, finite", 232400, 234733);
            GenerateLine("Boundaryless stray lights", 234733, 237400);
            GenerateLine("Fading what’s clear", 237400, 239900);
            GenerateLine("The illusion won’t shear", 240067, 242733);
            GenerateLine("So close but remote", 242733, 245400);
            GenerateLine("To hear you once more", 245400, 248067);
            GenerateLine("But the light is now calling", 248067, 250400);
            GenerateLine("Wait once more", 250900, 252067);
            GenerateLine("And all will be gone", 252067, 261066);

            GenerateLine("Will they all fall?", 319898, 324565);
            GenerateLine("Will they be gone?", 325232, 328565);
            GenerateLine("Sharded memories into spaces once gone", 328898, 335232);
            GenerateLine("Will they all fall?", 335898, 339232);
            GenerateLine("Collected, but rejected once more", 339898, 345898);
        }

        private void GenerateLine(string line, double startTime, double endTime)
        {
            float lineWidth = 0f;
            float lineHeight = 0f;
            foreach (var letter in line)
            {
                FontTexture texture = Font.GetTexture(letter.ToString());
                lineWidth += texture.BaseWidth * FontScale;
                lineHeight = Math.Max(lineHeight, texture.BaseHeight * FontScale);
            }

            float letterX = 320 - lineWidth * 0.5f;
            double delay = 0d;
            bool hasBox = false;
            foreach (var letter in line)
            {
                FontTexture texture = Font.GetTexture(letter.ToString());
                if (!texture.IsEmpty)
                {
                    Vector2 position = new Vector2(letterX, 410) + texture.OffsetFor(OsbOrigin.Centre) * FontScale;
                    if (!hasBox)
                    {
                        var box = GetLayer("").CreateSprite("sb/p.png", OsbOrigin.Centre, new Vector2(320, position.Y));
                        box.ScaleVec(OsbEasing.OutExpo, startTime, startTime + BeatDuration * 1.5, 0, lineHeight + 5, lineWidth + 20, lineHeight + 5);
                        box.ScaleVec(OsbEasing.InExpo, endTime - BeatDuration * 1.5, endTime, lineWidth + 20, lineHeight + 5, 0, lineHeight + 5);
                        box.Fade(startTime, 0.8);
                        box.Color(startTime, Color4.Black);

                        hasBox = true;
                    }

                    OsbSprite sprite = GetLayer("").CreateSprite(texture.Path, OsbOrigin.Centre, position);
                    sprite.Scale(startTime + delay, FontScale);
                    sprite.MoveX(OsbEasing.InExpo, endTime - BeatDuration * 1.5, endTime, position.X, 320);
                    sprite.Fade(startTime + delay, startTime + delay + BeatDuration, 0, 1);
                    sprite.Fade(endTime - BeatDuration, endTime, 1, 0);

                    delay += BeatDuration / 8;
                }

                letterX += texture.BaseWidth * FontScale;
            }
        }

        private FontGenerator SetupFont()
        {
            return LoadFont(FontDirectory, new FontDescription()
            {
                FontPath = FontPath,
                FontSize = 60,
                Color = Color4.White
            });
        }
    }
}
