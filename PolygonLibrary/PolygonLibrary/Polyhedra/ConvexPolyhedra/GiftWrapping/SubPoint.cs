using System;
using System.Diagnostics;
using System.Numerics;


namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Represents a point in a subspace greater than 2.
  /// </summary>
  public class SubPoint : Vector {

    // /// <summary>
    // /// Gets the original point in the original coordinate system.
    // /// Original point is the point before any projection to subspaces
    // /// </summary>
    // public Vector Original { get; }

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
    public override bool Equals(object? other) {
      if (other is null) {
        return false;
      }

      return (Vector)this == (Vector)other;
    }

    /// <summary>
    /// The hash code of SubPoint. It equal to Vector.GHC().
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() => base.GetHashCode();

    /// <summary>
    /// Construct a new instance of the <see cref="SubPoint"/> class.
    /// </summary>
    /// <param name="np">The point.</param>
    /// <param name="parent">The point from which the current point was projected.</param>
    public SubPoint(TNum[] np, SubPoint? parent) : base(np) { Parent = parent; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubPoint"/> class.
    /// </summary>
    /// <param name="p">The point.</param>
    /// <param name="parent">The point from which the current point was projected.</param>
    public SubPoint(Vector p, SubPoint? parent) : base(p) { Parent = parent; }

    /// <summary>
    /// Projects the current point to the specified affine basis.
    /// </summary>
    /// <param name="aBasis">The affine basis of non greater dimension to project the point to.</param>
    /// <returns>The projected point.</returns>
    public SubPoint ProjectTo(AffineBasis aBasis) => new SubPoint(aBasis.ProjectPoint(this), this);

    /// <summary>
    /// Returns the point from which the current point was firstly projected.
    /// </summary>
    /// <returns>The root vertex of the current point.</returns>
    public Vector GetRootVertex() {
      SubPoint root = this;
      while (root.Parent is not null) {
        root = root.Parent;
      }

      return root;
    }

  }

}
