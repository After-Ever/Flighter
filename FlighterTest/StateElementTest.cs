using Flighter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlighterTest
{
    class TestStateElement : StateElement
    {
        public TestStateElement(State state)
            : base(state) { }

        public void WrapSetStateCallback(Action callFirst)
        {
            var original = setDirty;
            setDirty = () =>
            {
                callFirst();
                original?.Invoke();
            };
        }
    }

    [TestClass]
    public class StateElementTest
    {
        [TestMethod]
        public void SettingStateMarksElementDirty()
        {
            var s = new TestState();
            var se = new TestStateElement(s);

            var r = TestUtilities.MakeSimpleRootElementNode();
            var sn = r.AddChild(se);

            var setDirty = false;

            se.WrapSetStateCallback(() => setDirty = true);

            s.SetState(null);

            Assert.IsTrue(setDirty);
        }

        [TestMethod]
        public void UpdateCallsActions()
        {
            bool called = false;
            
            var s = new TestState();
            s.SetState(() => called = true);

            var se = new StateElement(s);

            var r = TestUtilities.MakeSimpleRootElementNode();
            var sn = r.AddChild(se);

            Assert.ThrowsException<NullReferenceException>(() =>
            {
                // This will throw because the Element in not connected to a widget.
                r.Update();
            });

            Assert.IsTrue(called);
        }
    }
}
