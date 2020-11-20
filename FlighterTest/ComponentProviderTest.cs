using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

using Flighter;

namespace FlighterTest
{
    public class TestColorComponent : Flighter.Core.ColorComponent
    {
        public override Flighter.Core.Color Color { get; set; }
    }

    public class FailTestColorComponent : Flighter.Core.ColorComponent
    {
        public override Flighter.Core.Color Color { get; set; }

        public FailTestColorComponent(int arg) { }
    }

    [TestClass]
    public class ComponentProviderTest
    {
        [TestMethod]
        public void FailsToCreateComponent()
        {
            // Cannot create components which require arguments to their constructor.
            var provider = new ComponentProvider(new Dictionary<Type, Type>
            {
                {typeof(Flighter.Core.ColorComponent), typeof(FailTestColorComponent) }
            });

            Assert.ThrowsException<CannotCreateComponentException>(() =>
            {
                provider.CreateComponent<Flighter.Core.ColorComponent>();
            });

            // Cannot create unregistered component type.

            Assert.ThrowsException<CannotCreateComponentException>(() =>
            {
                provider.CreateComponent<Flighter.Core.TextComponent>();
            });
        }

        [TestMethod]
        public void ProvidesComponent()
        {
            var provider = new ComponentProvider(new Dictionary<Type, Type>
            {
                {typeof(Flighter.Core.ColorComponent), typeof(TestColorComponent) }
            });

            var c = provider.CreateComponent<Flighter.Core.ColorComponent>();

            Assert.IsNotNull(c);
        }
    }
}
