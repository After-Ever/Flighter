using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace Flighter.Core
{
    public class FlexPadding : LayoutWidget
    {
        public readonly Widget child;
        public readonly EdgeInsets edgeFlex;

        public FlexPadding(Widget child, EdgeInsets edgeFlex, string key = null)
            : base(key)
        {
            this.child = child ?? throw new ArgumentNullException("EdgePadding must have a child.");
            this.edgeFlex = edgeFlex;
        }

        public override Size Layout(BuildContext context, ILayoutController layout)
        {
            if (context.constraints.IsUnconstrained)
                throw new Exception("Context must be constrained for FlexPadding.");

            var totalHorizontalFlex = edgeFlex.left + edgeFlex.right + 1;
            var totalVerticalFlex = edgeFlex.top + edgeFlex.bottom + 1;

            var horizontalPerFlex = context.constraints.maxWidth / totalHorizontalFlex;
            var verticalPerFlex = context.constraints.maxHeight / totalVerticalFlex;

            var childLayout = layout.LayoutChild(child, new BoxConstraints(0, horizontalPerFlex, 0, verticalPerFlex));
            childLayout.offset = new Vector2(edgeFlex.left * horizontalPerFlex, edgeFlex.top * verticalPerFlex);

            return context.constraints.MaxSize;
        }
    }
}
