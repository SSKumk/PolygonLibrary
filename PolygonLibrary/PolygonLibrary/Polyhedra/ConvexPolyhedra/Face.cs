using System.Collections.Generic;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra; 

internal class Face : IFace {

  private HyperPlane _hyperPlane;

  private Polyhedron _polyhedron;
  
  
  public IEnumerable<ISubspacePoint> GetPoints() { throw new System.NotImplementedException(); }

}
