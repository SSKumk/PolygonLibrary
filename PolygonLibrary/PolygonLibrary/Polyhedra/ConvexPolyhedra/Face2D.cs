using System.Collections.Generic;
using PolygonLibrary.Basics;
using PolygonLibrary.Polygons.ConvexPolygons;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra; 

internal class Face2D : IFace {

  private ConvexPolygon _convexPolygon;
  
  
  
  //todo нужны ли нам точки в подпространстве с координатами локальными
  
  public IEnumerable<Point> GetPoints()        { throw new System.NotImplementedException(); }
  public IEnumerable<Point> GetPoints(int dim) { throw new System.NotImplementedException(); }

}
