using Flighter.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    public class RootWidget : InputWidget
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

        RootWidget(Widget child)
            : base(child, false) { }
    }
}
