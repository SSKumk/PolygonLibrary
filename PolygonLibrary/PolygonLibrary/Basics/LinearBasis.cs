using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : class, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Orthonormal basis
  /// </summary>
  public class LinearBasis {

#region Data and Properties
    /// <summary>
    /// The dimension of the basis vectors
    /// </summary>
    public int VecDim {
      get
        {
          Debug.Assert(Basis.Count != 0, "Basis must have at least one vector to determine it dimension");

          return Basis[0].Dim;
        }
    }

    /// <summary>
    /// Property True if basis contain d-linear independent vectors in dD
    /// </summary>
    public bool IsFullDim {
      get
        {
          if (Basis.Count != 0) { return VecDim == Basis.Count; }

          return false;
        }
    }

    /// <summary>
    /// Amount of vectors in the basis
    /// </summary>
    public int SpaceDim => Basis.Count;

    /// <summary>
    /// True if there are no vectors in the basis
    /// </summary>
    public bool IsEmpty => SpaceDim == 0;

    /// <summary>
    /// Index access
    /// </summary>
    /// <param name="ind">Index to be accessed</param>
    public Vector this[int ind] {
      get
        {
          Debug.Assert(ind >= 0 && ind < Basis.Count);

          return Basis[ind];
        }
    }

    /// <summary>
    /// Gets the current basis of the linear space as list of vectors.
    /// </summary>
    public List<Vector> Basis { get; }
#endregion

#region Functions
    /// <summary>
    /// Add the given vector to the basis. If it is zero vector or linear dependent with the basis then it don't includes in.
    /// </summary>
    /// <param name="v">Vector to be added</param>
    /// <param name="orthogonalize">If the vector does not need to be orthogonalized, it should be set to false</param>
    /// <returns>True if vector added to the basis, false otherwise</returns>
    public bool AddVectorToBasis(Vector v, bool orthogonalize = true) {
      if (!orthogonalize) { // Если вектор не нужно ортогонализировать, то просто добавляем в базис
        Basis.Add(v);

        return true;
      }

      Vector toAdd = Vector.OrthonormalizeAgainstBasis(v, Basis);
      if (toAdd.IsZero) { return false; } // Если вектор является линейно зависимым, то его не добавляем

      Basis.Add(toAdd); // Иначе добавляем

      return true;
    }

    /// <summary>
    /// The matrix representation of the basis.
    /// </summary>
    /// <returns>Basis by column vectors.</returns>
    public Matrix GetMatrix() {
      TNum[,] m = new TNum[VecDim, SpaceDim];

      for (int i = 0; i < VecDim; i++) {
        for (int j = 0; j < SpaceDim; j++) {
          m[i, j] = Basis[j][i];
        }
      }

      return new Matrix(m);
    }

    /// <summary>
    /// Projects a given point onto the linear basis.
    /// </summary>
    /// <param name="v">The vector to project.</param>
    /// <returns>The projected point.</returns>
    public Vector ProjectVector(Vector v) {
      Debug.Assert
        (
         VecDim == v.Dim
       , "LinearBasis.ProjectVector: The dimension of the basis vectors should be equal to the dimension of the given vector."
        );

      TNum[] np = new TNum[SpaceDim];
      for (int i = 0; i < SpaceDim; i++) {
        np[i] = Basis[i] * v;
      }

      return new Vector(np);
    }

    /// <summary>
    /// Projects a given set of vectors onto the linear basis.
    /// </summary>
    /// <param name="Swarm">The set of vectors to project.</param>
    /// <returns>The projected vectors.</returns>
    public IEnumerable<Vector> ProjectVectors(List<Vector> Swarm) {
      foreach (Vector v in Swarm) {
        yield return ProjectVector(v);
      }
    }
#endregion

#region Constructors
    /// <summary>
    /// Constructs the empty basis.
    /// </summary>
    public LinearBasis() => Basis = new List<Vector>();

    /// <summary>
    /// Construct the new linear basis of full dimension with d-dim zero origin and d-orth.
    /// </summary>
    /// <param name="spaceDim">The dimension of the space of basis.</param>
    public LinearBasis(int spaceDim) {
      Basis = new List<Vector>(spaceDim);

      for (int i = 0; i < spaceDim; i++) {
        AddVectorToBasis(Vector.MakeOrth(spaceDim, i + 1), false);
      }
    }

    /// <summary>
    /// Construct the new linear basis of given space dimension and the given vectors dim, with dim-zero origin and dim-orth
    /// from 0 to the space dim.
    /// </summary>
    /// <param name="basisDim">The dimension of the space of basis.</param>
    /// <param name="vecDim">The dimension of the vectors.</param>
    public LinearBasis(int basisDim, int vecDim) {
      Debug.Assert
        (
         vecDim >= basisDim
       , $"LinearBasis: The dimension of the vectors in basis must be greater or equal than basis subspace! Found vecDim = {vecDim} < basisDim = {basisDim}."
        );
      Basis = new List<Vector>(basisDim);

      for (int i = 0; i < basisDim; i++) {
        AddVectorToBasis(Vector.MakeOrth(vecDim, i + 1), false);
      }
    }

    /// <summary>
    /// Constructs the linear basis based on given vectors.
    /// </summary>
    /// <param name="Vs">Vectors on which basis should be constructed.</param>
    /// <param name="orthogonalize">If the vectors do not need to be orthogonalized, it should be set to false</param>
    public LinearBasis(IEnumerable<Vector> Vs, bool orthogonalize = true) {
      Basis = new List<Vector>();

      foreach (Vector v in Vs) {
        AddVectorToBasis(v, orthogonalize);
        if (IsFullDim) { break; }
      }
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="linearBasis">The linear basis to be copied.</param>
    public LinearBasis(LinearBasis linearBasis) { Basis = new List<Vector>(linearBasis.Basis); }
#endregion

#region Fabrics
    /// <summary>
    /// Generates a linear basis of given dimension.
    /// </summary>
    /// <param name="dim">The dimension of the basis.</param>
    /// <returns>A full dimensional basis.</returns>
    public static LinearBasis GenLinearBasis(int dim) {
      LinearBasis lb = new LinearBasis();
      do {
        lb.AddVectorToBasis(Vector.GenVector(dim));
      } while (!lb.IsFullDim);

      return lb;
    }
#endregion

    /// <summary>
    /// Two linear basics are equal, if it spans same space.
    /// </summary>
    /// <param name="obj">Object to compare with this linear basis.</param>
    /// <returns><c>True</c> if they are equal, else <c>False</c>.</returns>
    public override bool Equals(object? obj) {
      if (obj == null || this.GetType() != obj.GetType()) {
        return false;
      }

      LinearBasis other = (LinearBasis)obj;

      // Если хотя бы один вектор не лежит в подпротранстве нашего линейного базиса, то они не равны.
      foreach (Vector otherbv in other.Basis) {
        Vector check = Vector.OrthonormalizeAgainstBasis(otherbv, this.Basis);
        if (!check.IsZero) {
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Aux method to check then the basis is correct
    /// </summary>
    /// <param name="linearBasis">Basis to be checked</param>
    public static void CheckCorrectness(LinearBasis linearBasis) {
#if DEBUG
      foreach (Vector bvec in linearBasis.Basis) {
        Debug.Assert(Tools.EQ(bvec.Length, Tools.One), "Linear Basis: All vectors in the basis must have a unit length.");
      }

      for (int i = 0; i < linearBasis.SpaceDim - 1; i++) {
        for (int k = i + 1; k < linearBasis.SpaceDim; k++) {
          Debug.Assert
            (
             Tools.EQ(linearBasis.Basis[i] * linearBasis.Basis[k])
           , "Linear Basis: All pairwise different vectors must be orthogonal."
            );
        }
      }
#endif
    }

  }

}
