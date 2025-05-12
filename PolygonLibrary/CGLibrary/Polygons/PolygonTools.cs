namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public sealed class PolygonTools {

    /// <summary>
    /// Method for generating a convex polygon representing a rectangle with sides parallel
    /// to the coordinate axes. The rectangle is defined by points of two opposite vertices.
    /// If the points are located on a vertical or horizontal line, a segment or a point is generated
    /// (a polygon with two or one vertices)
    /// </summary>
    /// <param name="v1">One vertex from a pair of opposite vertices.</param>
    /// <param name="v2">Another vertex from a pair of opposite vertices.</param>
    /// <returns>The appropriate polygon.</returns>
    public static ConvexPolygon RectangleParallel(Vector2D v1, Vector2D v2) => RectangleParallel(v1.x, v1.y, v2.x, v2.y);

    /// <summary>
    /// Method for generating a convex polygon representing a rectangle with sides parallel
    /// to the coordinate axes. The rectangle is defined by coordinates of two opposite vertices.
    /// If the points are located on a vertical or horizontal line, a segment or a point is generated
    /// (a polygon with two or one vertices)
    /// </summary>
    /// <param name="x1o">The abscissa of the first vertex.</param>
    /// <param name="y1o">The ordinate of the first vertex.</param>
    /// <param name="x2o">The abscissa of the second vertex.</param>
    /// <param name="y2o">The ordinate of the second vertex.</param>
    /// <returns>The appropriate polygon.</returns>
    public static ConvexPolygon RectangleParallel(TNum x1o, TNum y1o, TNum x2o, TNum y2o) {
      TNum           x1 = TNum.Min(x1o, x2o), x2 = TNum.Max(x1o, x2o), y1 = TNum.Min(y1o, y2o), y2 = TNum.Max(y1o, y2o);
      List<Vector2D> vs = new List<Vector2D>();

      if (Tools.EQ(x1, x2) || Tools.EQ(y1, y2)) {
        // The case of a segment or a point
        vs.Add(new Vector2D(x1o, y1o));
        if (Tools.NE(x1, x2) || Tools.NE(y1, y2)) {
          vs.Add(new Vector2D(x2o, y2o));
        }
      }
      else {
        // The case of a rectangle
        vs.Add(new Vector2D(x1, y1));
        vs.Add(new Vector2D(x2, y1));
        vs.Add(new Vector2D(x2, y2));
        vs.Add(new Vector2D(x1, y2));
      }

      return new ConvexPolygon(vs, false);
    }

    /// <summary>
    /// Method for generating a convex polygon representing a rectangle with sides parallel to the
    /// coordinate axes. The rectangle is defined by points of two opposite vertices and the turn angle.
    /// If the points are located on a vertical or horizontal line, a segment or a point is generated
    /// (a polygon with two or one vertices)
    /// </summary>
    /// <param name="p1">One vertex from a pair of opposite vertices.</param>
    /// <param name="p2">Another vertex from a pair of opposite vertices.</param>
    /// <param name="alpha">Turn angle (in radians): angle between the Ox or Oy axis and
    /// the rectangle sides.</param>
    /// <returns>The appropriate polygon.</returns>
    public static ConvexPolygon RectangleTurned(Vector2D p1, Vector2D p2, TNum alpha)
      => RectangleTurned(p1.x, p1.y, p2.x, p2.y, alpha);

    /// <summary>
    /// Method for generating a convex polygon representing a rectangle with sides having a given angle with the
    /// coordinate axes. The rectangle is defined by coordinates of two opposite vertices and the turn angle.
    /// If the points are located on a vertical or horizontal line, a segment or a point is generated
    /// (a polygon with two or one vertices)
    /// </summary>
    /// <param name="x1o">The abscissa of the first vertex.</param>
    /// <param name="y1o">The ordinate of the first vertex.</param>
    /// <param name="x2o">The abscissa of the second vertex.</param>
    /// <param name="y2o">The ordinate of the second vertex.</param>
    /// <param name="alpha">Turn angle (in radians): angle between the Ox or Oy axis and
    /// the rectangle sides.</param>
    /// <returns>The appropriate polygon.</returns>
    public static ConvexPolygon RectangleTurned(TNum x1o, TNum y1o, TNum x2o, TNum y2o, TNum alpha) {
      // If the points coincide, return a single-pointed polygon
      if (Tools.EQ(x1o, x2o) && Tools.EQ(y1o, y2o)) {
        return new ConvexPolygon(new Vector2D[] { new Vector2D(x1o, y1o) }, false);
      }

      (TNum sn, TNum cs) = TNum.SinCos(alpha);

      TNum dvx = x2o - x1o, dvy = y2o - y1o, a = dvx * cs + dvy * sn, b = cs * dvy - sn * dvx;

      TNum[,] ar = new TNum[3, 3] { { cs, -sn, x1o }, { sn, cs, y1o }, { Tools.Zero, Tools.Zero, Tools.One } };

      Matrix mapMatrix = new Matrix(ar);

      TNum[] ar_v0 = new TNum[] { Tools.Zero, Tools.Zero, Tools.One }
           , ar_v1 = new TNum[] { a, Tools.Zero, Tools.One }
           , ar_v2 = new TNum[] { a, b, Tools.One }
           , ar_v3 = new TNum[] { Tools.Zero, b, Tools.One };

      Vector v0o = new Vector(ar_v0, false)
           , v0  = Matrix.MultMatrixByColumnVector(mapMatrix, v0o)
           , v1o = new Vector(ar_v1, false)
           , v1  = Matrix.MultMatrixByColumnVector(mapMatrix, v1o)
           , v2o = new Vector(ar_v2, false)
           , v2  = Matrix.MultMatrixByColumnVector(mapMatrix, v2o)
           , v3o = new Vector(ar_v3, false)
           , v3  = Matrix.MultMatrixByColumnVector(mapMatrix, v3o);

      List<Vector2D> res = new List<Vector2D>();
      res.Add(new Vector2D(v0[0], v0[1]));
      res.Add(new Vector2D(v1[0], v1[1]));
      res.Add(new Vector2D(v2[0], v2[1]));
      res.Add(new Vector2D(v3[0], v3[1]));

      return new ConvexPolygon(res, true);
    }

    /// <summary>
    /// Method for generating an approximation for the circle as a right n-polygon.
    /// If the radius equals zero, a one-pointed polygon is generated.
    /// </summary>
    /// <param name="center">Center point of the circle</param>
    /// <param name="R">The radius.</param>
    /// <param name="n">Number of vertices in the resultant polygon.</param>
    /// <returns>The appropriate polygon.</returns>
    public static ConvexPolygon Circle(Vector2D center, TNum R, int n) => Circle(center.x, center.y, R, n, Tools.Zero);

    /// <summary>
    /// Method for generating an approximation for the circle as a right n-polygon.
    /// If the radius equals zero, a one-pointed polygon is generated.
    /// </summary>
    /// <param name="x">The abscissa of the center.</param>
    /// <param name="y">The ordinate of the center.</param>
    /// <param name="R">The radius.</param>
    /// <param name="n">Number of vertices in the resultant polygon.</param>
    /// <returns>The appropriate polygon.</returns>
    public static ConvexPolygon Circle(TNum x, TNum y, TNum R, int n) => Circle(x, y, R, n, Tools.Zero);

    /// <summary>
    /// Method for generating an approximation for a ellipse as a n-polygon with vertices
    /// uniformly distributed in angle. If one semiaxis equals zero, a segment is generated.
    /// If the radius equals zero, a one-pointed polygon is generated.
    /// The polygon can be turned by some angle around the center.
    /// </summary>
    /// <param name="x">The abscissa of the center.</param>
    /// <param name="y">The ordinate of the center.</param>
    /// <param name="a">One semiaxis.</param>
    /// <param name="b">Other semiaxis.</param>
    /// <param name="n">Number of vertices in the resultant polygon.</param>
    /// <param name="phi">The angle of turn of the entire ellipse</param>
    /// <returns>The appropriate polygon.</returns>
    public static ConvexPolygon Ellipse(
        TNum x
      , TNum y
      , TNum a
      , TNum b
      , int  n
      , TNum phi
      )
      => Ellipse
        (
         x
       , y
       , a
       , b
       , n
       , phi
       , Tools.Zero
        );

    /// <summary>
    /// Method for generating an approximation for a ellipse as a n-polygon with vertices
    /// uniformly distributed in angle. If one semiaxis equals zero, a segment is generated.
    /// If the radius equals zero, a one-pointed polygon is generated.
    /// </summary>
    /// <param name="x">The abscissa of the center.</param>
    /// <param name="y">The ordinate of the center.</param>
    /// <param name="a">One semiaxis.</param>
    /// <param name="b">Other semiaxis.</param>
    /// <param name="n">Number of vertices in the resultant polygon.</param>
    /// <returns>The appropriate polygon.</returns>
    public static ConvexPolygon Ellipse(TNum x, TNum y, TNum a, TNum b, int n)
      => Ellipse
        (
         x
       , y
       , a
       , b
       , n
       , Tools.Zero
       , Tools.Zero
        );

#region Fuctions without default arguments
    /// <summary>
    /// Method for generating an approximation for the circle as a right n-polygon.
    /// If the radius equals zero, a one-pointed polygon is generated.
    /// The polygon can be turned by some angle around the center.
    /// </summary>
    /// <param name="x">The abscissa of the center.</param>
    /// <param name="y">The ordinate of the center.</param>
    /// <param name="R">The radius.</param>
    /// <param name="n">Number of vertices in the resultant polygon.</param>
    /// <param name="a0">The additional turn angle</param>
    /// <returns>The appropriate polygon.</returns>
    public static ConvexPolygon Circle(TNum x, TNum y, TNum R, int n, TNum a0) {
      if (Tools.EQ(R)) {
        return new ConvexPolygon(new Vector2D[] { new Vector2D(x, y) }, false);
      }

      List<Vector2D> res = new List<Vector2D>();
      TNum           da  = Tools.Two * Tools.PI / TConv.FromInt(n);
      for (int i = 0; i < n; i++) {
        res.Add(new Vector2D(x + R * TNum.Cos(TConv.FromInt(i) * da + a0), y + R * TNum.Sin(TConv.FromInt(i) * da + a0)));
      }

      return new ConvexPolygon(res, false);
    }

    /// <summary>
    /// Method for generating an approximation for a ellipse as a n-polygon with vertices
    /// uniformly distributed in angle. If one semiaxis equals zero, a segment is generated.
    /// If the radius equals zero, a one-pointed polygon is generated.
    /// The polygon can be turned by some angle around the center and for some angle around zeroth vertex.
    /// </summary>
    /// <param name="x">The abscissa of the center.</param>
    /// <param name="y">The ordinate of the center.</param>
    /// <param name="a">One semiaxis.</param>
    /// <param name="b">Other semiaxis.</param>
    /// <param name="n">Number of vertices in the resultant polygon.</param>
    /// <param name="phi">The angle of turn of the entire ellipse</param>
    /// <param name="a0">THe additional angle for turn of the zeroth vertex.</param>
    /// <returns>The appropriate polygon.</returns>
    public static ConvexPolygon Ellipse(
        TNum x
      , TNum y
      , TNum a
      , TNum b
      , int  n
      , TNum phi
      , TNum a0
      ) {
      if (Tools.EQ(a)) {
        if (Tools.EQ(b)) {
          return new ConvexPolygon(new Vector2D[] { new Vector2D(x, y) }, false);
        }
        else {
          (TNum sn, TNum cs) = TNum.SinCos(phi);

          return new ConvexPolygon
            (new Vector2D[] { new Vector2D(x + a * cs, y + a * sn), new Vector2D(x - a * cs, y - a * sn) }, false);
        }
      }
      if (Tools.EQ(b)) {
        (TNum sn, TNum cs) = TNum.SinCos(phi);

        return new ConvexPolygon
          (new Vector2D[] { new Vector2D(x + b * sn, y + b * cs), new Vector2D(x - b * sn, y - b * cs) }, false);
      }

      List<Vector2D> res    = new List<Vector2D>();
      TNum           da     = Tools.Two * Tools.PI / TConv.FromInt(n);
      Vector2D       center = new Vector2D(x, y);
      for (int i = 0; i < n; i++) {
        Vector2D v  = new Vector2D(a * TNum.Cos(TConv.FromInt(i) * da + a0), b * TNum.Sin(TConv.FromInt(i) * da + a0))
               , v1 = v.Turn(phi);
        res.Add(new Vector2D(center + v1));
      }

      return new ConvexPolygon(res, false);
    }
#endregion

  }

}
