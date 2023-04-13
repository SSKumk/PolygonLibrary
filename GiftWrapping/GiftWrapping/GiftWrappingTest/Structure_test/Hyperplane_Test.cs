using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GiftWrapping;
using GiftWrapping.Helpers;
using GiftWrapping.LinearEquations;
using GiftWrapping.Structures;
using NUnit.Framework;

namespace GiftWrappingTest.Structure_test
{
    [TestFixture]
    public class Hyperplane_Test
    {

        [Test]
        public void Angle_WhenCall_ReturnAngle()
        {
            PlanePoint p1 = new PlanePoint(3);
            Vector n1 = new Vector(new double[]{1,0,0});
            Vector n2 = new Vector(new double[] { 0, 1, 0 });
            Hyperplane h1 = new Hyperplane(p1, n1);
            Hyperplane h2 = new Hyperplane(p1, n2);
            const double expect = Math.PI/2;

            double result = h1.Angle(h2);

            Assert.AreEqual(expect, result, Constants.Esp);
        }

        [Test]
        public void Angle_SamePlane_ReturnAngle()
        {
            PlanePoint p1 = new PlanePoint(3);
            Vector n1 = new Vector(new double[] { 1, 0, 0 });
            Hyperplane h1 = new Hyperplane(p1, n1);
            const double expect = 0;

            double result = h1.Angle(h1);

            Assert.AreEqual(expect, result, Constants.Esp);
        }



        [Test]
        public void Side_PositivePoint_ReturnPosition()
        {
            PlanePoint p1 = new PlanePoint(3);
            Vector n1 = new Vector(new double[] { 1, 0, 0 });
            Hyperplane h1 = new Hyperplane(p1, n1);
            Point p2 = new Point(new double[]{1,1,1});
            const int expect = 1;

            int result = h1.Side(p2);

            Assert.AreEqual(expect, result);
        }

        [Test]
        public void Side_NegativePoint_ReturnPosition()
        {
            PlanePoint p1 = new PlanePoint(3);
            Vector n1 = new Vector(new double[] { 1, 0, 0 });
            Hyperplane h1 = new Hyperplane(p1, n1);
            Point p2 = new Point(new double[] { -1, -1, -1 });
            const int expect = -1;

            int result = h1.Side(p2);

            Assert.AreEqual(expect, result);
        }

        [Test]
        public void Side_PointOfPlane_ReturnPosition()
        {
            PlanePoint p1 = new PlanePoint(3);
            Vector n1 = new Vector(new double[] { 1, 0, 0 });
            Hyperplane h1 = new Hyperplane(p1, n1);
            Point p2 = new Point(new double[] { 0, 4, 4 });
            const int expect = 0;

            int result = h1.Side(p2);

            Assert.AreEqual(expect, result);
        }

        [Test]
        public void ReorientNormal_WhenCall_ChangeOrientationNormal()
        {
            PlanePoint p1 = new PlanePoint(3);
            Vector n1 = new Vector(new double[] { 1, 1, 1 });
            Hyperplane h1 = new Hyperplane(p1, n1);
            Vector n2 = new Vector(new double[] { -1, -1, -1 });
            Hyperplane h2 = new Hyperplane(p1, n2);

            h1.ReorientNormal();

            Assert.AreEqual(h2.Normal, h1.Normal);
        }

        [Test]
        public void Equals_EqualPlane_ReturnTrue()
        {
            PlanePoint p1 = new PlanePoint(new double[]{1, 7, 0});
            Vector n1 = new Vector(new double[] { 2, -1, 0 });
            Hyperplane h1 = new Hyperplane(p1, n1);
            PlanePoint p2 = new PlanePoint(new double[] { -1, 3, 0 });
            Vector n2 = new Vector(new double[] { -4, 2, 0 });
            Hyperplane h2 = new Hyperplane(p2, n2);

             bool result = h1.Equals(h2);

            Assert.AreEqual(true, result);
        }


        [Test]
        public void Equals_UnequalPlane_ReturnFalse()
        {
            PlanePoint p1 = new PlanePoint(new double[] { 1, 7, 0 });
            Vector n1 = new Vector(new double[] { 1, -1, 0 });
            Hyperplane h1 = new Hyperplane(p1, n1);
            PlanePoint p2 = new PlanePoint(new double[] { -1, 3, 0 });
            Vector n2 = new Vector(new double[] { -4, 2, 0 });
            Hyperplane h2 = new Hyperplane(p2, n2);

            bool result = h1.Equals(h2);

            Assert.AreEqual(false, result);
        }


        [Test]
        public void GetHashCode_WhenCall_SameValue()
        {
            PlanePoint p1 = new PlanePoint(new double[] { 1, 7, 0 });
            Vector n1 = new Vector(new double[] { -2, 1, 0 });
            Hyperplane h1 = new Hyperplane(p1, n1);
            PlanePoint p2 = new PlanePoint(new double[] { -1, 3, 0 });
            Vector n2 = new Vector(new double[] { -4, 2, 0 });
            Hyperplane h2 = new Hyperplane(p2, n2);

            int result1 = h1.GetHashCode();
            int result2 = h2.GetHashCode();
            
            Assert.AreEqual(result1,result2 );
        }   
        
        [Test]
        public void ConvertVector_ReturnPlanePoint()
        {
            PlanePoint mainPoint = new PlanePoint(new double[] { 2, 2, 4 });
            Vector normal = new Vector(new double[] { 1, 0, 1 });
            Vector[] basis = new[]
            {
                new Vector(new double[] {-1, 0, 1}),
                new Vector(new double[] {0, 1, 0}),
            };
            Hyperplane hyperplane = new Hyperplane(mainPoint, normal);
            hyperplane.Basis = basis;
            Vector vector = new Vector(new double[]{2,3});
            Vector expect = new Vector(new double[] { -2, 3, 2 });

            Vector actual = hyperplane.ConvertVector(vector);

            actual.Should().Be(expect);
        }

        [Test, TestCaseSource("GetDataForConvertPoint")]
        public PlanePoint ConvertPoint_ReturnPlanePoint(Hyperplane h, Point p)
        {
            PlanePoint planePoint = h.ConvertPoint(p.ToPlanePoint());

            return planePoint;
        }

        private static IEnumerable<TestCaseData> GetDataForConvertPoint()
        {
            PlanePoint mainPoint = new PlanePoint(new double[] { 1, 1, 2 });
            Vector normal = new Vector(new double[] { 1, 0, 0 });
            Vector[] basis = new[]
            {
                new Vector(new double[] {1, 0, 0}),
                new Vector(new double[] {0, 1, 0}),
            };
            Hyperplane hyperplane = new Hyperplane(mainPoint, normal);
            hyperplane.Basis = basis;
            Point point = new Point(new double[] { 2, 2, 2 });
            Point expect = new Point(new double[] { 1, 1 });

            yield return new TestCaseData(hyperplane, point).Returns(expect);

            mainPoint = new PlanePoint(new double[] { 2, 2, 4 });
            normal = new Vector(new double[] { 2, 0, 2 });
            basis = new[]
            {
                new Vector(new double[] {-2, 0, 2}),
                new Vector(new double[] {0, 1, 0}),
            };
            hyperplane = new Hyperplane(mainPoint, normal);
            hyperplane.Basis = basis;
            point = new Point(new double[] { 2, 5, 4});
            expect = new Point(new double[] { 0, 3 });

            yield return new TestCaseData(hyperplane, point).Returns(expect);
        }
    }
}