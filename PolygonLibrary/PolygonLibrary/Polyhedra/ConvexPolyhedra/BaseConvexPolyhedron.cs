using System;
using System.Collections.Generic;
using System.Diagnostics;
using PolygonLibrary.Basics;
using System.Linq;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra;

/// <summary>
/// <para><b>Simplex</b> - the polyhedron is a simplex</para>
/// <b>NonSimplex</b> - the polyhedron is a complex structure
/// <para><b>TwoDimensional</b> - the polyhedron is a 2D-plane polygon</para>
/// </summary>
public enum ConvexPolyhedronType { Simplex, NonSimplex, TwoDimensional }

/// <summary>
/// Represents the d-dimensional convex polyhedron
/// </summary>
public abstract class BaseConvexPolyhedron {
  /// <summary>
  /// Gets the dimension of the space in which the polyhedron is treated.
  /// </summary>
  public int SpaceDim => Vertices.First().Dim;
  
  /// <summary>
  /// Gets the dimension of the polyhedron.
  /// </summary>
  public abstract int PolyhedronDim { get; }

  /// <summary>
  /// Gets the type of the convex polyhedron.
  /// <para><b>Simplex</b> - the polyhedron is a simplex</para>
  /// <b>NonSimplex</b> - the polyhedron is a complex structure
  /// <para><b>TwoDimensional</b> - the polyhedron is a 2D-plane polygon</para>
  /// </summary>
  public abstract ConvexPolyhedronType Type { get; }

  /// <summary>
  /// Gets the set of vertices of the polyhedron.
  /// </summary>
  public abstract HashSet<Point> Vertices { get; }
  
  
  /// <summary>
  /// Determines whether the specified object is equal to convex polyhedron.
  /// Two polyhedra are equal if they have same dimensions and sets of their vertices are equal. 
  /// </summary>
  /// <param name="obj">The object to compare with convex polyhedron.</param>
  /// <returns>True if the specified object is equal to convex polyhedron, False otherwise</returns>
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
  /// Internal field for the hash of the polyhedron
  /// </summary>
  private int? _hash = null;

  /// <summary>
  /// Returns a hash code for the convex polyhedron based on specified set of vertices and dimension.
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
