using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace CGLibrary;

public partial class Geometry<TNum, TConv> where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
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
  /// <para><b>Simplex</b> - the polytop is a simplex.</para>
  /// <b>NonSimplex</b> - the polytop is a complex structure.
  /// <para><b>TwoDimensional</b> - the polytop is a 2D-plane polygon.</para>
  /// <b>OneDimensional</b> - the polytop is a 1D-segment.
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
    /// <para><b>Simplex</b> - the polytop is a simplex.</para>
    /// <b>NonSimplex</b> - the polytop is a complex structure.
    /// <para><b>TwoDimensional</b> - the polytop is a 2D-plane polygon.</para>
    /// <b>OneDimensional</b> - the polytop is a 1D-segment.
    /// </summary>
    public abstract SubCPType Type { get; }

    /// <summary>
    /// Gets the set of vertices of the polytop.
    /// </summary>
    public abstract HashSet<SubPoint> Vertices { get; }

    /// <summary>
    /// Returns set of original points.
    /// </summary>
    public HashSet<Vector> OriginalVertices => Vertices.Select(v => v.GetRootVertex()).ToHashSet();// todo их бы сохранить где-нибудь ...

    /// <summary>
    /// Gets the set of (d-1)-dimensional faces of the polytop.
    /// </summary>
    public abstract HashSet<BaseSubCP>? Faces { get; }

    /// <summary>
    /// The outward normal of the (d-1)-dimensional polytop (face).
    /// </summary>
    public Vector? Normal { get; set; }


    /// <summary>
    /// Gets the dictionary, which key is (d-2)-dimensional edge and the value is a pair of incident (d-1)-dimensional faces.
    /// The second face can be equal to null if it is not constructed yet.
    /// </summary>
    public abstract SubIncidenceInfo? FaceIncidence { get; }


    /// <summary>
    /// Converts current d-dimensional polytop in d-dimensional space to d-dimensional polytop in (d+1)-dimensional space.
    /// Assumed that the corresponding parents of vertices are exist.
    /// </summary>
    /// <returns>The d-dimensional polytop in (d+1)-dimensional space.</returns>
    public abstract BaseSubCP ToPreviousSpace();


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
