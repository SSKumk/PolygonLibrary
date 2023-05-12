using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PolygonLibrary.Toolkit;

namespace PolygonLibrary.Basics;

/// <summary>
/// Represents a (d-1)-hyperplane in a d-euclidean space.
/// </summary>
public class HyperPlane {

  /// <summary>
  /// The point through which the hyperplane passes.
  /// </summary>
  public readonly Point Origin;

  /// <summary>
  /// The dimension of the hyperplane.
  /// </summary>
  public int Dim => Origin.Dim - 1;

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

          _affineBasis = new AffineBasis(Origin, lBasis[1..]);
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
          int spaceDim = Origin.Dim;

          for (int i = 1; i <= spaceDim; i++) {
            Debug.Assert(_affineBasis != null, nameof(_affineBasis) + " != null");

            Vector orth = Vector.CreateOrth(spaceDim, i);
            Vector n    = Vector.OrthonormalizeAgainstBasis(orth, _affineBasis.Basis);

            if (!n.IsZero) {
              _normal = n;

              return _normal;
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
  }

  /// <summary>
  /// Constructs a hyperplane from a given affine basis.
  /// </summary>
  /// <param name="affineBasis">The affine basis that defines the hyperplane.</param>
  public HyperPlane(AffineBasis affineBasis) {
    Origin       = affineBasis.Origin;
    _affineBasis = affineBasis;
  }

  /// <summary>
  /// Copy constructor that creates a copy of the given hyperplane.
  /// </summary>
  /// <param name="hp">The hyperplane to be copied.</param>
  public HyperPlane(HyperPlane hp) {
    Origin = hp.Origin;

    if (hp._affineBasis is not null) {
      _affineBasis = hp._affineBasis;
    }

    if (hp._normal is not null) {
      _normal = hp._normal;
    }

    if (hp._constantTerm is not null) {
      _constantTerm = hp._constantTerm;
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
    
    bool isAtOneSide  = true;
    int  sign = Tools.CMP(Eval(Swarm.First()));
    
    foreach (Point p in Swarm) {
      if (Tools.CMP(Eval(p)) != sign) {
        isAtOneSide  = false;
        sign = int.MaxValue;
        break;
      }
    }
    
    return (isAtOneSide, sign);
  }

}
