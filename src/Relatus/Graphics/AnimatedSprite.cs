using Relatus.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public enum AnimationType
    {
        NoLoop,
        Loop
    }

    public class AnimatedSprite : Sprite
    {
        public int Columns { get; private set; }
        public int TotalFrames { get; private set; }
        public int CurrentFrame { get; private set; }
        public float FrameDuration { get; private set; }
        public bool Finished { get; private set; }
        public bool AnimationPlaying { get; private set; }
        public AnimationType AnimationType { get; set; }
        public string[] Sprites { get; set; }

        private readonly Timer timer;

        public AnimatedSprite(float x, float y, string sprite, AnimationType animationType, int totalFrames, int columns, float frameDuration) : this(x, y, sprite, animationType, totalFrames, columns, frameDuration, true)
        {

        }

        public AnimatedSprite(float x, float y, string sprite, AnimationType animationType, int totalFrames, int columns, float frameDuration, bool start) : base(x, y, sprite)
        {
            AnimationType = animationType;
            TotalFrames = totalFrames;
            Columns = columns;
            FrameDuration = frameDuration;
            AnimationPlaying = start;

            timer = new Timer(FrameDuration);

            if (AnimationPlaying)
            {
                timer.Start();
            }
        }

        public AnimatedSprite(float x, float y, string[] sprites, float frameDuration) : base(x, y, sprites[0])
        {
            Sprites = sprites;
            TotalFrames = sprites.Length;
            Columns = TotalFrames;
            FrameDuration = frameDuration;
            timer = new Timer(FrameDuration);
            AnimationType = AnimationType.Loop;
        }

        public void PlayAnimation()
        {
            timer.Start();
        }

        public void PauseAnimation()
        {
            timer.Stop();
        }

        public void ResetAnimation()
        {
            timer.Reset();
            CurrentFrame = 0;
        }

        public string CurrentSprite()
        {
            return Sprites[CurrentFrame];
        }

        public void SetCurrentFrame(int frame)
        {
            if (CurrentFrame == frame)
                return;

            CurrentFrame = frame;
            timer.Reset();
        }

        public override void Update()
        {
            if (Finished)
                return;

            timer.Update();

            if (timer.Done)
            {
                switch (AnimationType)
                {
                    case AnimationType.Loop:
                        CurrentFrame = CurrentFrame >= TotalFrames - 1 ? 0 : ++CurrentFrame;
                        break;
                    case AnimationType.NoLoop:
                        CurrentFrame = CurrentFrame >= TotalFrames - 1 ? TotalFrames : ++CurrentFrame;
                        break;
                }
                timer.Reset();
            }

            Finished = AnimationType == AnimationType.NoLoop && CurrentFrame == TotalFrames;
            AnimationPlaying = !Finished && timer.Enabled;

            if (Sprites == null)
                SetFrame(CurrentFrame, Columns);
            else
                SetSprite(CurrentSprite());
        }
    }
}
