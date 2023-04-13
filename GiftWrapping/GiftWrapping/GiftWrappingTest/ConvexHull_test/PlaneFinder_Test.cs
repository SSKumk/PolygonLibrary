using System;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Common;
using GiftWrapping;
using GiftWrapping.Helpers;
using GiftWrapping.LinearEquations;
using GiftWrapping.Structures;
using NUnit.Framework;

namespace GiftWrappingTest.ConvexHull_test
{
    [TestFixture]
    public class PlaneFinder_Test
    {
        private static IEnumerable SetPoints()
        {
            // PlanePoint[] points1 = new PlanePoint[] {
            //     new PlanePoint(new double[]{1, 1, 1, 1}),
            //     new PlanePoint(new double[]{5, 1, 1, 1}),
            //     new PlanePoint(new double[]{1, 5, 1, 1}),
            //     new PlanePoint(new double[]{5, 5, 1,1}),
            //     new PlanePoint(new double[]{1, 1, 5,1}),
            //     new PlanePoint(new double[]{5, 1, 5,1}),
            //     new PlanePoint(new double[]{1, 5, 5,1}),
            //     new PlanePoint(new double[]{5, 5, 5,1}),
            //     new PlanePoint(new double[]{1, 1, 1, 5}),
            //     new PlanePoint(new double[]{5, 1, 1, 5}),
            //     new PlanePoint(new double[]{1, 5, 1, 5}),
            //     new PlanePoint(new double[]{5, 5, 1,5}),
            //     new PlanePoint(new double[]{1, 1, 5,5}),
            //     new PlanePoint(new double[]{5, 1, 5,5}),
            //     new PlanePoint(new double[]{1, 5, 5,5}),
            //     new PlanePoint(new double[]{5, 5, 5,5}),
            //
            // };
            //
            // Vector v11 = new Vector(new double[] { -1, 0 });
            // Vector v21 = new Vector(new double[] { 0, -1 });
            // Hyperplane[] expect1 = new Hyperplane[]
            // {
            //     new Hyperplane(points1[3], v11),
            //     new Hyperplane(points1[3], v21),
            // };
            //
            // yield return new object[] { points1, expect1 };


            PlanePoint[] points = new PlanePoint[] {
                new PlanePoint(new double[]{4, 0}),
                new PlanePoint(new double[]{0, 4}),
                new PlanePoint(new double[]{4, 4}),
                new PlanePoint(new double[]{0, 0}),
                new PlanePoint(new double[]{0.5, 0.5}),
                new PlanePoint(new double[]{1, 1}),
            };
            Vector v1 = new Vector(new double[] { -1, 0});
            Vector v2 = new Vector(new double[] { 0, -1});
            Hyperplane[] expect = new Hyperplane[]
            {
                new Hyperplane(points[3], v1),
                new Hyperplane(points[3], v2),
            };

            yield return new object[] { points, expect };

            points = new PlanePoint[] {
                new PlanePoint(new double[]{4, 0, 0}),
                new PlanePoint(new double[]{0, 4, 0}),
                new PlanePoint(new double[]{0, 0, 4}),
                new PlanePoint(new double[]{0, 0, 0}),
                new PlanePoint(new double[]{0.5, 0.5, 0.5}),
                new PlanePoint(new double[]{1, 1, 1}),
                new PlanePoint(new double[]{1, 1, 0.5})
            };
            v1 = new Vector(new double[] { 0, 0, -1 });
            v2 = new Vector(new double[] { 0, -1, 0 });
            Vector v3 = new Vector(new double[] { -1, 0, 0 });
            Vector v4 = new Vector(new double[] { -1, -1, -1 });
            expect = new Hyperplane[]
            {
                new Hyperplane(points[3], v1),
                new Hyperplane(points[3], v2),
                new Hyperplane(points[3], v3),
                new Hyperplane(points[3], v4)
            };

            yield return new object[] {points, expect};


            points = new PlanePoint[] {
                new PlanePoint(new double[]{1, 1, 1}),
                new PlanePoint(new double[]{1, 5, 1}),
                new PlanePoint(new double[]{5, 1, 1}),
                new PlanePoint(new double[]{5, 5, 1}),
                new PlanePoint(new double[]{1, 1, 5}),
                new PlanePoint(new double[]{1, 5, 5}),
                new PlanePoint(new double[]{5, 1, 5}),
                new PlanePoint(new double[]{5, 5, 5}),
          
            };
            v1 = new Vector(new double[] { 0, 0, -1 });
            v2 = new Vector(new double[] { 0, -1, 0 });
            v3 = new Vector(new double[] { -1, 0, 0 });
            v4 = new Vector(new double[] { -1, -1, -1 });
            expect = new Hyperplane[]
            {
                new Hyperplane(points[0], v1),
                new Hyperplane(points[0], v2),
                new Hyperplane(points[0], v3),
                new Hyperplane(points[0], v4)
            };

            yield return new object[] { points, expect };

            // points = new PlanePoint[] {
            //     new PlanePoint(new double[]{4, 0, 0, 0}),
            //     new PlanePoint(new double[]{0, 4, 0, 0}),
            //     new PlanePoint(new double[]{0, 0, 4, 0}),
            //     new PlanePoint(new double[]{0, 0, 0, 4}),
            //     new PlanePoint(new double[]{0, 0, 0, 0}),
            //     new PlanePoint(new double[]{0.5, 0.5, 0.5, 0.5}),
            //     new PlanePoint(new double[]{1, 1, 1, 0.5}),
            //     new PlanePoint(new double[]{3, 1, 1, 1}),
            //     new PlanePoint(new double[]{1, 3, 1, 1}),
            //     new PlanePoint(new double[]{1, 1, 3, 1}),
            //     new PlanePoint(new double[]{1, 1, 1, 3}),
            //     new PlanePoint(new double[]{1, 1, 1, 1})
            // };
            points = new PlanePoint[] {
                new PlanePoint(new double[]{1, 1, 1, 1}),
                new PlanePoint(new double[]{5, 1, 1, 1}),
                new PlanePoint(new double[]{1, 5, 1, 1}),
                new PlanePoint(new double[]{5, 5, 1,1}),
                new PlanePoint(new double[]{1, 1, 5,1}),
                new PlanePoint(new double[]{5, 1, 5,1}),
                new PlanePoint(new double[]{1, 5, 5,1}),
                new PlanePoint(new double[]{5, 5, 5,1}),
                new PlanePoint(new double[]{1, 1, 1, 5}),
                new PlanePoint(new double[]{5, 1, 1, 5}),
                new PlanePoint(new double[]{1, 5, 1, 5}),
                new PlanePoint(new double[]{5, 5, 1,5}),
                new PlanePoint(new double[]{1, 1, 5,5}),
                new PlanePoint(new double[]{5, 1, 5,5}),
                new PlanePoint(new double[]{1, 5, 5,5}),
                new PlanePoint(new double[]{5, 5, 5,5}),

            };

            v1 = new Vector(new double[] { 0, 0, 0, -1 });
            v2 = new Vector(new double[] { 0, 0, -1, 0 });
            v3 = new Vector(new double[] { 0, -1, 0, 0 });
            v4 = new Vector(new double[] { -1, 0, 0, 0 });
            Vector v5 = new Vector(new double[] { -1, -1, -1, -1 });
            expect = new Hyperplane[]
            {
                new Hyperplane(points[4], v1),
                new Hyperplane(points[4], v2),
                new Hyperplane(points[4], v3),
                new Hyperplane(points[4], v4),
                new Hyperplane(points[4], v5)
            };

            yield return new object[] { points, expect };

            points = new PlanePoint[] {
                new PlanePoint(new double[]{4, 0, 0, 0, 0}),
                new PlanePoint(new double[]{0, 4, 0, 0, 0}),
                new PlanePoint(new double[]{0, 0, 4, 0, 0}),
                new PlanePoint(new double[]{0, 0, 0, 4, 0}),
                new PlanePoint(new double[]{0, 0, 0, 0, 4}),
                new PlanePoint(new double[]{0, 0, 0, 0, 0}),
                new PlanePoint(new double[]{0.5, 0.5, 0.5, 0.5, 0}),
                new PlanePoint(new double[]{1, 1, 1, 0.5, 0.5}),
                new PlanePoint(new double[]{3, 1, 1, 1, 1}),
                new PlanePoint(new double[]{1, 3, 1, 1, 1}),
                new PlanePoint(new double[]{1, 1, 3, 1, 1}),
                new PlanePoint(new double[]{1, 1, 1, 3, 1}),
                new PlanePoint(new double[]{1, 1, 1, 1, 3}),
                new PlanePoint(new double[]{1, 1, 1, 1, 1})
            };
           
            v1 = new Vector(new double[] { 0, 0, 0, 0, -1 });
            v2 = new Vector(new double[] { 0, 0, 0, -1, 0 });
            v3 = new Vector(new double[] { 0, 0, -1, 0, 0 });
            v4 = new Vector(new double[] { 0, -1, 0, 0, 0 });
            v5 = new Vector(new double[] { -1, 0, 0, 0, 0 });
            Vector v6 = new Vector(new double[] { -1, -1, -1, -1 });
            expect = new Hyperplane[]
            {
                new Hyperplane(points[5], v1),
                new Hyperplane(points[5], v2),
                new Hyperplane(points[5], v3),
                new Hyperplane(points[5], v4),
                new Hyperplane(points[5], v5)
            };

            yield return new object[] { points, expect };
        }

        [Test, TestCaseSource(nameof(SetPoints))]
        public void FindFirstPlane_Simplex_ReturnHyperplane(Point[] points, Hyperplane[] expected)
        {
            PlaneFinder planeFinder = new PlaneFinder();

            Hyperplane result = planeFinder.FindFirstPlane(points.ToPlanePoint());

            expected.Should().Contain(result);

        }

        [Test]
        public void FindStartingVector_WhenCall_ReturnIndexVector()
        {
            Point[] points = new Point[5] {
                new Point(new double[]{2, 3}),
                new Point(new double[]{3, 2}),
                new Point(new double[]{1, 2}),
                new Point(new double[]{5, 5}),
                new Point(new double[]{2, 2})};

            Point result = points.Min();


            Assert.AreEqual(points[2], result);
        }


        [Test, Ignore("SandBox")]
        public void Test_WhenCall_ReturnIndexVector()
        {
            PlanePoint[] points = new PlanePoint[] {
               
                new PlanePoint(new double[]{5, 1, 1, 1,1}),
                new PlanePoint(new double[]{1, 5, 1, 1,1}),
                new PlanePoint(new double[]{5, 5, 1,1,1}),
                new PlanePoint(new double[]{1, 1, 5,1,1}),
                new PlanePoint(new double[]{5, 1, 5,1,1}),
                new PlanePoint(new double[]{1, 5, 5,1,1}),
                new PlanePoint(new double[]{5, 5, 5,1,1}),
                new PlanePoint(new double[]{1, 1, 1, 5,1}),
                new PlanePoint(new double[]{5, 1, 1, 5,1}),
                new PlanePoint(new double[]{1, 5, 1, 5,1}),
                new PlanePoint(new double[]{5, 5, 1,5,1}),
                new PlanePoint(new double[]{1, 1, 5,5,1}),
                new PlanePoint(new double[]{1, 1, 1, 1,1}),
                new PlanePoint(new double[]{5, 1, 5,5,1}),
                new PlanePoint(new double[]{1, 5, 5,5,1}),
                new PlanePoint(new double[]{5, 5, 5,5,1}),
                new PlanePoint(new double[]{1, 1, 1, 1,5}),
                new PlanePoint(new double[]{5, 1, 1, 1,5}),
                new PlanePoint(new double[]{1, 5, 1, 1,5}),
                new PlanePoint(new double[]{5, 5, 1,1,5}),
                new PlanePoint(new double[]{1, 1, 5,1,5}),
                new PlanePoint(new double[]{5, 1, 5,1,5}),
                new PlanePoint(new double[]{1, 5, 5,1,5}),
                new PlanePoint(new double[]{5, 5, 5,1,5}),
                new PlanePoint(new double[]{1, 1, 1, 5,5}),
                new PlanePoint(new double[]{5, 1, 1, 5,5}),
                new PlanePoint(new double[]{1, 5, 1, 5,5}),
                new PlanePoint(new double[]{5, 5, 1,5,5}),
                new PlanePoint(new double[]{1, 1, 5,5,5}),
                new PlanePoint(new double[]{5, 1, 5,5,5}),
                new PlanePoint(new double[]{1, 5, 5,5,5}),
                new PlanePoint(new double[]{5, 5, 5,5,5}),
               

            };


            //PlaneFinder planeFinder = new PlaneFinder();
            //Hyperplane result = planeFinder.FindFirstPlane(points.ToPlanePoint());

             Stopwatch sp = new Stopwatch();
            // sp.Start();
            //
            // sp.Stop();
            //
            // double first = sp.ElapsedMilliseconds;
            sp.Reset();
            sp.Start();
            PlaneFinder planeFinder1 = new PlaneFinder();
            Hyperplane result1 = planeFinder1.FindFirstPlane(points.ToPlanePoint());
            sp.Stop();
         

            double sec = sp.ElapsedMilliseconds;
            sp.Reset();
            sp.Start();
            PlaneFinder planeFinder2 = new PlaneFinder();
            Hyperplane result2 = planeFinder1.FindFirstPlane(points.ToPlanePoint());
            sp.Stop();

            double sec2 = sp.ElapsedMilliseconds;

            Assert.AreEqual(points[2], result1);
        }

    }
}