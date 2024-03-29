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
  /// Represents an affine basis.
  /// </summary>
  public class AffineBasis {

#region Data and Properties
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
    public bool IsFullDim => LinearBasis.IsFullDim;

    /// <summary>
    /// Gets the number of vectors in the linear basis associated with the affine basis.
    /// </summary>
    public int SpaceDim => LinearBasis.SpaceDim;

    /// <summary>
    /// Gets a value indicating whether this affine basis is empty.
    /// </summary>
    public bool IsEmpty => SpaceDim == 0;

    /// <summary>
    /// Gets the vector corresponding to the specified index in the linear basis associated with the affine basis.
    /// </summary>
    /// <param name="ind">The index of the vector to get.</param>
    public Vector this[int ind] => LinearBasis[ind];

    /// <summary>
    /// Gets the current basis of the affine space as list of vectors.
    /// </summary>
    public List<Vector> Basis => LinearBasis.Basis;

    /// <summary>
    /// The linear basis associated with the affine basis.
    /// </summary>
    public LinearBasis LinearBasis { get; }
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

      return LinearBasis.AddVector(v, orthogonalize);
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

      return LinearBasis.Expansion(v);
    }

    /// <summary>
    /// Computes the expansion of the specified point in the affine basis.
    /// </summary>
    /// <param name="p">The point to expand.</param>
    /// <returns>The expansion of the point in the affine basis.</returns>
    public Vector Expansion(Point p) {
      Debug.Assert(Origin.Dim == p.Dim, "Expansion a point with a wrong dimension.");

      return LinearBasis.Expansion(p - Origin);
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
        np[i] = LinearBasis[i] * t;
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
    /// Translates given point from current coordinate system to the original one.
    /// </summary>
    /// <param name="point">The point should be written in terms of this affine basis.</param>
    /// <returns>The point expressed in terms of the original affine system.</returns>
    public Point TranslateToOriginal(Point point) {
      Debug.Assert
        (SpaceDim == point.Dim, "The dimension of the basis space should be equal to the dimension of the current point.");

      Vector np = new Vector(Origin);
      for (int i = 0; i < Basis.Count; i++) {
        np += point[i] * Basis[i];
      }

      return new Point(np);
    }

    /// <summary>
    /// Translates given set of points from current coordinate system to the original one.
    /// </summary>
    /// <param name="Ps">Points should be written in terms of this affine basis.</param>
    /// <returns>Points expressed in terms of the original affine system.</returns>
    public IEnumerable<Point> TranslateToOriginal(IEnumerable<Point> Ps) {
      foreach (Point point in Ps) {
        yield return TranslateToOriginal(point);
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
      Origin      = o;
      LinearBasis = new LinearBasis();
    }

    /// <summary>
    /// Construct the new affine basis of full dim with d-dim zero origin and d-orth
    /// </summary>
    /// <param name="d">The dimension of the basis</param>
    public AffineBasis(int d) {
      Origin      = new Point(d);
      LinearBasis = new LinearBasis();

      for (int i = 0; i < Origin.Dim; i++) {
        LinearBasis.AddVector(Vector.CreateOrth(Origin.Dim, i + 1), false);
      }
    }


    /// <summary>
    /// Construct the new affine basis for a given dimension using a d-dimensional origin of zero,
    /// and a given amount of d-orth starting from e1.
    /// </summary>
    /// <param name="basisDim">The amount of basis vectors in the basis.</param>
    /// <param name="vecDim">The dimension of the basis vectors.</param>
    public AffineBasis(int basisDim, int vecDim) {
      Debug.Assert(basisDim <= vecDim, "The dimension of the basis should be non greater than dimension of the vectors.");

      Origin      = new Point(vecDim);
      LinearBasis = new LinearBasis();

      for (int i = 0; i < basisDim; i++) {
        LinearBasis.AddVector(Vector.CreateOrth(Origin.Dim, i + 1), false);
      }
    }


    /// <summary>
    ///Construct the new affine basis with the specified origin point and specified vectors.
    /// </summary>
    /// <param name="o">The origin point of the affine basis.</param>
    /// <param name="Vs">The vectors to use in the linear basis associated with the affine basis.</param>
    /// <param name="orthogonalize">If the vectors do not need to be orthogonalized, it should be set to false</param>
    public AffineBasis(Point o, IEnumerable<Vector> Vs, bool orthogonalize = true) {
      Origin      = o;
      LinearBasis = new LinearBasis(Vs, orthogonalize);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AffineBasis"/> class with the specified origin point and points.
    /// </summary>
    /// <param name="o">The origin point of the affine basis.</param>
    /// <param name="Ps">The points to use in the linear basis associated with the affine basis.</param>
    public AffineBasis(Point o, IEnumerable<Point> Ps) {
      Origin      = o;
      LinearBasis = new LinearBasis();

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

      Origin      = Ps.First();
      LinearBasis = new LinearBasis();

      foreach (Point p in Ps) {
        AddVectorToBasis(p - Origin);
        if (IsFullDim) { break; }
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AffineBasis"/> class with the specified origin point and linear basis.
    /// </summary>
    /// <param name="o">The origin point of the affine basis.</param>
    /// <param name="lBasis">The linear basis associated with the affine basis.</param>
    public AffineBasis(Point o, LinearBasis lBasis) {
      Origin      = o;
      LinearBasis = new LinearBasis(lBasis);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="affineBasis">The affine basis to be copied.</param>
    public AffineBasis(AffineBasis affineBasis) : this(affineBasis.Origin, affineBasis.Basis, false) { }
#endregion


    /// <summary>
    /// Aux method to check then the basis is correct
    /// </summary>
    /// <param name="affineBasis">Basis to be checked</param>
    public static void CheckCorrectness(AffineBasis affineBasis) {
#if DEBUG
      LinearBasis.CheckCorrectness(affineBasis.LinearBasis);
#endif
    }

  }

}
