namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Represents the mutable variant of <see cref="LinearBasis"/>.
  /// </summary>
  /// <remarks>
  /// This type owns mutable matrix storage and is intended for incremental algorithms that extend the basis in place.
  /// </remarks>
  public class LinearBasisMutable : LinearBasis {

    private MatrixMutable MutableBasis => (MatrixMutable)_Basis;

    private static MatrixMutable PrepareMutableBasis(LinearBasis lb) {
      return new MatrixMutable(lb.BasisStorage, true);
    }

    /// <summary>
    /// Constructs a mutable one-dimensional basis spanned by the specified non-zero vector.
    /// </summary>
    public LinearBasisMutable(Vector v) : base(v) { _Basis = new MatrixMutable(_Basis, false); }

    /// <summary>
    /// Constructs the mutable standard full-dimensional basis of the ambient space.
    /// </summary>
    public LinearBasisMutable(int spaceDim) : base(spaceDim) { _Basis = new MatrixMutable(_Basis, false); }

    /// <summary>
    /// Constructs the mutable standard coordinate basis of the specified subspace dimension.
    /// </summary>
    public LinearBasisMutable(int spaceDim, int subSpaceDim) : base(spaceDim, subSpaceDim) {
      _Basis = new MatrixMutable(_Basis, false);
    }

    /// <summary>
    /// Constructs a mutable orthonormal basis from the specified vectors.
    /// </summary>
    public LinearBasisMutable(int spaceDim, IEnumerable<Vector> Vs) : base(spaceDim, Vs) {
      _Basis = new MatrixMutable(_Basis, false);
    }

    /// <summary>
    /// Constructs a mutable orthonormal basis from the specified vectors.
    /// </summary>
    public LinearBasisMutable(params IEnumerable<Vector> Vs) : base(Vs) { _Basis = new MatrixMutable(_Basis, false); }

    /// <summary>
    /// Constructs the mutable basis of the sum of two subspaces.
    /// </summary>
    public LinearBasisMutable(LinearBasis lb1, LinearBasis lb2) : base(lb1, lb2) { _Basis = new MatrixMutable(_Basis, false); }

    /// <summary>
    /// Constructs a mutable linear basis from another basis.
    /// </summary>
    /// <param name="lb">The source basis.</param>
    /// <remarks>
    /// A dedicated mutable copy is always materialized. Public mutable-sharing is not supported.
    /// </remarks>
    public LinearBasisMutable(LinearBasis lb) : base(PrepareMutableBasis(lb), lb.SubSpaceDim) { }


    /// <summary>
    /// Tries to add the specified vector to the basis.
    /// </summary>
    /// <param name="v">Vector to add if it increases the subspace dimension.</param>
    /// <returns><c>true</c> if the basis was extended; otherwise, <c>false</c>.</returns>
    public bool AddVector(Vector v) => AddVectorInPlace(MutableBasis, ref SubSpaceDim, ref _projMatrix, v);

    /// <summary>
    /// Tries to add all vectors from the sequence to the basis.
    /// </summary>
    /// <param name="vs">Vectors to process in order.</param>
    public void AddVectors(IEnumerable<Vector> vs) => AddVectorsInPlace(MutableBasis, ref SubSpaceDim, ref _projMatrix, vs);

    /// <summary>
    /// Generates a random mutable orthonormal basis of the specified subspace dimension.
    /// </summary>
    public new static LinearBasisMutable GenLinearBasis(int spaceDim, int subSpaceDim, GRandomLC? random = null) {
      LinearBasisMutable lb = new LinearBasisMutable(spaceDim, 0);
      if (subSpaceDim == 0) { return lb; }
      do {
        lb.AddVector(Vector.GenVector(spaceDim, random));
      } while (lb.SubSpaceDim != subSpaceDim);

      return lb;
    }

    /// <summary>
    /// Generates a random mutable full-dimensional orthonormal basis.
    /// </summary>
    public new static LinearBasisMutable GenLinearBasis(int spaceDim, GRandomLC? random = null)
      => new LinearBasisMutable(LinearBasis.GenLinearBasis(spaceDim, random));

  }

}
