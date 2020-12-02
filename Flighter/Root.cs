using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    public class Root
    {
        RootWidget rootWidget;
        public WidgetNode rootWidgetNode { get; private set; }
        public ElementNode rootElementNode { get; private set; }

        public Root(
            Widget widget,
            BuildContext initialBuildContext,
            IDisplayRect rootRect,
            ComponentProvider componentProvider,
            Input.Input input)
        {
            rootWidget = new RootWidget(
                widget ?? throw new ArgumentNullException(),
                initialBuildContext);

            rootElementNode = new RootElementNode(
                rootRect ?? throw new ArgumentNullException(), 
                componentProvider ?? throw new ArgumentNullException());

            rootWidgetNode = new WidgetNodeBuilder(
                new WidgetForest(),
                rootWidget,
                initialBuildContext,
                rootElementNode).Build(null);
        }

        public void UpdateBuildContext(BuildContext buildContext)
        {
            if (rootWidgetNode == null)
                throw new Exception("Root has been disposed");

            rootWidget.buildContext = buildContext;
            rootWidgetNode.Rebuild();
        }

        public void Update()
        {
            if (rootWidgetNode == null)
                throw new Exception("Root has been disposed");

            //if (rootElementNode.IsDirty)
            {
                // Manually update the root element, as the root widget node will never be replaced.
                rootElementNode.Update(rootWidgetNode);
                rootWidgetNode.Rebuild();
            }
        }

        public void Dispose()
        {
            rootWidgetNode?.Dispose();

            rootWidgetNode = null;
            rootElementNode = null;
        }
    }

    // This dummy display widget is necessary so that it can be paired with the root element.
    class RootWidget : DisplayWidget
    {
        // Manually use this build context.
        // This is very much not the proper way to hold state, and is only done here to handle the special root case.
        // If one needs to update state, they must use a StatefulWidget.
        public BuildContext buildContext;

        readonly Widget child;

        public RootWidget(Widget child, BuildContext buildContext)
        {
            this.child = child;
            this.buildContext = buildContext;
        }

        public override Element CreateElement()
        {
            throw new NotImplementedException("Root widget should never have to make an element.");
        }

        public override BuildResult Layout(BuildContext _, WidgetNodeBuilder node)
        {
            var c = node.AddChildWidget(child, buildContext);
            return new BuildResult(c.size);
        }
    }
    
    class RootElement : Element
    {
        public override string Name => "Root";

        protected override void _Init() { }

        protected override void _Update() { }
    }

    /// <summary>
    /// The root of an element tree.
    /// </summary>
    class RootElementNode : ElementNode
    {
        readonly IDisplayRect parent;
        readonly ComponentProvider componentProvider;

        /// <param name="parent">The parent object of
        /// the tree. This object will not be modified,
        /// and should not be modified externally.</param>
        public RootElementNode(IDisplayRect parent, ComponentProvider componentProvider)
            : base(new RootElement(), null, componentProvider)
        {
            this.parent = parent;
            this.componentProvider = componentProvider;
        }

        // Overriding lets us avoid not having a parent,
        // by manually passing the rectTransform.
        internal override void InitOrConnectElement()
        {
            element.Init(parent, componentProvider);
        }
    }
}
