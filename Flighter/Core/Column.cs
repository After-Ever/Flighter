using System;
using System.Collections.Generic;
using System.Text;

namespace Flighter.Core
{
    public class Column : StatelessWidget
    {
        public readonly List<Widget> children;
        public readonly HorizontalDirection horizontalDirection;
        public readonly VerticalDirection verticalDirection;
        public readonly MainAxisAlignment mainAxisAlignment;
        public readonly CrossAxisAlignment crossAxisAlignment;
        public readonly MainAxisSize mainAxisSize;

        public Column(
            List<Widget> children,
            HorizontalDirection horizontalDirection = HorizontalDirection.LeftToRight,
            VerticalDirection verticalDirection = VerticalDirection.TopToBottom,
            MainAxisAlignment mainAxisAlignment = MainAxisAlignment.Start,
            CrossAxisAlignment crossAxisAlignment = CrossAxisAlignment.Start,
            MainAxisSize mainAxisSize = MainAxisSize.Max)
        {
            this.children = children;
            this.horizontalDirection = horizontalDirection;
            this.verticalDirection = verticalDirection;
            this.mainAxisAlignment = mainAxisAlignment;
            this.crossAxisAlignment = crossAxisAlignment;
            this.mainAxisSize = mainAxisSize;
        }

        public override Widget Build(BuildContext context)
            => new SequenceLayout(
                children,
                Axis.Vertical,
                horizontalDirection,
                verticalDirection,
                mainAxisAlignment,
                crossAxisAlignment,
                mainAxisSize);
    }
}
