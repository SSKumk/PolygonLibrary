using System;
using System.Collections.Generic;
using PolygonLibrary.Basics;
using PolygonLibrary.Polygons.ConvexPolygons;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra;

public class Face2D : ConvexPolygon, IFace {

  public     BaseConvexPolyhedron           Polyhedron  => throw new NotImplementedException();
  public     HyperPlane                  HyperPlane  { get; }
  public new HashSet<BaseConvexPolyhedron>  Edges       => throw new NotImplementedException();
  public     IEnumerable<ISubspacePoint> GetPoints() { throw new NotImplementedException(); }

  public Face2D(IEnumerable<Point> Ps) : base(Ps) { //todo Наверное надо принимать ISubspacePoint, но как тогда конструировать base(Ps)???
    AffineBasis faceABasis = new AffineBasis(Ps);
    HyperPlane = new HyperPlane(faceABasis); //todo Как переориентировать нормаль если что???
    // Polyhedron = new TwoDimensional();
  }

}
