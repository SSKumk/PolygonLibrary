using System.Collections.Generic;
using System.Linq;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra;

public class NonSimplex : BaseConvexPolyhedron {

  /// <summary>
  /// Gets the dimension of the polyhedron.
  /// </summary>
  public override int Dim { get; }


  /// <summary>
  /// Gets the type of the convex polyhedron.
  /// </summary>
  public override ConvexPolyhedronType Type { get; }

  /// <summary>
  /// Gets the set of vertices of the polyhedron.
  /// </summary>
  public override HashSet<Point> Vertices { get; }

  /// <summary>
  /// Gets the set of (d-1)-dimensional faces of the polyhedron.
  /// </summary>
  public override HashSet<BaseConvexPolyhedron> Faces { get; }

  /// <summary>
  /// Construct a new instance of the <see cref="NonSimplex"/> class based on it's faces. 
  /// </summary>
  /// <param name="faces">Faces to construct the convex polyhedron</param>
  /// <param name="Vs">The vertices of polyhedron. </param>
  /// <param name="incid">The information about neighbours-faces of edges.</param>
  /// <param name="fansInfo">The information about faces incident to this point.</param>
  public NonSimplex(HashSet<BaseConvexPolyhedron> faces
                  , HashSet<Point>?               Vs       = null
                  , IncidenceInfo?                incid    = null
                  , FansInfo?                     fansInfo = null) {
    Dim   = faces.First().Dim + 1;
    Type  = ConvexPolyhedronType.NonSimplex;
    Faces = faces;

    if (Vs is not null) {
      Vertices = new HashSet<Point>(Vs);
    } else {
      HashSet<Point> tempVertices = new HashSet<Point>();

      foreach (BaseConvexPolyhedron face in faces) {
        tempVertices.UnionWith(face.Vertices);
      }

      Vertices = tempVertices;
    }


    if (incid is not null) {
      _faceIncidence = new IncidenceInfo(incid);
    }

    if (fansInfo is not null) {
      _fans = new FansInfo(fansInfo);
    }
  }

}
