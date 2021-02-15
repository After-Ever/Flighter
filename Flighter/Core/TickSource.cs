namespace Flighter.Core
{
    public class TickSource : InheritedWidget
    {
        public readonly TickProvider tickProvider;

        public TickSource(Widget child, TickProvider tickProvider, string key = null)
            : base(child, key) 
        {
            this.tickProvider = tickProvider;
        }

        public static TickSource Of(BuildContext context)
            => context.GetInheritedWidgetOfExactType<TickSource>();

        public static TickSource operator +(TickSource tickSource, OnTick onTick)
        {
            tickSource.tickProvider.Tick += onTick;
            return tickSource;
        }
        public static TickSource operator -(TickSource tickSource, OnTick onTick)
        {
            tickSource.tickProvider.Tick -= onTick;
            return tickSource;
        }
    }
}