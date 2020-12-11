using Flighter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlighterTest
{
    [TestClass]
    public class StateTest
    {
        [TestMethod]
        public void ActionsAreCalled()
        {
            bool called = false;

            var s = new TestState();
            s.SetState(() => called = true);

            Assert.IsFalse(called);

            s.InvokeUpdates();

            Assert.IsTrue(called);
        }

        [TestMethod]
        public void ActionsCanAddActions()
        {
            var s = new TestState();

            int timesRun = 0;
            Action ss = null;
            ss = () =>
            {
                timesRun++;
                s.SetState(ss);
            };

            s.SetState(ss);
            s.InvokeUpdates();

            Assert.AreEqual(1, timesRun);
            s.InvokeUpdates();
            Assert.AreEqual(2, timesRun);
        }

        [TestMethod]
        public void GetsWidget()
        {
            var s = new TestState();

            Assert.ThrowsException<Exception>(() => s.GetWidget<Widget>());

            var se = new StateElement(s);
            se.UpdateWidgetNode(TestUtilities.MakeSimpleRootWidgetNode());

            Assert.IsNotNull(s.GetWidget<Widget>());
        }
    }
}
