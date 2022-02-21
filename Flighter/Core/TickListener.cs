using System;

namespace Flighter.Core
{
    public delegate Widget TickListenerBuilder(BuildContext context, float time, float delta);

    public class TickListener : StatefulWidget
    {
        public readonly Func<float, float, bool> rebuildPredicate;
        public readonly TickListenerBuilder builder;

        public TickListener(Func<float, float, bool> rebuildPredicate, TickListenerBuilder builder)
        {
            this.rebuildPredicate = rebuildPredicate;
            this.builder = builder 
                ?? throw new ArgumentNullException(nameof(builder));
        }

        public override State CreateState()
            => new TickListenerState();
    }

    class TickListenerState : State<TickListener>
    {
        TickSource lastTickSource;
        float lastTime;
        float lastDelta;

        public override void Init()
        {
            lastTickSource = TickSource.Of(context)
                ?? throw new Exception($"{nameof(TickListener)} must inherit from a {nameof(TickSource)}!");
            lastTime = 0;
            lastDelta = lastTickSource.tickProvider.firstDelta;

            lastTickSource += OnTick;
        }

        public override void WidgetChanged()
        {
            var tickSource = TickSource.Of(context)
                ?? throw new Exception($"{nameof(TickListener)} must inherit from a {nameof(TickSource)}!");

            if (lastTickSource != tickSource)
            {
                lastTickSource -= OnTick;
                tickSource += OnTick;
                lastTickSource = tickSource;
            }
        }

        public override Widget Build(BuildContext context)
            => widget.builder(context, lastTime, lastDelta);

        void OnTick(float time, float delta)
        {
            // These values are always updated, regardless if a rebuild is required.
            lastTime = time;
            lastDelta = delta;

            if (widget.rebuildPredicate?.Invoke(time, delta) ?? true)
            {
                SetState(null);
            }
        }
    }
}
