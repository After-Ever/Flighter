using Flighter;
using Flighter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flighter.Core
{
    public delegate T Lerp<T>(T a, T b, float f);
    public delegate bool StopCondition<T>(T current, T target);
    public delegate Widget ValueBuilder<T>(T value, BuildContext context);

    public class LerpChange<T> : StatefulWidget
    {
        public readonly T value;
        public readonly ValueBuilder<T> builder;
        public readonly float ratioPerSecond;
        public readonly Lerp<T> lerp;
        public readonly StopCondition<T> stopCondition;

        public LerpChange(
            T value, 
            ValueBuilder<T> builder, 
            float ratioPerSecond, 
            Lerp<T> lerp,
            StopCondition<T> stopCondition = null)
        {
            this.value = value;
            this.builder = builder;

            if (ratioPerSecond <= 0 || ratioPerSecond >= 1)
                throw new ArgumentOutOfRangeException("ratioPerSecond");
            this.ratioPerSecond = ratioPerSecond;
            this.lerp = lerp;
            this.stopCondition = stopCondition;
        }
        
        public override State CreateState() => new LerpChangeSatate<T>();

        public override bool Equals(object obj)
        {
            var change = obj as LerpChange<T>;
            return change != null &&
                   EqualityComparer<T>.Default.Equals(value, change.value) &&
                   EqualityComparer<ValueBuilder<T>>.Default.Equals(builder, change.builder) &&
                   ratioPerSecond == change.ratioPerSecond &&
                   EqualityComparer<Lerp<T>>.Default.Equals(lerp, change.lerp) &&
                   EqualityComparer<StopCondition<T>>.Default.Equals(stopCondition, change.stopCondition);
        }

        public override int GetHashCode()
        {
            var hashCode = 1708436396;
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(value);
            hashCode = hashCode * -1521134295 + EqualityComparer<ValueBuilder<T>>.Default.GetHashCode(builder);
            hashCode = hashCode * -1521134295 + ratioPerSecond.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Lerp<T>>.Default.GetHashCode(lerp);
            hashCode = hashCode * -1521134295 + EqualityComparer<StopCondition<T>>.Default.GetHashCode(stopCondition);
            return hashCode;
        }
    }

    class LerpChangeSatate<T> : State
    {
        T curValue;
        bool isLerping;

        TickSource lastTickSource;

        public override void Init()
        {
            var w = GetWidget<LerpChange<T>>();
            curValue = w.value;

            lastTickSource = TickSource.Of(context)
                ?? throw new Exception("LerpChange must inherit from a TickerSource!");

            lastTickSource += TakeLerpStep;
        }

        public override void WidgetChanged()
        {
            var w = GetWidget<LerpChange<T>>();
            
            isLerping = true;
            if (curValue.Equals(w.value) || (w.stopCondition?.Invoke(curValue, w.value) ?? false))
                isLerping = false;

            var tickSource = TickSource.Of(context)
                ?? throw new Exception("LerpChange must inherit from a TickerSource!");

            if (tickSource != lastTickSource)
            {
                lastTickSource -= TakeLerpStep;
                tickSource += TakeLerpStep;
                lastTickSource = tickSource;
            }
        }

        public override void Dispose()
        {
            var w = GetWidget<LerpChange<T>>();
            if (lastTickSource != null)
                lastTickSource -= TakeLerpStep;
        }

        public override Widget Build(BuildContext context)
        {
            var w = GetWidget<LerpChange<T>>();
            return w.builder(curValue, context);
        }

        void TakeLerpStep(float time, float delta)
        {
            if (!isLerping)
                return;
            
            SetState(() =>
            {
                var w = GetWidget<LerpChange<T>>();

                curValue = w.lerp(curValue, w.value, 1 - (float)Math.Pow(1 - w.ratioPerSecond, delta));

                if (w.stopCondition?.Invoke(curValue, w.value) ?? false)
                    isLerping = false;
            });
        }
    }
}
