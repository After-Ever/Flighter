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

    public interface IDisposableChildLayout : IChildLayout 
    {
        /// <summary>
        /// Dispose this state, and any decendants.
        /// </summary>
        void DisposeState(); 
    }

    public interface ILayoutController
    {
        IChildLayout LayoutChild(Widget child, BoxConstraints constraints, int index = -1);
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