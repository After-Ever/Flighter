using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class Positioned : DisplayWidget
    {
        public override Element CreateElement()
        {
            throw new NotImplementedException();
        }

        public override bool IsSame(Widget other)
        {
            return base.IsSame(other);
        }

        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            throw new NotImplementedException();
        }
    }
}
