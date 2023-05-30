using System;
using System.Collections.Generic;
using System.Diagnostics;
using PolygonLibrary.Basics;
using System.Linq;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra;

/// <summary>
/// Type of temporary storage of face incidence information
/// </summary>
public class TempIncidenceInfo : Dictionary<BaseConvexPolyhedron, (BaseConvexPolyhedron F1, BaseConvexPolyhedron? F2)> { }

/// <summary>
/// Type of permanent storage of face incidence information.
/// For each pair (F1, F2) of incident faces, it is assumed that HashCode(F1) is less or equal than HashCode(F2)
/// </summary>
public class IncidenceInfo : Dictionary<BaseConvexPolyhedron, (BaseConvexPolyhedron F1, BaseConvexPolyhedron F2)> { }

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
  /// Gets the dimension of the polyhedron.
  /// </summary>
  public abstract int Dim { get; }

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
  /// Gets the set of (d-1)-dimensional faces of the polyhedron.
  /// </summary>
  public abstract HashSet<BaseConvexPolyhedron> Faces { get; }

  /// <summary>
  /// The set of (d-2)-dimensional edges of the polyhedron.
  /// </summary>
  protected HashSet<BaseConvexPolyhedron>? _edges = null;

  /// <summary>
  /// Gets the set of (d-2)-dimensional edges of the polyhedron.
  /// </summary>
  public HashSet<BaseConvexPolyhedron> Edges {
    get
      {
        if (_edges is null) {
          ConstructEdges();
        }

        return _edges!;
      }
  }

  /// <summary>
  /// The dictionary, which key is (d-2)-dimensional edge and the value is a pair of incident (d-1)-dimensional faces.
  /// </summary>
  protected IncidenceInfo? _faceIncidence = null;

  /// <summary>
  /// Gets the dictionary, which key is (d-2)-dimensional edge and the value is a pair of incident (d-1)-dimensional faces.
  /// The second face can be equal to null if it is not constructed yet. 
  /// </summary>
  public IncidenceInfo FaceIncidence {
    get
      {
        if (_faceIncidence is null) {
          ConstructFaceIncidence();
        }

        return _faceIncidence!;
      }
  }


  /// <summary>
  /// The dictionary where the key is a d-dimensional point and the value is a set of faces that are incident to this point.
  /// </summary>
  protected Dictionary<Point, HashSet<BaseConvexPolyhedron>>? _fans = null;

  /// <summary>
  /// Gets a dictionary where the key is a d-dimensional point and the value is a set of faces that are incident to this point.
  /// </summary>
  public Dictionary<Point, HashSet<BaseConvexPolyhedron>> Fans {
    get
      {
        if (_fans is null) {
          ConstructFans();
        }

        return _fans!;
      }
  }

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

    BaseConvexPolyhedron other = (BaseConvexPolyhedron)obj;

    if (this.Dim != other.Dim) {
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

      _hash = HashCode.Combine(hash, Dim);
    }

    return _hash.Value;
  }

  /// <summary>
  /// Aux procedure for constructing edges. Also, if the faceIncidence do not establish yet, the procedure does it.
  /// </summary>
  /// <returns>A set of edges of given faces.</returns>
  protected void ConstructEdges() {
    _edges = new HashSet<BaseConvexPolyhedron>();

    TempIncidenceInfo? tempIncidence = _faceIncidence is null ? new TempIncidenceInfo() : null;

    foreach (BaseConvexPolyhedron face in Faces) {
      _edges.UnionWith(face.Faces);

      if (tempIncidence is not null) {
        foreach (BaseConvexPolyhedron edge in face.Faces) {
          if (tempIncidence.ContainsKey(edge)) {
            tempIncidence[edge] = (tempIncidence[edge].F1, face);
          } else {
            tempIncidence.Add(edge, (face, null));
          }
        }
      }
    }

    if (_faceIncidence is null) {
      Debug.Assert(tempIncidence != null, nameof(tempIncidence) + " != null");

      _faceIncidence = new IncidenceInfo();

      foreach (KeyValuePair<BaseConvexPolyhedron, (BaseConvexPolyhedron F1, BaseConvexPolyhedron? F2)> pair in tempIncidence) {
        _faceIncidence.Add(pair.Key, (pair.Value.F1, pair.Value.F2)!);
      }
    }
  }

  /// <summary>
  /// Aux procedure for constructing of information about face incidence. If the edges do not establish yet, the procedure construct its.
  /// </summary>
  /// <returns>A dictionary which key is a edge, and a pair (F1, F2) two faces incidence to this key. </returns>
  protected void ConstructFaceIncidence() {
    _faceIncidence = new IncidenceInfo();
    TempIncidenceInfo tempIncidence = new TempIncidenceInfo();

    bool isEdgesWasNull = _edges is null;

    if (isEdgesWasNull) {
      _edges = new HashSet<BaseConvexPolyhedron>();
    }

    foreach (BaseConvexPolyhedron face in Faces) {
      if (isEdgesWasNull) {
        _edges!.UnionWith(face.Faces);
      }

      foreach (BaseConvexPolyhedron edge in face.Faces) {
        if (tempIncidence.ContainsKey(edge)) {
          tempIncidence[edge] = (tempIncidence[edge].F1, face);
        } else {
          tempIncidence.Add(edge, (face, null));
        }
      }
    }

    foreach (KeyValuePair<BaseConvexPolyhedron, (BaseConvexPolyhedron F1, BaseConvexPolyhedron? F2)> pair in tempIncidence) {
      _faceIncidence.Add(pair.Key, (pair.Value.F1, pair.Value.F2)!);
    }
  }

  /// <summary>
  /// Aux procedure for constructing fans of the polyhedra.
  /// </summary>
  /// <returns>A dictionary which key is a vertex and a value is a set of faces that contains this key</returns>
  protected void ConstructFans() {
    _fans = new Dictionary<Point, HashSet<BaseConvexPolyhedron>>();

    foreach (BaseConvexPolyhedron face in Faces) {
      foreach (Point vertex in face.Vertices) {
        if (_fans.ContainsKey(vertex)) {
          _fans[vertex].Add(face);
        } else {
          _fans.Add
            (
             vertex
           , new HashSet<BaseConvexPolyhedron>()
               {
                 face
               }
            );
        }
      }
    }
  }

}
