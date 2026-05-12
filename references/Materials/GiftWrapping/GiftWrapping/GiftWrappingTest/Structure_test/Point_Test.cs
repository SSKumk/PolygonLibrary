using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using GiftWrapping;
using GiftWrapping.Structures;
using NUnit.Framework;

namespace GiftWrappingTest.Structure_test
{
    [TestFixture]
    public class Point_Test
    {
        [Test]
        public void Set_IndexOutOfRange_ThrowsException()
        {

            Point point = new Point(new double[3]);

            Exception ex = Assert.Catch<Exception>(() =>
            {
                double i = point[4];
            });

            StringAssert.Contains("Index was outside the bounds of the array", ex.Message);
        }


        [Test]
        public void ToString_WhenCall_ReturnsString()
        {

            Point point = new Point(new double[3]{1,2,3});
            string expect = "1 2 3";

            string result = point.ToString();

            result.Should().Equals(expect);
        }


        [Test]
        public void GetDimension_WhenCall_ReturnDimension()
        {
            int expectDimension = 555;
            Point point = new Point(new double[expectDimension]);

            int resultingDimension = point.Dim;

            Assert.AreEqual(expectDimension, resultingDimension);
        }

        [Test]
        public void Equals_EqualObjects_ReturnTrue()
        {
            Point p1 = new Point(new double[] {4, 4, 0, 0});
            Point p2 = new Point(new double[] {4, 4, 0, 0});

            Assert.AreEqual(p1, p2);
        }


        [Test]
        public void Equals_PointArrays_ReturnTrue()
        {
            Point p1 = new Point(new double[] { 1, 1, 1, 0 });
            Point p2 = new Point(new double[] { 4, 4, 0, 0 });
            Point p3 = new Point(new double[] { 1, 1, 1, 0 });
            Point p4 = new Point(new double[] { 4, 4, 0, 0 });
            Point[] points1 = new Point[] { p1, p2 };
            Point[] points2 = new Point[] { p3, p4 };


            Assert.AreEqual(points1, points2);
        }

        [Test]
        public void LessOperator_Point_ReturnTrue()
        {
            Point p1 = new Point(new double[] { 1, 100000, 1, 0 });
            Point p2 = new Point(new double[] { 1, 100, 0, 0 });

            bool result = p2 < p1;


            result.Should().BeTrue();
        }

    }
}