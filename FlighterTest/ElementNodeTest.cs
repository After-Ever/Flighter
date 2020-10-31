using Microsoft.VisualStudio.TestTools.UnitTesting;

using Flighter;

namespace FlighterTest
{
    [TestClass]
    public class ElementNodeTest
    {
        [TestMethod]
        public void AddingChildren()
        {
            // Make the root.
            var root = new RootElementNode(null);

            // Add children.
            var c1 = root.AddChild(new TestElement());
            var c2 = root.AddChild(new TestElement());

            // Add grandchildren.
            var g1 = c1.AddChild(new TestElement());
            var g2 = c1.AddChild(new TestElement());
            var g3 = c2.AddChild(new TestElement());
            var g4 = c2.AddChild(new TestElement());

            Assert.AreEqual(
                "Root\n-TestElement\n--TestElement\n--TestElement\n-TestElement\n--TestElement\n--TestElement\n", 
                root.Print());
        }
    }
}
