using System.Collections.Generic;


namespace CGLibrary.Polyhedra.ConvexPolyhedra.GiftWrapping;

public class SubTwoDimensionalEdge : BaseSubCP {

  public override int                 PolyhedronDim => 1;
  public override SubCPType           Type          => SubCPType.OneDimensional;
  public override HashSet<SubPoint>   Vertices      { get; }
  private         SubPoint            first         { get; }
  private         SubPoint            second        { get; }
  public override HashSet<BaseSubCP>? Faces         => null;
  public override SubIncidenceInfo?   FaceIncidence => null;

  public override BaseSubCP ToPreviousSpace() => new SubTwoDimensionalEdge(first.Parent!, second.Parent!);

  public override BaseSubCP ProjectTo(AffineBasis aBasis) => new SubTwoDimensionalEdge
    (new SubPoint(first.ProjectTo(aBasis), first, first.Original), new SubPoint(second.ProjectTo(aBasis), second, second.Original));

  public SubTwoDimensionalEdge(SubPoint first, SubPoint second) {
    this.first  = first;
    this.second = second;
    HashSet<SubPoint> vertices = new HashSet<SubPoint>() { first, second };
    Vertices = vertices;
  }

}
