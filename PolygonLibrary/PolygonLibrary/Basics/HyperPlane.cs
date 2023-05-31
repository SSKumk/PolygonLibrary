using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PolygonLibrary.Toolkit;

namespace PolygonLibrary.Basics;

/// <summary>
/// Represents a (d-1)-dimensional hyperplane in a d-dimensional euclidean space.
/// </summary>
public class HyperPlane {

  /// <summary>
  /// The point through which the hyperplane passes.
  /// </summary>
  public readonly Point Origin;

  /// <summary>
  /// The dimension of the hyperplane.
  /// </summary>
  private readonly int _dim;

  /// <summary>
  /// The dimension of the hyperplane.
  /// </summary>
  public int Dim => _dim;

  /// <summary>
  /// The affine basis associated with the hyperplane.
  /// </summary>
  public AffineBasis AffineBasis {
    get
      {
        if (_affineBasis is null) {
          int         spaceDim = Origin.Dim;
          LinearBasis lBasis   = new LinearBasis();
          lBasis.AddVector(Normal);

          for (int i = 1; i <= spaceDim; i++) {
            Vector orth = Vector.CreateOrth(spaceDim, i);
            lBasis.AddVector(orth);
          }

          _affineBasis = new AffineBasis(Origin, lBasis.Basis.GetRange(1, lBasis.BasisDim - 1));
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
          Debug.Assert(_affineBasis != null, nameof(_affineBasis) + " != null");
          int spaceDim = Origin.Dim;


          for (int i = 1; i <= spaceDim; i++) {
            Vector orth = Vector.CreateOrth(spaceDim, i);
            Vector n    = Vector.OrthonormalizeAgainstBasis(orth, _affineBasis.Basis);

            if (!n.IsZero) {
              _normal = n;

              break;
            }
          }
        }

        return _normal!;
      }
  }

  private Vector? _normal = null;

  /// <summary>
  /// The constant term in the equation of the hyperplane.
  /// </summary>
  public double ConstantTerm {
    get
      {
        if (_constantTerm is null) {
          _constantTerm = -(Normal * new Vector(Origin));
        }

        return _constantTerm.Value;
      }
  }

  private double? _constantTerm = null;

  /// <summary>
  /// Constructs a hyperplane from a given point and normal vector.
  /// </summary>
  /// <param name="origin">The point through which the hyperplane passes.</param>
  /// <param name="normal">The normal vector to the hyperplane.</param>
  public HyperPlane(Point origin, Vector normal) {
    Origin  = origin;
    _normal = normal;
    _dim    = Origin.Dim - 1;
  }

  /// <summary>
  /// Constructs a hyperplane from a given affine basis.
  /// </summary>
  /// <param name="affineBasis">The affine basis that defines the hyperplane.</param>
  /// <param name="toOrient">A pair to orient the HyperPlane explicitly.
  /// If null, no orientation is defined.
  /// If not null, the point part of the pair should belong to the positive semi-space
  /// if the bool part of the pair is true, and to the negative semi-space otherwise.</param>
  public HyperPlane(AffineBasis affineBasis, (Point point, bool isPositive)? toOrient = null) {
    Debug.Assert
      (
       affineBasis.BasisDim == affineBasis.Origin.Dim - 1
     , $"Hyperplane should has (d-1) = {affineBasis.Origin.Dim - 1} independent vectors in its basis. Found {affineBasis.BasisDim}"
      );

    Origin       = affineBasis.Origin;
    _affineBasis = affineBasis;
    _dim         = Origin.Dim - 1;

    if (toOrient is not null) {
      OrientNormal(toOrient.Value.point, toOrient.Value.isPositive);
    }
  }

  /// <summary>
  /// Copy constructor that creates a copy of the given hyperplane.
  /// </summary>
  /// <param name="hp">The hyperplane to be copied.</param>
  public HyperPlane(HyperPlane hp) {
    Origin        = hp.Origin;
    _dim          = Origin.Dim - 1;
    _affineBasis  = hp._affineBasis;
    _normal       = hp._normal;
    _constantTerm = hp._constantTerm;
  }

  /// <summary>
  /// Method to orient normal of the plane.
  /// </summary>
  /// <param name="point">Point should belong to the semi-space.</param>
  /// <param name="isPositive">If the bool is <c>true</c> than point is in positive semi-space, in the negative semi-space otherwise.</param>
  public void OrientNormal(Point point, bool isPositive) {
    double res = Eval(point);

    Debug.Assert(Tools.NE(res), "For hyperplane orientation, a point is given, which belongs to the hyperplane");

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
  public double Eval(Point point) { return Normal * new Vector(point) + ConstantTerm; }

  /// <summary>
  /// Checks if the hyperplane contains a given point.
  /// </summary>
  /// <param name="point">The point to check.</param>
  /// <returns>True if the hyperplane contains the given point, otherwise False.</returns>
  public bool Contains(Point point) { return Tools.EQ(Eval(point)); }

  /// <summary>
  /// Filters a given collection of points, leaving only those that lie on the hyperplane.
  /// </summary>
  /// <param name="Swarm">The collection of points to filter.</param>
  /// <returns>A collection of points that lie on the hyperplane.</returns>
  public IEnumerable<Point> FilterIn(IEnumerable<Point> Swarm) {
    foreach (Point p in Swarm) {
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
  public IEnumerable<Point> FilterNotIn(IEnumerable<Point> Swarm) {
    foreach (Point p in Swarm) {
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
  public (bool, int) AllAtOneSide(IEnumerable<Point> Swarm) {
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

}
