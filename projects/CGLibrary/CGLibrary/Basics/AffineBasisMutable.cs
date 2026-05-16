namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Represents the mutable variant of <see cref="AffineBasis"/>.
  /// </summary>
  /// <remarks>
  /// The direction basis is mutable and can be extended incrementally.
  /// </remarks>
  public class AffineBasisMutable : AffineBasis {

    /// <summary>
    /// Constructs the mutable full-dimensional standard affine basis with zero origin.
    /// </summary>
    public AffineBasisMutable(int vecDim) : base(vecDim) {
      _linearBasis = new LinearBasisMutable(vecDim, vecDim);
    }

    /// <summary>
    /// Constructs the mutable zero-dimensional affine basis consisting of the specified origin point.
    /// </summary>
    public AffineBasisMutable(Vector o) : base(o) {
      _linearBasis = new LinearBasisMutable(o.SpaceDim, 0);
    }

    /// <summary>
    /// Constructs the mutable affine hull of the specified points.
    /// </summary>
    public AffineBasisMutable(IEnumerable<Vector> Ps) : base(Ps) {
      _linearBasis = new LinearBasisMutable(_linearBasis);
    }

    /// <summary>
    /// Constructs a mutable affine basis from another affine basis.
    /// </summary>
    /// <param name="affineBasis">Source affine basis.</param>
    /// <remarks>
    /// A dedicated mutable copy of the direction basis is always materialized.
    /// Public mutable-sharing is not supported.
    /// </remarks>
    public AffineBasisMutable(AffineBasis affineBasis) : base(affineBasis.Origin) {
      _linearBasis = new LinearBasisMutable(affineBasis.LinBasis);

#if DEBUG
      CheckCorrectness(this);
#endif
    }

    /// <summary>
    /// Constructs a mutable affine basis from an origin point and a direction basis.
    /// </summary>
    /// <param name="o">Origin point.</param>
    /// <param name="lBasis">Direction basis.</param>
    /// <remarks>
    /// A dedicated mutable copy of the direction basis is always materialized.
    /// Public mutable-sharing is not supported.
    /// </remarks>
    public AffineBasisMutable(Vector o, LinearBasis lBasis) : base(o) {
      _linearBasis = new LinearBasisMutable(lBasis);

#if DEBUG
      CheckCorrectness(this);
#endif
    }


    /// <summary>
    /// Adds the vector to the linear basis associated with the affine basis.
    /// </summary>
    /// <param name="v">The vector to add. Not the point!</param>
    /// <returns><c>true</c> if the vector was added successfully; otherwise, <c>false</c>.</returns>
    public bool AddVector(Vector v) {
      Debug.Assert
        (Origin.SpaceDim == v.SpaceDim, "AffineBasis.AddVector: Adding a vector with a wrong dimension into an affine basis.");

      return ((LinearBasisMutable)_linearBasis).AddVector(v);
    }

  }

}
