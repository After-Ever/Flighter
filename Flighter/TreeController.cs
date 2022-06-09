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
                rootContext,
                stateToRebuild);

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
                    iw.OnUpdate(e);

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
                    
                    // Check if root input node.
                    if (node.data.widget == null)
                    {
                        return
                           mousePos.X >= topLeft.X
                        && mousePos.Y >= topLeft.Y
                        && mousePos.X < botRight.X
                        && mousePos.Y < botRight.Y;
                    }

                    return node.data.widget.IsHovering(mousePos, topLeft, botRight);
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

        void DoStateUpdates()
        {
            if (stateToRebuild.Count > 0)
            {
                var toRebuild = widgetTree.Children[0];
                toRebuild.Emancipate();

                widgetTree.AddChild(RebuildWidgetNode(toRebuild, stateToRebuild));
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

                db.DisplayRect.SetParent(parentRect);
                db.Update();

                displayOffset = Vector2.Zero;
                parentRect = db.DisplayRect;
                displayInTree.Add(db);
            }

            var state = node.data.state;
            if (state != null)
                stateInTree.Add(state);

            if (node.data.widget is InputWidget iw)
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

        static WidgetNode BuildWidget(
            Widget widget,
            BuildContext context,
            HashSet<State> stateToRebuild,
            WidgetNode referenceWidgetNode = null)
        {
            if (widget == null)
                throw new Exception("Widget cannot be null!!");

            if (!widget.CanReplace(referenceWidgetNode?.data?.widget))
                referenceWidgetNode = null;

            switch (widget)
            {
                case LayoutWidget lw:
                    {
                        var lc = new LayoutController(
                            context,
                            stateToRebuild,
                            referenceWidgetNode);

                        var size = lw.Layout(context, lc);
                        var children = lc.GetChildren();

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

                        node.data.size = size;
                        foreach (var c in children)
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
                                s => stateToRebuild?.Add(s));
                        }
                        else
                        {
                            if (referenceWidgetNode.data.state == null)
                                throw new Exception("Stateful widgets node data does not contain state.");
                            state = referenceWidgetNode.data.state;

                            state.InvokeUpdates();
                            stateToRebuild?.Remove(state);
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
                            stateToRebuild,
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
                            stateToRebuild,
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
                            stateToRebuild,
                            referenceWidgetNode);
                        return node;
                    }
                default:
                    throw new NotSupportedException("Unhandled widget type: "
                        + widget.GetType().Name);
            }
        }

        static void SetUpSingleChildNode(
            WidgetNode node,
            Widget child,
            BuildContext context,
            HashSet<State> stateToRebuild,
            WidgetNode referenceWidgetNode)
        {

            if (referenceWidgetNode != null
                && referenceWidgetNode.Children.Count != 1)
                throw new Exception("Non-layout widgets must have exactly one child.");

            var childNode = BuildWidget(
                child,
                context,
                stateToRebuild,
                referenceWidgetNode?.Children?.First());
            node.data.size = childNode.data.size;
            node.AddChild(childNode);
        }

        static WidgetNode RebuildWidgetNode(
            WidgetNode node, 
            HashSet<State> stateToRebuild)
        {
            var newNode = new WidgetNode(node.data.RebuildCopy());

            if (newNode.data.widget is LayoutWidget lw)
            {
                var lc = new LayoutController(
                            node.data.context,
                            stateToRebuild,
                            node,
                            true);

                newNode.data.size = lw.Layout(node.data.context, lc);
                foreach (var c in lc.GetChildren())
                    newNode.AddChild(c);

                return newNode;
            }

            if (node.Children.Count != 1)
                throw new Exception("Non-layout widget must have exactly one child.");

            var oldChildNode = node.Children.First();

            WidgetNode childNode;
            if (newNode.data.state != null 
                && (stateToRebuild?.Remove(newNode.data.state) ?? false))
            {
                if (!(newNode.data.widget is StatefulWidget))
                    throw new Exception("Non-statefulWidget has attached state!");

                newNode.data.state.InvokeUpdates();

                childNode = BuildWidget(
                    newNode.data.state.Build(newNode.data.context),
                    newNode.data.context,
                    stateToRebuild,
                    oldChildNode);
            }
            else
                childNode = RebuildWidgetNode(oldChildNode, stateToRebuild);

            newNode.data.size = childNode.data.size;
            newNode.AddChild(childNode);
            return newNode;
        }

        class LayoutController : ILayoutController
        {
            readonly BuildContext buildContext;
            readonly List<WidgetNode> referenceChildren;
            readonly bool rebuild;

            readonly HashSet<State> stateToRebuild;

            readonly List<(WidgetNode w, int index)> childNodes
                = new List<(WidgetNode, int)>();

            public LayoutController(
                BuildContext buildContext,
                HashSet<State> stateToRebuild = null,
                WidgetNode referenceWidgetNode = null,
                bool rebuild = false)
            {
                this.buildContext = buildContext;
                this.stateToRebuild = stateToRebuild;
                referenceChildren = referenceWidgetNode?.Children?.ToList();
                this.rebuild = rebuild;
            }

            public List<WidgetNode> GetChildren()
            {
                var r = new List<WidgetNode>();
                var toSort = new List<(WidgetNode w, int index)>();
                foreach (var c in childNodes)
                {
                    if (c.index == -1)
                        r.Add(c.w);
                    else
                        toSort.Add(c);
                }

                toSort.Sort((a, b) => a.index - b.index);
                r.AddRange(toSort.Select(c => c.w));

                return r;
            }

            public IChildLayout LayoutChild(
                Widget child,
                BoxConstraints constraints,
                int index = -1)
            {
                WidgetNode childRef = GetChildRef(child, true);

                var childContext = buildContext.WithNewConstraints(constraints);
                WidgetNode node;

                if (rebuild
                    && childRef != null
                    && childRef.data.context.Equals(childContext))
                    node = RebuildWidgetNode(childRef, stateToRebuild);
                else
                    node = BuildWidget(
                        child,
                        childContext,
                        stateToRebuild,
                        childRef);

                childNodes.Add((node, index));
                return node.data;
            }

            public IChildLayout LayoutWithoutAttach(
                Widget child,
                BuildContext sandboxContext)
                => BuildWidget(
                    child, 
                    sandboxContext, 
                    null, 
                    GetChildRef(child, false)).data;

            WidgetNode GetChildRef(Widget child, bool removeRef)
            {
                if (referenceChildren == null)
                    return null;

                for (int i = 0; i < referenceChildren.Count; ++i)
                {
                    var rc = referenceChildren[i];
                    if (rc == null)
                        continue;

                    if (child.CanReplace(rc.data.widget))
                    {
                        if (removeRef)
                            referenceChildren[i] = null;
                        return rc;
                    }
                }

                return null;
            }
        }
    }
}
