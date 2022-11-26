using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using System.Drawing;

namespace StorybrewScripts
{
    public class Vignette : StoryboardObjectGenerator
    {
        private double BeatDuration;

        public override void Generate()
        {
            BeatDuration = GetBeatDuration(0);

            Bitmap bitmap = GetMapsetBitmap("sb/vig.png");
            OsbSprite sprite = GetLayer("").CreateSprite("sb/vig.png");
            sprite.Scale(0, 480f / bitmap.Height);
            sprite.Fade(0, 1);
            sprite.Fade(AudioDuration - BeatDuration * 8, AudioDuration + BeatDuration * 8, 1, 0);
        }

        private double GetBeatDuration(double time)
            => Beatmap.GetTimingPointAt((int)time).BeatDuration;
    }
}
