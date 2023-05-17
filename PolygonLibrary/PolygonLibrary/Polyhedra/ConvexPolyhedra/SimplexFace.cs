using System.Collections.Generic;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra; 

internal class SimplexFace : Polyhedron, IFace {

  private HyperPlane _hyperPlane;

  private List<ISubspacePoint> _verticies;

}
