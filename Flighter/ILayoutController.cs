using AEUtils;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Flighter
{
    public interface IChildLayout
    {
        Size size { get; }
        Vector2 offset { get; set; }

        Dictionary<State, (Size size, Vector2 offset)> GetDescendantBoxes(IEnumerable<State> handles);
    }

    public interface ILayoutController
    {
        IChildLayout LayoutChild(Widget child, BoxConstraints constraints, int index = -1);

        /// <summary>
        /// Allows building a child without attaching it to the widget tree.
        /// 
        /// This DOES use state from the current tree, and so <see cref="State.WidgetChanged"/>
        /// WILL be called on state in the actual tree. Neither <see cref="State.Dispose"/> nor 
        /// <see cref="State.Init"/> will be called.
        /// 
        /// State should be resistent to these types of opperations, but be sure to be aware!
        /// </summary>
        /// <returns>The without attach.</returns>
        /// <param name="child">Child.</param>
        /// <param name="sandboxContext">Sandbox context. Should be safe to modify any 
        /// referenced values.</param>
        IChildLayout LayoutWithoutAttach(
            Widget child,
            BuildContext sandboxContext);
    }

    public static class ChildLayoutUtil
    {
        /// <summary>
        /// Using <see cref="State"/> as a handle, get the size and offset of a descendant layout.
        /// Offset is relative to this layout.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static (Size size, Vector2 offset) GetDescendantBox(
            this IChildLayout childLayout, 
            State handle)
        {
            var boxes = childLayout.GetDescendantBoxes(handle.AsEnumerable());
            if (!boxes.ContainsKey(handle))
                throw new Exception("Could not find given handle...");

            return boxes[handle];
        }
    }
}