using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class ColoredBox : DisplayWidget
    {
        public override bool IsSame(Widget other)
        {
            throw new NotImplementedException();
        }

        public override Element CreateElement()
        {
            throw new NotImplementedException();
        }

        public override BuildResult Layout(BuildContext context, WidgetNode node)
        {
            throw new NotImplementedException();
        }
    }

    class ColoredBoxElement : Element
    {
        public override string name => "ColoredBox";

        protected override void _Init()
        {
            throw new NotImplementedException();
        }

        protected override void _Update()
        {
            throw new NotImplementedException();
        }
    }
}
