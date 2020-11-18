using Flighter.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    public class RootWidget : DisplayWidget
    {
        static public (WidgetNode, ElementNode) MakeRootWidgetNode(
            Widget child,
            BuildContext initialBuildContext,
            IDisplayRect baseRect,
            ComponentProvider componentProvider,
            Input.Input input)
        {
            var rootWidget = new RootWidget(child);
            var rootElementNode = new RootElementNode(baseRect, componentProvider);

            var widgetNode = new WidgetNodeBuilder(
                new WidgetTree(input),
                rootWidget,
                initialBuildContext,
                rootElementNode).Build(null);

            return (widgetNode, rootElementNode);
        }

        readonly Widget child;

        RootWidget(Widget child)
        {
            this.child = child;
        }

        public override Element CreateElement()
        {
            throw new NotImplementedException("Root widget should never have to make an element.");
        }

        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            var childNode = node.AddChildWidget(child, context);
            return new BuildResult(childNode.size);
        }
    }
}
