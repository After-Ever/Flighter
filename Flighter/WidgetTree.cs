using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    /// <summary>
    /// Supplied to all nodes in the tree, this allows access to common
    /// data.
    /// </summary>
    public class WidgetTree
    {
        public readonly Input.Input input;

        public WidgetTree(Input.Input input)
        {
            this.input = input;
        }
    }
}
