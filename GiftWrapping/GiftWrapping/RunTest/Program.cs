using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GiftWrapping;
using GiftWrapping.Structures;

namespace RunTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //PlanePoint[] points1 = PointsReader.MakeVertices(10);
            //Read(@"D:\1.txt");
            //
            // List<PlanePoint> points = new List<PlanePoint> {
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

            List<PlanePoint> points = new List<PlanePoint> {
                new PlanePoint(new double[]{0, 0, 0}),
                new PlanePoint(new double[]{ 100, 0, 0}),
                new PlanePoint(new double[]{100, 100, 0}),
                new PlanePoint(new double[]{0, 100, 0}),
                new PlanePoint(new double[]{0, 0, 100}),
                new PlanePoint(new double[]{ 100, 0, 100}),
                new PlanePoint(new double[]{100, 100, 100}),
                new PlanePoint(new double[]{0, 100, 100}),

            };

            Random r = new Random();
   
            for (int i = 0; i < 100; i++)
            {
                points.Add(new PlanePoint(new double[] { r.Next(1,20), r.Next(1, 20), 100 }));
            }
      

            // List<PlanePoint> points = new List<PlanePoint> {
            //     new PlanePoint(new double[]{0, 0, 0, 0}),
            //     new PlanePoint(new double[]{0, 10, 0, 0}),
            //     new PlanePoint(new double[]{10, 10, 0, 0}),
            //     new PlanePoint(new double[]{0, 10, 0, 0}),
            //     new PlanePoint(new double[]{0, 0, 10, 0}),
            //     new PlanePoint(new double[]{0, 10, 10, 0}),
            //     new PlanePoint(new double[]{10, 10, 10, 0}),
            //     new PlanePoint(new double[]{0, 10, 10, 0}),
            //     new PlanePoint(new double[]{0, 0, 0, 10}),
            //     new PlanePoint(new double[]{0, 10, 0, 10}),
            //     new PlanePoint(new double[]{10, 10, 0, 10}),
            //     new PlanePoint(new double[]{0, 10, 0, 10}),
            //     new PlanePoint(new double[]{0, 0, 10, 10}),
            //     new PlanePoint(new double[]{0, 10, 10, 10}),
            //     new PlanePoint(new double[]{10, 10, 10, 10}),
            //     new PlanePoint(new double[]{0, 10, 10, 10}),
            //     // new PlanePoint(new double[]{5, 5, 5}),
            //     // new PlanePoint(new double[]{3, 3, 3}),
            //     // new PlanePoint(new double[]{3, 3, 3}),
            //     // new PlanePoint(new double[]{3, 3, 3}),
            //     // new PlanePoint(new double[]{2, 2, 2}),
            //     // new PlanePoint(new double[]{5.5, 5.5, 5.5}),
            //     // new PlanePoint(new double[]{ 3.5, 3.5, 3.5}),
            //     // new PlanePoint(new double[]{ 3.5, 3.5, 3.5}),
            //     // new PlanePoint(new double[]{ 3.5, 3.5, 3.5}),
            //     // new PlanePoint(new double[]{ 2.5, 2.5, 2.5}),
            // };





            // List<PlanePoint> points = new List<PlanePoint> {
            //     new PlanePoint(new double[]{0, 0, 0, 0}),
            //     new PlanePoint(new double[]{10, 0, 0, 0}),
            //     new PlanePoint(new double[]{0, 10, 0, 0}),
            //     new PlanePoint(new double[]{0, 0, 10, 0}),
            //     new PlanePoint(new double[]{0, 0, 0, 10}),
            //     new PlanePoint(new double[]{1, 0, 0, 0}),
            //     new PlanePoint(new double[]{2, 0, 0, 0}),
            //     new PlanePoint(new double[]{3, 0, 0, 0}),
            //     new PlanePoint(new double[]{4, 0, 0, 0}),
            //     new PlanePoint(new double[]{5, 0, 0, 0}),
            //     new PlanePoint(new double[]{6, 0, 0, 0}),
            //     new PlanePoint(new double[]{7, 0, 0, 0}),
            //     new PlanePoint(new double[]{8, 0, 0, 0}),
            //     new PlanePoint(new double[]{8.5, 0, 0, 0}),
            //     new PlanePoint(new double[]{8.8, 0, 0, 0}),
            //     new PlanePoint(new double[]{9, 0, 0, 0}),
            //
            // };
            // new PlanePoint(new double[] { 0, 0, 0, 0 }),
            // new PlanePoint(new double[] { 10, 0, 0, 0 }),
            // new PlanePoint(new double[] { 0, 10, 0, 0 }),
            // new PlanePoint(new double[] { 10, 10, 0, 0 }),
            // new PlanePoint(new double[] { 0, 0, 10, 0 }),
            // new PlanePoint(new double[] { 10, 0, 10, 0 }),
            // new PlanePoint(new double[] { 0, 10, 10, 0 }),
            // new PlanePoint(new double[] { 10, 10, 10, 0 }),
            // new PlanePoint(new double[] { 0, 0, 0, 10 }),
            // new PlanePoint(new double[] { 10, 0, 0, 10 }),
            // new PlanePoint(new double[] { 0, 10, 0, 10 }),
            // new PlanePoint(new double[] { 10, 10, 0, 10 }),
            // new PlanePoint(new double[] { 0, 0, 10, 10 }),
            // new PlanePoint(new double[] { 10, 0, 10, 10 }),
            // new PlanePoint(new double[] { 0, 10, 10, 10 }),
            // new PlanePoint(new double[] { 10, 10, 10, 10 }),
       
            //
            //     new PlanePoint(new double[]{5, 5, 5, 5}),
            //     new PlanePoint(new double[]{3, 3, 3, 3}),
            //     new PlanePoint(new double[]{3, 3, 3, 3}),
            //     new PlanePoint(new double[]{3, 3, 3, 3}),
            //     new PlanePoint(new double[]{2, 2, 2, 2}),
            //     new PlanePoint(new double[]{5.5, 5.5, 5.5, 5.5}),
            //     new PlanePoint(new double[]{ 3.5, 3.5, 3.5, 3.5}),
            //     new PlanePoint(new double[]{ 3.5, 3.5, 3.5, 3.5}),
            //     new PlanePoint(new double[]{ 3.5, 3.5, 3.5, 3.5}),
            //     new PlanePoint(new double[]{ 2.5, 2.5, 2.5, 2.5}),
            //
            // };


            //Random r = new Random();
            //List<PlanePoint> points = new List<PlanePoint>();
            //for (int i = 0; i < 10; i++)
            //{
            //    points.Add(new PlanePoint(new double[] { r.NextDouble(), r.NextDouble(), r.NextDouble() }));
            //}

            // PlanePoint[] points = new PlanePoint[] {
            //      new PlanePoint(new double[]{1, 1, 1, 1,1}),
            //      new PlanePoint(new double[]{5, 1, 1, 1,1}),
            //      new PlanePoint(new double[]{1, 5, 1, 1,1}),
            //      new PlanePoint(new double[]{5, 5, 1,1,1}),
            //      new PlanePoint(new double[]{1, 1, 5,1,1}),
            //      new PlanePoint(new double[]{5, 1, 5,1,1}),
            //      new PlanePoint(new double[]{1, 5, 5,1,1}),
            //      new PlanePoint(new double[]{5, 5, 5,1,1}),
            //      new PlanePoint(new double[]{1, 1, 1, 5,1}),
            //      new PlanePoint(new double[]{5, 1, 1, 5,1}),
            //      new PlanePoint(new double[]{1, 5, 1, 5,1}),
            //      new PlanePoint(new double[]{5, 5, 1,5,1}),
            //      new PlanePoint(new double[]{1, 1, 5,5,1}),
            //      new PlanePoint(new double[]{5, 1, 5,5,1}),
            //      new PlanePoint(new double[]{1, 5, 5,5,1}),
            //      new PlanePoint(new double[]{5, 5, 5,5,1}),
            //      new PlanePoint(new double[]{1, 1, 1, 1,5}),
            //      new PlanePoint(new double[]{5, 1, 1, 1,5}),
            //      new PlanePoint(new double[]{1, 5, 1, 1,5}),
            //      new PlanePoint(new double[]{5, 5, 1,1,5}),
            //      new PlanePoint(new double[]{1, 1, 5,1,5}),
            //      new PlanePoint(new double[]{5, 1, 5,1,5}),
            //      new PlanePoint(new double[]{1, 5, 5,1,5}),
            //      new PlanePoint(new double[]{5, 5, 5,1,5}),
            //      new PlanePoint(new double[]{1, 1, 1, 5,5}),
            //      new PlanePoint(new double[]{5, 1, 1, 5,5}),
            //      new PlanePoint(new double[]{1, 5, 1, 5,5}),
            //      new PlanePoint(new double[]{5, 5, 1,5,5}),
            //      new PlanePoint(new double[]{1, 1, 5,5,5}),
            //      new PlanePoint(new double[]{5, 1, 5,5,5}),
            //      new PlanePoint(new double[]{1, 5, 5,5,5}),
            //      new PlanePoint(new double[]{5, 5, 5,5,5}),
            //
            //  };
            //Point min = points1.Min();
            //min = -min;
            //for (int i = 0; i < points1.Length; i++)
            //{
            //    points1[i] = new PlanePoint(((Point)points1[i]) + min);
            //}

            GiftWrappingAlgorithm giftWrapping = new GiftWrappingAlgorithm(points);
            Stopwatch sp = new Stopwatch();
             IFace result = giftWrapping.Create();
            Console.Out.WriteLine("Start");
            sp.Start();
            IFace result1 = giftWrapping.Create();
            sp.Stop();
            
             Console.Out.WriteLine("Stop");
            // ((ConvexHull)result1).Convert(@"D:\", "dode21");
            Console.Out.WriteLine("ms = {0}", sp.ElapsedMilliseconds);
            Console.Out.WriteLine("t = {0}", sp.ElapsedTicks);
        }
    }
}
