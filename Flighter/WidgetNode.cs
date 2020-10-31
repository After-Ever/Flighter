using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    public class WidgetNode
    {
        public readonly Widget Widget;

        /// <summary>
        /// The BuildContext with which this node was built.
        /// </summary>
        public readonly BuildContext BuildContext;

        /// <summary>
        /// The result of building this node.
        /// </summary>
        public BuildResult BuildResult { get; private set; }

        ElementNode elementNode;

        readonly WidgetTree tree;
        WidgetNode parent;
        List<WidgetNode> children = new List<WidgetNode>();

        Queue<WidgetNode> inheritedChildren;

        bool hasConstructed = false;

        public WidgetNode(
            Widget widget, 
            BuildContext buildContext, 
            WidgetTree tree,
            WidgetNode parent, 
            ElementNode inheritedElementNode, 
            Queue<WidgetNode> inheritedChildren = null)
        {
            this.Widget = widget ?? throw new ArgumentNullException("WidgetNode's widget must not be null.");
            this.BuildContext = buildContext;
            this.tree = tree ?? throw new ArgumentNullException("WidgetNode must be created with a tree.");
            this.parent = parent;

            this.inheritedChildren = inheritedChildren;

            if (Widget is StatelessWidget slw)
            {
                if (inheritedElementNode != null)
                    throw new Exception("StatelessWidget cannot inherit an element node!");

                var child = slw.Build(buildContext);
                var childNode = Add(child, buildContext);

                BuildResult = childNode.BuildResult;
            }
            else if (Widget is StatefullWidget sfw)
            {
                if (inheritedElementNode != null)
                {
                    ConnectInheritedElementNode(inheritedElementNode);
                    // TODO: Need to have inherited state re build...
                    //       Look into the above method, and see where
                    //       it could tell the state element to do its thing.
                    // TODO: Consider the clean/dirty state of the added elements after an update.
                    //       It need be certain that everything is clean afterwards.
                }
                else
                {
                    var state = sfw.CreateState();
                    ConnectElement(new StateElement(state));
                }

                BuildResult = children[0].BuildResult;
            }
            else if (Widget is LayoutWidget lw)
            {
                if (lw is DisplayWidget dw)
                {
                    if (inheritedElementNode != null)
                    {
                        ConnectInheritedElementNode(inheritedElementNode);
                    }
                    else
                    {
                        ConnectElement(dw.CreateElement());
                    }
                }

                BuildResult = lw.Layout(buildContext, this);
            }

            // Remove any inherited children which have not been used.
            // (This is a very sad bit of code...)
            foreach(var c in this.inheritedChildren)
            {
                // Goodnight sweet prince...
                c.Prune();
            }

            inheritedChildren = null;
            hasConstructed = true;
        }

        /// <summary>
        /// Add a child widget to this.
        /// All additions must occure durring the constructor; after construction,
        /// the sub tree is final.
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public WidgetNode Add(Widget widget, BuildContext context)
        {
            if (hasConstructed)
                throw new Exception("Cannot add children after the node has been constructed!");

            ElementNode childElementNode = null;
            Queue<WidgetNode> orphans = null;

            try
            {
                var c = inheritedChildren.Dequeue();
                // The context and widget are the same, so no need to add the new widget.
                if (context == c.BuildContext && widget.IsSame(c.Widget))
                    return c;
                if (widget.CanReplace(c.Widget))
                {
                    childElementNode = c.DisconnectElementNode();
                    orphans = c.PruneAndAdopt();
                }

                c.Prune();
            }
            catch (InvalidOperationException) { }

            WidgetNode child = new WidgetNode(
                                        widget,
                                        context,
                                        tree,
                                        this,
                                        childElementNode,
                                        orphans);

            children.Add(child);
            return child;
        }

        /// <summary>
        /// Disconnect this tree from the element tree.
        /// If this node is itself connected, then that connection is broken.
        /// Otherwise, this is called on it's children.
        /// </summary>
        /// <returns>The disconnected nodes.</returns>
        ElementNode DisconnectElementNode()
        {
            if (elementNode != null)
            {
                elementNode.Emancipate();
                return  elementNode;
            }
            return null;
        }

        /// <summary>
        /// Connect the given element node to this.
        /// </summary>
        /// <param name="node"></param>
        void ConnectInheritedElementNode(ElementNode node)
        {
            var nearestAncestor = NearestAncestorElementNode();
            elementNode = node;
            nearestAncestor.ConnectNode(elementNode);
            elementNode.Element.UpdateWidgetNode(this);
        }

        /// <summary>
        /// Connect the given element to this node, adding it to the element tree.
        /// </summary>
        /// <param name="element"></param>
        void ConnectElement(Element element)
        {
            element.UpdateWidgetNode(this);
            var nearestNode = NearestAncestorElementNode();
            elementNode = nearestNode.AddChild(element);
        }

        /// <summary>
        /// Remove this node and all children.
        /// This will deconstruct any attached elements as well.
        /// </summary>
        void Prune()
        {
            Emancipate();

            // We must clean up any element nodes attached to this tree.
            // Pruning an elementNode will also clear all children, so we can
            // stop this traversal once we hit such a node.
            if (elementNode != null)
            {
                elementNode.Prune();
            }
            else
            {
                // Pruning removes from this list, so we don't enumerate.
                while (children.Count > 0)
                {
                    children[0].Prune();
                }
            }
        }

        /// <summary>
        /// Remove this node, but leave the children orphans.
        /// This node's elements will be deconstructed, but children will be left alone.
        /// </summary>
        /// <returns></returns>
        Queue<WidgetNode> PruneAndAdopt()
        {
            // Take out all the children.
            Queue<WidgetNode> emancipatedChildren = new Queue<WidgetNode>();
            while(children.Count > 0)
            {
                var c = children[0];
                c.Emancipate();
                emancipatedChildren.Enqueue(c);
            }

            // Then prune.
            Prune();

            return emancipatedChildren;
        }

        /// <summary>
        /// Remove this from its parent.
        /// Does NOT disconnect an attached element from the element tree.
        /// </summary>
        void Emancipate()
        {
            parent?.children?.Remove(this);
            parent = null;
        }

        /// <summary>
        /// Find the nearest ancestor with an attached
        /// element node. Returns attached element node if this has one.
        /// </summary>
        /// <returns>The found node if one exists, null otherwise.</returns>
        ElementNode NearestAncestorElementNode()
        {
            if (elementNode != null) return elementNode;
            return parent?.NearestAncestorElementNode();
        }
    }
}
