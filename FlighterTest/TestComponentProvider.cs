using System;
using System.Collections.Generic;
using System.Text;

using Flighter;

namespace FlighterTest
{
    public class TestComponentProvider : IComponentProvider
    {
        public C CreateComponent<C>() where C : IComponent
        {
            throw new NotImplementedException();
        }
    }
}
