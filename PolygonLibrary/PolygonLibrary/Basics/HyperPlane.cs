namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Represents a (d-1)-dimensional hyperplane in a d-dimensional euclidean space.
  /// N * x = C, N - the outward normal vector and C is the constant term.
  /// </summary>
  public class HyperPlane {

#region Data and Properties
    /// <summary>
    /// The point through which the hyperplane passes.
    /// </summary>
    public readonly Vector Origin;

    /// <summary>
    /// The dimension of the vectors.
    /// </summary>
    public int SpaceDim => Origin.SpaceDim;

    /// <summary>
    /// The dimension of the hyperplane affine basis.
    /// </summary>
    public int SubSpaceDim { get; }

    /// <summary>
    /// The affine basis associated with the hyperplane.
    /// </summary>
    public AffineBasis AffBasis {
      get
        {
          if (_affBasis is null) {
            Debug.Assert
              (_normal is not null, "HyperPlane.AffBasis: Normal is null. Can't construct an affine basis of a hyperplane.");

            _affBasis = new AffineBasis(Origin, new LinearBasis(Normal).FindOrthogonalComplement()!, false);

#if DEBUG
            CheckCorrectness(this);
#endif
          }

          return _affBasis;
        }
    }

    private AffineBasis? _affBasis = null;

    /// <summary>
    /// The outward normal vector to the hyperplane.
    /// </summary>
    public Vector Normal {
      get
        {
          if (_normal is null) {
            Debug.Assert(_affBasis is not null, "HyperPlane.Normal: Affine basis is null. Can't construct the Normal.");

            _normal = AffBasis.LinBasis.FindOrthonormalVector();

#if DEBUG
            CheckCorrectness(this);
#endif
          }

          return _normal;
        }
    }

    /// <summary>
    /// The constant term in the equation of the hyperplane.
    /// </summary>
    public TNum ConstantTerm {
      get
        {
          _constantTerm ??= Normal * Origin;

          return _constantTerm.Value;
        }
    }

    private Vector? _normal       = null;
    private TNum?   _constantTerm = null;
#endregion

#region Constructors
    /// <summary>
    /// Constructs a hyperplane from the normal vector and the given point.
    /// </summary>
    /// <param name="normal">The normal vector to a hyperplane.</param>
    /// <param name="origin">The point through which a hyperplane passes.</param>
    /// <param name="needNormalize">Whether the normal vector should be normalized.</param>
    public HyperPlane(Vector normal, Vector origin, bool needNormalize = true) {
      _normal     = needNormalize ? normal.Normalize() : normal;
      Origin      = origin;
      SubSpaceDim = Origin.SpaceDim - 1;

#if DEBUG
      CheckCorrectness(this);
#endif
    }

    /// <summary>
    /// Constructs a hyperplane from the normal vector and the constant term.
    /// </summary>
    /// <param name="normal">The normal vector to a hyperplane.</param>
    /// <param name="constant">The constant term in right part.</param>
    public HyperPlane(Vector normal, TNum constant) {
      Origin      = normal * constant;
      SubSpaceDim = normal.SpaceDim - 1;
      TNum lengthN = normal.Length;
      _normal       = normal / lengthN;
      _constantTerm = constant / lengthN;

#if DEBUG
      CheckCorrectness(this);
#endif
    }

    /// <summary>
    /// Constructs a hyperplane from the given affine basis.
    /// </summary>
    /// <param name="affBasis">The affine basis that defines a hyperplane.</param>
    /// <param name="needCopy">Whether the affine basis should be copied.</param>
    /// <param name="toOrient">The pair to orient a hyperplane explicitly.
    /// If not null, the point part of the pair should belong to the positive semi-space
    /// if the bool part of the pair is <c>true</c>, and to the negative semi-space otherwise.</param>
    public HyperPlane(AffineBasis affBasis, bool needCopy, (Vector point, bool isPositive)? toOrient = null) {
      Debug.Assert
        (
         affBasis.SubSpaceDim == affBasis.Origin.SpaceDim - 1
       , $"HyperPlane.Ctor: Hyperplane should has (d-1) = {affBasis.Origin.SpaceDim - 1} independent vectors in its basis. Found {affBasis.SubSpaceDim}"
        );

      _affBasis   = needCopy ? new AffineBasis(affBasis) : affBasis;
      Origin      = affBasis.Origin;
      SubSpaceDim = Origin.SpaceDim - 1;

      if (toOrient is not null) {
        OrientNormal(toOrient.Value.point, toOrient.Value.isPositive);
      }


#if DEBUG
      CheckCorrectness(this);
#endif
    }

    /// <summary>
    /// Constructs a hyperplane from a given set of points that lie within the plane.
    /// </summary>
    /// <param name="inPlane">A collection of vectors that define the hyperplane.</param>
    /// <param name="toOrient">
    /// A pair to explicitly orient the hyperplane.
    /// If not null, the point part of the pair should belong to the positive semi-space
    /// if the bool part of the pair is true, and to the negative semi-space otherwise.
    /// </param>
    public HyperPlane(IEnumerable<Vector> inPlane, (Vector point, bool isPositive)? toOrient = null) : this
      (new AffineBasis(inPlane), false, toOrient) { }
#endregion

#region Functions
    /// <summary>
    /// Method to orient normal.
    /// </summary>
    /// <param name="point">Vector should belong to the semi-space.</param>
    /// <param name="isPositive">If the bool is <c>true</c> than point is in positive semi-space, in the negative semi-space otherwise.</param>
    public void OrientNormal(Vector point, bool isPositive) {
      TNum res = Eval(point);

      Debug.Assert(Tools.NE(res), "HyperPlane.OrientNormal: A given point belongs to the hyperplane.");

      if ((Tools.LT(res) && isPositive) || (Tools.GT(res) && !isPositive)) {
        _normal       = -_normal!;
        _constantTerm = -_constantTerm!;
      }
    }

    /// <summary>
    /// Evaluates the equation of the hyperplane at a given point.
    /// </summary>
    /// <param name="point">The point at which the equation is to be evaluated.</param>
    /// <returns>The value of the equation of the hyperplane at the given point.</returns>
    public TNum Eval(Vector point) { return Normal * point - ConstantTerm; }

    /// <summary>
    /// Checks if the hyperplane contains a given point.
    /// </summary>
    /// <param name="point">The point to check.</param>
    /// <returns><c>True</c> if the hyperplane contains the given point, otherwise <c>False</c>.</returns>
    public bool Contains(Vector point) { return Tools.EQ(Eval(point)); }

    /// <summary>
    /// Checks if a given point lies in the negative-part of the hyperplane.
    /// </summary>
    /// <param name="point">The point to check.</param>
    /// <returns><c>True</c> if a given point lies in the negative-part, otherwise <c>False</c>.</returns>
    public bool ContainsNegative(Vector point) { return Tools.LT(Eval(point)); }

    /// <summary>
    /// Checks if a given point lies in the positive-part of the hyperplane.
    /// </summary>
    /// <param name="point">The point to check.</param>
    /// <returns><c>True</c> if a given point lies in the positive-part, otherwise <c>False</c>.</returns>
    public bool ContainsPositive(Vector point) { return Tools.GT(Eval(point)); }

    /// <summary>
    /// Checks if a given point lies in the negative-part of the hyperplane or belongs to it.
    /// </summary>
    /// <param name="point">The point to check.</param>
    /// <returns><c>True</c> if a given point lies in the plane or in the negative-part, otherwise <c>False</c>.</returns>
    public bool ContainsNegativeNonStrict(Vector point) { return Tools.LE(Eval(point)); }

    /// <summary>
    /// Filters a given collection of points, leaving only those that lie on the hyperplane.
    /// </summary>
    /// <param name="Swarm">The collection of points to filter.</param>
    /// <returns>A collection of points that lie on the hyperplane.</returns>
    public IEnumerable<Vector> FilterIn(IEnumerable<Vector> Swarm) {
      foreach (Vector p in Swarm) {
        if (Contains(p)) {
          yield return p;
        }
      }
    }

    /// <summary>
    /// Filters a given collection of points, leaving only those that not lie on the hyperplane.
    /// </summary>
    /// <param name="Swarm">The collection of points to filter.</param>
    /// <returns>A collection of points that lie outside of the hyperplane.</returns>
    public IEnumerable<Vector> FilterNotIn(IEnumerable<Vector> Swarm) {
      foreach (Vector p in Swarm) {
        if (!Contains(p)) {
          yield return p;
        }
      }
    }

    /// <summary>
    /// Determines if all the points in the given collection lie on the same side of the hyperplane.
    /// </summary>
    /// <param name="Swarm">The collection of points to check.</param>
    /// <returns>A tuple containing a Boolean value indicating whether all the points are on the same side of the hyperplane,
    /// and an integer value
    /// +1 if all points lie in the direction of the normal vector
    /// 0 if all points lie in the plane
    /// -1 if all points lie in the opposite direction of the normal vector
    /// int.MaxValue if bool equals to False</returns>
    public (bool atOneSide, int where) AllAtOneSide(IEnumerable<Vector> Swarm) {
      Debug.Assert(Swarm.Any(), "HyperPlane.AllAtOneSide: The 'Swarm' is empty. The method requires a non-empty collection.");

      IEnumerable<int> temp = Swarm.Select(p => Tools.CMP(Eval(p))).Where(k => k != 0);

      if (!temp.Any()) {
        return (true, 0);
      }

      int  sign        = temp.First();
      bool isAtOneSide = temp.All(k => k == sign);

      return (isAtOneSide, sign);
    }
#endregion

#region Factories
    /// <summary>
    /// Makes the hyper plane which xy parallel and goes throw point (0,0,z).
    /// </summary>
    /// <param name="z">The z coordinate of the oz axis at which the plane crosses it.</param>
    /// <returns>The hyper plane which xy parallel and goes throw point (0,0,z).</returns>
    public static HyperPlane Make3D_xyParallel(TNum z) => new(Vector.MakeOrth(3, 3), z * Vector.MakeOrth(3, 3), false);
#endregion

#region Overrides
    public override bool Equals(object? obj) {
      if (obj == null || this.GetType() != obj.GetType()) {
        return false;
      }

      HyperPlane other = (HyperPlane)obj;

      if (this.SubSpaceDim != other.SubSpaceDim) {
        return false;
      }

      return Tools.EQ(this.ConstantTerm, other.ConstantTerm) && this.Normal == other.Normal;
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> because hash code generation is not supported for hyperplanes.
    /// </summary>
    public override int GetHashCode() => throw new InvalidOperationException();
#endregion


    /// <summary>
    /// Validates the correctness of the provided hyperplane.
    /// </summary>
    /// <param name="hp">The hyperplane to be checked.</param>
    /// <exception cref="ArgumentException">Thrown if the hyperplane is incorrect.</exception>
    private static void CheckCorrectness(HyperPlane hp) {
      AffineBasis.CheckCorrectness(hp.AffBasis);

      if (!Tools.EQ(hp.Normal.Length, Tools.One)) {
        throw new ArgumentException("HyperPlane.CheckCorrectness: Normal is not a unit vector.");
      }
      foreach (Vector bvec in hp.AffBasis.LinBasis) {
        if (Tools.NE(hp.Normal * bvec)) {
          throw new ArgumentException("HyperPlane.CheckCorrectness: Normal is not orthogonal to the affine basis of hyperplane!");
        }
      }
    }

  }

}
