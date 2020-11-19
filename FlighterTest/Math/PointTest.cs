using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Flighter;

namespace FlighterTest.Math
{
    [TestClass]
    public class PointTest
    {
        [TestMethod]
        public void Equality()
        {
            var a = new Point(10, 12);
            var b = new Point(10, 12);
            var c = new Point(-2, 3);
            var d = new Point(-2, 4);
            var e = new Point(-1.99999f, 4);
            var f = new Point(-10, 11.5f);

            Assert.AreEqual(a, b);
            Assert.AreNotEqual(c, d);
            Assert.AreNotEqual(d, e);
            Assert.AreNotEqual(a, f);
        }

        [TestMethod]
        public void HashCode()
        {
            var a = new Point(10, 12).GetHashCode();
            var b = new Point(10, 12).GetHashCode();
            var c = new Point(-2, 3).GetHashCode();
            var d = new Point(-2, 4).GetHashCode();
            var e = new Point(-1.99999f, 4).GetHashCode();
            var f = new Point(-10, 11.5f).GetHashCode();
            
            Assert.AreEqual(a, b);
            Assert.AreNotEqual(c, d);
            Assert.AreNotEqual(d, e);
            Assert.AreNotEqual(a, f);
        }

        [TestMethod]
        public void StringConversion()
        {
            var a = new Point(-3, 5.2f);

            Assert.AreEqual("(-3, 5.2)", a.ToString());
        }

        [TestMethod]
        public void Addition()
        {
            var a = new Point(10, 12);
            var b = new Point(-2, 3);

            var ab = new Point(8, 15);

            Assert.AreEqual(ab, a + b);
        }

        [TestMethod]
        public void Negation()
        {
            var a = new Point(10, 12);

            var _a = new Point(-10, -12);

            Assert.AreEqual(_a, -a);
        }

        [TestMethod]
        public void Subtraction()
        {
            var a = new Point(10, 12);
            var b = new Point(-2, 3);

            var ab = new Point(12, 9);

            Assert.AreEqual(ab, a - b);
        }

        [TestMethod]
        public void ScalarMultiplication()
        {
            var a = new Point(1, 3);

            var sa = new Point(2, 6);

            Assert.AreEqual(sa, 2 * a);
            Assert.AreEqual(sa, a * 2);
        }

        [TestMethod]
        public void DotProduct()
        {
            var a = new Point(10, 12);
            var b = new Point(-2, 3);

            var ab = 16;

            Assert.AreEqual(ab, a * b);
        }


    }
}
