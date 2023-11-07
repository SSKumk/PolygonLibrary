using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace CGLibrary;

public partial class Geometry<TNum, TConv> where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Represents an affine basis.
  /// </summary>
  public class AffineBasis {

    #region Data and Properties
    /// <summary>
    /// The linear basis associated with the affine basis.
    /// </summary>
    private readonly LinearBasis _basis;

    /// <summary>
    /// Gets the origin point of the affine basis.
    /// </summary>
    public Point Origin { get; }

    /// <summary>
    /// Gets the dimension of the affine basis.
    /// </summary>
    public int VecDim => Origin.Dim;

    /// <summary>
    /// <c>True</c> if this affine basis is full dimension.
    /// </summary>
    public bool IsFullDim => _basis.IsFullDim;

    /// <summary>
    /// Gets the number of vectors in the linear basis associated with the affine basis.
    /// </summary>
    public int SpaceDim => _basis.BasisDim;

    /// <summary>
    /// Gets a value indicating whether this affine basis is empty.
    /// </summary>
    public bool IsEmpty => SpaceDim == 0;

    /// <summary>
    /// Gets the vector corresponding to the specified index in the linear basis associated with the affine basis.
    /// </summary>
    /// <param name="ind">The index of the vector to get.</param>
    public Vector this[int ind] => _basis[ind];

    /// <summary>
    /// Gets the current basis of the affine space
    /// </summary>
    public List<Vector> Basis => _basis.Basis;
    #endregion

    #region Functions
    /// <summary>
    /// Adds the vector to the linear basis associated with the affine basis.
    /// </summary>
    /// <param name="v">The vector to add.</param>
    /// <param name="orthogonalize">If the vector does not need to be orthogonalized, it should be set to false</param>
    /// <returns><c>true</c> if the vector was added successfully; otherwise, <c>false</c>.</returns>
    public bool AddVectorToBasis(Vector v, bool orthogonalize = true) {
      Debug.Assert(Origin.Dim == v.Dim, "Adding a vector with a wrong dimension into an affine basis.");

      return _basis.AddVector(v, orthogonalize);
    }

    /// <summary>
    /// Adds the specified point to the linear basis associated with the affine basis.
    /// </summary>
    /// <param name="p">The point to add.</param>
    /// <param name="orthogonalize">If the vector does not need to be orthogonalized, it should be set to false</param>
    /// <returns><c>true</c> if the point was added successfully; otherwise, <c>false</c>.</returns>
    public bool AddPointToBasis(Point p, bool orthogonalize = true) {
      Debug.Assert(Origin.Dim == p.Dim, "Adding a point with a wrong dimension into an affine basis.");

      return AddVectorToBasis(p - Origin, orthogonalize);
    }

    /// <summary>
    /// Computes the expansion of the specified vector in the affine basis.
    /// </summary>
    /// <param name="v">The vector to expand.</param>
    /// <returns>The expansion of the vector in the affine basis.</returns>
    public Vector Expansion(Vector v) {
      Debug.Assert(Origin.Dim == v.Dim, "Expansion a vector with a wrong dimension.");

      return _basis.Expansion(v);
    }

    /// <summary>
    /// Computes the expansion of the specified point in the affine basis.
    /// </summary>
    /// <param name="p">The point to expand.</param>
    /// <returns>The expansion of the point in the affine basis.</returns>
    public Vector Expansion(Point p) {
      Debug.Assert(Origin.Dim == p.Dim, "Expansion a point with a wrong dimension.");

      return _basis.Expansion(p - Origin);
    }

    /// <summary>
    /// Projects a given point onto the affine basis.
    /// </summary>
    /// <param name="point">The point to project.</param>
    /// <returns>The projected point.</returns>
    public Point ProjectPoint(Point point) {
      Debug.Assert
        (VecDim == point.Dim, "The dimension of the basis vectors should be equal to the dimension of the current point.");

      Vector t = point - Origin;
      TNum[] np = new TNum[SpaceDim];

      for (int i = 0; i < SpaceDim; i++) {
        np[i] = _basis[i] * t;
      }

      return new Point(np);
    }

    /// <summary>
    /// Projects a given set of points onto the affine basis.
    /// </summary>
    /// <param name="Swarm">The set of points to project.</param>
    /// <returns>The projected points.</returns>
    public IEnumerable<Point> ProjectPoints(IEnumerable<Point> Swarm) {
      foreach (Point point in Swarm) {
        yield return ProjectPoint(point);
      }
    }

    /// <summary>
    /// Checks if a point belongs to the affine subspace defined by the basis.
    /// </summary>
    /// <param name="p">Point to be checked.</param>
    /// <returns><c>true</c> if the point belongs to the subspace, <c>false</c> otherwise.</returns>
    public bool Contains(Point p) {
      Debug.Assert
        (
         p.Dim == VecDim
       , "AffineBasis.Contains: The dimension of a point does not match to the vector dimension of the affine basis."
        );

      Vector res = Vector.OrthonormalizeAgainstBasis(p - Origin, Basis);

      return res.IsZero;
    }


    /// <summary>
    /// Gets the basis vectors as a list of points.
    /// </summary>
    /// <returns>The list of points representing the basis vectors.</returns>
    public List<Point> GetBasisAsPoints() {
      List<Point> points = new List<Point>();

      foreach (Vector bvec in Basis) {
        points.Add(new Point(bvec));
      }

      return points;
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Construct the new affine basis with the specified origin point.
    /// </summary>
    /// <param name="o">The origin point of the affine basis.</param>
    public AffineBasis(Point o) {
      Origin = o;
      _basis = new LinearBasis();
    }

    /// <summary>
    /// Construct the new affine basis of full dim with d-dim zero origin and d-orth
    /// </summary>
    /// <param name="d">The dimension of the basis</param>
    public AffineBasis(int d) {
      Origin = new Point(d);
      _basis = new LinearBasis();

      for (int i = 0; i < Origin.Dim; i++) {
        _basis.AddVector(Vector.CreateOrth(Origin.Dim, i + 1), false);
      }
    }

    /// <summary>
    /// Construct the new affine basis of given dimension with d-dim zero origin and d-orth
    /// </summary>
    public AffineBasis(int vecDim, int basisDim) {
      Debug.Assert(basisDim <= vecDim, "The dimension of the basis should be non greater than dimension of the vectors.");

      Origin = new Point(vecDim);
      _basis = new LinearBasis();

      for (int i = 0; i < basisDim; i++) {
        _basis.AddVector(Vector.CreateOrth(Origin.Dim, i + 1), false);
      }
    }


    /// <summary>
    ///Construct the new affine basis with the specified origin point and specified vectors.
    /// </summary>
    /// <param name="o">The origin point of the affine basis.</param>
    /// <param name="Vs">The vectors to use in the linear basis associated with the affine basis.</param>
    /// <param name="orthogonalize">If the vectors do not need to be orthogonalized, it should be set to false</param>
    public AffineBasis(Point o, IEnumerable<Vector> Vs, bool orthogonalize = true) {
      Origin = o;
      _basis = new LinearBasis(Vs, orthogonalize);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AffineBasis"/> class with the specified origin point and points.
    /// </summary>
    /// <param name="o">The origin point of the affine basis.</param>
    /// <param name="Ps">The points to use in the linear basis associated with the affine basis.</param>
    public AffineBasis(Point o, IEnumerable<Point> Ps) {
      Origin = o;
      _basis = new LinearBasis();

      foreach (Point p in Ps) {
        AddVectorToBasis(p - Origin);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AffineBasis"/> class with the enumerable of points.
    /// The first point is interpret as origin.
    /// </summary>
    /// <param name="Ps">The points to construct the affine basis.</param>
    public AffineBasis(IEnumerable<Point> Ps) {
      Debug.Assert(Ps.Any(), "At least one point should be in points");

      Origin = Ps.First();
      _basis = new LinearBasis();

      foreach (Point p in Ps) {
        AddVectorToBasis(p - Origin);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AffineBasis"/> class with the specified origin point and linear basis.
    /// </summary>
    /// <param name="o">The origin point of the affine basis.</param>
    /// <param name="lBasis">The linear basis associated with the affine basis.</param>
    public AffineBasis(Point o, LinearBasis lBasis) {
      Origin = o;
      _basis = new LinearBasis(lBasis);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="affineBasis">The affine basis to be copied.</param>
    public AffineBasis(AffineBasis affineBasis) : this(affineBasis.Origin, affineBasis.Basis, false) { }

    /// <summary>
    /// The basis construct on two affine bases.
    /// </summary>
    /// <param name="aBasis1">First affine basis.</param>
    /// <param name="aBasis2">Second affine basis.</param>
    public AffineBasis(AffineBasis aBasis1, AffineBasis aBasis2)
      : this(aBasis1) {
      foreach (Vector bvec in aBasis2.Basis) {
        _basis.AddVector(bvec);
      }
    }
    #endregion


    /// <summary>
    /// Aux method to check then the basis is correct
    /// </summary>
    /// <param name="affineBasis">Basis to be checked</param>
    public static void CheckCorrectness(AffineBasis affineBasis) {
#if DEBUG
      LinearBasis.CheckCorrectness(affineBasis._basis);
#endif
    }

  }

}
