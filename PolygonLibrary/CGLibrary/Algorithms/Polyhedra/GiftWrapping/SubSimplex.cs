namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// The polytope that is a simplex in d-dimensional space (3 and higher dimension).
  /// The faces of the simplex are calculated on demand.
  /// </summary>
  internal sealed class SubSimplex : BaseSubCP {

    /// <summary>
    /// Gets the dimension of the simplex.
    /// </summary>
    public override int PolytopDim { get; }

    /// <summary>
    /// Gets the type of the simplex. It is SubSimplex.
    /// </summary>
    public override SubCPType Type => SubCPType.SubSimplex;

    /// <summary>
    /// Gets the set of vertices of the simplex.
    /// </summary>
    public override SortedSet<SubPoint> Vertices { get; }

    /// <summary>
    /// Gets the set of (d-1)-dimensional faces of the simplex, it is also a simplex.
    /// If (d-1) == 2, then SubTwoDimensional polytope is formed.
    /// </summary>
    public override List<BaseSubCP>? Faces {
      get
        {
          List<BaseSubCP> Fs = new List<BaseSubCP>();
          foreach (SubPoint v in Vertices) {
            SortedSet<SubPoint> face = new SortedSet<SubPoint>(Vertices);
            face.Remove(v);

            if (PolytopDim == 0) {
              return null;
            }
            Fs.Add(new SubSimplex(face));
          }

          return Fs;
        }
    }

    public override BaseSubCP ToPreviousSpace() => new SubSimplex(Vertices.Select(s => s.Parent)!);

    public override BaseSubCP ProjectTo(AffineBasis affBasis) => new SubSimplex(Vertices.Select(s => s.ProjectTo(affBasis)));

    /// <summary>
    /// Construct a new instance of the <see cref="SubSimplex"/> class based on its faces.
    /// </summary>
    /// <param name="Vs">Vertices of this simplex.</param>
    public SubSimplex(IEnumerable<SubPoint> Vs) {
      PolytopDim = Vs.Count() - 1;
      Vertices   = Vs.ToSortedSet();
    }

  }

}
