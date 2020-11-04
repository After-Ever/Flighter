using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace Flighter
{
    public class RootWidget : DisplayWidget
    {
        static public (WidgetNode, ElementNode) MakeRootWidgetNode(
            Widget child,
            BuildContext initialBuildContext,
            RectTransform baseTransform)
        {
            var rootWidget = new RootWidget(child);
            var rootElementNode = new RootElementNode(baseTransform);

            var widgetNode = new WidgetNode(rootWidget, initialBuildContext, null, rootElementNode);

            return (widgetNode, rootElementNode);
        }

        readonly Widget child;

        RootWidget(Widget child)
        {
            this.child = child ?? throw new ArgumentNullException();
        }

        public override Element CreateElement()
        {
            throw new NotImplementedException("Root widget should never have to make an element.");
        }

        public override BuildResult Layout(BuildContext context, WidgetNode node)
        {
            var childNode = node.Add(child, context);
            return childNode.BuildResult;
        }
    }
}
