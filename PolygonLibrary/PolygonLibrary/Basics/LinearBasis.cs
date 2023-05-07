using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PolygonLibrary.Basics;

/// <summary>
/// Orthonormal basis
/// </summary>
public class LinearBasis {

  /// <summary>
  /// Internal storage
  /// </summary>
  private readonly List<Vector> _basis;

  /// <summary>
  /// The dimension of the basis vectors
  /// </summary>
  public int Dim {
    get
      {
        Debug.Assert(_basis.Any(), "Basis must have at least one vector to determine it dimension");

        return _basis[0].Dim;
      }
  }

  /// <summary>
  /// Property True if basis contain d-linear independent vectors in dD
  /// </summary>
  public bool IsFullDim => Dim == _basis.Count;

  /// <summary>
  /// Amount of vectors in the basis 
  /// </summary>
  public int Count => _basis.Count;

  /// <summary>
  /// True if there are no vectors in the basis
  /// </summary>
  public bool IsEmpty => Count == 0;

  /// <summary>
  /// Index access
  /// </summary>
  /// <param name="ind">Index to be accessed</param>
  public Vector this[int ind] {
    get
      {
        Debug.Assert(ind >= 0 && ind < _basis.Count);

        return _basis[ind];
      }
  }

  /// <summary>
  /// Add the given vector to the basis. If it is zero vector or linear dependent with the basis then it don't includes in.
  /// </summary>
  /// <param name="v">Vector to be added</param>
  /// <returns>True if vector added to the basis, false otherwise</returns>
  public bool AddVector(Vector v) {
    Vector toAdd = v;

    if (IsEmpty) {
      toAdd.NormalizeZero();
    } else {
      toAdd = Vector.OrthonormalizeAgainstBasis(v, _basis);
    }

    if (toAdd.IsZero) {
      return false;
    } else {
      _basis.Add(toAdd);

      return true;
    }
  }

  /// <summary>
  /// Expands a vector based on the basis of this linear space.
  /// </summary>
  /// <param name="v">The vector to expand.</param>
  /// <returns>The expanded vector.</returns>
  public Vector Expansion(Vector v) {
    Debug.Assert(v.Dim == Dim, "Vector dimension and basis dimensions don't match!");
    Debug.Assert(IsFullDim, "To expanse a vector the basis must be of full dimension!");

    var expan = new double[Dim];

    for (int i = 0; i < Dim; i++) {
      expan[i] = v * _basis[i];
    }

    return new Vector(expan);
  }

  /// <summary>
  /// Default constructor
  /// </summary>
  public LinearBasis() { _basis = new List<Vector>(); }

  /// <summary>
  /// Based on collection constructor 
  /// </summary>
  /// <param name="Vs">Vectors on which basis should be constructed</param>
  public LinearBasis(IEnumerable<Vector> Vs) {
    _basis = new List<Vector>();

    foreach (Vector v in Vs) {
      AddVector(v);
    }
  }

}
