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
    public class HitObjectsHighlights : StoryboardObjectGenerator
    {
        private double BeatDuration;
        private double Offset;

        public override void Generate()
        {
            BeatDuration = GetBeatDuration(0);
            Offset = GetOffset(0);

            GenerateGlowingHighlight(76067, 108067);
            GenerateGlowingHighlight(108067, 129400);
            GenerateGlowingHighlight(297066, 318565);
            GenerateGlowingHighlight(321232, 363898);
            GenerateGlowingHighlight(369232, 390565);

            GenerateGlowingHighlightWithDrawSlider(140400, 177400);
            GenerateGlowingHighlightWithDrawSlider(264400, 275066, true, 8);
            GenerateGlowingHighlightWithDrawSlider(275066, 297066);
            GenerateGlowingHighlightWithDrawSlider(395565, 402815);
            GenerateGlowingHighlightWithDrawSlider(402898, 403898, false, 1, 64);

            GenerateRingHighlight(264400, 275065, true, 8);
            GenerateRingHighlight(275066, 295733, true, 1);
            GenerateRingHighlight(392898, 395565);

            GenerateStrikeHighlight(140400, 177400, true, 1);
            GenerateStrikeHighlight(264400, 275065, true, 8);
            GenerateStrikeHighlight(275066, 295733, true, 1);
            GenerateStrikeHighlight(392898, 395565);

            GenerateExplodingParticles(108067, 129400, "exploding", false, 1, 8, 16, 125, true);
            GenerateExplodingParticles(321232, 363898, "exploding", false, 1, 8, 16, 125, true);

            GenerateExplodingParticles(264400, 275066, "exploding", true, 8, 24, 48);
            GenerateExplodingParticles(275066, 297066);
            GenerateExplodingParticles(392898, 403898, "exploding_2");
        }

        private void GenerateGlowingHighlight(double startTime, double endTime, string filepath = "sb/hl.png",
            float startScale = 0.75f, float endScale = 0.2f, int beatDivisor = 8, double beatMultiply = 2)
        {
            using (OsbSpritePool pool = new OsbSpritePool(GetLayer("glowing"), filepath, OsbOrigin.Centre,
                       (sprite, poolStartTime, poolEndTime) => { }))
            {
                foreach (OsuHitObject hitObject in Beatmap.HitObjects)
                {
                    if (hitObject.StartTime < startTime - 5 || endTime - 5 < hitObject.StartTime)
                    {
                        continue;
                    }

                    var end_time = hitObject.EndTime + BeatDuration * beatMultiply;
                    OsbSprite sprite = pool.Get(hitObject.StartTime, end_time);

                    if (hitObject.EndTime != hitObject.StartTime)
                    {
                        sprite.Scale(hitObject.StartTime, startScale);
                        sprite.Fade(hitObject.StartTime, 1);
                    }

                    sprite.Move(hitObject.StartTime, hitObject.Position);
                    sprite.Scale(OsbEasing.InSine, hitObject.EndTime, end_time, startScale, endScale);
                    sprite.Fade(OsbEasing.InSine, hitObject.EndTime, end_time, 1, 0);
                    sprite.Additive(hitObject.StartTime, end_time);
                    sprite.Color(hitObject.StartTime, hitObject.Color);

                    if (hitObject is OsuSlider)
                    {
                        var timestep = Beatmap.GetTimingPointAt((int)hitObject.StartTime).BeatDuration / beatDivisor;
                        var start_time = hitObject.StartTime;
                        while (true)
                        {
                            var endtime = start_time + timestep;

                            var complete = hitObject.EndTime - endtime < 5;
                            if (complete) endtime = hitObject.EndTime;

                            var startPosition = sprite.PositionAt(start_time);
                            sprite.Move(start_time, endtime, startPosition, hitObject.PositionAtTime(endtime));

                            if (complete) break;
                            start_time += timestep;
                        }
                    }
                }
            }
        }

        private void GenerateGlowingHighlightWithDrawSlider(double startTime, double endTime, bool needBeat = false,
            double beatMultiply = 4, int beatDivisor = 32)
        {
            Offset = GetOffset(startTime);

            using (OsbSpritePool pool = new OsbSpritePool(GetLayer("glowing_with_draw_slider"), "sb/hl.png",
                       OsbOrigin.Centre,
                       (sprite, poolStartTime, poolEndTime) => { }))
            {
                foreach (OsuHitObject hitObject in Beatmap.HitObjects)
                {
                    if (hitObject.StartTime < startTime - 5 || endTime - 5 < hitObject.StartTime)
                    {
                        continue;
                    }

                    if (needBeat && !IsNeededBeat(hitObject.StartTime, beatMultiply))
                    {
                        continue;
                    }

                    double endtime = (hitObject is OsuSpinner ? hitObject.EndTime : hitObject.StartTime) +
                                     BeatDuration * 4;
                    OsbSprite sprite = pool.Get(hitObject.StartTime, endtime);

                    sprite.Move(hitObject.StartTime, hitObject.Position);
                    sprite.Scale(hitObject.StartTime, endtime, 0.5, 0.2);
                    sprite.Fade(hitObject.StartTime, hitObject.StartTime + BeatDuration / 4, 0, 1);
                    sprite.Fade(hitObject is OsuSpinner ? hitObject.EndTime : hitObject.StartTime + BeatDuration / 4,
                        endtime, 1, 0);
                    sprite.Additive(hitObject.StartTime, endtime);
                    sprite.Color(hitObject.StartTime, hitObject.Color);

                    if (hitObject is OsuSlider)
                    {
                        double timestep = Beatmap.GetTimingPointAt((int)hitObject.StartTime).BeatDuration / beatDivisor;
                        double start_time = hitObject.StartTime;
                        while (true)
                        {
                            double end_time = start_time + timestep;

                            bool isCompleted = hitObject.EndTime - end_time < 5;
                            if (isCompleted)
                            {
                                end_time = hitObject.EndTime;
                            }

                            var sliderSprite = pool.Get(start_time, start_time + BeatDuration * 4);
                            sliderSprite.Move(start_time, hitObject.PositionAtTime(start_time));
                            sliderSprite.Scale(start_time, start_time + BeatDuration * 4, 0.5, 0.2);
                            sliderSprite.Fade(start_time, start_time + BeatDuration / 4, 0, 1);
                            sliderSprite.Fade(start_time + BeatDuration / 4, start_time + BeatDuration * 4, 1, 0);
                            sliderSprite.Additive(start_time, start_time + BeatDuration * 4);
                            sliderSprite.Color(start_time, hitObject.Color);

                            if (isCompleted)
                            {
                                break;
                            }

                            start_time += timestep;
                        }
                    }
                }
            }
        }

        private void GenerateRingHighlight(double startTime, double endTime, bool needBeat = false,
            double beatMultiply = 4)
        {
            Offset = GetOffset(startTime);

            using (OsbSpritePool pool = new OsbSpritePool(GetLayer("rings"), "sb/c.png", OsbOrigin.Centre,
                       (sprite, poolStartTime, poolEndTime) => { sprite.Additive(poolStartTime, poolEndTime); }))
            {
                foreach (OsuHitObject hitObject in Beatmap.HitObjects)
                {
                    if (hitObject.StartTime < startTime - 5 || endTime - 5 < hitObject.StartTime)
                    {
                        continue;
                    }

                    if (needBeat && !IsNeededBeat(hitObject.StartTime, beatMultiply))
                    {
                        continue;
                    }

                    OsbSprite sprite = pool.Get(hitObject.StartTime, hitObject.StartTime + BeatDuration * 2);
                    sprite.Move(hitObject.StartTime, hitObject.Position);
                    sprite.Fade(hitObject.StartTime, hitObject.StartTime + BeatDuration * 2, 1, 0);
                    sprite.Scale(OsbEasing.Out, hitObject.StartTime, hitObject.StartTime + BeatDuration * 2, 0, 0.35);
                    sprite.Additive(hitObject.StartTime, hitObject.StartTime + BeatDuration * 2);
                }
            }
        }

        private void GenerateStrikeHighlight(double startTime, double endTime, bool needBeat = false,
            double beatMultiply = 4)
        {
            Offset = GetOffset(startTime);

            using (OsbSpritePool pool = new OsbSpritePool(GetLayer("strikes"), "sb/p.png", OsbOrigin.Centre,
                       (sprite, poolStartTime, poolEndTime) => { sprite.Additive(poolStartTime, poolEndTime); }))
            {
                foreach (OsuHitObject hitObject in Beatmap.HitObjects)
                {
                    if (hitObject.StartTime < startTime - 5 || endTime - 5 < hitObject.StartTime)
                    {
                        continue;
                    }

                    if (needBeat && !IsNeededBeat(hitObject.StartTime, beatMultiply))
                    {
                        continue;
                    }

                    OsbSprite sprite = pool.Get(hitObject.StartTime, hitObject.StartTime + BeatDuration * 2);
                    sprite.Fade(hitObject.StartTime, hitObject.StartTime + BeatDuration * 2, 1, 0);
                    sprite.ScaleVec(OsbEasing.Out, hitObject.StartTime, hitObject.StartTime + BeatDuration * 2, 4, 1500,
                        0, 1500);
                    sprite.Move(hitObject.StartTime, hitObject.StartTime + BeatDuration * 2, hitObject.Position,
                        hitObject.Position);
                    sprite.Rotate(hitObject.StartTime, Random(-Math.PI / 6, Math.PI / 6));
                    sprite.Additive(hitObject.StartTime, hitObject.StartTime + BeatDuration * 2);
                }
            }
        }

        private void GenerateExplodingParticles(double startTime, double endTime, string layer = "exploding",
            bool needBeat = false, double beatMultiply = 4, int minParticleCount = 16, int maxParticleCount = 32,
            double explodingRadius = 120, bool focus = false)
        {
            Offset = GetOffset(startTime);

            using (OsbSpritePool pool = new OsbSpritePool(GetLayer(layer), "sb/d.png", OsbOrigin.Centre,
                       (sprite, poolStartTime, poolEndTime) => { sprite.Additive(poolStartTime, poolEndTime); }))
            {
                pool.MaxPoolDuration = 10000;
                foreach (OsuHitObject hitObject in Beatmap.HitObjects)
                {
                    if (hitObject.StartTime < startTime - 5 || endTime - 5 < hitObject.StartTime)
                    {
                        continue;
                    }

                    if (needBeat && !IsNeededBeat(hitObject.StartTime, beatMultiply))
                    {
                        continue;
                    }

                    for (int i = 0; i < Random(minParticleCount, maxParticleCount + 1); i++)
                    {
                        double angle = Random(Math.PI * 2);
                        double radius = Random(explodingRadius * 0.1, explodingRadius);
                        Vector2 endPosition = hitObject.Position + new Vector2(
                            (float)(Math.Cos(angle) * radius),
                            (float)(Math.Sin(angle) * radius)
                        );

                        double particleDuration = Random(BeatDuration * 2, BeatDuration * 4);
                        double focusTime = 0;
                        if (focus)
                        {
                            focusTime = Random(BeatDuration * 2, BeatDuration * 4);
                        }

                        OsbSprite sprite = pool.Get(hitObject.StartTime,
                            hitObject.StartTime + particleDuration + focusTime);
                        sprite.Move(OsbEasing.OutExpo, hitObject.StartTime, hitObject.StartTime + particleDuration,
                            hitObject.Position, endPosition);

                        if (focus)
                        {
                            sprite.Move(OsbEasing.InExpo, hitObject.StartTime + particleDuration,
                                hitObject.StartTime + particleDuration + focusTime, endPosition, new Vector2(182, 156));
                            sprite.Scale(hitObject.StartTime, radius * Random(0.001, 0.0001));
                            sprite.Fade(hitObject.StartTime, 1);
                            sprite.Fade(OsbEasing.In, hitObject.StartTime + particleDuration,
                                hitObject.StartTime + particleDuration + focusTime, 1, 0);
                        }
                        else
                        {
                            sprite.Scale(hitObject.StartTime, hitObject.StartTime + particleDuration, radius * 0.001,
                                0);
                            sprite.Fade(hitObject.StartTime, hitObject.StartTime + particleDuration, 1, 0);
                        }
                    }
                }
            }
        }

        private bool IsNeededBeat(double time, double beatMultiply)
        {
            double dist = (time - Offset - BeatDuration * beatMultiply) % (BeatDuration * beatMultiply);
            return dist < 5 || dist > BeatDuration * beatMultiply - 5;
        }

        public double GetBeatDuration(double time)
            => Beatmap.GetTimingPointAt((int)time).BeatDuration;

        public double GetOffset(double time)
            => Beatmap.GetTimingPointAt((int)time).Offset;
    }
}