using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Orthonormal linear basis in d-dimensional space.
  /// </summary>
  public class LinearBasis : IEnumerable<Vector> {

#region Data and Properties
    /// <summary>
    /// The dimension of the basis vectors.
    /// </summary>
    public int SpaceDim => _Basis.Cols;

    /// <summary>
    /// Number of vectors in the basis.
    /// </summary>
    public int SubSpaceDim = 0;

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
    /// Indexer to access the basis vectors by index.
    /// Provides access to individual vectors in the basis.
    /// </summary>
    /// <param name="ind">Index to be accessed</param>
    public Vector this[int ind] {
      get
        {
          if (ind < 0 || ind > SubSpaceDim) {
            throw new ArgumentException($"LinearBasis.this[]: Index should lie within [0, {SubSpaceDim}]. Found ind = {ind}");
          }

          Debug.Assert(!Empty, "LinearBasis.this[]: Basis is empty. Can't take a vector.");

          return Basis.TakeRowVector(ind);
        }
    }

    /// <summary>
    /// The matrix that stores the basis vectors in its rows.
    /// </summary>
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
    /// The projection matrix calculated as the product of the transposed basis matrix and the matrix itself.
    /// <c>B^T*B</c>
    /// </summary>
    public Matrix ProjMatrix => _projMatrix ??= Basis.Transpose() * Basis; // todo: В одну операцию! MultiplyTransposeBySelf()

    private readonly MatrixMutable _Basis;
    private          Matrix?       _projMatrix = null;
#endregion

#region Functions
    /// <summary>
    /// Finds the orthogonal complement of the given linear basis.
    /// </summary>
    /// <returns>The orthogonal complement of the basis.</returns>
    public LinearBasis OrthogonalComplement()
      => new(MatrixMutable.SwapRowBlocks(_Basis, SpaceDim, SubSpaceDim), SpaceDim - SubSpaceDim);

    /// <summary>
    /// Finds an orthonormal vector that is orthogonal to the given basis, i.e., some vector from orthogonal complement space.
    /// </summary>
    /// <returns>An orthonormal vector orthogonal to the basis. Returns the zero vector if the basis is full-dimensional.</returns>
    public Vector OrthonormalVector() => FullDim ? Vector.Zero(SpaceDim) : _Basis.TakeRowVector(SubSpaceDim);

    /// <summary>
    /// Projects a point onto the subspace with coordinates in the original space.
    /// </summary>
    /// <param name="v">The vector to be projected.</param>
    /// <returns>The projected vector in the subspace.</returns>
    public Vector ProjectVectorToSubSpace_in_OrigSpace(Vector v) => ProjMatrix * v;

    /// <summary>
    /// Checks if the given vector belongs to the linear basis.
    /// </summary>
    /// <param name="v">The vector to check.</param>
    /// <returns><c>True</c> if the vector is contained in the basis; otherwise, <c>false</c>.</returns>
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
    /// Orthonormalizes the given vector against the basis.
    /// </summary>
    /// <param name="v">The input vector to orthonormalize.</param>
    /// <returns>The resulting orthonormalized vector. If the basis is empty, returns a normalized vector.</returns>
    public Vector Orthonormalize(Vector v) { throw new NotImplementedException("todo"); }

    /// <summary>
    /// Adds the given vector to the basis. If the vector is zero or linearly dependent on the basis, it is not included.
    /// </summary>
    /// <param name="v">The vector to be potentially added.</param>
    /// <returns><c>True</c> if the vector is added to the basis; otherwise, false.</returns>
    protected bool AddVector(Vector v) {
      Debug.Assert(v.SpaceDim == SpaceDim, "LQ_FullUpdate: Vector v must have the same dimension as currentQ.");

      if (SubSpaceDim == SpaceDim || v.IsZero) { return false; }

      Vector y = _Basis * v;

      int    orthSize = SpaceDim - SubSpaceDim;
      TNum[] orthData = new TNum[orthSize];
      for (int k = 0; k < orthSize; k++) {
        orthData[k] = y[SubSpaceDim + k];
      }
      Vector orthPart = new Vector(orthData);

      TNum rho = orthPart.Length;
      if (orthPart.IsZero) { return false; }

      TNum[] houseData = orthPart.GetCopyAsArray();
      TNum   sign      = TConv.FromInt(Tools.Sign(orthPart[0]));
      if (Tools.EQ(orthPart[0], Tools.Zero)) {
        sign = Tools.One;
      }
      houseData[0] += sign * rho;
      Vector house = new Vector(houseData);

      if (!house.IsZero) {
        TNum   beta          = Tools.Two / house.Length2;
        TNum[] projectionRow = new TNum[SpaceDim];
        for (int col = 0; col < SpaceDim; col++) {
          TNum dot = Tools.Zero;
          for (int i = 0; i < orthSize; i++) {
            dot += house[i] * _Basis[SubSpaceDim + i, col];
          }
          projectionRow[col] = dot;
        }

        for (int i = 0; i < orthSize; i++) {
          int global_row_index = SubSpaceDim + i;
          for (int col = 0; col < SpaceDim; col++) {
            _Basis[global_row_index, col] -= beta * house[i] * projectionRow[col];
          }
        }
      }


      if (Tools.EQ(sign, Tools.One)) { // новый базисный вектор сонаправлен с v.
        for (int j = 0; j < SpaceDim; j++) {
          _Basis[SubSpaceDim, j] = -_Basis[SubSpaceDim, j];
        }
      }

      SubSpaceDim += 1;

      return true;
    }

    /// <summary>
    /// Adds a collection of vectors to the basis.
    /// </summary>
    /// <param name="Vs">The collection of vectors to add.</param>
    protected void AddVectors(IEnumerable<Vector> Vs) {
      foreach (Vector v in Vs) {
        AddVector(v);
        if (FullDim) { break; }
      }
    }

    /// <summary>
    /// Projects a given vector onto the linear basis in its coordinates. B * v
    /// </summary>
    /// <param name="v">The vector to project.</param>
    /// <returns>The projected vector.</returns>
    public Vector ProjectVectorToSubSpace(Vector v) => Basis * v;

    /// <summary>
    /// Projects a given collection of vectors onto the linear basis.
    /// </summary>
    /// <param name="Swarm">The collection of vectors to project.</param>
    /// <returns>The projected vectors as an enumerable collection.</returns>
    public IEnumerable<Vector> ProjectVectorsToSubSpace(IEnumerable<Vector> Swarm) {
      foreach (Vector v in Swarm) {
        yield return ProjectVectorToSubSpace(v);
      }
    }

    /// <summary>
    /// Maps a vector from the coordinate system of this basis to the original one.
    /// </summary>
    /// <param name="coords">The coordinates in this basis.</param>
    /// <returns>The corresponding vector in the original coordinate system.</returns>
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
    /// Constructs a linear basis with a single vector.
    /// </summary>
    /// <param name="v">The vector to form the basis.</param>
    public LinearBasis(Vector v) {
      if (v.IsZero) {
        throw new ArgumentException("Cannot construct a linear basis from a zero vector.", nameof(v));
      }
      _Basis = MatrixMutable.Eye(v.SpaceDim);
      AddVector(v);

#if DEBUG
      CheckCorrectness();
#endif
    }

    /// <summary>
    /// Constructs a new linear basis with full dimension in a given space.
    /// </summary>
    /// <param name="spaceDim">The dimension of the space.</param>
    public LinearBasis(int spaceDim) : this(spaceDim, spaceDim) { }

    /// <summary>
    /// Constructs a new linear basis with a specified space dimension and subspace dimension.
    /// </summary>
    /// <param name="spaceDim">The dimension of the space.</param>
    /// <param name="subSpaceDim">The dimension of the linear subspace.</param>
    public LinearBasis(int spaceDim, int subSpaceDim) {
      Debug.Assert
        (
         spaceDim >= subSpaceDim
       , $"LinearBasis: The dimension of the vectors in basis must be greater or equal than basis subspace! Found spaceDim = {spaceDim} < subSpaceDim = {subSpaceDim}."
        );

      _Basis      = MatrixMutable.Eye(spaceDim);
      SubSpaceDim = subSpaceDim;
    }

    /// <summary>
    /// Constructs a linear basis using a set of vectors.
    /// </summary>
    /// <param name="spaceDim">The dimension of the space.</param>
    /// <param name="Vs">The vectors to form the basis.</param>
    public LinearBasis(int spaceDim, IEnumerable<Vector> Vs) {
      _Basis      = MatrixMutable.Eye(spaceDim);
      SubSpaceDim = 0;

      AddVectors(Vs);

#if DEBUG
      CheckCorrectness();
#endif
    }

    /// <summary>
    /// Constructs a linear basis from a set of vectors.
    /// </summary>
    /// <param name="Vs">The vectors to form the basis.</param>
    public LinearBasis(params IEnumerable<Vector> Vs) : this(Vs.First().SpaceDim, Vs) { }

    /// <summary>
    /// Copy constructor for the linear basis.
    /// </summary>
    /// <param name="lb">The linear basis to copy.</param>
    /// <param name="needCopy"></param>
    public LinearBasis(LinearBasis lb, bool needCopy) {
      _Basis      = new MatrixMutable(lb._Basis, needCopy);
      SubSpaceDim = lb.SubSpaceDim;
#if DEBUG
      CheckCorrectness();
#endif
    }

    /// <summary>
    /// Merges two linear bases into one.
    /// </summary>
    /// <param name="lb1">The first basis to merge.</param>
    /// <param name="lb2">The second basis to merge.</param>
    public LinearBasis(LinearBasis lb1, LinearBasis lb2) {
      Debug.Assert
        (
         lb1.SpaceDim == lb2.SpaceDim
       , $"LinearBasis.Ctor: The dimensions of the basis should be the same. Found dim(lb1) = {lb1.SpaceDim}, dim(lb2) = {lb2.SpaceDim}"
        );

      if (lb1.Empty && lb2.Empty) {
        _Basis = MatrixMutable.Eye(lb1.SpaceDim);
      }
      else {
        if (lb1.SubSpaceDim > lb2.SubSpaceDim) {
          _Basis      = lb1._Basis;
          SubSpaceDim = lb1.SubSpaceDim;
          if (!FullDim) {
            AddVectors(lb2);
          }
        }
        else {
          _Basis      = lb2._Basis;
          SubSpaceDim = lb2.SubSpaceDim;
          if (!FullDim) {
            AddVectors(lb1);
          }
        }
      }


#if DEBUG
      CheckCorrectness();
#endif
    }

    private LinearBasis(MatrixMutable basis, int subSpaceDim) {
      _Basis      = basis;
      SubSpaceDim = subSpaceDim;
    }
#endregion

#region Factories
    /// <summary>
    /// Generates a full-dimensional linear basis for the specified dimension.
    /// </summary>
    /// <param name="spaceDim">The dimension of the space and the basis.</param>
    /// <param name="random">The random to be used. If null, the Random be used.</param>
    /// <returns>A linear basis with the given dimension.</returns>
    public static LinearBasis GenLinearBasis(int spaceDim, GRandomLC? random = null) => GenLinearBasis(spaceDim, spaceDim, random);

    /// <summary>
    /// Generates a k-dimensional linear basis in the specified dimension.
    /// </summary>
    /// <param name="spaceDim">The dimension of the space.</param>
    /// <param name="subSpaceDim">The dimension of the basis.</param>
    /// <param name="random">The random to be used. If null, the Random be used.</param>
    /// <returns>A linear basis with the given dimension.</returns>
    public static LinearBasis GenLinearBasis(int spaceDim, int subSpaceDim, GRandomLC? random = null) {
      LinearBasis lb = new LinearBasis(spaceDim, 0);
      if (subSpaceDim == 0) { return lb; }
      do {
        lb.AddVector(Vector.GenVector(spaceDim, random));
      } while (lb.SubSpaceDim != subSpaceDim);

      return lb;
    }
#endregion


#region Overrides
    public override int GetHashCode() => throw new InvalidOperationException();

    /// <summary>
    /// Returns a string representation of the basis as a list of basis vectors, each on a separate line.
    /// </summary>
    /// <returns>A <see cref="string"/> containing all basis vectors separated by newline characters.</returns>
    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      foreach (Vector bvec in this) {
        sb.Append(bvec);
        sb.Append('\n');
      }

      return sb.ToString();
    }

    /// <summary>
    /// Two linear basics are equal if they span the same space.
    /// </summary>
    /// <param name="obj">Object to compare with this linear basis.</param>
    /// <returns><c>True</c> if they are equal, else <c>False</c>.</returns>
    public override bool Equals(object? obj) {
      if (obj == null) {
        return false;
      }
      if (ReferenceEquals(this, obj)) {
        return true;
      }

      if (obj is LinearBasis other) {
        return SpanSameSpace(other);
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
    /// Method to check then the linear basis is correct
    /// </summary>
    /// <param name="linearBasis">Basis to be checked</param>
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

  public class LinearBasisMutable : LinearBasis {

    public LinearBasisMutable(Vector              v) : base(v) { }
    public LinearBasisMutable(int                 spaceDim) : base(spaceDim) { }
    public LinearBasisMutable(int                 spaceDim, int                 subSpaceDim) : base(spaceDim, subSpaceDim) { }
    public LinearBasisMutable(int                 spaceDim, IEnumerable<Vector> Vs) : base(spaceDim, Vs) { }
    public LinearBasisMutable(params IEnumerable<Vector> Vs) : base(Vs) { }
    public LinearBasisMutable(LinearBasis         lb1, LinearBasis lb2) : base(lb1, lb2) { }
    public LinearBasisMutable(LinearBasis         lb,  bool        needCopy) : base(lb, needCopy) { }


    public new bool AddVector(Vector               v)  => base.AddVector(v);
    public new void AddVectors(IEnumerable<Vector> vs) => base.AddVectors(vs);

    public new static LinearBasisMutable GenLinearBasis(int spaceDim, int subSpaceDim, GRandomLC? random = null)
      => new LinearBasisMutable(LinearBasis.GenLinearBasis(spaceDim, subSpaceDim, random), needCopy: false);

    public new static LinearBasisMutable GenLinearBasis(int spaceDim, GRandomLC? random = null)
      => new LinearBasisMutable(LinearBasis.GenLinearBasis(spaceDim, random), needCopy: false);

  }

}
