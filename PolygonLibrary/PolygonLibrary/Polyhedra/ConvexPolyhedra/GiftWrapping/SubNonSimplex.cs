using System.Collections.Generic;
using System.Linq;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra.GiftWrapping;

public class SubNonSimplex : BaseSubCP {

  /// <summary>
  /// Gets the dimension of the polyhedron.
  /// </summary>
  public override int Dim { get; }


  /// <summary>
  /// Gets the type of the convex polyhedron.
  /// </summary>
  public override SubCPType Type { get; }

  /// <summary>
  /// Gets the set of vertices of the polyhedron.
  /// </summary>
  public override HashSet<SubPoint> Vertices { get; }

  /// <summary>
  /// Gets the set of (d-1)-dimensional faces of the polyhedron.
  /// </summary>
  public override HashSet<BaseSubCP> Faces { get; }


  /// <summary>
  /// Gets the dictionary, which key is (d-2)-dimensional edge and the value is a pair of incident (d-1)-dimensional faces.
  /// The second face can be equal to null if it is not constructed yet. 
  /// </summary>
  public override IncidenceInfo? FaceIncidence { get; }


  private AffineBasis? _basis = null;

  /// <summary>
  /// todo ???????????????????
  /// </summary>
  public override AffineBasis? Basis { get; set; }

  /// <summary>
  /// Construct a new instance of the <see cref="SubNonSimplex"/> class based on it's faces. 
  /// </summary>
  /// <param name="faces">Faces to construct the convex polyhedron</param>
  /// <param name="tempIncidence">Information about a face incidence</param>
  public SubNonSimplex(HashSet<BaseSubCP> faces, TempIncidenceInfo tempIncidence) {
    Dim   = faces.First().Dim + 1;
    Type  = SubCPType.NonSimplex;
    Faces = faces;

    IncidenceInfo?     faceIncidence = new IncidenceInfo();
    HashSet<SubPoint> tempVertices  = new HashSet<SubPoint>();
    
    foreach (KeyValuePair<BaseSubCP, (BaseSubCP F1, BaseSubCP? F2)> pair in tempIncidence) {
      faceIncidence.Add(pair.Key, (pair.Value.F1, pair.Value.F2)!);
    }

    foreach (BaseSubCP face in faces) {
      tempVertices.UnionWith(face.Vertices);
    }
    
    FaceIncidence = faceIncidence;
    Vertices      = tempVertices;
  }

}
