using System;
using System.Collections.Generic;

namespace Flighter.Core
{
    public class ChangeBuilder : StatefulWidget
    {
        public readonly WidgetBuilder builder;
        public readonly IReadOnlyChangeNotifier notifier;

        public ChangeBuilder(
            WidgetBuilder builder,
            IReadOnlyChangeNotifier notifier, 
            string key = null)
            : base(key)
        {
            this.builder = builder ?? throw new ArgumentNullException();
            this.notifier = notifier ?? throw new ArgumentNullException();
        }

        public override State CreateState() => new ChangeBuliderState();

        public override bool Equals(object obj)
        {
            return obj is ChangeBuilder builder &&
                   key == builder.key &&
                   EqualityComparer<WidgetBuilder>.Default.Equals(this.builder, builder.builder) &&
                   EqualityComparer<IReadOnlyChangeNotifier>.Default.Equals(notifier, builder.notifier);
        }

        public override int GetHashCode()
        {
            int hashCode = -1445322201;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(key);
            hashCode = hashCode * -1521134295 + EqualityComparer<WidgetBuilder>.Default.GetHashCode(builder);
            hashCode = hashCode * -1521134295 + EqualityComparer<IReadOnlyChangeNotifier>.Default.GetHashCode(notifier);
            return hashCode;
        }
    }

    class ChangeBuliderState : State<ChangeBuilder>
    {
        public override void Init()
        {
            widget.notifier.Changed += OnChanged;
        }

        public override void Dispose()
        {
            widget.notifier.Changed -= OnChanged;
        }

        public override Widget Build(BuildContext context)
            => widget.builder(context);

        void OnChanged()
            => SetState(null);
    }
}
