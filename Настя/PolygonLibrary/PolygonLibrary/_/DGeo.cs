using System;
using System.Collections.Generic;

namespace Robust
{
    class DGeo
    {
        public static bool ALow(double a, double b)
        {
            return ((b > a && b - a <= Math.PI) || (b <= 0 && a >= 0 && a - b >= Math.PI));
        }

        public static double Angle(DPoly p, int i)
        {
            Point a = p[i], b = p[Inc(p, i)];
            return Math.Atan2(a.x - b.x, b.y - a.y);
        }

        public static int Inc(DPoly p, int i)
        { 
            return i < p.nPoints - 1 ? i + 1 : 0; 
        }
    }
}
