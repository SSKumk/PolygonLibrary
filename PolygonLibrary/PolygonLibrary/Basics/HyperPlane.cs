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
  /// Represents a (d-1)-dimensional hyperplane in a d-dimensional euclidean space.
  /// N * x = C, N - outward normal vector, C - constant term.
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
    public int VecDim => Origin.Dim;

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
          if (_affineBasis is null) {
            Debug.Assert(_normal is not null, "HyperPlane.AffBasis: Affine basis is null. Can't construct the Normal.");

            LinearBasis normalBasis = new LinearBasis(Normal);
            (Matrix Q, _) = QRDecomposition.ByReflection(normalBasis.Basis!); // span([1, VecDim - 1]) orthogonal to [0] = Normal

            LinearBasis lb = new LinearBasis();
            for (int i = 1; i < VecDim; i++) {
              lb.AddVectorToBasis(Q.TakeVector(i), false);
            }

            _affineBasis = new AffineBasis(Origin, lb);

            CheckCorrectness(this);
          }

          return _affineBasis;
        }
    }

    private AffineBasis? _affineBasis = null;

    /// <summary>
    /// The normal vector to the hyperplane.
    /// </summary>
    public Vector Normal {
      get
        {
          if (_normal is null) {
            Debug.Assert(_affineBasis is not null, "HyperPlane.Normal: Affine basis is null. Can't construct the Normal.");
            _normal = LinearBasis.FindOrthonormalVector(AffBasis.LinBasis);
#if DEBUG
            Debug.Assert
              (
               _normal is not null
             , "HyperPlane: Computation of the normal on the basis of the affine basis of the plane gives null vector reference!"
              );
            CheckCorrectness(this);
#endif
          }

          return _normal;
        }
    }

    private Vector? _normal = null;

    /// <summary>
    /// The constant term in the equation of the hyperplane.
    /// </summary>
    public TNum ConstantTerm {
      get
        {
          _constantTerm ??= Normal * new Vector(Origin);

          return _constantTerm.Value;
        }
    }

    private TNum? _constantTerm = null;
#endregion

#region Constructors
    /// <summary>
    /// Constructs a hyperplane from a given point and normal vector.
    /// </summary>
    /// <param name="normal">The normal vector to the hyperplane.</param>
    /// <param name="origin">The point through which the hyperplane passes.</param>
    public HyperPlane(Vector normal, Vector origin) {
      Origin      = origin;
      _normal     = normal.Normalize();
      SubSpaceDim = Origin.Dim - 1;


#if DEBUG
      CheckCorrectness(this);
#endif
    }

    /// <summary>
    /// Constructs a hyperplane from a normal vector and the constant term.
    /// </summary>
    /// <param name="normal">The normal vector to the hyperplane.</param>
    /// <param name="constant">The constant term in right part.</param>
    public HyperPlane(Vector normal, TNum constant) {
      TNum lengthN = normal.Length;
      _normal       = normal / lengthN;
      SubSpaceDim   = normal.Dim - 1;
      _constantTerm = constant / lengthN;
      Origin        = new Vector(Normal * constant);


#if DEBUG
      CheckCorrectness(this);
#endif
    }

    /// <summary>
    /// Constructs a hyperplane from a given affine basis.
    /// </summary>
    /// <param name="affineBasis">The affine basis that defines the hyperplane.</param>
    /// <param name="toOrient">A pair to orient the HyperPlane explicitly.
    /// If null, no orientation is defined.
    /// If not null, the point part of the pair should belong to the positive semi-space
    /// if the bool part of the pair is true, and to the negative semi-space otherwise.</param>
    public HyperPlane(AffineBasis affineBasis, (Vector point, bool isPositive)? toOrient = null) {
      Debug.Assert
        (
         affineBasis.SubSpaceDim == affineBasis.Origin.Dim - 1
       , $"Hyperplane should has (d-1) = {affineBasis.Origin.Dim - 1} independent vectors in its basis. Found {affineBasis.SubSpaceDim}"
        );

      Origin       = affineBasis.Origin;
      _affineBasis = affineBasis;
      SubSpaceDim  = Origin.Dim - 1;

      if (toOrient is not null) {
        OrientNormal(toOrient.Value.point, toOrient.Value.isPositive);
      }


#if DEBUG
      CheckCorrectness(this);
#endif
    }
#endregion

#region Functions
    /// <summary>
    /// Method to orient normal of the plane.
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
    /// <remarks>If there are no points in the swarm then (true, int.MaxValue) returns.</remarks>
    public (bool atOneSide, int where) AllAtOneSide(IEnumerable<Vector> Swarm) {
      if (!Swarm.Any()) {
        return (true, int.MaxValue);
      }

      IEnumerable<int> temp = Swarm.Select(p => Tools.CMP(Eval(p))).Where(k => k != 0);

      if (!temp.Any()) {
        return (true, 0);
      }

      int  sign        = temp.First();
      bool isAtOneSide = temp.All(k => k == sign);

      return (isAtOneSide, sign);
    }
#endregion

#region Fabrics
    /// <summary>
    /// Makes the hyper plane which xy parallel and goes throw point (0,0,z).
    /// </summary>
    /// <param name="z">The z coordinate of the oz axis at which the plane crosses it.</param>
    /// <returns>The hyper plane which xy parallel and goes throw point (0,0,z).</returns>
    public static HyperPlane Make3D_xyParallel(TNum z) => new(Vector.MakeOrth(3, 3), z * Vector.MakeOrth(3, 3));
#endregion

    public override int GetHashCode() => HashCode.Combine(Normal.GetHashCode(), ConstantTerm);

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
