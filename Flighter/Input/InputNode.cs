using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Input
{
    internal class InputNode : TreeNode
    {
        readonly WidgetNode widgetNode;
        readonly InputWidget widget;

        public InputNode(WidgetNode widgetNode, InputNode parent)
        {
            parent?.AddChild(this);

            this.widgetNode = widgetNode ?? throw new ArgumentNullException();
            this.widget = widgetNode.widget as InputWidget
                ?? throw new Exception("Created input node for non input widget!");
        }

        public void DistributeInputEvent(InputEvent e)
        {
            DFR2LSearch(
                onNode: node =>
                {
                    var inputNode = node as InputNode;
                    var nodeWidget = inputNode.widget;

                    if (nodeWidget.onlyWhileHovering)
                    {
                        foreach (var k in nodeWidget.KeyEventsToReceive)
                        {
                            if (e.CheckKeyEvent(k, nodeWidget.AbsorbEvents))
                                nodeWidget.OnKeyEvent(k);
                        }

                        foreach (var m in nodeWidget.MouseEventsToReceive)
                        {
                            if (e.CheckMouseEvent(m, nodeWidget.AbsorbEvents))
                                nodeWidget.OnMouseEvent(m);
                        }

                        if (nodeWidget.AbsorbWholeEvent)
                            e.SetFullyAbsorbed();
                    }
                },
                takeNode: node =>
                {
                    var inputNode = node as InputNode;
                    return !e.FullyAbsorbed
                        && inputNode.widgetNode.IsHovering(e.inputPoller.MousePoller.Position);
                },
                stopSearch: node => e.FullyAbsorbed);
        }
    }
}
