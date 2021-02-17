using System.Numerics;

namespace Flighter
{
    public interface IChildLayout
    {
        Size size { get; }
        Vector2 offset { get; set; }
    }

    public interface ILayoutController
    {
        IChildLayout LayoutChild(Widget child, BoxConstraints constraints, int index = -1);
    }
}