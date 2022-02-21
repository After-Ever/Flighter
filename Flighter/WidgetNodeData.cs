using System;
using System.Collections.Generic;
using System.Numerics;
using AEUtils;
using Flighter.Input;

namespace Flighter
{
    internal class WidgetNodeData : TreeNodeData<WidgetNodeData>, IChildLayout
    {
        public readonly Widget widget;
        public Size size { get; internal set; }
        public readonly BuildContext context;
        public Vector2 offset { get; set; }

        public readonly DisplayBox displayBox;
        public readonly State state;

        public WidgetNodeData(
            Widget widget, 
            BuildContext context,
            DisplayBox displayBox = null,
            State state = null)
        {
            this.widget = widget;
            this.context = context;

            this.displayBox = displayBox;
            this.state = state;
        }
        
        public Dictionary<State, (Size size, Vector2 offset)> 
            GetDescendantBoxes(IEnumerable<State> handles)
        {
            if (!inTree)
                throw new Exception("This ChildLayout is not in a tree...");

            var unfound = new HashSet<State>(handles);
            var found = new Dictionary<State, (Size size, Vector2 offset)>();
            node.BFSearch(
                n =>
                {
                    var ns = n.data.state;
                    if (unfound.Contains(ns))
                    {
                        var handleSize = n.data.size;
                        var handleOffset = Vector2.Zero;
                        var upSearchNode = n;
                        while (upSearchNode.data != this)
                        {
                            if (upSearchNode.data.state != null 
                            && found.TryGetValue(upSearchNode.data.state, out (Size, Vector2) l))
                            {
                                handleOffset += l.Item2;
                                break;
                            }

                            handleOffset += upSearchNode.data.offset;
                            upSearchNode = upSearchNode.Parent;
                        }

                        unfound.Remove(ns);
                        found[ns] = (handleSize, handleOffset);
                    }
                },
                stopSearch: _ => unfound.Count == 0);

            return found;
        }

        public WidgetNodeData RebuildCopy()
            => new WidgetNodeData(
                widget,
                context,
                displayBox,
                state);
    }
}
