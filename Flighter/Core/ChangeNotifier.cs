using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public delegate void ChangeCallback();

    public interface IReadOnlyChangeNotifier
    {
        event ChangeCallback Changed;
    }

    public class ChangeNotifier : IReadOnlyChangeNotifier
    {
        public event ChangeCallback Changed;

        public void NotifyChange()
            => Changed?.Invoke();
    }

    public delegate void ValueChangeCallback<T>(T newValue);

    public interface IReadOnlyValueChangeNotifier<T>
    {
        event ValueChangeCallback<T> ValueChanged;

        T Value { get; }
    }

    public class ValueChangeNotifier<T> : IReadOnlyValueChangeNotifier<T>, IReadOnlyChangeNotifier
    {
        public event ValueChangeCallback<T> ValueChanged;
        public event ChangeCallback Changed;

        public ValueChangeNotifier()
        {
            ValueChanged += _ => Changed?.Invoke();
        }

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

        public static implicit operator T(ValueChangeNotifier<T> notifier) => notifier.value;
    }
}
