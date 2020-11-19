using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;

using Flighter;

namespace FlighterTest
{
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

            testElement.Init(new TestDisplayRect(), new ComponentProvider(new Dictionary<Type, Type>()));

            // Set to initialized.
            Assert.IsTrue(testElement.IsInitialized);
            // Internal init method called.
            Assert.IsTrue(testElement.InitCalled);
            // Update has not been called yet.
            Assert.IsFalse(testElement.UpdateCalled);
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

            var r = new TestDisplayRect();
            var c = new ComponentProvider(new Dictionary<Type, Type>());

            testElement.Init(r, c);
            
            Assert.IsFalse(testElement.UpdateCalled);

            testElement.Update();

            Assert.IsTrue(testElement.UpdateCalled);
        }

        [TestMethod]
        public void UpdateWidgetNode()
        {
            var e = new TestElement();
            var n = TestUtilities.MakeSimpleRootWidgetNode();

            Assert.IsNull(e.WidgetNode);

            e.UpdateWidgetNode(n);

            Assert.AreSame(n, e.WidgetNode);
        }

        [TestMethod]
        public void GetWidget()
        {
            var e = new TestElement();
            var n = TestUtilities.MakeSimpleRootWidgetNode();
            var w = n.widget;

            Assert.IsNull(e.GetWidget<RootWidget>());

            e.UpdateWidgetNode(n);
            
            Assert.AreSame(w, e.GetWidget<RootWidget>());
        }

        [TestMethod]
        public void Teardown()
        {
            var e = new TestElement();
            var r = new TestDisplayRect();
            var c = new ComponentProvider(new Dictionary<Type, Type>());

            e.Init(r, c);
            e.TearDown();

            Assert.IsFalse(e.IsInitialized);
            Assert.IsNull(e.WidgetNode);
            Assert.IsTrue(r.WasTornDown);
        }
    }
}
