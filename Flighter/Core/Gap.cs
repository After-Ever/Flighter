using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class HorizontalGap : StatelessWidget
    {
        readonly float size;

        public HorizontalGap(float size)
        {
            this.size = size;
        }

        public override Widget Build(BuildContext context)
            => new BoxConstrained(
                constraints: new BoxConstraints(maxWidth: size, maxHeight: 0),
                child: new EmptyBox());
    }
    public class VerticalGap : StatelessWidget
    {
        readonly float size;

        public VerticalGap(float size)
        {
            this.size = size;
        }

        public override Widget Build(BuildContext context)
            => new BoxConstrained(
                constraints: new BoxConstraints(maxHeight: size, maxWidth: 0),
                child: new EmptyBox());
    }
}
