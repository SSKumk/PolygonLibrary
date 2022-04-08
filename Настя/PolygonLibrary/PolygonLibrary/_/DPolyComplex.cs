using System;
using System.Collections.Generic;

namespace Robust
{
  public class DPolyComplex
  {
    protected readonly DPoly[] poly;
    protected readonly bool empty;
    private const double prec = Constants.FloatPrec;

    public DPolyComplex ()
    {
      poly = null;
      empty = true;
    }

    public DPolyComplex (DPoly dpoly)
    {
      poly = new DPoly[] { dpoly };
      empty = false;
    }

    public DPolyComplex (DPoly[] dpoly)
    {
      poly = dpoly;
      empty = false;
    }

    public DPolyComplex (Point[] points)
    {
      poly = new DPoly[] { new DPoly (points) };
      empty = false;
    }

    public DPolyComplex (Point[][] points)
    {
      poly = new DPoly[points.Length];
      for (int i = 0; i < poly.Length; i++)
        poly[i] = new DPoly (points[i]);
      empty = false;
    }

    public bool isEmpty
    {
      get
      {
        return empty;
      }
    }

    public int nPoly
    {
      get
      {
        return !empty ? poly.Length : 0;
      }
    }

    public DPoly this[int i]
    {
      get
      {
        return !empty ? poly[i] : new DPoly ();
      }
    }

    public DPolyComplex Plus (DPoly plus, bool isPrimary)
    {
      if (empty || plus.isEmpty)
        return this;
      DPoly[] p = new DPoly[poly.Length];
      for (int i = 0; i < p.Length; i++)
        p[i] = poly[i].Plus (plus, isPrimary);
      return new DPolyComplex (p);
    }

    public DPolyComplex Minus (DPoly minus, bool isPrimary)
    {
      if (empty || minus.isEmpty)
        return this;
      DPoly[] p = new DPoly[poly.Length];
      for (int i = 0; i < p.Length; i++)
        p[i] = poly[i].Minus (minus, isPrimary);
      return new DPolyComplex (p);
    }

    public DPolyComplex PlusMinus (DPoly plus, DPoly minus)
    {
      if (empty)
        return this;
      if (plus.isEmpty)
        return Minus (minus, true);
      if (minus.isEmpty)
        return Plus (plus, true);
      DPoly[] p = new DPoly[poly.Length];
      for (int i = 0; i < p.Length; i++)
        p[i] = poly[i].Plus (plus, true).Minus (minus, false);
      return Normalize(p);
    }

    public DPolyComplex Unite (DPolyComplex unite)
    {
      if (empty)
        return unite;
      if (unite.empty)
        return this;
      List<DPoly> p = new List<DPoly> (poly);
      p.AddRange (unite.poly);
      return new DPolyComplex (UniteCrossIntersections (p).ToArray ());
    }

    public DPolyComplex Normalize (DPoly[] poly)
    {
      List<DPoly> poly0 = new List<DPoly> ();
      for (int ip = 0; ip < poly.Length; ip++)
        if (!poly[ip].isEmpty)
          poly0.Add (poly[ip]);
      if (poly0.Count == 0)
        return new DPolyComplex ();
      List<DPoly> poly1 = new List<DPoly> ();
      foreach (DPoly p in poly0)
        poly1.AddRange (DropSelfIntersections (p));
      if (poly1.Count == 0)
        return new DPolyComplex ();
      List<DPoly> poly2 = UniteCrossIntersections (poly1);
      return new DPolyComplex (poly2.ToArray ());
    }

    private List<DPoly> DropSelfIntersections (DPoly poly)
    {
      List<DPoly> curv = new List<DPoly> ();
      List<Point> v = new List<Point> ();
      Queue<int> newv = new Queue<int> ();
      Queue<int> newv0 = new Queue<int> ();
      Queue<Point> newx = new Queue<Point> ();
      bool[] checkv = new bool[poly.nPoints];
      int idxv = 0, idxv0 = 0;
      bool complete = false;
      while (!complete)
      {
        int xint = 0, xv = 0, xv1 = 0;
        Point xpoint = Point.Empty, x;
        int idxv1 = Inc (poly, idxv);
        checkv[idxv] = true;
        for (int iv = 0; iv < poly.nPoints; iv++)
        {
          int iv1 = Inc (poly, iv);
          if (idxv != iv && idxv1 != iv && idxv != iv1)
            if (Intersect (poly[idxv], poly[idxv1], poly[iv], poly[iv1],
                out x))
            {
              xint++;
              xv = iv;
              xv1 = iv1;
              xpoint = x;
            }
        }
        if (xint > 1)
          throw new ErrorListException (Constants.DPolyComplex);
        if (xint == 0)
        {
          v.Add (poly[idxv]);
          idxv = Inc (poly, idxv);
        }
        else
        {
          v.Add (poly[idxv]);
          v.Add (xpoint);
          newv.Enqueue (idxv1);
          newv0.Enqueue (xv);
          newx.Enqueue (xpoint);
          idxv = xv1;
        }
        if (idxv == idxv0)
        {
          curv.Add (new DPoly (v.ToArray ()));
          v.Clear ();
          complete = true;
          if (newv.Count > 0)
          {
            do
            {
              idxv = newv.Dequeue ();
              idxv0 = newv0.Dequeue ();
              x = newx.Dequeue ();
            } while (newv.Count > 0 && checkv[idxv0]);
            if (!checkv[idxv0])
            {
              v.Add (poly[idxv0]);
              v.Add (x);
              checkv[idxv0] = true;
              complete = false;
            }
          }
        }
      }
      List<DPoly> right = new List<DPoly> ();
      for (int i = 0; i < curv.Count; i++)
      {
        double area = 0;
        for (int j = 0; j < curv[i].nPoints; j++)
        {
          int j1 = j < curv[i].nPoints - 1 ? j + 1 : 0;
          area += (curv[i][j].y + curv[i][j1].y) *
              (curv[i][j1].x - curv[i][j].x) / 2;
        }
        if (area <= 0)
          right.Add (curv[i]);
      }
      return right;
    }

    private List<DPoly> UniteCrossIntersections (List<DPoly> poly)
    {
      if (poly.Count == 1)
        return poly;
      List<DPoly> p = new List<DPoly> (poly);
      bool xint;
      do
      {
        int ip0 = 0, ip1 = 0;
        xint = false;
        for (ip0 = 0; ip0 < p.Count && !xint; ip0++)
          for (ip1 = ip0 + 1; ip1 < p.Count && !xint; ip1++)
            xint = Intersect (p[ip0], p[ip1]);
        if (xint)
        {
          DPoly p0 = p[ip0 - 1], p1 = p[ip1 - 1];
          p.Remove (p0);
          p.Remove (p1);
          p.Add (UnitePoly (p0, p1));
        }
      } while (xint);
      return p;
    }

    private DPoly UnitePoly (DPoly poly0, DPoly poly1)
    {
      int iv0 = 0;
      while (poly1.Contains (poly0[iv0]))
        iv0++;
      int ip = 0, iv = iv0;
      DPoly[] p = new DPoly[] { poly0, poly1 };
      List<Point> v = new List<Point> ();
      do
      {
        int xint = 0, jp = 1 - ip, xv = 0;
        Point xpoint = Point.Empty, x;
        for (int jv = 0; jv < p[jp].nPoints; jv++)
          if (Intersect (p[ip], iv, p[jp], jv, out x))
            if (DGeo.ALow (DGeo.Angle (p[jp], jv), DGeo.Angle (p[ip], iv)))
            {
              xint++;
              xv = jv;
              xpoint = x;
            }
        if (xint == 0)
        {
          v.Add (p[ip][iv]);
          iv = Inc (p[ip], iv);
        }
        else
        {
          v.Add (p[ip][iv]);
          v.Add (xpoint);
          ip = jp;
          iv = Inc (p[jp], xv);
        }
      } while (ip != 0 || iv != iv0);
      return new DPoly (v.ToArray ());
    }

    private int Inc (DPoly p, int i)
    {
      return i < p.nPoints - 1 ? i + 1 : 0;
    }

    private bool Intersect (DPoly poly0, DPoly poly1)
    {
      bool xint = false, same = poly0 == poly1;
      Point x;
      for (int i0 = 0; i0 < poly0.nPoints && !xint; i0++)
        for (int i1 = 0; i1 < poly1.nPoints && !xint; i1++)
        {
          int j0 = Inc (poly0, i0), j1 = Inc (poly1, i1);
          if (!same || (i0 != i1 && i0 != j1 && i1 != j0))
            xint = Intersect (poly0[i0], poly0[j0], poly1[i1], poly1[j1],
                out x);
        }
      return xint;
    }

    private bool Intersect (DPoly poly0, int idx0, DPoly poly1, int idx1, out Point x)
    {
      int v0 = Inc (poly0, idx0), v1 = Inc (poly1, idx1);
      return Intersect (poly0[idx0], poly0[v0], poly1[idx1], poly1[v1], out x);
    }

    private bool Intersect (Point a, Point b, Point c, Point d, out Point x)
    {
      x = Point.Empty;
      double det = (b.x - a.x) * (c.y - d.y) - (c.x - d.x) * (b.y - a.y);
      if (Math.Abs (det) < prec)
        return false;
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
