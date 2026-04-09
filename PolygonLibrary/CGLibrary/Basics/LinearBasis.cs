using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Represents an orthonormal basis of a linear subspace in the ambient space.
  /// </summary>
  /// <remarks>
  /// <para>
  /// <see cref="LinearBasis"/> is the immutable variant of the type family.
  /// Its internal storage is immutable after construction.
  /// </para>
  /// <para>
  /// Use <see cref="LinearBasisMutable"/> when the basis must be extended incrementally.
  /// </para>
  /// </remarks>
  public class LinearBasis : IEnumerable<Vector>, IComparable<LinearBasis> {

#region Data and Properties
    /// <summary>
    /// The dimension of the basis vectors.
    /// </summary>
    public int SpaceDim => _Basis.Cols;

    /// <summary>
    /// Gets the dimension of the represented subspace.
    /// </summary>
    public int SubSpaceDim;

    /// <summary>
    /// <c>True</c> if the basis contains d-linearly independent vectors in d-dimensional space.
    /// Indicates that the basis spans the entire space.
    /// </summary>
    public bool FullDim {
      get
        {
          if (Empty) {
            return false;
          }

          return SpaceDim == SubSpaceDim;
        }
    }

    /// <summary>
    /// <c>True</c> if there are no vectors in the basis.
    /// Checks whether the basis is empty.
    /// </summary>
    public bool Empty => SubSpaceDim == 0;

    /// <summary>
    /// Gets the basis vector with the specified index.
    /// </summary>
    /// <param name="ind">Zero-based index of the basis vector.</param>
    /// <returns>The corresponding orthonormal basis vector.</returns>
    public Vector this[int ind] {
      get
        {
          if (ind < 0 || ind >= SubSpaceDim) {
            throw new ArgumentException($"LinearBasis.this[]: Index should lie within [0, {SubSpaceDim}]. Found ind = {ind}");
          }

          Debug.Assert(!Empty, "LinearBasis.this[]: Basis is empty. Can't take a vector.");

          return Basis.TakeRowVector(ind);
        }
    }

    /// <summary>
    /// Gets the matrix whose rows are the basis vectors.
    /// </summary>
    /// <remarks>
    /// The returned matrix is immutable and shares the internal immutable storage of this basis.
    /// </remarks>
    public Matrix Basis {
      get
        {
          if (Empty) {
            throw new ArgumentException("Accessing empty basis!");
          }

          return new Matrix(SubSpaceDim, _Basis, needCopy: false);
        }
    }

    /// <summary>
    /// Gets the orthogonal projector onto the represented subspace in ambient coordinates.
    /// </summary>
    /// <remarks>
    /// For the row-orthonormal basis matrix <c>B</c>, this matrix is <c>B^T * B</c>.
    /// The value is cached lazily.
    /// </remarks>
    public Matrix ProjMatrix => _projMatrix ??= Basis.Transpose() * Basis; // todo: В одну операцию! MultiplyTransposeBySelf()

    protected Matrix  _Basis;
    protected Matrix? _projMatrix = null;
    internal  Matrix  BasisStorage => _Basis;
#endregion

#region Functions
    /// <summary>
    /// Builds an orthonormal basis of the orthogonal complement of the current subspace.
    /// </summary>
    /// <returns>A basis spanning the orthogonal complement of the current basis.</returns>
    public LinearBasis OrthogonalComplement()
      => new(MatrixMutable.SwapRowBlocks(new MatrixMutable(_Basis, false), SpaceDim, SubSpaceDim), SpaceDim - SubSpaceDim);

    /// <summary>
    /// Returns a unit vector from the orthogonal complement of the subspace spanned by this basis.
    /// </summary>
    /// <returns>
    /// A unit vector orthogonal to every vector of the current basis.
    /// Returns the zero vector if the basis already spans the whole ambient space.
    /// </returns>
    public Vector OrthogonalComplementVector() => FullDim ? Vector.Zero(SpaceDim) : _Basis.TakeRowVector(SubSpaceDim);

    /// <summary>
    /// Projects a vector onto the represented subspace and returns the result in ambient coordinates.
    /// </summary>
    /// <param name="v">The vector to project.</param>
    /// <returns>The orthogonal projection of <paramref name="v"/> onto the represented subspace.</returns>
    public Vector ProjectVectorToSubSpace_in_OrigSpace(Vector v) => ProjMatrix * v;

    /// <summary>
    /// Determines whether the specified vector belongs to the represented subspace.
    /// </summary>
    /// <param name="v">The vector to check.</param>
    /// <returns><c>true</c> if <paramref name="v"/> lies in the subspace; otherwise, <c>false</c>.</returns>
    public bool Contains(Vector v) {
      Debug.Assert
        (
         SpaceDim == v.SpaceDim
       , $"LinearBasis.Contains: The dimension of the vector must be equal to the dimension of the basis vectors! Found: {v.SpaceDim}"
        );

      if (FullDim) { return true; }
      if (Empty) { return v.IsZero; }

      // ProjMatrix * v == v
      for (int row = 0; row < SpaceDim; row++) {
        if (Tools.NE(ProjMatrix.MultiplyRowByVector(row, v), v[row])) {
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Orthonormalizes the given vector against the current basis.
    /// </summary>
    /// <param name="v">The input vector to orthonormalize.</param>
    /// <returns>The normalized component of <paramref name="v"/> orthogonal to the current subspace.</returns>
    /// <exception cref="NotImplementedException">The operation is not implemented yet.</exception>
    public Vector Orthonormalize(Vector v) { throw new NotImplementedException("todo"); }

    /// <summary>
    /// Tries to extend the mutable orthonormal basis storage by the specified vector.
    /// </summary>
    /// <param name="basis">Mutable orthonormal storage whose first <paramref name="subSpaceDim"/> rows are active basis vectors.</param>
    /// <param name="subSpaceDim">Current active basis dimension. Increased by one when the vector is added.</param>
    /// <param name="projMatrix">Cached projector associated with the mutable basis. Invalidated on mutation.</param>
    /// <param name="v">The vector to be added if it is independent from the current subspace.</param>
    /// <returns><c>true</c> if the vector increased the subspace dimension; otherwise, <c>false</c>.</returns>
    protected static bool AddVectorInPlace(MatrixMutable basis, ref int subSpaceDim, ref Matrix? projMatrix, Vector v) {
      int newSubSpaceDim = Decomposition.LQ_IncrementalUpdateCore(ref basis, subSpaceDim, v, alignNewBasisVectorWithInput: true);
      if (newSubSpaceDim == subSpaceDim) {
        return false;
      }

      subSpaceDim = newSubSpaceDim;
      projMatrix  = null;

      return true;
    }

    /// <summary>
    /// Tries to add all vectors from the sequence to the mutable basis storage.
    /// </summary>
    /// <param name="basis">Mutable orthonormal storage whose first <paramref name="subSpaceDim"/> rows are active basis vectors.</param>
    /// <param name="subSpaceDim">Current active basis dimension.</param>
    /// <param name="projMatrix">Cached projector associated with the mutable basis. Invalidated on mutation.</param>
    /// <param name="Vs">Vectors to process in order until the space becomes full-dimensional or the sequence ends.</param>
    protected static void AddVectorsInPlace(
        MatrixMutable       basis
      , ref int             subSpaceDim
      , ref Matrix?         projMatrix
      , IEnumerable<Vector> Vs
      ) {
      foreach (Vector v in Vs) {
        AddVectorInPlace(basis, ref subSpaceDim, ref projMatrix, v);
        if (subSpaceDim == basis.Cols) { break; }
      }
    }

    /// <summary>
    /// Projects a vector onto the represented subspace and returns its coordinates in this basis.
    /// </summary>
    /// <param name="v">The vector to project.</param>
    /// <returns>The coordinate vector of the orthogonal projection in the current basis.</returns>
    public Vector ProjectVectorToSubSpace(Vector v) => Basis * v;

    /// <summary>
    /// Projects a sequence of vectors onto the represented subspace and returns their basis coordinates.
    /// </summary>
    /// <param name="Swarm">The vectors to project.</param>
    /// <returns>The coordinate vectors of the orthogonal projections.</returns>
    public IEnumerable<Vector> ProjectVectorsToSubSpace(IEnumerable<Vector> Swarm) {
      foreach (Vector v in Swarm) {
        yield return ProjectVectorToSubSpace(v);
      }
    }

    /// <summary>
    /// Maps a coordinate vector in this basis back to ambient coordinates.
    /// </summary>
    /// <param name="coords">Coordinates in the current basis.</param>
    /// <returns>The corresponding ambient vector.</returns>
    public Vector ToOriginalCoords(Vector coords) => Basis.Transpose() * coords;


    /// <summary>
    /// Determines whether the current basis spans the same subspace as the specified basis.
    /// </summary>
    /// <param name="other">The basis to compare with the current basis.</param>
    /// <returns><c>true</c> if both bases span the same subspace; otherwise, <c>false</c>.</returns>
    public bool SpanSameSpace(LinearBasis other) {
      if (this.SubSpaceDim != other.SubSpaceDim) { return false; }

      // Базисы лежащие в разных пространствах -- разные
      if (this.SpaceDim != other.SpaceDim) { return false; }

      foreach (Vector otherbv in other) {
        if (!Contains(otherbv)) { return false; }
      }

      return true;
    }
#endregion

#region Constructors
    /// <summary>
    /// Constructs a one-dimensional basis spanned by the specified non-zero vector.
    /// </summary>
    /// <param name="v">A non-zero vector spanning the basis.</param>
    public LinearBasis(Vector v) {
      if (v.IsZero) {
        throw new ArgumentException("Cannot construct a linear basis from a zero vector.", nameof(v));
      }
      MatrixMutable basis       = MatrixMutable.Eye(v.SpaceDim);
      int           subSpaceDim = 0;
      Matrix?       projMatrix  = null;
      AddVectorInPlace(basis, ref subSpaceDim, ref projMatrix, v);
      _Basis      = new Matrix(basis, false);
      SubSpaceDim = subSpaceDim;

#if DEBUG
      CheckCorrectness();
#endif
    }

    /// <summary>
    /// Constructs the standard full-dimensional orthonormal basis of the ambient space.
    /// </summary>
    /// <param name="spaceDim">The dimension of the space.</param>
    public LinearBasis(int spaceDim) : this(spaceDim, spaceDim) { }

    /// <summary>
    /// Constructs the standard coordinate basis of the specified subspace dimension.
    /// </summary>
    /// <param name="spaceDim">Ambient space dimension.</param>
    /// <param name="subSpaceDim">Number of leading standard basis vectors to include.</param>
    public LinearBasis(int spaceDim, int subSpaceDim) {
      Debug.Assert
        (
         spaceDim >= subSpaceDim
       , $"LinearBasis: The dimension of the vectors in basis must be greater or equal than basis subspace! Found spaceDim = {spaceDim} < subSpaceDim = {subSpaceDim}."
        );

      _Basis      = Matrix.Eye(spaceDim);
      SubSpaceDim = subSpaceDim;
    }

    /// <summary>
    /// Constructs an orthonormal basis from the specified vectors.
    /// </summary>
    /// <param name="spaceDim">Ambient space dimension.</param>
    /// <param name="Vs">Vectors whose span defines the subspace.</param>
    public LinearBasis(int spaceDim, IEnumerable<Vector> Vs) {
      MatrixMutable basis       = MatrixMutable.Eye(spaceDim);
      int           subSpaceDim = 0;
      Matrix?       projMatrix  = null;

      AddVectorsInPlace(basis, ref subSpaceDim, ref projMatrix, Vs);
      _Basis      = new Matrix(basis, false);
      SubSpaceDim = subSpaceDim;

#if DEBUG
      CheckCorrectness();
#endif
    }

    /// <summary>
    /// Constructs an orthonormal basis from the specified vectors.
    /// </summary>
    /// <param name="Vs">Vectors whose span defines the subspace.</param>
    public LinearBasis(params IEnumerable<Vector> Vs) : this(Vs.First().SpaceDim, Vs) { }

    /// <summary>
    /// Constructs a linear basis from another basis.
    /// </summary>
    /// <param name="lb">The linear basis to copy.</param>
    /// <param name="needCopy">
    /// If <c>true</c>, creates an independent copy.
    /// If <c>false</c>, reuses the source storage only for immutable <see cref="LinearBasis"/> sources.
    /// Zero-copy wrapping of <see cref="LinearBasisMutable"/> is not allowed here.
    /// </param>
    public LinearBasis(LinearBasis lb, bool needCopy) {
      if (!needCopy && lb is LinearBasisMutable) {
        throw new ArgumentException("Found LinearBasisMutable in LinearBasis copy constructor!");
      }
      _Basis      = new Matrix(lb._Basis, needCopy);
      SubSpaceDim = lb.SubSpaceDim;
#if DEBUG
      CheckCorrectness();
#endif
    }

    /// <summary>
    /// Constructs the orthonormal basis of the sum of two subspaces.
    /// </summary>
    /// <param name="lb1">The first basis.</param>
    /// <param name="lb2">The second basis.</param>
    public LinearBasis(LinearBasis lb1, LinearBasis lb2) {
      Debug.Assert
        (
         lb1.SpaceDim == lb2.SpaceDim
       , $"LinearBasis.Ctor: The dimensions of the basis should be the same. Found dim(lb1) = {lb1.SpaceDim}, dim(lb2) = {lb2.SpaceDim}"
        );

      if (lb1.Empty && lb2.Empty) {
        _Basis = Matrix.Eye(lb1.SpaceDim);
      }
      else {
        MatrixMutable basis;
        int           subSpaceDim;
        Matrix?       projMatrix = null;
        if (lb1.SubSpaceDim > lb2.SubSpaceDim) {
          basis       = new MatrixMutable(lb1._Basis, true);
          subSpaceDim = lb1.SubSpaceDim;
          if (subSpaceDim != lb1.SpaceDim) {
            AddVectorsInPlace(basis, ref subSpaceDim, ref projMatrix, lb2);
          }
        }
        else {
          basis       = new MatrixMutable(lb2._Basis, true);
          subSpaceDim = lb2.SubSpaceDim;
          if (subSpaceDim != lb2.SpaceDim) {
            AddVectorsInPlace(basis, ref subSpaceDim, ref projMatrix, lb1);
          }
        }
        _Basis      = new Matrix(basis, false);
        SubSpaceDim = subSpaceDim;
      }


#if DEBUG
      CheckCorrectness();
#endif
    }

    /// <summary>
    /// Initializes a basis directly from already prepared immutable storage.
    /// </summary>
    /// <param name="basis">Immutable orthonormal storage whose rows contain the basis and its orthogonal complement.</param>
    /// <param name="subSpaceDim">Number of active basis rows.</param>
    protected LinearBasis(Matrix basis, int subSpaceDim) {
      _Basis      = basis;
      SubSpaceDim = subSpaceDim;
    }
#endregion

#region Factories
    /// <summary>
    /// Generates a random full-dimensional orthonormal basis.
    /// </summary>
    /// <param name="spaceDim">Ambient space dimension.</param>
    /// <param name="random">Random generator. If <c>null</c>, the default generator is used.</param>
    /// <returns>A random full-dimensional orthonormal basis.</returns>
    public static LinearBasis GenLinearBasis(int spaceDim, GRandomLC? random = null) => GenLinearBasis(spaceDim, spaceDim, random);

    /// <summary>
    /// Generates a random orthonormal basis of the specified subspace dimension.
    /// </summary>
    /// <param name="spaceDim">Ambient space dimension.</param>
    /// <param name="subSpaceDim">Subspace dimension.</param>
    /// <param name="random">Random generator. If <c>null</c>, the default generator is used.</param>
    /// <returns>A random orthonormal basis.</returns>
    public static LinearBasis GenLinearBasis(int spaceDim, int subSpaceDim, GRandomLC? random = null) {
      MatrixMutable basis      = MatrixMutable.Eye(spaceDim);
      int           subDim     = 0;
      Matrix?       projMatrix = null;
      if (subSpaceDim == 0) { return new LinearBasis(new Matrix(basis, false), 0); }
      do {
        AddVectorInPlace(basis, ref subDim, ref projMatrix, Vector.GenVector(spaceDim, random));
      } while (subDim != subSpaceDim);

      return new LinearBasis(new Matrix(basis, false), subDim);
    }
#endregion

#region Overrides
    public override int GetHashCode() => throw new InvalidOperationException();

    /// <summary>
    /// Returns a string representation of the basis as a list of basis vectors.
    /// </summary>
    /// <returns>A newline-separated list of basis vectors.</returns>
    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      foreach (Vector bvec in this) {
        sb.Append(bvec);
        sb.Append('\n');
      }

      return sb.ToString();
    }

    /// <summary>
    /// Compares the current linear basis with another one to determine their relative order.
    /// The comparison is based on a canonical representation (Reduced Row Echelon Form) of the subspaces.
    /// </summary>
    /// <remarks>
    /// The comparison proceeds in the following order:
    /// 1. By the dimension of the ambient space (<see cref="SpaceDim"/>).
    /// 2. By the dimension of the subspace (<see cref="SubSpaceDim"/>).
    /// 3. Lexicographically by the elements of their canonical RREF matrices.
    /// </remarks>
    /// <param name="other">The linear basis to compare with this instance.</param>
    /// <returns>
    /// An integer that indicates the relative order of the objects being compared.
    /// <list type="bullet">
    /// <item><description>Less than zero: This instance precedes <paramref name="other"/> in the sort order.</description></item>
    /// <item><description>Zero: This instance occurs in the same position in the sort order as <paramref name="other"/> (they represent the same subspace).</description></item>
    /// <item><description>Greater than zero: This instance follows <paramref name="other"/> in the sort order.</description></item>
    /// </list>
    /// </returns>
    public int CompareTo(LinearBasis? other) {
      if (other is null) { return 1; } // null < this (always)

      int spaceDimCompare = this.SpaceDim.CompareTo(other.SpaceDim);
      if (spaceDimCompare != 0) { return spaceDimCompare; }

      int subSpaceDimCompare = this.SubSpaceDim.CompareTo(other.SubSpaceDim);
      if (subSpaceDimCompare != 0) { return subSpaceDimCompare; }

      return this.Basis.ToRREF().CompareTo(other.Basis.ToRREF());
    }

    /// <summary>
    /// Determines whether another object represents the same linear subspace.
    /// </summary>
    /// <param name="obj">Object to compare with this basis.</param>
    /// <returns><c>true</c> if the other object is a basis of the same subspace; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) {
      if (obj == null) { return false; }
      if (ReferenceEquals(this, obj)) { return true; }

      if (obj is LinearBasis other) {
        // return SpanSameSpace(other);

        return CompareTo(other) == 0;
      }

      return false;
    }
#endregion

    /// <summary>
    /// Returns an enumerator that iterates through the linear basis as an IEnumerable.
    /// </summary>
    public IEnumerator GetEnumerator() { return (this as IEnumerable<Vector>).GetEnumerator(); }

    /// <summary>
    /// Returns a generic enumerator that iterates through the vectors in the linear basis.
    /// </summary>
    IEnumerator<Vector> IEnumerable<Vector>.GetEnumerator() {
      for (int i = 0; i < SubSpaceDim; i++) {
        yield return this[i];
      }
    }

    /// <summary>
    /// Verifies the internal orthonormal representation of the basis.
    /// </summary>
    public void CheckCorrectness() {
      if (!this.Empty) {
        if (this.SubSpaceDim > this[0].SpaceDim) {
          throw new ArgumentException
            (
             "LinearBasis.CheckCorrectness: Number of the vectors in the linear basis must be less or equal than dimension of the it's vectors."
            );
        }

        bool   res = true;
        Matrix eye = Matrix.Eye(SpaceDim);
        res &= (_Basis.Transpose() * _Basis).Equals(eye);
        res &= (_Basis * _Basis.Transpose()).Equals(eye);
        if (!res) {
          throw new ArgumentException
            ("LinearBasis.CheckCorrectness: All pairwise different vectors must be orthogonal and have a lenght of one!");
        }
      }
    }

  }
}

