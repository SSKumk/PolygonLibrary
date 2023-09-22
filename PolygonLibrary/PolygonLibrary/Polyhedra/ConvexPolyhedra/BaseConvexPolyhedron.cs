using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace CGLibrary;

public partial class Geometry<TNum, TConv> where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// <para><b>Simplex</b> - the polytop is a simplex</para>
  /// <b>NonSimplex</b> - the polytop is a complex structure
  /// <para><b>TwoDimensional</b> - the polytop is a 2D-plane polygon</para>
  /// </summary>
  public enum ConvexPolyhedronType { Simplex, NonSimplex, TwoDimensional }

  /// <summary>
  /// Represents the d-dimensional convex polytop
  /// </summary>
  public abstract class BaseConvexPolyhedron {

    /// <summary>
    /// Gets the dimension of the space in which the polytop is treated.
    /// </summary>
    public int SpaceDim => Vertices.First().Dim;

    /// <summary>
    /// Gets the dimension of the polytop.
    /// </summary>
    public abstract int PolyhedronDim { get; }

    /// <summary>
    /// Gets the type of the convex polytop.
    /// <para><b>Simplex</b> - the polytop is a simplex</para>
    /// <b>NonSimplex</b> - the polytop is a complex structure
    /// <para><b>TwoDimensional</b> - the polytop is a 2D-plane polygon</para>
    /// </summary>
    public abstract ConvexPolyhedronType Type { get; }

    /// <summary>
    /// Gets the set of vertices of the polytop.
    /// </summary>
    public abstract HashSet<Point> Vertices { get; }


    /// <summary>
    /// Determines whether the specified object is equal to convex polytop.
    /// Two polyhedra are equal if they have same dimensions and sets of their vertices are equal.
    /// </summary>
    /// <param name="obj">The object to compare with convex polytop.</param>
    /// <returns>True if the specified object is equal to convex polytop, False otherwise</returns>
    public override bool Equals(object? obj) {
      if (obj == null || this.GetType() != obj.GetType()) {
        return false;
      }

      BaseConvexPolyhedron other = (BaseConvexPolyhedron)obj;

      if (this.PolyhedronDim != other.PolyhedronDim) {
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

        foreach (Point vertex in Vertices.OrderBy(v => v)) {
          hash = HashCode.Combine(hash, vertex.GetHashCode());
        }

        _hash = HashCode.Combine(hash, PolyhedronDim);
      }

      return _hash.Value;
    }

  }

}
