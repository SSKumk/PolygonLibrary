namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Represents a point in a subspace greater than 2.
  /// </summary>
  public class SubPoint : Vector {
    /// <summary>
    /// Gets the parent point in the parent coordinate system.
    /// Parent point is the point from which the current point was obtained
    /// </summary>
    public SubPoint? Parent { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubPoint"/> class.
    /// </summary>
    /// <param name="p">The point.</param>
    /// <param name="parent">The point from which the current point was projected.</param>
    public SubPoint(Vector p, SubPoint? parent) : base(p) { Parent = parent; }

    /// <summary>
    /// Projects the current point to the specified affine basis.
    /// </summary>
    /// <param name="aBasis">The affine basis of non-greater dimension to project the point to.</param>
    /// <returns>The projected point.</returns>
    public SubPoint ProjectTo(AffineBasis aBasis) => new SubPoint(aBasis.ProjectPointToSubSpace(this), this);

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
