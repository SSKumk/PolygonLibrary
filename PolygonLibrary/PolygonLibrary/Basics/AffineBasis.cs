using System.Collections.Generic;
using System.Diagnostics;

namespace PolygonLibrary.Basics;

/// <summary>
/// Represents an affine basis.
/// </summary>
public class AffineBasis {

  /// <summary>
  /// The linear basis associated with the affine basis.
  /// </summary>
  private LinearBasis _basis;

  /// <summary>
  /// The origin point of the affine basis.
  /// </summary>
  private Point _origin;

  /// <summary>
  /// Gets the origin point of the affine basis.
  /// </summary>
  public Point Origin => _origin;

  /// <summary>
  /// Gets the dimension of the affine basis.
  /// </summary>
  public int Dim => _origin.Dim;

  /// <summary>
  /// <c>True</c> if this affine basis is full dimension.
  /// </summary>
  public bool FullDim => _basis.IsFullDim;

  /// <summary>
  /// Gets the number of vectors in the linear basis associated with the affine basis.
  /// </summary>
  public int Count => _basis.Count;

  /// <summary>
  /// Gets a value indicating whether this affine basis is empty.
  /// </summary>
  public bool IsEmpty => Count == 0;

  /// <summary>
  /// Gets the vector corresponding to the specified index in the linear basis associated with the affine basis.
  /// </summary>
  /// <param name="ind">The index of the vector to get.</param>
  public Vector this[int ind] => _basis[ind];

  /// <summary>
  /// Adds the vector to the linear basis associated with the affine basis.
  /// </summary>
  /// <param name="v">The vector to add.</param>
  /// <returns><c>true</c> if the vector was added successfully; otherwise, <c>false</c>.</returns>
  public bool AddVectorToBasis(Vector v) {
    Debug.Assert(_origin.Dim == v.Dim, "Adding a vector with a wrong dimension into an affine basis.");

    return _basis.AddVector(v);
  }

  /// <summary>
  /// Adds the specified point to the linear basis associated with the affine basis.
  /// </summary>
  /// <param name="p">The point to add.</param>
  /// <returns><c>true</c> if the point was added successfully; otherwise, <c>false</c>.</returns>
  public bool AddPointToBasis(Point p) {
    Debug.Assert(_origin.Dim == p.Dim, "Adding a point with a wrong dimension into an affine basis.");

    return AddVectorToBasis(p - _origin);
  }

  /// <summary>
  /// Computes the expansion of the specified vector in the affine basis.
  /// </summary>
  /// <param name="v">The vector to expand.</param>
  /// <returns>The expansion of the vector in the affine basis.</returns>
  public Vector Expansion(Vector v) {
    Debug.Assert(_origin.Dim == v.Dim, "Expansion a vector with a wrong dimension.");

    return _basis.Expansion(v);
  }

  /// <summary>
  /// Computes the expansion of the specified point in the affine basis.
  /// </summary>
  /// <param name="p">The point to expand.</param>
  /// <returns>The expansion of the point in the affine basis.</returns>
  public Vector Expansion(Point p) {
    Debug.Assert(_origin.Dim == p.Dim, "Expansion a point with a wrong dimension.");

    return _basis.Expansion(p - _origin);
  }

  /// <summary>
  /// Construct the new affine basis with the specified origin point.
  /// </summary>
  /// <param name="o">The origin point of the affine basis.</param>
  public AffineBasis(Point o) {
    _origin = o;
    _basis  = new LinearBasis();
  }

  /// <summary>
  ///Construct the new affine basis with the specified origin point and specified vectors.
  /// </summary>
  /// <param name="o">The origin point of the affine basis.</param>
  /// <param name="Vs">The vectors to use in the linear basis associated with the affine basis.</param>
  public AffineBasis(Point o, IEnumerable<Vector> Vs) {
    _origin = o;
    _basis  = new LinearBasis(Vs);
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="AffineBasis"/> class with the specified origin point and points.
  /// </summary>
  /// <param name="o">The origin point of the affine basis.</param>
  /// <param name="Ps">The points to use in the linear basis associated with the affine basis.</param>
  public AffineBasis(Point o, IEnumerable<Point> Ps) {
    _origin = o;
    _basis  = new LinearBasis();

    foreach (Point p in Ps) {
      AddPointToBasis(p);
    }
  }

}
