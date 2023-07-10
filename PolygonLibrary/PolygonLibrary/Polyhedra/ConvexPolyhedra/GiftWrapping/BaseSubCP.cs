using System;
using System.Collections.Generic;
using System.Diagnostics;
using PolygonLibrary.Basics;
using System.Linq;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra.GiftWrapping;

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
/// <para><b>Simplex</b> - the polyhedron is a simplex</para>
/// <b>NonSimplex</b> - the polyhedron is a complex structure
/// <para><b>TwoDimensional</b> - the polyhedron is a 2D-plane polygon</para>
/// </summary>
public enum SubCPType { Simplex, NonSimplex, TwoDimensional, OneDimensional }

/// <summary>
/// Represents the d-dimensional convex polyhedron
/// </summary>
public abstract class BaseSubCP {

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
  public abstract SubCPType Type { get; }

  /// <summary>
  /// Gets the set of vertices of the polyhedron.
  /// </summary>
  public abstract HashSet<SubPoint> Vertices { get; }

  /// <summary>
  /// Returns set of original points.
  /// </summary>
  public HashSet<Point> OriginalVertices => new HashSet<Point>(Vertices.Select(v => v.Original));

  /// <summary>
  /// Gets the set of (d-1)-dimensional faces of the polyhedron.
  /// </summary>
  public abstract HashSet<BaseSubCP>? Faces { get; }


  /// <summary>
  /// Gets the dictionary, which key is (d-2)-dimensional edge and the value is a pair of incident (d-1)-dimensional faces.
  /// The second face can be equal to null if it is not constructed yet. 
  /// </summary>
  public abstract SubIncidenceInfo? FaceIncidence { get; }
  
  /// <summary>
  /// todo БАЗИС ???? ГРАНИ ????
  /// </summary>
  public abstract AffineBasis? Basis { get; set; }


  /// <summary>
  /// Converts current d-dimensional polyhedron in d-dimensional space to d-dimensional polyhedron in (d+1)-dimensional space.
  /// Assumed that the corresponding parents of vertices are exist. 
  /// </summary>
  /// <returns>The d-dimensional polyhedron in (d+1)-dimensional space.</returns>
  public abstract BaseSubCP ToPreviousSpace();

  // /// <summary>
  // /// Converts current k-dimensional polyhedron in d-dimensional space (k less d) to k-dimensional polyhedron in (d-1)-dimensional space.
  // /// </summary>
  // /// <param name="basis">The affine basis of (d-1)-dimensional space.</param>
  // /// <returns>The k-dimensional polyhedron in (d-1)-dimensional space.</returns>
  // public BaseSubCP ProjectToSpace(AffineBasis basis) {
  //   Vertices.Select()
  // }


  /// <summary>
  /// Determines whether the specified object is equal to convex polyhedron.
  /// Two polyhedra are equal if they have the same dimensions and the sets of their vertices are equal. 
  /// </summary>
  /// <param name="obj">The object to compare with convex polyhedron.</param>
  /// <returns>True if the specified object is equal to convex polyhedron, False otherwise</returns>
  public override bool Equals(object? obj) {
    if (obj == null || this.GetType() != obj.GetType()) {
      return false;
    }

    BaseSubCP other = (BaseSubCP)obj;

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

      foreach (SubPoint vertex in Vertices.OrderBy(v => v)) {
        hash = HashCode.Combine(hash, vertex.GetHashCode());
      }

      _hash = HashCode.Combine(hash, PolyhedronDim);
    }

    return _hash.Value;
  }

}
