using Flighter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flighter.Core
{
    public delegate void AnimationUpdate(float value);
    public delegate Widget AnimationBuilder(float t, BuildContext context);
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
        /// Once the target is reached, go back to the beginning, and stop playing.
        /// </summary>
        OnceAndReset,
        /// <summary>
        /// Once the target is reached, go back to the beginning, and continue playing.
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
        /// <summary>
        /// Emitted whenever the animation reaches its target.
        /// </summary>
        public event Action AnimationComplete;

        public float Value => curve?.Invoke(progress) ?? progress;

        public float progress = 0;
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

        public bool isPlaying { get; private set; } = false;
        public AnimationDirection playDirection { get; set; } = AnimationDirection.Forward;

        readonly TickProvider tickProvider;
        float speed;

        public AnimationController(
            TickProvider tickProvider,
            float speed = 1,
            AnimationBehavior behavior = AnimationBehavior.Once,
            AnimationDirection direction = AnimationDirection.Forward,
            Curve curve = null)
        {
            this.tickProvider = tickProvider;
            this.Speed = speed;
            this.behavior = behavior;
            this.curve = curve;
            this.playDirection = direction;

            if (playDirection == AnimationDirection.Reverse)
                progress = 1;
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
        /// Instantly sets the progress to the beginning, wrt playDirection.
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
                targetReached = progress >= target;
            }
            else
            {
                progress -= speed * delta;
                target = 0;
                targetReached = progress <= target;
            }


            bool emitUpdate = true;
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
                        // Reset emits an update.
                        emitUpdate = false;
                        break;
                    case AnimationBehavior.Loop:
                        Reset();
                        // Reset emits an update.
                        emitUpdate = false;
                        break;
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

            if (emitUpdate)
                EmitUpdate();
            if (targetReached)
                AnimationComplete?.Invoke();
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

        public Animation(AnimationBuilder builder, AnimationController controller, string key = null)
            : base(key)
        {
            this.builder = builder;
            this.controller = controller;
        }

        public override State CreateState() => new AnimationState();

        public override bool Equals(object obj)
        {
            var animation = obj as Animation;
            return animation != null &&
                   EqualityComparer<AnimationBuilder>.Default.Equals(builder, animation.builder) &&
                   EqualityComparer<AnimationController>.Default.Equals(controller, animation.controller);
        }

        public override int GetHashCode()
        {
            var hashCode = -639548479;
            hashCode = hashCode * -1521134295 + EqualityComparer<AnimationBuilder>.Default.GetHashCode(builder);
            hashCode = hashCode * -1521134295 + EqualityComparer<AnimationController>.Default.GetHashCode(controller);
            return hashCode;
        }
    }

    class AnimationState : State<Animation>
    {
        AnimationController subscribedController;

        public override void Init()
        {
            
            subscribedController = widget.controller;
            subscribedController.ValueChanged += OnValueChanged;
        }

        public override void WidgetChanged()
        {
            if (subscribedController != null)
                subscribedController.ValueChanged -= OnValueChanged;
            Init();
        }

        public override void Dispose()
        {
            if (subscribedController != null)
                subscribedController.ValueChanged -= OnValueChanged;
            subscribedController = null;
        }

        public override Widget Build(BuildContext context)
            => widget.builder(widget.controller.Value, context);

        void OnValueChanged(float _)
        {
            // Just set state to trigger an update and rebuild.
            SetState(null);
        }
    }
}
