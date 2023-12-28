using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
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
          if (Basis.Any())
            return VecDim == Basis.Count;
          else {
            return false;
          }
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
    public bool AddVector(Vector v, bool orthogonalize = true) {
      Vector toAdd = v;

      if (!orthogonalize) {
        Basis.Add(v);

        return true;
      }

      toAdd = IsEmpty ? toAdd.NormalizeZero() : Vector.OrthonormalizeAgainstBasis(v, Basis);

      if (toAdd.IsZero) {
        return false;
      } else {
        Basis.Add(toAdd);

        return true;
      }
    }


    /// <summary>
    /// Expands a vector based on the basis of this linear space.
    /// </summary>
    /// <param name="v">The vector to expand.</param>
    /// <returns>The expanded vector.</returns>
    public Vector Expansion(Vector v) {
      Debug.Assert(v.Dim == VecDim, "Vector dimension and basis dimensions don't match!");
      Debug.Assert(IsFullDim, "To expanse a vector the basis must be of full dimension!");

      TNum[] expan = new TNum[VecDim];

      for (int i = 0; i < VecDim; i++) {
        expan[i] = v * Basis[i];
      }

      return new Vector(expan);
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
      Debug.Assert(VecDim == v.Dim, "The dimension of the basis vectors should be equal to the dimension of the given vector.");

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
    public IEnumerable<Vector> ProjectVectors(IEnumerable<Vector> Swarm) {
      foreach (Vector v in Swarm) {
        yield return ProjectVector(v);
      }
    }
#endregion

#region Constructors
    /// <summary>
    /// Default constructor
    /// </summary>
    public LinearBasis() => Basis = new List<Vector>();

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="linearBasis">The linear basis to be copied.</param>
    public LinearBasis(LinearBasis linearBasis) { Basis = new List<Vector>(linearBasis.Basis); }

    /// <summary>
    /// Based on collection constructor
    /// </summary>
    /// <param name="Vs">Vectors on which basis should be constructed</param>
    /// <param name="orthogonalize">If the vectors do not need to be orthogonalized, it should be set to false</param>
    public LinearBasis(IEnumerable<Vector> Vs, bool orthogonalize = true) {
      Basis = new List<Vector>();

      foreach (Vector v in Vs) {
        AddVector(v, orthogonalize);
        if (IsFullDim) { break; }
      }
    }
#endregion


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
