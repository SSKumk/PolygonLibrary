namespace CGLibrary;

public partial class Geometry<TNum, TConv> where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// The polytope that is not a simplex in d-dimensional space (3 and higher dimension).
  /// </summary>
  internal sealed class SubPolytop : BaseSubCP {

    /// <summary>
    /// Gets the dimension of the polytop.
    /// </summary>
    public override int PolytopDim { get; }

    /// <summary>
    /// Gets the type of the convex polytop.
    /// </summary>
    public override SubCPType Type => SubCPType.SubPolytop;

    /// <summary>
    /// Gets the set of vertices of the polytop.
    /// </summary>
    public override SortedSet<SubPoint> Vertices { get; }

    /// <summary>
    /// Gets the set of (d-1)-dimensional faces of the polytop.
    /// </summary>
    public override List<BaseSubCP>? Faces { get; }


    // /// <summary>
    // /// Gets the dictionary, which key is (d-2)-dimensional edge and the value is a pair of incident (d-1)-dimensional faces.
    // /// The second face can be equal to null if it is not constructed yet.
    // /// </summary>
    // public virtual SubIncidenceInfo? FaceIncidence { get; }


    public override BaseSubCP ToPreviousSpace() {
      Debug.Assert(Faces is not null,$"SubPolytop.ToPreviousSpace: Faces are null");

      List<BaseSubCP> faces = new List<BaseSubCP>(Faces.Select(F => F.ToPreviousSpace()));
      return new SubPolytop(faces);
    }

    public override BaseSubCP ProjectTo(AffineBasis affBasis) {
      Debug.Assert(Faces is not null, $"SubPolytop.ProjectTo: Faces are null");

      IEnumerable<SubPoint> Vs = Vertices.Select(s => s.ProjectTo(affBasis));
      List<BaseSubCP> faces = new List<BaseSubCP>(Faces.Select(F => F.ProjectTo(affBasis)));

      return new SubPolytop(faces, new SortedSet<SubPoint>(Vs));
    }

    /// <summary>
    /// Construct a new instance of the <see cref="SubPolytop"/> class based on its faces.
    /// </summary>
    /// <param name="faces">Faces to construct the convex polytope.</param>
    /// <param name="Vs">Vertices of this convex polytop. If null then its construct base on faces.</param>
    public SubPolytop(List<BaseSubCP> faces, SortedSet<SubPoint>? Vs = null) {
      PolytopDim = faces.First().PolytopDim + 1;
      Faces = faces;

      if (Vs is null) {
        Vs = new SortedSet<SubPoint>();

        foreach (BaseSubCP face in faces) {
          Vs.UnionWith(face.Vertices);
          face.SuperFaces.Add(this);
        }
      }

      Vertices = Vs;


    }

  }

}
