using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class Builder : StatelessWidget
    {
        readonly WidgetBuilder builder;

        public Builder(WidgetBuilder builder)
        {
            this.builder = builder;
        }

        public override Widget Build(BuildContext context)
            => builder(context);
    }
}
