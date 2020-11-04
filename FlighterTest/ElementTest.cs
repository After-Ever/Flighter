using Microsoft.VisualStudio.TestTools.UnitTesting;

using Flighter;

namespace FlighterTest
{
    class TestElement : Element
    {
        public bool InitCalled { get; private set; } = false;
        public bool UpdateCalled { get; private set; } = false;
        public bool ClearCalled { get; private set; } = false;

        public override string Name => "TestElement";
        
        protected override void _Init() { InitCalled = true; }
        protected override void _Update() { UpdateCalled = true; }
    }

    [TestClass]
    public class ElementTest
    {
        [TestMethod]
        public void Initializes()
        {
            var testElement = new TestElement();

            // Starts uninitialized.
            Assert.IsFalse(testElement.IsInitialized);
            Assert.IsFalse(testElement.InitCalled);
            Assert.IsFalse(testElement.UpdateCalled);
            Assert.IsFalse(testElement.ClearCalled);

            testElement.Init(null);

            // Set to initialized.
            Assert.IsTrue(testElement.IsInitialized);
            // Internal init method called.
            Assert.IsTrue(testElement.InitCalled);
        }

        [TestMethod]
        public void CallsUpdate()
        {
            var testElement = new TestElement();

            // Should throw if update is called before Init.
            Assert.ThrowsException<ElementUninitializedException>(
                () => testElement.Update());
            // Internal method should remain uncalled.
            Assert.IsFalse(testElement.UpdateCalled);

            testElement.Init(null);

            testElement.Update();

            Assert.IsTrue(testElement.UpdateCalled);
        }
    }
}
