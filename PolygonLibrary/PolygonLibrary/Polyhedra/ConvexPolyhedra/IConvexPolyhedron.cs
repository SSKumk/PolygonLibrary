using System;
using System.Collections.Generic;
using PolygonLibrary.Basics;

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
public interface IConvexPolyhedron {

  /// <summary>
  /// Gets the dimension of the polyhedron.
  /// </summary>
  int Dim { get; }

  /// <summary>
  /// Gets the type of the convex polyhedron.
  /// <para><b>Simplex</b> - the polyhedron is a simplex</para>
  /// <b>NonSimplex</b> - the polyhedron is a complex structure
  /// <para><b>TwoDimensional</b> - the polyhedron is a 2D-plane polygon</para>
  /// </summary>
  ConvexPolyhedronType Type { get; }

  /// <summary>
  /// Gets the set of vertices of the polyhedron.
  /// </summary>
  HashSet<Point> Vertices { get; }

  /// <summary>
  /// Gets the set of (d-1)-dimensional faces of the polyhedron.
  /// </summary>
  HashSet<IConvexPolyhedron> Faces { get; }

  /// <summary>
  /// Gets the set of (d-2)-dimensional edges of the polyhedron.
  /// </summary>
  HashSet<IConvexPolyhedron> Edges { get; }

  /// <summary>
  /// Gets the dictionary, which key is (d-2)-dimensional edge and the value is a pair of incident (d-1)-dimensional faces.
  /// The second face can be equal to null if it is not constructed yet. 
  /// </summary>
  Dictionary<IConvexPolyhedron, (IConvexPolyhedron F1, IConvexPolyhedron F2)> FaceIncidence { get; }


  /// <summary>
  /// Gets a dictionary where the key is a d-dimensional point and the value is a set of faces that are incident to this point.
  /// </summary>
  Dictionary<Point, HashSet<IConvexPolyhedron>> Fans { get; }

  /// <summary>
  /// Determines whether the specified object is equal to convex polyhedron.
  /// </summary>
  /// <param name="obj">The object to compare with convex polyhedron.</param>
  /// <returns>True if the specified object is equal to convex polyhedron, False otherwise</returns>
  public bool Equals(object? obj);

  /// <summary>
  /// Returns a hash code for the convex polyhedron based on specified set of vertices and dimension.
  /// </summary>
  /// <param name="Vertices">The set of vertices to calculate a hash code for.</param>
  /// <param name="Dim">The dimension to calculate a hash code for.</param>
  /// <returns>A hash code for the specified set of vertices and dimension.</returns>
  public static int GetHashCode(HashSet<Point> Vertices, int Dim) {
    int hash = 0;
  
    foreach (Point vertex in Vertices) {
      hash = HashCode.Combine(hash, vertex.GetHashCode());
    }
    return HashCode.Combine(hash, Dim);
  }
  

}
