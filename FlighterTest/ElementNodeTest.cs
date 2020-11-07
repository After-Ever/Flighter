﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

using Flighter;

namespace FlighterTest
{
    [TestClass]
    public class ElementNodeTest
    {
        ElementNode MakeRoot() => new RootElementNode(new TestDisplayRect(), new TestComponentProvider());

        [TestMethod]
        public void UpdateInitializes()
        {
            TestElement e1, e2, e3;

            var root = MakeRoot();

            var c1 = root.AddChild(e1 = new TestElement());
            var c2 = root.AddChild(e2 = new TestElement());

            var g1 = c2.AddChild(e3 = new TestElement());

            root.Update();

            Assert.IsTrue(
                root.element.IsInitialized &&
                c1.element.IsInitialized &&
                c2.element.IsInitialized &&
                g1.element.IsInitialized);

            Assert.IsTrue(
                e1.InitCalled &&
                e2.InitCalled &&
                e3.InitCalled);
        }

        [TestMethod]
        public void UpdateOnlyAffectsDirtyNodes()
        {
            TestElement e1, e2, e3;

            var root = MakeRoot();

            var c1 = root.AddChild(e1 = new TestElement());
            var c2 = root.AddChild(e2 = new TestElement());

            var g1 = c2.AddChild(e3 = new TestElement());

            // Update to set all nodes clean.
            root.Update();

            e1.Reset();
            e2.Reset();
            e3.Reset();

            c2.SetDirty();

            root.Update();

            Assert.IsTrue(
                !e1.UpdateCalled &&
                e2.UpdateCalled &&
                !e3.UpdateCalled);
        }

        /// <summary>
        /// Tests that update leaves nodes clean.
        /// </summary>
        [TestMethod]
        public void UpdateCleans()
        {
            var root = MakeRoot();

            var c1 = root.AddChild(new TestElement());
            var c2 = root.AddChild(new TestElement());

            var g1 = c2.AddChild(new TestElement());
            
            root.Update();

            Assert.IsTrue(
                !root.IsDirty &&
                !c1.IsDirty &&
                !c2.IsDirty &&
                !g1.IsDirty);
        }

        [TestMethod]
        public void AddingChildren()
        {
            // Make the root.
            var root = MakeRoot();

            // Add children.
            var c1 = root.AddChild(new TestElement());
            var c2 = root.AddChild(new TestElement());

            // Add grandchildren.
            var g1 = c1.AddChild(new TestElement());
            var g2 = c1.AddChild(new TestElement());
            var g3 = c2.AddChild(new TestElement());
            var g4 = c2.AddChild(new TestElement());

            Assert.AreEqual(
                "Root\n-TestElement\n--TestElement\n--TestElement\n-TestElement\n--TestElement\n--TestElement\n", 
                root.Print());
        }

        [TestMethod]
        public void ConnectNode()
        {
            var a = new ElementNode(new TestElement(), null, new TestComponentProvider());
            var b = new ElementNode(new TestElement(), null, new TestComponentProvider());

            a.ConnectNode(b);

            Assert.AreEqual(
                "TestElement\n-TestElement\n",
                a.Print());
        }

        [TestMethod]
        public void SetDirtySetsSelf()
        {
            var root = MakeRoot();
            // Update to set clean.
            root.Update();
            root.SetDirty();
            Assert.IsTrue(root.IsDirty && !root.HasDirtyChild);
        }

        [TestMethod]
        public void SetDirtyUpdatesParent()
        {
            var root = MakeRoot();
            var c = root.AddChild(new TestElement());

            // Update to set clean.
            root.Update();
            c.SetDirty();

            Assert.IsTrue(root.HasDirtyChild && !root.IsDirty);
        }

        [TestMethod]
        public void SetDirtyPropagates()
        {
            var root = MakeRoot();
            var c = root
                .AddChild(new TestElement())
                .AddChild(new TestElement())
                .AddChild(new TestElement());

            // Update to set clean.
            root.Update();

            c.SetDirty();

            Assert.IsTrue(root.HasDirtyChild && !root.IsDirty);
        }

        [TestMethod]
        public void EmancipateSetsDirty()
        {
            var root = MakeRoot();
            var c = root.AddChild(new TestElement());

            root.Update();

            c.Emancipate();

            Assert.IsTrue(c.IsDirty && !c.HasDirtyChild);
            Assert.IsFalse(root.IsDirty);
        }

        /// <summary>
        /// Just sanity testing it works as expected.
        /// </summary>
        [TestMethod]
        public void EmancipatePreservesTreeState()
        {
            var a = new ElementNode(new TestElement(), null, new TestComponentProvider());
            a.AddChild(new TestElement())
                    .AddChild(new TestElement());
            
            var b = new ElementNode(new TestElement(), null, new TestComponentProvider());
            var youngestChild = 
                b.AddChild(new TestElement())
                    .AddChild(new TestElement());

            var aPrint = a.Print();
            var bPrint = b.Print();

            youngestChild.ConnectNode(a);

            // Make sure the tree is as we expect.
            Assert.AreEqual(
                "TestElement\n" +
                "-TestElement\n" +
                "--TestElement\n" +
                "---TestElement\n" +
                "----TestElement\n" +
                "-----TestElement\n",
                b.Print());

            a.Emancipate();

            Assert.AreEqual(a.Print(), aPrint);
            Assert.AreEqual(b.Print(), bPrint);
        }
    }
}
