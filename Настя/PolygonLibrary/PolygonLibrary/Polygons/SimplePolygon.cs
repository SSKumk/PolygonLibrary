using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolygonLibrary.Basics;

namespace PolygonLibrary.Polygons
{
  /// <summary>
  /// Class of a simple polygon whose boundary is a non-selfintersecting polyline
  /// counted counterclockeise
  /// </summary>
  public class SimplePolygon : Polyline
  {
    #region Constructors
    /// <summary>
    /// Default constructor producing an empty polygon
    /// </summary>
    public SimplePolygon() : base()  { }

    /// <summary>
    /// Constructing a polygon on the basis of the given list of its vertices
    /// </summary>
    /// <param name="ps">List of vertices</param>
    public SimplePolygon(Point2D[] ps) : 
      base(ps, PolylineOrientation.Counterclockwise, false, false) { }
    #endregion
  }
}
