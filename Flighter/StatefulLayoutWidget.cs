using System;

namespace Flighter
{
    /// <summary>
    /// Allows layout control from a stateful enviornment.
    /// 
    /// To use, wrap with a <see cref="StatefulWidget"/>.
    /// </summary>
    public abstract class StatefulLayoutState<T> : State<T>
        where T : StatefulWidget
    {
        protected abstract Size Layout(
            BuildContext bc, 
            ILayoutController layoutController);

        public sealed override Widget Build(BuildContext context)
            => new DelegateLayout(Layout);
    }

    class DelegateLayout : LayoutWidget
    {
        public delegate Size LayoutDelegate(BuildContext context, ILayoutController layoutController);

        readonly LayoutDelegate layoutDelegate;

        public DelegateLayout(LayoutDelegate layoutDelegate)
        {
            this.layoutDelegate = layoutDelegate
                ?? throw new ArgumentNullException(nameof(layoutDelegate));
        }

        public override Size Layout(BuildContext context, ILayoutController layoutController)
            => layoutDelegate(context, layoutController);
    }
}
