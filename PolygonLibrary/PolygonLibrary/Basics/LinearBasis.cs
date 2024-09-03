using System.Collections;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Orthonormal basis
  /// </summary>
  public class LinearBasis : IEnumerable<Vector> {

#region Data and Properties
    /// <summary>
    /// The dimension of the basis vectors
    /// </summary>
    public int SpaceDim { get; }

    /// <summary>
    /// Number of vectors in the basis
    /// </summary>
    public int SubSpaceDim => IsEmpty ? 0 : Basis!.Cols;

    /// <summary>
    /// Property True if the basis contains d-linear independent vectors in dD
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
    /// True if there are no vectors in the basis
    /// </summary>
    public bool IsEmpty => Basis is null;

    /// <summary>
    /// Index access
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
    /// Gets the current basis of the linear space as a list of vectors.
    /// </summary>
    public Matrix? Basis { get; private set; }
#endregion

#region Functions
    public static LinearBasis? FindOrthogonalComplement(LinearBasis linBasis) {
      if (linBasis.IsFullDim) { return null; }
      if (linBasis.IsEmpty) { return new LinearBasis(linBasis.SpaceDim, linBasis.SpaceDim); }

      (Matrix Q, _) = QRDecomposition.ByReflection(linBasis.Basis!);

      Matrix ogBasis = Q.TakeSubMatrix
        (null, Enumerable.Range(linBasis.SubSpaceDim, linBasis.SpaceDim - linBasis.SubSpaceDim).ToArray());

      return new LinearBasis(ogBasis);
    }

    //todo tests!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public static Vector FindOrthonormalVector(LinearBasis linBasis) {
      LinearBasis? oc = FindOrthogonalComplement(linBasis);

      return oc is null ? Vector.Zero(linBasis.SpaceDim) : oc[0];
    }

    public bool IsContains(Vector v) {
      Debug.Assert
        (
         v.Dim == SpaceDim
       , "LinearBasis.IsVectorBelongsToLinBasis: The dimension of the vector must be equal to dimensions of basis vectors!"
        );
      if (IsFullDim) { return true; }

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

      return new Vector(proj).Equals(v);
    }

    /// <summary>
    /// Orthonormalizes the given vector against the given basis.
    /// </summary>
    /// <param name="v">The input vector to orthonormalize.</param>
    /// <param name="linBasis">The basis to orthonormalize against.</param>
    /// <returns>The resulting orthonormalized vector. If the basis is empty, returns normalized vector.</returns>
    public static Vector OrthonormalizeAgainstBasis(Vector v, LinearBasis linBasis) {
      if (linBasis.IsFullDim) { return Vector.Zero(v.Dim); }
      if (linBasis.IsEmpty) { return v.NormalizeZero(); }

      Vector toAdd;
      (Matrix Q, Matrix R) = QRDecomposition.ByReflection(Matrix.hcat(linBasis.Basis, v));
      if (Tools.EQ(R[linBasis.SubSpaceDim, linBasis.SubSpaceDim])) {
        toAdd = Vector.Zero(v.Dim);
      }
      else {
        toAdd = Q.TakeVector(linBasis.SubSpaceDim);
      }

      if (Tools.LT(v * toAdd)) {
        toAdd = -toAdd;
      }


      return toAdd;
    }

    /// <summary>
    /// Add the given vector to the basis. If it is zero vector or linear dependent with the basis then it don't includes in.
    /// </summary>
    /// <param name="v">Vector perhaps to be added.</param>
    /// <param name="orthogonalize">If the vector does not need to be orthogonalized, it should be set to false</param>
    /// <returns>True if vector added to the basis, false otherwise</returns>
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
          Vector toAdd = OrthonormalizeAgainstBasis(v, this);
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

    //todo
    public void AddVectors(IEnumerable<Vector> Vs, bool orthogonalize = true) {
      foreach (Vector v in Vs) {
        AddVector(v, orthogonalize);
        if (IsFullDim) { break; }
      }
    }


    /// <summary>
    /// Projects a given point onto the linear basis.
    /// </summary>
    /// <param name="v">The vector to project.</param>
    /// <returns>The projected point.</returns>
    public Vector ProjectVectorToSubSpace(Vector v) {
      Debug.Assert
        (
         SpaceDim == v.Dim
       , "LinearBasis.ProjectVectorToSubSpace: The dimension of the basis vectors should be equal to the dimension of the given vector."
        );

      TNum[] np = new TNum[SubSpaceDim];
      for (int i = 0; i < SubSpaceDim; i++) {
        np[i] = this[i] * v;
      }

      return new Vector(np);
    }

    /// <summary>
    /// Projects a given set of vectors onto the linear basis.
    /// </summary>
    /// <param name="Swarm">The set of vectors to project.</param>
    /// <returns>The projected vectors.</returns>
    public IEnumerable<Vector> ProjectVectorsToSubSpace(List<Vector> Swarm) {
      foreach (Vector v in Swarm) {
        yield return ProjectVectorToSubSpace(v);
      }
    }
#endregion

#region Constructors
    public LinearBasis(Vector v) {
      Debug.Assert(!v.IsZero, "LinearBasis: Can't construct the linear basis based ob zero-vector!");
      SpaceDim = v.Dim;
      Basis    = new Matrix(v.Normalize());

#if DEBUG
      CheckCorrectness(this);
#endif
    }

    // /// <summary>
    // /// Construct the new linear basis of full dimension with d-dim zero origin and d-orth.
    // /// </summary>
    // /// <param name="vecDim">The dimension of the space of basis.</param>
    // public LinearBasis(int vecDim) {
    //   Basis       = Matrix.Eye(vecDim);
    //   this.SpaceDim = vecDim;
    // }

    /// <summary>
    /// Construct the new linear basis of given space dimension and the given vectors dim, with dim-zero origin and dim-orth
    /// from 0 to the space dim.
    /// </summary>
    /// <param name="spaceDim">The dimension of the vectors.</param>
    /// <param name="subSpaceDim">The dimension of the space of basis.</param>
    public LinearBasis(int spaceDim, int subSpaceDim) {
      Debug.Assert
        (
         spaceDim >= subSpaceDim
       , $"LinearBasis: The dimension of the vectors in basis must be greater or equal than basis subspace! Found spaceDim = {spaceDim} < subSpaceDim = {subSpaceDim}."
        );

      Basis    = subSpaceDim == 0 ? null : Matrix.Eye(spaceDim, subSpaceDim);
      SpaceDim = spaceDim;
    }

    //todo
    public LinearBasis(int spaceDim, IEnumerable<Vector> Vs, bool orthogonalize = true) {
      SpaceDim = spaceDim;
      Basis    = null;

      AddVectors(Vs, orthogonalize);

#if DEBUG
      CheckCorrectness(this);
#endif
    }

    /// <summary>
    /// Constructs the linear basis based on given vectors.
    /// </summary>
    /// <param name="Vs">Vectors on which basis should be constructed.</param>
    /// <param name="orthogonalize">If the vectors do not need to be orthogonalized, it should be set to false</param>
    public LinearBasis(IEnumerable<Vector> Vs, bool orthogonalize = true) {
      Debug.Assert(Vs.Count() != 0, "LinearBasis: The enumerable of points is empty! It requires at least one point!");
      SpaceDim = Vs.First().Dim;
      Basis    = null;

      AddVectors(Vs, orthogonalize);

#if DEBUG
      CheckCorrectness(this);
#endif
    }

    //todo TESTS!!!
    public LinearBasis(LinearBasis lb1, LinearBasis lb2) {
      Basis    = lb1.Basis;
      SpaceDim = lb1.SpaceDim;
      foreach (Vector bvec2 in lb2) {
        AddVector(bvec2);
        if (IsFullDim) {
          break;
        }
      }

#if DEBUG
      CheckCorrectness(this);
#endif
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="linearBasis">The linear basis to be copied.</param>
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
    /// Generates a linear basis of given dimension.
    /// </summary>
    /// <param name="dim">The dimension of the basis.</param>
    /// <returns>A full dimensional basis.</returns>
    public static LinearBasis GenLinearBasis(int dim) {
      LinearBasis lb = new LinearBasis(dim, 0);
      do {
        lb.AddVector(Vector.GenVector(dim));
      } while (!lb.IsFullDim);

#if DEBUG
      CheckCorrectness(lb);
#endif
      return lb;
    }
#endregion


#region Overrides
    public override int GetHashCode() => HashCode.Combine(SubSpaceDim, SpaceDim);

    /// <summary>
    /// Two linear basics are equal, if they span the same space.
    /// </summary>
    /// <param name="obj">Object to compare with this linear basis.</param>
    /// <returns><c>True</c> if they are equal, else <c>False</c>.</returns>
    public override bool Equals(object? obj) {
      if (obj == null || this.GetType() != obj.GetType()) {
        return false;
      }

      LinearBasis other = (LinearBasis)obj;

      // Если хотя бы один вектор не лежит в подпространстве нашего линейного базиса, то они не равны.
      foreach (Vector otherbv in other) {
        if (!IsContains(otherbv)) { return false; }
      }

      return true;
    }
#endregion

    public IEnumerator GetEnumerator() { return (this as IEnumerable<Vector>).GetEnumerator(); }

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
