using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra.GiftWrapping;

public class GiftWrapping {

  public static AffineBasis BuildInitialPlane(IEnumerable<Point> S) {
    Point origin = S.Min(p => p) ?? throw new InvalidOperationException("The swarm must has at least one point!");
    var   tempV  = new LinkedList<Vector>();
    var   Basis  = new AffineBasis(origin);

    int dim = Basis.Dim;

    for (int i = 1; i < dim; i++) {
      var e = new double[dim];
      e[i] = 1;
      tempV.AddLast(new Vector(e));
    }

    var L = new HashSet<Point>()
      {
        origin
      };

    while (tempV.Any()) {
      var minDot = double.MaxValue;
      var r      = new Vector(dim);

      foreach (Point s in S) { //todo Может стоит удалять просмотренные точки из роя? 
        if (L.Contains(s)) {
          continue;
        }

        Vector v = Vector.OrthonormalizeAgainstBasis(s - origin, Basis.Basis);

        if (!v.IsZero) {
          double dot = v * tempV.First();

          if (dot < minDot) {
            minDot = dot;
            r      = v;
          }
        }

        L.Add(s);
      }

      // if (L.Count == S.Count()) { //todo ????
      //   throw new ArgumentException($"All points of the Swarm lies in dimension less tan d = {dim}."); //todo ????
      // } //todo ????

      tempV.RemoveFirst();
      Basis.AddVectorToBasis(r); //todo тут опять ортонормализуем, боремся как-то?
    }


    return Basis;
  }

}
