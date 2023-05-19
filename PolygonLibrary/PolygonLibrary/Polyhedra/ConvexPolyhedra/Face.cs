using System;
using System.Collections.Generic;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra; 

public class Face : IFace {

  public IConvexPolyhedron Polyhedron => throw new System.NotImplementedException();

  public HyperPlane HyperPlane => throw new System.NotImplementedException();

  public HashSet<IConvexPolyhedron> Edges => throw new System.NotImplementedException();

  public IEnumerable<ISubspacePoint> GetPoints() { throw new System.NotImplementedException(); }

  public Face(IEnumerable<Point> Ps) {
    throw new NotImplementedException();
  }

}
