namespace Flighter.Core
{
    public class SizedBox : LayoutWidget
    {
        public readonly Size size;
        public readonly Widget child;

        public SizedBox(Size size, Widget child = null)
        {
            this.size = size;
            this.child = child;
        }

        public SizedBox(float width, float height, Widget child = null)
        {
            size = new Size(width, height);
            this.child = child;
        }

        public override Size Layout(BuildContext context, ILayoutController layoutController)
        {
            if (child != null)
                layoutController.LayoutChild(child, BoxConstraints.Tight(size));
            return size;
        }
    }
}
