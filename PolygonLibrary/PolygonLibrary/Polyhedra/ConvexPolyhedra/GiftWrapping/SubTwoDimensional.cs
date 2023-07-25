using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra.GiftWrapping;

public class SubTwoDimensional : BaseSubCP {

  public override int                 PolyhedronDim => 2;
  public override SubCPType           Type          => SubCPType.TwoDimensional;
  public override HashSet<SubPoint>   Vertices      { get; }
  public          List<SubPoint>      VerticesList  { get; }
  public override HashSet<BaseSubCP>? Faces         { get; }
  public override SubIncidenceInfo?      FaceIncidence => null;

  public override BaseSubCP ToPreviousSpace() {
    List<SubPoint> Vs = new List<SubPoint>(VerticesList.Select(v => v.Parent)!);

    return new SubTwoDimensional(Vs);
  }

  public override BaseSubCP ProjectTo(AffineBasis aBasis) {
    return new SubTwoDimensional(VerticesList.Select(s => new SubPoint(s.ProjectTo(aBasis), s, s.Original)).ToList());
  }

  public SubTwoDimensional(List<SubPoint> Vs) {
    Debug.Assert(Vs.Count > 2, $"At least three points must be to construct TwoDimensional! Found {Vs.Count}");


    List<SubPoint> vertices = new List<SubPoint>(Vs);
    Vertices     = new HashSet<SubPoint>(vertices);
    VerticesList = vertices;

    HashSet<BaseSubCP> faces = new HashSet<BaseSubCP>() { new SubTwoDimensionalEdge(Vs[^1], Vs[0]) };

    for (int i = 0; i < Vs.Count - 1; i++) {
      faces.Add(new SubTwoDimensionalEdge(Vs[i], Vs[i + 1]));
    }
    Faces = faces;
  }

}
