using System.Collections;


namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Represents an affine basis.
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
    public int VecDim => Origin.Dim;

    /// <summary>
    /// <c>True</c> if this affine basis is full dimension.
    /// </summary>
    public bool IsFullDim => LinBasis.IsFullDim;

    /// <summary>
    /// Gets the number of vectors in the linear basis associated with the affine basis.
    /// </summary>
    public int SubSpaceDim => LinBasis.SubSpaceDim;

    /// <summary>
    /// Gets a value indicating whether this affine basis is empty.
    /// </summary>
    public bool IsEmpty => LinBasis.IsEmpty;

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
    /// <param name="v">The vector to add.</param>
    /// <param name="orthogonalize">If the vector does not need to be orthogonalized, it should be set to false</param>
    /// <returns><c>true</c> if the vector was added successfully; otherwise, <c>false</c>.</returns>
    public bool AddVectorToBasis(Vector v, bool orthogonalize = true) {
      Debug.Assert(Origin.Dim == v.Dim, "AffineBasis.AddVector: Adding a vector with a wrong dimension into an affine basis.");

      return LinBasis.AddVector(v, orthogonalize);
    }

    /// <summary>
    /// Projects a given point onto the affine basis.
    /// </summary>
    /// <param name="point">The point to project.</param>
    /// <returns>The projected point.</returns>
    public Vector ProjectVector(Vector point) {
      Debug.Assert
        (
         VecDim == point.Dim
       , "AffineBasis.ProjectVector: The dimension of the basis vectors should be equal to the dimension of the current point."
        );

      Vector t = point - Origin;

      TNum[] np = new TNum[SubSpaceDim];
      for (int i = 0; i < SubSpaceDim; i++) {
        np[i] = LinBasis[i] * t;
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
        yield return ProjectVector(point);
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
         SubSpaceDim == point.Dim
       , "AffineBasis.TranslateToOriginal: The dimension of the basis space should be equal to the dimension of the current point."
        );

      Vector np = new Vector(Origin);
      for (int i = 0; i < SubSpaceDim; i++) {
        np += point[i] * this[i];
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
    public bool Contains(Vector p) => LinearBasis.IsContains(p - Origin, LinBasis);
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
      LinBasis = new LinearBasis(o.Dim, 0);
    }

    /// <summary>
    /// Construct the new affine basis based on origin and linear basis.
    /// </summary>
    /// <param name="o">The origin point of the affine basis.</param>
    /// <param name="lBasis">The linear basis associated with the affine basis.</param>
    public AffineBasis(Vector o, LinearBasis lBasis) {
      Origin   = o;
      LinBasis = new LinearBasis(lBasis); // new надо так как есть AddVector()


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
    /// Method to check then the basis is correct
    /// </summary>
    /// <param name="affineBasis">Basis to be checked</param>
    internal static void CheckCorrectness(AffineBasis affineBasis) { LinearBasis.CheckCorrectness(affineBasis.LinBasis); }

    //todo А как правильно тут vvv
    public IEnumerator GetEnumerator() { return LinBasis.GetEnumerator(); }

  }

}
