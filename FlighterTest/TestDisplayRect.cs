using System;
using System.Collections.Generic;
using System.Text;

using Flighter;
using Component = Flighter.Component;

namespace FlighterTest
{
    public class TestDisplayRect : IDisplayRect
    {
        public string Name { get; set; }
        public Size Size { get; set; }
        public Point Offset { get; set; }

        public void AddComponent(Component component)
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

        public bool RemoveComponent(Flighter.Component component)
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
