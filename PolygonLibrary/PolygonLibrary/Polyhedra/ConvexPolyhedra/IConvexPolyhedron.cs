using System;
using System.Collections.Generic;
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
  /// Two polyhedra are equal if they have the same dimensions and the sets of their vertices are equal. 
  /// </summary>
  /// <param name="polyhedron">The convex polyhedron to compare</param>
  /// <param name="obj">The object to compare with convex polyhedron.</param>
  /// <returns>True if the specified object is equal to convex polyhedron, False otherwise</returns>
  public static bool Equals(IConvexPolyhedron polyhedron, object? obj) {
    if (obj == null || polyhedron.GetType() != obj.GetType()) {
      return false;
    }

    IConvexPolyhedron other = (IConvexPolyhedron)obj;

    if (polyhedron.Dim != other.Dim) {
      return false;
    }

    return polyhedron.Vertices.SetEquals(other.Vertices);
  }

  /// <summary> todo Надо его хранить!
  /// Returns a hash code for the convex polyhedron based on specified set of vertices and dimension.
  /// </summary>
  /// <param name="Vertices">The set of vertices to calculate a hash code for.</param>
  /// <param name="Dim">The dimension to calculate a hash code for.</param>
  /// <returns>A hash code for the specified set of vertices and dimension.</returns>
  public static int GetHashCode(HashSet<Point> Vertices, int Dim) {
    int hash = 0;

    foreach (Point vertex in Vertices.OrderBy(v => v)) {
      hash = HashCode.Combine(hash, vertex.GetHashCode());
    }

    return HashCode.Combine(hash, Dim);
  }

  /// <summary>
  /// Aux procedure.
  /// </summary>
  /// <param name="Faces">Faces of the convex polyhedron to extract edges from.</param>
  /// <returns>A set of edges of given faces.</returns>
  protected static HashSet<IConvexPolyhedron> ConstructEdges(HashSet<IConvexPolyhedron> Faces) {
    HashSet<IConvexPolyhedron> Edges = new HashSet<IConvexPolyhedron>();

    foreach (IConvexPolyhedron face in Faces) {
      Edges.UnionWith(face.Faces);
    }

    return Edges;
  }

  /// <summary>
  /// Aux procedure.
  /// </summary>
  /// <param name="Faces">Faces of the convex polyhedron</param>
  /// <param name="Edges">Edges of the convex polyhedron</param>
  /// <returns>A dictionary which key is a edge, and a pair (F1, F2) two faces incidence to this key. </returns>
  protected static Dictionary<IConvexPolyhedron, (IConvexPolyhedron F1, IConvexPolyhedron F2)> ConstructFaceIncidence(
    HashSet<IConvexPolyhedron> Faces
  , HashSet<IConvexPolyhedron> Edges) {
    Dictionary<IConvexPolyhedron, (IConvexPolyhedron, IConvexPolyhedron)> faceIncidence =
      new Dictionary<IConvexPolyhedron, (IConvexPolyhedron, IConvexPolyhedron)>();

    foreach (IConvexPolyhedron edge in Edges) {
      IConvexPolyhedron[] x = Faces.Where(F => F.Faces.Contains(edge)).ToArray();

      faceIncidence.Add(edge, (x[0], x[1]));
    }

    return faceIncidence;
  }

  /// <summary>
  /// Aux procedure.
  /// </summary>
  /// <param name="Vertices">Vertices of the convex polyhedron.</param>
  /// <param name="Faces">Faces of the convex polyhedron.</param>
  /// <returns>A dictionary which key is a vertex and a value is a set of faces that contains this key</returns>
  protected static Dictionary<Point, HashSet<IConvexPolyhedron>> ConstructFans(HashSet<Point> Vertices, HashSet<IConvexPolyhedron> Faces) {
    Dictionary<Point, HashSet<IConvexPolyhedron>> fans = new Dictionary<Point, HashSet<IConvexPolyhedron>>();

    foreach (Point vertex in Vertices) {
      fans.Add(vertex, new HashSet<IConvexPolyhedron>(Faces.Where(F => F.Vertices.Contains(vertex))));
    }

    return fans;
  }

}
