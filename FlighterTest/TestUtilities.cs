using Flighter;
using System;
using System.Collections.Generic;

namespace FlighterTest
{
    public static class TestUtilities
    {
        public static WidgetNode MakeSimpleRootWidgetNode()
            => RootWidget.MakeRootWidgetNode(
                new TestDisplayWidget(),
                new BuildContext(),
                new TestDisplayRect(),
                new ComponentProvider(new Dictionary<Type, Type>()),
                null)
                    .Item1;

        public static ElementNode MakeSimpleRootElementNode()
            => new RootElementNode(
                new TestDisplayRect(), 
                new ComponentProvider(
                    new Dictionary<Type, Type>()));

        public static ElementNode MakeSimpleElementNode()
            => new ElementNode(new TestElement(), null);
    }
}
