using System;
using System.Collections.Generic;
using FluentAssertions;
using GiftWrapping.LinearEquations;
using GiftWrapping.Structures;
using NUnit.Framework;

namespace GiftWrappingTest.Structure_test
{
    [TestFixture]
    public class Edge2d_Test
    {
        public static IEnumerable<TestCaseData> GetTestEdges()
        {
            PlanePoint p1 = new PlanePoint(new double[] { 1, 1, 1, 0 });
            PlanePoint p2 = new PlanePoint(new double[] { 4, 4, 0, 0 });
            PlanePoint p3 = new PlanePoint(new double[] { 1, 1, 1, 0 });
            PlanePoint p4 = new PlanePoint(new double[] { 4, 4, 0, 0 });
            PlanePoint p5 = new PlanePoint(new double[] { 1, 1, 1, 1 });

            Edge edge1 = new Edge(p1, p2);
            Edge edge2 = new Edge(p3, p4);
            yield return new TestCaseData(edge1, edge2).SetName("{m}_When same point and same order.").Returns(true);

            Edge edge3 = new Edge(p4, p3);
            yield return new TestCaseData(edge1, edge3).SetName("{m}_Equals_When same point and different order.").Returns(true);

            Edge edge4 = new Edge(p4, p5);
            yield return new TestCaseData(edge1, edge4).SetName("{m}_Equals_When different point.").Returns(false);


            PlanePoint p11 = new PlanePoint(new double[] { 4, 4, 0, 0 });
            PlanePoint p21 = new PlanePoint(new double[] { 4, 4, 1, 0 });
            PlanePoint pp11 = new PlanePoint(new double[] { 1, 1, 1 }, p1);
            Point pp21 = new PlanePoint(new double[] { 2, 2, 2 }, p2);

            PlanePoint p31 = new PlanePoint(new double[] { 4, 4, 0, 0 });
            PlanePoint p41 = new PlanePoint(new double[] { 4, 4, 1, 0 });
            PlanePoint pp31 = new PlanePoint(new double[] { 1, 1, 2 }, p1);
            Point pp41 = new PlanePoint(new double[] { 2, 2, 3 }, p2);

            Edge edge11 = new Edge(p11, p21);
            Edge edge21 = new Edge(p41, p31);

            int i = edge11.GetHashCode();
            int i1 = edge21.GetHashCode();

            yield return new TestCaseData(edge11, edge21).SetName("{m}_When different point and order.").Returns(true);

        }

        [Test, TestCaseSource("GetTestEdges")]
        public bool Equals_WhenCalls(Edge point1, Edge point2)
        {
            return point1.Equals(point2);
        }

        [Test]
        public void Initialization_WhenSamePoints_ThrowsException()
        {
            PlanePoint p1 = new PlanePoint(new double[] { 1, 1, 1, 0 });
            PlanePoint p2 = new PlanePoint(new double[] { 1, 1, 1, 0 });


            Assert.Throws<ArgumentException>((() => new Edge(p1, p2)));
        }

        

        [Test]
        public void Initialization_WhenDifferentPoints_CreatesEdge2d()
        {
            PlanePoint p1 = new PlanePoint(new double[] { 1, 1, 1, 0 });
            PlanePoint p2 = new PlanePoint(new double[] { 0, 0, 0, 0 });


            Assert.DoesNotThrow((() => new Edge(p1, p2)));
        }

        [Test]
        public void Points_ContainsInsertedPoint()
        {
            PlanePoint p1 = new PlanePoint(new double[] { 1, 1, 1, 0 });
            PlanePoint p2 = new PlanePoint(new double[] { 0, 0, 0, 0 });
            Point[] points = new[] {p1, p2};

            Edge edge = new Edge(p1, p2);

            edge.GetPoints().Should().Equal(points);
        }
    }
}