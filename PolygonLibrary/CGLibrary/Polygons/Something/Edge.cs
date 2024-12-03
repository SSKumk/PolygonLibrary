using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolygonLibrary.Basics;
using PolygonLibrary.Segments;

namespace PolygonLibrary.Polygons
{
  /// <summary>
  /// Edge is a segment, which remembers to what contour it belongs
  /// </summary>
  /// <typeparam name="P">Type of the polygon of the edge</typeparam>
  public class Edge<P> : Segment
    where P : BasicPolygon
  {
    #region Constructors
    /// <summary>
    /// Coordinate constructor
    /// </summary>
    /// <param name="parent">Parent contour</param>
    /// <param name="x1">The abscissa of the first end</param>
    /// <param name="y1">The ordinate of the first end</param>
    /// <param name="x2">The abscissa of the second end</param>
    /// <param name="y2">The ordinate of the second end</param>
    public Edge(Contour<P> parent, double x1, double y1, double x2, double y2)
      : base(x1, y1, x2, y2)
    {
      Contour = parent;
    }

    /// <summary>
    /// Two points constructor
    /// </summary>
    /// <param name="parent">Parent contour</param>
    /// <param name="np1">The new first end</param>
    /// <param name="np2">The new second end</param>
    public Edge(Contour<P> parent, Point2D np1, Point2D np2) : base (np1, np2)
    {
      Contour = parent;
    }

    /// <summary>
    /// Segement constructor
    /// </summary>
    /// <param name="parent">Parent contour</param>
    /// <param name="s">The segment to be copied</param>
    public Edge(Contour<P> parent, Segment s) : base (s)
    {
      Contour = parent;
    }
    #endregion

    /// <summary>
    /// Parent contour property
    /// </summary>
    public Contour<P> Contour { get; protected set; } 
  }
}
