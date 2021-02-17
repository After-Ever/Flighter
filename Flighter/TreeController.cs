using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Flighter.Input;

namespace Flighter
{
    using WidgetNode = TreeNode<WidgetNodeData>;
    using InputNode = TreeNode<InputNodeData>;

    public class TreeController
    {
        readonly WidgetNode widgetTree;
        InputNode inputTree;

        readonly ComponentProvider componentProvider;

        readonly HashSet<State> stateToRebuild = new HashSet<State>();

        HashSet<State> lastBuildState = new HashSet<State>();
        HashSet<DisplayBox> lastBuildDisplays = new HashSet<DisplayBox>();

        bool disposed = false;

        public TreeController(
            Widget rootWidget,
            BuildContext rootContext,
            IDisplayRect rootRect,
            ComponentProvider componentProvider)
        {
            if (rootContext.constraints.IsUnconstrained)
                throw new Exception("Root cannot be unconstrained.");
            // TODO Maybe just take size then?

            this.componentProvider = componentProvider;
            widgetTree = new WidgetNode(new WidgetNodeData(
                null,
                rootContext,
                new RootDisplayBox(rootRect, componentProvider)));
            widgetTree.data.size = rootContext.constraints.MaxSize;

            // Build the root widget.
            var rootTree = BuildWidget(
                rootWidget,
                rootContext);

            widgetTree.AddChild(rootTree);
            UpdateTrees();
        }

        public void Draw()
        {
            if (disposed)
                throw new Exception("This has already been disposed.");

            DoStateUpdates();
        }

        public void DistributeInputEvent(InputEvent e)
        {
            if (disposed)
                throw new Exception("This has already been disposed.");

            inputTree.DFR2LSearch(
                onNode: node =>
                {
                    var iw = node.data.widget;
                    if (iw == null)
                        return;

                    foreach (var k in iw.KeyEventsToReceive)
                    {
                        if (e.CheckKeyEvent(k, iw.AbsorbEvents))
                            iw.OnKeyEvent(k);
                    }

                    foreach (var m in iw.MouseEventsToReceive)
                    {
                        if (e.CheckMouseEvent(m, iw.AbsorbEvents))
                            iw.OnMouseEvent(m);
                    }

                    if (iw.AbsorbWholeEvent)
                        e.SetFullyAbsorbed();
                },
                takeNode: node =>
                {
                    if (e.FullyAbsorbed)
                        return false;
                    var mousePos = e.inputPoller.MousePoller.Position;
                    var topLeft = node.data.offset;
                    var botRight = node.data.size.ToVector2() + topLeft;
                    return mousePos.X >= topLeft.X
                        && mousePos.Y >= topLeft.Y
                        && mousePos.X < botRight.X
                        && mousePos.Y < botRight.Y;
                },
                stopSearch: _ => e.FullyAbsorbed);
        }

        public void Dispose()
        {
            if (disposed)
                return;

            foreach (var state in lastBuildState)
                state.Dispose();
            foreach (var display in lastBuildDisplays)
                display.TearDown();

            lastBuildState = null;
            lastBuildDisplays = null;
            inputTree = null;
            widgetTree.Children[0].Emancipate();
            widgetTree.data.displayBox.TearDown();

            disposed = true;
        }

        void StateNeedsRebuild(State state)
            => stateToRebuild.Add(state);

        void DoStateUpdates()
        {
            if (stateToRebuild.Count > 0)
            {
                var toRebuild = widgetTree.Children[0];
                toRebuild.Emancipate();

                widgetTree.AddChild(RebuildWidgetNode(toRebuild));
                UpdateTrees();
            }
        }

        void UpdateTrees()
        {
            var rootDisplayBox = widgetTree.data.displayBox;
            var rootRect = rootDisplayBox.DisplayRect;
            var displayInTree = new HashSet<DisplayBox>();
            var stateInTree = new HashSet<State>();

            inputTree = new InputNode(new InputNodeData(
                null,
                widgetTree.data.size,
                Vector2.Zero));

            UpdateNode(
                widgetTree.Children.First(), 
                rootRect, 
                Vector2.Zero,
                Vector2.Zero,
                displayInTree,
                stateInTree,
                inputTree);

            foreach (var db in lastBuildDisplays)
                if (!displayInTree.Contains(db))
                    db.TearDown();

            foreach (var state in lastBuildState)
                if (!stateInTree.Contains(state))
                    state.Dispose();

            lastBuildDisplays = displayInTree;
            lastBuildState = stateInTree;
        }

        void UpdateNode(
            WidgetNode node,
            IDisplayRect parentRect,
            Vector2 displayOffset,
            Vector2 absoluteOffset,
            HashSet<DisplayBox> displayInTree,
            HashSet<State> stateInTree,
            InputNode parentInputNode)
        {
            displayOffset += node.data.offset;
            absoluteOffset += node.data.offset;

            var db = node.data.displayBox;
            if (db != null)
            {
                if (!db.IsInitialized)
                    db.Init(parentRect.CreateChild(), componentProvider);

                db.widget = node.data.widget;
                db.size = node.data.size;
                db.offset = displayOffset;

                db.Update();

                displayOffset = Vector2.Zero;
                parentRect = db.DisplayRect;
                displayInTree.Add(db);
            }

            var state = node.data.state;
            if (state != null)
                stateInTree.Add(state);

            var iw = node.data.widget as InputWidget;
            if (iw != null)
            {
                var inputNode = new InputNode(new InputNodeData(iw, node.data.size, absoluteOffset));
                parentInputNode.AddChild(inputNode);
                parentInputNode = inputNode;
            }

            foreach (var c in node.Children)
                UpdateNode(
                    c,
                    parentRect,
                    displayOffset,
                    absoluteOffset,
                    displayInTree,
                    stateInTree,
                    parentInputNode);
        }

        WidgetNode BuildWidget(
            Widget widget,
            BuildContext context,
            WidgetNode referenceWidgetNode = null)
        {
            if (!widget.CanReplace(referenceWidgetNode?.data?.widget))
                referenceWidgetNode = null;

            switch (widget)
            {
                case LayoutWidget lw:
                    {
                        DisplayBox displayBox = null;
                        if (lw is DisplayWidget dw)
                        {
                            displayBox = referenceWidgetNode
                                ?.data
                                ?.displayBox
                                ?? dw.CreateElement();
                        }

                        var node = new WidgetNode(new WidgetNodeData(
                            widget,
                            context,
                            displayBox));

                        var lc = new LayoutController(
                            this,
                            context,
                            referenceWidgetNode);

                        node.data.size = lw.Layout(context, lc);
                        foreach (var c in lc.childNodes)
                            node.AddChild(c);

                        return node;
                    };
                case StatefulWidget sfw:
                    {
                        State state;
                        if (referenceWidgetNode == null)
                        {
                            state = sfw.CreateState();

                            state._Init(
                                widget,
                                context,
                                StateNeedsRebuild);
                        }
                        else
                        {
                            if (referenceWidgetNode.data.state == null)
                                throw new Exception("Stateful widgets node data does not contain state.");
                            state = referenceWidgetNode.data.state;

                            state.InvokeUpdates();
                            stateToRebuild.Remove(state);
                            state._ReBuilt(widget, context);
                        }

                        var node = new WidgetNode(new WidgetNodeData(
                            widget,
                            context,
                            state: state));

                        SetUpSingleChildNode(
                            node,
                            state.Build(context),
                            context,
                            referenceWidgetNode);
                        return node;
                    }
                case StatelessWidget slw:
                    {
                        var node = new WidgetNode(new WidgetNodeData(
                            widget,
                            context));
                        SetUpSingleChildNode(
                            node,
                            slw.Build(context),
                            context,
                            referenceWidgetNode);
                        return node;
                    }
                case InheritedWidget iw:
                    {
                        var node = new WidgetNode(new WidgetNodeData(
                            widget,
                            context));

                        context = context.AddInheritedWidget(iw, iw.GetType());
                        SetUpSingleChildNode(
                            node,
                            iw.child,
                            context,
                            referenceWidgetNode);
                        return node;
                    }
                default:
                    throw new NotSupportedException("Unhandled widget type: "
                        + widget.GetType().Name);
            }
        }

        void SetUpSingleChildNode(
            WidgetNode node,
            Widget child,
            BuildContext context,
            WidgetNode referenceWidgetNode)
        {

            if (referenceWidgetNode != null
                && referenceWidgetNode.Children.Count != 1)
                throw new Exception("Non-layout widgets must have exactly one child.");

            var childNode = BuildWidget(
                child,
                context,
                referenceWidgetNode?.Children?.First());
            node.data.size = childNode.data.size;
            node.AddChild(childNode);
        }

        WidgetNode RebuildWidgetNode(WidgetNode node)
        {
            var newNode = new WidgetNode(node.data.RebuildCopy());

            if (newNode.data.widget is LayoutWidget lw)
            {
                var lc = new LayoutController(
                            this,
                            node.data.context,
                            node,
                            true);

                newNode.data.size = lw.Layout(node.data.context, lc);
                foreach (var c in lc.childNodes)
                    newNode.AddChild(c);

                return newNode;
            }

            if (node.Children.Count != 1)
                throw new Exception("Non-layout widget must have exactly one child.");

            var oldChildNode = node.Children.First();

            WidgetNode childNode;
            if (newNode.data.state != null 
                && stateToRebuild.Remove(newNode.data.state))
            {
                if (!(newNode.data.widget is StatefulWidget))
                    throw new Exception("Non-statefulWidget has attached state!");

                newNode.data.state.InvokeUpdates();

                childNode = BuildWidget(
                    newNode.data.state.Build(newNode.data.context),
                    newNode.data.context,
                    oldChildNode);
            }
            else
                childNode = RebuildWidgetNode(oldChildNode);

            newNode.data.size = childNode.data.size;
            newNode.AddChild(childNode);
            return newNode;
        }

        class LayoutController : ILayoutController
        {
            readonly TreeController treeController;
            readonly BuildContext buildContext;
            readonly List<WidgetNode> referenceChildren;
            readonly bool rebuild;

            public readonly List<WidgetNode> childNodes
                = new List<WidgetNode>();

            public LayoutController(
                TreeController treeController,
                BuildContext buildContext,
                WidgetNode referenceWidgetNode = null,
                bool rebuild = false)
            {
                this.treeController = treeController;
                this.buildContext = buildContext;

                referenceChildren = referenceWidgetNode?.Children?.ToList();
                this.rebuild = rebuild;
            }

            public IChildLayout LayoutChild(
                Widget child,
                BoxConstraints constraints,
                int index = -1)
            {
                WidgetNode childRef = null;
                if (referenceChildren != null)
                {
                    for (int i = 0; i < referenceChildren.Count; ++i)
                    {
                        var rc = referenceChildren[i];
                        if (rc == null)
                            continue;

                        if (child.CanReplace(rc.data.widget))
                        {
                            referenceChildren[i] = null;
                            childRef = rc;
                            break;
                        }
                    }
                }

                var childContext = buildContext.WithNewConstraints(constraints);
                WidgetNode node;

                if (rebuild 
                    && childRef != null
                    && childRef.data.context.Equals(childContext))
                    node = treeController.RebuildWidgetNode(childRef);
                else
                    node = treeController.BuildWidget(
                        child,
                        childContext,
                        childRef);

                if (index == -1)
                    index = childNodes.Count;
                childNodes.Insert(index, node);
                return node.data;
            }
        }
    }
}
