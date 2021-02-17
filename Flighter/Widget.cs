using System.Collections.Generic;

namespace Flighter
{
    public delegate Widget WidgetBuilder(BuildContext context);

    // TODO: Name this something better, and describe it! 
    public class WidgetEquality : IEqualityComparer<Widget>
    {
        public bool Equals(Widget x, Widget y) => ReferenceEquals(x, y);

        public int GetHashCode(Widget obj) => obj.GetHashCode();
    }

    public abstract class Widget
    {
        public readonly string key;

        public Widget(string key = null)
        {
            this.key = key;
        }

        /// <summary>
        /// Can this replace <paramref name="other"/> in the tree.
        /// By default, this returns true if both types are equal.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CanReplace(Widget other)
            => other != null
            && GetType() == other.GetType()
            && key == other.key;
    }

    public abstract class StatelessWidget : Widget
    {
        public StatelessWidget(string key = null)
            : base(key) { }

        public abstract Widget Build(BuildContext context);
    }

    public abstract class StatefulWidget : Widget
    {
        public StatefulWidget(string key = null)
            : base(key) { }
        public abstract State CreateState();
    }

    public abstract class LayoutWidget : Widget
    {
        public LayoutWidget(string key = null)
            : base(key) { }
        public abstract Size Layout(BuildContext context, ILayoutController layoutController);
    }

    public abstract class DisplayWidget : LayoutWidget
    {
        public DisplayWidget(string key = null)
            : base(key) { }
     
        public abstract DisplayBox CreateElement();
    }

    public abstract class InheritedWidget : Widget
    {
        public readonly Widget child;

        public InheritedWidget(Widget child, string key = null)
            : base (key)
        {
            this.child = child;
        }
    }
}
