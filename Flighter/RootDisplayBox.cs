namespace Flighter
{
    public class RootDisplayBox : DisplayBox
    {
        public override string Name => "Root";

        public RootDisplayBox(IDisplayRect rect, ComponentProvider componentProvider)
        {
            Init(rect, componentProvider);
        }

        protected override void _Init() { }

        protected override void _Update() { }
    }
}
