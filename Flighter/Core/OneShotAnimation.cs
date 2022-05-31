using System;
namespace Flighter.Core
{
    public class OneShotAnimation : StatefulWidget
    {
        public readonly AnimationBuilder builder;
        /// <summary>
        /// In seconds.
        /// </summary>
        public readonly float length;
        public readonly AnimationBehavior behavior;
        public readonly AnimationDirection direction;
        public readonly Curve curve;
        public readonly Action onComplete;

        public OneShotAnimation(
            AnimationBuilder builder,
            float length,
            Action onComplete = null, 
            AnimationBehavior behavior = AnimationBehavior.Once, 
            AnimationDirection direction = AnimationDirection.Forward,
            Curve curve = null)
        {
            this.builder = builder 
                ?? throw new ArgumentNullException(nameof(builder));
            this.length = length;
            this.onComplete = onComplete;
            this.behavior = behavior;
            this.direction = direction;
            this.curve = curve;
        }

        public override State CreateState()
            => new OneShotAnimationState();
    }

    class OneShotAnimationState : State<OneShotAnimation>
    {
        AnimationController anim;

        public override Widget Build(BuildContext context)
            => new Animation(widget.builder, anim);

        public override void Init()
        {
            anim = new AnimationController(
                tickProvider: TickSource.Of(context).tickProvider,
                behavior: widget.behavior,
                direction: widget.direction,
                speed: 1 / widget.length);

            if (widget.onComplete != null)
                anim.AnimationComplete += widget.onComplete;

            anim.Play();
        }

        public override void WidgetChanged()
        {
            if (widget.onComplete != null)
                anim.AnimationComplete -= widget.onComplete;

            Init();
        }

        public override void Dispose()
        {
            anim.Stop();
            if (widget.onComplete != null)
                anim.AnimationComplete -= widget.onComplete;
        }
    }
}
