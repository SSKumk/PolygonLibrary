using System;
using System.Collections.Generic;
using System.Linq;
using GiftWrapping.Helpers;
using GiftWrapping.Structures;
using NUnit.Framework;

namespace GiftWrappingTest.Structure_test
{
    [TestFixture]
    public class ConvexHull_Test
    {
        [Test]
        public void Equals_EqualObjects_ReturnTrue()
        {
            List<PlanePoint> p1 = new List<PlanePoint> {
                new PlanePoint(new double[]{0, 0}),
                new PlanePoint(new double[]{4, 0}),
                new PlanePoint(new double[]{0, 4}),
            };
            List<PlanePoint> p2 = new List<PlanePoint> {
                new PlanePoint(new double[]{0, 0}),
                new PlanePoint(new double[]{4, 0}),
                new PlanePoint(new double[]{0, 4}),
            };

            List<PlanePoint> p3 = new List<PlanePoint> {
                new PlanePoint(new double[]{0, 0}),
                new PlanePoint(new double[]{4, 0}),
                new PlanePoint(new double[]{0, 4}),
            };
            List<PlanePoint> p4 = new List<PlanePoint> {
                new PlanePoint(new double[]{0, 0}),
                new PlanePoint(new double[]{4, 0}),
                new PlanePoint(new double[]{0, 4}),
            };
            ConvexHull2d c1 = p1.ToConvexHull2d();
            ConvexHull2d c2 = p2.ToConvexHull2d();
            ConvexHull2d c3 = p3.ToConvexHull2d();
            ConvexHull2d c4 = p4.ToConvexHull2d();

            ConvexHull ch1 = new ConvexHull(3);
            ch1.AddInnerCell(c1);
            ch1.AddInnerCell(c2);

            ConvexHull ch2 = new ConvexHull(3);
            ch2.AddInnerCell(c3);
            ch2.AddInnerCell(c4);

            Assert.AreEqual(ch1, ch2);
        }


        [Test, Ignore("Not working")]
        public void GetPoints_When3dConvexHull_ReturnTrue()
        {
            List<PlanePoint> p1 = new List<PlanePoint> {
                new PlanePoint(new double[]{0, 0, 0, 0}),
                new PlanePoint(new double[]{4, 0, 0, 0}),
                new PlanePoint(new double[]{0, 4, 0, 0}),
            };
            List<PlanePoint> p2 = new List<PlanePoint> {
                new PlanePoint(new double[]{0, 0, 0, 0}),
                new PlanePoint(new double[]{4, 0, 0, 0}),
                new PlanePoint(new double[]{0, 0, 4, 0}),
            };
            ConvexHull2d c1 = p1.ToConvexHull2d();
            ConvexHull2d c2 = p2.ToConvexHull2d();
            ConvexHull ch1 = new ConvexHull(3);
            ch1.AddInnerCell(c1);
            ch1.AddInnerCell(c2);
            PlanePoint[] expect = new PlanePoint[]{
                new PlanePoint(new double[]{0, 0, 0, 0}),
                new PlanePoint(new double[]{4, 0, 0, 0}),
                new PlanePoint(new double[]{0, 4, 0, 0}),
                new PlanePoint(new double[]{0, 0, 4, 0}),
            };
            Array.Sort(expect);
            
            Point[] actual = ch1.GetPoints().ToArray();
            Array.Sort(actual);

            Assert.AreEqual(expect, actual);
        }
    }
}