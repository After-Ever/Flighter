using System;

using Flighter;

namespace FlighterTest
{
    public class TestDisplayRect : IDisplayRect
    {
        public string Name { get; set; }
        public Size Size { get; set; }
        public Point Offset { get; set; }

        public void AddComponent(IComponent component)
        {
            throw new NotImplementedException();
        }

        public void ClearComponents()
        {
            throw new NotImplementedException();
        }

        public IDisplayRect CreateChild()
        {
            return new TestDisplayRect();
        }

        public bool RemoveComponent(IComponent component)
        {
            return true;
        }

        public void SetParent(IDisplayRect rect)
        {
        }

        public void TearDown()
        {
        }
    }
}
