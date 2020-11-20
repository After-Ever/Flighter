using Microsoft.VisualStudio.TestTools.UnitTesting;

using Flighter;
using System;

namespace FlighterTest
{
    [TestClass]
    public class WidgetNodeTest
    {
        [TestMethod]
        public void StatePreservedOnReplacement()
        {
            var child = new TestStatefulWidget();
            var parent = new TestStatefulWidget(child);
            
            (var rootWidgetNode, var rootElementNode) = TestUtilities.MakeTestRoot(parent);

            rootElementNode.Update();

            (parent.state as TestState).SetState(null);

            rootElementNode.Update();

            // Child should no longer be apart of the tree,
            // but its original state should remain. (And all the element stuff, but new widget node).
        }
    }
}
