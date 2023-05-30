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
public class Simplex : BaseConvexPolyhedron {

  /// <summary>
  /// Gets the dimension of the simplex.
  /// </summary>
  public override int Dim { get; }

  /// <summary>
  /// Gets the type of the convex polyhedron.
  /// </summary>
  public override ConvexPolyhedronType Type { get; }

  /// <summary>
  /// Gets the set of vertices of the simplex.
  /// </summary>
  public override HashSet<Point> Vertices { get; }

  /// <summary>
  /// The set of (d-1)-dimensional faces of a simplex, which are in turn a Simplex.
  /// </summary>
  private HashSet<BaseConvexPolyhedron>? _faces = null; //todo почему я сюда не могу написать Simplex. 
  // todo Simplex2D как с ним быть?

  /// <summary>
  /// Gets the set of (d-1)-dimensional faces of the simplex, which are in turn a Simplex.
  /// </summary>
  public override HashSet<BaseConvexPolyhedron> Faces {
    get
      {
        if (_faces is null) {
          _faces = new HashSet<BaseConvexPolyhedron>();

          foreach (Point vertex in Vertices) {
            IEnumerable<Point>    faceVert = Vertices.Where(v => !v.Equals(vertex));
            BaseConvexPolyhedron? simplex  = null;

            if (Dim == 3) { //Therefore Face.Dim == 2
              // simplex = new TwoDimensional(faceVert);
              throw new NotImplementedException();
            } else {
              simplex = new Simplex(faceVert, Dim - 1);
            }

            _faces.Add(simplex);
          }
        }

        return _faces;
      }
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Simplex"/> class with the specified set of vertices and dimension.
  /// </summary>
  /// <param name="simplex">The set of vertices of the simplex.</param>
  /// <param name="simplexDim">The dimension of the simplex</param>
  public Simplex(IEnumerable<Point> simplex, int simplexDim) {
    Debug.Assert(simplex.Count() >= 3, $"The simplex must have at least three points! Found {simplex.Count()}.");
    Debug.Assert(simplex.Count() == simplexDim + 1, "The simplex must have amount points equal to simplexDim + 1");

    Dim      = simplexDim;
    Type     = ConvexPolyhedronType.Simplex;
    Vertices = new HashSet<Point>(simplex);
  }

  /// <summary>
  /// The copy constructor
  /// </summary>
  /// <param name="simplex">The copied simplex</param>
  public Simplex(Simplex simplex) {
    Dim            = simplex.Dim;
    Type           = ConvexPolyhedronType.Simplex;
    Vertices       = simplex.Vertices;
    _faces         = simplex._faces;
    _fans          = simplex._fans;
    _faceIncidence = simplex._faceIncidence;
  }

}
