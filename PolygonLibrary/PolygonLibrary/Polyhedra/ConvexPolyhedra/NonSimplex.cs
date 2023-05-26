using System.Collections.Generic;
using System.Linq;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra;

public class NonSimplex : IConvexPolyhedron {

  /// <summary>
  /// Gets the dimension of the polyhedron.
  /// </summary>
  public int Dim { get; }


  /// <summary>
  /// Gets the type of the convex polyhedron.
  /// </summary>
  public ConvexPolyhedronType Type { get; }

  /// <summary>
  /// Gets the set of vertices of the polyhedron.
  /// </summary>
  public HashSet<Point> Vertices { get; }

  /// <summary>
  /// Gets the set of (d-1)-dimensional faces of the polyhedron.
  /// </summary>
  public HashSet<IConvexPolyhedron> Faces { get; }


  /// <summary>
  /// Gets the set of (d-2)-dimensional edges of the polyhedron.
  /// </summary>
  private HashSet<IConvexPolyhedron>? _edges = null;

  /// <summary>
  /// Gets the set of (d-2)-dimensional edges of the polyhedron.
  /// </summary>
  public HashSet<IConvexPolyhedron> Edges => _edges ??= IConvexPolyhedron.ConstructEdges(Faces);


  /// <summary>
  /// The dictionary, which key is (d-2)-dimensional edge and the value is a pair of incident (d-1)-dimensional faces.
  /// </summary>
  private Dictionary<IConvexPolyhedron, (IConvexPolyhedron, IConvexPolyhedron)>? _faceIncidence = null;

  /// <summary>
  /// Gets the dictionary, which key is (d-2)-dimensional edge and the value is a pair of incident (d-1)-dimensional faces.
  /// </summary>
  public Dictionary<IConvexPolyhedron, (IConvexPolyhedron F1, IConvexPolyhedron F2)> FaceIncidence =>
    _faceIncidence ??= IConvexPolyhedron.ConstructFaceIncidence(Faces, Edges);

  /// <summary>
  /// The dictionary where the key is a d-dimensional point and the value is a set of faces that are incident to this point.
  /// </summary>
  private Dictionary<Point, HashSet<IConvexPolyhedron>>? _fans = null;

  /// <summary>
  /// Gets the dictionary where the key is a d-dimensional point and the value is a set of faces that are incident to this point.
  /// </summary>
  public Dictionary<Point, HashSet<IConvexPolyhedron>> Fans => _fans ??= IConvexPolyhedron.ConstructFans(Vertices, Faces);

  /// <summary>
  /// Construct a new instance of the <see cref="NonSimplex"/> class based on it's faces. 
  /// </summary>
  /// <param name="faces">Faces to construct the convex polyhedron</param>
  public NonSimplex(HashSet<IConvexPolyhedron> faces) {
    Dim   = faces.First().Dim + 1;
    Type  = ConvexPolyhedronType.NonSimplex;
    Faces = faces;

    Vertices = new HashSet<Point>();

    foreach (IConvexPolyhedron face in Faces) {
      Vertices.UnionWith(face.Vertices);
    }
  }

}
