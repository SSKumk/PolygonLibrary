using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using GiftWrapping;
using GiftWrapping.Helpers;
using GiftWrapping.LinearEquations;
using GiftWrapping.Structures;
using NUnit.Framework;

namespace GiftWrappingTest.ConvexHull_test
{
    [TestFixture]
    public class GiftWrappingAlgorithm_Test
    {
     
        private static IEnumerable GetNDimensionPoints()
        {
            PlanePoint[] points = new PlanePoint[] {
                new PlanePoint(new double[]{1, 1, 0}),
                new PlanePoint(new double[]{5, 1, 0}),
                new PlanePoint(new double[]{1, 5, 0}),
                new PlanePoint(new double[]{5, 5, 0}),
                new PlanePoint(new double[]{1, 1, 5}),
                new PlanePoint(new double[]{5, 1, 5}),
                new PlanePoint(new double[]{1, 5, 5}),
                new PlanePoint(new double[]{5, 5, 5}),
                new PlanePoint(new double[]{3, 3, 3}),
            };

            Vector v1 = new Vector(new double[] { 0, 0, -1});
            Vector v2 = new Vector(new double[] { 0, -1, 0});
            Vector v3 = new Vector(new double[] { -1, 0, 0});
            Hyperplane[] expect = new Hyperplane[]
            {
                new Hyperplane(points[0], v1),
                new Hyperplane(points[0], v2),
                new Hyperplane(points[0], v3),
            };
            yield return new TestCaseData(points).SetName("{m}_3dPoints");
        }

        [Test]
        public void FindFirstPlane_ReturnHyperplane()
        {
            // PlanePoint[] points = new PlanePoint[] {
            //      new PlanePoint(new double[]{1, 1, 1}),
            //      new PlanePoint(new double[]{5, 1, 1}),
            //      new PlanePoint(new double[]{1, 5, 1}),
            //      new PlanePoint(new double[]{5, 5, 1}),
            //      new PlanePoint(new double[]{1, 1, 5}),
            //      new PlanePoint(new double[]{5, 1, 5}),
            //      new PlanePoint(new double[]{1, 5, 5}),
            //      new PlanePoint(new double[]{5, 5, 5}),
            //      new PlanePoint(new double[]{3, 3, 3}),
            //  };
            // List<PlanePoint> points = new List<PlanePoint> {
            //      new PlanePoint(new double[]{1, 1, 1, 1}),
            //      new PlanePoint(new double[]{5, 1, 1, 1}),
            //      new PlanePoint(new double[]{1, 5, 1, 1}),
            //      new PlanePoint(new double[]{5, 5, 1,1}),
            //      new PlanePoint(new double[]{1, 1, 5,1}),
            //      new PlanePoint(new double[]{5, 1, 5,1}),
            //      new PlanePoint(new double[]{1, 5, 5,1}),
            //      new PlanePoint(new double[]{5, 5, 5,1}),
            //      new PlanePoint(new double[]{1, 1, 1, 5}),
            //      new PlanePoint(new double[]{5, 1, 1, 5}),
            //      new PlanePoint(new double[]{1, 5, 1, 5}),
            //      new PlanePoint(new double[]{5, 5, 1,5}),
            //      new PlanePoint(new double[]{1, 1, 5,5}),
            //      new PlanePoint(new double[]{5, 1, 5,5}),
            //      new PlanePoint(new double[]{1, 5, 5,5}),
            //      new PlanePoint(new double[]{5, 5, 5,5}),
            //
            //  };
            // for (int i = 2; i < 100; i++)
            // {
            //     double res = 1 + 4.0 / i;
            //     points.Add(new PlanePoint(new double[]{res,res,res,res}));
            // }
            //
            PlanePoint[] points = new PlanePoint[]
            {
                new PlanePoint(new double[] {1, 1, 1, 1, 1}),
                new PlanePoint(new double[] {5, 1, 1, 1, 1}),
                new PlanePoint(new double[] {1, 5, 1, 1, 1}),
                new PlanePoint(new double[] {5, 5, 1, 1, 1}),
                new PlanePoint(new double[] {1, 1, 5, 1, 1}),
                new PlanePoint(new double[] {5, 1, 5, 1, 1}),
                new PlanePoint(new double[] {1, 5, 5, 1, 1}),
                new PlanePoint(new double[] {5, 5, 5, 1, 1}),
                new PlanePoint(new double[] {1, 1, 1, 5, 1}),
                new PlanePoint(new double[] {5, 1, 1, 5, 1}),
                new PlanePoint(new double[] {1, 5, 1, 5, 1}),
                new PlanePoint(new double[] {5, 5, 1, 5, 1}),
                new PlanePoint(new double[] {1, 1, 5, 5, 1}),
                new PlanePoint(new double[] {5, 1, 5, 5, 1}),
                new PlanePoint(new double[] {1, 5, 5, 5, 1}),
                new PlanePoint(new double[] {5, 5, 5, 5, 1}),
                new PlanePoint(new double[] {1, 1, 1, 1, 5}),
                new PlanePoint(new double[] {5, 1, 1, 1, 5}),
                new PlanePoint(new double[] {1, 5, 1, 1, 5}),
                new PlanePoint(new double[] {5, 5, 1, 1, 5}),
                new PlanePoint(new double[] {1, 1, 5, 1, 5}),
                new PlanePoint(new double[] {5, 1, 5, 1, 5}),
                new PlanePoint(new double[] {1, 5, 5, 1, 5}),
                new PlanePoint(new double[] {5, 5, 5, 1, 5}),
                new PlanePoint(new double[] {1, 1, 1, 5, 5}),
                new PlanePoint(new double[] {5, 1, 1, 5, 5}),
                new PlanePoint(new double[] {1, 5, 1, 5, 5}),
                new PlanePoint(new double[] {5, 5, 1, 5, 5}),
                new PlanePoint(new double[] {1, 1, 5, 5, 5}),
                new PlanePoint(new double[] {5, 1, 5, 5, 5}),
                new PlanePoint(new double[] {1, 5, 5, 5, 5}),
                new PlanePoint(new double[] {5, 5, 5, 5, 5}),
            };
            //PlanePoint[] points = new PlanePoint[1000];
            //Random rnd = new Random();
            //for (int j = 0; j < points.Length; j++)
            //{
            //    double[] p = new double[]{rnd.Next(1,500), rnd.Next(250, 500), rnd.Next(500, 600)};
            //    points[j] = new PlanePoint(p);
            //}
            GiftWrappingAlgorithmTestClass giftWrapping = new GiftWrappingAlgorithmTestClass(points, Tools.Eps);
            Stopwatch sp = new Stopwatch();
            IFace result = giftWrapping.Create();
            sp.Start();
            IFace result1 = giftWrapping.Create();
            sp.Stop();
            long first = sp.ElapsedMilliseconds;

            //sp.Reset();
            //sp.Start();
            //IFace result1 = giftWrapping.FindConvexHull(points.ToPlanePoint());
            //sp.Stop();

            //double sec = sp.ElapsedMilliseconds;
          //  int i = 0;
           // ((ConvexHull)result1).Convert(@"D:\Projects\GiftWrapping", "test331" + i++);
            //foreach (ICell cell in result.InnerCells)
            //{
            //    ((ConvexHull)cell).Convert(@"D:\Projects\GiftWrapping", "test1"+i++);
            //}


            Assert.IsTrue(false);
        }


        [Test]
        public void FindConvexHull2D_Points2d_ReturnConvexHull2D()
        {
            PlanePoint[] points = new PlanePoint[] {
                new PlanePoint(new double[]{4, 0}),
                new PlanePoint(new double[]{0, 4}),
                new PlanePoint(new double[]{4, 4}),
                new PlanePoint(new double[]{0, 0}),
                new PlanePoint(new double[]{0.5, 0.5}),
                new PlanePoint(new double[]{1, 1}),
            };
            List<PlanePoint> expectPoint = new List<PlanePoint>
            {
                new PlanePoint(new double[]{0, 0}),
                new PlanePoint(new double[]{4, 0}),
                new PlanePoint(new double[]{0, 4}),
                new PlanePoint(new double[]{4, 4}),
            };
            ConvexHull2d expect = expectPoint.ToConvexHull2d();

            GiftWrappingAlgorithmTestClass giftWrapping = new GiftWrappingAlgorithmTestClass(points, Tools.Eps);
            
            GrahamScan2d per = new GrahamScan2d();
            
            ConvexHull2d actual = (ConvexHull2d)giftWrapping.Create();

            actual.Should().Be(expect);
        }


        [Test, TestCaseSource("Get2dPoints")]
        public ConvexHull2d FindConvexHull2D(IList<PlanePoint> points)
        {
            GiftWrappingAlgorithmTestClass algorithm = new GiftWrappingAlgorithmTestClass(points, Tools.Eps);

            return (ConvexHull2d)algorithm.Create();
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
            List <PlanePoint> points2 = new List<PlanePoint> {
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
           

            IEnumerable<PlanePoint> p1  = new PlanePoint[100];
            p1 = p1.Select(((_, i) => new PlanePoint(new double[] {1, i+1})));
            IEnumerable<PlanePoint> p2 = new PlanePoint[100];
            p2 = p2.Select(((_, i) => new PlanePoint(new double[] { i+1, 102 })));
            IEnumerable<PlanePoint> p3 = new PlanePoint[100];
            p3 = p3.Select(((_, i) => new PlanePoint(new double[] { 100, 101-i })));
            IEnumerable<PlanePoint> p4 = new PlanePoint[100];
            p4 = p4.Select(((_, i) => new PlanePoint(new double[] { 100-i, 0.5 })));
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