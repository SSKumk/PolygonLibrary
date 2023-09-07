using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace CGLibrary.Polyhedra.ConvexPolyhedra.GiftWrapping;

/// <summary>
/// Represents a simplex, which is a convex polyhedron with (d + 1) vertices in d-dimensional space.
/// </summary>
internal class SubSimplex : BaseSubCP {

  /// <summary>
  /// Gets the dimension of the simplex.
  /// </summary>
  public override int PolyhedronDim { get; }

  /// <summary>
  /// Gets the type of the convex polyhedron.
  /// </summary>
  public override SubCPType Type { get; }

  /// <summary>
  /// Gets the set of vertices of the simplex.
  /// </summary>
  public override HashSet<SubPoint> Vertices { get; }


  /// <summary>
  /// The set of (d-1)-dimensional faces of a simplex, which are in turn a Simplex.
  /// </summary>
  private HashSet<BaseSubCP>? _faces = null;

  /// <summary>
  /// Gets the set of (d-1)-dimensional faces of the simplex, which are in turn a Simplex.
  /// </summary>
  public override HashSet<BaseSubCP> Faces {
    get
      {
        if (_faces is null) {
          _faces = new HashSet<BaseSubCP>();

          foreach (SubPoint vertex in Vertices) {
            SubPoint[] faceVert = Vertices.Where(v => !v.Equals(vertex)).ToArray();

            if (PolyhedronDim == 2) {
              BaseSubCP edge = new SubTwoDimensionalEdge(faceVert[0], faceVert[1]);
              _faces.Add(edge);
            } else {
              BaseSubCP simplex = new SubSimplex(faceVert);
              _faces.Add(simplex);
            }
          }
        }

        return _faces;
      }
  }

  /// <summary>
  /// Information about face incidence.
  /// </summary>
  public override SubIncidenceInfo? FaceIncidence { get; }


  /// <summary>
  /// Lifts a simplex to the previous space.
  /// </summary>
  /// <returns>A simplex in (d+1)-space.</returns>
  public override BaseSubCP ToPreviousSpace() => new SubSimplex(Vertices.Select(v => v.Parent)!);

  public override BaseSubCP ProjectTo(AffineBasis aBasis) {
    return new SubSimplex(Vertices.Select(s => new SubPoint(s.ProjectTo(aBasis), s, s.Original)));
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="SubSimplex"/> class with the specified set of vertices and dimension.
  /// </summary>
  /// <param name="simplex">The set of vertices of the simplex.</param>
  /// <param name="faces">The set of faces of the simplex.</param>
  /// <param name="incidence">Information about face incidence.</param>
  public SubSimplex(IEnumerable<SubPoint> simplex, HashSet<BaseSubCP>? faces = null, SubIncidenceInfo? incidence = null) {
    Debug.Assert(simplex.Count() >= 3, $"The simplex must have at least three points! Found {simplex.Count()}.");

    PolyhedronDim = simplex.Count() - 1;
    Type          = SubCPType.Simplex;
    Vertices      = new HashSet<SubPoint>(simplex);
    _faces        = faces;
    FaceIncidence = incidence;
  }

}
