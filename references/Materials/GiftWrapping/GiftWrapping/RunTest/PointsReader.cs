using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using GiftWrapping.Structures;

namespace RunTest
{
    public static class PointsReader
    {
        public static PlanePoint[] Read(string path)
        {
            List<PlanePoint> points = new List<PlanePoint>();
            string line;
            using (StreamReader sr = new StreamReader(path))
            {
                while ( (line =  sr.ReadLine()) != null)
                {
                    double[] cords = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Select(s => double.Parse(s,CultureInfo.InvariantCulture)).ToArray();
                    PlanePoint p = new PlanePoint(cords);

                    points.Add(p);

                }
            }

            return points.ToArray();
        }



        public static PlanePoint[] MakeVertices(double side_length)
        {
            // Value t1 is actually never used.
            double s = side_length;
            //double t1 = 2.0 * Math.PI / 5.0;
            double t2 = Math.PI / 10.0;
            double t3 = 3.0 * Math.PI / 10.0;
            double t4 = Math.PI / 5.0;
            double d1 = s / 2.0 / Math.Sin(t4);
            double d2 = d1 * Math.Cos(t4);
            double d3 = d1 * Math.Cos(t2);
            double d4 = d1 * Math.Sin(t2);
            double Fx =
                (s * s - (2.0 * d3) * (2.0 * d3) -
                    (d1 * d1 - d3 * d3 - d4 * d4)) /
                        (2.0 * (d4 - d1));
            double d5 = Math.Sqrt(0.5 *
                (s * s + (2.0 * d3) * (2.0 * d3) -
                    (d1 - Fx) * (d1 - Fx) -
                        (d4 - Fx) * (d4 - Fx) - d3 * d3));
            double Fy = (Fx * Fx - d1 * d1 - d5 * d5) / (2.0 * d5);
            double Ay = d5 + Fy;

            PlanePoint A = new PlanePoint(d1, Ay, 0);
            PlanePoint B = new PlanePoint(d4, Ay, d3);
            PlanePoint C = new PlanePoint(-d2, Ay, s / 2);
            PlanePoint D = new PlanePoint(-d2, Ay, -s / 2);
            PlanePoint E = new PlanePoint(d4, Ay, -d3);
            PlanePoint F = new PlanePoint(Fx, Fy, 0);
            PlanePoint G = new PlanePoint(Fx * Math.Sin(t2), Fy,
                Fx * Math.Cos(t2));
            PlanePoint H = new PlanePoint(-Fx * Math.Sin(t3), Fy,
                Fx * Math.Cos(t3));
            PlanePoint I = new PlanePoint(-Fx * Math.Sin(t3), Fy,
                -Fx * Math.Cos(t3));
            PlanePoint J = new PlanePoint(Fx * Math.Sin(t2), Fy,
                -Fx * Math.Cos(t2));
            PlanePoint K = new PlanePoint(Fx * Math.Sin(t3), -Fy,
                Fx * Math.Cos(t3));
            PlanePoint L = new PlanePoint(-Fx * Math.Sin(t2), -Fy,
                Fx * Math.Cos(t2));
            PlanePoint M = new PlanePoint(-Fx, -Fy, 0);
            PlanePoint N = new PlanePoint(-Fx * Math.Sin(t2), -Fy,
                -Fx * Math.Cos(t2));
            PlanePoint O = new PlanePoint(Fx * Math.Sin(t3), -Fy,
                -Fx * Math.Cos(t3));
            PlanePoint P = new PlanePoint(d2, -Ay, s / 2);
            PlanePoint Q = new PlanePoint(-d4, -Ay, d3);
            PlanePoint R = new PlanePoint(-d1, -Ay, 0);
            PlanePoint S = new PlanePoint(-d4, -Ay, -d3);
            PlanePoint T = new PlanePoint(d2, -Ay, -s / 2);

            List<PlanePoint> points = new List<PlanePoint>();
            points.Add(A);
            points.Add(B);
            points.Add(C);
            points.Add(D);//3
            points.Add(E);
            points.Add(F);
            points.Add(G);
            points.Add(H);
            points.Add(I);//8
            points.Add(J);
            points.Add(K);
            points.Add(L);
            points.Add(M);//12
            points.Add(N);
            points.Add(O);
            points.Add(P);
            points.Add(Q);
            points.Add(R);
            points.Add(S);
            points.Add(T);

            return points.ToArray();
        }
    }
}