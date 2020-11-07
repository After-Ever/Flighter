﻿using System;
using System.Collections.Generic;
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
        Point Offset { get; set; }

        IDisplayRect CreateChild();
        void SetParent(IDisplayRect rect);

        void AddComponent(Component component);
        bool RemoveComponent(Component component);
        void ClearComponents();

        void TearDown();
    }

    /// <summary>
    /// Provides platform based implementations of DisplayRects.
    /// </summary>
    public interface IDisplayRectProvider
    {
        IDisplayRect CreateDisplayRect();
    }
}
