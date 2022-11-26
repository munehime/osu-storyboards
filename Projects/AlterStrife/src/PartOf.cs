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
    public class PartOf : StoryboardObjectGenerator
    {
        [Configurable]
        public string FontPath = "fonts/Paulo Goode - Torus Light.otf";
        [Configurable]
        public bool ClientSize = false;

        private readonly string FontDirectory = "sb/f/c/b";
        private readonly float FontScale = 0.3f;

        private FontGenerator Font;

        private double BeatDuration;

        public override void Generate()
        {
            Font = SetupFont();
            BeatDuration = Beatmap.GetTimingPointAt(0).BeatDuration;

            GenerateMapperPart("Alvearia", 10733, 76067);
            GenerateMapperPart("Ducky-", 76067, 108067);
            GenerateMapperPart("Vermasium", 108067, 140400);
            GenerateMapperPart("[Boy]DaLat", 140400, 221733);
            GenerateMapperPart("Ducky-", 221733, 267066);
            GenerateMapperPart("Alvearia", 267066, 297066);
            GenerateMapperPart("Vermasium", 297066, 321232);
            GenerateMapperPart("Yugu", 321232, 363898);
            GenerateMapperPart("Alvearia", 363898, 403232);
        }

        private void GenerateMapperPart(string name, double startTime, double endTime)
        {
            float lineWidth = 0;
            float lineHeight = 0f;
            foreach (var letter in name)
            {
                FontTexture texture = Font.GetTexture(letter.ToString());
                lineWidth += texture.BaseWidth * FontScale;
                lineHeight = Math.Max(lineHeight, texture.BaseHeight * FontScale);
            }

            Vector2 center = new Vector2(625, 370);
            if (ClientSize || Beatmap.Name == "Sharded Memories Into Spaces Once Gone - client sb size")
            {
                center = new Vector2(520, 370);
            }
            float letterX = center.X - lineWidth * 0.5f;
            bool hasBox = false;
            foreach (var letter in name)
            {
                FontTexture texture = Font.GetTexture(letter.ToString());
                if (!texture.IsEmpty)
                {
                    Vector2 position = new Vector2(letterX, center.Y) + texture.OffsetFor(OsbOrigin.Centre) * FontScale;
                    if (!hasBox)
                    {
                        OsbSprite box = GetLayer("").CreateSprite("sb/p.png", OsbOrigin.Centre, new Vector2(center.X, position.Y));
                        box.ScaleVec(OsbEasing.OutExpo, startTime, startTime + BeatDuration * 4, 0, lineHeight + 5, lineWidth + 20, lineHeight + 5);
                        box.ScaleVec(OsbEasing.InExpo, endTime - BeatDuration * 2, endTime, lineWidth + 20, lineHeight + 5, 0, lineHeight + 5);
                        box.Fade(startTime, 0.8);
                        box.Color(startTime, Color4.Black);

                        hasBox = true;
                    }

                    OsbSprite sprite = GetLayer("").CreateSprite(texture.Path, OsbOrigin.Centre, position);
                    sprite.Scale(startTime, FontScale);
                    sprite.MoveX(OsbEasing.OutExpo, startTime, startTime + BeatDuration * 3, center.X, position.X);
                    sprite.MoveX(OsbEasing.InExpo, endTime - BeatDuration * 3, endTime, position.X, center.X);
                    sprite.Fade(startTime, startTime + BeatDuration * 1.5, 0, 1);
                    sprite.Fade(OsbEasing.InExpo, endTime - BeatDuration * 1.5, endTime, 1, 0);
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
