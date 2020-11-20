using System;

using Flighter;

namespace FlighterTest
{
    public class TestDisplayRect : IDisplayRect
    {
        public string Name { get; set; }
        public Size Size { get; set; }
        public Point Offset { get; set; }

        public bool WasTornDown { get; private set; } = false;

        public void AddComponent(Component component)
        {
            throw new NotImplementedException();
        }

        public IDisplayRect CreateChild()
        {
            return new TestDisplayRect();
        }

        public void RemoveComponent(Component component)
        {
        }

        public void SetParent(IDisplayRect rect)
        {
        }

        public void TearDown()
        {
            WasTornDown = true;
        }
    }
}
