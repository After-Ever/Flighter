using System.Collections.Generic;

namespace Flighter
{
    public class WidgetEquality : IEqualityComparer<Widget>
    {
        public bool Equals(Widget x, Widget y) => ReferenceEquals(x, y);

        public int GetHashCode(Widget obj) => obj.GetHashCode();
    }

    public abstract class Widget
    {
        /// <summary>
        /// Can this replace <paramref name="other"/> in the tree.
        /// By default, this returns true if both types are equal.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool CanReplace(Widget other) => GetType() == other.GetType();
    }

    public abstract class StatelessWidget : Widget
    {
        public abstract Widget Build(BuildContext context);
    }

    public abstract class StatefulWidget : Widget
    {
        public abstract State CreateState();
    }

    public abstract class LayoutWidget : Widget
    {
        public abstract BuildResult Layout(BuildContext context, WidgetNodeBuilder node);
    }

    public abstract class DisplayWidget : LayoutWidget
    {
        public abstract Element CreateElement();
    }
}
