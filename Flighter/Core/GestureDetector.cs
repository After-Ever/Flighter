using System;
using System.Collections.Generic;
using System.Text;

using Flighter.Input;

namespace Flighter.Core
{
    /// <summary>
    /// Widget that supports gesture recognizers.
    /// </summary>
    public class GestureDetector : InputWidget
    {
        public readonly List<GestureRecognizer> recognizers;

        public GestureDetector(Widget child, List<GestureRecognizer> recognizers, bool onlyWhileHovering = true)
            : base(child, onlyWhileHovering)
        {
            this.recognizers = recognizers;
        }
    }
}
