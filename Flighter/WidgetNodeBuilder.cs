﻿using System;
using System.Collections.Generic;

namespace Flighter
{
    public class WidgetNodeBuilder
    {
        public Point Offset;

        readonly WidgetTree tree;
        public readonly Widget widget;
        readonly BuildContext buildContext;
        public readonly Size size;
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
            WidgetTree tree,
            Widget widget, 
            BuildContext buildContext,
            ElementNode inheritedElementNode = null,
            Queue<WidgetNode> inheritedChildren = null)
        {
            this.tree = tree
                ?? throw new ArgumentNullException("WidgetNode must be part of a tree.");
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
                    if (inheritedChildren == null || inheritedChildren.Count != 1)
                        throw new Exception("When StatefulWidgets inherit an element node, they must inherit exactly one child.");

                    elementNode = inheritedElementNode;

                    // Directly add the inherited child The inherited element node
                    // is marked dirty, so the state will rebuild the widget when it's element is updated.
                    var child = inheritedChildren.Dequeue();
                    var childNode = new WidgetNodeBuilder(child);
                    children.Add(childNode);

                    size = childNode.size;
                }
                else
                {
                    var state = sfw.CreateState();
                    elementNode = new ElementNode(new StateElement(state, this), null);

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
                else if (inheritedElementNode != null)
                    throw new Exception("Pure layout widget cannot inherit element!");

                size = lw.Layout(buildContext, this).size;
            }
            else
                throw new Exception("Widget type \"" + widget.GetType() + "\" not handled!");

            if (inheritedChildren != null)
            {
                // Clear any remaining inherited children.
                foreach (var c in inheritedChildren)
                    c.Prune();
                inheritedChildren.Clear();
            }
        }

        public WidgetNodeBuilder(WidgetNode builtNode)
        {
            this.builtNode = builtNode;

            // Inherit size and offset. Size will stay the same, buy offset could change.
            size = builtNode.Size;
            // TODO: Should offset be inheritted? Maybe the default should be zero...
            Offset = builtNode.Offset;
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
                node = new WidgetNode(
                    tree,
                    widget,
                    buildContext,
                    new NodeLayout(size, Offset),
                    parent,
                    children,
                    elementNode);
            }

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

                // TODO This is the same code as in WidgetNode.ReplaceChildren...
                //      Should probably consolidate.
                if (widget.CanReplace(toReplace.widget))
                {
                    childElementNode = toReplace.TakeElementNode();
                    orphans = toReplace.EmancipateChildren();
                }

                toReplace.Prune();
            }

            var childBuilder = new WidgetNodeBuilder(tree, widget, context, childElementNode, orphans);
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
