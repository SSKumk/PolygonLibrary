using System;
using System.Collections;

namespace Robust
{
  public class DPoly
  {
    protected readonly Point[] p;
    protected readonly bool empty;
    private const double prec = Constants.FloatPrec;

    public enum ADif
    {
      Out,
      In,
      EqA,
      EqB
    };

    public DPoly ()
    {
      p = null;
      empty = true;
    }

    public DPoly (Point[] points)
    {
      p = points;
      empty = false;
      SetDefNums ();
    }

    private void SetDefNums ()
    {
      for (int i = 0; i < p.Length; i++)
        p[i].num = i;
    }

    public bool isEmpty
    {
      get
      {
        return empty;
      }
    }

    public int nPoints
    {
      get
      {
        return !empty ? p.Length : 0;
      }
    }

    public Point this[int i]
    {
      get
      {
        return !empty ? p[i] : Point.Empty;
      }
      set
      {
        if (i >= 0 && i < p.Length)
          p[i] = value;
      }
   }

    public DPoly Plus (DPoly plus, bool isPrimary)
    {
      if (empty || plus.empty)
        return this;
      if (plus.nPoints == 1)
      {
        Point[] pp = new Point[p.Length];
        for (int i = 0; i < pp.Length; i++)
        {
          pp[i] = p[i] + plus[0];
          pp[i].num = i;
          if (isPrimary)
            pp[i].parent = p[i].num;
          else
            pp[i].parent = p[i].parent;
        }
        return new DPoly (pp);
      }
      double[] ap = new double[p.Length];
      for (int i = 0; i < ap.Length; i++)
        ap[i] = Angle (i);
      double[] aplus = new double[plus.p.Length];
      for (int i = 0; i < aplus.Length; i++)
        aplus[i] = plus.Angle (i);
      ArrayList curv = new ArrayList ();
      ArrayList v = new ArrayList ();
      int iplus = PlusStartNorm (plus, ap, aplus);

      // Main computation
      for (int idx = 0; idx < p.Length; idx++)
      {
        if (ALow (ap[Dec (idx)], ap[idx]))
        {
          ADif dif = ADifPlusConvex (ap[Dec (idx)], ap[idx], aplus[iplus]);
          if (dif != ADif.EqA)
          {
            Point tempPoint = p[idx] + plus[iplus];
            if (isPrimary)
              tempPoint.parent = p[idx].num;
            else
              tempPoint.parent = p[idx].parent;
            v.Add (tempPoint);
          }
          while (dif == ADif.In || dif == ADif.EqA)
          {
            iplus = plus.Inc (iplus);

            Point tempPoint = p[idx] + plus[iplus];
            if (isPrimary)
              tempPoint.parent = p[idx].num;
            else
              tempPoint.parent = p[idx].parent;
            v.Add (tempPoint);

            dif = ADifPlusConvex (ap[Dec (idx)], ap[idx], aplus[iplus]);
          }
        }
        else
        {
          ADif dif = ADifPlusConcave (ap[Dec (idx)], ap[idx], aplus[plus.Dec (iplus)]);
          if (dif == ADif.EqA)
          {
            iplus = plus.Dec (iplus);
            dif = ADifPlusConcave (ap[Dec (idx)], ap[idx], aplus[plus.Dec (iplus)]);
          }
          Point tempPoint = p[idx] + plus[iplus];
          if (isPrimary)
            tempPoint.parent = p[idx].num;
          else
            tempPoint.parent = p[idx].parent;
          v.Add (tempPoint);
          if (dif == ADif.In)
          {
            while (dif == ADif.In || dif == ADif.EqB)
            {
              iplus = plus.Dec (iplus);
              dif = ADifPlusConcave (ap[Dec (idx)], ap[idx], aplus[plus.Dec (iplus)]);
            }
            curv.Add (v);
            v = new ArrayList ();

            tempPoint = p[idx] + plus[iplus];
            if (isPrimary)
              tempPoint.parent = p[idx].num;
            else
              tempPoint.parent = p[idx].parent;
            v.Add (tempPoint);
          }
        }
      }
      return CurveIntersection (curv, v);
    }

    public DPoly Minus (DPoly minus, bool isPrimary)
    {
      if (empty || minus.empty)
        return this;

      if (minus.nPoints == 1)
      {
        Point[] pp = new Point[p.Length];
        for (int i = 0; i < pp.Length; i++)
        {
          pp[i] = p[i] - minus[0];
          pp[i].num = i;
          if (isPrimary)
            pp[i].parent = p[i].num;
          else
            pp[i].parent = p[i].parent;
        }
        return new DPoly(pp);
      }

      double[] ap = new double[p.Length];
      for (int i = 0; i < ap.Length; i++)
        ap[i] = Angle (i);
      double[] aminus = new double[minus.p.Length];
      for (int i = 0; i < aminus.Length; i++)
        aminus[i] = minus.Angle (i);
      ArrayList curv = new ArrayList ();
      ArrayList v = new ArrayList ();
      int iminus = MinusStartNorm (minus, ap, aminus);

      // Main computation
      for (int idx = 0; idx < p.Length; idx++)
      {
        if (ALow (ap[Dec (idx)], ap[idx]))
        {
          ADif dif = ADifMinusConvex (ap[Dec (idx)], ap[idx], aminus[iminus]);
          if (dif == ADif.EqA)
          {
            iminus = minus.Inc (iminus);
            dif = ADifMinusConvex (ap[Dec (idx)], ap[idx], aminus[iminus]);
          }
          Point tempPoint = p[idx] + minus[iminus];
          if (isPrimary)
            tempPoint.parent = p[idx].num;
          else
            tempPoint.parent = p[idx].parent;
          v.Add (tempPoint);
          if (dif == ADif.In)
          {
            while (dif == ADif.In || dif == ADif.EqB)
            {
              iminus = minus.Inc (iminus);
              dif = ADifMinusConvex (ap[Dec (idx)], ap[idx], aminus[iminus]);
            }
            curv.Add (v);
            v = new ArrayList ();
            tempPoint = p[idx] + minus[iminus];
            if (isPrimary)
              tempPoint.parent = p[idx].num;
            else
              tempPoint.parent = p[idx].parent;
            v.Add (tempPoint);
          }
        }
        else
        {
          ADif dif = ADifMinusConcave (ap[Dec (idx)], ap[idx], aminus[minus.Dec (iminus)]);
          if (dif != ADif.EqA)
          {
            Point tempPoint = p[idx] + minus[iminus];
            if (isPrimary)
              tempPoint.parent = p[idx].num;
            else
              tempPoint.parent = p[idx].parent;
            v.Add (tempPoint);
          }
          while (dif == ADif.In || dif == ADif.EqA)
          {
            iminus = minus.Dec (iminus);

            Point tempPoint = p[idx] + minus[iminus];
            if (isPrimary)
              tempPoint.parent = p[idx].num;
            else
              tempPoint.parent = p[idx].parent;
            v.Add (tempPoint);
            
            dif = ADifMinusConcave (ap[Dec (idx)], ap[idx], aminus[minus.Dec (iminus)]);
          }
        }
      }
      return CurveIntersection (curv, v);
    }

    private int PlusStartNorm (DPoly plus, double[] ap, double[] aplus)
    {
      int iplus = -1;
      if (ALow (ap[Dec (0)], ap[0]))
        for (int ip = 0; ip < plus.p.Length; ip++)
        {
          ADif dif = ADifPlusConvex (ap[Dec (0)], ap[0], aplus[ip]);
          if (dif != ADif.Out)
            if (iplus == -1)
              iplus = ip;
            else if (ALow (aplus[ip], aplus[iplus]))
              iplus = ip;
        }
      else
        for (int ip = 0; ip < plus.p.Length; ip++)
        {
          ADif dif = ADifPlusConcave (ap[Dec (0)], ap[0], aplus[plus.Dec (ip)]);
          if (dif != ADif.Out)
            if (iplus == -1)
              iplus = ip;
            else if (ALow (aplus[iplus], aplus[ip]))
              iplus = ip;
        }
      if (iplus == -1)
      {
        double vmax = double.NegativeInfinity;
        for (int ip = 0; ip < plus.p.Length; ip++)
        {
          double val = Math.Cos (ap[0]) * plus.p[ip].x + Math.Sin (ap[0]) * plus.p[ip].y;
          if (val > vmax)
          {
            vmax = val;
            iplus = ip;
          }
        }
      }
      return iplus;
    }

    private int MinusStartNorm (DPoly minus, double[] ap, double[] aminus)
    {
      int iminus = -1;
      if (ALow (ap[Dec (0)], ap[0]))
        for (int ip = 0; ip < minus.p.Length; ip++)
        {
          ADif dif = ADifMinusConvex (ap[Dec (0)], ap[0], aminus[ip]);
          if (dif != ADif.Out)
            if (iminus == -1)
              iminus = ip;
            else if (ALow (aminus[iminus], aminus[ip]))
              iminus = ip;
        }
      else
        for (int ip = 0; ip < minus.p.Length; ip++)
        {
          ADif dif = ADifMinusConcave (ap[Dec (0)], ap[0], aminus[minus.Dec (ip)]);
          if (dif != ADif.Out)
            if (iminus == -1)
              iminus = ip;
            else if (ALow (aminus[ip], aminus[iminus]))
              iminus = ip;
        }
      if (iminus == -1)
      {
        double vmax = double.PositiveInfinity;
        for (int ip = 0; ip < minus.p.Length; ip++)
        {
          double val = Math.Cos (ap[0]) * minus.p[ip].x + Math.Sin (ap[0]) * minus.p[ip].y;
          if (val < vmax)
          {
            vmax = val;
            iminus = ip;
          }
        }
      }
      return iminus;
    }

    private DPoly CurveIntersection (ArrayList curv, ArrayList v)
    {
      if (curv.Count == 0)
        return new DPoly ((Point[])v.ToArray (typeof (Point)));
      ((ArrayList)curv[0]).InsertRange (0, v);
      int[] start = new int[curv.Count];
      int[] stop = new int[curv.Count];
      Point[] inter = new Point[curv.Count];
      for (int idx = 0; idx < curv.Count; idx++)
      {
        int idx1 = (idx + 1) % curv.Count;
        ArrayList curv0 = (ArrayList)curv[idx];
        ArrayList curv1 = (ArrayList)curv[idx1];
        int istart = 0, istop = 0;
        bool xint = false;
        Point x = Point.Empty;
        for (istart = 0; !xint && istart < curv1.Count - 1; istart++)
          for (istop = curv0.Count - 1; !xint && istop > 0 &&
              !(curv0 == curv1 && istop - 1 == istart + 1); istop--)
            xint = Intersect ((Point)curv0[istop], (Point)curv0[istop - 1],
                (Point)curv1[istart], (Point)curv1[istart + 1], out x);
        if (!xint)
          return new DPoly ();
        start[idx1] = istart;
        stop[idx] = istop;
        inter[idx] = x;
      }
      v = new ArrayList ();
      for (int idx = 0; idx < curv.Count; idx++)
      {
        v.AddRange (((ArrayList)curv[idx]).GetRange (
            start[idx], stop[idx] - start[idx] + 1));
        v.Add (inter[idx]);
      }
      return new DPoly ((Point[])v.ToArray (typeof (Point)));
    }

    public bool Contains (Point x)
    {
      if (empty)
        return false;
      int xint = 0;
      for (int i = 0; i < p.Length; i++)
      {
        int i1 = Inc (i);
        if (DEqual (p[i].y, p[i1].y))
        {
          int i0 = Dec (i), i2 = Inc (i1);
          if (Math.Sign (p[i0].y - p[i].y) != Math.Sign (p[i2].y - p[i1].y))
            xint++;
        }
        else
        {
          double t = (x.y - p[i].y) / (p[i1].y - p[i].y);
          double v = p[i1].x * t + p[i].x * (1 - t) - x.x;
          if (v > 0 && t >= 0 && t <= 1)
          {
            if (t < prec)
            {
              int i0 = Dec (i);
              if (Math.Sign (p[i1].y - p[i].y) != Math.Sign (p[i0].y - p[i].y))
                xint++;
            }
            else if (t <= 1 - prec)
              xint++;
          }
        }
      }
      return xint % 2 == 1;
    }

    public Point NearestPoint (Point x)
    {
      Point near = Point.Empty;
      double dist = double.PositiveInfinity;
      for (int i = 0; i < p.Length; i++)
      {
        Point a = p[i], b = p[Inc (i)];
        double r = (b.x - a.x) * (b.x - a.x) + (b.y - a.y) * (b.y - a.y);
        double t = ((x.x - a.x) * (b.x - a.x) + (x.y - a.y) * (b.y - a.y)) / r;
        double v = ((x.x - a.x) * (b.y - a.y) - (x.y - a.y) * (b.x - a.x)) / r;
        double d = Math.Abs (v) * Math.Sqrt (r);
        if (t >= 0 && t <= 1 && d < dist)
        {
          dist = d;
          near = b * t + a * (1 - t);
        }
      }
      for (int i = 0; i < p.Length; i++)
      {
        Point v = p[i] - x;
        double d = Math.Sqrt (v.x * v.x + v.y * v.y);
        if (d < dist)
        {
          dist = d;
          near = p[i];
        }
      }
      return near;
    }

    protected int Inc (int i)
    {
      return i < p.Length - 1 ? i + 1 : 0;
    }

    protected int Dec (int i)
    {
      return i > 0 ? i - 1 : p.Length - 1;
    }

    private bool DEqual (double a, double b)
    {
      return Math.Abs (a - b) < prec;
    }

    protected double Angle (int i)
    {
      Point a = p[i], b = p[Inc (i)];
      return Math.Atan2 (a.x - b.x, b.y - a.y);
    }

    protected bool AngleEqual (double a, double b)
    {
      if (Math.Abs (a - b) < prec)
        return true;
      //if (a == -b && Math.Abs (a - Math.PI) < prec)
      if (Math.Abs (a + b) < prec && Math.Abs (Math.Abs(a) - Math.PI) < prec)
        return true;
      return false;
    }

    protected ADif ADifPlusConvex (double a, double b, double x)
    {
      if (AngleEqual (x, a))
        return ADif.EqA;
      if (AngleEqual (x, b))
        return ADif.EqB;
      return ADifNorm (a, b, x);
    }

    protected ADif ADifPlusConcave (double a, double b, double x)
    {
      if (AngleEqual (x, a))
        return ADif.EqA;
      if (AngleEqual (x, b))
        return ADif.EqB;
      a = a > 0 ? a - Math.PI : a + Math.PI;
      b = b > 0 ? b - Math.PI : b + Math.PI;
      x = x > 0 ? x - Math.PI : x + Math.PI;
      return ADifNorm (b, a, x);
    }

    protected ADif ADifMinusConvex (double a, double b, double x)
    {
      x = x > 0 ? x - Math.PI : x + Math.PI;
      if (AngleEqual (x, a))
        return ADif.EqA;
      if (AngleEqual (x, b))
        return ADif.EqB;
      return ADifNorm (a, b, x);
    }

    protected ADif ADifMinusConcave (double a, double b, double x)
    {
      a = a > 0 ? a - Math.PI : a + Math.PI;
      b = b > 0 ? b - Math.PI : b + Math.PI;
      if (AngleEqual (x, a))
        return ADif.EqA;
      if (AngleEqual (x, b))
        return ADif.EqB;
      return ADifNorm (b, a, x);
    }

    private ADif ADifNorm (double a, double b, double x)
    {
      if (a < b)
        return (a < x && x < b) ? ADif.In : ADif.Out;
      else
        return (a < x || x < b) ? ADif.In : ADif.Out;
    }

    private bool ALow (double a, double b)
    {
      return ((b > a && b - a <= Math.PI) || (b <= 0 && a >= 0 && a - b >= Math.PI));
    }

    private bool Intersect (Point a, Point b, Point c, Point d, out Point x)
    {
      x = Point.Empty;
      double det = (b.x - a.x) * (c.y - d.y) - (c.x - d.x) * (b.y - a.y);
      double alpha = (c.x - a.x) * (c.y - d.y) - (c.x - d.x) * (c.y - a.y);
      double beta = (b.x - a.x) * (c.y - a.y) - (c.x - a.x) * (b.y - a.y);
      if (det < 0)
      {
        det = -det;
        alpha = -alpha;
        beta = -beta;
      }
      if (alpha >= 0 && alpha <= det && beta >= 0 && beta <= det)
      {
        x = (b - a) * (alpha / det) + a;
        return true;
      }
      return false;
    }
  }
}
