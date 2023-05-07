using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Polygons.ConvexPolygons.ConvexificationByGiftWrapping;

public class GiftWrapping {

  public static (Point, List<Vector>) BuildInitialPlane(IEnumerable<Point> S) {
    Point origin = S.Min(p => p) ?? throw new InvalidOperationException("The swarm must has at least one point!");
    LinkedList<Vector> tempV = null!;
    List<Vector> finalV = null!;

    Debug.Assert(origin is not null, nameof(origin) + " != null");

    int dim = origin.Dim;

    for (int i = 1; i < dim; i++) {
      var e = new double[dim];
      e[i] = 1;
      tempV.AddLast(new Vector(e));
    }

    var L = new HashSet<Point>()
      {
        origin
      };

    while (!tempV.Any()) {
      var minDot = double.MaxValue;
      var r    = new Vector(dim);

      foreach (Point s in S) { //todo Может стоит удалять просмотренные точки из роя? 
        if (L.Contains(s)) {
          continue;
        }

        Vector v = Vector.OrthonormalizeAgainstBasis(s - origin, finalV);

        if (v.IsZero) {
          L.Add(v.ToPoint());
        } else {
          double dot = v * tempV.First();

          if (dot < minDot) {
            minDot = dot;
            r    = v;
            L.Add(s);
          }
        }
      }

      if (!L.Any()) { //todo ????
        throw new ArgumentException($"All points of the Swarm lies in dimension less tan d = {dim}.");
      }

      tempV.RemoveFirst();

      if (!r.IsZero) {
        finalV.Add(r);
      }
    }


    return (origin, finalV);
  }

}
