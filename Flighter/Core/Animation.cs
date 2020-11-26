﻿using Flighter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flighter.Core
{
    public delegate void AnimationUpdate(float value);
    public delegate Widget AnimationBuilder(float t);
    public delegate float Curve(float f);

    public enum AnimationDirection
    {
        Forward,
        Reverse
    }

    public enum AnimationBehavior
    {
        /// <summary>
        /// Once the target is reached, stop playing.
        /// </summary>
        Once,
        /// <summary>
        /// Once the target is reached, go back to the begining, and stop playing.
        /// </summary>
        OnceAndReset,
        /// <summary>
        /// Once the target is reached, go back to the begining, and continue playing.
        /// </summary>
        Loop,
        /// <summary>
        /// Once the target is reached, reverse direction and continue playing.
        /// </summary>
        BackAndForth,
        /// <summary>
        /// Once the target is reached, continue playing passed the target.
        /// </summary>
        Continue
    }

    public class AnimationController
    {
        public event AnimationUpdate ValueChanged;

        public float Value => curve?.Invoke(progress) ?? progress;

        public float progress;
        /// <summary>
        /// Units <see cref="progress"/> will increase per second.
        /// Must be positive. Use <see cref="Reverse"/> to go backwards.
        /// </summary>
        public float Speed
        {
            get => speed;
            set
            {
                if (value <= 0)
                    throw new Exception("Speed must be positive.");
                speed = value;
            }
        }
        public AnimationBehavior behavior;
        public Curve curve;

        public bool isPlaying { get; private set; }
        public AnimationDirection playDirection { get; private set; } = AnimationDirection.Forward;

        readonly TickProvider tickProvider;
        float speed;

        public AnimationController(
            TickProvider tickProvider,
            float speed = 1,
            AnimationBehavior behavior = AnimationBehavior.Once,
            Curve curve = null)
        {
            this.tickProvider = tickProvider;
            this.Speed = speed;
            this.behavior = behavior;
            this.curve = curve;
        }

        public void Play()
        {
            if (isPlaying)
                return;

            tickProvider.Tick += OnTick;
            isPlaying = true;
        }

        public void Stop()
        {
            if (!isPlaying)
                return;

            tickProvider.Tick -= OnTick;
            isPlaying = false;
        }

        public void Reverse()
        {
            if (playDirection == AnimationDirection.Reverse)
                playDirection = AnimationDirection.Forward;
            else
                playDirection = AnimationDirection.Reverse;
        }

        /// <summary>
        /// Instantly sets the progress to the begining, wrt playDirection.
        /// </summary>
        public void Reset()
        {
            if (playDirection == AnimationDirection.Forward)
                progress = 0;
            else
                progress = 1;

            EmitUpdate();
        }

        void OnTick(float time, float delta)
        {
            bool targetReached = false;
            float target;
            if (playDirection == AnimationDirection.Forward)
            {
                progress += speed * delta;
                target = 1;
                if (progress >= target)
                    targetReached = true;
            }
            else
            {
                progress -= speed * delta;
                target = 0;
                if (progress <= target)
                    targetReached = true;
            }
            
            if (targetReached)
            {
                switch(behavior)
                {
                    case AnimationBehavior.Once:
                        progress = target;
                        Stop();
                        break;
                    case AnimationBehavior.OnceAndReset:
                        Stop();
                        Reset();
                        // Return here to avoid emiting another update (Reset emits one).
                        return;
                    case AnimationBehavior.Loop:
                        Reset();
                        // Return here to avoid emiting another update (Reset emits one).
                        return;
                    case AnimationBehavior.BackAndForth:
                        progress = target;
                        Reverse();
                        break;
                    case AnimationBehavior.Continue:
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            EmitUpdate();
        }

        void EmitUpdate()
        {
            ValueChanged?.Invoke(Value);
        }
    }

    public class Animation : StatefulWidget
    {
        public readonly AnimationBuilder builder;
        public readonly AnimationController controller;

        public Animation(AnimationBuilder builder, AnimationController controller)
        {
            this.builder = builder;
            this.controller = controller;
        }

        public override State CreateState() => new AnimationState();
    }

    class AnimationState : State
    {
        public override void Init()
        {
            var w = GetWidget<Animation>();

            w.controller.ValueChanged += OnValueChanged;
        }

        public override void Dispose()
        {
            GetWidget<Animation>().controller.ValueChanged -= OnValueChanged;
        }

        public override Widget Build(BuildContext context)
        {
            var w = GetWidget<Animation>();

            return w.builder(w.controller.Value);
        }

        void OnValueChanged(float _)
        {
            // Just set state to trigger an update and rebuild.
            SetState(null);
        }
    }
}