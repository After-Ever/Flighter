using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class Builder : StatelessWidget
    {
        readonly WidgetBuilder builder;

        public Builder(WidgetBuilder builder, string key = null)
            : base(key)
        {
            this.builder = builder;
        }

        public override Widget Build(BuildContext context)
            => builder(context);
    }
}
