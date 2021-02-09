using System;
using System.Collections.Generic;
using System.Numerics;

namespace Flighter
{
    public class WidgetNodeBuilder
    {
        public Vector2 Offset;

        public readonly WidgetForest forest;
        public readonly Widget widget;
        readonly BuildContext buildContext;
        public readonly Size size;
        readonly ElementNode elementNode;

        readonly List<WidgetNodeBuilder> children = new List<WidgetNodeBuilder>();
        readonly Queue<WidgetNode> inheritedChildren;

        bool hasBuilt = false;

        public WidgetNodeBuilder(
            WidgetForest forest,
            Widget widget, 
            BuildContext buildContext,
            ElementNode inheritedElementNode = null,
            Queue<WidgetNode> inheritedChildren = null)
        {
            this.forest = forest
                ?? throw new ArgumentNullException("Widget must belong to a tree");
            this.widget = widget
                ?? throw new ArgumentNullException("Widget must not be null.");
            this.buildContext = buildContext;
            this.inheritedChildren = inheritedChildren;

            switch (widget)
            {
                case StatelessWidget slw:
                    {
                        if (inheritedElementNode != null)
                            throw new Exception("StatelessWidget cannot inherit an element node!");

                        var child = slw.Build(this.buildContext);
                        var childNode = AddChildWidget(child, this.buildContext);

                        size = childNode.size;
                        break;
                    }
                case StatefulWidget sfw:
                    {
                        State state;
                        if (inheritedElementNode != null)
                        {
                            if (inheritedChildren == null || inheritedChildren.Count != 1)
                                throw new Exception("When StatefulWidgets inherit an element node, they must inherit exactly one child.");

                            elementNode = inheritedElementNode;

                            var stateElement = elementNode.element as StateElement
                                ?? throw new Exception("StatefulWidget inherited nonStateElement!");
                            stateElement.Builder = this;

                            state = stateElement.state;
                            // Manually call this to reflect the change to the builder.
                            state.WidgetChanged();
                            // This state has been around the block, and may have some scores to settle before rebuilding.
                            state.InvokeUpdates();
                        }
                        else
                        {
                            state = sfw.CreateState();
                            var stateElement = new StateElement(state);
                            elementNode = new ElementNode(stateElement, null);

                            stateElement.Builder = this;
                            state.Init();
                        }

                        var child = state.Build(this.buildContext);
                        var childNode = AddChildWidget(child, this.buildContext);

                        size = childNode.size;
                        break;
                    }
                case LayoutWidget lw:
                    {
                        if (lw is DisplayWidget dw)
                        {
                            elementNode = inheritedElementNode ?? new ElementNode(dw.CreateElement(), null);
                        }
                        else if (inheritedElementNode != null)
                            throw new Exception("Pure layout widget cannot inherit element!");

                        size = lw.Layout(buildContext, this).size;
                        break;
                    }
                case InheritedWidget iw:
                    {
                        this.buildContext = buildContext.Copy();
                        this.buildContext.AddInheritedWidget(iw, iw.GetType());

                        var child = iw.child;
                        var childNode = AddChildWidget(child, this.buildContext);

                        size = childNode.size;
                        break;
                    }
                default:
                    throw new Exception("Widget type \"" + widget.GetType() + "\" not handled!");
            }

            if (inheritedChildren != null)
            {
                // Clear any remaining inherited children.
                foreach (var c in inheritedChildren)
                    c.Dispose();
                inheritedChildren.Clear();
            }
        }

        public WidgetNode Build(WidgetNode parent)
        {
            if (hasBuilt)
                throw new HasBuiltException();

            WidgetNode node = new WidgetNode(
                forest,
                widget,
                buildContext,
                new NodeLayout(size, Offset),
                parent,
                children,
                elementNode);

            children.Clear();
            hasBuilt = true;

            return node;
        }

        public WidgetNodeBuilder LayoutChild(Widget child, BoxConstraints constraints)
        {
            var childBuildContext = buildContext.Copy(constraints);
            return AddChildWidget(child, childBuildContext);
        }

        WidgetNodeBuilder AddChildWidget(Widget widget, BuildContext context)
        {
            if (hasBuilt)
                throw new HasBuiltException();

            ElementNode childElementNode = null;
            Queue<WidgetNode> orphans = null;

            if (inheritedChildren?.Count > 0)
            {
                var toReplace = inheritedChildren.Dequeue();

                if (widget.CanReplace(toReplace.widget))
                {
                    childElementNode = toReplace.TakeElementNode();
                    orphans = toReplace.EmancipateChildren();
                }
                
                toReplace.Dispose();
            }

            var childBuilder = new WidgetNodeBuilder(
                forest, 
                widget,
                context, 
                childElementNode,
                orphans);
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
