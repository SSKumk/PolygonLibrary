using System.Collections.Generic;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra;

public interface IFace {

  IEnumerable<AffineBasis> BasisEdges();

  IEnumerable<ISubspacePoint> GetPoints();

  IEnumerable<Point> GetPoints(int dim);

}
