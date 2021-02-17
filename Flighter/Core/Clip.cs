using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    /// <summary>
    /// Clips the content of any children to the bounds of this widget.
    /// </summary>
    public class Clip : DisplayWidget
    {
        public readonly Widget child;

        public Clip(Widget child, string key = null)
            : base(key)
        {
            this.child = child;
        }

        public override DisplayBox CreateElement() => new ClipElement();

        public override bool Equals(object obj)
        {
            var clip = obj as Clip;
            return clip != null &&
                   EqualityComparer<Widget>.Default.Equals(child, clip.child);
        }

        public override int GetHashCode()
        {
            return -1589309467 + EqualityComparer<Widget>.Default.GetHashCode(child);
        }

        public override Size Layout(BuildContext context, ILayoutController layout)
            => layout.LayoutChild(child, context.constraints).size;
    }

    class ClipElement : DisplayBox
    {
        public override string Name => "Clip";

        protected override void _Init()
        {
            var clipComponent = componentProvider.CreateComponent<ClipComponent>();
            DisplayRect.AddComponent(clipComponent);
        }

        protected override void _Update() { }
    }
}
