using Microsoft.VisualStudio.TestTools.UnitTesting;

using Flighter;

namespace FlighterTest
{
    [TestClass]
    public class ElementNodeTest
    {
        [TestMethod]
        public void StartsDirty()
        {
            var en = TestUtilities.MakeSimpleElementNode();

            Assert.IsTrue(en.IsDirty);
        }

        [TestMethod]
        public void UpdateCleans()
        {
            var en = TestUtilities.MakeSimpleRootElementNode();

            en.Update();

            Assert.IsFalse(en.IsDirty);
        }

        [TestMethod]
        public void DirtyChildren()
        {
            var r = TestUtilities.MakeSimpleRootElementNode();

            r.Update();

            var ce = new TestElement();
            var c = r.AddChild(ce);

            Assert.IsFalse(r.IsDirty);
            Assert.IsTrue(r.HasDirtyChild);

            r.Update();

            Assert.IsFalse(r.HasDirtyChild);
            Assert.IsFalse(c.IsDirty);

            c.SetDirty();

            Assert.IsFalse(r.IsDirty);
            Assert.IsTrue(r.HasDirtyChild);
            Assert.IsTrue(c.IsDirty);

            r.Update();

            Assert.IsFalse(r.HasDirtyChild);
            Assert.IsFalse(c.IsDirty);
            
            var g = new ElementNode(new TestElement(), null);
            c.ConnectNode(g);

            Assert.IsFalse(r.IsDirty);
            Assert.IsFalse(c.IsDirty);
            Assert.IsTrue(r.HasDirtyChild);
            Assert.IsTrue(c.HasDirtyChild);
            Assert.IsTrue(g.IsDirty);
        }

        [TestMethod]
        public void UpdateInitializes()
        {
            TestElement e1, e2, e3;

            var root = TestUtilities.MakeSimpleRootElementNode();

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

            var root = TestUtilities.MakeSimpleRootElementNode();

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

        [TestMethod]
        public void AddingChildren()
        {
            // Make the root.
            var root = TestUtilities.MakeSimpleRootElementNode();

            // Add children.
            var c1 = root.AddChild(new TestElement());
            var c2 = root.AddChild(new TestElement());

            // Add grandchildren.
            var g1 = c1.AddChild(new TestElement());
            var g2 = c1.AddChild(new TestElement());
            var g3 = c2.AddChild(new TestElement());
            var g4 = c2.AddChild(new TestElement());

            Assert.AreEqual(
                "Root\n" +
                "-TestElement\n" +
                "--TestElement\n" +
                "--TestElement\n" +
                "-TestElement\n" +
                "--TestElement\n" +
                "--TestElement\n", 
                root.Print());
        }

        [TestMethod]
        public void ConnectNode()
        {
            var a = new ElementNode(new TestElement(), null, new ComponentProvider(new System.Collections.Generic.Dictionary<System.Type, System.Type>()));
            var b = new ElementNode(new TestElement(), null, new ComponentProvider(new System.Collections.Generic.Dictionary<System.Type, System.Type>()));

            a.ConnectNode(b);

            Assert.AreEqual(
                "TestElement\n" +
                "-TestElement\n",
                a.Print());
        }

        [TestMethod]
        public void EmancipateSetsDirty()
        {
            var root = TestUtilities.MakeSimpleRootElementNode();
            var c = root.AddChild(new TestElement());

            root.Update();

            c.Emancipate();

            Assert.IsTrue(c.IsDirty && !c.HasDirtyChild);
            Assert.IsFalse(root.IsDirty);
        }

        [TestMethod]
        public void EmancipatedUpdatesDirtyChildren()
        {
            var root = TestUtilities.MakeSimpleRootElementNode();
            root.Update();

            var c = root.AddChild(new TestElement());

            Assert.IsTrue(root.HasDirtyChild);

            c.Emancipate();

            Assert.IsFalse(root.HasDirtyChild);
        }

        /// <summary>
        /// Just sanity testing it works as expected.
        /// </summary>
        [TestMethod]
        public void EmancipatePreservesTreeState()
        {
            var a = new ElementNode(new TestElement(), null, new ComponentProvider(new System.Collections.Generic.Dictionary<System.Type, System.Type>()));
            a.AddChild(new TestElement())
                    .AddChild(new TestElement());
            
            var b = new ElementNode(new TestElement(), null, new ComponentProvider(new System.Collections.Generic.Dictionary<System.Type, System.Type>()));
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

        [TestMethod]
        public void PruneCallsTearDown()
        {
            var r = TestUtilities.MakeSimpleRootElementNode();
            var e = new TestElement();
            var en = r.AddChild(e);

            r.Update();

            var testRect = e.DisplayRect as TestDisplayRect;

            en.Prune();

            Assert.IsTrue(testRect.WasTornDown);
        }
    }
}
