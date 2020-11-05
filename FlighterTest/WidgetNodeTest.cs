using Microsoft.VisualStudio.TestTools.UnitTesting;

using Flighter;
using System;

namespace FlighterTest
{
    [TestClass]
    public class WidgetNodeTest
    {
        WidgetNode makeSimpleRoot()
            => RootWidget.MakeRootWidgetNode(
                new TestDisplayWidget(), 
                new BuildContext(), 
                null)
                    .Item1;

        [TestMethod]
        /// <summary>
        /// Bare-bones test. Root element and test display widget.
        /// </summary>
        public void SimpleTest()
        {
            makeSimpleRoot();
        }

        [TestMethod]
        public void ComplexTest()
        {
            // Make a big messy widget.
            Widget w = new TestLayoutWidget(
                left: new TestStatelessWidget(
                    child: new TestDisplayWidget()),
                right: new TestLayoutWidget(
                    left: new TestLayoutWidget(
                        left: new TestDisplayWidget(),
                        right: new TestStatelessWidget(
                            child: new TestDisplayWidget())),
                    right: new TestDisplayWidget()));
            
            (var root, var elementNode) = RootWidget.MakeRootWidgetNode(
                w,
                new BuildContext(),
                null);

            // Update the element tree.
            elementNode.Update();

            Assert.IsFalse(elementNode.IsDirty || elementNode.HasDirtyChild);
        }

        [TestMethod]
        /// <summary>
        /// After instrumenting a widget, the build result is
        /// inflated as expected.
        /// </summary>
        public void GetBuildResults()
        {
            var root = makeSimpleRoot();

            Assert.AreEqual(new UnityEngine.Vector2(10,10), root.Layout.size);
        }

        [TestMethod]
        public void CannotAddUnlessConstructing()
        {
            var root = makeSimpleRoot();

            Assert.ThrowsException<Exception>(() => root.Add(new TestDisplayWidget(), new BuildContext()));
        }

        [TestMethod]
        public void ReplaceChild()
        {
            var root = makeSimpleRoot();
            root.ReplaceChildren(new System.Collections.Generic.List<(Widget, BuildContext)>
            {
                (new TestDisplayWidget(), new BuildContext()),
            });
        }
    }
}
