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

        public Clip(Widget child)
        {
            this.child = child;
        }

        public override Element CreateElement() => new ClipElement();

        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            var childNode = node.AddChildWidget(child, context);
            return new BuildResult(childNode.size);
        }
    }

    class ClipElement : Element
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
