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

namespace StorybrewScripts
{
    public class Credits : StoryboardObjectGenerator
    {
        [Configurable]
        public string TopFontPath = "fonts/Paulo Goode - Torus Bold.otf";
        [Configurable]
        public string BottomFontPath = "fonts/Paulo Goode - Torus Light.otf";
        [Configurable]
        public bool ClientSize = false;

        private readonly string TopFontDirectory = "sb/f/c/t";
        private readonly string BottomFontDirectory = "sb/f/c/b";

        private readonly float FontScale = 0.35f;

        private FontGenerator TopFont;
        private FontGenerator BottomFont;

        private double BeatDuration;

        public override void Generate()
        {
            TopFont = SetupFont(TopFontDirectory, TopFontPath, FontStyle.Bold);
            BottomFont = SetupFont(BottomFontDirectory, BottomFontPath, FontStyle.Regular);

            BeatDuration = GetBeatDuration(0);

            GenerateLine("Vietnam osu! Championship 4", 42733, 45400, new Vector2(320, 220));
            GenerateLine("Presents", 42733, 45400, new Vector2(320, 220), false);

            if (!ClientSize && Beatmap.Name != "Sharded Memories Into Spaces Once Gone - client sb size")
            {
                GenerateLine("Alter//Strife feat. vally.exe", 45400, 48067, new Vector2(320, 220));
                GenerateLine("Sad Keyboard Guy, sleepless & Myntian", 45400, 48067, new Vector2(320, 220), false);
                GenerateLine("Sharded Memories Into Spaces Once Gone", 48067, 50733, new Vector2(320, 220));
                GenerateLine("Alvearia × Ducky- × Vermasium × [Boy]DaLat × Yugu", 48067, 50733, new Vector2(320, 220), false);
            }
            else
            {
                GenerateLine("Alter//Strife feat. vally.exe", 45400, 48067, new Vector2(320, 196));
                GenerateLine("Sad Keyboard Guy", 45400, 48067, new Vector2(320, 196), false);
                GenerateLine("sleepless", 45400, 48067, new Vector2(320, 222), false);
                GenerateLine("Myntian", 45400, 48067, new Vector2(320, 248), false);
                
                GenerateLine("Sharded Memories Into Spaces", 48067, 50733, new Vector2(320, 195));
                GenerateLine("Once Gone", 48067, 50733, new Vector2(320, 220));
                GenerateLine("Alvearia × Ducky- × Vermasium", 48067, 50733, new Vector2(320, 220), false);
                GenerateLine("[Boy]DaLat × Yugu", 48067, 50733, new Vector2(320, 245), false);
            }
          
            GenerateLine("storyboard", 50733, 53400, new Vector2(320, 140));
            GenerateLine("Ningguang", 50733, 53400, new Vector2(320, 140), false);
            GenerateLine("hitsound", 50733, 53400, new Vector2(320, 220));
            GenerateLine("Vermasium", 50733, 53400, new Vector2(320, 220), false);
            GenerateLine("background", 50733, 53400, new Vector2(320, 300));
            GenerateLine("Neyako", 50733, 53400, new Vector2(320, 300), false);

            GenerateParticles(42733);
            GenerateParticles(45400);
            GenerateParticles(48067);
            GenerateParticles(50733);

            AddBackground("sb/bg/87273494_p0.jpg", 42733, 53400);
        }

        private void GenerateLine(string title, double startTime, double endTime, Vector2 center, bool isTop = true)
        {
            FontGenerator font = isTop ? TopFont : BottomFont;

            float lineWidth = 0;
            float lineHeight = 0;
            foreach (var letter in title)
            {
                FontTexture texture = font.GetTexture(letter.ToString());
                lineWidth += (texture.BaseWidth * 1.25f) * FontScale;
                lineHeight = Math.Max(lineHeight, texture.BaseHeight * FontScale);
            }

            float letterX = center.X - lineWidth * 0.5f;
            int sign = isTop ? -1 : 1;
            foreach (var letter in title)
            {
                FontTexture texture = font.GetTexture(letter.ToString());
                if (!texture.IsEmpty)
                {
                    Vector2 position = new Vector2(letterX, center.Y + sign * (lineHeight * 0.5f)) + texture.OffsetFor(OsbOrigin.Centre) * FontScale;
                    OsbSprite sprite = GetLayer("").CreateSprite(texture.Path, OsbOrigin.Centre, position);

                    sprite.Scale(startTime, FontScale);
                    sprite.MoveX(OsbEasing.OutExpo, startTime, endTime - BeatDuration * 3, 320, position.X);
                    sprite.MoveX(OsbEasing.InExpo, endTime - BeatDuration * 3, endTime, position.X, 320);
                    sprite.Fade(startTime, startTime + BeatDuration * 2, 0, 1);
                    sprite.Fade(OsbEasing.InExpo, endTime - BeatDuration * 2, endTime, 1, 0);
                }

                letterX += (texture.BaseWidth * 1.25f) * FontScale;
            }
        }

        private void GenerateParticles(double startTime)
        {
            Vector2 center = new Vector2(320, 240);

            for (int i = 0; i < 50; i++)
            {
                double angle = Random(0, Math.PI * 2);
                double radiusX = Random(-472, 472f);
                double radiusY = Random(-240, 240f);

                Vector2 endPosition = new Vector2(
                    (float)(center.X + Math.Cos(angle) * radiusX),
                    (float)(center.Y + Math.Sin(angle) * radiusY)
                );

                double size = Random(0.02f, 0.05f);
                OsbSprite sprite = GetLayer("").CreateSprite("sb/d.png", OsbOrigin.Centre, center);

                sprite.Scale(startTime, size);
                sprite.Move(OsbEasing.OutExpo, startTime, startTime + BeatDuration * 8, center, endPosition);
                sprite.Fade(OsbEasing.Out, startTime + BeatDuration * 2, startTime + BeatDuration * 8, 1, 0);
                sprite.Additive(startTime, startTime + BeatDuration * 8);
            }
        }

        private void AddBackground(string filePath, double startTime, double endTime)
        {
            Bitmap bitmap = GetMapsetBitmap(filePath);
            OsbSprite sprite = GetLayer("bg").CreateSprite(filePath);
            sprite.Scale(startTime, (480.0 / bitmap.Height) * 2.1);
            sprite.Rotate(startTime, endTime, 0, Math.PI);
            sprite.Fade(startTime, 0.4);
            sprite.Fade(endTime, 0);
        }

        private FontGenerator SetupFont(string fontDirectory, string fontPath, FontStyle fontStyle)
        {
            return LoadFont(fontDirectory, new FontDescription()
            {
                FontPath = fontPath,
                FontStyle = fontStyle,
                FontSize = 60,
                Color = Color4.White,
            });
        }

        public double GetBeatDuration(double time)
            => Beatmap.GetTimingPointAt((int)time).BeatDuration;
    }
}
