using Microsoft.VisualStudio.TestTools.UnitTesting;

using Flighter;
using System;

namespace FlighterTest
{
    [TestClass]
    public class WidgetNodeTest
    {
        WidgetNode makeSimpleRoot()
            => RootWidget.MakeRootWidgetNode(new TestDisplayWidget(), new BuildContext(), null);

        [TestMethod]
        /// <summary>
        /// Bare-bones test. Root element and test display widget.
        /// </summary>
        public void SimpleTest()
        {
            makeSimpleRoot();
        }

        [TestMethod]
        /// <summary>
        /// After instrumenting a widget, the build result is
        /// inflated as expected.
        /// </summary>
        public void GetBuildResults()
        {
            var root = makeSimpleRoot();

            Assert.AreEqual(new BuildResult(10, 10), root.BuildResult);
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
