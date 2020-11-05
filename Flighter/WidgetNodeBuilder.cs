using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Flighter
{
    public class WidgetNodeBuilder
    {
        public Vector2 Offset;

        readonly WidgetNodeBuilder parent;
        readonly Widget widget;
        readonly BuildContext buildContext;
        public readonly Vector2 size;
        readonly ElementNode elementNode;

        readonly List<WidgetNodeBuilder> children = new List<WidgetNodeBuilder>();
        readonly Queue<WidgetNode> inheritedChildren;

        bool hasBuilt = false;
        /// <summary>
        /// If not null when <see cref="Build(WidgetNode)"/> is called,
        /// this will be returned instead of actually building.
        /// </summary>
        WidgetNode builtNode = null;

        public WidgetNodeBuilder(
            WidgetNodeBuilder parent,
            Widget widget, 
            BuildContext buildContext,
            ElementNode inheritedElementNode = null,
            Queue<WidgetNode> inheritedChildren = null)
        {
            this.widget = widget
                ?? throw new ArgumentNullException("Widget must not be null.");
            this.buildContext = buildContext;
            this.inheritedChildren = inheritedChildren;

            if (widget is StatelessWidget slw)
            {
                if (inheritedElementNode != null)
                    throw new Exception("StatelessWidget cannot inherit an element node!");

                var child = slw.Build(this.buildContext);
                var childNode = AddChildWidget(child, this.buildContext);

                size = childNode.size;
            }
            else if (widget is StatefulWidget sfw)
            {
                if (inheritedElementNode != null)
                {
                    elementNode = inheritedElementNode;
                    // TODO: Need to attach inherited child...
                }
                else
                {
                    var state = sfw.CreateState();
                    // Must connect the element before building the rest of the tree.
                    elementNode = new ElementNode(new StateElement(state), null);

                    // Manually do the first build. The rest will be handled with state updates.
                    var child = state.Build(this.buildContext);
                    var childNode = AddChildWidget(child, this.buildContext);

                    size = childNode.size;
                }
            }
            else if (widget is LayoutWidget lw)
            {
                if (lw is DisplayWidget dw)
                {
                    elementNode = inheritedElementNode ?? new ElementNode(dw.CreateElement(), null);
                }

                size = lw.Layout(buildContext, this).size;
            }
        }

        public WidgetNodeBuilder(WidgetNode builtNode)
        {
            this.builtNode = builtNode;
            this.size = this.builtNode.layout.size;
        }

        public WidgetNode Build(WidgetNode parent)
        {
            if (hasBuilt)
                throw new HasBuiltException();

            WidgetNode node;

            if (builtNode != null)
            {
                builtNode.UpdateConnection(parent, new NodeLayout(size, Offset));
                node = builtNode;
            }
            else
            {
                node = builtNode ?? new WidgetNode(
                    widget,
                    buildContext,
                    new NodeLayout(size, Offset),
                    parent,
                    children,
                    elementNode);

            }
            parent = null;
            children.Clear();
            builtNode = null;
            hasBuilt = true;

            return node;
        }

        public WidgetNodeBuilder AddChildWidget(Widget widget, BuildContext context)
        {
            if (hasBuilt)
                throw new HasBuiltException();

            ElementNode childElementNode = null;
            Queue<WidgetNode> orphans = null;

            if (inheritedChildren?.Count > 0)
            {
                var toReplace = inheritedChildren.Dequeue();

                if (toReplace.buildContext.Equals(context) && widget.IsSame(toReplace.widget))
                {
                    var b = new WidgetNodeBuilder(toReplace);
                    children.Add(b);
                    return b;
                }

                if (widget.CanReplace(toReplace.widget))
                {
                    childElementNode = toReplace.elementNode;
                    orphans = toReplace.EmancipateChildren();
                }

                toReplace.Prune();
            }

            var childBuilder = new WidgetNodeBuilder(this, widget, context, childElementNode, orphans);
            children.Add(childBuilder);
            return childBuilder;
        }
    }

    public class HasBuiltException : Exception
    {
        public HasBuiltException()
            : base("Cannot perform this operation after the" +
                  " node has been built.")
        { }
    }
}
