using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ArkanoidGame.Geometry;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Vector2D vector = new Vector2D(Math.Sqrt(2), 0);
            vector.Rotate(Math.PI / 4);
            double expected = 1;
            Assert.AreEqual(expected, vector.X);
            Assert.AreEqual(expected, vector.Y);
        }
    }
}
