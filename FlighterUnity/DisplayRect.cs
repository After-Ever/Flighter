using System;
using System.Collections.Generic;
using System.Text;

using Flighter;

namespace FlighterUnity
{
    public class DisplayRect : IDisplayRect
    {
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Size Size { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Point Offset { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
            throw new NotImplementedException();
        }

        public bool RemoveComponent(Component component)
        {
            throw new NotImplementedException();
        }

        public void SetParent(IDisplayRect rect)
        {
            throw new NotImplementedException();
        }

        public void TearDown()
        {
            throw new NotImplementedException();
        }
    }
}
