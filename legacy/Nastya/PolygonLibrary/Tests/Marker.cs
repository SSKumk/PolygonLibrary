using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using PolygonLibrary;
using PolygonLibrary.Basics;
using PolygonLibrary.Segments;
using System.Collections;
using PolygonLibrary.Rearranging;
using PolygonLibrary.Build;
using PolygonLibrary.Marker;
namespace Tests
{
    public class MyWrite {

        public static void myWrite(List<List<EdgeMarker>> res, string file)
        {
            StreamWriter sw = new StreamWriter("C:/Users/1/Desktop/Универ/Курсовая/1/GNUPlot/"+file);
            for (int i = 0; i < res.Count; i++)
            {
                List<EdgeMarker> res1 = new List<EdgeMarker>(res[i]);
                for (int j = 0; j < res1.Count; j++) 
                {

                    sw.WriteLine(res1[j].edge.p1.x + " " + res1[j].edge.p1.y);
                    
                }
                sw.WriteLine(res1[0].edge.p1.x + " " + res1[0].edge.p1.y);
                sw.WriteLine(" ");
            }
            
            sw.Close();            
        }


        public static void myWritePoint(List<List<Point2D>> res, string file)
        {
            StreamWriter sw = new StreamWriter("C:/Users/1/Desktop/Универ/Курсовая/1/GNUPlot/" + file);
            for (int i = 0; i < res.Count; i++)
            {
                List<Point2D> res1 = new List<Point2D>(res[i]);
                for (int j = 0; j < res1.Count; j++)
                {

                    sw.WriteLine(res1[j].x + " " + res1[j].y);

                }
                sw.WriteLine(res1[0].x + " " + res1[0].y);
                sw.WriteLine(" ");
            }
            sw.Close();
        }
    
    
    }

    [TestClass]
    public class MarkerTest
    {

        //[TestMethod]
        //public void M1Test()
        //{
            //Segment s1 = new Segment(-3, 0, 4, -1);
            //Segment s2 = new Segment(4, -1, 1, 0);
            //Segment s3 = new Segment(1, 0, 2, 2);
        //    //Segment s4 = new Segment(2, 2, -2, 2);

        //    //Segment s5 = new Segment(-2, 2, -1, 1);
        //    //Segment s6 = new Segment(-1, 1, -3, 0);
        //    Segment s1 = new Segment(0, 0, 0, 1);
        //    Segment s2 = new Segment(0, 1, 1, 1);
        //    Segment s3 = new Segment(1, 1, 1, 0);
        //    Segment s4 = new Segment(1, 0, 0, 0);
        //    //Segment s1 = new Segment(0, 0, 1, 0);
        //    //Segment s2 = new Segment(1, 0, 1, 1);
        //    //Segment s3 = new Segment(1, 1, 0, 1);
        //    //Segment s4 = new Segment(0, 1, 0, 0);
        //    //Segment s5 = new Segment(-2, 2, -1, 1);
        //    //Segment s6 = new Segment(-1, 1, -3, 0);
        //    List<Segment> m = new List<Segment>();
        //    m.Add(s1);
        //    m.Add(s2);
        //    m.Add(s3);
        //    m.Add(s4);
        //    //m.Add(s5);
        //    //m.Add(s6);
        //    MarkerBasis.Inside(new Point2D(0.5, 0.5), m);

        //}
        //[TestMethod]
        //public void M2Test()
        //{
        //    Segment s1 = new Segment(-3, 0, 4, -1);
        //    Segment s2 = new Segment(4, -1, 1, 0);
        //    Segment s3 = new Segment(1, 0, 2, 2);
        //    Segment s4 = new Segment(2, 2, -2, 2);

        //    Segment s5 = new Segment(-2, 2, -1, 1);
        //    Segment s6 = new Segment(-1, 1, -3, 0);


        //   // MarkerBasis.argum(new Point2D(0, 0), s2);

        //}
        //[TestMethod]
        //public void M3Test()
        //{

        //    List<List<Point2D>> m1 = new List<List<Point2D>>();
        //    List<Point2D> k = new List<Point2D>();
        //    k.Add(new Point2D(0, 0));
        //    k.Add(new Point2D(1, 0));
        //    k.Add(new Point2D(1, 1));
        //    k.Add(new Point2D(0, 1));
        //    m1.Add(k);

        //    List<List<Point2D>> m2 = new List<List<Point2D>>();
        //    List<Point2D> k1 = new List<Point2D>();
        //    k1.Add(new Point2D(0.5, 0.5));
        //    k1.Add(new Point2D(1.5, 0.5));
        //    k1.Add(new Point2D(1.5, 1.5));
        //    k1.Add(new Point2D(0.5, 1.5));
        //    m2.Add(k1);

        //    List<Point2D> k2 = new List<Point2D>();
        //    k2.Add(new Point2D(-0.5, 0));
        //    k2.Add(new Point2D(-1, 0));
        //    k2.Add(new Point2D(-1, -1));
        //    k2.Add(new Point2D(0, -1));
        //    //m1.Add(k2);

        //    List<List<Segment>> s1, s2;
        //    SortedDictionary<Point2D, SortedSet<Descriptor>> PointDictionary = new SortedDictionary<Point2D, SortedSet<Descriptor>>();

        //    Rearranger.polygonRearranging(m1, m2, out s1, out s2, out PointDictionary);
        //   // MarkerBasis.markerContour(s1[0], s2, PointDictionary);
        //   // MarkerBasis.insideCorner(new Segment(1, 0.5, 1, 0), PointDictionary[new Point2D(1, 0.5)], 1);




        //}
        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        [TestMethod]
        public void M1Test()
        {
            List<List<Point2D>> m1 = new List<List<Point2D>>();
            List<Point2D> k = new List<Point2D>();
            k.Add(new Point2D(-1, 5));
            k.Add(new Point2D(-1, -1));
            k.Add(new Point2D(4, -1));
           

            m1.Add(k);

            List<List<Point2D>> m2 = new List<List<Point2D>>();
            List<Point2D> k1 = new List<Point2D>();
            k1.Add(new Point2D(-6, 1));
            k1.Add(new Point2D(-6, -4));
            k1.Add(new Point2D(1, -4));
            k1.Add(new Point2D(1, 1));
            m2.Add(k1);



            List<List<Segment>> s1, s2;
            SortedDictionary<Point2D, SortedSet<Descriptor>> PointDictionary = new SortedDictionary<Point2D, SortedSet<Descriptor>>();

            //Rearranging.newR1(m1, m2, out s1, out s2, out PointDictionary);
            // MarkerBasis.markerContour(s1[0], s2, PointDictionary);
            //MarkerBasis.insideCorner(new Segment(1, 0.5, 1, 0), PointDictionary[new Point2D(1, 0.5)], 1);
            //  MarkerBasis.markerEdge(s1[0], s2, PointDictionary, 1);
            MyWrite.myWritePoint(m1, "Test1M1.txt");
            MyWrite.myWritePoint(m2, "Test1M2.txt");
            List<List<EdgeMarker>> res = new List<List<EdgeMarker>>();
            BuildResult.buildResult(m1, m2, "SIMSUB", out res);
            MyWrite.myWrite(res, "Test1ResSIMSUB.txt");
            BuildResult.buildResult(m1, m2, "SUB", out res);
            MyWrite.myWrite(res, "Test1ResM1SUBM2.txt");
            BuildResult.buildResult(m1, m2, "OR", out res);
            MyWrite.myWrite(res, "Test1ResOR.txt");
            BuildResult.buildResult(m1, m2, "AND", out res);
            MyWrite.myWrite(res, "Test1ResAND.txt");
            BuildResult.buildResult(m2, m1, "SUB", out res);
            MyWrite.myWrite(res, "Test1ResM2SUBM1.txt");

        }

        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        [TestMethod]
        public void M2Test()
        {
            List<List<Point2D>> m1 = new List<List<Point2D>>();
            List<Point2D> k = new List<Point2D>();
            k.Add(new Point2D(-2, -1));
            k.Add(new Point2D(3, -1));
            k.Add(new Point2D(3, 3));
            k.Add(new Point2D(-2, 3));

            m1.Add(k);

            List<List<Point2D>> m2 = new List<List<Point2D>>();
            List<Point2D> k1 = new List<Point2D>();
            k1.Add(new Point2D(-1, 3));
            k1.Add(new Point2D(2, 3));
            k1.Add(new Point2D(2, 6));
            k1.Add(new Point2D(-1, 6));
            m2.Add(k1);



            List<List<Segment>> s1, s2;
            SortedDictionary<Point2D, SortedSet<Descriptor>> PointDictionary = new SortedDictionary<Point2D, SortedSet<Descriptor>>();

            //Rearranging.newR1(m1, m2, out s1, out s2, out PointDictionary);
            // MarkerBasis.markerContour(s1[0], s2, PointDictionary);
            //MarkerBasis.insideCorner(new Segment(1, 0.5, 1, 0), PointDictionary[new Point2D(1, 0.5)], 1);
            //  MarkerBasis.markerEdge(s1[0], s2, PointDictionary, 1);
            MyWrite.myWritePoint(m1, "Test2M1.txt");
            MyWrite.myWritePoint(m2, "Test2M2.txt");
            List<List<EdgeMarker>> res = new List<List<EdgeMarker>>();
            BuildResult.buildResult(m1, m2, "SIMSUB", out res);
            MyWrite.myWrite(res, "Test2ResSIMSUB.txt");
            BuildResult.buildResult(m1, m2, "SUB", out res);
            MyWrite.myWrite(res, "Test2ResM1SUBM2.txt");
            BuildResult.buildResult(m1, m2, "OR", out res);
            MyWrite.myWrite(res, "Test2ResOR.txt");
            BuildResult.buildResult(m1, m2, "AND", out res);
            MyWrite.myWrite(res, "Test2ResAND.txt");
            BuildResult.buildResult(m2, m1, "SUB", out res);
            MyWrite.myWrite(res, "Test2ResM2SUBM1.txt");

        }


        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        [TestMethod]
        public void M3Test()
        {
            List<List<Point2D>> m1 = new List<List<Point2D>>();
            List<Point2D> k = new List<Point2D>();
            k.Add(new Point2D(-5, 7));
            k.Add(new Point2D(-2, 1));
            k.Add(new Point2D(0, 5));
            k.Add(new Point2D(2, 1));
            k.Add(new Point2D(5, 7));

            m1.Add(k);

            List<List<Point2D>> m2 = new List<List<Point2D>>();
            List<Point2D> k1 = new List<Point2D>();
            k1.Add(new Point2D(-4, 3));
            k1.Add(new Point2D(-4, 0));
            k1.Add(new Point2D(0, -4));
            k1.Add(new Point2D(4, 0));
            k1.Add(new Point2D(4, 3));
            m2.Add(k1);



            List<List<Segment>> s1, s2;
            SortedDictionary<Point2D, SortedSet<Descriptor>> PointDictionary = new SortedDictionary<Point2D, SortedSet<Descriptor>>();

            //Rearranging.newR1(m1, m2, out s1, out s2, out PointDictionary);
            // MarkerBasis.markerContour(s1[0], s2, PointDictionary);
            //MarkerBasis.insideCorner(new Segment(1, 0.5, 1, 0), PointDictionary[new Point2D(1, 0.5)], 1);
            //  MarkerBasis.markerEdge(s1[0], s2, PointDictionary, 1);
            MyWrite.myWritePoint(m1, "Test3M1.txt");
            MyWrite.myWritePoint(m2, "Test3M2.txt");
            List<List<EdgeMarker>> res = new List<List<EdgeMarker>>();
            BuildResult.buildResult(m1, m2, "SIMSUB", out res);
            MyWrite.myWrite(res, "Test3ResSIMSUB.txt");
            BuildResult.buildResult(m1, m2, "SUB", out res);
            MyWrite.myWrite(res, "Test3ResM1SUBM2.txt");
            BuildResult.buildResult(m1, m2, "OR", out res);
            MyWrite.myWrite(res, "Test3ResOR.txt");
            BuildResult.buildResult(m1, m2, "AND", out res);
            MyWrite.myWrite(res, "Test3ResAND.txt");
            BuildResult.buildResult(m2, m1, "SUB", out res);
            MyWrite.myWrite(res, "Test3ResM2SUBM1.txt");

        }

        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        [TestMethod]
        public void M4Test()
        {
            List<List<Point2D>> m1 = new List<List<Point2D>>();
            List<Point2D> k = new List<Point2D>();
            k.Add(new Point2D(-3, 6));
            k.Add(new Point2D(-5, 4));
            k.Add(new Point2D(-3, -1));
            k.Add(new Point2D(-3, 1));
            k.Add(new Point2D(-2, 1));
            k.Add(new Point2D(-3, 4));
            k.Add(new Point2D(3, -3));
            k.Add(new Point2D(3, -1));

            m1.Add(k);

            List<List<Point2D>> m2 = new List<List<Point2D>>();
            List<Point2D> k1 = new List<Point2D>();
            k1.Add(new Point2D(-3, 1));
            k1.Add(new Point2D(-3, -3));
            k1.Add(new Point2D(3, -3));
            k1.Add(new Point2D(3, 1));
         
            m2.Add(k1);



            List<List<Segment>> s1, s2;
            SortedDictionary<Point2D, SortedSet<Descriptor>> PointDictionary = new SortedDictionary<Point2D, SortedSet<Descriptor>>();

            //Rearranging.newR1(m1, m2, out s1, out s2, out PointDictionary);
            // MarkerBasis.markerContour(s1[0], s2, PointDictionary);
            //MarkerBasis.insideCorner(new Segment(1, 0.5, 1, 0), PointDictionary[new Point2D(1, 0.5)], 1);
            //  MarkerBasis.markerEdge(s1[0], s2, PointDictionary, 1);
            MyWrite.myWritePoint(m1, "Test4M1.txt");
            MyWrite.myWritePoint(m2, "Test4M2.txt");
            List<List<EdgeMarker>> res = new List<List<EdgeMarker>>();
            BuildResult.buildResult(m1, m2, "SIMSUB", out res);
            MyWrite.myWrite(res, "Test4ResSIMSUB.txt");
            BuildResult.buildResult(m1, m2, "SUB", out res);
            MyWrite.myWrite(res, "Test4ResM1SUBM2.txt");
            BuildResult.buildResult(m1, m2, "OR", out res);
            MyWrite.myWrite(res, "Test4ResOR.txt");
            BuildResult.buildResult(m1, m2, "AND", out res);
            MyWrite.myWrite(res, "Test4ResAND.txt");
            BuildResult.buildResult(m2, m1, "SUB", out res);
            MyWrite.myWrite(res, "Test4ResM2SUBM1.txt");

        }


        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        [TestMethod]
        public void M5Test()
        {
            List<List<Point2D>> m1 = new List<List<Point2D>>();
            List<Point2D> k = new List<Point2D>();
            k.Add(new Point2D(-2, 7));
            k.Add(new Point2D(-2, 2));
            k.Add(new Point2D(-1, 2));
            k.Add(new Point2D(-1, 5));
            k.Add(new Point2D(1, 5));
            k.Add(new Point2D(1, 2));
            k.Add(new Point2D(2, 2));
            k.Add(new Point2D(2, 7));

            m1.Add(k);

            List<List<Point2D>> m2 = new List<List<Point2D>>();
            List<Point2D> k1 = new List<Point2D>();
            k1.Add(new Point2D(-3, 2));
            k1.Add(new Point2D(-3, -4));
            k1.Add(new Point2D(3, -4));
            k1.Add(new Point2D(3, 2));

            m2.Add(k1);



            List<List<Segment>> s1, s2;
            SortedDictionary<Point2D, SortedSet<Descriptor>> PointDictionary = new SortedDictionary<Point2D, SortedSet<Descriptor>>();

            //Rearranging.newR1(m1, m2, out s1, out s2, out PointDictionary);
            // MarkerBasis.markerContour(s1[0], s2, PointDictionary);
            //MarkerBasis.insideCorner(new Segment(1, 0.5, 1, 0), PointDictionary[new Point2D(1, 0.5)], 1);
            //  MarkerBasis.markerEdge(s1[0], s2, PointDictionary, 1);
            MyWrite.myWritePoint(m1, "Test5M1.txt");
            MyWrite.myWritePoint(m2, "Test5M2.txt");
            List<List<EdgeMarker>> res = new List<List<EdgeMarker>>();
            BuildResult.buildResult(m1, m2, "SIMSUB", out res);
            MyWrite.myWrite(res, "Test5ResSIMSUB.txt");
            BuildResult.buildResult(m1, m2, "SUB", out res);
            MyWrite.myWrite(res, "Test5ResM1SUBM2.txt");
            BuildResult.buildResult(m1, m2, "OR", out res);
            MyWrite.myWrite(res, "Test5ResOR.txt");
            BuildResult.buildResult(m1, m2, "AND", out res);
            MyWrite.myWrite(res, "Test5ResAND.txt");
            BuildResult.buildResult(m2, m1, "SUB", out res);
            MyWrite.myWrite(res, "Test5ResM2SUBM1.txt");

        }

        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        [TestMethod]
        public void M6Test()
        {
            List<List<Point2D>> m1 = new List<List<Point2D>>();
            List<Point2D> k = new List<Point2D>();
            List<Point2D> k2 = new List<Point2D>();
            k.Add(new Point2D(-1, 5));
            k.Add(new Point2D(-1, -1));
            k.Add(new Point2D(5, -1));
            k.Add(new Point2D(5, 5));


            k2.Add(new Point2D(1, 4));
            k2.Add(new Point2D(4, 4));
            k2.Add(new Point2D(4, 1));
            k2.Add(new Point2D(1, 1));

            m1.Add(k);
            m1.Add(k2);




            List<List<Point2D>> m2 = new List<List<Point2D>>();
            List<Point2D> k1 = new List<Point2D>();
            List<Point2D> k3 = new List<Point2D>();
            k1.Add(new Point2D(-4, 2));
            k1.Add(new Point2D(-4, -4));
            k1.Add(new Point2D(2, -4));
            k1.Add(new Point2D(2, 2));

            k3.Add(new Point2D(-2, 0));
            k3.Add(new Point2D(0, 0));
            k3.Add(new Point2D(0, -2));
            k3.Add(new Point2D(-2, -2));

            m2.Add(k1);

            m2.Add(k3);

            List<List<Segment>> s1, s2;
            SortedDictionary<Point2D, SortedSet<Descriptor>> PointDictionary = new SortedDictionary<Point2D, SortedSet<Descriptor>>();

            //Rearranging.newR1(m1, m2, out s1, out s2, out PointDictionary);
            // MarkerBasis.markerContour(s1[0], s2, PointDictionary);
            //MarkerBasis.insideCorner(new Segment(1, 0.5, 1, 0), PointDictionary[new Point2D(1, 0.5)], 1);
            //  MarkerBasis.markerEdge(s1[0], s2, PointDictionary, 1);
            MyWrite.myWritePoint(m1, "Test6M1.txt");
            MyWrite.myWritePoint(m2, "Test6M2.txt");
            List<List<EdgeMarker>> res = new List<List<EdgeMarker>>();
            BuildResult.buildResult(m1, m2, "SIMSUB", out res);
            MyWrite.myWrite(res, "Test6ResSIMSUB.txt");
            BuildResult.buildResult(m1, m2, "SUB", out res);
            MyWrite.myWrite(res, "Test6ResM1SUBM2.txt");
            BuildResult.buildResult(m1, m2, "OR", out res);
            MyWrite.myWrite(res, "Test6ResOR.txt");
            BuildResult.buildResult(m1, m2, "AND", out res);
            MyWrite.myWrite(res, "Test6ResAND.txt");
            BuildResult.buildResult(m2, m1, "SUB", out res);
            MyWrite.myWrite(res, "Test6ResM2SUBM1.txt");

        }



        
        [TestMethod]
        public void M7Test()
        {
            List<List<Point2D>> m1 = new List<List<Point2D>>();
            List<Point2D> k = new List<Point2D>();
            k.Add(new Point2D(0, 2));
            k.Add(new Point2D(4, 0));
            k.Add(new Point2D(1, -4));
            k.Add(new Point2D(6, -4));
            k.Add(new Point2D(4, 0));
            k.Add(new Point2D(4, 4));
            m1.Add(k);

            List<List<Point2D>> m2 = new List<List<Point2D>>();
            List<Point2D> k1 = new List<Point2D>();
             k1.Add(new Point2D(2, 5));
            k1.Add(new Point2D(2, 2));
            k1.Add(new Point2D(6, 2));
            k1.Add(new Point2D(6, 5));
           
            m2.Add(k1);



            List<List<Segment>> s1, s2;
            SortedDictionary<Point2D, SortedSet<Descriptor>> PointDictionary = new SortedDictionary<Point2D, SortedSet<Descriptor>>();
            List<List<EdgeMarker>> res = new List<List<EdgeMarker>>();

            // Rearranging.newR1(m2, m1, out s1, out s2, out PointDictionary);
            //   BuildResult.listOftrue(PointDictionary[new Point2D(4, 0)], 1, 0, 0);
            //MarkerBasis.markerContour(s1[0], s2, PointDictionary,1);
            // MarkerBasis.insideCorner2(new Segment(4, 4, 2, 3), PointDictionary[new Point2D(2, 3)], 1);
            // MarkerBasis.markerEdge(s1[0], s2, PointDictionary, 1);
            //  BuildResult.P(m1, m2, "SIMSUB",out res);
            MyWrite.myWritePoint(m1, "Test7M1.txt");
            MyWrite.myWritePoint(m2, "Test7M2.txt");
            BuildResult.buildResult(m1, m2, "SUB", out res);
            MyWrite.myWrite(res, "Test7ResM1SUBM2.txt");
            BuildResult.buildResult(m1, m2, "OR", out res);
            MyWrite.myWrite(res, "Test7ResOR.txt");
            BuildResult.buildResult(m1, m2, "AND", out res);
            MyWrite.myWrite(res, "Test7ResAND.txt");
            BuildResult.buildResult(m2, m1, "SUB", out res);
            MyWrite.myWrite(res, "Test7ResM2SUBM1.txt");
            BuildResult.buildResult(m2, m1, "SIMSUB", out res);
            MyWrite.myWrite(res, "Test7ResSIMSUB.txt");
        }
        ///// <summary>
        ///// /////////////////////////////////////////////////////////
        ///// </summary>
        [TestMethod]
        public void M8Test()
        {
            List<List<Point2D>> m1 = new List<List<Point2D>>();
            List<Point2D> k = new List<Point2D>();
            k.Add(new Point2D(2, 6));
            k.Add(new Point2D(2, -3));
            k.Add(new Point2D(5, -3));
            k.Add(new Point2D(3, -1));
            k.Add(new Point2D(9, 3));
            k.Add(new Point2D(9, 6));
            m1.Add(k);
            List<Point2D> k2 = new List<Point2D>();
            k2.Add(new Point2D(3, 5));
            k2.Add(new Point2D(6, 5));
            k2.Add(new Point2D(6, 2));
            k2.Add(new Point2D(3, 2));
            m1.Add(k2);

            List<List<Point2D>> m2 = new List<List<Point2D>>();
            List<Point2D> k1 = new List<Point2D>();
            k1.Add(new Point2D(1, 1));
            k1.Add(new Point2D(4, 1));
            k1.Add(new Point2D(4, 3));
            k1.Add(new Point2D(1, 3));
            m2.Add(k1);


          

            List<List<Segment>> s1, s2;
            SortedDictionary<Point2D, SortedSet<Descriptor>> PointDictionary = new SortedDictionary<Point2D, SortedSet<Descriptor>>();

            Rearranger.polygonRearranging(m1, m2, out s1, out s2, out PointDictionary);
            //MarkerBasis.markerContour(s1[0], s2, PointDictionary);
            //   MarkerBasis.insideCorner(new Segment(1, 0.5, 1, 0), PointDictionary[new Point2D(1, 0.5)], 1);
            MarkerBasis.markerEdge(s1[0], s2, PointDictionary, 1);
            List<List<EdgeMarker>> res = new List<List<EdgeMarker>>();
            MyWrite.myWritePoint(m1, "Test8M1.txt");
            MyWrite.myWritePoint(m2, "Test8M2.txt");
            BuildResult.buildResult(m1, m2, "SIMSUB", out res);
            MyWrite.myWrite(res, "Test8ResSIMSUB.txt");
            BuildResult.buildResult(m1, m2, "SUB", out res);
            MyWrite.myWrite(res, "Test8ResM1SUBM2.txt");
            BuildResult.buildResult(m1, m2, "OR", out res);
            MyWrite.myWrite(res, "Test8ResOR.txt");
            BuildResult.buildResult(m1, m2, "AND", out res);
            MyWrite.myWrite(res, "Test8ResAND.txt");
            BuildResult.buildResult(m2, m1, "SUB", out res);
            MyWrite.myWrite(res, "Test8ResM2SUBM1.txt");

        }

        ///// <summary>
        ///// /////////////////////////////////////////////////////////
        ///// </summary>
        [TestMethod]
        public void M9Test()
        {
            List<List<Point2D>> m1 = new List<List<Point2D>>();
            List<Point2D> k = new List<Point2D>();
            k.Add(new Point2D(-2, 6));
            k.Add(new Point2D(-2, -2));
            k.Add(new Point2D(4, -2));
            k.Add(new Point2D(4, 6));
            m1.Add(k);
            List<Point2D> k2 = new List<Point2D>();
            k2.Add(new Point2D(-1, -1));
            k2.Add(new Point2D(-1, 0));
            k2.Add(new Point2D(1, 0));
            k2.Add(new Point2D(1, -1));
            m1.Add(k2);

            List<List<Point2D>> m2 = new List<List<Point2D>>();
            List<Point2D> k1 = new List<Point2D>();
            
            k1.Add(new Point2D(-4, 1));
            k1.Add(new Point2D(-4, -5));
            k1.Add(new Point2D(2, -5));
            k1.Add(new Point2D(2, 1));
            m2.Add(k1);
            List<Point2D> k3 = new List<Point2D>();
            k3.Add(new Point2D(-3, -3));
            k3.Add(new Point2D(-1, -3));
            k3.Add(new Point2D(-1, -4));
            k3.Add(new Point2D(-3, -4));
            m2.Add(k3);



            List<List<Segment>> s1, s2;
            SortedDictionary<Point2D, SortedSet<Descriptor>> PointDictionary = new SortedDictionary<Point2D, SortedSet<Descriptor>>();

            Rearranger.polygonRearranging(m1, m2, out s1, out s2, out PointDictionary);
            //MarkerBasis.markerContour(s1[0], s2, PointDictionary);
            //   MarkerBasis.insideCorner(new Segment(1, 0.5, 1, 0), PointDictionary[new Point2D(1, 0.5)], 1);
            MarkerBasis.markerEdge(s1[0], s2, PointDictionary, 1);
            List<List<EdgeMarker>> res = new List<List<EdgeMarker>>();
            MyWrite.myWritePoint(m1, "Test9M1.txt");
            MyWrite.myWritePoint(m2, "Test9M2.txt");
            BuildResult.buildResult(m1, m2, "SIMSUB", out res);
            MyWrite.myWrite(res, "Test9ResSIMSUB.txt");
            BuildResult.buildResult(m1, m2, "SUB", out res);
            MyWrite.myWrite(res, "Test9ResM1SUBM2.txt");
            BuildResult.buildResult(m1, m2, "OR", out res);
            MyWrite.myWrite(res, "Test9ResOR.txt");
           
            BuildResult.buildResult(m2, m1, "SUB", out res);
            MyWrite.myWrite(res, "Test9ResM2SUBM1.txt");
            BuildResult.buildResult(m1, m2, "AND", out res);
            MyWrite.myWrite(res, "Test9ResAND.txt");
        }


    }
}
