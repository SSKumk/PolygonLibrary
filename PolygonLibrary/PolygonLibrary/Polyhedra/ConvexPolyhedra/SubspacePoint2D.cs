using System;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra; 

public class SubspacePoint2D : Point2D, ISubspacePoint {

  public Point Original { get; }

  public ISubspacePoint? Parent { get; }
  
  
  /// <summary>
  /// The dimension of the point2D = 2
  /// </summary>
  public int Dim => 2;

  /// <summary>
  /// The comparison of the subspace point with another
  /// </summary>
  /// <param name="other">another subspace point to compare</param>
  /// <returns>true if equal, false otherwise</returns>
  public bool Equals(ISubspacePoint? other) {
    if (other is null) {
      return false;
    }

    if (Dim != other.Dim) {
      return false;
    }

    return (Point2D)this == (Point2D)other;
  }
  public ISubspacePoint ProjectTo(AffineBasis aBasis) { throw new ArgumentException("Can not project a two dimensional point."); }

  
  /// <summary>
  /// Determines whether subspace point and object are equal.
  /// </summary>
  /// <param name="o">The object to compare.</param>
  /// <returns>true if the points are equal; otherwise, false.</returns>
  public override bool Equals(Object? o) {
    if (o is null) {
      return false;
    }

    if (o is SubspacePoint2D otherSPoint) {
      if (Dim != otherSPoint.Dim) {
        return false;
      }

      return (Point2D)this == otherSPoint;
    }

    if (o is Point2D otherPoint) {
      if (Dim != 2) {
        return false;
      }

      return (Point2D)this == otherPoint;
    }

    return false;
  }

  /// <summary>
  /// The hash code
  /// </summary>
  /// <returns>The hash code</returns>
  public override int GetHashCode() {
    return HashCode.Combine(base.GetHashCode(), Original.GetHashCode());
  }
  
  /// <summary>
  /// Initializes a new instance of the <see cref="SubspacePoint2D"/> class.
  /// </summary>
  /// <param name="p">The projected point</param>
  /// <param name="parent">The point from which the current point was projected</param>
  /// <param name="original">The point from which all parents point were projected</param>
  public SubspacePoint2D(Point2D p, ISubspacePoint? parent, Point original) : base(p) {
    Original = original;
    Parent   = parent;
  }

}
