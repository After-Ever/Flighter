using Flighter;
using System;
using System.Collections.Generic;

namespace FlighterTest
{
    public static class TestUtilities
    {
        public static Root MakeTestRoot(Widget child)
            => new Root(
                child,
                new BuildContext(),
                new TestDisplayRect(),
                new ComponentProvider(new Dictionary<Type, Type>()),
                null);

        public static WidgetNode MakeSimpleRootWidgetNode()
            => new Root(
                new TestDisplayWidget(),
                new BuildContext(),
                new TestDisplayRect(),
                new ComponentProvider(new Dictionary<Type, Type>()),
                null)
                    .rootWidgetNode;

        public static ElementNode MakeSimpleRootElementNode()
            => null;// TODO: Fix this

        public static ElementNode MakeSimpleElementNode()
            => new ElementNode(new TestElement(), null);
    }
}
