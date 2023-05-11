using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AVLUtils;
using PolygonLibrary.Basics;
using PolygonLibrary.Toolkit;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra.GiftWrapping;

public class GiftWrapping {

  public static AffineBasis BuildInitialPlane(IEnumerable<Point> S) {
    Debug.Assert(S.Any(), "The swarm must has at least one point!");

    Point              origin = S.Min(p => p)!;
    LinkedList<Vector> tempV  = new LinkedList<Vector>();
    AffineBasis        Basis  = new AffineBasis(origin);

    int dim = Basis.Dim;

    for (int i = 1; i < dim; i++) {
      tempV.AddLast(Vector.CreateOrth(dim, i + 1));
    }

    HashSet<Point> Viewed = new HashSet<Point>()
      {
        origin
      };

    double  minDot;
    Vector? r = null;
    Point?  sMin;

    while (tempV.Any()) {
      minDot = double.MaxValue;
      sMin   = null;

      foreach (Point s in S) {
        if (Viewed.Contains(s)) {
          continue;
        }

        Vector v = Vector.OrthonormalizeAgainstBasis(s - origin, Basis.Basis);

        if (!v.IsZero) {
          double dot = v * tempV.First();

          if (dot < minDot) {
            minDot = dot;
            r      = v;
            sMin   = s;
          }
        } else {
          Viewed.Add(s);
        }
      }

      if (sMin is null) {
        return Basis;
      }

      Viewed.Add(sMin);
      tempV.RemoveFirst();
      Basis.AddVectorToBasis(r!); //todo добавить флаг orthonormalize = true
    }


    return Basis;
  }

}
