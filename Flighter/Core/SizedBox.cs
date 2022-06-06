namespace Flighter.Core
{
    public class SizedBox : LayoutWidget
    {
        public readonly Size size;

        public SizedBox(Size size)
        {
            this.size = size;
        }

        public SizedBox(float width, float height)
        {
            this.size = new Size(width, height);
        }

        public override Size Layout(BuildContext context, ILayoutController layoutController)
            => size;
    }
}
