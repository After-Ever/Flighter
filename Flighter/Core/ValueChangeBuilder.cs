using System;
using System.Collections.Generic;

namespace Flighter.Core
{
    public class ValueChangeBuilder<T> : StatefulWidget
    {
        public readonly ValueBuilder<T> builder;
        public readonly IReadOnlyValueChangeNotifier<T> notifier;

        public ValueChangeBuilder(ValueBuilder<T> builder, IReadOnlyValueChangeNotifier<T> notifier, string key = null)
            : base (key)
        {
            this.builder = builder ?? throw new ArgumentNullException();
            this.notifier = notifier ?? throw new ArgumentNullException();
        }

        public override State CreateState() => new ValueChangeBuliderState<T>();

        public override bool Equals(object obj)
        {
            var builder = obj as ValueChangeBuilder<T>;
            return builder != null &&
                   EqualityComparer<ValueBuilder<T>>.Default.Equals(this.builder, builder.builder) &&
                   EqualityComparer<IReadOnlyValueChangeNotifier<T>>.Default.Equals(notifier, builder.notifier);
        }

        public override int GetHashCode()
        {
            var hashCode = -746545925;
            hashCode = hashCode * -1521134295 + EqualityComparer<ValueBuilder<T>>.Default.GetHashCode(builder);
            hashCode = hashCode * -1521134295 + EqualityComparer<IReadOnlyValueChangeNotifier<T>>.Default.GetHashCode(notifier);
            return hashCode;
        }
    }

    class ValueChangeBuliderState<T> : State<ValueChangeBuilder<T>>
    {
        public override void Init()
        {
            widget.notifier.ValueChanged += OnValueChanged;
        }

        public override void Dispose()
        {
            widget.notifier.ValueChanged -= OnValueChanged;
        }

        public override Widget Build(BuildContext context)
            => widget.builder(widget.notifier.Value, context);

        void OnValueChanged(T _)
            => SetState(null);
    }
}
