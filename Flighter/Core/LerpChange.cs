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
    public delegate Widget ValueBuilder<T>(T value);

    public class LerpChange<T> : StatefulWidget
    {
        public readonly T value;
        public readonly ValueBuilder<T> builder;
        public readonly TickProvider tickProvider;
        public readonly float ratioPerSecond;
        public readonly Lerp<T> lerp;
        public readonly Func<T, T, bool> stopCondition;

        public LerpChange(
            T value, 
            ValueBuilder<T> builder, 
            float ratioPerSecond, 
            TickProvider tickProvider, 
            Lerp<T> lerp,
            Func<T, T, bool> stopCondition = null)
        {
            this.value = value;
            this.builder = builder;
            this.ratioPerSecond = ratioPerSecond;
            this.tickProvider = tickProvider;
            this.lerp = lerp;
            this.stopCondition = stopCondition;
        }

        public override State CreateState() => new LerpChangeSatate<T>();
    }

    class LerpChangeSatate<T> : State
    {
        T curValue;
        bool isLerping;

        public override void Init()
        {
            var w = GetWidget<LerpChange<T>>();
            curValue = w.value;

            w.tickProvider.Tick += TakeLerpStep;
        }

        public override void WidgetChanged()
        {
            isLerping = true;
        }

        public override void Dispose()
        {
            var w = GetWidget<LerpChange<T>>();
            w.tickProvider.Tick -= TakeLerpStep;
        }

        public override Widget Build(BuildContext context)
        {
            var w = GetWidget<LerpChange<T>>();
            return w.builder(curValue);
        }

        void TakeLerpStep(float time, float delta)
        {
            if (!isLerping)
                return;

            var w = GetWidget<LerpChange<T>>();

            if (w?.stopCondition(curValue, w.value) ?? false)
            {
                isLerping = false;
                return;
            }

            SetState(() =>
            {
                curValue = w.lerp(curValue, w.value, 1 - (float)Math.Pow(w.ratioPerSecond, delta));
            });
        }
    }
}
