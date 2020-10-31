using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter
{
    public abstract class State
    {
        public abstract Widget Build(BuildContext context);

        protected void SetState(Action action)
        {
            throw new NotImplementedException();
        }
    }
}
