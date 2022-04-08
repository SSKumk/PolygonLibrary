using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using PolygonLibrary;
using PolygonLibrary.Basics;
using PolygonLibrary.Segments;
using System.Collections;
//using PolygonLibrary.Rearranging;

namespace Tests
{
    //[TestClass]
    //public class RearrangingTest
    //{

    //    [TestMethod]
    //    public void R1Test()
    //    {
    //        Segment s = new Segment(0, 0, 1, 1);
    //        List<SegmentCrosser2.CrossData> k = new List<SegmentCrosser2.CrossData>();
    //        k.Add(new SegmentCrosser2.CrossData(new Segment(0, 0, 1, 1), new Segment(1, 0, 0, 1), new Point2D(0.5, 0.5)));
    //        k.Add(new SegmentCrosser2.CrossData(new Segment(0, 0, 1, 1), new Segment(1.25, 0, 0, 1.25), new Point2D(0.75, 0.75)));
    //        //Rearranging.divSegment(s, k);

    //    }
    //    [TestMethod]
    //    public void R2Test()
    //    {
            
    //        List<Point2D> k = new List<Point2D>();
    //        k.Add(new Point2D(1, 2));
    //        k.Add(new Point2D(5, 7));
    //        k.Add(new Point2D(7, 9));
    //        Rearranging.PointToSegment(k);
    //    }
    //    [TestMethod]
    //    public void R3Test()
    //    {
    //        List<Point2D> k = new List<Point2D>();
    //        k.Add(new Point2D(0, 0));
    //        k.Add(new Point2D(1, 0));
    //        k.Add(new Point2D(0, 1));
    //        k.Add(new Point2D(1, 1));
    //       // Rearranging.newR(k);
    //    }

    //    [TestMethod]
    //    public void R4Test()
    //    {
            
    //        List<List<Point2D>> m1 = new List<List<Point2D>>();
    //        List<Point2D> k = new List<Point2D>();
    //        k.Add(new Point2D(0, 0));
    //        k.Add(new Point2D(1, 0));
    //        k.Add(new Point2D(1, 1));
    //        k.Add(new Point2D(0, 1));
    //        m1.Add(k);
            
    //        List<List<Point2D>> m2 = new List<List<Point2D>>();
    //        List<Point2D> k1 = new List<Point2D>();
    //        k1.Add(new Point2D(0.5, 0.5));
    //        k1.Add(new Point2D(1.5, 0.5));
    //        k1.Add(new Point2D(1.5, 1.5));
    //        k1.Add(new Point2D(0.5, 1.5));
    //        m2.Add(k1);

    //        List<Point2D> k2 = new List<Point2D>();
    //        k2.Add(new Point2D(-0.5, 0));
    //        k2.Add(new Point2D(-1, 0));
    //        k2.Add(new Point2D(-1, -1));
    //        k2.Add(new Point2D(0, -1));
    //        m1.Add(k2);

    //        List<List<Segment>> s1, s2;
    //        SortedDictionary<Point2D, SortedSet<Descriptor>> PointDictionary = new SortedDictionary<Point2D, SortedSet<Descriptor>>();

    //        Rearranging.newR1(m1, m2, out s1, out s2, out PointDictionary);
    //    }
    //     [TestMethod]
    //    public void R5Test()
    //    {
    //        List<List<Point2D>> m1 = new List<List<Point2D>>();
    //        List<Point2D> k = new List<Point2D>();
    //        k.Add(new Point2D(0, 0));
    //        k.Add(new Point2D(1, 0));
    //        k.Add(new Point2D(1, 1));
    //        k.Add(new Point2D(0, 1));
    //        m1.Add(k);

    //        List<List<Point2D>> m2 = new List<List<Point2D>>();
    //        List<Point2D> k1 = new List<Point2D>();
    //        k1.Add(new Point2D(1, 1));
    //        k1.Add(new Point2D(5, 1));
    //        k1.Add(new Point2D(5, 4));
    //        k1.Add(new Point2D(8, 4));
    //        k1.Add(new Point2D(8, 6));
    //        k1.Add(new Point2D(5, 6));
    //        k1.Add(new Point2D(5, 4));
    //        k1.Add(new Point2D(1, 4));
            
    //        m2.Add(k1);

           

    //        List<List<Segment>> s1, s2;
    //        SortedDictionary<Point2D, SortedSet<Descriptor>> PointDictionary = new SortedDictionary<Point2D, SortedSet<Descriptor>>();

    //        Rearranging.newR1(m1, m2, out s1, out s2, out PointDictionary);
    //     }
    //     [TestMethod]
    //     public void R6Test()
    //     {
    //         List<List<Point2D>> m1 = new List<List<Point2D>>();
    //         List<Point2D> k = new List<Point2D>();
    //         k.Add(new Point2D(0, 0));
    //         k.Add(new Point2D(3, 0));
    //         k.Add(new Point2D(3, 3));
    //         k.Add(new Point2D(0, 3));
    //         m1.Add(k);

    //         List<List<Point2D>> m2 = new List<List<Point2D>>();
    //         List<Point2D> k1 = new List<Point2D>();
    //         k1.Add(new Point2D(2, 3));
    //         k1.Add(new Point2D(4, 3));
    //         k1.Add(new Point2D(4, 5));
    //         k1.Add(new Point2D(2, 5));
            

    //         m2.Add(k1);



    //         List<List<Segment>> s1, s2;
    //         SortedDictionary<Point2D, SortedSet<Descriptor>> PointDictionary = new SortedDictionary<Point2D, SortedSet<Descriptor>>();

    //         Rearranging.newR1(m1, m2, out s1, out s2, out PointDictionary);
    //     }
    //}


}
