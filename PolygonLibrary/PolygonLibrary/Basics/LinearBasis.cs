using System.Collections;

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
    public int SpaceDim { get; }

    /// <summary>
    /// Number of vectors in the basis.
    /// </summary>
    public int SubSpaceDim => Empty ? 0 : Basis.Cols;

    /// <summary>
    /// True if the basis contains d-linearly independent vectors in d-dimensional space.
    /// Indicates that the basis spans the entire space.
    /// </summary>
    public bool IsFullDim {
      get
        {
          if (Empty) {
            return false;
          }

          return SpaceDim == SubSpaceDim;
        }
    }

    /// <summary>
    /// True if there are no vectors in the basis.
    /// Checks whether the basis is empty.
    /// </summary>
    public bool Empty => _Basis is null;

    /// <summary>
    /// Indexer to access the basis vectors by index.
    /// Provides access to individual vectors in the basis.
    /// </summary>
    /// <param name="ind">Index to be accessed</param>
    public Vector this[int ind] {
      get
        {
          Debug.Assert(ind >= 0 && ind < SubSpaceDim);

          Debug.Assert(Basis is not null, "LinearBasis.this[]: Basis is null. Can't do this task.");

          return Basis.TakeVector(ind);
        }
    }

    /// <summary>
    /// The matrix that stores the basis vectors in its columns.
    /// </summary>
    public Matrix Basis {
      get
        {
          if (_Basis is null) {
            throw new ArgumentException("Accessing empty basis!");
          }
          else {
            return _Basis;
          }
        }
    }

    /// <summary>
    /// The projection matrix calculated as the product of the basis matrix and its transpose.
    /// </summary>
    public Matrix ProjMatrix => _projMatrix ??= Basis.MultiplyBySelfTranspose();

    private Matrix? _Basis      = null;
    private Matrix? _projMatrix = null;
#endregion

#region Functions
    /// <summary>
    /// Finds the orthogonal complement of the given linear basis.
    /// </summary>
    /// <returns>The orthogonal complement of the basis. Returns null if the basis is full-dimensional.</returns>
    public LinearBasis? FindOrthogonalComplement() {
      if (IsFullDim) { return null; }
      if (Empty) { return new LinearBasis(SpaceDim, SpaceDim); }

      (Matrix Q, _) = QRDecomposition.ByReflection(Basis);

      Matrix ogBasis = Q.TakeSubMatrix(null, Enumerable.Range(SubSpaceDim, SpaceDim - SubSpaceDim).ToArray());

      return new LinearBasis(ogBasis);
    }

    /// <summary>
    /// Finds an orthonormal vector that is orthogonal to the given basis, i.e., some vector from orthogonal complement space.
    /// </summary>
    /// <returns>An orthonormal vector orthogonal to the basis. Returns the zero vector if the basis is full-dimensional.</returns>
    public Vector FindOrthonormalVector() {
      LinearBasis? oc = FindOrthogonalComplement();

      if (oc is null) {
        throw new ArgumentException
          (
           $"LinearBasis.FindOrthonormalVector: Can not find an orthonormal vector to the linear basis. Found SpaceDim == SubSpaceDim"
          );
      }

      return oc[0];
    }

    /// <summary>
    /// Projects a given vector onto the subspace of the linear basis expressed in d-space coordinates.
    /// </summary>
    /// <param name="v">The vector to be projected.</param>
    /// <returns>The projected vector in the subspace.</returns>
    public Vector GetProjectionToSubSpace(Vector v) => ProjMatrix * v;

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

      if (IsFullDim) { return true; }

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
    public Vector Orthonormalize(Vector v) {
      if (IsFullDim) { return Vector.Zero(v.SpaceDim); }
      if (Empty) { return v.NormalizeZero(); }

      Vector toAdd;
      (Matrix Q, Matrix R) = QRDecomposition.ByReflection(Matrix.hcat(Basis, v));
      if (Tools.EQ(R[SubSpaceDim, SubSpaceDim])) {
        toAdd = Vector.Zero(v.SpaceDim);
      }
      else {
        toAdd = Q.TakeVector(SubSpaceDim);
      }

      if (Tools.LT(v * toAdd)) {
        toAdd = -toAdd;
      }


      return toAdd;
    }

    /// <summary>
    /// Adds the given vector to the basis. If the vector is zero or linearly dependent on the basis, it is not included.
    /// </summary>
    /// <param name="v">The vector to be potentially added.</param>
    /// <param name="orthogonalize">If false, the vector is added without orthogonalization.</param>
    /// <returns>True if the vector is added to the basis; otherwise, false.</returns>
    public bool AddVector(Vector v, bool orthogonalize = true) {
      if (IsFullDim) { return false; }
      if (v.IsZero) { return false; }

      bool isAdded;
      if (Empty) {
        Vector init = orthogonalize ? v.Normalize() : v;
        _Basis  = new Matrix(init);
        isAdded = true;
      }
      else {
        Debug.Assert
          (SpaceDim == v.SpaceDim, "The dimension the vector to be added must be equal to the dimension of basis vectors!");

        if (!orthogonalize) {
          _Basis  = Matrix.hcat(Basis, v);
          isAdded = true;
        }
        else {
          Vector toAdd = Orthonormalize(v);
          if (toAdd.IsZero) {
            isAdded = false;
          }
          else {
            _Basis  = Matrix.hcat(Basis, toAdd);
            isAdded = true;
          }
        }
      }

      if (isAdded) { _projMatrix = null; }

#if DEBUG
      CheckCorrectness(this);
#endif


      return isAdded;
    }

    /// <summary>
    /// Adds a collection of vectors to the basis.
    /// </summary>
    /// <param name="Vs">The collection of vectors to add.</param>
    /// <param name="orthogonalize">If false, the vectors are added without orthogonalization.</param>
    public void AddVectors(IEnumerable<Vector> Vs, bool orthogonalize = true) {
      foreach (Vector v in Vs) {
        AddVector(v, orthogonalize);
        if (IsFullDim) { break; }
      }
    }

    /// <summary>
    /// Projects a given vector onto the linear basis in its coordinates.
    /// </summary>
    /// <param name="v">The vector to project.</param>
    /// <returns>The projected vector.</returns>
    public Vector ProjectVectorToSubSpace(Vector v) => Basis.MultiplyTransposedByVector(v);

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
#endregion

#region Constructors
    /// <summary>
    /// Constructs a linear basis with a single vector.
    /// </summary>
    /// <param name="v">The vector to form the basis.</param>
    public LinearBasis(Vector v) {
      Debug.Assert(!v.IsZero, "LinearBasis: Can't construct the linear basis based ob zero-vector!");

      SpaceDim = v.SpaceDim;
      _Basis   = new Matrix(v.Normalize());

#if DEBUG
      CheckCorrectness(this);
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

      _Basis   = subSpaceDim == 0 ? null : Matrix.Eye(spaceDim, subSpaceDim);
      SpaceDim = spaceDim;
    }

    /// <summary>
    /// Constructs a linear basis using a set of vectors.
    /// </summary>
    /// <param name="spaceDim">The dimension of the space.</param>
    /// <param name="Vs">The vectors to form the basis.</param>
    /// <param name="orthogonalize">Indicates whether the vectors should be orthogonalized.</param>
    public LinearBasis(int spaceDim, IEnumerable<Vector> Vs, bool orthogonalize = true) {
      SpaceDim = spaceDim;
      _Basis   = null;

      AddVectors(Vs, orthogonalize);

#if DEBUG
      CheckCorrectness(this);
#endif
    }

    /// <summary>
    /// Constructs a linear basis from a set of vectors.
    /// </summary>
    /// <param name="Vs">The vectors to form the basis.</param>
    /// <param name="orthogonalize">Indicates whether the vectors should be orthogonalized.</param>
    public LinearBasis(IEnumerable<Vector> Vs, bool orthogonalize = true) : this(Vs.First().SpaceDim, Vs, orthogonalize) { }

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

      SpaceDim = lb1.SpaceDim;

      if (lb1.Empty && lb2.Empty) {
        _Basis = null;
      }
      else {
        if (lb1.SubSpaceDim > lb2.SubSpaceDim) {
          _Basis = lb1.Basis;
          if (!IsFullDim) {
            foreach (Vector bvec2 in lb2) {
              AddVector(bvec2);
              // TODO: Как следует !*ПОДУМАТЬ*! об оптимальности - в предыдущей строке многократно пересоздается объект матрицы

              if (IsFullDim) {
                break;
              }
            }
          }
        }
        else {
          _Basis = lb2.Basis;
          if (!IsFullDim) {
            foreach (Vector bvec1 in lb1) {
              AddVector(bvec1);
              // TODO: Как следует !*ПОДУМАТЬ*! об оптимальности - в предыдущей строке многократно пересоздается объект матрицы

              if (IsFullDim) {
                break;
              }
            }
          }
        }
      }


#if DEBUG
      CheckCorrectness(this);
#endif
    }

    /// <summary>
    /// Copy constructor for the linear basis.
    /// </summary>
    /// <param name="lb">The linear basis to copy.</param>
    public LinearBasis(LinearBasis lb) {
      _Basis   = lb.Empty ? null : new Matrix(lb.Basis);
      SpaceDim = lb.SpaceDim;

#if DEBUG
      CheckCorrectness(this);
#endif
    }

    // Хорошая матрица! m x n, m >= n; rang = n
    private LinearBasis(Matrix m) {
      _Basis   = m;
      SpaceDim = m.Rows;
    }
#endregion

#region Fabrics
    /// <summary>
    /// Generates a full-dimensional linear basis for the specified dimension.
    /// </summary>
    /// <param name="spaceDim">The dimension of the space and the basis.</param>
    /// <param name="random">The random to be used. If null, the Random be used.</param>
    /// <returns>A linear basis with the given dimension.</returns>
    public static LinearBasis GenLinearBasis(int spaceDim, GRandomLC? random = null)
      => GenLinearBasis(spaceDim, spaceDim, random);

    /// <summary>
    /// Generates a k-dimensional linear basis in the specified dimension.
    /// </summary>
    /// <param name="spaceDim">The dimension of the space.</param>
    /// <param name="subSpaceDim">The dimension of the basis.</param>
    /// <param name="random">The random to be used. If null, the Random be used.</param>
    /// <returns>A linear basis with the given dimension.</returns>
    public static LinearBasis GenLinearBasis(int spaceDim, int subSpaceDim, GRandomLC? random = null) {
      LinearBasis lb = new LinearBasis(spaceDim, 0);
      do {
        lb.AddVector(Vector.GenVector(spaceDim, random));
        // TODO: Опять *ПОДУМАТЬ* об оптимальности
      } while (lb.SubSpaceDim != subSpaceDim);

#if DEBUG
      CheckCorrectness(lb);
#endif
      return lb;
    }
#endregion


#region Overrides
    // public override int GetHashCode() => HashCode.Combine(SubSpaceDim, SpaceDim);
    public override int GetHashCode() => throw new InvalidOperationException();

    /// <summary>
    /// Two linear basics are equal if they span the same space.
    /// </summary>
    /// <param name="obj">Object to compare with this linear basis.</param>
    /// <returns><c>True</c> if they are equal, else <c>False</c>.</returns>
    public override bool Equals(object? obj) {
      if (obj == null || this.GetType() != obj.GetType()) {
        return false;
      }

      LinearBasis other = (LinearBasis)obj;

      // Сравниваем размерности пространств, задаваемых базисами
      if (this.SubSpaceDim != other.SubSpaceDim) { return false; }

      // Если хотя бы один вектор не лежит в подпространстве нашего линейного базиса, то они не равны.
      foreach (Vector otherbv in other) {
        if (!Contains(otherbv)) { return false; }
      }

      return true;
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
      List<Vector> Vs = new List<Vector>(SubSpaceDim);
      for (int i = 0; i < SubSpaceDim; i++) {
        Vs.Add(this[i]);
      }

      return Vs.GetEnumerator();
    }

    /// <summary>
    /// Method to check then the linear basis is correct
    /// </summary>
    /// <param name="linearBasis">Basis to be checked</param>
    public static void CheckCorrectness(LinearBasis linearBasis) {
      if (!linearBasis.Empty) {
        if (linearBasis.SubSpaceDim > linearBasis[0].SpaceDim) {
          throw new ArgumentException
            (
             "LinearBasis.CheckCorrectness: Number of the vectors in the linear basis must be less or equal than dimension of the it's vectors."
            );
        }


        foreach (Vector bvec in linearBasis) {
          if (Tools.NE(bvec.Length, Tools.One)) {
            throw new ArgumentException("LinearBasis.CheckCorrectness: All vectors in the basis must have a unit length.");
          }
        }

        for (int i = 0; i < linearBasis.SubSpaceDim - 1; i++) {
          for (int k = i + 1; k < linearBasis.SubSpaceDim; k++) {
            if (Tools.NE(linearBasis[i] * linearBasis[k])) {
              throw new ArgumentException("LinearBasis.CheckCorrectness: All pairwise different vectors must be orthogonal.");
            }
          }
        }
      }
    }

  }

}
