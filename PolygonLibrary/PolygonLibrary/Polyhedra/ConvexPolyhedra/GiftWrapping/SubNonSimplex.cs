using System.Collections.Generic;
using System.Linq;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra.GiftWrapping;

/// <summary>
/// Non simplex 3 and higher dimension
/// </summary>
public class SubNonSimplex : BaseSubCP {

  /// <summary>
  /// Gets the dimension of the polyhedron.
  /// </summary>
  public override int PolyhedronDim { get; }


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
  public override SubIncidenceInfo? FaceIncidence { get; }


  private AffineBasis? _basis = null;

  /// <summary>
  /// todo ???????????????????
  /// </summary>
  public override AffineBasis? Basis { get; set; }

  /// <summary>
  /// 
  /// </summary>
  /// <returns></returns>
  public override BaseSubCP ToPreviousSpace() {
    //todo Поднимать не весь комплекс, а только d-1 и d-2
    HashSet<BaseSubCP> faces = new HashSet<BaseSubCP>(Faces.Select(F => F.ToPreviousSpace()));
    // SubIncidenceInfo      info  = new SubIncidenceInfo();

    // foreach (KeyValuePair<BaseSubCP, (BaseSubCP F1, BaseSubCP F2)> pair in FaceIncidence!) {
    //   info.Add(pair.Key.ToPreviousSpace(), (pair.Value.F1.ToPreviousSpace(), pair.Value.F2.ToPreviousSpace()));
    // }

    return new SubNonSimplex(faces, FaceIncidence!);
  }

  /// <summary>
  /// Construct a new instance of the <see cref="SubNonSimplex"/> class based on it's faces. 
  /// </summary>
  /// <param name="faces">Faces to construct the convex polyhedron</param>
  /// <param name="incidence"></param>
  /// <param name="Vs">Vertices of this convex polyhedron. If null then its construct base on faces.</param>
  public SubNonSimplex(HashSet<BaseSubCP> faces, SubIncidenceInfo incidence, HashSet<SubPoint>? Vs = null) {
    PolyhedronDim = faces.First().PolyhedronDim + 1;
    Type          = SubCPType.NonSimplex;
    Faces         = faces;

    SubIncidenceInfo faceIncidence = new SubIncidenceInfo();

    foreach (KeyValuePair<BaseSubCP, (BaseSubCP F1, BaseSubCP F2)> pair in incidence) {
      faceIncidence.Add(pair.Key, (pair.Value.F1, pair.Value.F2)!);
    }

    if (Vs is null) {
      Vs = new HashSet<SubPoint>();

      foreach (BaseSubCP face in faces) {
        Vs.UnionWith(face.Vertices);
      }
    }

    FaceIncidence = faceIncidence;
    Vertices      = Vs;
  }

}
