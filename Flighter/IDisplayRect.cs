using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Flighter
{
    /// <summary>
    /// An interface through which display is controlled.
    /// </summary>
    public interface IDisplayRect
    {
        string Name { get; set; }

        Size Size { get; set; }
        Vector2 Offset { get; set; }

        IDisplayRect CreateChild();
        void SetParent(IDisplayRect rect);

        void AddComponent(Component component);
        void RemoveComponent(Component component);

        void TearDown();
    }
}
