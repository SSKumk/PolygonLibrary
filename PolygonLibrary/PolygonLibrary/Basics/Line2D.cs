using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolygonLibrary.Toolkit;
using PolygonLibrary.Segments;

namespace PolygonLibrary.Basics
{
  /// <summary>
  /// A call of a straight line in the plane
  /// </summary>
  public class Line2D
  {
    #region Data and properties
    /// <summary>
    /// The property of the coefficient A in the general equation of the line
    /// </summary>
    public double A { get; protected set; }

    /// <summary>
    /// The property of the coefficient B in the general equation of the line
    /// </summary>
    public double B { get; protected set; }

    /// <summary>
    /// The property of the coefficient C in the general equation of the line
    /// </summary>
    public double C { get; protected set; }

    /// <summary>
    /// The property of the directional vector of the line
    /// </summary>
    public Vector2D Direct { get; protected set; }

    /// <summary>
    /// The property of the normal vector of the line
    /// </summary>
    public Vector2D Normal { get; protected set; }
    #endregion

    #region Contructors, factories, and helping contruction functions
    /// <summary>
    /// Default constructor that produces the abscissa axis
    /// </summary>
    public Line2D()
    {
      A = 0;
      B = 1;
      C = 0;
      Direct = new Vector2D(1, 0);
      Normal = new Vector2D(0, 1);
    }

    /// <summary>
    /// Constructing a line passing through two points <paramref name="p1"/> and <paramref name="p2"/>.
    /// The positive halfplane is laying to the left from the vector p1->p2.
    /// </summary>
    /// <param name="p1">The first point that lies in the line</param>
    /// <param name="p2">The second point that lies in the line</param>
    public Line2D(Point2D p1, Point2D p2)
    {
      PointAndDirect(p1, p2 - p1);
    }

    /// <summary>
    /// Constructing a line passing through two points <paramref name="p1"/> and <paramref name="p2"/>.
    /// The positive halfplane is laying to the left from the vector p1->p2.
    /// </summary>
    /// <param name="p1">The first point that lies in the line</param>
    /// <param name="p2">The second point that lies in the line</param>
    /// <param name="p3">A point that does not belong to the line and defines the positive halfplane</param>
    public Line2D(Point2D p1, Point2D p2, Point2D p3)
    {
      PointAndDirect(p1, p2 - p1, p3);
    }

    /// <summary>
    /// Factory that produces a line on the basis of a point that belongs to the line
    /// and the direction vector of the line. The positive halfplane is supposed to be
    /// to the left from the vector <paramref name="v"/>.
    /// </summary>
    /// <param name="p">A point that belongs to the line</param>
    /// <param name="v">The direction vector of the line</param>
    /// <returns>The resultant line</returns>
    public static Line2D Line2D_PointAndDirect(Point2D p, Vector2D v)
    {
      Line2D res = new Line2D();
      res.PointAndDirect(p, v);
      return res;
    }

    /// <summary>
    /// Factory that produces a line on the basis of a point that belongs to the line
    /// and the direction vector of the line. The positive halfplane is defined 
    /// by the point <paramref name="p1"/>.
    /// </summary>
    /// <param name="p">A point that belongs to the line</param>
    /// <param name="v">The direction vector of the line</param>
    /// <param name="p1">A point that does not belong to the line and defines the positive halfplane</param>
    /// <returns>The resultant line</returns>
    public static Line2D Line2D_PointAndDirect(Point2D p, Vector2D v, Point2D p1)
    {
      Line2D res = new Line2D();
      res.PointAndDirect(p, v, p1);
      return res;
    }

    /// <summary>
    /// Internal method that fills the fields such that finally we have a line on the basis 
    /// of a point that belongs to the line and the direction vector of the line. 
    /// The positive halfplane is supposed to be to the left from the vector <paramref name="v"/>.
    /// </summary>
    /// <param name="p">A point that belongs to the line</param>
    /// <param name="v">The direction vector of the line</param>
    private void PointAndDirect(Point2D p, Vector2D v)
    {
      Direct = new Vector2D(v).Normalize();

      Normal = new Vector2D(-Direct.Y, Direct.X);

      A = Normal.X;
      B = Normal.Y;
      C = -Normal * (Vector2D)p;
    }

    /// <summary>
    /// Internal method that fills the fields such that finally we have a line on the basis 
    /// of a point that belongs to the line and the direction vector of the line. 
    /// The positive halfplane is defined by the point <paramref name="p1"/>.
    /// </summary>
    /// <param name="p">A point that belongs to the line</param>
    /// <param name="v">The direction vector of the line</param>
    /// <param name="p1">A point that does not belong to the line and defines the positive halfplane</param>
    private void PointAndDirect(Point2D p, Vector2D v, Point2D p1)
    {
      PointAndDirect (p, v);

      double val = this[p1];
      if (Tools.EQ(val))
        throw new ArgumentException("The point that should define the positive halfplane belongs to the line");

      if (Tools.LT(val))
      {
        A = -A;
        B = -B;
        C = -C;
        Normal = -Normal;
        Direct = -Direct;
      }
    }

    /// <summary>
    /// Factory that produces a line on the basis of a point that belongs to the line
    /// and the normal vector of the line. The positive halfplane is defined by the vector <paramref name="v"/>.
    /// </summary>
    /// <param name="p">A point that belongs to the line</param>
    /// <param name="v">The direction vector of the line</param>
    /// <returns>The resultant line</returns>
    public static Line2D Line2D_PointAndNormal (Point2D p, Vector2D v)
    {
      Line2D res = new Line2D();
      res.Normal = new Vector2D(v).Normalize();

      res.Direct = new Vector2D(res.Normal.Y, -res.Normal.X);
      res.A = res.Normal.X;
      res.B = res.Normal.Y;
      res.C = -res.Normal * (Vector2D)p;

      return res;
    }

    /// <summary>
    /// Copying constructor
    /// </summary>
    /// <param name="l">The line to be copied</param>
    public Line2D (Line2D l)
    {
      A = l.A;
      B = l.B;
      C = l.C;
      Direct = l.Direct;
      Normal = l.Normal;
    }

    /// <summary>
    /// Constructing a line passing through the given segment; 
    /// the positive halfplane os to the left of directional vector of the segment
    /// </summary>
    /// <param name="s">The given segment</param>
    public Line2D (Segment s) : this (s.p1, s.p2) { }
    #endregion

    #region Supplementary functions
    /// <summary>
    /// Compute value of the corresponding linear function at the given point
    /// </summary>
    /// <param name="p">The point where to compute the function value</param>
    /// <returns>The computed value</returns>
    public double this[Point2D p]
    {
      get => A * p.x + B * p.y + C;
    }

    /// <summary>
    /// Checks whether the line passes through the given point
    /// </summary>
    /// <param name="p">The given point</param>
    /// <returns>true, if passes; false, otherwise</returns>
    public bool PassesThrough (Point2D p) => Tools.EQ(this[p]);

    /// <summary>
    /// A function that generates a new line that coincides with the current one, 
    /// but has the opposite orientation
    /// </summary>
    /// <returns></returns>
    public Line2D Reorient ()
    {
      Line2D res = new Line2D();
      res.A = -A;
      res.B = -B;
      res.C = -C;
      res.Direct = -Direct;
      res.Normal = -Normal;
      return res;
    }

    /// <summary>
    /// Type of crossing two lines
    /// </summary>
    public enum LineCrossType 
    {
      /// <summary>
      ///  Two lines are not parallel or overlapping
      /// </summary>
      SinglePoint,

      /// <summary>
      /// Two lines are parallel
      /// </summary>
      Parallel,

      /// <summary>
      /// Two lines are overlapping
      /// </summary>
      Overlap
    }

    /// <summary>
    /// Intersectio of two lines.
    /// Returns the intersection type (single point, parallel, overlap) and
    /// the intersection point (if it exists and is single) 
    /// </summary>
    /// <param name="l1">The first line</param>
    /// <param name="l2">The second line</param>
    /// <param name="res">The intersection point (if exists and unique)</param>
    /// <returns>The type of imposition of the lines (crossing, parallel, overlapping) </returns>
    public static LineCrossType Intersect (Line2D l1, Line2D l2, out Point2D res)
    {
      double
        d = l1.A * l2.B - l1.B * l2.A,
        d1 = -(l1.C * l2.B - l2.C * l1.B),
        d2 = -(l1.A * l2.C - l1.C * l2.A);
      if (Tools.NE(d))
      {
        res = new Point2D(d1 / d, d2 / d);
        return LineCrossType.SinglePoint;
      }
      else
      {
        res = null;
        if (Tools.EQ(d1))
          return LineCrossType.Overlap;
        else
          return LineCrossType.Parallel;
      }
    }
    #endregion
  }
}
