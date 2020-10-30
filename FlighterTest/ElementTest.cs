using Microsoft.VisualStudio.TestTools.UnitTesting;

using Flighter;

namespace FlighterTest
{
    class TestElement : Element
    {
        public bool InitCalled { get; private set; } = false;

        public override string Name => "TestElement";

        protected override void _Clear() { }
        protected override void _Init() { InitCalled = true; }
        protected override void _Update() { }
    }

    [TestClass]
    public class ElementTest
    {
        [TestMethod]
        public void ElementInitializes()
        {
            var testElement = new TestElement();

            // Starts uninitialized.
            Assert.IsFalse(testElement.IsInitialized);
            Assert.IsFalse(testElement.InitCalled);

            testElement.Init(null);

            // Set to initialized.
            Assert.IsTrue(testElement.IsInitialized);
            // Internal init method called.
            Assert.IsTrue(testElement.InitCalled);
        }
    }
}
