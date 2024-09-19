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
    public int SubSpaceDim => IsEmpty ? 0 : Basis!.Cols;

    /// <summary>
    /// True if the basis contains d-linearly independent vectors in d-dimensional space.
    /// Indicates that the basis spans the entire space.
    /// </summary>
    public bool IsFullDim {
      get
        {
          if (IsEmpty) {
            return false;
          }

          return SpaceDim == SubSpaceDim;
        }
    }

    /// <summary>
    /// True if there are no vectors in the basis.
    /// Checks whether the basis is empty.
    /// </summary>
    public bool IsEmpty => Basis is null;

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
    /// The matrix representation of the basis vectors.
    /// </summary>
    public Matrix? Basis { get; private set; }
#endregion

#region Functions

    /// <summary>
    /// Finds the orthogonal complement of the given linear basis.
    /// </summary>
    /// <returns>The orthogonal complement of the basis. Returns null if the basis is full-dimensional.</returns>
    public LinearBasis? FindOrthogonalComplement() {
      if (IsFullDim) { return null; }
      if (IsEmpty) { return new LinearBasis(SpaceDim, SpaceDim); }

      (Matrix Q, _) = QRDecomposition.ByReflection(Basis!);

      Matrix ogBasis = Q.TakeSubMatrix
        (null, Enumerable.Range(SubSpaceDim, SpaceDim - SubSpaceDim).ToArray());

      return new LinearBasis(ogBasis);
    }

    /// <summary>
    /// Finds an orthonormal vector that is orthogonal to the given basis, i.e., some vector from orthogonal complement space.
    /// </summary>
    /// <returns>An orthonormal vector orthogonal to the basis. Returns the zero vector if the basis is full-dimensional.</returns>
    public Vector FindOrthonormalVector() {
      LinearBasis? oc = FindOrthogonalComplement();

      return oc is null ? Vector.Zero(SpaceDim) : oc[0];
    }

    /// <summary>
    /// Checks if the given vector belongs to the linear basis.
    /// </summary>
    /// <param name="v">The vector to check.</param>
    /// <returns>True if the vector is contained in the basis; otherwise, false.</returns>
    public bool Contains(Vector v) {
      Debug.Assert
        (
         v.Dim == SpaceDim
       , "LinearBasis.IsVectorBelongsToLinBasis: The dimension of the vector must be equal to dimensions of basis vectors!"
        );

      if (IsFullDim) { return true; }

      // TODO: А точно проверка на принадлежность подпространству - это проверка совпадение спроектированного вектора и исходного? Проще нет пути?
      // Можно Гауссом Bx = v, где B - базис, v проверяемый вектор.
      // TODO: А не умножение ли это матрицы на вектор?! И вообще - это проектирование вектора в пространство базиса! Оно написано ниже!
      // Да, но там он выражен в координатах подпространства. А мне нужен в "большом" пространстве.

      //  proj += (v * bvec) * bvec;
      TNum[] proj = new TNum[SpaceDim];
      for (int j = 0; j < SubSpaceDim; j++) {
        TNum dotProduct = Tools.Zero;
        for (int i = 0; i < SpaceDim; i++) { // Вычисляем скалярное произведение v * bvec
          dotProduct += v[i] * Basis![i, j];
        }
        for (int i = 0; i < SpaceDim; i++) { // Добавляем проекцию на bvec
          proj[i] += dotProduct * Basis![i, j];
        }
      }

      return new Vector(proj, false).Equals(v);
    }

    /// <summary>
    /// Orthonormalizes the given vector against the given basis.
    /// </summary>
    /// <param name="v">The input vector to orthonormalize.</param>
    /// <returns>The resulting orthonormalized vector. If the basis is empty, returns a normalized vector.</returns>
    public Vector Orthonormalize(Vector v) {
      if (IsFullDim) { return Vector.Zero(v.Dim); }
      if (IsEmpty) { return v.NormalizeZero(); }

      Vector toAdd;
      (Matrix Q, Matrix R) = QRDecomposition.ByReflection(Matrix.hcat(Basis, v));
      if (Tools.EQ(R[SubSpaceDim, SubSpaceDim])) {
        toAdd = Vector.Zero(v.Dim);
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
      if (IsEmpty) {
        Vector init = orthogonalize ? v.Normalize() : v;
        Basis   = new Matrix(init);
        isAdded = true;
      }
      else {
        Debug.Assert(SpaceDim == v.Dim, "The dimension the vector to be added must be equal to the dimension of basis vectors!");

        if (!orthogonalize) {
          Basis   = Matrix.hcat(Basis, v);
          isAdded = true;
        }
        else {
          Vector toAdd = Orthonormalize(v);
          if (toAdd.IsZero) {
            isAdded = false;
          }
          else {
            Basis   = Matrix.hcat(Basis, toAdd);
            isAdded = true;
          }
        }
      }

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
    /// Projects a given vector onto the linear basis.
    /// </summary>
    /// <param name="v">The vector to project.</param>
    /// <returns>The projected vector.</returns>
    public Vector ProjectVectorToSubSpace(Vector v) {
      Debug.Assert
        (
         SpaceDim == v.Dim
       , "LinearBasis.ProjectVectorToSubSpace: The dimension of the basis vectors should be equal to the dimension of the given vector."
        );

      // TODO: Тоже ведь матричное умножение!  <>  Да, но у меня векторы-столбцы

      TNum[] np = new TNum[SubSpaceDim];
      for (int i = 0; i < SubSpaceDim; i++) {
        np[i] = this[i] * v; // bvec * v
      }

      return new Vector(np, false);
    }

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

      SpaceDim = v.Dim;
      Basis    = new Matrix(v.Normalize());

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

      Basis    = subSpaceDim == 0 ? null : Matrix.Eye(spaceDim, subSpaceDim);
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
      Basis    = null;

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
    public LinearBasis(IEnumerable<Vector> Vs, bool orthogonalize = true) :
      this(Vs.First().Dim, Vs, orthogonalize)
    { }

    /// <summary>
    /// Merges two linear bases into one.
    /// </summary>
    /// <param name="lb1">The first basis to merge.</param>
    /// <param name="lb2">The second basis to merge.</param>
    public LinearBasis(LinearBasis lb1, LinearBasis lb2) {
      Basis    = lb1.Basis;
      SpaceDim = lb1.SpaceDim;
      foreach (Vector bvec2 in lb2) {
        AddVector(bvec2);
        // TODO: Как следует !*ПОДУМАТЬ*! об оптимальности - в предыдущей строке многократно пересоздается объект матрицы

        if (IsFullDim) {
          break;
        }
      }

#if DEBUG
      CheckCorrectness(this);
#endif
    }

    /// <summary>
    /// Copy constructor for the linear basis.
    /// </summary>
    /// <param name="linearBasis">The linear basis to copy.</param>
    public LinearBasis(LinearBasis linearBasis) {
      Basis    = linearBasis.Basis is null ? null : new Matrix(linearBasis.Basis);
      SpaceDim = linearBasis.SpaceDim;

#if DEBUG
      CheckCorrectness(this);
#endif
    }

    // Хорошая матрица! m x n, m >= n; rang = n
    private LinearBasis(Matrix m) {
      Basis    = m;
      SpaceDim = m.Rows;
    }
#endregion

#region Fabrics
    /// <summary>
    /// Generates a full-dimensional linear basis for the specified dimension.
    /// </summary>
    /// <param name="spaceDim">The dimension of the basis.</param>
    /// <param name="random">The random to be used. If null, the Random be used.</param>
    /// <returns>A linear basis with the given dimension.</returns>
    public static LinearBasis GenLinearBasis(int spaceDim, GRandomLC? random = null) {
      LinearBasis lb = new LinearBasis(spaceDim, 0);
      do {
        lb.AddVector(Vector.GenVector(spaceDim, random));
        // TODO: Опять *ПОДУМАТЬ* об оптимальности
      } while (!lb.IsFullDim);

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
      if (!linearBasis.IsEmpty) {
        if (linearBasis.SubSpaceDim > linearBasis[0].Dim) {
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
