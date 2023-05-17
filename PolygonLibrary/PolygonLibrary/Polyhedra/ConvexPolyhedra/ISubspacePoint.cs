using System;
using System.Collections.Generic;
using System.Diagnostics;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra;

/// <summary>
/// Interface represents a point in a subspace.
/// </summary>
public interface ISubspacePoint : IEquatable<ISubspacePoint> {

  //todo Как получить точку в подпространстве размерности этого подпространства
  /// <summary>
  /// Gets the original point in the original coordinate system.
  /// Original point is the point before any projection to subspaces
  /// </summary>
  Point Original { get; }

  /// <summary>
  /// Gets the parent point in the parent coordinate system.
  /// Parent point is the point from which the current point was obtained
  /// </summary>
  ISubspacePoint? Parent { get; }

  /// <summary>
  /// The dimension of the point
  /// </summary>
  int Dim { get; }


  /// <summary>
  /// Projects the current point to the specified affine basis.
  /// </summary>
  /// <param name="aBasis">The affine basis of non greater dimension to project the point to.</param>
  /// <returns>The projected point.</returns>
  ISubspacePoint ProjectTo(AffineBasis aBasis);

  /// <summary>
  /// Projects a collection of subspace points to the specified affine basis.
  /// </summary>
  /// <param name="aBasis">The affine basis to project the points to.</param>
  /// <param name="Swarm">The collection of subspace points to project.</param>
  /// <returns>The collection of projected subspace points.</returns>
  public static IEnumerable<ISubspacePoint> ProjectTo(AffineBasis aBasis, IEnumerable<ISubspacePoint> Swarm) {
    foreach (ISubspacePoint p in Swarm) {
      yield return p.ProjectTo(aBasis);
    }
  }

}
