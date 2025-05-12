using System.Collections;


namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Represents an affine basis (origin, linear basis).
  /// </summary>
  public class AffineBasis : IEnumerable {

#region Data and Properties
    /// <summary>
    /// Gets the origin point of the affine basis.
    /// </summary>
    public Vector Origin { get; }

    /// <summary>
    /// Gets the dimension of the affine basis.
    /// </summary>
    public int SpaceDim => Origin.SpaceDim;

    /// <summary>
    /// <c>True</c> if this affine basis is full dimension.
    /// </summary>
    public bool IsFullDim => LinBasis.FullDim;

    /// <summary>
    /// Gets the number of vectors in the linear basis associated with the affine basis.
    /// </summary>
    public int SubSpaceDim => LinBasis.SubSpaceDim;

    /// <summary>
    /// Gets a value indicating whether the linear basis associated with this affine basis is empty.
    /// </summary>
    public bool IsEmpty => LinBasis.Empty;

    /// <summary>
    /// Gets the vector corresponding to the specified index in the linear basis associated with the affine basis.
    /// </summary>
    /// <param name="ind">The index of the vector to get.</param>
    public Vector this[int ind] => LinBasis[ind];

    /// <summary>
    /// The linear basis associated with the affine basis.
    /// </summary>
    public LinearBasis LinBasis { get; }
#endregion

#region Functions
    /// <summary>
    /// Adds the vector to the linear basis associated with the affine basis.
    /// </summary>
    /// <param name="v">The vector to add. Not the point!</param>
    /// <param name="orthogonalize">If the vector does not need to be orthogonalized, it should be set to false</param>
    /// <returns><c>true</c> if the vector was added successfully; otherwise, <c>false</c>.</returns>
    public bool AddVector(Vector v, bool orthogonalize = true) {
      Debug.Assert
        (Origin.SpaceDim == v.SpaceDim, "AffineBasis.AddVector: Adding a vector with a wrong dimension into an affine basis.");

      return LinBasis.AddVector(v, orthogonalize);
    }

    /// <summary>
    /// Projects a point onto the subspace with coordinates in the original space.
    /// </summary>
    /// <param name="v">The vector to project.</param>
    /// <returns>The projected vector.</returns>
    public Vector ProjectPointToSubSpace_in_OrigSpace(Vector v) {
      if (SubSpaceDim == 0) {
        return Origin;
      }

      return LinBasis.ProjectVectorToSubSpace_in_OrigSpace(v - Origin) + Origin;
    }

    /// <summary>
    /// Projects a given point onto the affine basis in its coordinates.
    /// </summary>
    /// <param name="v">The point to project.</param>
    /// <returns>The projected point.</returns>
    public Vector ProjectPointToSubSpace(Vector v) => LinBasis.ProjectVectorToSubSpace(v - Origin);

    /// <summary>
    /// Projects a given set of points onto the affine basis.
    /// </summary>
    /// <param name="Swarm">The set of points to project.</param>
    /// <returns>The projected points.</returns>
    public IEnumerable<Vector> ProjectPoints(IEnumerable<Vector> Swarm) {
      foreach (Vector point in Swarm) {
        yield return ProjectPointToSubSpace(point);
      }
    }

    /// <summary>
    /// Translates given point from the current coordinate system to the original one.
    /// </summary>
    /// <param name="point">The point should be written in terms of this affine basis.</param>
    /// <returns>The point expressed in terms of the original affine system.</returns>
    public Vector ToOriginalCoords(Vector point) {
      Debug.Assert
        (
         SubSpaceDim == point.SpaceDim
       , "AffineBasis.ToOriginalCoords: The dimension of the basis space should be equal to the dimension of the current point."
        );

      return LinBasis.ToOriginalCoords(point) + Origin;
    }

    /// <summary>
    /// Translates given set of points from the current coordinate system to the original one.
    /// </summary>
    /// <param name="Ps">Points should be written in terms of this affine basis.</param>
    /// <returns>Points expressed in terms of the original affine system.</returns>
    public IEnumerable<Vector> ToOriginalCoords(IEnumerable<Vector> Ps) {
      foreach (Vector point in Ps) {
        yield return ToOriginalCoords(point);
      }
    }

    /// <summary>
    /// Checks if a point belongs to the affine subspace defined by the basis.
    /// </summary>
    /// <param name="v">Vector to be checked.</param>
    /// <returns><c>true</c> if the point belongs to the subspace, <c>false</c> otherwise.</returns>
    public bool Contains(Vector v) {
      Debug.Assert
        (
         SpaceDim == v.SpaceDim
       , $"AffineBasis.Contains: The dimension of the vector must be equal to the dimension of the basis vectors! Found: {v.SpaceDim}"
        );

      if (IsFullDim) { return true; }

      if (IsEmpty) {
        return Origin == v;
      }

      // Equivalent to: LinBasis.Contains(v - Origin)
      for (int row = 0; row < SpaceDim; row++) {
        if (Tools.NE(LinBasis.ProjMatrix.MultiplyRowByDiffOfVectors(row, v, Origin), v[row] - Origin[row])) {
          return false;
        }
      }

      return true;
    }
#endregion

#region Constructors
    /// <summary>
    /// Construct the new affine basis of full dim with d-dim zero origin and d-orth
    /// </summary>
    /// <param name="vecDim">The dimension of the basis</param>
    public AffineBasis(int vecDim) {
      Origin   = new Vector(vecDim);
      LinBasis = new LinearBasis(vecDim, vecDim);
    }

    /// <summary>
    /// Construct the new affine basis with the specified origin point.
    /// </summary>
    /// <param name="o">The origin point of the affine basis.</param>
    public AffineBasis(Vector o) {
      Origin   = o;
      LinBasis = new LinearBasis(o.SpaceDim, 0);
    }

    /// <summary>
    /// Construct the new affine basis based on origin and linear basis.
    /// </summary>
    /// <param name="o">The origin point of the affine basis.</param>
    /// <param name="lBasis">The linear basis associated with the affine basis.</param>
    /// <param name="needCopy">Whether the affine basis should be copied.</param>
    public AffineBasis(Vector o, LinearBasis lBasis, bool needCopy = true) {
      Origin = o;
      if (needCopy) {
        LinBasis = new LinearBasis(lBasis); // new надо так как есть AddVector()
      }
      else {
        LinBasis = lBasis;
      }

#if DEBUG
      CheckCorrectness(this);
#endif
    }

    /// <summary>
    /// Construct the new affine basis which spans given points.
    /// The first point is interpreted as origin.
    /// </summary>
    /// <param name="Ps">The points to construct the affine basis.</param>
    public AffineBasis(IEnumerable<Vector> Ps) {
      Debug.Assert(Ps.Any(), "AffineBasis: At least one point must be in points.");

      Origin   = Ps.First();
      LinBasis = new LinearBasis(Ps.Select(v => v - Origin));

#if DEBUG
      CheckCorrectness(this);
#endif
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="affineBasis">The affine basis to be copied.</param>
    public AffineBasis(AffineBasis affineBasis) {
      Origin   = affineBasis.Origin;
      LinBasis = new LinearBasis(affineBasis.LinBasis);

#if DEBUG
      CheckCorrectness(this);
#endif
    }
#endregion

#region Factories
    /// <summary>
    /// Produce the new affine basis with the specified origin point and specified vectors.
    /// </summary>
    /// <param name="o">The origin point of the affine basis.</param>
    /// <param name="Vs">The vectors to use in the linear basis associated with the affine basis.</param>
    /// <param name="orthogonalize">If the vectors do not need to be orthogonalized, it should be set to false</param>
    /// <returns>The affine basis.</returns>
    public static AffineBasis FromVectors(Vector o, IEnumerable<Vector> Vs, bool orthogonalize = true) {
      return new AffineBasis(o, new LinearBasis(Vs, orthogonalize), false);
    }

    /// <summary>
    /// Produce the new affine basis which is the span of {o, Ps}.
    /// </summary>
    /// <param name="o">The origin of the affine basis.</param>
    /// <param name="Ps">The points that lie in affine space.</param>
    /// <returns>The affine basis.</returns>
    public static AffineBasis FromPoints(Vector o, IEnumerable<Vector> Ps) {
      return new AffineBasis(o, new LinearBasis(Ps.Select(v => v - o)), false);
    }

    /// <summary>
    /// Generates an affine basis in the given space dimension and subspace dimension.
    /// </summary>
    /// <param name="spaceDim">The dimension of the ambient space.</param>
    /// <param name="subSpaceDim">The dimension of the linear basis subspace.</param>
    /// <param name="random">The random to be used. If null, the Random be used.</param>
    /// <returns>An affine basis composed of a random vector and a linear basis.</returns>
    public static AffineBasis GenAffineBasis(int spaceDim, int subSpaceDim, GRandomLC? random = null)
      => new(Vector.GenVector(spaceDim, random), LinearBasis.GenLinearBasis(spaceDim, subSpaceDim, random), false);
#endregion

    /// <summary>
    /// Determines whether the specified object represents the same affine subspace as the current instance.
    /// </summary>
    /// <param name="obj">Object to compare with this affine basis.</param>
    /// <returns><c>True</c> if they are equal, else <c>False</c>.</returns>
    public override bool Equals(object? obj) {
      if (obj == null || this.GetType() != obj.GetType()) {
        return false;
      }


      AffineBasis other = (AffineBasis)obj;

      if (!LinBasis.Equals(other.LinBasis)) { return false; }

      return SubSpaceDim == 0 ? Origin.Equals(other.Origin) : Contains(other.Origin);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the linear basis of an affine basis as an IEnumerable.
    /// </summary>
    public IEnumerator GetEnumerator() { return LinBasis.GetEnumerator(); }

    /// <summary>
    /// Method to check then the basis is correct
    /// </summary>
    /// <param name="affineBasis">Basis to be checked</param>
    public static void CheckCorrectness(AffineBasis affineBasis) {
      if (!affineBasis.LinBasis.Empty) {
        if (affineBasis.Origin.SpaceDim != affineBasis.LinBasis.SpaceDim) {
          throw new ArgumentException
            (
             $"AffineBasis.CheckCorrectness: The space dimensions of the Origin and the LinearBasis should be equal! Found sdim(Orig) = {affineBasis.Origin.SpaceDim}, sdim(LBasis) = {affineBasis.LinBasis.SpaceDim}"
            );
        }
      }
      LinearBasis.CheckCorrectness(affineBasis.LinBasis);
    }

  }

}
