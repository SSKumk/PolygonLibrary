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
    public Vector Origin { get; }

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
      Debug.Assert
        (Origin.Dim == v.Dim, "AffineBasis.AddVectorToBasis: Adding a vector with a wrong dimension into an affine basis.");

      return LinearBasis.AddVectorToBasis(v, orthogonalize);
    }

    /// <summary>
    /// Projects a given point onto the affine basis.
    /// </summary>
    /// <param name="point">The point to project.</param>
    /// <returns>The projected point.</returns>
    public Vector ProjectPoint(Vector point) {
      Debug.Assert
        (
         VecDim == point.Dim
       , "AffineBasis.ProjectPoint: The dimension of the basis vectors should be equal to the dimension of the current point."
        );

      Vector t = point - Origin;

      TNum[] np = new TNum[SpaceDim];
      for (int i = 0; i < SpaceDim; i++) {
        np[i] = LinearBasis[i] * t;
      }

      return new Vector(np);
    }

    /// <summary>
    /// Projects a given set of points onto the affine basis.
    /// </summary>
    /// <param name="Swarm">The set of points to project.</param>
    /// <returns>The projected points.</returns>
    public IEnumerable<Vector> ProjectPoints(IEnumerable<Vector> Swarm) {
      foreach (Vector point in Swarm) {
        yield return ProjectPoint(point);
      }
    }

    /// <summary>
    /// Translates given point from current coordinate system to the original one.
    /// </summary>
    /// <param name="point">The point should be written in terms of this affine basis.</param>
    /// <returns>The point expressed in terms of the original affine system.</returns>
    public Vector TranslateToOriginal(Vector point) {
      Debug.Assert
        (
         SpaceDim == point.Dim
       , "AffineBasis.TranslateToOriginal: The dimension of the basis space should be equal to the dimension of the current point."
        );

      Vector np = new Vector(Origin);
      for (int i = 0; i < Basis.Count; i++) {
        np += point[i] * Basis[i];
      }

      return new Vector(np);
    }

    /// <summary>
    /// Translates given set of points from current coordinate system to the original one.
    /// </summary>
    /// <param name="Ps">Points should be written in terms of this affine basis.</param>
    /// <returns>Points expressed in terms of the original affine system.</returns>
    public IEnumerable<Vector> TranslateToOriginal(IEnumerable<Vector> Ps) {
      foreach (Vector point in Ps) {
        yield return TranslateToOriginal(point);
      }
    }

    /// <summary>
    /// Checks if a point belongs to the affine subspace defined by the basis.
    /// </summary>
    /// <param name="p">Vector to be checked.</param>
    /// <returns><c>true</c> if the point belongs to the subspace, <c>false</c> otherwise.</returns>
    public bool Contains(Vector p) {
      Debug.Assert
        (
         p.Dim == VecDim
       , "AffineBasis.Contains: The dimension of a point does not match to the vector dimension of the affine basis."
        );

      Vector res = Vector.OrthonormalizeAgainstBasis(p - Origin, Basis);

      return res.IsZero;
    }
#endregion

#region Constructors
    /// <summary>
    /// Construct the new affine basis of full dim with d-dim zero origin and d-orth
    /// </summary>
    /// <param name="vecDim">The dimension of the basis</param>
    public AffineBasis(int vecDim) {
      Origin      = new Vector(vecDim);
      LinearBasis = new LinearBasis(vecDim);
    }

    /// <summary>
    /// Construct the new affine basis with the specified origin point.
    /// </summary>
    /// <param name="o">The origin point of the affine basis.</param>
    public AffineBasis(Vector o) {
      Origin      = o;
      LinearBasis = new LinearBasis();
    }

    /// <summary>
    /// Construct the new affine basis based on origin and linear basis.
    /// </summary>
    /// <param name="o">The origin point of the affine basis.</param>
    /// <param name="lBasis">The linear basis associated with the affine basis.</param>
    public AffineBasis(Vector o, LinearBasis lBasis) {
      Origin      = o;
      LinearBasis = new LinearBasis(lBasis); // new надо так как есть AddVectorToBasis()
    }

    /// <summary>
    /// Construct the new affine basis which spans given points.
    /// The first point is interpret as origin.
    /// </summary>
    /// <param name="Ps">The points to construct the affine basis.</param>
    public AffineBasis(IEnumerable<Vector> Ps) {
      Debug.Assert(Ps.Any(), "AffineBasis: At least one point must be in points.");

      Origin      = Ps.First();
      LinearBasis = new LinearBasis(Ps.Select(v => v - Origin));
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="affineBasis">The affine basis to be copied.</param>
    public AffineBasis(AffineBasis affineBasis) {
      Origin      = affineBasis.Origin;
      LinearBasis = new LinearBasis(affineBasis.LinearBasis);
    }
#endregion

#region Fabrics
    /// <summary>
    /// Produce the new affine basis with the specified origin point and specified vectors.
    /// </summary>
    /// <param name="o">The origin point of the affine basis.</param>
    /// <param name="Vs">The vectors to use in the linear basis associated with the affine basis.</param>
    /// <param name="orthogonalize">If the vectors do not need to be orthogonalized, it should be set to false</param>
    /// <returns>The affine basis.</returns>
    public static AffineBasis AsVectors(Vector o, IEnumerable<Vector> Vs, bool orthogonalize = true) {
      return new AffineBasis(o, new LinearBasis(Vs, orthogonalize));
    }

    /// <summary>
    /// Produce the new affine basis which is the span of {o, Ps}.
    /// </summary>
    /// <param name="o">The origin of the affine basis.</param>
    /// <param name="Ps">The points which lies in affine space.</param>
    /// <returns>The affine basis.</returns>
    public static AffineBasis AsPoints(Vector o, IEnumerable<Vector> Ps) {
      return new AffineBasis(o, new LinearBasis(Ps.Select(v => v - o)));
    }
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
