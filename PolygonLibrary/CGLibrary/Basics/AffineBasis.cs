using System.Collections;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Represents an affine subspace by its origin point and direction basis.
  /// </summary>
  /// <remarks>
  /// <para>
  /// <see cref="AffineBasis"/> is the immutable variant of the type family.
  /// Its direction space is represented by immutable <see cref="LinearBasis"/>.
  /// </para>
  /// <para>
  /// Use <see cref="AffineBasisMutable"/> when the direction space must be extended in place.
  /// </para>
  /// </remarks>
  public class AffineBasis : IEnumerable, IComparable<AffineBasis> {

#region Data and Properties
    /// <summary>
    /// Gets the origin point of the affine basis.
    /// </summary>
    public Vector Origin { get; }

    /// <summary>
    /// Gets the canonical point of the affine subspace.
    /// </summary>
    /// <remarks>
    /// This is the orthogonal projection of the ambient origin onto the affine subspace.
    /// </remarks>
    public Vector CanonicalOrigin => ProjectPointToSubSpace_in_OrigSpace(Vector.Zero(SpaceDim));

    /// <summary>
    /// Gets the dimension of the affine basis.
    /// </summary>
    public int SpaceDim => Origin.SpaceDim;

    /// <summary>
    /// <c>True</c> if this affine basis is full dimension.
    /// </summary>
    public bool FullDim => LinBasis.FullDim;

    /// <summary>
    /// Gets the number of vectors in the linear basis associated with the affine basis.
    /// </summary>
    public int SubSpaceDim => LinBasis.SubSpaceDim;

    /// <summary>
    /// Gets a value indicating whether the linear basis associated with this affine basis is empty.
    /// </summary>
    public bool Empty => LinBasis.Empty;

    /// <summary>
    /// Gets the vector corresponding to the specified index in the linear basis associated with the affine basis.
    /// </summary>
    /// <param name="ind">The index of the vector to get.</param>
    public Vector this[int ind] => LinBasis[ind];

    /// <summary>
    /// Gets the direction basis of the affine subspace.
    /// </summary>
    public LinearBasis LinBasis => _linearBasis;

    protected LinearBasis _linearBasis;
#endregion

#region Functions
    /// <summary>
    /// Projects a point onto the affine subspace and returns the result in ambient coordinates.
    /// </summary>
    /// <param name="v">The point to project.</param>
    /// <returns>The orthogonal projection of <paramref name="v"/> onto the affine subspace.</returns>
    public Vector ProjectPointToSubSpace_in_OrigSpace(Vector v) {
      if (SubSpaceDim == 0) {
        return Origin;
      }

      return LinBasis.ProjectVectorToSubSpace_in_OrigSpace(v - Origin) + Origin;
    }

    /// <summary>
    /// Projects a point onto the affine subspace and returns its coordinates in the direction basis.
    /// </summary>
    /// <param name="v">The point to project.</param>
    /// <returns>The coordinate vector of the orthogonal projection relative to <see cref="Origin"/> and <see cref="LinBasis"/>.</returns>
    public Vector ProjectPointToSubSpace(Vector v) => LinBasis.ProjectVectorToSubSpace(v - Origin);

    /// <summary>
    /// Projects a sequence of points onto the affine subspace.
    /// </summary>
    /// <param name="Swarm">Points to project.</param>
    /// <returns>The coordinate vectors of the orthogonal projections.</returns>
    public IEnumerable<Vector> ProjectPoints(IEnumerable<Vector> Swarm) {
      foreach (Vector point in Swarm) {
        yield return ProjectPointToSubSpace(point);
      }
    }

    /// <summary>
    /// Maps a coordinate vector in this affine basis back to ambient coordinates.
    /// </summary>
    /// <param name="point">Coordinates relative to <see cref="Origin"/> and <see cref="LinBasis"/>.</param>
    /// <returns>The corresponding ambient point.</returns>
    public Vector ToOriginalCoords(Vector point) {
      Debug.Assert
        (
         SubSpaceDim == point.SpaceDim
       , "AffineBasis.ToOriginalCoords: The dimension of the basis space should be equal to the dimension of the current point."
        );

      return LinBasis.ToOriginalCoords(point) + Origin;
    }

    /// <summary>
    /// Maps a sequence of affine coordinates back to ambient coordinates.
    /// </summary>
    /// <param name="Ps">Coordinate vectors relative to this affine basis.</param>
    /// <returns>The corresponding ambient points.</returns>
    public IEnumerable<Vector> ToOriginalCoords(IEnumerable<Vector> Ps) {
      foreach (Vector point in Ps) {
        yield return ToOriginalCoords(point);
      }
    }

    /// <summary>
    /// Determines whether the specified point belongs to the affine subspace.
    /// </summary>
    /// <param name="v">Point to test.</param>
    /// <returns><c>true</c> if <paramref name="v"/> belongs to the affine subspace; otherwise, <c>false</c>.</returns>
    public bool Contains(Vector v) {
      Debug.Assert
        (
         SpaceDim == v.SpaceDim
       , $"AffineBasis.Contains: The dimension of the vector must be equal to the dimension of the basis vectors! Found: {v.SpaceDim}"
        );

      if (FullDim) { return true; }

      if (Empty) {
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

    /// <summary>
    /// Returns a unit vector from the orthogonal complement of the affine basis direction space.
    /// </summary>
    /// <returns>
    /// A unit vector orthogonal to every direction vector of the affine basis.
    /// Returns the zero vector if the affine basis is full-dimensional.
    /// </returns>
    public Vector OrthogonalComplementVector() => _linearBasis.OrthogonalComplementVector();
#endregion

#region Constructors
    /// <summary>
    /// Constructs the full-dimensional standard affine basis with zero origin.
    /// </summary>
    /// <param name="vecDim">Ambient space dimension.</param>
    public AffineBasis(int vecDim) {
      Origin       = new Vector(vecDim);
      _linearBasis = new LinearBasis(vecDim, vecDim);
    }

    /// <summary>
    /// Constructs the zero-dimensional affine basis consisting of the specified origin point.
    /// </summary>
    /// <param name="o">The origin point of the affine basis.</param>
    public AffineBasis(Vector o) {
      Origin       = o;
      _linearBasis = new LinearBasis(o.SpaceDim, 0);
    }

    /// <summary>
    /// Constructs an affine basis from an origin point and a direction basis.
    /// </summary>
    /// <param name="o">The origin point of the affine basis.</param>
    /// <param name="lBasis">The linear basis associated with the affine basis.</param>
    /// <param name="needCopy">
    /// If <c>true</c>, creates an independent immutable copy of the linear basis.
    /// If <c>false</c>, reuses the source basis only when it is already immutable.
    /// Zero-copy wrapping of <see cref="LinearBasisMutable"/> is not allowed here.
    /// </param>
    public AffineBasis(Vector o, LinearBasis lBasis, bool needCopy = false) {
      Origin = o;

      if (needCopy) {
        _linearBasis = new LinearBasis(lBasis, true);
      }
      else {
        if (lBasis is LinearBasisMutable) {
          throw new ArgumentException("Found LinearBasisMutable in AffineBasis constructor!");
        }
        _linearBasis = lBasis;
      }

#if DEBUG
      CheckCorrectness(this);
#endif
    }

    /// <summary>
    /// Constructs the affine hull of the specified points.
    /// </summary>
    /// <param name="Ps">Points whose affine hull is represented. The first point is used as the origin.</param>
    public AffineBasis(IEnumerable<Vector> Ps) {
      Debug.Assert(Ps.Any(), "AffineBasis: At least one point must be in points.");

      Origin       = Ps.First();
      _linearBasis = new LinearBasis(Ps.Select(v => v - Origin));

#if DEBUG
      CheckCorrectness(this);
#endif
    }

    /// <summary>
    /// Constructs an affine basis from another affine basis.
    /// </summary>
    /// <param name="affineBasis">The affine basis to be copied.</param>
    /// <param name="needCopy">
    /// If <c>true</c>, creates an independent immutable copy.
    /// If <c>false</c>, reuses the source linear basis only for immutable sources.
    /// Zero-copy wrapping of <see cref="AffineBasisMutable"/> is not allowed here.
    /// </param>
    public AffineBasis(AffineBasis affineBasis, bool needCopy) {
      Origin = affineBasis.Origin;

      if (needCopy) {
        _linearBasis = new LinearBasis(affineBasis._linearBasis, true);
      }
      else {
        if (affineBasis is AffineBasisMutable) {
          throw new ArgumentException("Found AffineBasisMutable in AffineBasis copy constructor!");
        }
        _linearBasis = affineBasis._linearBasis;
      }

#if DEBUG
      CheckCorrectness(this);
#endif
    }
#endregion

#region Factories
    /// <summary>
    /// Constructs an affine basis from an origin point and direction vectors.
    /// </summary>
    /// <param name="o">The origin point of the affine basis.</param>
    /// <param name="Vs">Direction vectors spanning the affine subspace.</param>
    /// <returns>The resulting affine basis.</returns>
    public static AffineBasis FromVectors(Vector o, IEnumerable<Vector> Vs) => new(o, new LinearBasis(Vs), false);

    /// <summary>
    /// Constructs the affine hull of the origin point and the specified points.
    /// </summary>
    /// <param name="o">The origin of the affine basis.</param>
    /// <param name="Ps">Points that should belong to the affine subspace.</param>
    /// <returns>The resulting affine basis.</returns>
    public static AffineBasis FromPoints(Vector o, IEnumerable<Vector> Ps) => new(o, new LinearBasis(Ps.Select(v => v - o)), false);

    /// <summary>
    /// Generates a random affine basis in the specified ambient and subspace dimensions.
    /// </summary>
    /// <param name="spaceDim">Ambient space dimension.</param>
    /// <param name="subSpaceDim">Direction subspace dimension.</param>
    /// <param name="random">Random generator. If <c>null</c>, the default generator is used.</param>
    /// <returns>A random affine basis.</returns>
    public static AffineBasis GenAffineBasis(int spaceDim, int subSpaceDim, GRandomLC? random = null)
      => new(Vector.GenVector(spaceDim, random), LinearBasis.GenLinearBasis(spaceDim, subSpaceDim, random), false);
#endregion

    /// <summary>
    /// Compares the current affine basis with another one to determine their relative order.
    /// The comparison is based on a canonical representation of the affine spaces.
    /// </summary>
    /// <remarks>
    /// An affine space is uniquely defined by its parallel linear subspace and a point.
    /// The comparison uses a canonical representation for both:
    /// <list type="number">
    /// <item><description>The linear subspace is compared using its canonical Reduced Row Echelon Form (RREF).</description></item>
    /// <item><description>The position is compared using a canonical origin point, which is the projection of the ambient space's origin onto the affine space.</description></item>
    /// </list>
    /// The comparison proceeds in the following order:
    /// 1. By the dimension of the ambient space (<see cref="SpaceDim"/>).
    /// 2. By the dimension of the subspace (<see cref="SubSpaceDim"/>).
    /// 3. By the canonical representation of the associated linear subspaces (<see cref="LinBasis"/>).
    /// 4. By the coordinates of their canonical origin points.
    /// </remarks>
    /// <param name="other">The affine basis to compare with this instance.</param>
    /// <returns>
    /// An integer that indicates the relative order of the objects being compared.
    /// <list type="bullet">
    /// <item><description>Less than zero: This instance precedes <paramref name="other"/> in the sort order.</description></item>
    /// <item><description>Zero: This instance occurs in the same position in the sort order as <paramref name="other"/> (they represent the same affine space).</description></item>
    /// <item><description>Greater than zero: This instance follows <paramref name="other"/> in the sort order.</description></item>
    /// </list>
    /// </returns>
    public int CompareTo(AffineBasis? other) {
      if (other is null) { return 1; }

      if (SubSpaceDim == 0) { return this.Origin.CompareTo(other.Origin); }

      int basisCompare = this.LinBasis.CompareTo(other.LinBasis);

      return basisCompare != 0 ? basisCompare : this.CanonicalOrigin.CompareTo(other.CanonicalOrigin);
    }

    /// <summary>
    /// Determines whether the specified affine basis represents the same affine subspace as the current instance.
    /// </summary>
    /// <param name="other">Affine basis to compare with this instance.</param>
    /// <returns><c>True</c> if they are equal, else <c>False</c>.</returns>
    public bool Equals(AffineBasis? other) => other is not null && CompareTo(other) == 0;

    /// <summary>
    /// Determines whether the specified object represents the same affine subspace as the current instance.
    /// </summary>
    /// <param name="obj">Object to compare with this affine basis.</param>
    /// <returns><c>True</c> if they are equal, else <c>False</c>.</returns>
    public override bool Equals(object? obj) => obj is AffineBasis other && Equals(other);

    public override int GetHashCode() => throw new InvalidOperationException();

    /// <summary>
    /// Returns an enumerator that iterates through the linear basis of an affine basis as an IEnumerable.
    /// </summary>
    /// <summary>
    /// Returns an enumerator over the direction basis vectors.
    /// </summary>
    public IEnumerator GetEnumerator() { return LinBasis.GetEnumerator(); }

    /// <summary>
    /// Verifies the consistency of the affine basis.
    /// </summary>
    /// <param name="affineBasis">Affine basis to validate.</param>
    public static void CheckCorrectness(AffineBasis affineBasis) {
      if (!affineBasis.LinBasis.Empty) {
        if (affineBasis.Origin.SpaceDim != affineBasis.LinBasis.SpaceDim) {
          throw new ArgumentException
            (
             $"AffineBasis.CheckCorrectness: The space dimensions of the Origin and the LinearBasis should be equal! Found sdim(Orig) = {affineBasis.Origin.SpaceDim}, sdim(LBasis) = {affineBasis.LinBasis.SpaceDim}"
            );
        }
      }
      affineBasis.LinBasis.CheckCorrectness();
    }

  }
}



