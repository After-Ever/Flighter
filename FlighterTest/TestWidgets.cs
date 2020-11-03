using System;
using System.Collections.Generic;
using System.Text;

using Flighter;

namespace FlighterTest
{
    public class TestDisplayWidget : DisplayWidget
    {
        public override bool CanReplace(Widget other)
        {
            return base.CanReplace(other);
        }

        public override Element CreateElement()
        {
            return new TestElement();
        }

        public override bool IsSame(Widget other)
        {
            return base.IsSame(other);
        }

        public override BuildResult Layout(BuildContext context, WidgetNode node)
        {
            return new BuildResult(10, 10);
        }
    }

    public class TestLayoutWidget : LayoutWidget
    {
        Widget left, right;

        public TestLayoutWidget(Widget left, Widget right)
        {
            this.left = left;
            this.right = right;
        }

        public override bool CanReplace(Widget other)
        {
            return base.CanReplace(other);
        }

        public override bool IsSame(Widget other)
        {
            return base.IsSame(other);
        }

        public override BuildResult Layout(BuildContext context, WidgetNode node)
        {
            BuildResult l = new BuildResult(0,0), r = new BuildResult(0,0);
            if (left != null)
                l = node.Add(left, context).BuildResult;
            if (right != null)
                r = node.Add(right, context).BuildResult;

            float x = Math.Max(l.size.x, r.size.x);
            float y = Math.Max(l.size.y, r.size.y);

            return new BuildResult(x, y);
        }
    }

    public class TestStatelessWidget : StatelessWidget
    {
        public override Widget Build(BuildContext context)
        {
            return new TestDisplayWidget();
        }

        public override bool CanReplace(Widget other)
        {
            return base.CanReplace(other);
        }

        public override bool IsSame(Widget other)
        {
            return base.IsSame(other);
        }
    }

    public class TestStatefulWidget : StatefullWidget
    {
        public override bool CanReplace(Widget other)
        {
            return base.CanReplace(other);
        }

        public override State CreateState()
        {
            return new TestState();
        }

        public override bool IsSame(Widget other)
        {
            return base.IsSame(other);
        }
    }

    public class TestState : State
    {
        public override Widget Build(BuildContext context)
        {
            return new TestDisplayWidget();
        }
    }
}
