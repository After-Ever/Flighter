using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

using Flighter;

namespace FlighterTest.Math
{
    [TestClass]
    public class BoxConstraintsTest
    {
        [TestMethod]
        public void MaxSize()
        {
            var free = BoxConstraints.Free;
            var freeMax = Size.Zero;

            var tight = BoxConstraints.Tight(100, 200);
            var tightMax = new Size(100, 200);

            var freeWithMin = new BoxConstraints(minWidth: 13, minHeight: 35);
            var freeWithMinMax = new Size(13, 35);

            var loose = new BoxConstraints(maxWidth: 333, maxHeight: 135);
            var looseMax = new Size(333, 135);

            Assert.AreEqual(freeMax, free.MaxSize);
            Assert.AreEqual(tightMax, tight.MaxSize);
            Assert.AreEqual(freeWithMinMax, freeWithMin.MaxSize);
            Assert.AreEqual(looseMax, loose.MaxSize);
        }

        [TestMethod]
        public void InvalidConstraintsThrow()
        {
            // MinX larger than maxX.
            Assert.ThrowsException<BoxConstrainstException>(() =>
            {
                new BoxConstraints(10,5);
            });

            // MinY larger than maxY.
            Assert.ThrowsException<BoxConstrainstException>(() =>
            {
                new BoxConstraints(0, 10, 10, 5);
            });

            // MinX < 0.
            Assert.ThrowsException<BoxConstrainstException>(() =>
            {
                new BoxConstraints(-1);
            });

            // MinY < 0.
            Assert.ThrowsException<BoxConstrainstException>(() =>
            {
                new BoxConstraints(0,10,-1);
            });
        }
    }
}
