using System;
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

                    // Directly add the inherited child, as the inherited element node
                    // is marked dirty, so the state will rebuild the widget when it's element is updated.
                    var c = inheritedChildren.Dequeue();
                    children.Add(new WidgetNodeBuilder(c));
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

            // Clear any remaining inherited children.
            while (inheritedChildren?.Count > 0)
            {
                inheritedChildren.Dequeue().Prune();
            }
        }

        public WidgetNodeBuilder(WidgetNode builtNode)
        {
            this.builtNode = builtNode;
            this.size = this.builtNode.Size;
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
