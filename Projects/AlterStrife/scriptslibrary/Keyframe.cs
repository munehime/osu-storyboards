using OpenTK;
using StorybrewCommon.Mapset;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Storyboarding.Util;
using StorybrewCommon.Subtitles;
using StorybrewCommon.Util;

namespace StorybrewCommon.Util
{
    public class Keyframe
    {
        public double Time { get; private set; }
        public Vector2 Position { get; private set; }
        public double Angle { get; private set; }

        public Keyframe(double time, Vector2 position, double angle)
        {
            Time = time;
            Position = position;
            Angle = angle;
        }
    }
}