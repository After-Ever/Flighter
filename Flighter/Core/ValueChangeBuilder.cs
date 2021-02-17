﻿using System;
using System.Collections.Generic;

namespace Flighter.Core
{
    public delegate void ValueChangeCallback<T>(T newValue);

    public class ValueChangeNotifier<T>
    {
        public event ValueChangeCallback<T> ValueChanged;

        public T Value
        {
            get => value;
            set
            {
                if (this.value?.Equals(value) ?? value == null)
                    return;

                this.value = value;
                ValueChanged?.Invoke(value);
            }
        }

        T value;
    }

    public class ValueChangeBuilder<T> : StatefulWidget
    {
        public readonly ValueBuilder<T> builder;
        public readonly ValueChangeNotifier<T> notifier;

        public ValueChangeBuilder(ValueBuilder<T> builder, ValueChangeNotifier<T> notifier, string key = null)
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
                   EqualityComparer<ValueChangeNotifier<T>>.Default.Equals(notifier, builder.notifier);
        }

        public override int GetHashCode()
        {
            var hashCode = -746545925;
            hashCode = hashCode * -1521134295 + EqualityComparer<ValueBuilder<T>>.Default.GetHashCode(builder);
            hashCode = hashCode * -1521134295 + EqualityComparer<ValueChangeNotifier<T>>.Default.GetHashCode(notifier);
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
