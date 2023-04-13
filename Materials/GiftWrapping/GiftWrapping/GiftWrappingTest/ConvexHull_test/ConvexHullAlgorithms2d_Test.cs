using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GiftWrapping;
using GiftWrapping.Helpers;
using GiftWrapping.Structures;
using NUnit.Framework;

namespace GiftWrappingTest.ConvexHull_test
{
    [TestFixture]
    public class ConvexHullAlgorithms2d_Test
    {
        [Test, TestCaseSource("Get2dPoints")]
        public ConvexHull2d GiftWrapping2d(IList<PlanePoint> points)
        {
            GiftWrapping2d algorithm = new GiftWrapping2d();

            return (ConvexHull2d)algorithm.FindConvexHull(points);
        }

        [Test, TestCaseSource("Get2dPoints")]
        public ConvexHull2d GrahamScan2d(IList<PlanePoint> points)
        {
            GrahamScan2d algorithm = new GrahamScan2d();

            return (ConvexHull2d)algorithm.FindConvexHull(points);
        }
        public static IEnumerable<TestCaseData> Get2dPoints()
        {
            List<PlanePoint> points1 = new List<PlanePoint> {
                new PlanePoint(new double[]{1, 5}),
                new PlanePoint(new double[]{1, 3}),
                new PlanePoint(new double[]{2, 1}),
                new PlanePoint(new double[]{4, 1.1}),
                new PlanePoint(new double[]{4, 0.5}),
                new PlanePoint(new double[]{5, 3}),
            };
            PlanePoint[] expectPoint1 = new PlanePoint[]{
                new PlanePoint(new double[]{1, 3}),
                new PlanePoint(new double[]{2, 1}),
                new PlanePoint(new double[]{4, 0.5}),
                new PlanePoint(new double[]{5, 3}),
                new PlanePoint(new double[]{1, 5}),
            };

            ConvexHull2d expect1 = expectPoint1.ToConvexHull2d();

            yield return new TestCaseData(points1).SetName("{m}_When2dPoints").Returns(expect1);
            List<PlanePoint> points2 = new List<PlanePoint> {
                new PlanePoint(new double[]{1, 1}),
                new PlanePoint(new double[]{1, 5}),
                new PlanePoint(new double[]{5, 1}),
                new PlanePoint(new double[]{7, 1}),
                new PlanePoint(new double[]{10, 1.1}),
                new PlanePoint(new double[]{10, 5}),
                new PlanePoint(new double[]{10, 8}),
                new PlanePoint(new double[]{10, 10}),

            };
            PlanePoint[] expectPoint2 = new PlanePoint[]{
                new PlanePoint(new double[]{1, 1}),
                new PlanePoint(new double[]{7, 1}),
                new PlanePoint(new double[]{10, 1.1}),
                new PlanePoint(new double[]{10, 10}),
                new PlanePoint(new double[]{1, 5}),
            };

            ConvexHull2d expect2 = expectPoint2.ToConvexHull2d();

            yield return new TestCaseData(points2).SetName("{m}_WhenMultiplePointsOnLine").Returns(expect2);


            IEnumerable<PlanePoint> p1 = new PlanePoint[100];
            p1 = p1.Select(((_, i) => new PlanePoint(new double[] { 1, i + 1 })));
            IEnumerable<PlanePoint> p2 = new PlanePoint[100];
            p2 = p2.Select(((_, i) => new PlanePoint(new double[] { i + 1, 102 })));
            IEnumerable<PlanePoint> p3 = new PlanePoint[100];
            p3 = p3.Select(((_, i) => new PlanePoint(new double[] { 100, 101 - i })));
            IEnumerable<PlanePoint> p4 = new PlanePoint[100];
            p4 = p4.Select(((_, i) => new PlanePoint(new double[] { 100 - i, 0.5 })));
            List<PlanePoint> points3 = new List<PlanePoint>();
            points3.AddRange(p1);
            points3.AddRange(p2);
            points3.AddRange(p3);
            points3.AddRange(p4);
            PlanePoint[] expectPoint3 = new PlanePoint[]{
                new PlanePoint(new double[]{1, 0.5}),
                new PlanePoint(new double[]{100, 0.5}),
                new PlanePoint(new double[]{100, 102}),
                new PlanePoint(new double[]{1, 102}),
            };

            ConvexHull2d expect3 = expectPoint3.ToConvexHull2d();

            yield return new TestCaseData(points3).SetName("{m}_WhenMultiplePointsOnLine2").Returns(expect3);
            //SetName("FindConvexHull2D_WhenMultiplePointsOnLine")
        }
    }
}