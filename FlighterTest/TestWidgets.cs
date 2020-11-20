using System;
using System.Collections.Generic;
using System.Text;

using Flighter;

namespace FlighterTest
{
    public class TestDisplayWidget : DisplayWidget
    {
        public override Element CreateElement()
        {
            return new TestElement();
        }

        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            return new BuildResult(10, 10);
        }
    }

    public class TestLayoutWidget : LayoutWidget
    {
        readonly Widget left, right;

        public TestLayoutWidget(Widget left, Widget right)
        {
            this.left = left;
            this.right = right;
        }

        public override BuildResult Layout(BuildContext context, WidgetNodeBuilder node)
        {
            Size l, r;
            l = r = Size.Zero;
            if (left != null)
                l = node.AddChildWidget(left, context).size;
            if (right != null)
                r = node.AddChildWidget(right, context).size;

            float width = System.Math.Max(l.width, r.width);
            float height = System.Math.Max(l.height, r.height);

            return new BuildResult(width, height);
        }
    }

    public class TestStatelessWidget : StatelessWidget
    {
        readonly Widget child;

        public TestStatelessWidget(Widget child = null)
        {
            this.child = child;
        }

        public override Widget Build(BuildContext context)
        {
            return child ?? new TestDisplayWidget();
        }
    }

    public class TestStatefulWidget : StatefulWidget
    {
        public readonly Widget child;

        public State state { get; private set; }

        public TestStatefulWidget(Widget child = null)
        {
            this.child = child;
        }

        public override State CreateState()
        {
            return state = new TestState();
        }
    }

    public class TestState : State
    {
        public new W GetWidget<W>() where W : Widget => base.GetWidget<W>();
        public new void SetState(Action action) => base.SetState(action);

        public override Widget Build(BuildContext context)
        {
            var w = GetWidget<TestStatefulWidget>();

            return w?.child ?? new TestDisplayWidget();
        }
    }
}
