using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra;

/// <summary>
/// Represents a simplex, which is a convex polyhedron with (d + 1) vertices in d-dimensional space.
/// </summary>
public class Simplex : IConvexPolyhedron {

  /// <summary>
  /// Gets the dimension of the simplex.
  /// </summary>
  public int Dim { get; }

  /// <summary>
  /// Gets the type of the convex polyhedron.
  /// </summary>
  public ConvexPolyhedronType Type { get; }

  /// <summary>
  /// Gets the set of vertices of the simplex. //todo Где-то должна быть связь по точкам. Возможно вершины ISubspacePoint
  /// </summary>
  public HashSet<Point> Vertices { get; }

  /// <summary>
  /// The set of (d-1)-dimensional faces of the simplex.
  /// </summary>
  private HashSet<IFace>? _faces = null;

  /// <summary>
  /// Gets the set of (d-1)-dimensional faces of the simplex.
  /// </summary>
  public HashSet<IFace> Faces {
    get
      {
        if (_faces is null) {
          _faces = new HashSet<IFace>();

          foreach (Point vertex in Vertices) {
            IEnumerable<Point> faceVert          = Vertices.Where(v => !v.Equals(vertex));
            AffineBasis        faceABasis        = new AffineBasis(faceVert);
            IEnumerable<Point> faceVertProjected = faceABasis.ProjectPoints(faceVert);

            IFace face;

            if (faceABasis.BasisDim == 2) {
              face = new Face2D(faceVertProjected);
            } else {
              face = new Face(faceVertProjected);
            }

            _faces.Add(face);
          }
        }

        return _faces;
      }
  }

  /// <summary>
  /// The set of (d-2)-dimensional edges of the simplex.
  /// </summary>
  private HashSet<IConvexPolyhedron>? _edges = null;

  /// <summary>
  /// Gets the set of (d-2)-dimensional edges of the simplex.
  /// </summary>
  public HashSet<IConvexPolyhedron> Edges {
    get
      {
        if (_edges is null) {
          _edges = new HashSet<IConvexPolyhedron>();

          foreach (IFace face in Faces) {
            _edges.UnionWith(face.Polyhedron.Faces.Select(e => e.Polyhedron));
          }
        }

        return _edges;
      }
  }

  private Dictionary<IConvexPolyhedron, (IFace, IFace?)>? _faceIncidence = null;

  private Dictionary<Point, HashSet<IFace>>? _fans = null;


  /// <summary>
  /// Gets the dictionary, which key is (d-2)-dimensional edge and the value is a pair of incident (d-1)-dimensional faces.
  /// The second face can be equal to null if it is not constructed yet. 
  /// </summary>
  public Dictionary<IConvexPolyhedron, (IFace, IFace?)> FaceIncidence => throw new NotImplementedException();

  /// <summary>
  /// Gets a dictionary where the key is a d-dimensional point and the value is a set of faces that are incident to this point.
  /// </summary>
  public Dictionary<Point, HashSet<IFace>> Fans => throw new NotImplementedException();

  /// <summary>
  /// Initializes a new instance of the <see cref="Simplex"/> class with the specified set of vertices and dimension.
  /// </summary>
  /// <param name="simplex">The set of vertices of the simplex.</param>
  /// <param name="spaceDim">The dimension of the space containing the simplex.</param>
  public Simplex(IEnumerable<Point> simplex) {
    Debug.Assert
      (
       simplex.Count() >= 4
     , $"The simplex must have at least four points! Found {simplex.Count()}." +
       $"\n If you want to create 2D-simplex use the TwoDimensional class instead."
      );

    int spaceDim = simplex.First().Dim;

    Debug.Assert(simplex.Count() == spaceDim + 1, $"Simplex must have spaceDim + 1 = {spaceDim + 1} points! Found {simplex.Count()}");

    AffineBasis aBasis = new AffineBasis(simplex);

    Debug.Assert(aBasis.IsFullDim, "All points can't lie in one hyperplane.");

    Dim      = spaceDim;
    Type     = ConvexPolyhedronType.Simplex;
    Vertices = new HashSet<Point>(simplex);
  }

}
