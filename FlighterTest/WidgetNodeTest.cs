using Microsoft.VisualStudio.TestTools.UnitTesting;

using Flighter;
using System;

namespace FlighterTest
{
    [TestClass]
    public class WidgetNodeTest
    {
        [TestMethod]
        /// <summary>
        /// Bare-bones test. Root element and test display widget.
        /// </summary>
        public void SimpleTest()
        {
            TestUtilities.MakeSimpleRootWidgetNode();
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
                new TestDisplayRect(),
                new ComponentProvider(new System.Collections.Generic.Dictionary<Type, Type>()),
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
            var root = TestUtilities.MakeSimpleRootWidgetNode();

            Assert.AreEqual(new Size(10, 10), root.Size);
        }

        [TestMethod]
        public void ReplaceChild()
        {
            var root = TestUtilities.MakeSimpleRootWidgetNode();
            root.ReplaceChildren(new System.Collections.Generic.List<(Widget, BuildContext)>
            {
                (new TestDisplayWidget(), new BuildContext()),
            });
        }
    }
}
