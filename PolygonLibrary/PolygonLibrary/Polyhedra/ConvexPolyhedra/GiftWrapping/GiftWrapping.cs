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

    int dim = Basis.VecDim;

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

        if (v.IsZero) {
          Viewed.Add(s);
        } else {
          double dot = v * tempV.First();

          if (dot < minDot) {
            minDot = dot;
            r      = v;
            sMin   = s;
          }
        }
      }

      if (sMin is null) {
        return Basis;
      }

      Viewed.Add(sMin);
      tempV.RemoveFirst();
      Basis.AddVectorToBasis(r!, false);
    }
    
    return Basis;
  }

  //todo
  /*
   * Взять
   * Набор граней и информация о соседстве. Какие грани сходятся в каждой 0-мерной вершине + массив граней = Polyhedron
   *
   * Point, Point2D -- не трогаем
   * 
   * ) ISubspacePoint -- сюда добавляем всё что связывает точку 
   * ) SubspacePoint : ISubspacePoint, Point умеет хранить о предыдущем предке и о самом первом
   * ) SubspacePoint2D : ISubspacePoint, Point2D умеет хранить в 2D о предыдущем предке и о самом первом
   *    - возможно сюда проектирование нужно принести
   * 
   * ) Polyhedron
   *    Face : Polyhedron, IFace
   *    Face2D : Polyhedron,IFace
   *   - GetPoints в своём пространстве 
   *   - GetPoints в заданном пространстве большей размерности 
   */
}
