using System;
using System.Collections.Generic;
using PolygonLibrary.Basics;
using PolygonLibrary.Toolkit;
using PolygonLibrary.Polygons.ConvexPolygons;

namespace PolygonLibrary.Polygons; 

public sealed class PolygonTools
{
  /// <summary>
  /// Method for generating a convex polygon representing a rectangle with sides parallel 
  /// to the coordinate axes. The rectangle is defined by points of two opposite vertices.
  /// If the points are located on a vertical or horizontal line, a segment or a point is generated
  /// (a polygon with two or one vertices)
  /// </summary>
  /// <param name="v1">One vertex from a pair of opposite vertices</param>
  /// <param name="v2">Another vertex from a pair of opposite vertices</param>
  /// <returns>The appropriate polygon</returns>
  public static ConvexPolygon RectangleParallel(Point2D v1, Point2D v2) => RectangleParallel(v1.x, v1.y, v2.x, v2.y);

  /// <summary>
  /// Method for generating a convex polygon representing a rectangle with sides parallel 
  /// to the coordinate axes. The rectangle is defined by coordinates of two opposite vertices.
  /// If the points are located on a vertical or horizontal line, a segment or a point is generated
  /// (a polygon with two or one vertices)
  /// </summary>
  /// <param name="x1o">The abscissa of the first vertex</param>
  /// <param name="y1o">The ordinate of the first vertex</param>
  /// <param name="x2o">The abscissa of the second vertex</param>
  /// <param name="y2o">The ordinate of the second vertex</param>
  /// <returns>The appropriate polygon</returns>
  public static ConvexPolygon RectangleParallel(double x1o, double y1o, double x2o, double y2o)
  {
    double
      x1 = Math.Min(x1o, x2o),
      x2 = Math.Max(x1o, x2o),
      y1 = Math.Min(y1o, y2o),
      y2 = Math.Max(y1o, y2o);
    List<Point2D> vs = new List<Point2D>();

    if (Tools.EQ(x1, x2) || Tools.EQ(y1, y2))
    {
      // The case of a segment or a point
      vs.Add(new Point2D(x1o, y1o));
      if (Tools.NE(x1, x2) || Tools.NE(y1, y2)) {
        vs.Add(new Point2D(x2o, y2o));
      }
    }
    else
    {
      // The case of a rectangle
      vs.Add(new Point2D(x1, y1));
      vs.Add(new Point2D(x2, y1));
      vs.Add(new Point2D(x2, y2));
      vs.Add(new Point2D(x1, y2));
    }

    return new ConvexPolygon(vs, true);
  }

  /// <summary>
  /// Method for generating a convex polygon representing a rectangle with sides parallel to the 
  /// coordinate axes. The rectangle is defined by points of two opposite vertices and the turn angle.
  /// If the points are located on a vertical or horizontal line, a segment or a point is generated
  /// (a polygon with two or one vertices)
  /// </summary>
  /// <param name="p1">One vertex from a pair of opposite vertices</param>
  /// <param name="p2">Another vertex from a pair of opposite vertices</param>
  /// <param name="alpha">Turn angle (in radians): angle between the Ox or Oy axis and 
  /// the rectangle sides</param>
  /// <returns>The appropriate polygon</returns>
  public static ConvexPolygon RectangleTurned(Point2D p1, Point2D p2, double alpha) => RectangleTurned(p1.x, p1.y, p2.x, p2.y, alpha);

  /// <summary>
  /// Method for generating a convex polygon representing a rectangle with sides having a given angle with the 
  /// coordinate axes. The rectangle is defined by coordinates of two opposite vertices and the turn angle.
  /// If the points are located on a vertical or horizontal line, a segment or a point is generated
  /// (a polygon with two or one vertices)
  /// </summary>
  /// <param name="x1o">The abscissa of the first vertex</param>
  /// <param name="y1o">The ordinate of the first vertex</param>
  /// <param name="x2o">The abscissa of the second vertex</param>
  /// <param name="y2o">The ordinate of the second vertex</param>
  /// <param name="alpha">Turn angle (in radians): angle between the Ox or Oy axis and 
  /// the rectangle sides</param>
  /// <returns>The appropriate polygon</returns>
  public static ConvexPolygon RectangleTurned(double x1o, double y1o, double x2o, double y2o, double alpha)
  {
    // If the points coincide, return a single-pointed polygon
    if (Tools.EQ(x1o, x2o) && Tools.EQ(y1o, y2o)) {
      return new ConvexPolygon(new Point2D[] { new Point2D(x1o, y1o) }, false);
    }

    double
      dvx = x2o - x1o,
      dvy = y2o - y1o,
      a = dvx * Math.Cos(alpha) + dvy * Math.Sin(alpha),
      b = Math.Cos(alpha) * dvy - Math.Sin(alpha) * dvx;
    double[,] ar = new double[3, 3]
    { 
      { Math.Cos(alpha), -Math.Sin(alpha), x1o },
      { Math.Sin(alpha),  Math.Cos(alpha), y1o },
      { 0,                0,               1   }
    };
    Matrix mapMatrix = new Matrix(ar);
    double[]
      ar_v0 = new double[3] { 0, 0, 1 },
      ar_v1 = new double[3] { a, 0, 1 },
      ar_v2 = new double[3] { a, b, 1 },
      ar_v3 = new double[3] { 0, b, 1 };
    Vector
      v0o = new Vector(ar_v0), v0 = mapMatrix * v0o,
      v1o = new Vector(ar_v1), v1 = mapMatrix * v1o,
      v2o = new Vector(ar_v2), v2 = mapMatrix * v2o,
      v3o = new Vector(ar_v3), v3 = mapMatrix * v3o;

    List<Point2D> res = new List<Point2D>();
    res.Add(new Point2D(v0[0], v0[1]));
    res.Add(new Point2D(v1[0], v1[1]));
    res.Add(new Point2D(v2[0], v2[1]));
    res.Add(new Point2D(v3[0], v3[1]));

    return new ConvexPolygon(res, true);
  }

  /// <summary>
  /// Method for generating an approximation for the circle as a right n-polygon.
  /// If the radius equals zero, a one-pointed polygon is generated.
  /// The polygon can be turned by some angle around the center.
  /// </summary>
  /// <param name="center">Center point of the circle</param>
  /// <param name="R">The radius</param>
  /// <param name="n">Number of vertices in the resultant polygon</param>
  /// <param name="a0">The additional turn angle</param>
  /// <returns>The appropriate polygon</returns>
  public static ConvexPolygon Circle(Point2D center, double R, int n, double a0 = 0) => Circle(center.x, center.y, R, n, a0);

  /// <summary>
  /// Method for generating an approximation for the circle as a right n-polygon.
  /// If the radius equals zero, a one-pointed polygon is generated.
  /// The polygon can be turned by some angle around the center.
  /// </summary>
  /// <param name="x">The abscissa of the center</param>
  /// <param name="y">The ordinate of the center</param>
  /// <param name="R">The radius</param>
  /// <param name="n">Number of vertices in the resultant polygon</param>
  /// <param name="a0">The additional turn angle</param>
  /// <returns>The appropriate polygon</returns>
  public static ConvexPolygon Circle(double x, double y, double R, int n, double a0 = 0)
  {
    if (Tools.EQ(R)) {
      return new ConvexPolygon(new Point2D[] { new Point2D(x, y) }, false);
    }

    List<Point2D> res = new List<Point2D>();
    double da = 2 * Math.PI / n;
    for (int i = 0; i < n; i++) {
      res.Add(new Point2D(x + R * Math.Cos(i * da + a0), y + R * Math.Sin(i * da + a0)));
    }

    return new ConvexPolygon(res, false);
  }

  /// <summary>
  /// Method for generating an approximation for a ellipse as a n-polygon with vertices 
  /// uniformly distributed in angle. If one semiaxis equals zero, a segment is generated.
  /// If the radius equals zero, a one-pointed polygon is generated.
  /// The polygon can be turned by some angle around the center.
  /// </summary>
  /// <param name="x">The abscissa of the center</param>
  /// <param name="y">The ordinate of the center</param>
  /// <param name="a">One semiaxis</param>
  /// <param name="b">Other semiaxis</param>
  /// <param name="n">Number of vertices in the resultant polygon</param>
  /// <param name="phi">The angle of turn of the entire ellipse</param>
  /// <param name="a0">THe additional angle for turn of the zeroth vertex</param>
  /// <returns>The appropriate polygon</returns>
  public static ConvexPolygon Ellipse(double x, double y, double a, double b, int n, double phi = 0, double a0 = 0)
  {
    if (Tools.EQ(a))
    {
      if (Tools.EQ(b)) {
        return new ConvexPolygon(new Point2D[] { new Point2D(x, y) }, false);
      } else {
        return new ConvexPolygon(new Point2D[] 
        { 
          new Point2D(x + a*Math.Cos(phi), y + a*Math.Sin(phi)), 
          new Point2D(x - a*Math.Cos(phi), y - a*Math.Sin(phi)) 
        }, false);
      }
    }
    else if (Tools.EQ(b)) {
      return new ConvexPolygon(new Point2D[] 
      { 
        new Point2D(x + b*Math.Sin(phi), y + b*Math.Cos(phi)), 
        new Point2D(x - b*Math.Sin(phi), y - b*Math.Cos(phi)) 
      }, false);
    }

    List<Point2D> res = new List<Point2D>();
    double da = 2 * Math.PI / n;
    Point2D center = new Point2D(x,y);
    for (int i = 0; i < n; i++)
    {
      Vector2D 
        v = new Vector2D(a * Math.Cos(i * da + a0), b * Math.Sin(i * da + a0)),
        v1 = v.Turn(phi);
      res.Add(new Point2D(center + v1));
    }
    return new ConvexPolygon(res, false);
  }
}