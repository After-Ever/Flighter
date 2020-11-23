namespace Flighter
{
    public abstract class Widget
    {
        /// <summary>
        /// As far as the widget tree is concerned, is this the same as <paramref name="other"/>?
        /// During replacement, if this returns true, the original is kept, 
        /// and the new widget is dropped.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool IsSame(Widget other) => this == other;

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
        public abstract State<W> CreateState<W>() where W : StatefulWidget;
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
