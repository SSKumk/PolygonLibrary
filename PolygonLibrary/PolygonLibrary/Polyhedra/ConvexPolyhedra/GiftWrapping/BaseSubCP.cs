using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Type of temporary storage of face incidence information
  /// </summary>
  public class TempIncidenceInfo : Dictionary<BaseSubCP, (BaseSubCP F1, BaseSubCP? F2)> { }

  /// <summary>
  /// Type of permanent storage of face incidence information.
  /// For each pair (F1, F2) of incident faces, it is assumed that HashCode(F1) is less or equal than HashCode(F2)
  /// </summary>
  public class SubIncidenceInfo : Dictionary<BaseSubCP, (BaseSubCP F1, BaseSubCP F2)> { }

  /// <summary>
  /// <para><b>Simplex</b> - the polytop is a simplex</para>
  /// <b>NonSimplex</b> - the polytop is a complex structure
  /// <para><b>TwoDimensional</b> - the polytop is a 2D-plane polygon</para>
  /// </summary>
  public enum SubCPType {

    Simplex
  , NonSimplex
  , TwoDimensional
  , OneDimensional
  , ZeroDimensional

  }

  /// <summary>
  /// Represents the d-dimensional convex polytop
  /// </summary>
  public abstract class BaseSubCP {

    /// <summary>
    /// Gets the dimension of the space in which the polytop is treated.
    /// </summary>
    public int SpaceDim => Vertices.First().Dim;

    /// <summary>
    /// Gets the dimension of the polytop.
    /// </summary>
    public abstract int PolytopDim { get; }

    /// <summary>
    /// Gets the type of the convex polytop.
    /// <para><b>Simplex</b> - the polytop is a simplex</para>
    /// <b>NonSimplex</b> - the polytop is a complex structure
    /// <para><b>TwoDimensional</b> - the polytop is a 2D-plane polygon</para>
    /// </summary>
    public abstract SubCPType Type { get; }


    /// <summary>
    /// The set of vertices of the polytop.
    /// </summary>
    protected HashSet<SubPoint>? _vertices = null;

    /// <summary>
    /// Gets the set of vertices of the polytop.
    /// </summary>
    public virtual HashSet<SubPoint> Vertices => _vertices ??= Faces.SelectMany(F => F.Vertices).ToHashSet();

    /// <summary>
    /// Returns set of original points.
    /// </summary>
    public HashSet<Point> OriginalVertices => new HashSet<Point>(Vertices.Select(v => v.Original));

    /// <summary>
    /// The set of faces pf the polytop.
    /// </summary>
    protected HashSet<BaseSubCP>? _faces = null;

    /// <summary>
    /// Gets the set of (d-1)-dimensional faces of the polytop.
    /// </summary>
    public virtual HashSet<BaseSubCP> Faces => _faces;

    /// <summary>
    /// The outward normal of the (d-1)-dimensional polytop (face).
    /// </summary>
    public Vector? Normal { get; set; }


    /// <summary>
    /// The list represents the affine space of the polytop.
    /// </summary>
    protected List<Point>? _affine = null;


    /// <summary>
    /// Gets the list of d-dimensional points which forms the affine space (not a Affine basis) corresponding to the this polytop.
    /// </summary>
    public virtual List<Point> Affine {
      get
        {
          if (_affine is null) {
            BaseSubCP   subF   = Faces.First();
            List<Point> affine = new List<Point>(subF.Affine) { new Point(subF.InnerPoint - InnerPoint) };
            _affine = affine;

            Console.WriteLine("Affine!");
          }

          return _affine;
        }
    }

    /// <summary>
    /// Gets the d-dimensional point 'p' which lies within P and does not lie on any faces of P.
    /// </summary>
    protected Point? _innerPoint = null;

    // point(x) = 1/2*(point(y) + point(y')), where y,y' \in Faces, y != y'.
    public virtual Point InnerPoint => _innerPoint ??= new Point
                                         (
                                          (new Vector(Faces.First().InnerPoint) + new Vector(Faces.Last().InnerPoint)) / Tools.Two
                                         );


    /// <summary>
    /// Gets the dictionary, which key is (d-2)-dimensional edge and the value is a pair of incident (d-1)-dimensional faces.
    /// The second face can be equal to null if it is not constructed yet.
    /// </summary>
    public abstract SubIncidenceInfo? FaceIncidence { get; }


    // /// <summary>
    // /// Converts current d-dimensional polytop in d-dimensional space to d-dimensional polytop in (d+1)-dimensional space.
    // /// Assumed that the corresponding parents of vertices are exist.
    // /// </summary>
    // /// <returns>The d-dimensional polytop in (d+1)-dimensional space.</returns>
    // public abstract BaseSubCP ToPreviousSpace();


    public abstract BaseSubCP ProjectTo(AffineBasis aBasis);


    /// <summary>
    /// Determines whether the specified object is equal to convex polytop.
    /// Two polyhedra are equal if they have the same dimensions and the sets of their vertices are equal.
    /// </summary>
    /// <param name="obj">The object to compare with convex polytop.</param>
    /// <returns>True if the specified object is equal to convex polytop, False otherwise</returns>
    public override bool Equals(object? obj) {
      if (obj == null || this.GetType() != obj.GetType()) {
        return false;
      }

      BaseSubCP other = (BaseSubCP)obj;

      if (this.PolytopDim != other.PolytopDim) {
        return false;
      }

      return this.Vertices.SetEquals(other.Vertices);
    }

    /// <summary>
    /// Internal field for the hash of the polytop
    /// </summary>
    private int? _hash = null;

    /// <summary>
    /// Returns a hash code for the convex polytop based on specified set of vertices and dimension.
    /// </summary>
    /// <returns>A hash code for the specified set of vertices and dimension.</returns>
    public override int GetHashCode() {
      if (_hash is null) {
        int hash = 0;

        foreach (SubPoint vertex in Vertices.OrderBy(v => v)) {
          hash = HashCode.Combine(hash, vertex.GetHashCode());
        }

        _hash = HashCode.Combine(hash, PolytopDim);
      }

      return _hash.Value;
    }

  }

}
