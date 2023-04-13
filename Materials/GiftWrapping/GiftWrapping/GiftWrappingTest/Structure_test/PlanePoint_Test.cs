using System;
using FluentAssertions;
using GiftWrapping.Structures;
using NUnit.Framework;

namespace GiftWrappingTest.Structure_test
{
    [TestFixture]
    public class PlanePoint_Test
    {
       //[Test]
       // public void GetDimension_WhenCall_ReturnDimension()
       // {
       //     int expectDimension = 555;
       //     PlanePoint planePoint = new PlanePoint(new double[expectDimension])
       //     {
       //         OriginalPoint = new PlanePoint(new double[expectDimension + 1])
       //     };

       //     int resultingDimension = planePoint.Dim;

       //     Assert.AreEqual(expectDimension, resultingDimension);
       // }

        [Test]
        public void Equals_EqualObjects_ReturnTrue()
        {
            PlanePoint p1 = new PlanePoint(new double[] { 4, 4, 0, 0 });
            PlanePoint p2 = new PlanePoint(new double[] { 4, 4, 0, 0 });
            PlanePoint pp1 = new PlanePoint(new double[] {1, 1, 1}, p1);
            Point pp2 = new PlanePoint(new double[] { 2,2,2}, p2);


            Assert.AreEqual(pp1, pp2);
        }
    }
}