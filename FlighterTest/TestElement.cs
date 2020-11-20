using Flighter;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlighterTest
{
    class TestElement : Element
    {
        public WidgetNode WidgetNode => widgetNode;

        public bool InitCalled { get; private set; } = false;
        public bool UpdateCalled { get; private set; } = false;

        public override string Name => "TestElement";

        public void Reset()
        {
            InitCalled = UpdateCalled = false;
        }

        /// <summary>
        /// Set the element dirty.
        /// </summary>
        public void SetDirty()
        {
            if (setDirty == null)
                throw new Exception("Element has not been given a setDirty callback.");

            setDirty();
        }

        protected override void _Init() { InitCalled = true; }
        protected override void _Update() { UpdateCalled = true; }
    }
}
