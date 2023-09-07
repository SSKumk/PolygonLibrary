using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace CGLibrary.Polyhedra.ConvexPolyhedra.GiftWrapping;

/// <summary>
/// Represents a point in a subspace greater than 2.
/// </summary>
public class SubPoint : Point {

  /// <summary>
  /// Gets the original point in the original coordinate system.
  /// Original point is the point before any projection to subspaces
  /// </summary>
  public Point Original { get; }

  /// <summary>
  /// Gets the parent point in the parent coordinate system.
  /// Parent point is the point from which the current point was obtained
  /// </summary>
  public SubPoint? Parent { get; }

  /// <summary>
  /// The dimension of the point
  /// </summary>
  public new int Dim => base.Dim;

  /// <summary>
  /// The comparison of the subspace point with another
  /// </summary>
  /// <param name="other">another subspace point to compare</param>
  /// <returns>true if equal, false otherwise</returns>
  public bool Equals(SubPoint? other) {
    if (other is null) {
      return false;
    }

    if (Dim != other.Dim) {
      return false;
    }

    return (Point)this == (Point)other;
  }

  /// <summary>
  /// Determines whether subspace point and object are equal.
  /// </summary>
  /// <param name="o">The object to compare.</param>
  /// <returns>true if the points are equal; otherwise, false.</returns>
  public override bool Equals(Object? o) {
    if (o is null) {
      return false;
    }

    if (o is SubPoint otherSPoint) {
      if (Dim != otherSPoint.Dim) {
        return false;
      }

      return (Point)this == otherSPoint;
    }

    if (o is Point otherPoint) {
      if (Dim != otherPoint.Dim) {
        return false;
      }

      return (Point)this == otherPoint;
    }

    return false;
  }

  /// <summary>
  /// The hash code
  /// </summary>
  /// <returns>The hash code</returns>
  public override int GetHashCode() { return HashCode.Combine(base.GetHashCode(), Original.GetHashCode()); }

  /// <summary>
  /// Construct a new instance of the <see cref="SubPoint"/> class.
  /// </summary>
  /// <param name="np">The projected point</param>
  /// <param name="parent">The point from which the current point was projected</param>
  /// <param name="original">The point from which all parents point were projected</param>
  public SubPoint(double[] np, SubPoint? parent, Point original) : base(np) {
    Original = original;
    Parent   = parent;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="SubPoint"/> class.
  /// </summary>
  /// <param name="p">The projected point</param>
  /// <param name="parent">The point from which the current point was projected</param>
  /// <param name="original">The point from which all parents point were projected</param>
  public SubPoint(Point p, SubPoint? parent, Point original) : base(p) {
    Original = original;
    Parent   = parent;
  }

  /// <summary>
  /// Projects the current point to the specified affine basis.
  /// </summary>
  /// <param name="aBasis">The affine basis of non greater dimension to project the point to.</param>
  /// <returns>The projected point.</returns>
  public SubPoint ProjectTo(AffineBasis aBasis) {
    Debug.Assert(aBasis.SpaceDim > 1, "Can not create one dimensional subspace point.");

    Point projectPoint = aBasis.ProjectPoint(this);

    return new SubPoint(projectPoint, this, Original);
  }


  // /// <summary>
  // /// Projects the current point to the specified affine basis.
  // /// </summary>
  // /// <param name="aBasis">The affine basis of non greater dimension to project the point to.</param>
  // /// <param name="p">Point to project</param>
  // /// <returns>The projected point.</returns>
  // public static SubPoint ProjectTo(AffineBasis aBasis, Point p) {
  //   Point projectPoint = aBasis.ProjectPoint(p);
  //
  //   return new SubPoint(projectPoint, new SubPoint(p, null, p), p);
  // }

}
