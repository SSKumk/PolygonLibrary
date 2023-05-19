using System.Collections.Generic;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra;

/// <summary>
/// Represents a (d-1)-dimensional face of a convex polyhedron.
/// </summary>
public interface IFace {

  /// <summary>
  /// Gets the convex polyhedron which forms this face.
  /// </summary>
  IConvexPolyhedron Polyhedron { get; }

  /// <summary>
  /// Gets the (d-1)-dimensional hyperplane that goes through this face.
  /// </summary>
  HyperPlane HyperPlane { get; }

  /// <summary>
  /// Gets the (d-1)-dimensional affine basis of the hyperplane that goes through this face.
  /// </summary>
  AffineBasis AffineBasis => HyperPlane.AffineBasis;

  /// <summary>
  /// Gets the normal vector of the hyperplane that defines this face.
  /// The normal lies in the positive semi-space of the hyperplane.
  /// </summary>
  Vector Normal => HyperPlane.Normal;

  /// <summary>
  /// Gets the set of (d-2)-dimensional edges that belong to this face.
  /// </summary>
  HashSet<IConvexPolyhedron> Edges { get; }

  /// <summary>
  /// Gets an enumerable collection of subspace points that belong to this face.
  /// </summary>
  IEnumerable<ISubspacePoint> GetPoints();

}
