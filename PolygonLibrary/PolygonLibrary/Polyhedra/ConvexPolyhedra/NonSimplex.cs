using System.Collections.Generic;
using System.Linq;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra;

public class NonSimplex : BaseConvexPolyhedron {

  /// <summary>
  /// Gets the dimension of the polyhedron.
  /// </summary>
  public override int Dim { get; }


  /// <summary>
  /// Gets the type of the convex polyhedron.
  /// </summary>
  public override  ConvexPolyhedronType Type { get; }

  /// <summary>
  /// Gets the set of vertices of the polyhedron.
  /// </summary>
  public override HashSet<Point> Vertices { get; }

  /// <summary>
  /// Gets the set of (d-1)-dimensional faces of the polyhedron.
  /// </summary>
  public override HashSet<BaseConvexPolyhedron> Faces { get; }

  /// <summary>
  /// Construct a new instance of the <see cref="NonSimplex"/> class based on it's faces. 
  /// </summary>
  /// <param name="faces">Faces to construct the convex polyhedron</param>
  public NonSimplex(HashSet<BaseConvexPolyhedron> faces) {
    Dim   = faces.First().Dim + 1;
    Type  = ConvexPolyhedronType.NonSimplex;
    Faces = faces;

    Vertices = new HashSet<Point>();

    foreach (BaseConvexPolyhedron face in Faces) {
      Vertices.UnionWith(face.Vertices);
    }
  }

}
